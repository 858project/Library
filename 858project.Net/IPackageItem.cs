using System;

namespace project858.Net
{
    /// <summary>
    /// Base frame item from tcp protocol
    /// </summary>
    public interface IPackageItem
    {
        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        PackageItemTypes ItemType { get; }
        /// <summary>
        /// Item address
        /// </summary>
        UInt32 Address { get; }
        /// <summary>
        /// Data item
        /// </summary>
        Byte[] Data { get; }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// this function returns value
        /// </summary>
        /// <returns>Value</returns>
        Object GetValue();
        /// <summary>
        /// This function returns item data
        /// </summary>
        /// <returns>Item array data</returns>
        Byte[] ToByteArray();
        /// <summary>
        /// This method prints value as string
        /// </summary>
        /// <returns>String value</returns>
        String ValueToString();
        #endregion
    }
}
