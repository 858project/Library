using System;

namespace project858.Net
{
    /// <summary>
    /// Element na definiciu adresy tagu
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class PackageItemAttribute : Attribute
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public PackageItemAttribute()
        {
            this.Type = PackageItemTypes.Unknown;
            this.Address = 0x0000;
        }
        #endregion
 
        #region - Properties -
        /// <summary>
        /// Typ objektu
        /// </summary>
        public PackageItemTypes Type { get; set; }
        /// <summary>
        /// Hodnota adresy
        /// </summary>
        public UInt32 Address { get; set; }
        #endregion
    }
}
