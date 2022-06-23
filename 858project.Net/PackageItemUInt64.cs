using System;

namespace project858.Net
{
    /// <summary>
    /// Frame item with Int64 value
    /// </summary>
    public sealed class PackageItemUInt64 : PackageItemBase<UInt64>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public PackageItemUInt64(UInt32 address, UInt64 value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="data">Byte array</param>
        /// <param name="address">Item address</param>

        public PackageItemUInt64(Byte[] data, UInt32 address)
            : base(data, address)
        {
 
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        public override PackageItemTypes ItemType { get { return PackageItemTypes.UInt64; } }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// This method prints value as string
        /// </summary>
        /// <returns>String value</returns>
        public override String ValueToString()
        {
            return this.Value.ToString();
        }
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return String.Format("[Int64] : 0x{0:X4} = {1}", this.Address, this.Value);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse value from byt array
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Value</returns>
        protected override UInt64 InternalParseValue(Byte[] data)
        {
            return BitConverter.ToUInt64(data, 0);
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(UInt64 value)
        {
            return BitConverter.GetBytes(value);
        }
        #endregion
    }
}
