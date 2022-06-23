using System;

namespace project858.Net
{
    /// <summary>
    /// Frame item with Int16 value
    /// </summary>
    public sealed class PackageItemUInt16 : PackageItemBase<UInt16>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public PackageItemUInt16(UInt32 address, UInt16 value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="data">Byte array</param>
        /// <param name="address">Item address</param>
        public PackageItemUInt16(Byte[] data, UInt32 address)
            : base(data, address)
        {
 
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        public override PackageItemTypes ItemType { get { return PackageItemTypes.UInt16; } }
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
            return String.Format("[Int16] : 0x{0:X4} = {1}", this.Address, this.Value);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse value from byt array
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Value</returns>
        protected override UInt16 InternalParseValue(Byte[] data)
        {
            UInt16 result = (UInt16)(data[1] << 8 | data[0]);
            return result;
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(UInt16 value)
        {
            Byte[] result = new Byte[2];
            result[0] = (Byte)value;
            result[1] = (Byte)(value >> 8);
            return result;
        }
        #endregion
    }
}
