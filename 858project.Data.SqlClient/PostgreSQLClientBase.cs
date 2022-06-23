using System;
using System.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using project858.ComponentModel.Client;
using Npgsql;
using project858.Diagnostics;

namespace project858.Data.SqlClient
{
    /// <summary>
    /// Klient zabezpecujuci vykonavanie prikazov do SQL databazy 
    /// </summary>
    public abstract class PostgreSQLClientBase : ClientBase
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="builder">Strng builder na vytvorenie SQL connection stringu</param>
        public PostgreSQLClientBase(NpgsqlConnectionStringBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");

            //nastavime pozadovane hodnoty
            this.mConnectionStringBuilder = builder;
            this.mLockObj = new Object();
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Definuje ci dojde k skracovaniu hodnot pri vlozeni do DB
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean TruncateValue
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this.mTruncateValue;
            }
            set
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                this.mTruncateValue = value;
            }
        }
        /// <summary>
        /// (Get) String builder na vytvoreie connection stringu
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        public NpgsqlConnectionStringBuilder ConnectionStringBuilder
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this.mConnectionStringBuilder;
            }
        }

        #endregion

        #region - Variable -
        /// <summary>
        /// Sql transakcia ktora prebieha
        /// </summary>
        private NpgsqlTransaction mTransaction = null;
        /// <summary>
        /// Definuje ci dojde k skracovaniu hodnot pri vlozeni do DB
        /// </summary>
        private Boolean mTruncateValue = false;
        /// <summary>
        /// Pomocny synchronizacny objekt na pristup k pripojeniu
        /// </summary>
        private readonly Object mLockObj = null;
        /// <summary>
        /// Sql pripojenie k serveru
        /// </summary>
        private NpgsqlConnection mConnection = null;
        /// <summary>
        /// String builder na vytvorenie SQL connection stringu
        /// </summary>
        private NpgsqlConnectionStringBuilder mConnectionStringBuilder = null;
        #endregion

        #region - Public Method -
        /// <summary>
        /// Zaciatok transakcie
        /// </summary>
        /// <param name="level">The isolation level under which the transaction should run.</param>
        /// <exception cref="InvalidOperationException">
        /// Chyba vyvolana v pripade ze konekcia nie je aktivna
        /// </exception>
        public void BeginTransaction(IsolationLevel level)
        {
            // client state
            if (this.IsDisposed)
                throw new ObjectDisposedException("Object was disposed");

            if (this.mConnection == null || this.mConnection.State != System.Data.ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection is not valid!");
            }
            this.mTransaction = this.mConnection.BeginTransaction(level);
        }
        /// <summary>
        /// Koniec transakcie
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Chyba vyvolana v pripade ze nie je aktivna ziadna transakcia
        /// </exception>
        public void EndTransaction()
        {
            // client state
            if (this.IsDisposed)
                throw new ObjectDisposedException("Object was disposed");

            if (this.mTransaction == null)
            {
                throw new InvalidOperationException("Transaction is not valid!");
            }
            this.mTransaction.Commit();
        }
        /// <summary>
        /// Vratenie zmien transakcie
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Chyba vyvolana v pripade ze nie je aktivna ziadna transakcia
        /// </exception>
        public void RollbackTransaction()
        {
            // client state
            if (this.IsDisposed)
                throw new ObjectDisposedException("Object was disposed");

            if (this.mTransaction == null)
            {
                throw new InvalidOperationException("Transaction is not valid!");
            }
            this.mTransaction.Rollback();
        }
        /// <summary>
        /// Vykona pozadovany prikaz na aktivne pripojenie k SQL serveru
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="NpgsqlException">
        /// Chyba tykajuca sa SQL servera alebo commandu
        /// </exception>
        /// <exception cref="Exception">
        /// Ina chyba v ramci metody
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Command neobsahuje ziadny CommandText
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Vynimka nastavne v pripade ze je metoda volana ak je klient v inom stave ako 'Start'
        /// </exception>
        /// <param name="command">Prikaz ktory chceme vykonat</param>
        /// <returns>The first column of the first row in the result set, or a null reference</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public NpgsqlDataReader ExecuteReader(NpgsqlCommand command)
        {
            // client state
            if (this.IsDisposed)
                throw new ObjectDisposedException("Object was disposed");
 
            // arguments
            if (command == null)
                throw new ArgumentNullException("command");
 
            try
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "SQL Command: '{0}'", command.CommandText);

                //pridame aktivne spojenie do priakzu
                command.Connection = this.mConnection;
                if (this.mTransaction != null)
                {
                    command.Transaction = this.mTransaction;
                }

                //vykoname pozadovany prikaz
                return command.ExecuteReader();
            }
            catch (NpgsqlException ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);
                throw;
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);
                throw;
            }
        }
        /// <summary>
        /// Vykona pozadovany prikaz na aktivne pripojenie k SQL serveru
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="NpgsqlException">
        /// Chyba tykajuca sa SQL servera alebo commandu
        /// </exception>
        /// <exception cref="Exception">
        /// Ina chyba v ramci metody
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Command neobsahuje ziadny CommandText
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Vynimka nastavne v pripade ze je metoda volana ak je klient v inom stave ako 'Start'
        /// </exception>
        /// <param name="command">Prikaz ktory chceme vykonat</param>
        /// <returns>The number of rows affected.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int ExecuteNonQuery(NpgsqlCommand command)
        {
            // client state
            if (this.IsDisposed)
                throw new ObjectDisposedException("Object was disposed");

            // arguments
            if (command == null)
                throw new ArgumentNullException("command");

            try
            {
                //pridame aktivne spojenie do priakzu
                command.Connection = this.mConnection;
                if (this.mTransaction != null)
                {
                    command.Transaction = this.mTransaction;
                }

                //vykoname pozadovany prikaz
                return command.ExecuteNonQuery();
            }
            catch (NpgsqlException ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);
                throw;
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);
                throw;
            }
        }
        /// <summary>
        /// Vykona vstupnu query a vrati scalarnu hodnotu
        /// </summary>
        /// <param name="query">Query na vykonanie prikazu</param>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <returns>The first column of the first row in the result set, or a null reference</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object ExecuteScalarWithQuery(String query)
        {
            // client state
            if (this.IsDisposed)
                throw new ObjectDisposedException("Object was disposed");

            // arguments
            if (String.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException("query");

            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                command.CommandText = query;
                return this.ExecuteScalar(command);
            }
        }
        /// <summary>
        /// Vykona pozadovany prikaz na aktivne pripojenie k SQL serveru
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="NpgsqlException">
        /// Chyba tykajuca sa SQL servera alebo commandu
        /// </exception>
        /// <exception cref="Exception">
        /// Ina chyba v ramci metody
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Command neobsahuje ziadny CommandText
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Vynimka nastavne v pripade ze je metoda volana ak je klient v inom stave ako 'Start'
        /// </exception>
        /// <param name="command">Prikaz ktory chceme vykonat</param>
        /// <returns>The first column of the first row in the result set, or a null reference</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object ExecuteScalar(NpgsqlCommand command)
        {
            // client state
            if (this.IsDisposed)
                throw new ObjectDisposedException("Object was disposed");

            // arguments
            if (command == null)
                throw new ArgumentNullException("command");

            try
            {
                //pridame aktivne spojenie do priakzu
                command.Connection = this.mConnection;
                if (this.mTransaction != null)
                {
                    command.Transaction = this.mTransaction;
                }

                //vykoname pozadovany prikaz
                return command.ExecuteScalar();
            }
            catch (NpgsqlException ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);
                throw;
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);
                throw;
            }
        }
        /// <summary>
        /// Vrati meno, popis triedy
        /// </summary>
        /// <returns>Meno</returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }
        #endregion

        #region - Protected Method -
        /// <summary>
        /// Interne spustenie klienta
        /// </summary>
        /// <returns>True = spustenie klienta bolo uspesne</returns>
        protected override bool InternalStart()
        {
            //vykoname start pripajania k serveru
            return this.InternalConnect();
        }
        /// <summary>
        /// Ukonci funkciu klienta
        /// </summary>
        protected override void InternalStop()
        {
            //ukoncime komunikaciu
            this.InternalDisconnect();
        }
        /// <summary>
        /// Vykona pred ukoncenim klienta
        /// </summary>
        protected override void InternalDispose()
        {
            //ukoncime klienta
            this.InternalStop();
        }
        #endregion

        #region - Private Method -
        /*
        /// <summary>
        /// Overi stav klienta a komunikaciu pri volani public metody
        /// </summary>
        private void InternalCheckClientState()
        {
            //je objekt _disposed ?
            if (this.IsDisposed)
                throw new ObjectDisposedException("Object was disposed");

            //overime stav klienta
            if (this.ClientState != ClientStates.Start)
                throw new InvalidOperationException("Client state is not 'Start'");
            if (this.ConnectionState != ConnectionStates.Connected)
                throw new InvalidOperationException("ConnectionState is not 'Connected'");

            //overime stav spojenia
            if (!this.InternalCheckConnection())
                throw new Exception("Connection is lost.");
        }
        /// <summary>
        /// Overi stav spojenia. Ak spojenie nie je vykona start automatickeho reconnectu
        /// </summary>
        /// <returns>True = spojenie je aktivne</returns>
        private Boolean InternalCheckConnection()
        {
            //ak spojenie nie je aktivne
            if (this.mConnection != null && this.mConnection.State == System.Data.ConnectionState.Open)
                return true;

            //zatvorime existujuce spojenie
            this.InternalCloseConnection();

            //spustime nove pripajanie k sql serveru
            this.InternalConnect();

            //spojenie nieje aktivne
            return false;
        }
        */
        /// <summary>
        /// Spusti automaticky connect k sql serveru
        /// </summary>
        /// <returns>True = spojenie bolo vytvorene, inak false</returns>
        private bool InternalConnect()
        {
            // trace
            this.InternalTrace(TraceTypes.Verbose, "Opening connection to server...");

            // open connection
            return this.InternalOpenConnection();
        }
        /// <summary>
        /// Ukonci automaticky reconnect alebo aktivne spojenie k sql serveru
        /// </summary>
        private void InternalDisconnect()
        {
            // trace
            this.InternalTrace(TraceTypes.Verbose, "Closing connection to server...");

            //ukoncime komunikaciu
            this.InternalCloseConnection();
        }
        /// <summary>
        /// Inicializuje a otvori spojenie na definovany _sqlServer a databazu
        /// </summary>
        /// <returns>True = connect bol uspesny</returns>
        private Boolean InternalOpenConnection()
        {
            try
            {
                //inicializujeme spojenie
                this.mConnection = new NpgsqlConnection();
                this.mConnection.ConnectionString = this.mConnectionStringBuilder.ToString();

                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, this.mConnection.ConnectionString);

                //otvorime spojenie
                this.mConnection.Open();

                //spojenie bolo uspesne vytvorene
                return true;
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri vytvarani spojenia k SQL serveru. {0}", ex.Message);
                throw;
            }
        }
        /// <summary>
        /// Ukonci spojenie s sql serverom
        /// </summary>
        private void InternalCloseConnection()
        {
            //ukoncime pripojenie
            if (this.mConnection != null)
            {
                if (this.mConnection.State != System.Data.ConnectionState.Closed)
                {
                    this.mConnection.Close();
                    this.mConnection.Dispose();
                    this.mConnection = null;
                }
            }
        }
        #endregion
    }
}
