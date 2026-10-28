namespace Calendarium.DbLogger
{
    public class Error
    {
        public int Id { get; set; }
        public string Values { get; set; } = string.Empty;
        public DateTime Created { get; set; }
    }
}
