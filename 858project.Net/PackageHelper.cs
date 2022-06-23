using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using project858.Reflection;

namespace project858.Net
{
    /// <summary>
    /// Frame helper for serializing or deserializing frame
    /// </summary>
    public static class PackageHelper
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        static PackageHelper()
        {
            PackageHelper.mReflectionPropertyCollection = new ReflectionPackageModelCollection();
        }
        #endregion

        #region - Private Static Class -
        /// <summary>
        /// Item property
        /// </summary>
        private sealed class PackageModelPropertyItem
        {
            #region - Constructor -
            /// <summary>
            /// Initialize this class
            /// </summary>
            /// <param name="property">Property ktoru objekt reprezentuje</param>
            public PackageModelPropertyItem(PropertyInfo property)
            {
                if (property == null)
                    throw new ArgumentNullException("property");

                this.m_property = property;
            }
            #endregion

            #region - Properties -
            /// <summary>
            /// Vrati atributy prisluchajucej property
            /// </summary>
            public PackageItemAttribute ItemAttribute
            {
                get
                {
                    if (this.m_columnAttributes == null)
                    {
                        Object[] attributes = this.Property.GetCustomAttributes(typeof(PackageItemAttribute), true);
                        if (attributes != null && attributes.Length == 1)
                        {
                            this.m_columnAttributes = attributes[0] as PackageItemAttribute;
                        }
                    }
                    return this.m_columnAttributes;
                }
            }
            /// <summary>
            /// Property ktoru objekt reprezentuje
            /// </summary>
            public PropertyInfo Property
            {
                get { return this.m_property; }
            }
            #endregion

            #region - Variable -
            /// <summary>
            /// Atributy prisluchajucej property
            /// </summary>
            private PackageItemAttribute m_columnAttributes = null;
            /// <summary>
            /// Property
            /// </summary>
            private PropertyInfo m_property = null;
            #endregion
        }
        /// <summary>
        /// Package item
        /// </summary>
        private sealed class ReflectionPackageModelItem : Dictionary<UInt32, PackageModelPropertyItem>
        {
            #region - Constructor -
            /// <summary>
            /// Initialize this class
            /// </summary>
            /// <param name="type">Typ objektu ktoreho property objekt reprezentuje</param>
            public ReflectionPackageModelItem(Type type)
            {
                if (type == null)
                    throw new ArgumentNullException("type");

                this.Type = type;
                this.InternalInitializeProperty(type);
            }
            #endregion

            #region - Properties -
            /// <summary>
            /// Typ objektu
            /// </summary>
            public Type Type { get; set; }
            /// <summary>
            /// Atribut definujuci informacie o view prisluchajucemu k objektu
            /// </summary>
            public PackageGroupAttribute GroupAttribute
            {
                get
                {
                    if (this.m_viewAttribute == null)
                    {
                        Object[] attributes = this.Type.GetCustomAttributes(typeof(PackageGroupAttribute), true);
                        if (attributes != null && attributes.Length == 1)
                        {
                            this.m_viewAttribute = attributes[0] as PackageGroupAttribute;
                        }
                    }
                    return this.m_viewAttribute;
                }
            }
            #endregion

            #region - Variable -
            /// <summary>
            /// Atribut definujuci informacie o view prisluchajucemu k objektu
            /// </summary>
            private PackageGroupAttribute m_viewAttribute = null;
            #endregion

            #region - Public Methods -
            /// <summary>
            /// Vrati pozadovanu property a jej informacie na zaklade mena
            /// </summary>
            /// <param name="address">Meno property ktoru ziadame</param>
            /// <returns>ReflectionPropertyItem alebo null</returns>
            public PackageModelPropertyItem FindProperty(UInt32 address)
            {
                if (this.ContainsKey(address))
                {
                    return this[address];
                }
                return null;
            }
            #endregion

