using System;
using project858.ComponentModel.Client;

namespace project858.Net
{
    /// <summary>
    /// Interface ako predpis pre transportnu komunikacnu vrstvu
    /// </summary>
    public interface ITransportClient : IClient
    {
        #region - Properties -
        /// <summary>
        /// (Get) Detekcia pripojenia
        /// </summary>
        Boolean IsConnected { get; }
        #endregion

        #region - Event -
        /// <summary>
        /// Event na oznamenie spustenia spojenia pre vyssu vrstvu
        /// </summary>
        event EventHandler ConnectedEvent;
        /// <summary>
        /// Event na oznamenie ukoncenia spojenia pre vyssu vrstvu
        /// </summary>
        event EventHandler DisconnectedEvent;
        /// <summary>
        /// Event na oznamenue prichodu dat na transportnej vrstve
        /// </summary>
        event DataEventHandler ReceivedDataEvent;
        #endregion

        #region - Public Method -
        /// <summary>
        /// This method execute waiting for data
        /// </summary>
        /// <returns></returns>
        Boolean WaitForData();
        /// <summary>
        /// Zapise _data na komunikacnu linku
        /// </summary>
        /// <returns>True = _data boli uspesne zapisane, False = chyba pri zapise dat</returns>
        Boolean Write(Byte[] data);
        #endregion
    }
}
