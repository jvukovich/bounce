using Xunit;

namespace Bounce.Tests
{
    public class TargetsAssemblyArgumentsParserTest
    {
        [Fact]
        public void ShouldParseTargetsFromParameterWithColon()
        {
            var targetsAndArgs = TargetsAssemblyArgumentsParser.GetTargetsAssembly(new[] {"/bounceDir:adir", "other", "args"});

            Assert.Equal(new[] {"other", "args"}, targetsAndArgs.RemainingArguments);
            Assert.Equal("adir", targetsAndArgs.BounceDirectory);
        }

        [Fact]
        public void ShouldParseTargetsFromParameter()
        {
            var targetsAndArgs = TargetsAssemblyArgumentsParser.GetTargetsAssembly(new[] {"/bounceDir", "adir", "other", "args"});

            Assert.Equal(new[] {"other", "args"}, targetsAndArgs.RemainingArguments);
            Assert.Equal("adir", targetsAndArgs.BounceDirectory);
        }

        [Fact]
        public void ShouldFindTargetsIfNoTargetsParameterGiven()
        {
            var targetsAndArgs = TargetsAssemblyArgumentsParser.GetTargetsAssembly(new[] {"build", "SomeTarget", "/other", "args"});

            Assert.Equal(new[] {"build", "SomeTarget", "/other", "args"}, targetsAndArgs.RemainingArguments);
            Assert.NotNull(targetsAndArgs.BounceDirectory);
        }

        [Fact]
        public void ShouldAttemptToFindAssemblyIfNoArgsGiven()
        {
            var targetsAndArgs = TargetsAssemblyArgumentsParser.GetTargetsAssembly(new string[0]);

            Assert.Empty(targetsAndArgs.RemainingArguments);
            Assert.NotNull(targetsAndArgs.BounceDirectory);
        }

        // todo: additional unit tests to cover multiple task invocation
    }
}