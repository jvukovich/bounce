using System;
using Microsoft.Web.Administration;
using NUnit.Framework;

namespace Bounce.Tests {
    [TestFixture]
    public class Iis7Admin {
        [Test]
        public void Stuff() {
            var sv = new ServerManager();

            Site existing = sv.Sites["Test"];
            if (existing != null) {
                sv.Sites.Remove(existing);
            }

            Site s = sv.Sites.Add("Test", @"c:\Users\Public\Documents\Development\LondonTyrant\LondonTyrant", 5000);

            foreach (var attr in s.Attributes) {
                Console.WriteLine("{0}: {1}", attr.Name, attr.Value);
            }

            foreach (var app in s.Applications) {
//                Console.WriteLine(app.Path);
                foreach (var vd in app.VirtualDirectories) {
                    Console.WriteLine(vd.Path);
                    Console.WriteLine(vd.PhysicalPath);
                }
            }

            foreach (var b in s.Bindings) {
                Console.WriteLine(b.BindingInformation);
            }

            sv.CommitChanges();
        }
    }
}