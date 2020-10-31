using System;
using Shouldly;
using Xunit;

namespace ScaffoldLibrary.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void AddsNumbers()
        {
            var test = new ScaffoldClass();

            test.TestAdd(2, 2).ShouldBe(4);
            test.TestAdd(1, 1).ShouldNotBe(0);
        }

        [Fact]
        public void DividesIntegers()
        {
            var test = new ScaffoldClass();

            test.TestDivide(12, 4).ShouldBe(3);
        }

        [Fact]
        public void ThrowsExceptionWithDivideByZero()
        {
            var test = new ScaffoldClass();

            Should.Throw<DivideByZeroException>(() => test.TestDivide(6, 0));
        }
    }
}
