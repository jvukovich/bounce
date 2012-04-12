namespace Bounce.Framework.Obsolete
{
    public class EnvironmentVariables
    {
        public static Task<T> Default<T>(string name, T value)
        {
            return new EnvironmentVariable<T>(name) {DefaultValue = value};
        }

        public static Task<T> Required<T>(string name)
        {
            return new EnvironmentVariable<T>(name);
        }
    }
}