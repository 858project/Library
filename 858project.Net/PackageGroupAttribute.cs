using System;

namespace project858.Net
{
    /// <summary>
    /// Element na definiciu adresy skupiny
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class PackageGroupAttribute : Attribute
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public PackageGroupAttribute()
        {
            this.Address = 0x0000;
        }
        #endregion
 
        #region - Properties -
        /// <summary>
        /// Hodnota adresy
        /// </summary>
        public UInt32 Address { get; set; }
        #endregion
    }
}
