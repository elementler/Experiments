using System.Collections.Generic;

namespace BoolExprParser
{
    public class Token
    {
        public enum TokenType
        {
            OPEN_PAREN,
            CLOSE_PAREN,
            UNARY_OP,
            BINARY_OP,
            LITERAL,
            EMPTY_OR_WHITE_SPACE
        }

        protected static readonly Dictionary<string, KeyValuePair<TokenType, string>> dict = new Dictionary<string, KeyValuePair<TokenType, string>>()
        {
            {
                "(", new KeyValuePair<TokenType, string>(TokenType.OPEN_PAREN, "(")
            },
            {
                ")", new KeyValuePair<TokenType, string>(TokenType.CLOSE_PAREN, ")")
            },
            {
                "NOT", new KeyValuePair<TokenType, string>(TokenType.UNARY_OP, "NOT")
            },
            {
                "AND", new KeyValuePair<TokenType, string>(TokenType.BINARY_OP, "AND")
            },
            {
                "OR", new KeyValuePair<TokenType, string>(TokenType.BINARY_OP, "OR")
            }
        };

        public Token(string tokenStr)
        {
            if (string.IsNullOrWhiteSpace(tokenStr))
            {
                Type = TokenType.EMPTY_OR_WHITE_SPACE;
                Value = tokenStr;
            }
            else
            {
                var upperTokenStr = tokenStr.Trim().ToUpper();

                if (dict.ContainsKey(upperTokenStr))
                {
                    Type = dict[upperTokenStr].Key;
                    Value = dict[upperTokenStr].Value;
                }
                else
                {
                    Type = TokenType.LITERAL;
                    Value = tokenStr.Trim();
                }
            }
        }

        public TokenType Type { get; protected set; }
        public string Value { get; protected set; }
    }
}
