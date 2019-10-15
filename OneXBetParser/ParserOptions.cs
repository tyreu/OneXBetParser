namespace TestParser
{
    public class ParserOptions
    {
        public ParserOptions(uint timeoutSeconds, string uri)
        {
            TimeoutSeconds = timeoutSeconds;
            Uri = uri;
        }

        public string Login { get; set; }
        public string Password { get; set; }
        public uint TimeoutSeconds { get; set; } = 0;
        public string Uri { get; set; }
    }
}
