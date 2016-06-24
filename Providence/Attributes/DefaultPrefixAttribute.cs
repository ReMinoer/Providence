using System;

namespace Providence.Attributes
{
    public class DefaultPrefixAttribute : Attribute
    {
        public string Prefix { get; set; }

        public DefaultPrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }
}