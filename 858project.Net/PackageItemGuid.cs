using System;
using System.Text;

namespace project858.Net
{
    /// <summary>
    /// Frame item with byte array value
    /// </summary>
    public sealed class PackageItemGuid : PackageItemBase<Guid>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public PackageItemGuid(UInt32 address, Guid value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="data">Byte array</param>
        /// <param name="address">Item address</param>
        public PackageItemGuid(Byte[] data, UInt32 address)
            : base(data, address)
        {
 
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        public override PackageItemTypes ItemType { get { return PackageItemTypes.Guid; } }
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
            return String.Format("[Guid] : 0x{0:X4} = {1}", this.Address, this.Value);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse value from byt array
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Value</returns>
        protected override Guid InternalParseValue(Byte[] data)
        {
            if (data.Length != 32)
            {
                throw new Exception("The data length is incorrect");
            }

            String hashValue = Encoding.ASCII.GetString(data);
            Nullable<Guid> value = hashValue.ToGuidWithoutDash();
            return value.Value;
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(Guid value)
        {
            String hashValue = value.ToStringWithoutDash();
            return Encoding.ASCII.GetBytes(hashValue);
        }
        #endregion
    }
}
