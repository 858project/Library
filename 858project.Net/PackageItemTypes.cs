using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace project858.Net
{
    /// <summary>
    /// Frame item types
    /// </summary>
    public enum PackageItemTypes : byte
    {
        /// <summary>
        /// Unknown type
        /// </summary>
        Unknown = 0x00,
        /// <summary>
        /// Group witn other items
        /// </summary>
        Group = 0x01,
        /// <summary>
        /// GUID / UUID
        /// </summary>
        Guid = 0x02,
        /// <summary>
        /// String
        /// </summary>
        String = 0x03,
        /// <summary>
        /// Date Time
        /// </summary>
        DateTime = 0x04,
        /// <summary>
        /// Bool
        /// </summary>
        Boolean = 0x05,
        /// <summary>
        /// Enum (UInt32)
        /// </summary>
        Enum = 0x06,
        /// <summary>
        /// Byte
        /// </summary>
        Byte = 0x07,
        /// <summary>
        /// Bytes
        /// </summary>
        Bytes = 0x08,
        /// <summary>
        /// Int 16
        /// </summary>
        Int16 = 0x09,
        /// <summary>
        /// Int 32
        /// </summary>
        Int32 = 0x0A,
        /// <summary>
        /// Int 64
        /// </summary>
        Int64 = 0x0B,
        /// <summary>
        /// UInt 16
        /// </summary>
        UInt16 = 0x0C,
        /// <summary>
        /// UInt 32
        /// </summary>
        UInt32 = 0x0D,
        /// <summary>
        /// UInt 64
        /// </summary>
        UInt64 = 0x0E,
    }
}
