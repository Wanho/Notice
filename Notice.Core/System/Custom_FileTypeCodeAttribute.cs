namespace System
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class Custom_CodeFileTypeAttribute : Attribute
    {
        public string DateFormat { get; set; }
        public string DirectoryName { get; set; }
        public bool Private { get; set; }

        public Custom_CodeFileTypeAttribute(string dateFormat)
        {
            this.DateFormat = dateFormat;
        }

        public Custom_CodeFileTypeAttribute(string dateFormat, string directoryName)
        {
            this.DateFormat = dateFormat;
            this.DirectoryName = directoryName;
        }

        public Custom_CodeFileTypeAttribute(string dateFormat, string directoryName, bool isPrivate)
        {
            this.DateFormat = dateFormat;
            this.DirectoryName = directoryName;
            this.Private = isPrivate;
        }
    }
}


