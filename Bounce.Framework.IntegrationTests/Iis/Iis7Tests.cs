using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bounce.Framework.Web;
using Microsoft.Web.Administration;
using NUnit.Framework;

namespace Bounce.Framework.IntegrationTests.Iis
{
    [TestFixture]
    public class Iis7Tests
    {
        private ServerManager IisServer;
        private string WebSiteName = "Bounce.Framework.IntegrationTests.WebSite";
        private Iis7 Iis;

        [SetUp]
        public void SetUp() {
            IisServer = new ServerManager();
            RemoveSite();
            RemoveApplication();
            RefreshIisServer();
            Iis = new Iis7();
        }

        private void RemoveSite() {
            var site = IisServer.Sites[WebSiteName];

            if (site != null) {
                IisServer.Sites.Remove(site);
                IisServer.CommitChanges();
            }
        }

        private void RemoveApplication() {
            var site = IisServer.Sites.Single(s => s.Bindings.Any(b => b.BindingInformation == "*:80:"));

            var app = site.Applications["/bounce.framework.integrationtests.website"];
            if (app != null) {
                app.Delete();
                IisServer.CommitChanges();
            }
        }

        [Test]
        public void ShouldInstallWebSite() {
            var site = Iis.WebSite(WebSiteName);
            site.Directory = @"c:\";
            site.Bindings.Add("http://*:4000/");
            site.Save();

            RefreshIisServer();

            Assert.That(IisServer.Sites[WebSiteName], Is.Not.Null);
        }

        [Test]
        public void ShouldInstallWebSiteWithBinding() {
            var site = Iis.WebSite(WebSiteName);
            site.Directory = @"c:\";
            site.Bindings.Add("http://example.com:4000/");
            site.Save();

            RefreshIisServer();

            var actualSite = IisServer.Sites[WebSiteName];
            Assert.That(actualSite.Bindings.Count, Is.EqualTo(1));
            Binding binding = actualSite.Bindings.ElementAt(0);
            Assert.That(binding.Protocol, Is.EqualTo("http"));
            Assert.That(binding.Host, Is.EqualTo("example.com"));
            Assert.That(binding.BindingInformation, Is.EqualTo("*:4000:example.com"));
        }

        [Test]
        public void ShouldInstallWebSiteWithMultipleBindings() {
            var site = Iis.WebSite(WebSiteName);
            site.Directory = @"c:\";
            site.Bindings.Add("http://example.com:4000/");
            site.Bindings.Add("http://*:4001/");
            site.Bindings.Add("http://127.0.0.1:4002/");
            site.Save();

            RefreshIisServer();

            var actualSite = IisServer.Sites[WebSiteName];
            Assert.That(actualSite.Bindings.Count, Is.EqualTo(3));

            Assert.That(actualSite.Bindings.Select(b => b.BindingInformation).ToArray(),
                Is.EquivalentTo(new [] {"*:4000:example.com", "*:4001:", "127.0.0.1:4002:"}));
            Assert.That(actualSite.Bindings.Select(b => b.Protocol).ToArray(),
                Is.EquivalentTo(new [] {"http", "http", "http"}));
        }

        [Test]
        public void ShouldInstallWebSiteAsApplicationUnderDefault() {
            var app = Iis.Application("http://*/bounce.framework.integrationtests.website");
            app.Directory = @"c:\";
            app.Save();

            RefreshIisServer();

            var site = IisServer.Sites.Single(s => s.Bindings.Any(b => b.BindingInformation == "*:80:"));
            var actualApp = site.Applications["/bounce.framework.integrationtests.website"];
            Assert.That(actualApp, Is.Not.Null);
        }

        [Test]
        public void WhenWebSiteExistsItShouldKnowIt() {
            var site = Iis.WebSite(WebSiteName);
            site.Directory = @"c:\";
            site.Bindings.Add("http://*:4000/");
            site.Save();

            Assert.That(Iis.WebSite(WebSiteName).Exists, Is.True);
        }

        [Test]
        public void WhenWebSiteDoesntExistsItShouldKnowIt() {
            Assert.That(Iis.WebSite(WebSiteName).Exists, Is.False);
        }

        [Test]
        public void WhenWebApplicationExistsItShouldKnowIt() {
            var app = Iis.Application("http://*/bounce.framework.integrationtests.website");
            app.Directory = @"c:\";
            app.Save();

            Assert.That(Iis.Application("http://*/bounce.framework.integrationtests.website").Exists);
        }

        [Test]
        public void WhenWebApplicationDoesntExistsItShouldKnowIt() {
            Assert.That(Iis.Application("http://*/bounce.framework.integrationtests.website").Exists, Is.False);
        }

        [Test]
        public void ThrowExceptionWhenWebSiteBindingCannotBeFound() {
            Assert.That(() => Iis.Application("http://*:4321/bounce.framework.integrationtests.website"), Throws.InstanceOf<WebSiteNotFoundException>());
        }

        private void RefreshIisServer() {
            IisServer = new ServerManager();
        }
    }
}
