using System;
using System.Collections.Generic;

namespace project858.Net
{
    /// <summary>
    /// Frame item with byte array value
    /// </summary>
    public sealed class PackageItemUnkown : PackageItemBase<List<Byte>>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public PackageItemUnkown(UInt32 address, List<Byte> value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="data">Byte array</param>
        public PackageItemUnkown(Byte[] data, UInt32 address)
            : base(data, address)
        {
 
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        public override PackageItemTypes ItemType { get { return PackageItemTypes.Unknown; } }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// This method prints value as string
        /// </summary>
        /// <returns>String value</returns>
        public override String ValueToString()
        {
            return String.Format("List[{0}]", this.Value.Count);
        }
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return String.Format("[Unkown] : 0x{0:X4} = {1}", this.Address, this.Value.ToHexaString());
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse value from byt array
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Value</returns>
        protected override List<Byte> InternalParseValue(Byte[] data)
        {
            return new List<Byte>(data);
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override byte[] InternalParseFromValue(List<Byte> value)
        {
            return value.ToArray();
        }
        #endregion
    }
}
