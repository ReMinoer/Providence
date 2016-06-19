using System;

namespace Providence.Attributes
{
    public class IconAttribute : Attribute
    {
        public string IconSource { get; set; }

        public IconAttribute(string iconSource)
        {
            IconSource = iconSource;
        }
    }
}