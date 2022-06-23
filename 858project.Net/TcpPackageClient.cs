using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using project858.Diagnostics;

namespace project858.Net
{
    /// <summary>
    /// Implement TCP client with custom protocol
    /// </summary>
    public class TcpProtocolClient : TcpTransportClient
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Neinicializovana IPAddress
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybny argument / rozsah IP portu
        /// </exception>
        /// <param name="ipEndPoint">IpPoint ku ktoremu sa chceme pripojit</param>
        public TcpProtocolClient(IPEndPoint ipEndPoint)
            : base(ipEndPoint)
        {

        }
        /// <summary>
		/// Initialize this class
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Neinicializovana IPAddress
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Chybny argument jedneho z timeoutov, alebo IP portu
		/// </exception>
        /// <param name="ipEndPoint">IpPoint ku ktoremu sa chceme pripojit</param>
		/// <param name="tcpReadTimeout">TcpClient Read Timeout</param>
		/// <param name="tcpWriteTimeout">TcpClient Write Timeout</param>
		/// <param name="nsReadTimeout">NetworkStream Read Timeout</param>
		/// <param name="nsWriteTimeout">NetworkStream Write Timeout</param>
        public TcpProtocolClient(IPEndPoint ipEndPoint, Int32 tcpReadTimeout, Int32 tcpWriteTimeout, Int32 nsReadTimeout, Int32 nsWriteTimeout)
            : base(ipEndPoint, tcpReadTimeout, tcpWriteTimeout, nsReadTimeout, nsWriteTimeout)
        {

        }
        /// <summary>
		/// Initialize this class
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Neinicializovany TcpClient
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Chybny argument jedneho z timeoutov
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		/// Ak je RemoteEndPoint v tcpClientovi Disposed
		/// </exception>
		/// <param name="tcpClient">Client zabezpecujuci tcp komunikaciu</param>
		public TcpProtocolClient(TcpClient tcpClient)
			: base(tcpClient)
		{
		}
		/// <summary>
		/// Initialize this class
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Neinicializovany TcpClient
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Chybny argument jedneho z timeoutov
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		/// Ak je RemoteEndPoint v tcpClientovi Disposed
		/// </exception>
		/// <param name="tcpClient">Client zabezpecujuci tcp komunikaciu</param>
		/// <param name="tcpReadTimeout">TcpClient Read Timeout</param>
		/// <param name="tcpWriteTimeout">TcpClient Write Timeout</param>
		/// <param name="nsReadTimeout">NetworkStream Read Timeout</param>
		/// <param name="nsWriteTimeout">NetworkStream Write Timeout</param>
        public TcpProtocolClient(TcpClient tcpClient, Int32 tcpReadTimeout, Int32 tcpWriteTimeout, Int32 nsReadTimeout, Int32 nsWriteTimeout)
            : base(tcpClient, tcpReadTimeout, tcpWriteTimeout, nsReadTimeout, nsWriteTimeout)
		{

		}
        #endregion

        #region - Event -
        /// <summary>
        /// Event na oznamenue prichodu frame na transportnej vrstve
        /// </summary>
        private event PackageEventHandler mReceivedPackageEvent = null;
        /// <summary>
        /// Event na oznamenue prichodu frame na transportnej vrstve
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        public event PackageEventHandler ReceivedPackageEvent
        {
            add
            {

                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
                    this.mReceivedPackageEvent += value;
            }
            remove
            {

                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                //netusim preco ale to mi zvykne zamrznut thread a ja fakt neviem preco
                lock (this.mEventLock)
                    this.mReceivedPackageEvent -= value;
            }
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Synchronization object
        /// </summary>
        private readonly Object mLockObject = new Object();
        /// <summary>
        /// Buffer collection for processing data
        /// </summary>
        private List<Byte> mBuffer = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// This function sends frame to transport layer
        /// </summary>
        /// <param name="package">Frame to send</param>
        /// <returns>True | false</returns>
        public virtual Boolean Send(Package package)
        {
            return this.Write(package.ToByteArray());
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Vytvori asynchronne volanie na metodu zabezpecujucu vytvorenie eventu
        /// oznamujuceho prijatie dat
        /// </summary>
        /// <param name="e">DataEventArgs</param>
        protected override void OnReceivedData(DataEventArgs e)
        {
            //base event
            base.OnReceivedData(e);

            //lock
            lock (this.mLockObject)
            {
                //check buffer
                if (this.mBuffer == null)
                {
                    this.mBuffer = new List<Byte>();
                }

                //zalogujeme prijate dat
                this.InternalTrace(TraceTypes.Verbose, "Receiving data: [{0}]", e.Data.ToHexaString());

                //add data to buffer
                this.mBuffer.AddRange(e.Data);

                //loop
                while (true)
                {
                    //find frame
                    Package frame = PackageHelper.FindPackage(this.mBuffer);
                    if (frame != null)
                    {
                        //send receive event
                        this.OnReceivedPackage(new PackageEventArgs(frame, e.RemoteEndPoint));
                    }
                    else
                    {
                        //any data for frame
                        break;
                    }
                }
            }
        }
        #endregion

        #region - Call Event Method -
        /// <summary>
        /// Vytvori asynchronne volanie na metodu zabezpecujucu vytvorenie eventu
        /// oznamujuceho prijatie dat
        /// </summary>
        /// <param name="e">EventArgs obsahujuci data</param>
        protected virtual void OnReceivedPackage(PackageEventArgs e)
        {
            PackageEventHandler handler = this.mReceivedPackageEvent;

            if (handler != null)
                handler(this, e);
        }
        #endregion
    }
}
