using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using project858.Diagnostics;
using project858.Models;
using project858.Net;

namespace project858.IO.Ports
{
    /// <summary>
    /// The implementation which provides the interface for communication with terminal directly with C# classes,
    /// </summary>
    public sealed class TerminalClient : IDisposable
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="portName">Com port name</param>
        /// <param name="handshake">Handshaking protocol for serial port transmission of data</param>
        public TerminalClient(String portName, Handshake handshake) :
            this(portName, 115200, Parity.None, 8, StopBits.One, handshake)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="portName">Com port name</param>
        /// <param name="handshake">Handshaking protocol for serial port transmission of data</param>
        /// <param name="readTimeout">Read timeut for serial port</param>
        /// <param name="writeTimeout">Write timeout for serial port</param>
        public TerminalClient(String portName, Handshake handshake, Int32 readTimeout, Int32 writeTimeout) :
            this(portName, 9600, Parity.None, 8, StopBits.One, handshake, readTimeout, writeTimeout)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="portName">Com port name/param>
        /// <param name="baudRate">Baud rate for serial port</param>
        /// <param name="handshake">Handshaking protocol for serial port transmission of data</param>
        public TerminalClient(String portName, Int32 baudRate, Handshake handshake) :
            this(portName, baudRate, Parity.None, 8, StopBits.One, handshake)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="portName">Com port name</param>
        /// <param name="baudRate">Baud rate for serial port</param>
        /// <param name="handshake">Handshaking protocol for serial port transmission of data</param>
        /// <param name="readTimeout">Read timeut for serial port</param>
        /// <param name="writeTimeout">Write timeout for serial port</param>
        public TerminalClient(String portName, Int32 baudRate, Handshake handshake, Int32 readTimeout, Int32 writeTimeout) :
            this(portName, baudRate, Parity.None, 8, StopBits.One, handshake, readTimeout, writeTimeout)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="portName">Com port name</param>
        /// <param name="baudRate">Baud rate for serial port</param>
        /// <param name="parity">Parity for serial port</param>
        /// <param name="handshake">Handshaking protocol for serial port transmission of data</param>
        public TerminalClient(String portName, Int32 baudRate, Parity parity, Handshake handshake) :
            this(portName, baudRate, parity, 8, StopBits.One, handshake)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="portName">Com port name</param>
        /// <param name="baudRate">Baud rate for serial port</param>
        /// <param name="parity">Parity for serial port</param>
        /// <param name="handshake">Handshaking protocol for serial port transmission of data</param>
        /// <param name="readTimeout">Read timeut for serial port</param>
        /// <param name="writeTimeout">Write timeout for serial port</param>
        public TerminalClient(String portName, Int32 baudRate, Parity parity, Handshake handshake, Int32 readTimeout, Int32 writeTimeout) :
            this(portName, baudRate, parity, 8, StopBits.One, handshake, readTimeout, writeTimeout)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="portName">Com port name/param>
        /// <param name="baudRate">Baud rate for serial port</param>
        /// <param name="handshake">Handshaking protocol for serial port transmission of data</param>
        /// <param name="parity">Parity for serial port</param>
        /// <param name="dataBits">Data bits for serial port</param>
        public TerminalClient(String portName, Int32 baudRate, Parity parity, Handshake handshake, int dataBits) :
            this(portName, baudRate, parity, dataBits, StopBits.One, handshake)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="portName">Com port name</param>
        /// <param name="baudRate">Baud rate for serial port</param>
        /// <param name="parity">Parity for serial port</param>
        /// <param name="dataBits">Data bits for serial port</param>
        /// <param name="handshake">Handshaking protocol for serial port transmission of data</param>
        /// <param name="readTimeout">Read timeut for serial port</param>
        /// <param name="writeTimeout">Write timeout for serial port</param>
        public TerminalClient(String portName, Int32 baudRate, Parity parity, int dataBits, Handshake handshake, Int32 readTimeout, Int32 writeTimeout) :
            this(portName, baudRate, parity, dataBits, StopBits.One, handshake, readTimeout, writeTimeout)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="portName">Com port name</param>
        /// <param name="baudRate">Baud rate for serial port</param>
        /// <param name="parity">Parity for serial port</param>
        /// <param name="dataBits">Data bits for serial port</param>
        /// <param name="stopBits">Stop bits for serial port</param>
        /// <param name="handshake">Handshaking protocol for serial port transmission of data</param>
        public TerminalClient(String portName, Int32 baudRate, Parity parity, Int32 dataBits, StopBits stopBits, Handshake handshake) :
            this(portName, baudRate, parity, dataBits, stopBits, handshake, 1000, 2000)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="portName">Com port name</param>
        /// <param name="baudRate">Baud rate for serial port</param>
        /// <param name="parity">Parity for serial port</param>
        /// <param name="dataBits">Data bits for serial port</param>
        /// <param name="stopBits">Stop bits for serial port</param>
        /// <param name="handshake">Handshaking protocol for serial port transmission of data</param>
        /// <param name="readTimeout">Read timeut for serial port</param>
        /// <param name="writeTimeout">Write timeout for serial port</param>
        public TerminalClient(String portName, Int32 baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake, Int32 readTimeout, Int32 writeTimeout)
        {
            //inicializujeme port
            this.mPort = new SerialPort(portName);
            this.mPort.BaudRate = baudRate;
            this.mPort.Parity = parity;
            this.mPort.DataBits = dataBits;
            this.mPort.StopBits = stopBits;
            this.mPort.WriteTimeout = readTimeout;
            this.mPort.ReadTimeout = writeTimeout;
            this.mPort.Handshake = handshake;

            //open port
            this.mPort.Open();

            //vyprazdnime buffre
            this.mPort.DiscardInBuffer();
            this.mPort.DiscardOutBuffer();
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool mDisposed = false;
        /// <summary>
        /// Serial port for communication
        /// </summary>
        private SerialPort mPort = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Asynchronously reads a sequence of bytes and convert their to response
        /// </summary>
        /// <typeparam name="T">Type of response</typeparam>
        /// <param name="timeout">Max timeout for this operation</param>
        /// <param name="cancellationToken"> 
        /// The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.
        /// </param>
        /// <returns>Response with data | NULL</returns>
        public async Task<ITerminalResponseModel<T>> ReadAsync<T>(TimeSpan timeout, CancellationToken cancellationToken)
        {
            // execute async task
            var task = this.InternalReadAsync<T>(this.mPort, cancellationToken);

            // cancellation token
            using (var timeoutCancellationToken = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationToken.Token));
                if (completedTask == task)
                {
                    // cancel timeout
                    timeoutCancellationToken.Cancel();

                    // Very important in order to propagate exceptions
                    return await task;
                }
                else
                {
                    // timeout expired
                    throw new TimeoutException("The operation has timed out.");
                }
            }
        }
        /// <summary>
        /// Asynchronously writes a sequence of bytes from the request to ComPort.
        /// </summary>
        /// <param name="request">Request to send</param>
        /// <param name="version">Version of package</param>
        /// <param name="cancellationToken"> 
        /// The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.
        /// </param>
        /// <returns>True | False</returns>
        public async Task<Boolean> SendAsync<T>(ITerminalRequestModel<T> request, Byte version, CancellationToken cancellationToken)
        {
            // serialize this data
            Package package = PackageHelper.Serialize<T>(request.Data, version, request.Address);

            // send data to port
            return await this.InternalSendAsync(this.mPort, package, cancellationToken);
        }
        /// <summary>
        /// Public implementation of Dispose pattern callable by consumers.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Asynchronously reads a sequence of bytes and convert their to response
        /// </summary>
        /// <typeparam name="T">Type of response</typeparam>
        /// <param name="port">Com port to read data</param>
        /// <param name="cancellationToken"> 
        /// The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.
        /// </param>
        /// <returns>Response with data | NULL</returns>
        private async Task<ITerminalResponseModel<T>> InternalReadAsync<T>(SerialPort port, CancellationToken cancellationToken)
        {
            //variables
            Byte[] data = new Byte[1024];
            List<Byte> dataCollection = new List<Byte>();

            //loop
            while (port.IsOpen)
            {
                try
                {
                    //read data from port
                    int numberOfBytes = await port.BaseStream.ReadAsync(data, 0x00, data.Length, cancellationToken);
                    if (numberOfBytes > 0x00)
                    {
                        //add data to collection
                        Byte[] tempData = new Byte[numberOfBytes];
                        Buffer.BlockCopy(data, 0x00, tempData, 0x00, numberOfBytes);
                        dataCollection.AddRange(tempData);

                        // find package
                        // this feature automatically removes the data it used from the collection
                        Package response = PackageHelper.FindPackage(dataCollection);
                        if (response != null)
                        {
                            // parse response
                            return this.InternalParseResponse<T>(response);
                        }
                    }
                }
                catch (IOException)
                {
                    // ignore this error. It is abort exception
                    return null;
                }
                catch (Exception ex)
                {
                    //trace error
                    ConsoleLogger.Error(ex);
                    return null;
                }
            }

            // no response from this operation
            return null;
        }
        /// <summary>
        /// Parses data from the package to the required response
        /// </summary>
        /// <typeparam name="T">Type of response</typeparam>
        /// <param name="package">Package to convert</param>
        /// <returns>Response with data</returns>
        private ITerminalResponseModel<T> InternalParseResponse<T>(Package package)
        {
            // create response
            TerminalResponseModel<T> response = new TerminalResponseModel<T>
            {
                State = package.State,
                Address = package.Address
            };

            // set data
            switch (response.State)
            {
                case Package.StateTypes.MESSAGE:
                    {
                        response.Messages = PackageHelper.Deserialize<MessageModel>(package);
                        break;
                    }
                case Package.StateTypes.VALIDATION_ERROR:
                    {
                        response.Validations = PackageHelper.Deserialize<ValidationModel>(package);
                        break;
                    }
                case Package.StateTypes.DATA:
                    {
                        response.Results = PackageHelper.Deserialize<T>(package);
                        break;
                    }
                default:
                    {
                        // other state
                        break;
                    }
            }

            // return response with data
            return response;
        }
        /// <summary>
        /// Asynchronously writes a sequence of bytes from the package.
        /// </summary>
        /// <param name="port">Serial port</param>
        /// <param name="package">Package to send</param>
        /// <param name="cancellationToken"> 
        /// The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.
        /// </param>
        /// <returns>True | False</returns>
        private async Task<Boolean> InternalSendAsync(SerialPort port, Package package, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    if (port.IsOpen)
                    {
                        Byte[] data = package.ToByteArray();
                        port.WriteTimeout = 1000;
                        port.Write(data, 0x00, data.Length);
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    // trace
                    ConsoleLogger.Error(ex.Message);
                    return false;
                }
            });
        }
        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">This parameter determines two distinct scenarios for disposing</param>
        private void Dispose(bool disposing)
        {
            if (!this.mDisposed)
            {
                if (disposing)
                {
                    // Free any other managed objects here.
                    if (this.mPort != null)
                    {
                        if (this.mPort.IsOpen)
                        {
                            this.mPort.Close();
                            this.mPort.Dispose();
                        }
                        this.mPort = null;
                    }
                }
                mDisposed = true;
            }
        }
        #endregion

        #region - Destructor -
        /// <summary>
        /// Deinitialize this class
        /// </summary>
        ~TerminalClient()
        {
            Dispose(false);
        }
        #endregion}
    }
}
