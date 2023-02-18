using System;

namespace KerryShaleFanPage.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class FrontColorNameAttribute : Attribute
    {
        public static readonly FrontColorNameAttribute Default = new FrontColorNameAttribute();

        private string _name;

        public FrontColorNameAttribute()
            : this(string.Empty)
        {
        }

        public FrontColorNameAttribute(string name)
        {
            _name = name;
        }

        public virtual string Name => NameValue;

        protected string NameValue
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            return (obj is FrontColorNameAttribute other) && other.Name == Name;
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
