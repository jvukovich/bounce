using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bounce.Framework.Tests
{
    public class ParameterParserTest
    {
        [Fact]
        public void ShouldParseNoParameters()
        {
            var paramDict = ArgumentsParser.ParseParameters(new string[0]);

            Assert.Empty(paramDict);
        }

        [Fact]
        public void ShouldParseParametersWithColons()
        {
            var args = Params("/file:afile.txt /name:nobody").ToList();
            var paramDict = ArgumentsParser.ParseParameters(args);

            Assert.Equal("afile.txt", paramDict["file"]);
            Assert.Equal("nobody", paramDict["name"]);

            Assert.Equal("afile.txt", Props.Get("file"));
            Assert.Equal("nobody", Props.Get("name"));
        }

        [Fact]
        public void ShouldParseParametersWithSpaces()
        {
            var paramDict = ArgumentsParser.ParseParameters(Params("/file afile.txt /name nobody"));

            Assert.Equal("afile.txt", paramDict["file"]);
            Assert.Equal("nobody", paramDict["name"]);

            Assert.Equal("afile.txt", Props.Get("file"));
            Assert.Equal("nobody", Props.Get("name"));
        }

        [Fact]
        public void ShouldParseBooleanParameterAtEnd()
        {
            var paramDict = ArgumentsParser.ParseParameters(Params("/file:afile.txt /fast"));

            Assert.Equal("afile.txt", paramDict["file"]);
            Assert.Equal("true", paramDict["fast"]);

            Assert.Equal("afile.txt", Props.Get("file"));
            Assert.Equal("true", Props.Get("fast"));
        }

        [Fact]
        public void ShouldParseBooleanParameterInMiddle()
        {
            var paramDict = ArgumentsParser.ParseParameters(Params("/fast /file:afile.txt"));

            Assert.Equal("afile.txt", paramDict["file"]);
            Assert.Equal("true", paramDict["fast"]);

            Assert.Equal("afile.txt", Props.Get("file"));
            Assert.Equal("true", Props.Get("fast"));
        }

        [Fact]
        public void ThrowsIfNonNamedArgumentFound()
        {
            var ex = Assert.Throws<Exception>(() => ArgumentsParser.ParseParameters(Params("afile.txt")));

            Assert.Contains("expected switch argument beginning with", ex.Message);
        }

        private static IEnumerable<string> Params(string parameters)
        {
            return parameters.Split(' ');
        }
    }
}