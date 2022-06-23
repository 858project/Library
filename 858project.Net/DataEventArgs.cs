using System;
using System.Net;

namespace project858.Net
{
    /// <summary>
    /// EventArgs pre event na prichod dat
    /// </summary>
    public class DataEventArgs : EventArgs
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="data">Prijate data</param>
        /// <param name="remoteEndPoint">EndPoint odosielatela dat</param>
        public DataEventArgs(Byte[] data, IPEndPoint remoteEndPoint)
        {
            this.Data = data;
            this.RemoteEndPoint = remoteEndPoint;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get) Prijate _data
        /// </summary>
        public Byte[] Data
        {
            get;
            private set;
        }
        /// <summary>
        /// (Get) End point z ktoreho boli data prijate
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get;
            private set;
        }
        #endregion
    }
}
