using System;

namespace project858.Models
{
    /// <summary>
    /// Interface which defines generic request
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public interface ITerminalRequestModel<T>
    {
        #region - Properties -
        /// <summary>
        /// Package address (command request address)
        /// </summary>
        UInt16 Address { get; set; }
        /// <summary>
        /// Data to send
        /// </summary>
        T Data { get; set; }
        #endregion
    }
}
