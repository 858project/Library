using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace project858.Net
{
    /// <summary>
    /// Protocol frame
    /// </summary>
    public sealed class Package
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Command address</param>
        /// <param name="state">State value</param>
        public Package(Byte version, UInt16 address, Package.StateTypes state)
            : this(version, address, state, null)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Command address</param>
        /// <param name="state">State value</param>
        /// <param name="data">Frame data</param>
        public Package(Byte version, UInt16 address, Package.StateTypes state, Byte[] data)
        {
            this.Version = version;
            this.Address = address;
            this.State = state;
            this.mItems = new List<IPackageItem>();
            if (data != null)
            {
                this.InternalParsePackage(data);
            }
        }
        #endregion

        #region - Public Class -
        /// <summary>
        /// Define base value for frame
        /// </summary>
        public enum StateTypes : Byte
        {
            /// <summary>
            /// State OK
            /// </summary>
            OK = 0x00,
            /// <summary>
            /// Frame contains data
            /// </summary>
            DATA = 0x01,
            /// <summary>
            /// Frame contains data
            /// </summary>
            NO_DATA = 0x02,
            /// <summary>
            /// In the target system has experienced an internal error
            /// </summary>
            INTERNAL_ERROR = 0x03,
            /// <summary>
            /// Required data or operation are not available
            /// </summary>
            NOT_AVAILABLE = 0x04,
            /// <summary>
            /// The target system is busy
            /// </summary>
            BUSY = 0x05,
            /// <summary>
            /// Command was denied
            /// </summary>
            DENIED = 0x06,
            /// <summary>
            /// Start sequence, after this response continues frames with data, see STATE_START_SEQUENCE
            /// </summary>
            START_SEQUENCE = 0x07,
            /// <summary>
            /// End sequence, see STATE_START_SEQUENCE
            /// </summary>
            END_SEQUENCE = 0x08,
            /// <summary>
            /// Frame contains message
            /// </summary>
            MESSAGE = 0x09,
            /// <summary>
            /// Command was canceled
            /// </summary>
            CANCELED = 0x0A,
            /// <summary>
            /// Validation error
            /// </summary>
            VALIDATION_ERROR = 0x0B,
            /// <summary>
            /// Declined
            /// </summary>
            DECLINED = 0x0C,
            /// <summary>
            /// Request is not valid, some data is missing
            /// </summary>
            BAD_REQUEST = 0x0D,
            /// <summary>
            /// This state occurs usually when the terminal has a bad time
            /// </summary>
            INVALID_DATE_TIME = 0x0E
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Package version
        /// </summary>
        public Byte Version
        {
            get;
            private set;
        }
        /// <summary>
        /// Command address
        /// </summary>
        public UInt16 Address
        {
            get;
            private set;
        }
        /// <summary>
        /// Frame state
        /// </summary>
        public StateTypes State
        {
            get;
            set;
        }
        /// <summary>
        /// Item collections
        /// </summary>
        public ReadOnlyCollection<IPackageItem> Items
        {
            get { return this.mItems.AsReadOnly(); }
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Group collections
        /// </summary>
        private List<IPackageItem> mItems = null;
        #endregion

        #region - Public Static Methods -
        /// <summary>
        /// This function checks whether the state is final or not
        /// </summary>
        /// <param name="state">State to validate</param>
        /// <returns>True | False</returns>
        public static Boolean IsFinalState(Package.StateTypes state)
        {
            switch (state)
            {
                case StateTypes.NO_DATA:
                case StateTypes.DATA:
                case StateTypes.INTERNAL_ERROR:
                case StateTypes.NOT_AVAILABLE:
                case StateTypes.BUSY:
                case StateTypes.DENIED:
                case StateTypes.END_SEQUENCE:
                case StateTypes.CANCELED:
                case StateTypes.VALIDATION_ERROR:
                case StateTypes.DECLINED:
                case StateTypes.BAD_REQUEST:
                case StateTypes.INVALID_DATE_TIME:
                    return true;
                case StateTypes.OK:
                case StateTypes.START_SEQUENCE:
                case StateTypes.MESSAGE:
                default:
                    return false;
            }
        }
        /// <summary>
        /// Create frame from Byte value
        /// </summary>
        /// <param name="address">Frame address</param>
        /// <param name="state">Frate state</param>
        /// <param name="value">Value as Byte</param>
        /// <returns>New frame</returns>
        public static Package CreatePackage(Byte version, UInt16 address, Package.StateTypes state, Byte value)
        {
            Package frame = new Package(version, address, state);
            frame.State = state;
            return frame;
        }
        /// <summary>
        /// This function initialize item of type
        /// </summary>
        /// <param name="type">Item type</param>
        /// <param name="address">Item address for value</param>
        /// <param name="value">Value</param>
        /// <returns>Frame item | null</returns>
        public static IPackageItem CreatePackageItem(PackageItemTypes type, UInt32 address, Object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            switch (type)
            {
                case PackageItemTypes.DateTime:
                    return new PackageItemDateTime(address, (DateTime)value);
                case PackageItemTypes.Guid:
                    return new PackageItemGuid(address, (Guid)value);
                case PackageItemTypes.String:
                     return new PackageItemString(address, (String)value);
                case PackageItemTypes.Int16:
                    return new PackageItemInt16(address, (Int16)value);
                case PackageItemTypes.Int32:
                    return new PackageItemInt32(address, (Int32)value);
                case PackageItemTypes.Int64:
                    return new PackageItemInt64(address, (Int64)value);
                case PackageItemTypes.Byte:
                    return new PackageItemByte(address, (Byte)value);
                case PackageItemTypes.Boolean:
                    return new PackageItemBoolean(address, (Boolean)value);
                case PackageItemTypes.UInt16:
                    return new PackageItemUInt16(address, (UInt16)value);
                case PackageItemTypes.UInt32:
                    return new PackageItemUInt32(address, (UInt32)value);
                case PackageItemTypes.UInt64:
                    return new PackageItemUInt64(address, (UInt64)value);
                case PackageItemTypes.Enum:
                    return new PackageItemEnum(address, value);
                case PackageItemTypes.Bytes:
                    return new PackageItemBytes(address, (Byte[])value);
                default:
                    return new PackageItemUnkown(address, (List<Byte>)value);
            }
        }
        /// <summary>
        /// This function return NET object type from Frame Item type
        /// </summary>
        /// <param name="type">Frame item type</param>
        /// <returns>NET object type</returns>
        public static Type GetObjectTypeFromFrameItemType(PackageItemTypes type)
        {
            switch (type)
            {
                case PackageItemTypes.DateTime:
                    return typeof(DateTime);
                case PackageItemTypes.Guid:
                    return typeof(Guid);
                case PackageItemTypes.Int16:
                    return typeof(Int16);
                case PackageItemTypes.Int32:
                    return typeof(Int32);
                case PackageItemTypes.Int64:
                    return typeof(Int64);
                case PackageItemTypes.String:
                    return typeof(String);
                case PackageItemTypes.Enum:
                    return typeof(Object);
                case PackageItemTypes.Byte:
                    return typeof(Byte);
                case PackageItemTypes.Boolean:
                    return typeof(Boolean);
                case PackageItemTypes.UInt64:
                    return typeof(UInt64);
                case PackageItemTypes.UInt32:
                    return typeof(UInt32);
                default:
                    return typeof(Object);
            }
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// This method returns first froup with required address
        /// </summary>
        /// <param name="address">Required address</param>
        /// <returns></returns>
        public IPackageItem FindItem(UInt16 address)
        {
            int groupCount = this.mItems.Count;
            for (int i = 0; i < groupCount; i++)
            {
                IPackageItem group = this.mItems[i];
                if (group.Address == address)
                {
                    return group;
                }
            }
            return null;
        }
        /// <summary>
        ///  Removes the first occurrence of a specific object
        /// </summary>
        /// <param name="group">The object to remove</param>
        public void Remove(IPackageItem group)
        {
            this.mItems.Remove(group);
        }
        /// <summary>
        /// Adds an item to the end of the Frame
        /// </summary>
        /// <param name="groups">The item to be added to the end of the Frame</param>
        public void Add(List<IPackageItem> groups)
        {
            foreach (IPackageItem group in groups)
            {
                this.Add(group);
            }
        }
        /// <summary>
        /// Adds an item to the end of the Frame
        /// </summary>
        /// <param name="group">The item to be added to the end of the Frame</param>
        public void Add(IPackageItem group)
        {
            this.mItems.Add(group);
        }
        /// <summary>
        /// Inserts an item into the Frame at the specified index.
        /// </summary>
        /// <param name="group">The object to insert. The value can be null for reference types.</param>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        public void Add(IPackageItem group, int index)
        {
            this.mItems.Insert(index, group);
        }
        /// <summary>
        /// Change frame address
        /// </summary>
        /// <param name="address">new address</param>
        public void ChangeAddress(UInt16 address)
        {
            this.Address = address;
        }
        /// <summary>
        /// This finction converts frame to byte array
        /// </summary>
        /// <returns>Byte array | null</returns>
        public Byte[] ToByteArray()
        {
            return this.InternalToByteArray();
        }
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            //initialize builder
            StringBuilder builder = new StringBuilder();

            //base information
            builder.AppendLine("------------------");
            builder.AppendFormat("Version: {0:X2}, Address: {0}, Items: {1}", this.Version, this.Address, this.Items.Count);
            builder.Append(Environment.NewLine);

            //items
            foreach (IPackageItem item in this.mItems)
            {
                this.InternalToString(item, builder, 0x01);
            }

            //create end line
            builder.AppendLine("------------------");

            //return string
            return builder.ToString();
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This method prints item to string builder
        /// </summary>
        /// <param name="item"></param>
        /// <param name="level"></param>
        /// <param name="builder"></param>
        private void InternalToString(IPackageItem item, StringBuilder builder, int level = 0x00)
        {
            if (item.ItemType == PackageItemTypes.Group && item is PackageItemGroup group)
            {
                // group information
                builder.Append(' ', level);
                builder.AppendFormat("Group address: {0} [0x{0:X4}], Items: {1}", group.Address, group.Count);
                builder.Append(Environment.NewLine);

                // next level
                level += 0x01;

                // items
                foreach (IPackageItem childItem in group.Items)
                {
                    this.InternalToString(childItem, builder, level);
                }
            }
            else
            {
                // base information
                builder.Append(' ', level);
                builder.AppendFormat("Item address: {0} [0x{0:X4}], Type: {1}", item.Address, item.ItemType);
                builder.Append(Environment.NewLine);

                // data information
                builder.Append(' ', level);
                builder.AppendFormat("Length: {0}, Value: {1}, Data: {2}", item.Data.Length, item.ValueToString(), BitConverter.ToString(item.Data));
                builder.Append(Environment.NewLine);
            }
        }
        /// <summary>
        /// This finction converts frame to byte array
        /// </summary>
        /// <returns>Byte array | null</returns>
        private Byte[] InternalToByteArray()
        {
            //variables
            List<Byte> collection = new List<Byte>();
            Int16 length = 0x07;

            //add items
            foreach (PackageItemGroup item in this.mItems)
            {
                Byte[] data = item.ToByteArray();
                length += (Int16)data.Length;
                collection.AddRange(data);
            }

            //add headers
            Byte[] header = new Byte[7];
            header[0] = 0x70;
            header[1] = (Byte)(length);
            header[2] = (Byte)(length >> 8);
            header[3] = (Byte)this.Version;
            header[4] = (Byte)(this.Address);
            header[5] = (Byte)(this.Address >> 8);
            header[6] = (Byte)this.State;
            collection.InsertRange(0, header);

            //check sum
            Byte checkSum = this.internalGetFrameDataCheckSum(collection);
            collection.Add(checkSum);

            //return frame as data array
            return collection.ToArray();
        }
        /// <summary>
        /// This function calculate check sum from frame
        /// </summary>
        /// <param name="array">Data array</param>
        /// <returns>Check sum</returns>
        private Byte internalGetFrameDataCheckSum(List<Byte> array)
        {
            int sum = 0;
            int count = array.Count;
            for (int currentIndex = 1; currentIndex < count; currentIndex++)
            {
                sum += (int)array[currentIndex];
            }
            sum += 0xA5;
            sum = sum & 0xFF;
            return (byte)(256 - sum);
        }
        /// <summary>
        /// This method parses package item from data
        /// </summary>
        /// <param name="address">item address</param>
        /// <param name="length">item length</param>
        /// <param name="type">item type</param>
        /// <param name="data">item data</param>
        /// <returns>Package item</returns>
        private IPackageItem InternalParseItem(UInt32 address, UInt16 length, PackageItemTypes type, byte[] data)
        {
            // check group
            if (type != PackageItemTypes.Group)
            {
                return this.InternalGetPackageItem(address, length, type, data);
            }
            else
            {
                // create group item
                PackageItemGroup mGroup = new PackageItemGroup(address);

                //variables
                int count = data.Length;
                int currentIndex = 0x00;
                Byte[] dataItem = null;

                //parse data
                while (currentIndex < count)
                {
                    //get data address
                    address = (UInt32)(data[currentIndex + 3] << 24 | data[currentIndex + 2] << 16 | data[currentIndex + 1] << 8 | data[currentIndex]);
                    currentIndex += 0x04;

                    // get length
                    length = (UInt16)(data[currentIndex + 1] << 8 | data[currentIndex]);
                    currentIndex += 0x02;

                    // get type
                    type = (PackageItemTypes)data[currentIndex];
                    currentIndex += 0x01;

                    //read data
                    dataItem = new Byte[length];
                    Buffer.BlockCopy(data, currentIndex, dataItem, 0, length);

                    // parse item
                    IPackageItem item = this.InternalParseItem(address, length, type, dataItem);
                    if (item == null)
                    {
                        throw new Exception("Parsing PackageItem failed");
                    }

                    // add item to package
                    mGroup.Add(item);

                    // next data
                    currentIndex += length;
                }

                //return group with data
                return mGroup;
            }
        }
        /// <summary>
        /// This function parse frame from data
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="action">Function to get frame item type</param>
        private void InternalParsePackage(Byte[] data)
        {
            //vriables
            UInt32 address = 0x00;
            UInt16 length = 0x00;
            int count = data.Length;
            int currentIndex = 0x00;
            PackageItemTypes type = PackageItemTypes.Unknown;
            Byte[] dataItem = null;

            //parse data
            while (currentIndex < count)
            {
                //get data address
                address = (UInt32)(data[currentIndex + 3] << 24 | data[currentIndex + 2] << 16 | data[currentIndex + 1] << 8 | data[currentIndex]);
                currentIndex += 0x04;

                // get length
                length = (UInt16)(data[currentIndex + 1] << 8 | data[currentIndex]);
                currentIndex += 0x02;

                // get type
                type = (PackageItemTypes)data[currentIndex];
                currentIndex += 0x01;

                //read data
                dataItem = new Byte[length];
                Buffer.BlockCopy(data, currentIndex, dataItem, 0, length);

                // parse item
                IPackageItem item = this.InternalParseItem(address, length, type, dataItem);
                if (item == null)
                {
                    throw new Exception("Parsing PackageItem failed");
                }

                // add item to package
                this.Add(item);

                // next data
                currentIndex += length;
            }
        }
        /// <summary>
        /// This function parse frame item from item type
        /// </summary>
        /// <param name="type">Type of frame</param>
        /// <param name="address">Frame item address</param>
        /// <param name="length">Frame item length</param>
        /// <param name="data">Data frame item</param>
        /// <returns>Frame item or null</returns>
        private IPackageItem InternalGetPackageItem(UInt32 address, UInt16 length, PackageItemTypes type, Byte[] data)
        {
            switch (type)
            {
                case PackageItemTypes.DateTime:
                    return new PackageItemDateTime(data, address);
                case PackageItemTypes.Guid:
                    return new PackageItemGuid(data, address);
                case PackageItemTypes.String:
                    return new PackageItemString(data, address);
                case PackageItemTypes.Int16:
                    return new PackageItemInt16(data, address);
                case PackageItemTypes.Int32:
                    return new PackageItemInt32(data, address);
                case PackageItemTypes.Int64:
                    return new PackageItemInt64(data, address);
                case PackageItemTypes.Byte:
                    return new PackageItemByte(data, address);
                case PackageItemTypes.Boolean:
                    return new PackageItemBoolean(data, address);
                case PackageItemTypes.UInt16:
                    return new PackageItemUInt16(data, address);
                case PackageItemTypes.UInt32:
                    return new PackageItemUInt32(data, address);
                case PackageItemTypes.Enum:
                    return new PackageItemEnum(data, address);
                case PackageItemTypes.UInt64:
                    return new PackageItemUInt64(data, address);
                case PackageItemTypes.Bytes:
                    return new PackageItemBytes(data, address);
                default:
                    return new PackageItemUnkown(data, address);
            }
        }
        #endregion
    }
}
