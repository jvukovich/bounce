namespace LegacyBounce.Framework {
    public class AspNetWebSiteDirectory : Copy {
        public AspNetWebSiteDirectory() {
            Excludes = new[] {
                "**.cs",
                @"obj\**",
                "**.csproj.user",
                "**.csproj",
                "**.pdb",
            };
        }
    }
}