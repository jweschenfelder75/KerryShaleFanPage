using System;

namespace KerryShaleFanPage.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class BackColorNameAttribute : Attribute
    {
        public static readonly BackColorNameAttribute Default = new BackColorNameAttribute();

        public BackColorNameAttribute() 
            : this(string.Empty)
        {
        }

        public BackColorNameAttribute(string name)
        {
            NameValue = name;
        }

        public virtual string Name => NameValue;

        protected string NameValue { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            return (obj is BackColorNameAttribute other) && other.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool IsDefaultAttribute()
        {
            return this.Equals(Default);
        }
    }
}
