using System.Collections.Generic;
using Xunit;
using TableEntityGenerator.Core.Templates;
using TableEntityGenerator.Core.Models;

namespace TableEntityGenerator.Tests.Templates
{
    public class TableEntityTest
    {
        private readonly Dictionary<string, string> _typeDictonary = new Dictionary<string, string>(){
            { "varchar", "string" },
            { "numeric", "decimal" },
            { "integer", "int" },
        };

        [Fact]
        public void TransformText_Returns_CSFile_String()
        {
            const string nameSpace = "Test";
            const string tableDescription = "master table";
            const string tableName = "master_table";
            var pkColumn = new ColumnInfo("id", "integer", true, true, "primary");
            var nameColumn = new ColumnInfo("name", "varchar");
            ITextTemplate template = new TableEntity(_typeDictonary, nameSpace, new TableInfo(
                tableName,
                new[] { pkColumn, nameColumn },
                tableDescription
            ));
            #region Expected String
            string expected =
$@"using System;
using System.Collections.Generic;

namespace {nameSpace}
{{
    /// <summery>
    /// {tableDescription}
    /// </summery>
    public class {tableName.ToPascalCase()}
    {{
        /// <summery>
        /// {pkColumn.Description}
        /// </summery>
        public {_typeDictonary[pkColumn.Type]} {pkColumn.Name.ToPascalCase()} {{ get; }}

        public {_typeDictonary[nameColumn.Type]}? {nameColumn.Name.ToPascalCase()} {{ get; set; }}

        public {tableName.ToPascalCase()}(
            {_typeDictonary[pkColumn.Type]} {pkColumn.Name.ToCamelCase()}
        )
        {{
            {pkColumn.Name.ToPascalCase()} = {pkColumn.Name.ToCamelCase()};
        }}

        public {tableName.ToPascalCase()}(
            {_typeDictonary[pkColumn.Type]} {pkColumn.Name.ToCamelCase()},
            {_typeDictonary[nameColumn.Type]}? {nameColumn.Name.ToCamelCase()}
        )
        {{
            {pkColumn.Name.ToPascalCase()} = {pkColumn.Name.ToCamelCase()};
            {nameColumn.Name.ToPascalCase()} = {nameColumn.Name.ToCamelCase()};
        }}
    }}
}}";
            #endregion
            Assert.Equal(expected, template.TransformText());
        }

        [Theory]
        [InlineData("varchar", true, "string")]
        [InlineData("varchar", false, "string?")]
        [InlineData("numeric", true, "decimal")]
        [InlineData("numeric", false, "decimal?")]
        [InlineData("foo", true, "foo")]
        [InlineData("foo", false, "foo?")]
        public void GetColumnType_Returns_DotNet_TypeName(string dbType, bool required, string outputType)
        {
            var template = new TableEntity(_typeDictonary, "Test", null!);
            Assert.Equal(outputType, template.GetColumnType(new ColumnInfo("", dbType, false, required)));
        }
    }
}