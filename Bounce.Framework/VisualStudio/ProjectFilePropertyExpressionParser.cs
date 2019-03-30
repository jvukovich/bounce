using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bounce.Framework.VisualStudio
{
    public class ProjectFilePropertyExpressionParser
    {
        private readonly IPropertyValues Props;

        public ProjectFilePropertyExpressionParser(IPropertyValues props)
        {
            Props = props;
        }

        public T Parse<T>(string sourceString, Func<char[], int, ParseResult<T>> parser)
        {
            char[] source = sourceString.ToCharArray();
            ParseResult<T> parseResult = parser(source, 0);

            if (parseResult != null)
            {
                var whitespace = ParseWhitespace(source, parseResult.Index);

                if (whitespace != null && whitespace.Index == source.Length)
                {
                    return parseResult.Value;
                }
            }

            throw new ConditionParseException();
        }

        private ParseResult<object> ParseWhitespace(char[] source, int index)
        {
            while (index < source.Length && (source[index] == ' ' || source[index] == '\t'))
            {
                index++;
            }

            return new ParseResult<object> {Index = index};
        }

        private ParseResult<object> ParseKeyword(char[] source, int index, string keyword)
        {
            if (index < source.Length && new string(source, index, keyword.Length) == keyword)
            {
                return new ParseResult<object> {Index = index + keyword.Length};
            } else
            {
                return null;
            }
        }

        private ParseResult<bool> ParseOrExpression(char[] source, int index)
        {
            var left = ParseEqualityExpression(source, index);

            if (left == null)
            {
                return null;
            }

            var whitespace = ParseWhitespace(source, left.Index);
            var op = ParseKeyword(source, whitespace.Index, "Or");

            if (op == null)
            {
                return null;
            }

            whitespace = ParseWhitespace(source, op.Index);
            var right = ParseExpression(source, whitespace.Index);
            if (right == null)
            {
                return null;
            }

            return new ParseResult<bool> {Index = right.Index, Value = left.Value || right.Value};
        }

        private ParseResult<bool> ParseExpression(char[] source, int index)
        {
            var expressionParsers = new Func<char[], int, ParseResult<bool>>[] {ParseOrExpression, ParseEqualityExpression};
            return expressionParsers.Select(parser => parser(source, index)).FirstOrDefault(result => result != null);
        }

        private ParseResult<string> ParseTerminal(char[] source, int index)
        {
            var expressionParsers = new Func<char[], int, ParseResult<string>>[] {ParseVariable, ParseString};
            return expressionParsers.Select(parser => parser(source, index)).FirstOrDefault(result => result != null);
        }

        public ParseResult<bool> ParseEqualityExpression(char[] source, int index)
        {
            var whitespace = ParseWhitespace(source, index);
            var firstString = ParseTerminal(source, whitespace.Index);
            if (firstString == null)
            {
                return null;
            }

            whitespace = ParseWhitespace(source, firstString.Index);
            var equalityOperator = ParseEquality(source, whitespace.Index);
            if (equalityOperator == null)
            {
                return null;
            }

            whitespace = ParseWhitespace(source, equalityOperator.Index);
            var secondString = ParseTerminal(source, whitespace.Index);
            if (secondString == null)
            {
                return null;
            }

            return new ParseResult<bool> {Value = firstString.Value == secondString.Value, Index = secondString.Index};
        }

        private ParseResult<object> ParseEquality(char[] source, int index)
        {
            if (index >= source.Length)
            {
                return null;
            }

            if (source[index] != '=')
            {
                return null;
            }

            index++;
            if (index >= source.Length)
            {
                return null;
            }

            if (source[index] != '=')
            {
                return null;
            } else
            {
                index++;
                return new ParseResult<object> {Index = index};
            }
        }

        public ParseResult<string> ParseString(char[] source, int index)
        {
            if (index >= source.Length)
            {
                return null;
            }

            if (source[index] == '\'')
            {
                index++;

                var value = new StringBuilder();
                while (index < source.Length && source[index] != '\'')
                {
                    ParseResult<string> variable = ParseVariable(source, index);

                    if (variable != null)
                    {
                        value.Append(variable.Value);
                        index = variable.Index;
                    } else
                    {
                        value.Append(source[index]);
                        index++;
                    }
                }

                if (index < source.Length)
                {
                    index++;
                    return new ParseResult<string>() {Value = value.ToString(), Index = index};
                } else
                {
                    return null;
                }
            } else
            {
                return null;
            }
        }

        private ParseResult<string> ParseVariable(char[] source, int index)
        {
            if (source[index] != '$')
            {
                return null;
            }

            index++;

            if (index >= source.Length || source[index] != '(')
            {
                return null;
            }

            index++;

            var variable = new StringBuilder();

            while (index < source.Length && source[index] != ')')
            {
                variable.Append(source[index]);
                index++;
            }

            index++;

            if (index < source.Length)
            {
                return new ParseResult<string> {Value = Props[variable.ToString()], Index = index};
            } else
            {
                return null;
            }
        }

        public bool ParseCondition(string value)
        {
            return Parse(value, ParseExpression);
        }
    }
}