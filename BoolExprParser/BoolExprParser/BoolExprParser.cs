using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BoolExprParser
{
    public static class BoolExprParser
    {
        /// <summary>
        /// Converts the raw Boolean expression to a list of tokens.
        /// </summary>
        /// <param name="rawBoolExpr"></param>
        /// <returns></returns>
        public static List<Token> ExtractTokens(string rawBoolExpr)
        {
            if (string.IsNullOrWhiteSpace(rawBoolExpr))
            {
                throw new ArgumentException($"Please specify a valid Boolean expression for argument ${nameof(rawBoolExpr)}.");
            }

            // The pattern below includes the following operators:
            // (, ), AND, OR, NOT
            var pattern = @"(\x28)|(\x29)|(\sAND\s)|(\sOR\s)|(\s?NOT\s)";

            var rawArray = Regex.Split(rawBoolExpr, pattern, RegexOptions.IgnoreCase);

            var tokenList = new List<Token>();
            foreach (var rawElement in rawArray)
            {
                var token = new Token(rawElement);
                if (token.Type != Token.TokenType.EMPTY_OR_WHITE_SPACE)
                {
                    tokenList.Add(token);
                }
            }

            return tokenList;
        }

        /// <summary>
        /// Transforms the list of tokens in Polish Notation order.
        /// </summary>
        /// <param name="infixTokenList"></param>
        /// <returns></returns>
        public static List<Token> TransformToPolishNotation(List<Token> infixTokenList)
        {
            if (null == infixTokenList || infixTokenList.Count == 0)
            {
                throw new ArgumentException($"Please specify a list of valid tokens for argument {nameof(infixTokenList)}.");
            }

            var outputQueue = new Queue<Token>();
            var stack = new Stack<Token>();

            int index = 0;
            while (infixTokenList.Count > index)
            {
                var token = infixTokenList[index];

                switch (token.Type)
                {
                    case Token.TokenType.LITERAL:
                        outputQueue.Enqueue(token);
                        break;
                    case Token.TokenType.BINARY_OP:
                    case Token.TokenType.UNARY_OP:
                    case Token.TokenType.OPEN_PAREN:
                        stack.Push(token);
                        break;
                    case Token.TokenType.CLOSE_PAREN:
                        while (stack.Peek().Type != Token.TokenType.OPEN_PAREN)
                        {
                            outputQueue.Enqueue(stack.Pop());
                        }
                        stack.Pop();
                        if (stack.Count > 0 && stack.Peek().Type == Token.TokenType.UNARY_OP)
                        {
                            outputQueue.Enqueue(stack.Pop());
                        }
                        break;
                    default:
                        break;
                }

                ++index;
            }
            while (stack.Count > 0)
            {
                outputQueue.Enqueue(stack.Pop());
            }

            return outputQueue.Reverse().ToList();
        }

        /// <summary>
        /// Builds the Binary Boolean Expression Tree.
        /// </summary>
        /// <param name="polishNotationTokensEnumerator"></param>
        /// <returns></returns>
        public static BoolExprTreeNode GenerateBoolBinTree(ref List<Token>.Enumerator polishNotationTokensEnumerator)
        {
            if (polishNotationTokensEnumerator.Current.Type == Token.TokenType.LITERAL)
            {
                var literalNode = BoolExprTreeNode.CreateBoolVar(polishNotationTokensEnumerator.Current.Value);
                polishNotationTokensEnumerator.MoveNext();
                return literalNode;
            }
            else
            {
                if (polishNotationTokensEnumerator.Current.Value == "NOT")
                {
                    polishNotationTokensEnumerator.MoveNext();
                    var operand = GenerateBoolBinTree(ref polishNotationTokensEnumerator);
                    return BoolExprTreeNode.CreateNotOpNode(operand);
                }
                else if (polishNotationTokensEnumerator.Current.Value == "AND")
                {
                    polishNotationTokensEnumerator.MoveNext();
                    var leftNode = GenerateBoolBinTree(ref polishNotationTokensEnumerator);
                    var rightNode = GenerateBoolBinTree(ref polishNotationTokensEnumerator);
                    return BoolExprTreeNode.CreateAndOpNode(leftNode, rightNode);
                }
                else if (polishNotationTokensEnumerator.Current.Value == "OR")
                {
                    polishNotationTokensEnumerator.MoveNext();
                    var leftNode = GenerateBoolBinTree(ref polishNotationTokensEnumerator);
                    var rightNode = GenerateBoolBinTree(ref polishNotationTokensEnumerator);
                    return BoolExprTreeNode.CreateOrOpNode(leftNode, rightNode);
                }
            }
            return null;
        }

        /// <summary>
        /// Traverse the Binary Boolean Expression Tree from the given starting
        /// node, in order to generate a list of AND groups of operands.
        /// </summary>
        /// <param name="boolExprTreeNode"></param>
        /// <returns></returns>
        public static List<List<Operand>> ConvertToAnddGroups(BoolExprTreeNode boolExprTreeNode)
        {
            if (null != boolExprTreeNode)
            {
                if (boolExprTreeNode.IsLeaf())
                {
                    return new List<List<Operand>>()
                {
                    new List<Operand>()
                    {
                        new Operand(boolExprTreeNode.Literal)
                    }
                };
                }
                else
                {
                    if (boolExprTreeNode.Operator == BoolExprTreeNode.BoolOperator.OR)
                    {
                        var left = ConvertToAnddGroups(boolExprTreeNode.Left);
                        var right = ConvertToAnddGroups(boolExprTreeNode.Right);

                        if (null == left || null == right)
                        {
                            throw new BoolExprTreeException("The specified Binary Boolean Expression Tree is not valid.");
                        }

                        left.AddRange(right);
                        return left;
                    }

                    if (boolExprTreeNode.Operator == BoolExprTreeNode.BoolOperator.AND)
                    {
                        var left = ConvertToAnddGroups(boolExprTreeNode.Left);
                        var right = ConvertToAnddGroups(boolExprTreeNode.Right);

                        if (null == left || null == right)
                        {
                            throw new BoolExprTreeException("The specified Binary Boolean Expression Tree is not valid.");
                        }

                        var combinations = (from l in left
                                            from r in right
                                            select new { l, r }).ToList();

                        var results = new List<List<Operand>>();
                        combinations.ForEach(combination =>
                        {
                            var result = new List<Operand>(combination.l);
                            var newRight = new List<Operand>(combination.r);
                            result.AddRange(newRight);
                            results.Add(result);
                        });

                        return results;
                    }

                    if (boolExprTreeNode.Operator == BoolExprTreeNode.BoolOperator.NOT)
                    {
                        if (null != boolExprTreeNode.Right)
                        {
                            throw new BoolExprTreeException("The specified Binary Boolean Expression Tree is not valid.");
                        }

                        var left = ConvertToAnddGroups(boolExprTreeNode.Left);

                        left.ForEach(andGroup =>
                        {
                            andGroup.ForEach(operand =>
                            {
                                operand.IsNot = true;
                            });
                        });

                        return left;
                    }
                }
            }

            return null;
        }

    }
}
