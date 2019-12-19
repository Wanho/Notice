namespace System
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class Custom_FileRootAttribute : Attribute
    {
        public string Name { get; private set; }

        public Custom_FileRootAttribute(string rootName)
        {
            this.Name = rootName;
        }
    }
}
