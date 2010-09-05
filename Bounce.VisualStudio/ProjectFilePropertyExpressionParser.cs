using System;
using System.Text;

namespace Bounce.VisualStudio {
    public class ProjectFilePropertyExpressionParser {
        private readonly IPropertyValues Props;


        public ProjectFilePropertyExpressionParser(IPropertyValues props) {
            Props = props;
        }

        public T Parse<T>(string sourceString, Func<char[], int, ParseResult<T>> parser) {
            char[] source = sourceString.ToCharArray();
            ParseResult<T> parseResult = parser(source, 0);

            if (parseResult != null) {
                var whitespace = ParseWhitespace(source, parseResult.Index);

                if (whitespace != null && whitespace.Index == source.Length) {
                    return parseResult.Value;
                }
            }

            throw new ConditionParseException();
        }

        private ParseResult<object> ParseWhitespace(char [] source, int index) {
            while (index < source.Length && (source[index] == ' ' || source[index] == '\t'))
            {
                index++;
            }

            return new ParseResult<object> {Index = index};
        }

        public ParseResult<bool> ParseEqualityExpression(char [] source, int index) {
            var whitespace = ParseWhitespace(source, index);
            var firstString = ParseString(source, whitespace.Index);
            if (firstString == null) {
                return null;
            }
            whitespace = ParseWhitespace(source, firstString.Index);
            var equalityOperator = ParseEquality(source, whitespace.Index);
            if (equalityOperator == null) {
                return null;
            }
            whitespace = ParseWhitespace(source, equalityOperator.Index);
            var secondString = ParseString(source, whitespace.Index);
            if (secondString == null) {
                return null;
            }

            return new ParseResult<bool> {Value = firstString.Value == secondString.Value, Index = secondString.Index};
        }

        private ParseResult<object> ParseEquality(char[] source, int index) {
            if (index >= source.Length) {
                return null;
            }
            
            if (source[index] != '=') {
                return null;
            }

            index++;
            if (index >= source.Length) {
                return null;
            }

            if (source[index] != '=') {
                return null;
            } else {
                index++;
                return new ParseResult<object> {Index = index};
            }
        }

        public ParseResult<string> ParseString(char [] source, int index) {
            if (index >= source.Length) {
                return null;
            }

            if (source[index] == '\'') {
                index++;

                var value = new StringBuilder();
                while (index < source.Length && source[index] != '\'')
                {
                    ParseResult<string> variable = ParseVariable(source, index);

                    if (variable != null) {
                        value.Append(variable.Value);
                        index = variable.Index;
                    } else {
                        value.Append(source[index]);
                    }
                    index++;
                }

                if (index < source.Length) {
                    index++;
                    return new ParseResult<string>() {Value = value.ToString(), Index = index};
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        private ParseResult<string> ParseVariable(char[] source, int index) {
            if (source[index] != '$') {
                return null;
            }

            index++;

            if (index >= source.Length || source[index] != '(') {
                return null;
            }

            index++;

            var variable = new StringBuilder();

            while (index < source.Length && source[index] != ')') {
                variable.Append(source[index]);
                index++;
            }

            if (index < source.Length) {
                return new ParseResult<string> {Value = Props[variable.ToString()], Index = index};
            } else {
                return null;
            }
        }

        public bool ParseCondition(string value) {
            return Parse<bool>(value, ParseEqualityExpression);
        }
    }
}