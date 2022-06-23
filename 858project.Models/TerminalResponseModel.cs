using project858.Net;
using System;
using System.Collections.Generic;

namespace project858.Models
{
    /// <summary>
    /// Class which defines generic response
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public sealed class TerminalResponseModel<T> : ITerminalResponseModel<T>
    {
        #region - Properties -
        /// <summary>
        /// Package state
        /// </summary>
        public Package.StateTypes State { get; set; }
        /// <summary>
        /// Package address (command response address)
        /// </summary>
        public UInt16 Address { get; set; }
        /// <summary>
        /// Result data
        /// </summary>
        public List<T> Results { get; set; }
        /// <summary>
        /// Validations if State == VALIDATION_ERROR
        /// </summary>
        public List<ValidationModel> Validations { get; set; }
        /// <summary>
        /// Messages if State = MESSAGE
        /// </summary>
        public List<MessageModel> Messages { get; set; }
        #endregion
    }
}
