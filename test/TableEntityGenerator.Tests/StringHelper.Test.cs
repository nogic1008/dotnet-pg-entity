using System;
using Xunit;
using TableEntityGenerator.Core;

namespace TableEntityGenerator.Tests
{
    public class StringHelperTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ToCamelCase_Returns_NullOrEmpty_If_NullOrEmpty(string source)
            => Assert.Equal(source, source.ToCamelCase());

        [Theory]
        [InlineData("normal", "normal")]
        [InlineData("snake_case", "snakeCase")]
        [InlineData("UPPER_SNAKE", "upperSnake")]
        [InlineData("_prefix", "prefix")]
        [InlineData("double__under", "doubleUnder")]
        [InlineData("camelCase", "camelcase")]
        [InlineData("PascalCase", "pascalcase")]
        [InlineData("postfix__", "postfix")]
        public void ToCamelCase_Returns_camelCased_String(string source, string expected)
            => Assert.Equal(expected, source.ToCamelCase());

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ToPascalCase_Returns_NullOrEmpty_If_NullOrEmpty(string source)
            => Assert.Equal(source, source.ToPascalCase());

        [Theory]
        [InlineData("normal", "Normal")]
        [InlineData("snake_case", "SnakeCase")]
        [InlineData("UPPER_SNAKE", "UpperSnake")]
        [InlineData("_prefix", "Prefix")]
        [InlineData("double__under", "DoubleUnder")]
        [InlineData("camelCase", "Camelcase")]
        [InlineData("PascalCase", "Pascalcase")]
        [InlineData("postfix__", "Postfix")]
        public void ToPascalCase_Returns_PascalCased_String(string source, string expected)
            => Assert.Equal(expected, source.ToPascalCase());
    }
}
