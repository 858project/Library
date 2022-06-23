using System;
using project858.Enums;
using project858.Net;

namespace project858.Models
{
    /// <summary>
    /// Validation model
    /// </summary>
    [PackageGroup(Address = ValidationModel.MODEL)]
    public sealed class ValidationModel
    {
        #region - Constants -
        /// <summary>
        /// PackageGroupItem address for this model
        /// </summary>
        public const UInt16 MODEL = 0x0101;
        /// <summary>
        /// PackageItem address for Id
        /// </summary>
        public const UInt32 ID = 0x0001;
        /// <summary>
        /// PackageItem address for Level
        /// </summary>
        public const UInt32 LEVEL = 0x0002;
        /// <summary>
        /// PackageItem address for Type
        /// </summary>
        public const UInt32 TYPE = 0x0003;
        /// <summary>
        /// PackageItem address for ErrorCode
        /// </summary>
        public const UInt32 ERROR_CODE = 0x0004;
        /// <summary>
        /// PackageItem address for Message
        /// </summary>
        public const UInt32 MESSAGE = 0x0005;
        #endregion

        #region - Properties -
        /// <summary>
        /// Unique id for this item
        /// </summary>
        [PackageItem(Address = ValidationModel.ID, Type = PackageItemTypes.Guid)]
        public Guid Id { get; set; }
        /// <summary>
        /// Validation level
        /// </summary>
        [PackageItem(Address = ValidationModel.LEVEL, Type = PackageItemTypes.Enum)]
        public ValidationLevelTypes Level { get; set; }
        /// <summary>
        /// Validation type
        /// </summary>
        [PackageItem(Address = ValidationModel.TYPE, Type = PackageItemTypes.Enum)]
        public ValidationTypes Type { get; set; }
        /// <summary>
        /// Error code
        /// </summary>
        [PackageItem(Address = ValidationModel.ERROR_CODE, Type = PackageItemTypes.Enum)]
        public ValidationErrorCodeTypes ErrorCode { get; set; }
        /// <summary>
        /// Text message
        /// </summary>
        [PackageItem(Address = ValidationModel.MESSAGE, Type = PackageItemTypes.String)]
        public String Message { get; set; }
        #endregion
    }
}
