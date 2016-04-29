using System.Linq;
using Xunit;

namespace Commandor.Tests
{
    public class ParseTests
    {
        [Fact]
        public void EmptyInputShouldReturnsEmptyOutput()
        {
            var args = new string[] {};
            var parser = new FloxDc.Commandor.Commandor();

            var result = parser.Parse(args);

            Assert.IsType(typeof(Lookup<string, string>), result);
            Assert.Empty(result);
        }


        [Theory]
        [InlineData("/c", "a1")]
        [InlineData("-c", "a1")]
        [InlineData("--C", "a1")]
        [InlineData("dummy", "-c")]
        public void ShouldReturnsCommandWithZeroOrOneArgument(string key, string value)
        {
            var args = new[] { key, value };
            var parser = new FloxDc.Commandor.Commandor();

            var result = parser.Parse(args);

            Assert.IsType(typeof(Lookup<string, string>), result);
            Assert.Equal(1, result.Count);
            Assert.Equal("c", result.First().Key);
        }


        [Theory]
        [InlineData("--c", "a1", "a2")]
        public void ShouldReturnsCommandWithSeveralArguments(string key, string arg1, string arg2)
        {
            var args = new[] { key, arg1, arg2 };
            var parser = new FloxDc.Commandor.Commandor();

            var result = parser.Parse(args);
            var vals = result["c"];

            Assert.IsType(typeof(Lookup<string, string>), result);
            Assert.Equal(2, vals.Count());
        }


        [Theory]
        [InlineData("--C:a0", "a1", "a2")]
        public void ShouldReturnsCommandWithSeveralArgumentsWhenOneOfItNotSeparatedFromCommand(string key, string arg1, string arg2)
        {
            var args = new[] { key, arg1, arg2 };
            var parser = new FloxDc.Commandor.Commandor();

            var result = parser.Parse(args);
            var vals = result["c"];

            Assert.IsType(typeof(Lookup<string, string>), result);
            Assert.Equal(3, vals.Count());
        }


        [Fact]
        public void ShouldReturnsArgumentsWithoutOuterQuotes()
        {
            var args = new[] { "--C:a0", "\"a1\"", "a2" };
            var parser = new FloxDc.Commandor.Commandor();

            var result = parser.Parse(args);
            var vals = result["c"].ToList();

            Assert.IsType(typeof(Lookup<string, string>), result);
            Assert.Equal(3, vals.Count());
            Assert.Contains("a1", vals);
        }


        [Fact]
        public void ShouldReturnsArgumentsWithoutOuterQuotes1()
        {
            var args = new[] { "/m", "/p", "\"c:\\Projects\\Ming\\Dictionaries\\ГОСТ 7.79-2000 ISO 9.txt\"", "/c", "ru", "/f", "ru", "/t", "en", "/n", "\"ISO9 A\"" };
            var parser = new FloxDc.Commandor.Commandor();

            var result = parser.Parse(args);
            var vals = result["c"].ToList();

            Assert.IsType(typeof(Lookup<string, string>), result);
            Assert.Equal(6, result.Count());
            Assert.Equal(1, vals.Count());
            Assert.Contains("ru", vals);
        }
    }
}