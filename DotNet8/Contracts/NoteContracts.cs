using System.ComponentModel.DataAnnotations;
using Calendarium.Models;

namespace Calendarium.Contracts
{
    public sealed record NoteDto(int Id, int SortNum, string Text, NoteStatus Status)
    {
        public static NoteDto From(Note n) => new(n.Id, n.SortNum, n.Text, n.Status);
    }

    public sealed class NoteInput
    {
        public int SortNum { get; set; }

        [Required, StringLength(4000)]
        public string Text { get; set; } = string.Empty;

        public NoteStatus Status { get; set; } = NoteStatus.Active;
    }
}
