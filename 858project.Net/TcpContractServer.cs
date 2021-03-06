using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using project858.Diagnostics;

namespace project858.Net
{
    /// <summary>
    /// Nadstavba TCP servera s rozsirenim
    /// </summary>
    public abstract class TcpContractServer : TcpServer
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public TcpContractServer()
            : base()
        {
        }
        /// <summary>
		/// Initialize this class
		/// </summary>
		/// <param name="ipAddress">Ip adresa servera</param>
		/// <param name="ipPort">Ip port servera</param>
        public TcpContractServer(IPAddress ipAddress, Int32 ipPort)
            : base(ipAddress, ipPort)
        {
        }
        #endregion

        #region - Events -
        /// <summary>
        /// Event with transport client which was added
        /// </summary>
        private event TransportClientEventHandler mTransportClientAdded = null;
        /// <summary>
        /// Event with transport client which was added
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event TransportClientEventHandler TransportClientAdded
        {
            add
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
                    this.mTransportClientAdded += value;
            }
            remove
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
                    this.mTransportClientAdded -= value;
            }
        }
        /// <summary>
        /// Event with transport client which was removed
        /// </summary>
        private event TransportClientEventHandler mTransportClientRemoved = null;
        /// <summary>
        /// Event with transport client which was removed
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event TransportClientEventHandler TransportClientRemoved
        {
            add
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
                    this.mTransportClientRemoved += value;
            }
            remove
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.mEventLock)
                    this.mTransportClientRemoved -= value;
            }
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get) Kolekcia aktualne pripojenych klientov a ich obsluh
        /// </summary>
        public List<ITransportClient> Contracts
        {
            get { return mContracts; }
        }
        /// <summary>
        /// (Get / Set) Maximalny pocet pripojenych klientov. Povoleny rozsah je od 1 do 100
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Mimo povoleneho rozsahu. Povoleny rozsah je od 1 do 100
        /// </exception>
        public Int32 IsConnected
        {
            get
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this.mMaxConnection;
            }
            set
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                //osetrenie rozsahu
                if (value < 1 || value > 100)
                    throw new ArgumentOutOfRangeException("value");

                //nastavime hodnotu
                this.mMaxConnection = value;
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Maximalny pocet pripojenych klientov
        /// </summary>
        private Int32 mMaxConnection = 10;
        /// <summary>
        /// Kolekcia aktualne pripojenych klientov a ich obsluh
        /// </summary>
        private List<ITransportClient> mContracts = null;
        #endregion

        #region - Protected and Private Method -
        /// <summary>
        /// Inicializuje server na pzoadovanej adrese a porte
        /// </summary>
        /// <returns>True = start klienta bol uspesny</returns>
        protected override bool InternalStart()
        {
            try
            {
                //spustime tcp server
                if (!base.InternalStart())
                {
                    //start servera sa nepodaril
                    return false;
                }

                //inicializujeme
                this.mContracts = new List<ITransportClient>();

                //start servera bol uspesny
                return true;
            }
            catch (Exception ex)
            {
                this.InternalException(ex);
                throw;
            }
        }
        /// <summary>
        /// Ukonci server a akceptovanie klientov
        /// </summary>
        protected override void InternalStop()
        {
            try
            {
                //ukoncime base server
                base.InternalStop();

                //ukoncime contracty
                this.InternalStopAllContract();
            }
            catch (Exception ex)
            {
                this.InternalException(ex);
                throw;
            }
            finally
            {
                //deinicializujeme
                this.mContracts.Clear();
                this.mContracts = null;
            }
        }
        /// <summary>
        /// Inicializuje, spusti a vrati contract na obsluhu akceptovaneho klienta
        /// </summary>
        /// <param name="client">Client ktory bol akceptovany</param>
        /// <returns>Contract na obsluhu akceptovaneho klienta</returns>
        protected virtual ITransportClient InternalCreateContract(TcpClient client)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Odoberie contract klienta ktory sa odpojil. Prepis metody by mal zabezpecit volanie base.InternalRemoveContract(ITransportClient contrat);
        /// </summary>
        /// <param name="contrat">Contract ktory vykonaval obsluhu klienta ktory zrusil spojenie</param>
        protected virtual void InternalRemoveContract(ITransportClient contrat)
        {
            try
            {
                //odstranime klienta
                this.InternalRemoveSpecifiedContract(contrat);
            }
            catch (Exception ex)
            {
                this.InternalException(ex);
                throw;
            }
        }
        /// <summary>
        /// Ukonci vsetky contracty pred ukoncim celeho servera
        /// </summary>
        protected virtual void InternalStopAllContract()
        {
            try
            {
                //ukoncime beziace contracty
                lock (this)
                {
                    for (int i = this.mContracts.Count - 1; i > -1; i--)
                    {
                        try
                        {
                            //ziskame pristup
                            ITransportClient contract = this.mContracts[i];

                            //ukoncime contract
                            this.InternalStopContract(contract);

                            //remove contract
                            this.InternalRemoveContract(contract);
                        }
                        catch (Exception)
                        {
                            //chybu ignorujeme
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.InternalException(ex);
                throw;
            }
        }
        /// <summary>
        /// Ukonci contract pred ukoncenim celeho servera
        /// </summary>
        /// <param name="contract">Contract ktory chceme ukoncit</param>
        protected virtual void InternalStopContract(ITransportClient contract)
        {
            try
            {
                //ukoncime klienta
                if (!contract.IsDisposed)
                    if (contract.IsRun)
                    {
                        //odoberieme event oznamujuci odhlasenie klienta
                        contract.DisconnectedEvent -= new EventHandler(contract_DisconnectedEvent);
                        contract.Stop();
                    }
            }
            catch (Exception ex)
            {
                this.InternalException(ex);
                throw;
            }
        }
        /// <summary>
        /// Prepis a rozsirenie prijatia / akceptovania klienta
        /// </summary>
        /// <param name="e">TcpClientEventArgs</param>
        protected override void OnTcpClientReceived(TcpClientEventArgs e)
        {
            try
            {
                //overime ci je mozne pridat dalsieho klienta
                if (this.InternalCheckContracts())
                {
                    try
                    {
                        //pridame instaniu servera
                        ITransportClient contract = this.InternalCreateContract(e.Client);

                        //validate contract
                        if (contract != null && contract.IsConnected)
                        {
                            //namapujeme event oznamujuci odhlasenie klienta
                            contract.DisconnectedEvent += new EventHandler(contract_DisconnectedEvent);

                            //pridame contract 
                            this.InternalCreateContract(contract);

                            //create event about new client
                            this.OnTransportClientAdded(new TransportClientEventArgs(contract));

                            //base volanie na vytvorenie eventu
                            base.OnTcpClientReceived(e);

                            // wait for data
                            contract.WaitForData();
                        }
                        else
                        {
                            //ukoncime inicializovany contract
                            if (contract != null)
                            {
                                contract.Dispose();
                                contract = null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //chybu ignorujeme
                        this.InternalTrace(TraceTypes.Error, "Interna chyba pri udalostiach spojenych z pridavanim pripojeneho klienta. {0}", ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        //zalogujeme
                        this.InternalTrace(TraceTypes.Verbose, "Ukoncovanie akceptovaneho klienta...");

                        //ziskame pristup
                        TcpClient client = e.Client;
                        client.Dispose();
                    }
                    catch (Exception)
                    {
                        //ignorujeme
                    }
                }
            }
            catch (Exception ex)
            {
                this.InternalException(ex);
                throw;
            }
        }
        /// <summary>
        /// Reaguje na ukoncenie klienta. Odoberie contract z kolekcie
        /// </summary>
        /// <param name="sender">Odosielatel udalosti</param>
        /// <param name="e">EventArgs</param>
        private void contract_DisconnectedEvent(object sender, EventArgs e)
        {
            try
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Removing the client which was exited.");

                //ziskame pristup
                ITransportClient contract = sender as ITransportClient;

                try
                {
                    //odoberame klienta
                    this.InternalRemoveContract(contract);
                }
                catch (Exception ex)
                {
                    this.InternalException(ex);
                }
            }
            catch (Exception ex)
            {
                this.InternalException(ex);
                throw;
            }
        }
        /// <summary>
        /// Prida instanciu ktora obsluhuje prijateho / akceptovaneho klienta
        /// </summary>
        /// <param name="contract">Contract obsluhujuci akceptovaneho klienta</param>
        private void InternalCreateContract(ITransportClient contract)
        {
            try
            {
                //overime instaniu
                if (contract == null || contract.IsRun == false || contract.IsDisposed)
                    return;

                lock (this)
                {
                    //pridame dalsi contract do kolekcia
                    this.mContracts.Add(contract);
                }
            }
            catch (Exception ex)
            {
                this.InternalException(ex);
                throw;
            }
        }
        /// <summary>
        /// Odoberie instanciu ktora obsluhovala prijateho / akceptovaneho klienta ktory ukoncil spojenie
        /// </summary>
        /// <param name="contract">Contract obsluhujuci akceptovaneho klienta</param>
        private void InternalRemoveSpecifiedContract(ITransportClient contract)
        {
            try
            {
                //overime instaniu
                if (contract == null)
                    return;

                lock (this)
                {
                    //zalofujeme
                    this.InternalTrace(TraceTypes.Info, "Pocet klientov: pred odobratim dalsieho {0}", this.mContracts.Count);

                    //odoberieme contract
                    this.mContracts.Remove(contract);

                    //create event about remove client
                    this.OnTransportClientRemoved(new TransportClientEventArgs(contract));

                    //zalogujeme
                    this.InternalTrace(TraceTypes.Info, "Pocet klientov: po odobrati dalsieho {0}", this.mContracts.Count);
                }

                //ukoncime inicializovany contract
                if (contract != null)
                {
                    contract.Dispose();
                    contract = null;
                }
            }
            catch (Exception ex)
            {
                this.InternalException(ex);
                throw;
            }
        }
        /// <summary>
        /// Overi maximalny pocet moznych klientov a vykona start alebo stop listenera
        /// </summary>
        /// <returns>True = je mozne aktivovat dalsieho klient</returns>
        private Boolean InternalCheckContracts()
        {
            try
            {
                //ziskame pocet aktualnych instancii
                Int32 count = 0;

                //synchronizujeme pristup
                lock (this)
                {
                    count = this.mContracts.Count;
                }

                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Overovanie stavu listenera. Count: {0}", count);

                //ak je pocet vacsi ako povoleny
                if (count >= this.mMaxConnection)
                {
                    //zalogujeme
                    this.InternalTrace(TraceTypes.Verbose, "Maximalny pocet pripojenych klientov bol dosiahnuty. [IsListened: {0}]", this.IsListened);
                    return false;
                }
                //ak este mozme prijimat klientov
                else
                {
                    //zalogujeme
                    this.InternalTrace(TraceTypes.Verbose, "Maximalny pocet pripojenych klientov este nebol dosiahnuty. [IsListened: {0}]", this.IsListened);
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.InternalException(ex);
                throw;
            }
        }
        #endregion

        #region - Protected Event Methods -
        /// <summary>
        /// This method executes event with client which was removed
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnTransportClientRemoved(TransportClientEventArgs e)
        {
            TransportClientEventHandler handler = this.mTransportClientRemoved;

            if (handler != null)
                handler(this, e);
        }
        /// <summary>
        /// This method executes event with client which was added
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnTransportClientAdded(TransportClientEventArgs e)
        {
            TransportClientEventHandler handler = this.mTransportClientAdded;

            if (handler != null)
                handler(this, e);
        }
        #endregion
    }
}
