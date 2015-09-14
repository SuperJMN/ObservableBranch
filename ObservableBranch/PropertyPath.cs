namespace ObservableBranch
{
    using System.Collections.Generic;

    public class PropertyPath
    {
        private readonly string[] parts;

        public PropertyPath(string str)
        {
            parts = str.Split('.');
        }

        public IEnumerable<string> Parts => parts;

        public int PartCount => parts.Length;
    }
}