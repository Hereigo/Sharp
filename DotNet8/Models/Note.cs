namespace DotNet8.Models
{
    public enum NoteStatus
    {
        Active,
        Disabled,
        Deleted
    }

    public class Note
    {
        public int Id { get; set; }

        public int SortNum { get; set; }

        public string Text { get; set; }

        public NoteStatus Status { get; set; } = NoteStatus.Active;
    }
}
