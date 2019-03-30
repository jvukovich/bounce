using Moq;
using Xunit;

namespace Bounce.Console.Tests
{
    public class TargetsAssemblyArgumentsParserTest
    {
        [Fact]
        public void ShouldParseTargetsFromParameterWithColon()
        {
            var finder = new Mock<IBounceDirectoryFinder>();
            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            var targetsAndArgs = parser.GetTargetsAssembly(new[] {"/bounceDir:adir", "other", "args"});

            Assert.Equal(new[] {"other", "args"}, targetsAndArgs.RemainingArguments);
            Assert.Equal("adir", targetsAndArgs.BounceDirectory);
        }

        [Fact]
        public void ShouldParseTargetsFromParameter()
        {
            var finder = new Mock<IBounceDirectoryFinder>();
            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            var targetsAndArgs = parser.GetTargetsAssembly(new[] {"/bounceDir", "adir", "other", "args"});

            Assert.Equal(new[] {"other", "args"}, targetsAndArgs.RemainingArguments);
            Assert.Equal("adir", targetsAndArgs.BounceDirectory);
        }

        [Fact]
        public void ShouldParseTargetsAndRecurseFromArguments()
        {
            var finder = new Mock<IBounceDirectoryFinder>();
            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            var targetsAndArgs = parser.GetTargetsAssembly(new[] {"/bounceDir", "adir", "/recurse", "other", "args"});

            Assert.Equal(new[] {"other", "args"}, targetsAndArgs.RemainingArguments);
            Assert.Equal("adir", targetsAndArgs.BounceDirectory);
            Assert.True(targetsAndArgs.Recurse);
        }

        [Fact]
        public void ShouldParseRecurseFromArguments()
        {
            var finder = new Mock<IBounceDirectoryFinder>();

            finder.Setup(f => f.FindBounceDirectory()).Returns(@"path\to\bounce");

            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            var targetsAndArgs = parser.GetTargetsAssembly(new[] {"/recurse", "other", "args"});

            Assert.True(targetsAndArgs.Recurse);
            Assert.Equal(new[] {"other", "args"}, targetsAndArgs.RemainingArguments);
            Assert.Equal(@"path\to\bounce", targetsAndArgs.BounceDirectory);
        }

        [Fact]
        public void ShouldFindTargetsIfNoTargetsParameterGiven()
        {
            var finder = new Mock<IBounceDirectoryFinder>();

            finder.Setup(f => f.FindBounceDirectory()).Returns(@"path\to\bounce");

            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            var targetsAndArgs = parser.GetTargetsAssembly(new[] {"build", "SomeTarget", "/other", "args"});

            Assert.Equal(new[] {"build", "SomeTarget", "/other", "args"}, targetsAndArgs.RemainingArguments);
            Assert.Equal(@"path\to\bounce", targetsAndArgs.BounceDirectory);
            Assert.False(targetsAndArgs.Recurse);
        }

        [Fact]
        public void ShouldThrowIfTargetsAssemblyNotFound()
        {
            var finder = new Mock<IBounceDirectoryFinder>();
            var parser = new TargetsAssemblyArgumentsParser(finder.Object);

            var ex = Assert.Throws<TargetsAssemblyNotFoundException>(() => parser.GetTargetsAssembly(new[] {"SomeTask", "/other", "args"}));
            
            Assert.Equal("Exception of type 'Bounce.Console.TargetsAssemblyNotFoundException' was thrown.", ex.Message);
        }

        [Fact]
        public void ShouldAttemptToFindAssemblyIfNoArgsGiven()
        {
            var finder = new Mock<IBounceDirectoryFinder>();

            finder.Setup(f => f.FindBounceDirectory()).Returns(@"path\to\bounce");

            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            var targetsAndArgs = parser.GetTargetsAssembly(new string[0]);

            Assert.Empty(targetsAndArgs.RemainingArguments);
            Assert.Equal(@"path\to\bounce", targetsAndArgs.BounceDirectory);
        }
    }
}