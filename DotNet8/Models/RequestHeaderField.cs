namespace DotNet8.Models
{
    public enum ReqHeadFieldType
    {
        Accept,
        Crawler,
        Device,
        Encode,
        Language,
        Referer,
        UAgent,
    }

    public class RequestHeaderField
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public ReqHeadFieldType Field { get; set; }
        public string Text { get; set; }

        public RequestHeaderField(ReqHeadFieldType field, string text)
        {
            Created = DateTime.Now;
            Field = field;
            Text = text;
        }
    }
}
