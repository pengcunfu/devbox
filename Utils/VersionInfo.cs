using System.Reflection;

namespace ProgramBox.Utils
{
    public static class VersionInfo
    {
        public const string DocumentationUrl = "https://programbox.github.io/programbox/";
        public const string RepositoryUrl = "https://github.com/programbox/programbox";
        public const string ReleasesUrl = "https://github.com/programbox/programbox/releases";

        public static string Current =>
            Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
            ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)
            ?? "1.0.0";

        public static string Display => $"v{Current}";
    }
}
