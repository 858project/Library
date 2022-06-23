using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace project858.Enums
{
    /// <summary>
    /// Validation type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ValidationTypes : UInt32
    {
        Unknown = 0x00,
	    Model = 0x01,
	    Operation = 0x02
    }
}
