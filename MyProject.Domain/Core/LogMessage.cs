namespace MyProject.Domain.Core
{
    public abstract class LogMessage
    {
        public string Message { get; set; }
        public string Details { get; set; }
    }
}