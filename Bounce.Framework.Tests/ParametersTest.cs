using System.Collections.Generic;
using Xunit;

namespace Bounce.Framework.Tests
{
    public class ParametersTest
    {
        [Fact]
        public void CanParseDefaultStringParameter()
        {
            var p = new TaskParameters(new Dictionary<string, string> {{"file", "thefile.txt"}});
            Assert.Equal("thefile.txt", p.Parameter("file", "afile.txt"));
        }

        [Fact]
        public void CanParseDefaultStringParameterIfNotPresent()
        {
            var p = new TaskParameters(new Dictionary<string, string>());
            Assert.Equal("afile.txt", p.Parameter("file", "afile.txt"));
        }

        [Fact]
        public void CanParseStringParameter()
        {
            var p = new TaskParameters(new Dictionary<string, string> {{"file", "thefile.txt"}});
            Assert.Equal("thefile.txt", p.Parameter("file", "afile.txt"));
        }

        [Fact]
        public void CanParseDefaultStringParameterWithType()
        {
            var p = new TaskParameters(new Dictionary<string, string> {{"file", "thefile.txt"}});
            Assert.Equal("thefile.txt", p.Parameter(typeof(string), "file", "afile.txt"));
        }

        [Fact]
        public void CanParseDefaultStringParameterIfNotPresentWithType()
        {
            var p = new TaskParameters(new Dictionary<string, string>());
            Assert.Equal("afile.txt", p.Parameter(typeof(string), "file", "afile.txt"));
        }

        [Fact]
        public void CanParseStringParameterWithType()
        {
            var p = new TaskParameters(new Dictionary<string, string> {{"file", "thefile.txt"}});
            Assert.Equal("thefile.txt", p.Parameter(typeof(string), "file", "afile.txt"));
        }

        [Fact]
        public void ThrowsExceptionWhenStringParameterIsNotPresentWithType()
        {
            var p = new TaskParameters(new Dictionary<string, string>());
            var ex = Assert.Throws<RequiredParameterNotGivenException>(() => p.Parameter(typeof(string), "file"));

            Assert.Equal("required parameter 'file' not given", ex.Message);
        }

        [Fact]
        public void ThrowsExceptionWhenStringParameterIsNotPresent()
        {
            var p = new TaskParameters(new Dictionary<string, string>());
            var ex = Assert.Throws<RequiredParameterNotGivenException>(() => p.Parameter<string>("file"));

            Assert.Equal("required parameter 'file' not given", ex.Message);
        }

        [Fact]
        public void CanParseEnumeration()
        {
            var p = new TaskParameters(new Dictionary<string, string> {{"lake", "constance"}});
            Assert.Equal(Lakes.Constance, p.Parameter<Lakes>("lake"));
        }

        enum Lakes
        {
            Constance,
            Coniston,
            Consequence
        }
    }
}