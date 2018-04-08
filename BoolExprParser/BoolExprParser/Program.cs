using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoolExprParser
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Please input a Boolean expression: ");
                var expr = Console.ReadLine();

                var tokens = BoolExprParser.ExtractTokens(expr);

                var pnTokens = BoolExprParser.TransformToPolishNotation(tokens);

                var enumerator = pnTokens.GetEnumerator();
                enumerator.MoveNext();
                var root = BoolExprParser.GenerateBoolBinTree(ref enumerator);

                var andGroups = BoolExprParser.ConvertToAnddGroups(root);

                for (int i = 0; i < andGroups.Count; i++)
                {
                    var andGroup = andGroups[i];
                    andGroup.ForEach(operand =>
                    {
                        Console.WriteLine($"AND Group: {i + 1} - IsNot: {operand.IsNot}; Literal: {operand.Literal}");
                    });
                }

                Console.WriteLine();
                Console.WriteLine();
            } while (1 > 0);
        }
    }
}
