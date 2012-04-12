using System.Collections.Generic;
using System.Linq;
using Bounce.Framework.Obsolete;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete
{
    [TestFixture]
    public class BuilderTest
    {
        [Test]
        public void ShouldBuildStuff() {
            var x = new Builder<A>();
            var a = new BuilderWithCreator<A>(() => new A());

            var b = a.WithName("name").WithDescription("description");
            var c = b.WithName("cname");

            var fromC = c.Build;
            var fromB = b.Build;

            Assert.That(fromB.Name, Is.EqualTo("name"));
            Assert.That(fromB.Description, Is.EqualTo("description"));

            Assert.That(fromC.Name, Is.EqualTo("cname"));
            Assert.That(fromC.Description, Is.EqualTo("description"));
        }

        Dictionary<string,IEnumerable<WebSiteConfig>> WebSiteEnvironmnents() {
            var webSite = new Builder<WebSiteConfig>(() => new WebSiteConfig {
                AppPoolName = "MyWebSite",
                Port = 80,
                HostName = "mywebsite.com",
                Directory = @"C:\Sites\MyWebSite",
            });

            var envs = new Dictionary<string,IEnumerable<WebSiteConfig>>();

            var live = webSite;
            envs["live"] = live.OnMachines("live1", "live2");

            var stage = webSite.WithHostName("mywebsitestage.com");
            envs["stage"] = stage.OnMachines("stage1", "stage2");

            var test = webSite.WithHostName("mywebsitetest");
            envs["test"] = test.OnMachines("test1");

            var dev = webSite.WithNoHostName().WithPort(6001);
            envs["dev"] = dev.OnMachines("localhost");

            return envs;
        }
    }

    class WebSiteConfig {
        public string Name;
        public string AppPoolName;
        public int Port;
        public string HostName;
        public string Directory;
        public string Machine;
    }

    static class WebSiteConfigBuilderExtensions {
        public static IBuilder<WebSiteConfig> WithName(this IBuilder<WebSiteConfig> b, string name) {
            return b.With(w => w.Name = name);
        }

        public static IBuilder<WebSiteConfig> WithAppPoolName(this IBuilder<WebSiteConfig> b, string name) {
            return b.With(w => w.AppPoolName = name);
        }

        public static IBuilder<WebSiteConfig> WithPort(this IBuilder<WebSiteConfig> b, int port) {
            return b.With(w => w.Port = port);
        }

        public static IBuilder<WebSiteConfig> WithHostName(this IBuilder<WebSiteConfig> b, string hostName) {
            return b.With(w => w.HostName = hostName);
        }

        public static IBuilder<WebSiteConfig> WithNoHostName(this IBuilder<WebSiteConfig> b) {
            return b.WithHostName(null);
        }

        public static IBuilder<WebSiteConfig> WithDirectory(this IBuilder<WebSiteConfig> b, string directory) {
            return b.With(w => w.Directory = directory);
        }

        public static IBuilder<WebSiteConfig> OnMachine(this IBuilder<WebSiteConfig> b, string machine) {
            return b.With(w => w.Machine = machine);
        }

        public static IEnumerable<WebSiteConfig> OnMachines(this IBuilder<WebSiteConfig> b, params string [] machines) {
            return machines.Select(machine => b.With(w => w.Machine = machine).Build);
        }
    }

    class A {
        public string Name;
        public string Description;
    }

    static class AExtensions {
        public static IBuilder<A> WithName(this IBuilder<A> builder, string name) {
            return builder.With(a => a.Name = name);
        }

        public static IBuilder<A> WithDescription(this IBuilder<A> builder, string description) {
            return builder.With(a => a.Description = description);
        }
    }
}
