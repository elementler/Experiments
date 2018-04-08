using System;

namespace BoolExprParser
{
    [Serializable]
    public class BoolExprTreeException : Exception
    {
        public BoolExprTreeException()
        { }

        public BoolExprTreeException(string message) : base(message)
        { }

        public BoolExprTreeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
