namespace Bounce.Framework
{
    public static class LogExtensions
    {
        public static string LogEscape(this string arg)
        {
            return string.IsNullOrWhiteSpace(arg) ? arg : arg.Replace("{", "{{").Replace("}", "}}");
        }
    }
}