using System;

namespace project858.Net
{
    /// <summary>
    /// Frame item with Byte value
    /// </summary>
    public sealed class PackageItemBoolean : PackageItemBase<Boolean>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public PackageItemBoolean(UInt32 address, Boolean value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="data">Byte array</param>
        public PackageItemBoolean(Byte[] data, UInt32 address)
            : base(data, address)
        {
 
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        public override PackageItemTypes ItemType { get { return PackageItemTypes.Boolean; } }
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
            return String.Format("[Boolean] : 0x{0:X4} = {1}", this.Address, this.Value);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse value from byt array
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Value</returns>
        protected override Boolean InternalParseValue(Byte[] data)
        {
            return data[0] == 0x01;
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(Boolean value)
        {
            Byte[] result = new Byte[] { (byte)(value ? 0x01 : 0x00) };
            return result;
        }
        #endregion
    }
}
