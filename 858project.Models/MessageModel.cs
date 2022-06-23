using System;
using project858.Net;

namespace project858.Models
{
    /// <summary>
    /// Message model
    /// </summary>
    [PackageGroup(Address = MessageModel.MODEL)]
    public sealed class MessageModel
    {
        #region - Constants -
        /// <summary>
        /// PackageGroupItem address for this model
        /// </summary>
        public const UInt16 MODEL = 0x0102;
        /// <summary>
        /// PackageItem address for Message
        /// </summary>
        public const UInt32 MESSAGE = 0x0001;
        #endregion

        #region - Properties -
        /// <summary>
        /// Text message
        /// </summary>
        [PackageItem(Address = MessageModel.MESSAGE, Type = PackageItemTypes.String)]
        public String Message { get; set; }
        #endregion
    }
}
