namespace Calendarium.Models;

public class BacsCallbackPayload
{
    public bool Success { get; set; }

    public DateTime CreationDate { get; set; }

    public Guid SubmissionId { get; set; }

    public string? Message { get; set; }

    public string? NextAction { get; set; }
}
