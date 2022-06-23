using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace project858.Enums
{
    /// <summary>
    /// Validation level
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ValidationLevelTypes : UInt32
    {
        Unknown = 0x00,
	    Warning = 0x01,
	    Error = 0x02
    }
}
