using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace project858.Net
{
    /// <summary>
    /// One group for frame v2
    /// </summary>
    public sealed class PackageItemGroup : IPackageItem
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Data adress for this group</param>
        public PackageItemGroup(UInt32 address)
        {
            this.mItems = new List<IPackageItem>();
            this.Address = address;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Command address
        /// </summary>
        public UInt32 Count
        {
            get { return Convert.ToUInt32(this.mItems.Count); }
        }
        /// <summary>
        /// Item collections
        /// </summary>
        public ReadOnlyCollection<IPackageItem> Items
        {
            get { return this.mItems.AsReadOnly(); }
        }
        /// <summary>
        /// Item type
        /// </summary>
        public PackageItemTypes ItemType
        {
            get { return PackageItemTypes.Group; }
        }
        /// <summary>
        /// Item address
        /// </summary>
        public UInt32 Address
        {
            get;
            private set;
        }
        /// <summary>
        /// Data item
        /// </summary>
        public Byte[] Data
        {
            get { return this.InternalGetItemsData(); }
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Item collections
        /// </summary>
        private List<IPackageItem> mItems = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// This method prints value as string
        /// </summary>
        /// <returns>String value</returns>
        public String ValueToString()
        {
            return String.Format("Items[{0}]", this.mItems.Count);
        }
        /// <summary>
        ///  Removes the first occurrence of a specific object with address
        /// </summary>
        /// <param name="address">Specific address</param>
        public void Remove(UInt16 address)
        {
            int itemCount = this.mItems.Count;
            for (int i = 0; i < itemCount; i++)
            {
                IPackageItem item = this.mItems[i];
                if (item.Address == address)
                {
                    this.Remove(item);
                    return;
                }
            }
        }
        /// <summary>
        ///  Removes the first occurrence of a specific object
        /// </summary>
        /// <param name="item">The object to remove</param>
        public void Remove(IPackageItem item)
        {
            this.mItems.Remove(item);
        }
        /// <summary>
        /// Adds an item to the end of the Frame
        /// </summary>
        /// <param name="item">The item to be added to the end of the Frame</param>
        public void Add(IPackageItem item)
        {
            this.mItems.Add(item);
        }
        /// <summary>
        /// Inserts an item into the Frame at the specified index.
        /// </summary>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        public void Add(IPackageItem item, int index)
        {
            this.mItems.Insert(index, item);
        }
        /// <summary>
        /// This function returns frame item with address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Item | null</returns>
        public IPackageItem GetItem(UInt32 address)
        {
            foreach (IPackageItem item in this.mItems)
            {
                if (item.Address == address)
                {
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// This function returns value 
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="address">Address</param>
        /// <returns>Value | null</returns>
        public T GetValue<T>(UInt32 address)
        {
            foreach (IPackageItem item in this.mItems)
            {
                if (item.Address == address && item.ItemType != PackageItemTypes.Group)
                {
                    Object value = item.GetValue();
                    try
                    {
                        //check type
                        if (typeof(T) == typeof(Object))
                        {
                            return (T)value;
                        }
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidCastException(String.Format("Value: {0}, Type: {1}, Item: {2}", (value == null ? "NULL" : value.ToString()), typeof(T), item), ex);
                    }
                }
            }
            return default(T);
        }
        /// <summary>
        /// This function returns item data
        /// </summary>
        /// <returns>Item array data</returns>
        public Byte[] ToByteArray()
        {
            // get items data
            Byte[] data = this.InternalGetItemsData();
            int length = this.Data.Length;

            //add headers
            Byte[] result = new Byte[7 + length];
            result[0] = (Byte)(this.Address);
            result[1] = (Byte)(this.Address >> 8);
            result[2] = (Byte)(this.Address >> 16);
            result[3] = (Byte)(this.Address >> 24);
            result[4] = (Byte)(length);
            result[5] = (Byte)(length >> 8);
            result[6] = (Byte)this.ItemType;

            // add data to result
            Buffer.BlockCopy(this.Data, 0, result, 7, length);

            //return collection
            return result;
        }
        /// <summary>
        /// This function returns value
        /// </summary>
        /// <returns>Value</returns>
        public object GetValue()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This method returns data from all items
        /// </summary>
        /// <returns></returns>
        private Byte[] InternalGetItemsData()
        {
            //create collection
            List<Byte> collection = new List<Byte>();

            //add items
            foreach (IPackageItem item in this.mItems)
            {
                Byte[] data = item.ToByteArray();
                collection.AddRange(data);
            }

            //return collection
            return collection.ToArray();
        }
        #endregion
    }
}
