namespace TableEntityGenerator.Core.Models
{
    public class ColumnInfo
    {
        public ColumnInfo(string name, string type, bool isPrimary = false, bool notNull = false, string? description = null)
            => (Name, Type, IsPrimary, NotNull, Description) = (name, type, isPrimary, isPrimary || notNull, description);

        public string Name { get; }

        public string Type { get; }

        public bool IsPrimary { get; }

        public bool NotNull { get; }

        public string? Description { get; }

        public void Deconstruct(out string name, out string type, out bool isPrimary, out bool notNull, out string? description)
        => (name, type, isPrimary, notNull, description) = (Name, Type, IsPrimary, NotNull, Description);
    }
}