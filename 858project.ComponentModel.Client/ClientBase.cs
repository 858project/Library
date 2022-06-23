using System;
using System.ComponentModel;
using project858.Diagnostics;

namespace project858.ComponentModel.Client
{
    /// <summary>
    /// ClientBase / predpis a implementacia zakladnych vlastnosti klienta / modulu
    /// </summary>traceTypes
    public abstract class ClientBase : IClient
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public ClientBase()
        {
        }
        #endregion

        #region - Event -
        /// <summary>
        /// Event oznamujuci uspesny start klienta
        /// </summary>
        private event EventHandler mClientStartEvent = null;
        /// <summary>
        /// Event oznamujuci uspesny start klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event EventHandler ClientStartEvent
        {
            add
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
                    this.mClientStartEvent += value;
            }
            remove
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
                    this.mClientStartEvent -= value;
            }
        }
        /// <summary>
        /// Event oznamujuci uspesne ukoncanie klienta
        /// </summary>
        private event EventHandler mClientStopEvent = null;
        /// <summary>
        /// Event oznamujuci uspesne ukoncanie klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event EventHandler ClientStopEvent
        {
            add
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
                    this.mClientStopEvent += value;
            }
            remove
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
                    this.mClientStopEvent -= value;
            }
        }
        /// <summary>
        /// Event oznamujuci poziadavku na logovanie informacii
        /// </summary>
        private event TraceEventHandler traceEvent = null;
        /// <summary>
        /// Event oznamujuci poziadavku na logovanie informacii
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event TraceEventHandler TraceEvent
        {
            add
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
                    this.traceEvent += value;
            }
            remove
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
                    this.traceEvent -= value;
            }
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get / Set) Definuje ci je event 'TraceEvent' v asynchronnom mode
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean TraceEventAsync
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this.mTraceEventAsync;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                this.mTraceEventAsync = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje typ logovania informacii
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public TraceTypes TraceType
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this.mTraceType;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                this.mTraceType = value;
            }
        }
        /// <summary>
        /// (Get) Definuje ci bolo na tiedu zavolane Dispose()
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool IsDisposed
        {
            get { return this._disposed; }
        }
        /// <summary>
        /// (Get) Urcuje ci je klient spusteny. Ak ano stav klienta je 'start' alebo 'pause'
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool IsRun
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this.mClientState != ClientStates.Stop;
            }
        }
        /// <summary>
        /// (Get) Definuje stav klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public ClientStates ClientState
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return mClientState;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je event 'ClientStart' v asynchronnom mode
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean ClientStartEventAsync
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this._clientStartEventAsync;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                this._clientStartEventAsync = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je event 'ClientStop' v asynchronnom mode
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean ClientStopEventAsync
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this._clientStopEventAsync;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                this._clientStopEventAsync = value;
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Definuje ci je event 'ClientStart' v asynchronnom mode
        /// </summary>
        private Boolean _clientStartEventAsync = false;
        /// <summary>
        /// Definuje ci je event 'ClientStop' v asynchronnom mode
        /// </summary>
        private Boolean _clientStopEventAsync = false;
        /// <summary>
        /// Definuje stav klienta
        /// </summary>
        private ClientStates mClientState = ClientStates.Stop;
        /// <summary>
        /// Track if dispose has been called
        /// </summary>
        private Boolean _disposed = false;
        /// <summary>
        /// Pomocny objekt na synchronizaciu pristupu k eventom
        /// </summary>
        protected readonly Object mEventLock = new Object();
        /// <summary>
        /// Definuje ci je event 'TraceEvent' v asynchronnom mode
        /// </summary>
        private Boolean mTraceEventAsync = false;
        /// <summary>
        /// Typ logovania ktory je nastaveny
        /// </summary>
        private TraceTypes mTraceType = TraceTypes.Off;
        #endregion

        #region - Public Method -
        /// <summary>
        /// Inicializuje klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Ak je klient v stave 'start'
        /// </exception>
        /// <returns>True = start klienta bol uspesny</returns>
        public Boolean Start()
        {
            //je objekt _disposed ?
            if (this._disposed)
                throw new ObjectDisposedException("Object was disposed");

            //ak je klient spusteny
            if (this.mClientState == ClientStates.Start)
                throw new InvalidOperationException(String.Format("{0} is already running.", this));

            try
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Initializing {0}...", this);

                //interny start klienta
                if (!this.InternalStart())
                    return false;

                //prechod do ineho stavu
                this.mClientState = ClientStates.Start;

                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "{0} was initialized successfully", this);

                //event oznamujuci uspesny start klienta
                this.OnClientStart(EventArgs.Empty);

                //uspesny start klienta
                return true;
            }
            catch (Exception ex)
            {
                //trace message
                this.InternalException(ex);

                //chybna
                return false;
            }
        }
        /// <summary>
        /// Ukonci funkciu vrstvy / klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        public void Stop()
        {
            //je objekt _disposed ?
            if (this._disposed)
                throw new ObjectDisposedException("Object was disposed");

            try
            {
                //ak ide o prechod z ineho stavu
                if (this.mClientState != ClientStates.Stop)
                {
                    //zalogujeme
                    this.InternalTrace(TraceTypes.Verbose, "Exiting {0}...", this);

                    //interny stop klienta
                    this.InternalStop();

                    //zmena satvu
                    this.mClientState = ClientStates.Stop;

                    //zalogujeme
                    this.InternalTrace(TraceTypes.Verbose, "{0} was exited successfully", this);

                    //event o ukonceni klienta
                    this.OnClientStop(EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalException(ex);
            }
        }
        /// <summary>
        /// Deinicializuje cely objekt
        /// </summary>
        public void Dispose()
        {
            //internal call
            this.InternalDispose();

            //dispose
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Vrati meno, popis klienta
        /// </summary>
        /// <returns>Popis klienta</returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// This function is raised when client is starting
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Metoda nie je implementovana
        /// </exception>
        /// <returns>NotImplementedException</returns>
        protected virtual Boolean InternalStart()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This function is raised when client is stoping
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Metoda nie je implementovana
        /// </exception>
        protected virtual void InternalStop()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This function is raised when client is disposing
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Metoda nie je implementovana
        /// </exception>
        protected virtual void InternalDispose()
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// Metoda volana vzdy pri internej chybe klienta
        /// </summary>
        /// <param name="exception">Chyba ktora vznikla</param>
        /// <param name="message">Message to trace</param>
        /// <param name="args">Arguments for string.format('message', args)</param>
        protected virtual void InternalException(Exception exception)
        {
			//trace error
            this.InternalTrace(TraceTypes.Error, exception.ToString());
        }
        /// <summary>
        /// Spracuje logovacie spravy. Ak sprava vyhovuje nastaveniam odosle ju vyssej vrstve
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Argument 'message' je null alebo empty
        /// </exception>
        /// <param name="type">Typ spravy</param>
        /// <param name="message">Text spravy</param>
        /// <param name="msgArgs">Dalsie argumenty do String.Format k sprave</param>
        protected void InternalTrace(TraceTypes type, String message, params Object[] msgArgs)
        {
            //preposleme dalej
            this.InternalTrace(DateTime.Now, type, message, msgArgs);
        }
        /// <summary>
        /// Spracuje logovacie spravy. Ak sprava vyhovuje nastaveniam odosle ju na logovanie
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Argument 'message' je null alebo empty
        /// </exception>
        /// <param name="time">Cas kedy sprava vznikla</param>
        /// <param name="type">Typ spravy</param>
        /// <param name="message">Text spravy</param>
        /// <param name="msgArgs">Dalsie argumenty do String.Format k sprave</param>
        protected void InternalTrace(DateTime time, TraceTypes type, String message, params Object[] msgArgs)
        {
            //osetrenie
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            //sk sprava vyhovuje jednymu z typov logovania
            if (((this.mTraceType & TraceTypes.Error) == type) ||
                ((this.mTraceType & TraceTypes.Info) == type) ||
                ((this.mTraceType & TraceTypes.Verbose) == type) ||
                ((this.mTraceType & TraceTypes.Warning) == type) ||
                (this.mTraceType == TraceTypes.Verbose))
            {
                //odosleme spravu na logovanie
                this.OnInternalTrace(time, type, message, msgArgs);
            }
        }
        /// <summary>
        /// Vykona logovanie spravy
        /// </summary>
        /// <param name="time">Cas kedy sprava vznikla</param>
        /// <param name="type">Typ spravy</param>
        /// <param name="message">Text spravy</param>
        /// <param name="msgArgs">Dalsie argumenty do String.Format k sprave</param>
        private void OnInternalTrace(DateTime time, TraceTypes type, String message, params Object[] msgArgs)
        {
            //odosleme spravu
            this.OnTrace(this, new TraceEventArgs(time, this.ToString(), type, String.Format(message, msgArgs)));
        }
        #endregion

        #region - Event Call Method -
        /// <summary>
        /// Vygeneruje event oznamujuci uspesny start klienta
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnClientStart(EventArgs e)
        {
            EventHandler handler = this.mClientStartEvent;

            if (handler != null)
            {
                if (this._clientStartEventAsync)
                    handler.BeginInvoke(this, e, null, null);
                else
                    handler(this, e);
            }
        }
        /// <summary>
        /// Vygeneruje event oznamujuci uspesne ukoncenie klienta
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnClientStop(EventArgs e)
        {
            EventHandler handler = this.mClientStopEvent;

            if (handler != null)
            {
                if (this._clientStopEventAsync)
                    handler.BeginInvoke(this, e, null, null);
                else
                    handler(this, e);
            }
        }
        /// <summary>
        /// Vygeneruje event oznamujuci poziadavku na logovanie informacii
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">TraceEventArgs</param>
        protected virtual void OnTrace(Object sender, TraceEventArgs e)
        {
            TraceEventHandler handler = this.traceEvent;
            if (handler != null)
            {
                if (this.mTraceEventAsync)
                    handler.BeginInvoke(sender, e, null, null);
                else
                    handler(sender, e);
            }
        }
        #endregion

        #region - IDisposable Members -
        /// <summary>
        /// Releases the unmanaged resources used by the Transport object.
        /// </summary>
        private void Dispose(bool disposing)
        {
            // Check recipients see if Dispose has already been called. 
            if (disposing)
            {
                //ak je vrstva spustena
                if (this.IsRun)
                {
                    //ukoncime funkciu vrstvy
                    this.Stop();
                    //
                    //TODO: ukoncenie vrstvy
                    //
                }
            }

            this._disposed = true;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~ClientBase()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
