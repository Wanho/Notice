namespace System
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CodeAttribute : Attribute
    {
        public string[] Group { get; private set; }
        public int NumericValue { get; private set; }
        public int Order { get; private set; }
        public string Value { get; private set; }

        public CodeAttribute(string value = null, int order = -1, params string[] group) {  this.Value = value; this.Order = order; this.Group = group; }
        public CodeAttribute(int value, int order = -1, params string[] group) { this.NumericValue = value;  this.Order = order; this.Group = group; }
        public CodeAttribute(string[] group) { this.Group = group; this.Order = -1; }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class CodeDataAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public CodeDataAttribute(string name, string value) { this.Value = value; this.Name = name; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CodeGroupAttribute : Attribute
    {
        public string[] Group { get; private set; }

        public CodeGroupAttribute(params string[] group) { this.Group = group; }
    }
}
