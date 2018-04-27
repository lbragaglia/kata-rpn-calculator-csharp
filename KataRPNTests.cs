using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace KataRPN
{
    class KataRPNTests
    {
        private Parser _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new Parser();
        }

        [Test]
        [TestCase("7", 7)]
        [TestCase("20 5 /", 4)]
        [TestCase("4 2 +", 6)]
        [TestCase("4 2 + 3 -", 3)]
        [TestCase("3 5 8 * 7 + *", 141)]
        [TestCase("9 SQRT", 3)]
        [TestCase("5 3 4 2 9 1 MAX", 9)]
        [TestCase("4 5 MAX 1 2 MAX *", 10)]
        public void Test(string expression, int result)
        {
            var actual = _sut.Calculate(expression);
            Assert.AreEqual(result, actual);
        }
    }

    internal class Parser
    {
        private static readonly TokenFactory TokenFactory = new TokenFactory();

        public int Calculate(string expression)
        {
            var tokens = CreateTokens(expression);
            var operands = new Stack<int>();
            foreach (var token in tokens)
            {
                token.Evaluate(operands);
            }

            return operands.Pop();
        }

        private static IEnumerable<IToken> CreateTokens(string expression)
        {
            return expression.Split(' ').Select(t => TokenFactory.Create(t));
        }
    }

    internal class TokenFactory
    {
        private readonly IDictionary<string, OperatorToken> _tokenMap = new OperatorToken[]
        {
            new DivisionOperator(),
            new MultiplyOperation(),
            new PlusOperation(),
            new MinusOperation(),
            new SqrtOperation(),
            new MaxOperation()
        }.ToDictionary(v => v.Value, v => v);

        public IToken Create(string tokenValue)
        {
            if (_tokenMap.ContainsKey(tokenValue))
            {
                return _tokenMap[tokenValue];
            }
            return new NumberToken(tokenValue);
        }
    }

    internal class SqrtOperation : OperatorToken
    {
        public SqrtOperation() : base("SQRT")
        {
        }

        public override void Evaluate(Stack<int> operands)
        {
            operands.Push((int) Math.Sqrt(operands.Pop()));
        }
    }

    internal class MaxOperation : OperatorToken
    {
        public MaxOperation() : base("MAX")
        {
        }

        public override void Evaluate(Stack<int> operands)
        {
            var numbers = new List<int>();
            while (operands.Count > 0)
            {
                numbers.Add(operands.Pop());
            }
            operands.Push(numbers.Max());
        }
    }

    internal class MultiplyOperation : OperatorToken
    {
        public MultiplyOperation() : base("*")
        {
        }

        public override void Evaluate(Stack<int> operands)
        {
            var rightOp = operands.Pop();
            var leftOp = operands.Pop();
            operands.Push(leftOp*rightOp);
        }
    }

    internal class PlusOperation : OperatorToken
    {
        public PlusOperation() : base("+")
        {
        }

        public override void Evaluate(Stack<int> operands)
        {
            var rightOp = operands.Pop();
            var leftOp = operands.Pop();
            operands.Push(leftOp + rightOp);
        }
    }

    internal class MinusOperation : OperatorToken
    {
        public MinusOperation() : base("-")
        {
        }

        public override void Evaluate(Stack<int> operands)
        {
            var rightOp = operands.Pop();
            var leftOp = operands.Pop();
            operands.Push(leftOp - rightOp);
        }
    }

    internal class DivisionOperator : OperatorToken
    {
        public DivisionOperator() : base("/")
        {
        }

        public override void Evaluate(Stack<int> operands)
        {
            var rightOp = operands.Pop();
            var leftOp = operands.Pop();
            operands.Push(leftOp/rightOp);
        }
    }

    internal abstract class OperatorToken : IToken
    {
        public string Value { get; private set; }

        protected OperatorToken(string tokenValue)
        {
            Value = tokenValue;
        }

        public abstract void Evaluate(Stack<int> operands);
    }

    internal interface IToken
    {
        void Evaluate(Stack<int> operands);
    }

    internal class NumberToken : IToken
    {
        private readonly int _value;

        public NumberToken(string tokenValue)
        {
            _value = int.Parse(tokenValue);
        }

        public void Evaluate(Stack<int> operands) => operands.Push(_value);
    }
}