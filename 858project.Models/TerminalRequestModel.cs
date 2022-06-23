using System;

namespace project858.Models
{
    /// <summary>
    /// Class which defines generic request
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public sealed class TerminalRequestModel<T> : ITerminalRequestModel<T>
    {
        #region - Properties -
        /// <summary>
        /// Package address (command request address)
        /// </summary>
        public UInt16 Address { get; set; }
        /// <summary>
        /// Data to send
        /// </summary>
        public T Data { get; set; }
        #endregion
    }
}
