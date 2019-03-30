namespace Bounce.Console
{
    public class OptionsAndArguments
    {
        public string[] RemainingArguments { get; set; }
        public string BounceDirectory { get; set; }
        public string WorkingDirectory { get; set; }
        public bool Recurse { get; set; }
    }
}