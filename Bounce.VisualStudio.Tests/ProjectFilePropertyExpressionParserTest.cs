using NUnit.Framework;

namespace Bounce.VisualStudio.Tests {
    [TestFixture]
    public class ProjectFilePropertyExpressionParserTest {
        [Test]
        public void ShouldParseString() {
            var parser = new ProjectFilePropertyExpressionParser(null);
            AssertParseStringResult(parser, "'one'", "one");
        }

        [Test]
        public void ShouldParseVariableInString() {
            var props = new PropertyValues();
            props["var"] = "value";

            var parser = new ProjectFilePropertyExpressionParser(props);
            AssertParseStringResult(parser, "'$(var)'", "value");
        }

        [Test]
        public void ShouldParseCondition() {
            var props = new PropertyValues();
            props["var"] = "value";

            var parser = new ProjectFilePropertyExpressionParser(props);
            AssertParseConditionResult(parser, " '$(var)' == 'value' ", true);
            AssertParseConditionResult(parser, " '$(var)' == 'valu' ", false);
            AssertParseConditionResult(parser, " '$(var) ' == 'value' ", false);
        }

        private void AssertParseStringResult(ProjectFilePropertyExpressionParser parser, string source, string expected) {
            var result = parser.Parse<string>(source, parser.ParseString);
            Assert.That(result, Is.EqualTo(expected));
        }

        private void AssertParseConditionResult(ProjectFilePropertyExpressionParser parser, string source, bool expectedResult) {
            var result = parser.Parse<bool>(source, parser.ParseEqualityExpression);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}