            #region - Private Methods -
            /// <summary>
            /// Inicializuje property objektu
            /// </summary>
            /// <param name="type">Typ objektu ktoreho property objekt reprezentuje</param>
            private void InternalInitializeProperty(Type type)
            {
                PropertyInfo[] properties = type.GetProperties();
                int length = properties.Length;
                for (int i = 0; i < length; i++)
                {
                    PropertyInfo property = properties[i];
                    PackageModelPropertyItem item = new PackageModelPropertyItem(property);
                    if (item.ItemAttribute != null)
                    {
                        this.Add(item.ItemAttribute.Address, item);
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// Cache data
        /// </summary>
        private sealed class ReflectionPackageModelCollection : Dictionary<UInt32, ReflectionPackageModelItem>
        {
            #region - Public Methods -
            /// <summary>
            /// Vrati alebo vytvori a vrati kolekciu property pre pozadovany typ objektu
            /// </summary>
            /// <param name="type">Model type</param>
            /// <returns>ReflectionPropertyItemCollection</returns>
            public ReflectionPackageModelItem FindPropertyCollection(Type type)
            {
                //get group attribude
                PackageGroupAttribute attribute = InternalGetGroupAttribute(type);
                if (attribute != null)
                {
                    if (!this.ContainsKey(attribute.Address))
                    {
                        return this.InternalCreateType(type);
                    }
                    return this[attribute.Address];
                }
                return null;
            }
            #endregion

            #region - Private Methods -
            /// <summary>
            /// Prida objekt pre pozadovany type
            /// </summary>
            /// <param name="type">Type objektu pre ktory chceme property nacitat</param>
            private ReflectionPackageModelItem InternalCreateType(Type type)
            {
                ReflectionPackageModelItem item = new ReflectionPackageModelItem(type);
                if (item.GroupAttribute != null)
                {
                    this.Add(item.GroupAttribute.Address, item);
                    return item;
                }
                return null;
            }
            /// <summary>
            /// This method returns group attribude from type
            /// </summary>
            /// <param name="type">Type</param>
            /// <returns>Group attribude</returns>
            private PackageGroupAttribute InternalGetGroupAttribute(Type type)
            {
                Object[] attributes = type.GetCustomAttributes(typeof(PackageGroupAttribute), true);
                if (attributes != null && attributes.Length == 1)
                {
                    return attributes[0] as PackageGroupAttribute;
                }
                return null;
            }
            #endregion
        }
        #endregion

        #region - Private Static Variable -
        /// <summary>
        /// Static variable for this item
        /// </summary>
        private static ReflectionPackageModelCollection mReflectionPropertyCollection;
        #endregion

        #region - Public Static Methods -
        /// <summary>
        /// This function calculate check sum from frame
        /// </summary>
        /// <param name="data">Data array</param>
        /// <param name="index">Start frame index</param>
        /// <param name="length">Frame length</param>
        /// <returns>Check sum</returns>
        public static Byte GetPackageDataCheckSum(Byte[] data, int index, int length)
        {
            int sum = 0;
            for (int currentIndex = index; currentIndex < (length + index); currentIndex++)
            {
                sum += (int)data[currentIndex];
            }
            sum += 0xA5;
            sum = sum & 0xFF;
            return (byte)(256 - sum);
        }
        /// <summary>
        /// This function registers model to cache
        /// </summary>
        /// <param name="type">Type</param>
        public static void RegisterModel(Type type)
        {
            PackageHelper.mReflectionPropertyCollection.FindPropertyCollection(type);
        }
        /// <summary>
        /// This function finds frame in array
        /// </summary>
        /// <param name="array">Input array data</param>
        /// <param name="action">Callback for parsing frame items</param>
        /// <returns>Frame | null</returns>
        public static Package FindPackage(List<Byte> array)
        {
            //variables
            int count = array.Count;
            int index = 0x00;

            //find start byte
            for (index = 0x00; index < count; index++)
            {
                //check start byte and length
                if (array[index] == 0x70)
                {
                    // check min length
                    if ((count - (index + 2)) < 2)
                    {
                        // no enough data
                        break;
                    }

                    //get length from array
                    UInt16 length = (UInt16)(array[index + 2] << 8 | array[index + 1]);

                    // version
                    Byte version = array[index + 0x03];

                    //get command from array
                    UInt16 address = (UInt16)(array[index + 5] << 8 | array[index + 4]);

                    //get state
                    Package.StateTypes state = (Package.StateTypes)array[index + 6];

                    // overime ci je dostatok dat na vytvorenie package
                    // package length + 0x01 (start byte)
                    if (count >= (length + 0x01))
                    {
                        //get data
                        Byte[] data = array.GetRange(index, length + 0x01).ToArray();

                        // parse frame
                        Package package = PackageHelper.InternalConstructPackage(length, version, address, state, data);
                        if (package != null)
                        {
                            // remove data
                            array.RemoveRange(0, index + 0x01 + length);

                            //return package
                            return package;
                        }
                    }
                    else
                    {
                        //remove first data
                        if (index > 0)
                        {
                            array.RemoveRange(0, index);
                        }

                        //nedostatok dat
                        return null;
                    }
                }            
            }

            //remove first data
            if (index > 0)
            {
                array.RemoveRange(0, index);
            }

            //any package
            return null;
        }
        /// <summary>
        /// This method constructs package from imput parameters
        /// </summary>
        /// <param name="length">Package length</param>
        /// <param name="version">Version number</param>
        /// <param name="address">Package address</param>
        /// <param name="state">Package state</param>
        /// <param name="data">Package data</param>
        /// <returns>Package | null</returns>
        private static Package InternalConstructPackage(UInt16 length, Byte version, UInt16 address, Package.StateTypes state, Byte[] data)
        {
            //get checksum
            Byte checkSum = PackageHelper.GetPackageDataCheckSum(data, 0x01, data.Length - 0x02);
            Byte currentCheckSum = data[data.Length - 0x01];
            if (checkSum != currentCheckSum)
            {
                return null;
            }

            // create data part
            int mLength = data.Length - /*header*/0x07 - /*LRC*/0x01;
            Byte[] mData = new Byte[mLength];
            Buffer.BlockCopy(data, 0x07, mData, 0x00, mLength);
 
            //initialize package
            return new Package(version, address, state, mData);
        }
        /// <summary>
        /// This methods gets item type for current package and group
        /// </summary>
        /// <param name="frameAddress">Package address</param>
        /// <param name="groupAddress">Grop address</param>
        /// <param name="itemAddress">Item address</param>
        /// <returns></returns>
        private static PackageItemTypes InternalGetPackageItemType(UInt16 frameAddress, UInt16 groupAddress, UInt32 itemAddress)
        {
            if (PackageHelper.mReflectionPropertyCollection != null)
            {
                if (PackageHelper.mReflectionPropertyCollection.ContainsKey(groupAddress))
                {
                    ReflectionPackageModelItem modelItem = PackageHelper.mReflectionPropertyCollection[groupAddress];
                    if (modelItem != null)
                    {
                        if (modelItem.ContainsKey(itemAddress))
                        {
                            PackageModelPropertyItem propertyItem = modelItem[itemAddress];
                            return propertyItem.ItemAttribute.Type;
                        }
                    }
                }
            }
            return PackageItemTypes.Unknown;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Group collection</returns>
        public static List<PackageItemGroup> SerializeToGroup<T>(List<T> data)
        {
            return PackageHelper.InternalSerializeToGroup<T>(data);
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Group collection</returns>
        public static PackageItemGroup SerializeToGroup<T>(T data)
        {
            return PackageHelper.InternalSerializeToGroup<T>(data);
        }
        /// <summary>
        /// Serializes the specified object to a Frame.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="address">Frame command address</param>
        /// <returns>Frame | null</returns>
        public static Package Serialize<T>(T obj, Byte version, UInt16 address)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            //create collection
            List<T> collection = new List<T>();
            collection.Add(obj);
 
            //serialize
            return Serialize<T>(collection, version, address);
        }
        /// <summary>
        /// Serializes the specified object to a Frame.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="version">
        /// <param name="address">Frame command address</param>
        /// <returns>Frame | null</returns>
        public static Package Serialize<T>(List<T> obj, Byte version, UInt16 address)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            //intiailize object
            Package result = new Package(version, address, Package.StateTypes.DATA);

            //serialize
            return Serialize<T>(obj, result);
        }
        /// <summary>
        /// Serializes the specified object to a Frame.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="frame">Frame</param>
        /// <returns>Frame | null</returns>
        public static Package Serialize<T>(List<T> obj, Package frame)
        {
            //intiailize object
            return InternalSerialize<T>(obj, frame);
        }
        /// <summary>
        /// Deserializes the Frame to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="package">The Frame to deserialize.</param>
        /// <returns>The deserialized object from the Frame.</returns>
        public static List<T> Deserialize<T>(Package package)
        {
            if (package == null)
                throw new ArgumentNullException("frame");

            return PackageHelper.InternalDeserialize<T>(package);
        }
        /// <summary>
        /// Deserializes the Group to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="group">The Group to deserialize.</param>
        /// <returns>The deserialized object from the Group.</returns>
        public static List<T> Deserialize<T>(PackageItemGroup group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            return PackageHelper.InternalDeserialize<T>(group);
        }
        #endregion

        #region - Private Static Methods -
        /// <summary>
        /// Serializes the specified Object to Frame
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="frame">Frame</param>
        /// <returns>Frame | null</returns>
        private static Package InternalSerialize<T>(List<T> obj, Package frame)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return PackageHelper.InternalSerialize<T>(obj, frame, reflection);
            }
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Group collection</returns>
        private static PackageItemGroup InternalSerializeToGroup<T>(T data)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return PackageHelper.InternalSerializeToGroup<T>(data, reflection);
            }
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Group collection</returns>
        private static List<PackageItemGroup> InternalSerializeToGroup<T>(List<T> data)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return PackageHelper.InternalSerializeToGroup<T>(data, reflection);
            }
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <param name="reflection">reflection info for this data type</param>
        /// <returns>Group collection</returns>
        private static List<PackageItemGroup> InternalSerializeToGroup<T>(List<T> data, ReflectionType reflection)
        {
            //get group attribute
            PackageGroupAttribute groupAttribute = reflection.GetCustomAttribute<PackageGroupAttribute>();

            //check group attribute
            if (groupAttribute != null)
            {
                //get collection
                List<PackageItemGroup> results = new List<PackageItemGroup>();

                //loop all object
                foreach (T obj in data)
                {
                    //serialize object
                    PackageItemGroup group = new PackageItemGroup(groupAttribute.Address);
                        
                    // serialize to group
                    PackageHelper.InternalSerializeToGroup(obj, group, reflection);

                    // add to results
                    results.Add(group);
                }

                //return groups
                return results;
            }

            //no data
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <param name="reflection">reflection info for this data type</param>
        /// <returns>Group | null</returns>
        private static PackageItemGroup InternalSerializeToGroup<T>(T data, ReflectionType reflection)
        {
            //get group attribute
            PackageGroupAttribute groupAttribute = reflection.GetCustomAttribute<PackageGroupAttribute>();

            //check group attribute
            if (groupAttribute != null)
            {
                //serialize object
                PackageItemGroup group = new PackageItemGroup(groupAttribute.Address);

                // serialize to group
                PackageHelper.InternalSerializeToGroup(data, group, reflection);

                // add to results
                return group;
            }

            //no data
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <param name="reflection">reflection info for this data type</param>
        /// <param name="groupAttribute">Current group attribute</param>
        private static void InternalSerializeToGroup(Object data, PackageItemGroup group, ReflectionType reflection)
        {
            //variables
            Object value = null;

            //set property
            foreach (ReflectionProperty reflectionItem in reflection.PropertyCollection.Values)
            {
                //check property type
                if (reflectionItem.Property.CanRead)
                {
                    //get current attribute
                    PackageItemAttribute attribute = reflectionItem.GetCustomAttribute<PackageItemAttribute>();
                    if (attribute != null)
                    {
                        try
                        {
                            // check group type
                            if (attribute.Type == PackageItemTypes.Group)
                            {
                                // get value
                                value = reflectionItem.Property.GetValue(data);

                                // initialize value
                                if (value == null)
                                {
                                    continue;
                                }

                                // check list collection
                                Type type = reflectionItem.Property.PropertyType;

                                // check list
                                if (!type.IsGenericListType())
                                {
                                    throw new Exception(String.Format("Property for group 0x{0:X8} is not defines as List<>", attribute.Address));
                                }

                                // get item type
                                Type itemType = type.GetGenericArguments()[0x00];

                                // get reflection for this item
                                ReflectionType childReflection = ReflectionHelper.GetType(itemType);
                                if (childReflection != null)
                                {
                                    //get group attribute
                                    PackageGroupAttribute groupAttribute = childReflection.GetCustomAttribute<PackageGroupAttribute>();

                                    //check group attribute
                                    if (groupAttribute != null)
                                    {
                                        foreach (var listItem in (value as IEnumerable))
                                        {
                                            // create new group
                                            PackageItemGroup childGroup = new PackageItemGroup(groupAttribute.Address);

                                            // serialize object
                                            PackageHelper.InternalSerializeToGroup(listItem, childGroup, childReflection);

                                            // add group
                                            group.Add(childGroup);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                value = reflectionItem.Property.GetValue(data, null);
                                if (value != null)
                                {
                                    IPackageItem frameItem = Package.CreatePackageItem(attribute.Type, attribute.Address, value);
                                    if (frameItem != null)
                                    {
                                        group.Add(frameItem);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(String.Format("Error type: {0} -> {1} [{2}]", attribute.Type, reflectionItem.Property.Name, value), ex);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Serializes the specified Object to Frame
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="data">The object to serialize.</param>
        /// <param name="frame">Frame</param>
        /// <param name="reflection">Reflection information for the Object to deserialize</param>
        /// <returns>Frame | null</returns>
        private static Package InternalSerialize<T>(List<T> data, Package frame, ReflectionType reflection)
        {
            //get group attribute
            PackageGroupAttribute groupAttribute = reflection.GetCustomAttribute<PackageGroupAttribute>();

            //check attribute
            if (groupAttribute != null)
            {
                //each object
                foreach (T obj in data)
                {
                    // create new group
                    PackageItemGroup group = new PackageItemGroup(groupAttribute.Address);

                    //serialize group
                    PackageHelper.InternalSerializeToGroup(obj, group, reflection);

                    //create group to frame
                    frame.Add(group);
                }
            }

            //return result
            return frame;
        }
        /// <summary>
        /// Deserializes the Frame to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="package">The Frame to deserialize.</param>
        /// <returns>The deserialized object from the Frame.</returns>
        private static List<T> InternalDeserialize<T>(Package package)
        {
            // result
            List<T> results = new List<T>();
            foreach (IPackageItem item in package.Items)
            {
                if (item.ItemType == PackageItemTypes.Group && item is PackageItemGroup group)
                {
                    List<T> childResults = PackageHelper.InternalDeserialize<T>(group);
                    if (childResults != null && childResults.Count > 0x00)
                    {
                        results.AddRange(childResults);
                    }
                }
            }
            return results;
        }
        /// <summary>
        /// Deserializes the Group to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="group">The Group to deserialize.</param>
        /// <returns>The deserialized object from the Group.</returns>
        private static List<T> InternalDeserialize<T>(PackageItemGroup group)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return PackageHelper.InternalDeserialize<T>(group, reflection);
            }
            return default(List<T>);
        }
        /// <summary>
        /// This method deserializes data to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="item">Destination item</param>
        /// <param name="group">Items with data</param>
        /// <param name="reflection">Reflection information for the Object to deserialize</param>
        private static void InternalDeserialize(Object item, PackageItemGroup group, ReflectionType reflection)
        {
            //set property
            foreach (ReflectionProperty reflectionItem in reflection.PropertyCollection.Values)
            {
                //check property type
                if (reflectionItem.Property.CanWrite)
                {
                    //get current attribute
                    PackageItemAttribute attribute = reflectionItem.GetCustomAttribute<PackageItemAttribute>();
                    if (attribute != null)
                    {
                        // check group type
                        if (attribute.Type == PackageItemTypes.Group)
                        {
                            // check list collection
                            Type type = reflectionItem.Property.PropertyType;

                            // check list
                            if (!type.IsGenericListType())
                            {
                                throw new Exception(String.Format("Property for group 0x{0:X8} is not defines as List<>", attribute.Address));
                            }

                            // get value
                            Object value = reflectionItem.Property.GetValue(item);

                            // initialize value
                            if (value == null)
                            {
                                value = Activator.CreateInstance(type);
                                reflectionItem.Property.SetValue(item, value, null);
                            }

                            // get item type
                            Type itemType = type.GetGenericArguments()[0x00];

                            // get reflection for this item
                            ReflectionType childReflection = ReflectionHelper.GetType(itemType);
                            if (childReflection != null)
                            {
                                // loop all items
                                foreach (IPackageItem childItem in group.Items)
                                {
                                    // check this item
                                    if (childItem.Address == attribute.Address && childItem.ItemType == PackageItemTypes.Group && childItem is PackageItemGroup childGroup)
                                    {
                                        // intiailize object
                                        Object result = Activator.CreateInstance(itemType);

                                        // deserialize
                                        PackageHelper.InternalDeserialize(result, childGroup, childReflection);

                                        // add item to list
                                        type.GetMethod("Add").Invoke(value, new[] { result });
                                    }
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                //get vale from frame
                                Object value = group.GetValue<Object>(attribute.Address);

                                //update value
                                value = PackageHelper.InternalUpdateValue(attribute.Type, reflectionItem.Property.PropertyType, value);

                                //set value to property
                                reflectionItem.Property.SetValue(item, value, null);
                            }
                            catch (Exception ex)
                            {
                                //throw new Exception(String.Format("Error type: {0} -> {1}.{2}", (value == null ? "NULL" : value.ToString()), typeof(T), item.Property.Name), ex);
                                throw ex;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Deserializes the Group to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="group">The Group to deserialize.</param>
        /// <param name="reflection">Reflection information for the Object to deserialize</param>
        /// <returns>The deserialized object from the Group.</returns>
        private static List<T> InternalDeserialize<T>(PackageItemGroup group, ReflectionType reflection)
        {
            // results
            List<T> results = new List<T>();

            // get attribute
            PackageGroupAttribute groupAttribute = reflection.GetCustomAttribute<PackageGroupAttribute>();

            // check group and address
            if (groupAttribute != null && group.Address == groupAttribute.Address)
            {
                // intiailize object
                T result = (T)Activator.CreateInstance(typeof(T));

                // deserialize object
                PackageHelper.InternalDeserialize(result, group, reflection);

                // add result
                results.Add(result);
            }
            else
            {
                // loop all items
                foreach (IPackageItem item in group.Items)
                {
                    // check group
                    if (item.ItemType == PackageItemTypes.Group && item is PackageItemGroup childGroup)
                    {
                        List<T> childResults = PackageHelper.InternalDeserialize<T>(childGroup, reflection);
                        if (childResults != null && childResults.Count > 0x00)
                        {
                            results.AddRange(childResults);
                        }
                    }
                }
            }

            // return all results
            return results;
        }
        /// <summary>
        /// Deserializes the Frame to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="frame">The Frame to deserialize.</param>
        /// <param name="reflection">Reflection information for the Object to deserialize</param>
        /// <returns>The deserialized object from the Frame.</returns>
        private static List<T> InternalDeserialize<T>(Package frame, ReflectionType reflection)
        {
            /*
            //get group attribute
            PackageGroupAttribute PackageGroupAttribute = reflection.GetCustomAttribute<PackageGroupAttribute>();
            if (PackageGroupAttribute == null)
            {
                throw new Exception(String.Format("Type {0} does not contain FrameGroup attribute!", reflection.Type.Name));
            }

            //create collection
            List<T> collection = new List<T>();

            //each objects
            foreach (PackageItemGroup group in frame.Items)
            {
                //check group address for this type
                if (group.Address == PackageGroupAttribute.Address)
                {
                    //intiailize object
                    T result = PackageHelper.InternalDeserialize<T>(group, reflection);

                    //create objet to collection
                    collection.Add(result);
                }
            }

            //return result
            return collection;
            */
            throw new Exception("XXXXXXXX");
        }
        /// <summary>
        /// This method updates or converts value to target type
        /// </summary>
        /// <param name="type">Frame item type</param>
        /// <param name="targetType">Target property type</param>
        /// <param name="value">Value to convert</param>
        /// <returns></returns>
        private static Object InternalUpdateValue(PackageItemTypes type, Type targetType, Object value)
        {
            //check value
            if (value != null)
            {
                //check type
                if (type == PackageItemTypes.String && targetType == typeof(Guid) && value is String)
                {
                    Nullable<Guid> guidValue = (value as String).ToGuidWithoutDash();
                    return guidValue.Value;
                }
                else if (type == PackageItemTypes.Enum)
                {
                    if (Utility.IsNullableType(targetType))
                    {
                        targetType = Nullable.GetUnderlyingType(targetType);
                    }
                    if (!targetType.IsEnum)
                    {
                        throw new Exception(String.Format("TargetType is not enum type. [{0}]", targetType));
                    }
                    return Enum.ToObject(targetType, value);
                }
            }
            return value;
        }
        #endregion
    }
}
