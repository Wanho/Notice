namespace System
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class Custom_CodeAttribute : Attribute
    {
        public string[] Group { get; private set; }
        public int NumericValue { get; private set; }
        public int Order { get; private set; }
        public string Value { get; private set; }

        public Custom_CodeAttribute(string value = null, int order = -1, params string[] group) {  this.Value = value; this.Order = order; this.Group = group; }
        public Custom_CodeAttribute(int value, int order = -1, params string[] group) { this.NumericValue = value;  this.Order = order; this.Group = group; }
        public Custom_CodeAttribute(string[] group) { this.Group = group; this.Order = -1; }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class Custom_CodeDataAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public Custom_CodeDataAttribute(string name, string value) { this.Value = value; this.Name = name; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class Custom_CodeGroupAttribute : Attribute
    {
        public string[] Group { get; private set; }

        public Custom_CodeGroupAttribute(params string[] group) { this.Group = group; }
    }
}
