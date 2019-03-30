using Bounce.Framework.VisualStudio;
using Xunit;

namespace Bounce.Framework.Tests.VisualStudio
{
    public class ProjectFilePropertyExpressionParserTest
    {
        private readonly ProjectFilePropertyExpressionParser parser;
        private readonly PropertyValues props;

        public ProjectFilePropertyExpressionParserTest()
        {
            props = new PropertyValues();
            parser = new ProjectFilePropertyExpressionParser(props);
        }

        [Fact]
        public void ReturnsFalseWhenVariableDoesNotEqualString()
        {
            props["var"] = "value";
            AssertConditionResult(" '$(var)' == 'valu' ", false);
        }

        [Fact]
        public void ReturnsTrueWhenVariableEqualsString()
        {
            props["var"] = "value";
            AssertConditionResult(" '$(var)' == 'value' ", true);
        }

        [Fact]
        public void ReturnsTrueWhenEitherOperandToOrIsTrue()
        {
            props["var"] = "value";
            AssertConditionResult(" '$(var)' == 'value' Or '$(var)' == 'valu' ", true);
            AssertConditionResult(" '$(var)' == 'valu' Or '$(var)' == 'value' ", true);
            AssertConditionResult(" '$(var)' == 'value' Or '$(var)' == 'value' ", true);
        }

        [Fact]
        public void ReturnsFalseWhenNeitherOperandToOrIsTrue()
        {
            props["var"] = "value";
            AssertConditionResult(" '$(var)' == 'alue' Or '$(var)' == 'valu' ", false);
        }

        private void AssertConditionResult(string source, bool expectedResult)
        {
            var result = parser.ParseCondition(source);

            Assert.Equal(expectedResult, result);
        }
    }
}