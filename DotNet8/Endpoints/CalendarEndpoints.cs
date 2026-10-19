using Calendarium.Contracts;
using Calendarium.Data;
using Calendarium.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Calendarium.Endpoints
{
    public static class CalendarEndpoints
    {
        private const int HistoryLines = 40;

        public static RouteGroupBuilder MapCalendarEndpoints(this RouteGroupBuilder api)
        {
            api.MapGet("/calendar", GetSheet).AllowAnonymous();
            api.MapGet("/history", GetHistory);
            return api;
        }

        private static async Task<Results<Ok<CalendarSheetDto>, ValidationProblem>> GetSheet(
            int? year, int? month, HttpRequest request, CalendarService calendar, RequestHeaderLogger headerLogger,
            IHostEnvironment env, CancellationToken ct)
        {
            var today = calendar.Today();
            int y = year ?? today.Year;
            int m = month ?? today.Month;

            if (y is < 1900 or > 2999 || m is < 1 or > 12)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["month"] = ["Year must be 1900-2999 and month 1-12."],
                });
            }

            await headerLogger.LogAsync(request.Headers, ct);

            var sheet = await calendar.BuildSheetAsync(y, m, ct);

            return TypedResults.Ok(new CalendarSheetDto(
                DateOnly.FromDateTime(sheet.Today),
                y,
                m,
                DateOnly.FromDateTime(sheet.FirstDay),
                DateOnly.FromDateTime(sheet.LastDay),
                sheet.Occurrences.Select(CalEventDto.From).ToList(),
                sheet.TotalCount,
                env.IsDevelopment()));
        }

        private static async Task<Ok<List<HistoryItemDto>>> GetHistory(ApplicationDbContext db, CancellationToken ct) =>
            TypedResults.Ok(await db.RequestsHeaders
                .Where(rh => rh.Field == Models.ReqHeadFieldType.UsrAgent || rh.Field == Models.ReqHeadFieldType.Language)
                .OrderByDescending(rh => rh.Created)
                .Take(HistoryLines)
                .Select(rh => new HistoryItemDto(rh.Created, rh.Field, rh.Text))
                .ToListAsync(ct));
    }
}
