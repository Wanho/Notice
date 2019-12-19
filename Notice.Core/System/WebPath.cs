namespace System
{
    public class WebPath
    {
        public string AbsolutePath { get; internal set; }
        public string Path { get; internal set; }

        internal WebPath() { }

        public override string ToString()
        {
            return this.AbsolutePath;
        }
    }
}
