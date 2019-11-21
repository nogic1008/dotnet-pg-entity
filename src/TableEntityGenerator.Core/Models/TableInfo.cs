using System.Collections.Generic;

namespace TableEntityGenerator.Core.Models
{
    public class TableInfo
    {
        public TableInfo(string name, IEnumerable<ColumnInfo> columns, string? description = null)
        => (Name, Columns, Description) = (name, columns, description);

        public string Name { get; }

        public IEnumerable<ColumnInfo> Columns { get; }

        public string? Description { get; }

        public void Deconstruct(out string name, out IEnumerable<ColumnInfo> columns, out string? description)
        => (name, columns, description) = (Name, Columns, Description);

        public override string ToString()
            => $"{Name} : {Description}";
    }
}