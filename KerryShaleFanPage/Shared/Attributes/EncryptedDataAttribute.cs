using System;

namespace KerryShaleFanPage.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    public class EncryptedDataAttribute : Attribute
    {
        public EncryptedDataAttribute() 
        {
        }
    }
}
