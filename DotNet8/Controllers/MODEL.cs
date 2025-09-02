namespace Calendarium.Controllers
{
    public class MODEL
    {
        public bool Success { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid SubmissionId { get; set; }

        public string Message { get; set; }

        public string NextAction { get; set; }
    }
}
