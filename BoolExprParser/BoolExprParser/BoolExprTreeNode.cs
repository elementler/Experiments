using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoolExprParser
{
    public class BoolExprTreeNode : ICloneable
    {
        public enum BoolOperator
        {
            LEAF,
            AND,
            OR,
            NOT
        };

        private BoolOperator _op;
        private BoolExprTreeNode _left;
        private BoolExprTreeNode _right;
        private string _literal;

        //
        // Private constructors
        //
        private BoolExprTreeNode(BoolOperator op, BoolExprTreeNode left, BoolExprTreeNode right)
        {
            _op = op;
            _left = left;
            _right = right;
            _literal = null;
        }

        private BoolExprTreeNode(string literal)
        {
            _op = BoolOperator.LEAF;
            _left = null;
            _right = null;
            _literal = literal;
        }

        public BoolOperator Operator
        {
            get { return _op; }
            set { _op = value; }
        }

        public BoolExprTreeNode Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public BoolExprTreeNode Right
        {
            get { return _right; }
            set { _right = value; }
        }

        public string Literal
        {
            get { return _literal; }
            set { _literal = value; }
        }

        //
        // Public factories
        //
        public static BoolExprTreeNode CreateAndOpNode(BoolExprTreeNode left, BoolExprTreeNode right)
        {
            return new BoolExprTreeNode(BoolOperator.AND, left, right);
        }

        public static BoolExprTreeNode CreateNotOpNode(BoolExprTreeNode child)
        {
            return new BoolExprTreeNode(BoolOperator.NOT, child, null);
        }

        public static BoolExprTreeNode CreateOrOpNode(BoolExprTreeNode left, BoolExprTreeNode right)
        {
            return new BoolExprTreeNode(BoolOperator.OR, left, right);
        }

        public static BoolExprTreeNode CreateBoolVar(string literal)
        {
            return new BoolExprTreeNode(literal);
        }

        public BoolExprTreeNode(BoolExprTreeNode other)
        {
            _op = other._op;
            _left = other._left == null ? null : new BoolExprTreeNode(other._left);
            _right = other._right == null ? null : new BoolExprTreeNode(other._right);
            _literal = new StringBuilder(other._literal).ToString();
        }

        public bool IsLeaf()
        {
            return (_op == BoolOperator.LEAF);
        }

        /// <summary>
        /// Implements deep copy.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new BoolExprTreeNode(this);
        }
    }
}
