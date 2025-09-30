using System.ComponentModel.DataAnnotations;

namespace Calendarium.Models;

public enum ReqHeadFieldType
{
    Accept,
    Crawler,
    Device,
    Encode,
    Language,
    Referer,
    UsrAgent
}

public class RequestHeaderField
{
    [DisplayFormat(DataFormatString = "{0:dd.MM_HH:mm}")]
    public DateTime Created { get; set; }

    public int Id { get; set; }

    public ReqHeadFieldType Field { get; set; }

    public string Text { get; set; }

    public RequestHeaderField(ReqHeadFieldType field, string text)
    {
        Created = DateTime.Now;
        Field = field;
        Text = text;
    }
}
