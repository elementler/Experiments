using System;

namespace BoolExprParser
{
    public struct Operand
    {
        public string Literal { get; set; }
        public bool IsNot { get; set; }

        public Operand(string literal, bool isNot = false)
        {
            Literal = literal;
            IsNot = isNot;
        }
    }
}
