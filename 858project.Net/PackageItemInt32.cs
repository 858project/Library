using System;
 
namespace project858.Net
{
    /// <summary>
    /// Frame item with Int32 value
    /// </summary>
    public sealed class PackageItemInt32 : PackageItemBase<Int32>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public PackageItemInt32(UInt32 address, Int32 value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="data">Byte array</param>
        /// <param name="address">Item address</param>
        public PackageItemInt32(Byte[] data, UInt32 address)
            : base(data, address)
        {
 
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        public override PackageItemTypes ItemType { get { return PackageItemTypes.Int32; } }
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
            return String.Format("[Int32] : 0x{0:X4} = {1}", this.Address, this.Value);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse value from byt array
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Value</returns>
        protected override Int32 InternalParseValue(Byte[] data)
        {
            Int32 result = data[3] << 24 | data[2] << 16 | data[1] << 8 | data[0];
            return result;
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(Int32 value)
        {
            Byte[] result = new Byte[4];
            result[0] = (byte)value;
            result[1] = (Byte)(value >> 8);
            result[2] = (Byte)(value >> 16);
            result[3] = (Byte)(value >> 24);
            return result;
        }
        #endregion
    }
}
