using System;

namespace project858.Net
{
    /// <summary>
    /// Frame item with Int64 value
    /// </summary>
    public sealed class PackageItemInt64 : PackageItemBase<Int64>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public PackageItemInt64(UInt32 address, Int64 value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="data">Byte array</param>
        /// <param name="address">Item address</param>
        public PackageItemInt64(Byte[] data, UInt32 address)
            : base(data, address)
        {
 
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        public override PackageItemTypes ItemType { get { return PackageItemTypes.Int64; } }
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
        protected override Int64 InternalParseValue(Byte[] data)
        {
            return BitConverter.ToInt64(data, 0);
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(Int64 value)
        {
            return BitConverter.GetBytes(value);
        }
        #endregion
    }
}
