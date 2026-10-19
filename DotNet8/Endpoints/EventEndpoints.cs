using System.Text.Json;
using System.Text.Json.Serialization;
using Calendarium.Contracts;
using Calendarium.Data;
using Calendarium.Models;
using Calendarium.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Calendarium.Endpoints
{
    public static class EventEndpoints
    {
        private const long MaxImportBytes = 2 * 1024 * 1024;

        // Backup file format: PascalCase entity properties, as produced by the old Home/GetJson.
        private static readonly JsonSerializerOptions _backupOptions = new()
        {
            PropertyNamingPolicy = null,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        public static RouteGroupBuilder MapEventEndpoints(this RouteGroupBuilder api)
        {
            var events = api.MapGroup("/events");

            events.MapGet("/", GetAll);
            events.MapGet("/export", Export);
            events.MapPost("/import", Import).DisableAntiforgery();
            events.MapGet("/{id:int}", GetById);
            events.MapPost("/", Create);
            events.MapPut("/{id:int}", Update);
            events.MapDelete("/{id:int}", Delete);

            return api;
        }

        private static async Task<Ok<List<CalEventDto>>> GetAll(ApplicationDbContext db, CancellationToken ct)
        {
            var events = await db.CalEvents.OrderBy(x => x.Month).ThenBy(x => x.Day).ToListAsync(ct);
            return TypedResults.Ok(events.Select(CalEventDto.From).ToList());
        }

        private static async Task<Results<Ok<CalEventDto>, NotFound>> GetById(int id, ApplicationDbContext db, CancellationToken ct) =>
            await db.CalEvents.FindAsync([id], ct) is { } evt
                ? TypedResults.Ok(CalEventDto.From(evt))
                : TypedResults.NotFound();

        private static async Task<Created<CalEventDto>> Create(CalEventInput input, ApplicationDbContext db, CancellationToken ct)
        {
            var evt = new CalEvent
            {
                Created = DateTime.Now,
                Description = input.Description,
                Modified = DateTime.Now,
                Status = CalEventStatus.Active,
                Time = input.Time,
            };
            CalendarService.ApplySchedule(evt, input.Started.ToDateTime(TimeOnly.MinValue), input.Repeat, input.EveryXDays);

            db.CalEvents.Add(evt);
            await db.SaveChangesAsync(ct);

            return TypedResults.Created($"/api/events/{evt.Id}", CalEventDto.From(evt));
        }

        private static async Task<Results<Ok<CalEventDto>, NotFound>> Update(int id, CalEventInput input, ApplicationDbContext db, CancellationToken ct)
        {
            var evt = await db.CalEvents.FindAsync([id], ct);
            if (evt is null) return TypedResults.NotFound();

            evt.Description = input.Description;
            evt.Modified = DateTime.Now;
            evt.Status = input.Status;
            evt.Time = input.Time;
            CalendarService.ApplySchedule(evt, input.Started.ToDateTime(TimeOnly.MinValue), input.Repeat, input.EveryXDays);

            await db.SaveChangesAsync(ct);
            return TypedResults.Ok(CalEventDto.From(evt));
        }

        private static async Task<Results<NoContent, NotFound>> Delete(int id, ApplicationDbContext db, CancellationToken ct)
        {
            int deleted = await db.CalEvents.Where(e => e.Id == id).ExecuteDeleteAsync(ct);
            return deleted > 0 ? TypedResults.NoContent() : TypedResults.NotFound();
        }

        private static async Task<FileContentHttpResult> Export(ApplicationDbContext db, CancellationToken ct)
        {
            var events = await db.CalEvents.AsNoTracking().ToArrayAsync(ct);
            var bytes = JsonSerializer.SerializeToUtf8Bytes(events, new JsonSerializerOptions { PropertyNamingPolicy = null });
            return TypedResults.File(bytes, "application/json", "calendBkp.json");
        }

        private static async Task<Results<Ok<int>, NotFound, ValidationProblem>> Import(
            IFormFile file, ApplicationDbContext db, IHostEnvironment env, CancellationToken ct)
        {
            if (!env.IsDevelopment()) return TypedResults.NotFound();

            if (file.Length > MaxImportBytes)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]> { ["file"] = ["File must be under 2 MB."] });
            }

            List<CalEvent>? imported;
            try
            {
                await using var stream = file.OpenReadStream();
                imported = await JsonSerializer.DeserializeAsync<List<CalEvent>>(stream, _backupOptions, ct);
            }
            catch (JsonException ex)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]> { ["file"] = [$"Invalid backup file: {ex.Message}"] });
            }

            int count = 0;
            foreach (var src in imported ?? [])
            {
                if (src is null) continue;

                var evt = new CalEvent
                {
                    Created = DateTime.Now,
                    Description = src.Description,
                    Modified = DateTime.Now,
                    Status = CalEventStatus.Active,
                    Time = src.Time,
                };
                CalendarService.ApplySchedule(evt, src.Started, src.Repeat, src.EveryXDays);
                db.CalEvents.Add(evt);
                count++;
            }
            await db.SaveChangesAsync(ct);

            return TypedResults.Ok(count);
        }
    }
}
