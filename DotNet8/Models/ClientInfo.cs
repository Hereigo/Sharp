namespace DotNet8.Models
{
    public enum ClientInfoType
    {
        Accept,
        Crawler,
        Device,
        Encode,
        Language,
        Referer,
        UAgent,
    }

    public class ClientInfo
    {
        public int Id { get; set; }

        public ClientInfoType Type { get; set; }

        public string Info { get; set; }
    }
}
