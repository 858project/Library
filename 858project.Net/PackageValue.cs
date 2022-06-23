using System;
using System.Collections.Generic;
using System.Text;

namespace project858.Net
{
    internal class PackageValue<ValueType>
    {
        public PackageValue()
        {
        }
        /// <summary>
        /// Item type
        /// </summary>
        public PackageItemTypes ItemType { get; }
        /// <summary>
        /// Generics value
        /// </summary>
        public ValueType Value { get; }
        /// <summary>
        /// Item address
        /// </summary>
        public UInt32 Address { get; }
    }
}
