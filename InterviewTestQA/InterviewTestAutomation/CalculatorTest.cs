using InterviewTestQA.InterviewTestAutomation;

namespace InterviewTestQA
{
    public class CalculatorTest
    {
        private Calculator calculator = new Calculator();
        private int? result;

        [Fact]
        public void AddNumber_Positive()
        {
            result = calculator.Add(5, 3);
            Assert.Equal(8, result);
        }

        [Fact]
        public void Test1()
        public void AddNumber_Negative()
        {
            result = calculator.Add(-5, -3);
            Assert.Equal(-8, result);
        }

        [Fact]
        public void SubTwoNumbers_Positive()
        {
            result = calculator.Subtract(8, 3);
            Assert.Equal(5, result);
        }

        [Fact]
        public void SubTwoNumbers_Negative()
        {
            result = calculator.Subtract(-5, -8);
            Assert.Equal(3, result);
        }

        [Fact]
        public void MulTwoNumbers()
        {
            result = calculator.Multiply(5, 3);
            Assert.Equal(15, result);
        }

        [Fact]
        public void MulTwoNumbers_WithZero()
        {
            result = calculator.Multiply(5, 0);
            Assert.Equal(0, result);
        }

        [Fact]
        public void DivTwoNumbers_ByZero()
        {
            Assert.Throws<ArgumentException>(() => calculator.Divide(5, 0));
        }

        [Fact]
        public void DivTwoNumbers()
        {
            result = calculator.Divide(10, 2);
            Assert.Equal(5, result);
        }

        [Fact]
        public void Square_Positive()
        {
            result = calculator.Square(8);
            Assert.Equal(64, result);
        }

        [Fact]
        public void Square_Negative()
        {
            result = calculator.Square(-8);
            Assert.Equal(64, result);
        }

        [Fact]
        public void SquareRoot()
        {
            result = calculator.SquareRoot(8);
            Assert.Equal(1, result);
        }

        [Fact]
        public void SquareRoot_WithZero()
        {
            Assert.Throws<ArgumentException>(() => calculator.SquareRoot(0));
        }
    }
}