using project858.Net;
using System;
using System.Collections.Generic;

namespace project858.Models
{
    /// <summary>
    /// Interface which defines generic response
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public interface ITerminalResponseModel<T>
    {
        #region - Properties -
        /// <summary>
        /// Package state
        /// </summary>
        Package.StateTypes State { get; set; }
        /// <summary>
        /// Package address (command response address)
        /// </summary>
        UInt16 Address { get; set; }
        /// <summary>
        /// Result data
        /// </summary>
        List<T> Results { get; set; }
        /// <summary>
        /// Validations if State == VALIDATION_ERROR
        /// </summary>
        List<ValidationModel> Validations { get; set; }
        /// <summary>
        /// Messages if State = MESSAGE
        /// </summary>
        List<MessageModel> Messages { get; set; }
        #endregion
    }
}
