using System;
using System.Net;
using System.Net.Sockets;
using project858.ComponentModel.Client;
using project858.Diagnostics;

namespace project858.Net
{
    /// <summary>
    /// Zakladna komunikacna vrstva, pracujuca na linkovej urovni TCP
    /// </summary>
    public class TcpTransportClient : ClientBase, ITransportClient
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
        public TcpTransportClient(IPEndPoint ipEndPoint)
            : this(ipEndPoint, 1000, 1000, 1000, 300)
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
        /// <param name="ipEndPoint">Kolekcia IpEndPointov ku ktory sa bude klient striedavo pripajat pri autoreconnecte</param>
        /// <param name="tcpReadTimeout">TcpClient Read Timeout</param>
        /// <param name="tcpWriteTimeout">TcpClient Write Timeout</param>
        /// <param name="nsReadTimeout">NetworkStream Read Timeout</param>
        /// <param name="nsWriteTimeout">NetworkStream Write Timeout</param>
        public TcpTransportClient(IPEndPoint ipEndPoint, Int32 tcpReadTimeout, Int32 tcpWriteTimeout, Int32 nsReadTimeout, Int32 nsWriteTimeout)
		{
            //osetrime vstup
            if (ipEndPoint == null)
                throw new ArgumentNullException("ipEndPoints");
			if (tcpReadTimeout < -1)
				throw new ArgumentOutOfRangeException("tcpReadTimeout must be >= -1");
			if (tcpWriteTimeout < -1)
				throw new ArgumentOutOfRangeException("tcpWriteTimeout must be >= -1");
			if (nsReadTimeout < -1)
				throw new ArgumentOutOfRangeException("nsReadTimeout must be >= -1");
			if (nsWriteTimeout < -1)
				throw new ArgumentOutOfRangeException("nsWriteTimeout must be >= -1");

			//nastavime timeoty
			this.m_tcpReadTimeout = tcpReadTimeout;
			this.m_tcpWriteTimeout = tcpWriteTimeout;
			this.m_nsReadTimeout = nsReadTimeout;
			this.m_nsWriteTimeout = nsWriteTimeout;
			this.mIpEndPoint = ipEndPoint;
            this.mTcpClient = null;
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
		public TcpTransportClient(TcpClient tcpClient)
			: this(tcpClient, 1000, 1000, 1000, 300)
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
		public TcpTransportClient(TcpClient tcpClient, Int32 tcpReadTimeout, Int32 tcpWriteTimeout, Int32 nsReadTimeout, Int32 nsWriteTimeout)
		{
			if (tcpClient == null)
				throw new ArgumentNullException("tcpClient", "Value cannot be null.");
			if (tcpReadTimeout < -1)
				throw new ArgumentOutOfRangeException("tcpReadTimeout must be >= -1");
			if (tcpWriteTimeout < -1)
				throw new ArgumentOutOfRangeException("tcpWriteTimeout must be >= -1");
			if (nsReadTimeout < -1)
				throw new ArgumentOutOfRangeException("nsReadTimeout must be >= -1");
			if (nsWriteTimeout < -1)
				throw new ArgumentOutOfRangeException("nsWriteTimeout must be >= -1");

			//nastavime timeoty
			this.m_tcpReadTimeout = tcpReadTimeout;
			this.m_tcpWriteTimeout = tcpWriteTimeout;
			this.m_nsReadTimeout = nsReadTimeout;
			this.m_nsWriteTimeout = nsWriteTimeout;
			this.mTcpClient = tcpClient;
			this.mIpEndPoint = null;
		}
		#endregion

		#region - State Class -
		/// <summary>
		/// pomocna trieda na event na streame
		/// </summary>
		private class SocketState
		{
			#region - Constructor -
			/// <summary>
			/// Initialize this class
			/// </summary>
			public SocketState()
			{
				this.Data = new byte[65536];
			}
			#endregion

			#region - Properties -
            /// <summary>
            /// Klient cez ktoreho prebieha komunikacia
            /// </summary>
            public TcpClient Client { get; set; }
			/// <summary>
			/// Buffer na citanie _data
			/// </summary>
			public Byte[] Data { get; set; }
            /// <summary>
            /// Stream cez ktory sa komunikuje
            /// </summary>
            public NetworkStream Stream { get; set; }
			#endregion
		}
		#endregion

		#region - Event -
		/// <summary>
		/// Event na oznamenie spustenia spojenia pre vyssu vrstvu
		/// </summary>
		private event EventHandler mConnectedEvent = null;
		/// <summary>
		/// Event na oznamenie spustenia spojenia pre vyssu vrstvu
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		public event EventHandler ConnectedEvent
		{
			add
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

				lock (this.mEventLock)
					this.mConnectedEvent += value;
			}
			remove
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
					this.mConnectedEvent -= value;
			}
		}
		/// <summary>
		/// Event na oznamenie ukoncenia spojenia pre vyssu vrstvu
		/// </summary>
		private event EventHandler mDisconnectedEvent = null;
		/// <summary>
		/// Event na oznamenie ukoncenia spojenia pre vyssu vrstvu
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		public event EventHandler DisconnectedEvent
		{
			add
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
					this.mDisconnectedEvent += value;
			}
			remove
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
					this.mDisconnectedEvent -= value;
			}
		}
		/// <summary>
		/// Event na oznamenue prichodu dat na transportnej vrstve
		/// </summary>
		private event DataEventHandler mReceivedDataEvent = null;
		/// <summary>
		/// Event na oznamenue prichodu dat na transportnej vrstve
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
        public event DataEventHandler ReceivedDataEvent
		{
			add
			{

				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

				lock (this.mEventLock)
					this.mReceivedDataEvent += value;
			}
			remove
			{

				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

				//netusim preco ale to mi zvykne zamrznut thread a ja fakt neviem preco
                lock (this.mEventLock)
					this.mReceivedDataEvent -= value;
			}
		}
        /// <summary>
        /// Event oznamujuci zmenu stavu pripojenia
        /// </summary>
        private event EventHandler mChangeConnectionStateEvent = null;
        /// <summary>
        /// Event oznamujuci zmenu stavu pripojenia
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Ak nie je autoreconnect povoleny
        /// </exception>
        public event EventHandler ChangeConnectionStateEvent
        {
            add
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
                    this.mChangeConnectionStateEvent += value;
            }
            remove
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");
 
                lock (this.mEventLock)
                    this.mChangeConnectionStateEvent -= value;
            }
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get) Detekcia pripojenia
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        public Boolean IsConnected
		{
			get
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

				return this.mIsConnectied;
			}
		}
		#endregion

		#region - Variable -
		/// <summary>
		/// Stream na komunikaciu
		/// </summary>
		private NetworkStream mNetworkStream = null;
		/// <summary>
		/// TcpClient Read Timeout
		/// </summary>
		private Int32 m_tcpReadTimeout = 0;
		/// <summary>
		/// TcpClient Write Timeout
		/// </summary>
		private Int32 m_tcpWriteTimeout = 0;
		/// <summary>
		/// NetworkStream Read Timeout
		/// </summary>
		private Int32 m_nsReadTimeout = 0;
		/// <summary>
		/// NetworkStream Write Timeout
		/// </summary>
		private Int32 m_nsWriteTimeout = 0;
		/// <summary>
		/// detekcia pripojenia
		/// </summary>
		private volatile Boolean mIsConnectied = false;
		/// <summary>
		/// Client na pripojenie cez TCP/IP
		/// </summary>
		private TcpClient mTcpClient;
		/// <summary>
		/// pristupovy bod cez ktory sme pripojeny
		/// </summary>
		private IPEndPoint mIpEndPoint = null;
		#endregion

		#region - Public Method -
        /// <summary>
        /// Vrati meno, popis klienta
        /// </summary>
        /// <returns>Popis klienta</returns>
        public override string ToString()
        {
            if (this.mIpEndPoint == null)
            {
                return base.ToString();
            }
            return String.Format("{0}_[{1}]", base.ToString(), this.mIpEndPoint.ToString().Replace(":", "_"));
        }
        /// <summary>
        /// This function reads data from stream
        /// </summary>
        /// <param name="buffer">Buffer to insert data</param>
        /// <param name="size">Max size for buffer</param>
        /// <param name="result">Result, data count from stream</param>
        /// <returns>True | false</returns>
        public Boolean Read(Byte[] buffer, int size, out int result)
        {
            //je objekt _isDisposed ?
            if (this.IsDisposed)
                throw new ObjectDisposedException("Object was disposed");

            //otvorenie nie je mozne ak je connection == true
            if (!this.IsConnected)
                throw new InvalidOperationException("Writing data is not possible! The client is not connected!");

            //reset value
            result = 0x00;

            try
            {
                //zapiseme data
                result = this.mNetworkStream.Read(buffer, 0x00, size);

                //successfully
                return true;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalException(ex);

                //ukoncime klienta
                this.InternalStop();

                //chybne ukoncenie metody
                return false;
            }
        }
        /// <summary>
        /// Zapise _data na komunikacnu linku
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Vynimka v pripade ze sa snazime zapisat _data, ale spojenie nie je.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        /// <returns>True = _data boli uspesne zapisane, False = chyba pri zapise dat</returns>
        public Boolean Write(Byte[] data)
		{
			//je objekt _isDisposed ?
			if (this.IsDisposed)
				throw new ObjectDisposedException("Object was disposed");

			//otvorenie nie je mozne ak je connection == true
			if (!this.IsConnected)
                throw new InvalidOperationException("Writing data is not possible! The client is not connected!");

			try
			{
                //zalogujeme prijate dat
 //               this.InternalTrace(TraceTypes.Verbose, "Sending data: [{0}]", data.ToHexaString());

				//zapiseme data
				this.mNetworkStream.Write(data, 0, data.Length);
				this.mNetworkStream.Flush();

                //zalogujeme prijate dat
                this.InternalTrace(TraceTypes.Verbose, "Sending the data has been successfully");

				//uspesne ukoncenie metody
				return true;
			}
			catch (Exception ex)
			{
				//zalogujeme
                this.InternalException(ex);

				//ukoncime klienta
				this.InternalStop();

				//chybne ukoncenie metody
				return false;
			}
		}
        /// <summary>
        /// This method execute waiting for data
        /// </summary>
        /// <returns></returns>
        public Boolean WaitForData()
        {
            try
            {
                // trace
                this.InternalTrace(TraceTypes.Verbose, "Executing wait for data...");

                //inicializujeme state
                SocketState state = new SocketState()
                {
                    Stream = this.mNetworkStream,
                    Client = this.mTcpClient
                };

                //otvorime asynchronne citanie na streame
                this.mNetworkStream.BeginRead(state.Data, 0, state.Data.Length, this.InternalTcpDataReceived, state);

                // successfully
                return true;
            }
            catch (Exception ex)
            {
                // trace error
                this.InternalException(ex);

                // end with error
                return false;
            }
        }
        #endregion

        #region - Protected and Private Method -
        /// <summary>
        /// Podla inicializacie otvori TCP spojenie na pripojeneho clienta
        /// </summary>
        /// <returns>True = uspesne otvorene spojenie, False = chyba pri otvarani spojenia</returns>
        protected override bool InternalStart()
		{
            try
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Info, "Initializing socket...");

                //ak inicializujeme triedu na zaklade IP a portu
                if (this.mTcpClient == null)
                {
                    //inicializujeme a pripojime klienta
                    this.mTcpClient = new TcpClient();
                    this.mTcpClient.Connect(this.mIpEndPoint);
                }
                else
                {
                    //adresa ku ktorej som pripojeny
                    this.mIpEndPoint = (IPEndPoint)this.mTcpClient.Client.RemoteEndPoint;
                }

                //nastavime timeoty pre socket
                this.mTcpClient.ReceiveTimeout = this.m_tcpReadTimeout;
                this.mTcpClient.SendTimeout = this.m_tcpWriteTimeout;

                //ziskame komunikacny stream
                this.mNetworkStream = mTcpClient.GetStream();
                this.mNetworkStream.ReadTimeout = this.m_nsReadTimeout;
                this.mNetworkStream.WriteTimeout = this.m_nsWriteTimeout;

                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Initialization socket was successful");

                //nastavime detekciu spjenia
                this.mIsConnectied = true;

                // event oznamujuci zmenu stavu
                this.OnChangeConnectionState(EventArgs.Empty);

                // client is connected
                this.OnConnected(EventArgs.Empty);

                //successfully
                return true;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalException(ex);

                //nastavime detekciu spjenia
                this.mIsConnectied = false;
                this.mTcpClient = null;
                this.mNetworkStream = null;

                //preposleme vynimku
                return false;
            }
        }
		/// <summary>
		/// Zatvori spojenie
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		/// <returns>True = uspesne zatvorene spojenie, False = chyba pri zatvarani spojenia</returns>
        protected override void InternalStop()
        {
            try
            {
                // check connection
                if (this.IsConnected)
                {
                    // execute disconnect
                    this.InternalDisconnect();
                }
            }
            catch (Exception)
            {
                //preposleme vynimku vyssie
                throw;
            }
            finally
            {
                //nova inicializaia / deinicializacia premennych
                this.mIsConnectied = false;
                this.mTcpClient = null;
                this.mNetworkStream = null;
            }
        }
        /// <summary>
        /// This method executes disconnect for this client
        /// </summary>
        private void InternalDisconnect()
        {
            //ukoncime stream ak je vytvoreny
            if (this.mNetworkStream != null)
            {
                this.mNetworkStream.Close();
                this.mNetworkStream = null;
            }

            //ukoncime tcp spojenie
            if (this.mTcpClient != null)
            {
                this.mTcpClient.Close();
                this.mTcpClient = null;
            }

            //spojenie bolo ukoncene
            this.mIsConnectied = false;

            // event oznamujuci zmenu stavu
            this.OnChangeConnectionState(EventArgs.Empty);

            // client is disconnected
            this.OnDisconnected(EventArgs.Empty);
        }
		/// <summary>
		/// Calback / prichod dat an streame
		/// </summary>
		/// <param name="ar">IAsyncResult</param>
		private void InternalTcpDataReceived(IAsyncResult ar)
		{
            try
            {
                this.InternalDataReceived(ar);
            }
            catch (Exception ex)
            {
                //trace message
                this.InternalException(ex);
            }
		}
		/// <summary>
		/// Calback / prichod dat an streame
		/// </summary>
		/// <param name="ar">IAsyncResult</param>
		private void InternalDataReceived(IAsyncResult ar)
		{
            try
            {
                lock (this)
                {
                    //inicializacia
                    SocketState state = (SocketState)ar.AsyncState;
                    NetworkStream stream = state.Stream;
                    Int32 r = 0;

                    // kontrola ci mozme zo streamu citat
                    if (!stream.CanRead)
                        return;

                    try
                    {
                        //prerusime asynchronne citanie
                        r = stream.EndRead(ar);

                        //ak neboli nacitane ziadne _data asi ide o pad spojenia
                        if (r == 0)
                        {
                            //niekedy dochadza k oneskoreniu vlakna zo streamu ktory oznamuje pad spojenia
                            if (this.IsDisposed == false)
                            {
                                // trace, disconnect
                                this.InternalTrace(TraceTypes.Error, "Loss connection with the remote end point!");

                                // disconnect
                                this.InternalDisconnect();
                            }

                            //nebolo by vhodne nahodit t stav disconnected ???
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        // trace, disconnect
                        this.InternalException(ex);
                        this.InternalStop();
                        return;
                    }

                    //skopirujeme pole bytov
                    Byte[] rdata = new Byte[r];
                    Buffer.BlockCopy(state.Data, 0, rdata, 0, r);

                    //zalogujeme prijate dat
                    //this.InternalTrace(TraceTypes.Verbose, "Received _data: [{0}]", BitConverter.ToString(rdata));
                    this.InternalTrace(TraceTypes.Verbose, "Received data: [{0}]", rdata.Length);

                    //_data su akeptovane len ak sme pripojeny
                    if (this.IsConnected)
                    {
                        //vytvorime udalost a posleme data
                        this.OnReceivedData(new DataEventArgs(rdata, state.Client.Client.RemoteEndPoint as IPEndPoint));
                    }

                    try
                    {
                        //otvorime asynchronne citanie na streame
                        stream.BeginRead(state.Data, 0, state.Data.Length, InternalTcpDataReceived, state);
                    }
                    catch (Exception ex)
                    {
                        // trace
                        this.InternalTrace(TraceTypes.Error, "Chyba pri opatovnom spusteni asynchronneho citania zo streamu. {0}", ex.Message);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                this.InternalException(ex);
                throw;
            }
		}
		#endregion

		#region - Call Event Method -
		/// <summary>
		/// Vytvori asynchronne volanie na metodu zabezpecujucu vytvorenie eventu
		/// oznamujuceho prijatie dat
		/// </summary>
		/// <param name="e">EventArgs obsahujuci data</param>
		protected virtual void OnReceivedData(DataEventArgs e)
		{
			DataEventHandler handler = this.mReceivedDataEvent;
			if (handler != null)
				handler(this, e);
		}
		/// <summary>
		/// Vytvori asynchronne volanie na metodu zabezpecujucu vytvorenie eventu
		/// oznamujuceho pripojenie
		/// </summary>
		/// <param name="e">EventArgs</param>
		protected virtual void OnConnected(EventArgs e)
		{
			EventHandler handler = this.mConnectedEvent;
			if (handler != null)
				handler(this, e);
		}
		/// <summary>
		/// Vytvori asynchronne volanie na metodu zabezpecujucu vytvorenie eventu
		/// oznamujuceho pad spojenia
		/// </summary>
		/// <param name="e">EventArgs</param>
		protected virtual void OnDisconnected(EventArgs e)
		{
			EventHandler handler = this.mDisconnectedEvent;
			if (handler != null)
				handler(this, e);
		}
        /// <summary>
        /// Vygeneruje event oznamujui zmenu stavu pripojenia
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnChangeConnectionState(EventArgs e)
        {
            //ziskame pristup
            EventHandler handler = this.mChangeConnectionStateEvent;

            //vyvolame event
            if (handler != null)
                handler(this, e);
        }
        #endregion
    }
}
