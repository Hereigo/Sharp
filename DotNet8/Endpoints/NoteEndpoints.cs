using Calendarium.Contracts;
using Calendarium.Data;
using Calendarium.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Calendarium.Endpoints
{
    public static class NoteEndpoints
    {
        public static RouteGroupBuilder MapNoteEndpoints(this RouteGroupBuilder api)
        {
            var notes = api.MapGroup("/notes");

            notes.MapGet("/", GetAll);
            notes.MapGet("/{id:int}", GetById);
            notes.MapPost("/", Create);
            notes.MapPut("/{id:int}", Update);
            notes.MapDelete("/{id:int}", Delete);

            return api;
        }

        private static async Task<Ok<List<NoteDto>>> GetAll(ApplicationDbContext db, CancellationToken ct) =>
            TypedResults.Ok(await db.Notes
                .OrderBy(n => n.SortNum).ThenBy(n => n.Id)
                .Select(n => new NoteDto(n.Id, n.SortNum, n.Text, n.Status))
                .ToListAsync(ct));

        private static async Task<Results<Ok<NoteDto>, NotFound>> GetById(int id, ApplicationDbContext db, CancellationToken ct) =>
            await db.Notes.FindAsync([id], ct) is { } note
                ? TypedResults.Ok(NoteDto.From(note))
                : TypedResults.NotFound();

        private static async Task<Created<NoteDto>> Create(NoteInput input, ApplicationDbContext db, CancellationToken ct)
        {
            var note = new Note { SortNum = input.SortNum, Text = input.Text, Status = input.Status };
            db.Notes.Add(note);
            await db.SaveChangesAsync(ct);
            return TypedResults.Created($"/api/notes/{note.Id}", NoteDto.From(note));
        }

        private static async Task<Results<Ok<NoteDto>, NotFound>> Update(int id, NoteInput input, ApplicationDbContext db, CancellationToken ct)
        {
            var note = await db.Notes.FindAsync([id], ct);
            if (note is null) return TypedResults.NotFound();

            note.SortNum = input.SortNum;
            note.Text = input.Text;
            note.Status = input.Status;
            await db.SaveChangesAsync(ct);

            return TypedResults.Ok(NoteDto.From(note));
        }

        private static async Task<Results<NoContent, NotFound>> Delete(int id, ApplicationDbContext db, CancellationToken ct)
        {
            int deleted = await db.Notes.Where(n => n.Id == id).ExecuteDeleteAsync(ct);
            return deleted > 0 ? TypedResults.NoContent() : TypedResults.NotFound();
        }
    }
}
