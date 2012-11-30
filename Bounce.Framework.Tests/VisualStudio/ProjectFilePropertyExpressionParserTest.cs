using Bounce.Framework.VisualStudio;
using NUnit.Framework;

namespace Bounce.Framework.Tests.VisualStudio {
    [TestFixture]
    public class ProjectFilePropertyExpressionParserTest {
        private ProjectFilePropertyExpressionParser Parser;
        private PropertyValues Props;

        [SetUp]
        public void SetUp() {
            Props = new PropertyValues();
            Parser = new ProjectFilePropertyExpressionParser(Props);
        }

        [Test]
        public void ReturnsFalseWhenVariableDoesNotEqualString()
        {
            Props["var"] = "value";
            AssertConditionResult(" '$(var)' == 'valu' ", false);
        }

        [Test]
        public void ReturnsTrueWhenVariableEqualsString()
        {
            Props["var"] = "value";
            AssertConditionResult(" '$(var)' == 'value' ", true);
        }

        [Test]
        public void ReturnsTrueWhenEitherOperandToOrIsTrue()
        {
            Props["var"] = "value";
            AssertConditionResult(" '$(var)' == 'value' Or '$(var)' == 'valu' ", true);
            AssertConditionResult(" '$(var)' == 'valu' Or '$(var)' == 'value' ", true);
            AssertConditionResult(" '$(var)' == 'value' Or '$(var)' == 'value' ", true);
        }

        [Test]
        public void ReturnsFalseWhenNeitherOperandToOrIsTrue()
        {
            Props["var"] = "value";
            AssertConditionResult(" '$(var)' == 'alue' Or '$(var)' == 'valu' ", false);
        }

        private void AssertConditionResult(string source, bool expectedResult) {
            var result = Parser.ParseCondition(source);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}