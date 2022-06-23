using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Dynamic;
using Npgsql;
using project858.Diagnostics;

namespace project858.Data.SqlClient
{
    /// <summary>
    /// Klient zabezpecujuci vykonavanie prikazov do SQL databazy 
    /// </summary>
    public class PostgreSQLClient : PostgreSQLClientBase
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="builder">Strng builder na vytvorenie SQL connection stringu</param>
        public PostgreSQLClient(NpgsqlConnectionStringBuilder builder)
            : base(builder)
        {
            this.mLockObj = new Object();
            this.mReflectionPropertyCollection = new ReflectionModelItemCollection();
        }
        #endregion

        #region - Class -
        /// <summary>
        /// Polozka zahrnucjuca reglekciu objektu a jeho property
        /// </summary>
        private sealed class ReflectionPropertyItem
        {
            #region - Constructor -
            /// <summary>
            /// Initialize this class
            /// </summary>
            /// <param name="property">Property ktoru objekt reprezentuje</param>
            public ReflectionPropertyItem(PropertyInfo property)
            {
                this.Property = property ?? throw new ArgumentNullException("property");
            }
            #endregion

            #region - Properties -
            /// <summary>
            /// Primary key attribute for this property
            /// </summary>
            public PrimaryKeyAttribute PrimaryKeyAttribute
            {
                get
                {
                    if (this.mPrimaryKeyAttributes == null)
                    {
                        Object[] attributes = this.Property.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
                        if (attributes != null && attributes.Length == 1)
                        {
                            this.mPrimaryKeyAttributes = attributes[0] as PrimaryKeyAttribute;
                        }
                    }
                    return this.mPrimaryKeyAttributes;
                }
            }
            /// <summary>
            /// Vrati atributy prisluchajucej property
            /// </summary>
            public ColumnAttribute ColumnAttribute
            {
                get
                {
                    if (this.mColumnAttributes == null)
                    {
                        Object[] attributes = this.Property.GetCustomAttributes(typeof(ColumnAttribute), true);
                        if (attributes != null && attributes.Length == 1)
                        {
                            this.mColumnAttributes = attributes[0] as ColumnAttribute;
                        }
                    }
                    return this.mColumnAttributes;
                }
            }
            /// <summary>
            /// Property ktoru objekt reprezentuje
            /// </summary>
            public PropertyInfo Property
            {
                get;
                private set;
            }
            #endregion

            #region - Variable -
            /// <summary>
            /// This column is primary key
            /// </summary>
            private PrimaryKeyAttribute mPrimaryKeyAttributes = null;
            /// <summary>
            /// This column is database column
            /// </summary>
            private ColumnAttribute mColumnAttributes = null;
            #endregion
        }
        /// <summary>
        /// Kolekcia property prisluchajuca konkretnemu objektu
        /// </summary>
        private sealed class ReflectionModelItem : Dictionary<String, ReflectionPropertyItem>
        {
            #region - Constructor -
            /// <summary>
            /// Initialize this class
            /// </summary>
            /// <param name="type">Typ objektu ktoreho property objekt reprezentuje</param>
            public ReflectionModelItem(Type type)
            {
                // set base data
                this.Type = type ?? throw new ArgumentNullException("type");
                this.InternalInitializeProperty(type);

                // check table attribude
                if (this.TableAttribute == null)
                {
                    throw new Exception("The model does not contain table attribute.");
                }

                // get primary key
                if (this.InternalGetPrimaryKeyCount() > 0x01)
                {
                    throw new Exception("The model contains more than one primary key attribute.");
                }

                // check count
                if (this.Values.Count == 0x00)
                {
                    throw new Exception("The model does not contain column attributes.");
                }
            }
            #endregion

            #region - Properties -
            /// <summary>
            /// Typ objektu
            /// </summary>
            public Type Type { get; private set; }
            /// <summary>
            /// Atribut definujuci informacie o tabulke prisluchajucej k objekte
            /// </summary>
            public TableAttribute TableAttribute
            {
                get
                {
                    if (this.mTableAttribute == null)
                    {
                        Object[] attributes = this.Type.GetCustomAttributes(typeof(TableAttribute), true);
                        if (attributes != null && attributes.Length == 1)
                        {
                            this.mTableAttribute = attributes[0] as TableAttribute;
                        }
                    }
                    return this.mTableAttribute;
                }
            }
            #endregion

            #region - Variable -
            /// <summary>
            /// Atribut definujuci informacie o tabulke prisluchajucej k objekte
            /// </summary>
            private TableAttribute mTableAttribute = null;
            #endregion

            #region - Public Methods -
            /// <summary>
            /// This method returns property with primary key attribude
            /// </summary>
            /// <returns>Property information</returns>
            public ReflectionPropertyItem FindPrimaryKeyProperty()
            {
                foreach (ReflectionPropertyItem item in this.Values)
                {
                    if (item.PrimaryKeyAttribute != null)
                    {
                        return item;
                    }
                }
                return null;
            }
            /// <summary>
            /// Vrati pozadovanu property a jej informacie na zaklade mena
            /// </summary>
            /// <param name="name">Meno property ktoru ziadame</param>
            /// <returns>ReflectionPropertyItem alebo null</returns>
            public ReflectionPropertyItem FindProperty(String name)
            {
                if (this.ContainsKey(name))
                {
                    return this[name];
                }
                return null;
            }
            #endregion

            #region - Private Methods -
            /// <summary>
            /// This method returns number of columns with primary key attribude
            /// </summary>
            /// <returns>Count</returns>
            private UInt32 InternalGetPrimaryKeyCount()
            {
                UInt32 count = 0x00;
                foreach (ReflectionPropertyItem item in this.Values)
                {
                    if (item.PrimaryKeyAttribute != null)
                    {
                        count += 0x01;
                    }
                }
                return count;
            }
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
                    ReflectionPropertyItem item = new ReflectionPropertyItem(property);
                    this.Add(property.Name, item);
                }
            }
            #endregion
        }
        /// <summary>
        /// Kolekcia objektov definovanych typom obsahujucich informacie o properties
        /// </summary>
        private sealed class ReflectionModelItemCollection : Dictionary<Type, ReflectionModelItem>
        {
            #region - Public Methods -
            /// <summary>
            /// Vrati alebo vytvori a vrati kolekciu property pre pozadovany typ objektu
            /// </summary>
            /// <param name="type">Typ objektu pre ktory chceme informacie vratit</param>
            /// <returns>ReflectionPropertyItemCollection</returns>
            public ReflectionModelItem FindPropertyCollection(Type type)
            {
                if (!this.ContainsKey(type))
                {
                    this.InternalCreateType(type);
                }
                return this[type];
            }
            #endregion

            #region - Private Methods -
            /// <summary>
            /// Prida objekt pre pozadovany type
            /// </summary>
            /// <param name="type">Type objektu pre ktory chceme property nacitat</param>
            private void InternalCreateType(Type type)
            {
                ReflectionModelItem item = new ReflectionModelItem(type);
                this.Add(type, item);
            }
            #endregion
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Pomocny synchronizacny objekt na pristup k pripojeniu
        /// </summary>
        private readonly Object mLockObj = null;
        /// <summary>
        /// Buffer na ukladanie reglekcii objektov
        /// </summary>
        private ReflectionModelItemCollection mReflectionPropertyCollection = null;
        #endregion

        #region - Public Method -
        /// <summary>
        /// Vykona pozadovany query prikaz
        /// </summary>
        /// <param name="query">Query prikaz</param>
        /// <returns>Pocet ovplyvnenych riadkov</returns>
        public int ExecuteNonQuery(String query)
        {
            return this.InternalExecuteNonQuery(query);
        }
        /// <summary>
        /// Vykona vymazanie objektu
        /// </summary>
        /// <param name="item">Objekt ktory chceme vymazat</param>
        /// <returns>True = objekt bol vymazany, inak false</returns>
        public int DeleteObject(Object item)
        {
            try
            {
                return this.InternalDeleteObject(item);
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri vymazavani objektu z SQL {0} [{1} : {2}]", ex.Message, item.GetType(), item.ToJsonString());
                throw;
            }
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="whereClause">Where podmienka</param>
        /// <param name="orderClause">Order klauzula</param>
        /// <param name="limit">Urcuje maximalne mnozstvo poloziek ktore chceme nacitat</param>
        /// <param name="page">Stranka dat</param>
        /// <param name="excludeColumns">Columns which are not required in this request</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        public List<T> Select<T>(String whereClause = null, String orderClause = null, Nullable<UInt32> limit = null, Nullable<UInt32> page = null, params String[] excludeColumns)
        {
            if (limit != null && limit.Value < 1)
            {
                throw new ArgumentException("Value 'limit' cannot be less than the minimum value 1");
            }
            return this.InternalSelect<T>(whereClause, orderClause, limit, page, excludeColumns);
        }
        /// <summary>
        /// Nacita pocet dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="whereClause">Where podmienka</param>
        /// <returns>Pocet dat</returns>
        public Int32 SelectCount<T>(String whereClause = null)
        {
            return this.InternalSelectCount<T>(whereClause);
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="query">Query ktorym chceme nacitat data</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        public List<T> SelectFromQuery<T>(String query)
        {
            return this.InternalSelectFromQuery<T>(query);
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="whereClause">Where podmienka</param>
        /// <param name="orderClause">Order klauzula</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        public T SelectFirstObject<T>(String whereClause = null, String orderClause = null)
        {
            return this.InternalSelectFirstObject<T>(whereClause, orderClause);
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="query">Query ktorym chceme nacitat data</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        public T SelectFirstObjectFromQuery<T>(String query)
        {
            return this.InternalSelectFirstObjectFromQuery<T>(query);
        }
        /// <summary>
        /// Vlozi pozadovany objekt do SQL
        /// </summary>
        /// <param name="item">objekt ktorych chceme vlozit</param>
        /// <returns>True = objekt bol uspesne vlozeny</returns>
        public Boolean TryInsertObject(Object item)
        {
            try
            {
                var result = this.InternalInsertObject(item);
                return result == 1;
            }
            catch (Exception ex)
            {
                //trace message
                this.InternalException(ex);
                return false;
            }
        }
        /// <summary>
        /// Vlozi pozadovany objekt do SQL
        /// </summary>
        /// <param name="item">objekt ktorych chceme vlozit</param>
        public int InsertObject(Object item)
        {
            try
            {
                return this.InternalInsertObject(item);
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri vkladani objektu do SQL {0} [{1} : {2}]", ex.Message, item.GetType(), item.ToJsonString());
                throw;
            }
        }
        /// <summary>
        /// Insertne kolekciu dat do DB
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme insertnut</typeparam>
        /// <param name="collection">Kolekcia dat</param>
        /// <returns>Pocet riadkov ovplyvnenych insertom</returns>
        public int InsertCollection<T>(List<T> collection)
        {
            return this.InternalInsertCollection<T>(collection);
        }
        /// <summary>
        /// Aktualizuje pozadovany objekt v SQL
        /// </summary>
        /// <param name="item">objekt ktorych chceme aktualizovat</param>
        /// <param name="whereClause">Podmienka na aktualizaciu objektu</param>
        /// <returns>True = objekt bol uspesne aktualizovany</returns>
        public Boolean TryUpdateObject(Object item, String whereClause = null)
        {
            try
            {
                var result = this.InternalUpdateObject(item, whereClause);
                return result == 1;
            }
            catch (Exception ex)
            {
                //trace message
                this.InternalException(ex);
                return false;
            }
        }
        /// <summary>
        /// Aktualizuje pozadovany objekt v SQL
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme aktualizovat</typeparam>
        /// <param name="item">objekt ktorych chceme aktualizovat</param>
        /// <param name="whereClause">Podmienka na aktualizaciu objektu</param>
        public int UpdateObject(Object item, String whereClause = null)
        {
            try
            {
                return this.InternalUpdateObject(item, whereClause);
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri aktualizacii objektu v SQL {0} [{1} : {2}]", ex.Message, item.GetType(), item.ToJsonString());
                throw;
            }
        }
        /// <summary>
        /// Aktualizuje a nacita pozadovany objekt v / z SQL
        /// </summary>
        /// <param name="item">Objekt ktory chceme aktualizovat</param>
        /// <returns>Aktualizovany objekt</returns>
        public Object UpdateAndReloadObject(Object item)
        {
            try
            {
                return this.InternalUpdateAndReloadObject(item);
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri aktualizacii a obnove objektu v / z SQL {0} [{1} : {2}]", ex.Message, item.GetType(), item.ToJsonString());
                throw;
            }
        }
        /// <summary>
        /// Nacita objekt z sql readera
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="reader">Reader pomocou ktoreho citame data</param>
        /// <returns>Objekt ktory necitavame</returns>
        public T GetObject<T>(NpgsqlDataReader reader)
        {
            return this.InternalGetObject<T>(reader, Activator.CreateInstance<T>());
        }
        /// <summary>
        /// Nacita objekty z pozadovaneho commandu
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="command">NpgsqlCommand</param>
        /// <returns>Kolekcia objektov alebo null</returns>
        public List<T> ReadObjectCollection<T>(NpgsqlCommand command)
        {
            //inicializujeme
            List<T> collection = new List<T>();

            //synchronizacia
            lock (this.mLockObj)
            {
                using (NpgsqlDataReader reader = this.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        T item = this.GetObject<T>(reader);
                        collection.Add(item);
                    }
                }
            }

            //vratime nacitanu kolekciu
            return collection;
        }
        /// <summary>
        /// Nacita prvy objekt alebo vrati null
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="command">NpgsqlCommand</param>
        /// <returns>Object alebo null</returns>
        public T ReadFirstObject<T>(NpgsqlCommand command)
        {
            //inicializujeme
            List<T> collection = new List<T>();

            //synchronizacia
            lock (this.mLockObj)
            {
                using (NpgsqlDataReader reader = this.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        T item = this.GetObject<T>(reader);
                        return item;
                    }
                }
            }

            //vratime default
            return default(T);
        }
        /// <summary>
        /// Vrati pocet data
        /// </summary>
        /// <param name="whereClause">Podmienka pre ziskanie poctu</param>
        /// <returns>Pocet dat</returns>
        public int GetCount<T>(String whereClause = null)
        {
            return this.InternalGetCount<T>(whereClause);
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
            return base.InternalStart();
        }
        /// <summary>
        /// Ukonci funkciu klienta
        /// </summary>
        protected override void InternalStop()
        {
            //ukoncime komunikaciu
            base.InternalStop();
        }
        /// <summary>
        /// Vykona pred ukoncenim klienta
        /// </summary>
        protected override void InternalDispose()
        {
            base.InternalDispose();
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// Vrati pocet data
        /// </summary>
        /// <param name="whereClause">Podmienka pre ziskanie poctu</param>
        /// <returns>Pocet dat</returns>
        public int InternalGetCount<T>(String whereClause = null)
        {
            //premapujeme datovy objekt
            ReflectionModelItem properties = this.mReflectionPropertyCollection.FindPropertyCollection(typeof(T));

            //vytvorime command
            String query = String.Format("SELECT COUNT(*) FROM [{0}]{1}", properties.TableAttribute.Name,
                 (String.IsNullOrWhiteSpace(whereClause) ? String.Empty : String.Format(" WHERE {0}", whereClause)));

            //spracujeme command do SQL
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                //vykoname prikaz
                command.CommandText = query;
                Object value = this.ExecuteScalar(command);
                if (value is int)
                {
                    return (int)value;
                }
                return 0;
            }
        }
        /// <summary>
        /// Vykona pozadovany query prikaz
        /// </summary>
        /// <param name="query">Query prikaz</param>
        /// <returns>Pocet ovplyvnenych riadkov</returns>
        public int InternalExecuteNonQuery(String query)
        {
            //spracujeme dommand do SQL
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                //vykoname prikad
                command.CommandText = query;
                return this.ExecuteNonQuery(command);
            }
        }
        /// <summary>
        /// Aktualizuje a nacita pozadovany objekt v / z SQL
        /// </summary>
        /// <param name="item">Objekt ktory chceme aktualizovat</param>
        /// <returns>Aktualizovany objekt</returns>
        private Object InternalUpdateAndReloadObject(Object item)
        {
            this.InternalUpdateObject(item);
            return this.InternalReloadObject(item);
        }
        /// <summary>
        /// Vykona vymazanie objektu
        /// </summary>
        /// <param name="item">Objekt ktory chceme vymazat</param>
        /// <returns>True = objekt bol vymazany, inak false</returns>
        private int InternalDeleteObject(Object item)
        {
            //najdeme informacie o datovom type
            ReflectionModelItem properties = this.mReflectionPropertyCollection.FindPropertyCollection(item.GetType());

            // check primary key
            ReflectionPropertyItem primaryKey = properties.FindPrimaryKeyProperty();
            if (primaryKey == null)
            {
                throw new Exception("Update requires column with primary key.");
            }
 
            //vytvorime command
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                //vytvorime jednotlive polozky commandu
                command.CommandText = String.Format("DELETE FROM [{0}] WHERE [{1}] = @{1}", properties.TableAttribute.Name, primaryKey.Property.Name);
                NpgsqlParameter primaryKeyParameter = new NpgsqlParameter(primaryKey.Property.Name, primaryKey.ColumnAttribute.Type);
                primaryKeyParameter.Value = this.InternalPrepareValue(primaryKey.Property.GetValue(item, null), primaryKey.ColumnAttribute);
                command.Parameters.Add(primaryKeyParameter);

                //vykoname priklad do DB
                return this.ExecuteNonQuery(command);
            }
        }
        /// <summary>
        /// Vykona aktualizaciu objektu
        /// </summary>
        /// <param name="item">Objekt ktory chceme aktualizovat</param>
        /// <returns>Aktualizovany objekt</returns>
        private Object InternalReloadObject(Object item)
        {
            //najdeme informacie o datovom type
            ReflectionModelItem properties = this.mReflectionPropertyCollection.FindPropertyCollection(item.GetType());

            // check primary key
            ReflectionPropertyItem primaryKey = properties.FindPrimaryKeyProperty();
            if (primaryKey == null)
            {
                throw new Exception("Update requires column with primary key.");
            }

            //synchronizacia
            lock (this.mLockObj)
            {
                //vytvorime command
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    //vytvorime jednotlive polozky commandu
                    command.CommandText = String.Format("SELECT * FROM [{0}] WHERE [{1}] = @{1}", properties.TableAttribute.Name, primaryKey.Property.Name);
                    NpgsqlParameter primaryKeyParameter = new NpgsqlParameter(primaryKey.Property.Name, primaryKey.ColumnAttribute.Type);
                    primaryKeyParameter.Value = this.InternalPrepareValue(primaryKey.Property.GetValue(item, null), primaryKey.ColumnAttribute);
                    command.Parameters.Add(primaryKeyParameter);

                    //nacitame data
                    using (NpgsqlDataReader reader = this.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            return this.InternalGetObject(reader, item);
                        }
                    }
                }
            }

            //vratime aktualnu hodnotu
            throw new InvalidOperationException("Object is not available !");
        }
        /// <summary>
        /// Nacita objekt z sql readera
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="reader">Reader pomocou ktoreho citame data</param>
        /// <param name="item">Objekt ktoryc chceme aktualizovat</param>
        /// <returns>Objekt ktory necitavame</returns>
        private T InternalGetObject<T>(NpgsqlDataReader reader, T item)
        {
            String name = String.Empty;
            ReflectionModelItem properties = this.mReflectionPropertyCollection.FindPropertyCollection(item.GetType());
            if (properties != null)
            {
                Object value = null;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    name = reader.GetName(i);
                    ReflectionPropertyItem property = properties.FindProperty(name);
                    if (property != null && property.Property.CanRead)
                    {
                        if (property.ColumnAttribute != null)
                        {
                            value = reader[name];
                            try
                            {
                                value = ((value == null || value == DBNull.Value) ? null : value);
                                if (value == null && !property.ColumnAttribute.CanBeNull)
                                {
                                    throw new InvalidCastException(String.Format("Specified cast is not valid. [{0} - {1} - {2}]", item.GetType().Name, property.Property.Name, property.Property.PropertyType));
                                }
                                value = this.InternalUpdateValue(value);
                                property.Property.SetValue(item, value, null);
                            }
                            catch (Exception ex)
                            {
                                this.InternalTrace(TraceTypes.Error, "Chyba pri citani dat z SQL. {0} [{1} - {2}]", ex.Message, property.Property.Name, property.Property.PropertyType);
                                throw;
                            }
                        }
                    }
                }
            }
            return item;
        }
        /// <summary>
        /// Aktualizuje hodnotu skor ako dojde k jej spracovaniu
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme aktualizovat</param>
        /// <returns>Aktualizovana hodnota alebo povodna hodnota</returns>
        private Object InternalUpdateValue(Object value)
        {
            if (value != null)
            {
                if (value is String)
                {
                    return ((String)value).Trim();
                }
            }
            return value;
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="whereClause">Where podmienka</param>
        /// <param name="orderClause">Order klauzula</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        private T InternalSelectFirstObject<T>(String whereClause, String orderClause)
        {
            //ziskame informacie o objekte
            ReflectionModelItem properties = this.mReflectionPropertyCollection.FindPropertyCollection(typeof(T));

            //overime whereClause 
            if (!String.IsNullOrWhiteSpace(whereClause))
            {
                //overime obsah
                if (whereClause.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    whereClause.IndexOf(" ASC ", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    whereClause.IndexOf(" DESC ", StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    throw new Exception(String.Format("Whereclause can't contains order by part. [{0}]", whereClause));
                }
            }
            //overime orderClause 
            if (!String.IsNullOrWhiteSpace(orderClause))
            {
                //overime obsah
                if (orderClause.IndexOf("WHERE", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    orderClause.IndexOf(">", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    orderClause.IndexOf("<", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    orderClause.IndexOf("=", StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    throw new Exception(String.Format("OrderClause can't contains where part. [{0}]", orderClause));
                }
            }
 

            //spracujeme dommand do SQL
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                //get table name
                String tableName = properties.TableAttribute.Name;

                //create command text
                String commandText = String.Format("SELECT TOP 1 * FROM [{0}]{1}{2}", tableName, (String.IsNullOrWhiteSpace(whereClause) ? String.Empty : String.Format(" WHERE {0}", whereClause)), (String.IsNullOrWhiteSpace(orderClause) ? String.Empty : String.Format(" ORDER BY {0}", orderClause)));
                command.CommandText = commandText;

                //read object
                return this.ReadFirstObject<T>(command);
            }
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="query">Query ktorym chceme nacitat data</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        private T InternalSelectFirstObjectFromQuery<T>(String query)
        {
            //overime query
            if (String.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException("query");
            }

            //spracujeme dommand do SQL
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                //vykoname prikad
                command.CommandText = query;
                return this.ReadFirstObject<T>(command);
            }
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="query">Query ktorym chceme nacitat data</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        private List<T> InternalSelectFromQuery<T>(String query)
        {
            //overime query
            if (String.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException("query");
            }

            //spracujeme dommand do SQL
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                //vykoname prikad
                command.CommandText = query;
                return this.ReadObjectCollection<T>(command);
            }
        }
        /// <summary>
        /// Nacita pocet dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="whereClause">Where podmienka</param>
        /// <returns>Pocet dat alebo null</returns>
        private Int32 InternalSelectCount<T>(String whereClause)
        {
            //ziskame informacie o objekte
            ReflectionModelItem properties = this.mReflectionPropertyCollection.FindPropertyCollection(typeof(T));

            //overime whereClause 
            if (!String.IsNullOrWhiteSpace(whereClause))
            {
                //overime obsah
                if (whereClause.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    whereClause.IndexOf(" ASC ", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    whereClause.IndexOf(" DESC ", StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    throw new Exception("Whereclause can't contains order by part");
                }
            }

            //spracujeme dommand do SQL
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                //vytvorime command
                String commandText = String.Format("SELECT COUNT(*) FROM [{0}]{1}", properties.TableAttribute.Name,
                    (String.IsNullOrWhiteSpace(whereClause) ? String.Empty : String.Format(" WHERE {0}", whereClause)));

                //vykoname prikad
                command.CommandText = commandText;
                return (Int32)this.ExecuteScalar(command);
            }
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="whereClause">Where podmienka</param>
        /// <param name="orderClause">Order klauzula</param>
        /// <param name="limit">Max limit for reading rows</param>
        /// <param name="page">offset or page for reading data</param>
        /// <param name="excludeColumns">Columns which are not required in this request</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        private List<T> InternalSelect<T>(String whereClause, String orderClause, Nullable<UInt32> limit, Nullable<UInt32> page, params String[] excludeColumns)
        {
            //ziskame informacie o objekte
            ReflectionModelItem properties = this.mReflectionPropertyCollection.FindPropertyCollection(typeof(T));

            //overime whereClause 
            if (!String.IsNullOrWhiteSpace(whereClause))
            {
                //overime obsah
                if (whereClause.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    whereClause.IndexOf(" ASC ", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    whereClause.IndexOf(" DESC ", StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    throw new Exception("Whereclause can't contains order by part");
                }
            }
            //overime orderClause 
            if (!String.IsNullOrWhiteSpace(orderClause))
            {
                //overime obsah
                if (orderClause.IndexOf("WHERE", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    orderClause.IndexOf(">", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    orderClause.IndexOf("<", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    orderClause.IndexOf("=", StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    throw new Exception("OrderClause can't contains where part");
                }
            }

            //spracujeme dommand do SQL
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                //vykoname prikad
                command.CommandText = this.InternalCreateCommandText(properties, whereClause, orderClause, limit, page, excludeColumns);
                return this.ReadObjectCollection<T>(command);
            }
        }
        /// <summary>
        /// This function returns columns text for command
        /// </summary>
        /// <param name="properties">All columns collection</param>
        /// <returns>Columns or String.Empty</returns>
        private String InternalGetColumnNamesForCommandText(List<ReflectionPropertyItem> properties)
        {
            //get column count
            int length = properties.Count;
            
            //builder
            StringBuilder builder = new StringBuilder();

            //create all cokumn
            for (int i = 0; i < length; i++)
            {
                builder.AppendFormat("[{0}]", properties[i].Property.Name);
                if (i < length - 1)
                {
                    builder.Append(",");
                }
            }

            //return columns
            return builder.ToString();
        }
        /// <summary>
        /// This method return filtered columns
        /// </summary>
        /// <param name="properties">Object type reflection</param>
        /// <param name="excludeColumns">Columns which are not required in this request</param>
        /// <returns>Filtered columns</returns>
        private List<ReflectionPropertyItem> InternalGetFilteredColumns(ReflectionModelItem properties, String[] excludeColumns)
        {
            List<ReflectionPropertyItem> collection = new List<ReflectionPropertyItem>();
            if (excludeColumns != null && excludeColumns.Length > 0)
            {
                //regex for update
                Regex startRegex = new Regex(@"^\s*\[?\s*", RegexOptions.Compiled);
                Regex endRegex = new Regex(@"\s*\]?\s*$", RegexOptions.Compiled);

                //update columns name
                List<String> columnNames = new List<String>();
                foreach (String column in excludeColumns)
                {
                    String newColumName = startRegex.Replace(column, String.Empty);
                    newColumName = endRegex.Replace(newColumName, String.Empty);

                    //create new name
                    columnNames.Add(newColumName);
                }

                //filtered column
                foreach (ReflectionPropertyItem property in properties.Values)
                {
                    if (!columnNames.Contains(property.Property.Name, true))
                    {
                        collection.Add(property);
                    }
                }
            }
            return collection;
        }
        /// <summary>
        /// Vytvori command pre select podla parametrov
        /// </summary>
        /// <param name="properties">Property reflector na objekt</param>
        /// <param name="whereClause">Podmiebja</param>
        /// <param name="orderClause">Zoradenie</param>
        /// <param name="limit">Limit</param>
        /// <param name="page">Stranka</param>
        /// <param name="excludeColumns">Columns which are not required in this request</param>
        /// <returns>Command text</returns>
        private String InternalCreateCommandText(ReflectionModelItem properties, String whereClause, String orderClause, Nullable<UInt32> limit, Nullable<UInt32> page, params String[] excludeColumns)
        {
            StringBuilder commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("SELECT");
            if (!page.HasValue && limit.HasValue)
            {
                commandTextBuilder.AppendFormat(" TOP {0}", limit.Value);
            }

            //create columns
            List<ReflectionPropertyItem> filteredColumns = this.InternalGetFilteredColumns(properties, excludeColumns);
 
            if (filteredColumns.Count > 0)
            {
                //get columns
                commandTextBuilder.AppendFormat(" {0} ", this.InternalGetColumnNamesForCommandText(filteredColumns));
            }
            else
            {
                //all columns
                commandTextBuilder.Append(" * ");
            }
            commandTextBuilder.AppendFormat("FROM [{0}]", properties.TableAttribute.Name);
            if (!String.IsNullOrWhiteSpace(whereClause))
            {
                commandTextBuilder.AppendFormat(" WHERE {0}", whereClause);
            }
            if (!String.IsNullOrWhiteSpace(orderClause))
            {
                commandTextBuilder.AppendFormat(" ORDER BY {0}", orderClause);
            }
            if (page.HasValue && limit.HasValue)
            {
                commandTextBuilder.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", ((page.Value - 1) * limit.Value), limit.Value);
            }
            return commandTextBuilder.ToString();
        }
        /*
        /// <summary>
        /// Vlozi pozadovany objekt do SQL
        /// </summary>
        /// <param name="item">objekt ktorych chceme vlozit</param>
        /// <returns>Kod navratovej hodnoty</returns>
        private int InternalInsertObjectWithProcedure(Object item)
        {
            //objekt musi byt zadany
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            //najdeme informacie o datovom type
            ReflectionObjectItem properties = this.m_reflectionPropertyCollection.FindPropertyCollection(item.GetType());

            //objekt musi obsahovat definiciu tabulky
            if (properties.TableAttribute == null)
            {
                throw new Exception("Missing attribute TableAttribute");
            }

            //overime nazov procedury
            if (String.IsNullOrWhiteSpace(((TableAttribute)properties.Attributes[0]).InsertProcedureName))
            {
                throw new Exception("InsertProcedureName is empty or null");
            }

            //informacie o properties
            if (properties.Count == 0)
            {
                throw new Exception("Missing attribute ColumnAttribute");
            }
  
            //vytvorime command
            List<NpgsqlParameter> parameterCollection = new List<NpgsqlParameter>();
            foreach (var value in properties.Values)
            {
                if (value.Attributes.Length == 1)
                {
                    ColumnAttribute attribude = (ColumnAttribute)value.Attributes[0];
                    if (!attribude.IsDbGenerated && attribude.IsRequiredWhenInserting)
                    {
                        NpgsqlParameter parameter = new NpgsqlParameter(value.Property.Name, attribude.Type);
                        parameter.Value = this.InternalValidateValue(value.Property.GetValue(item, null));
                        parameterCollection.Add(parameter);
                    }
                }
            }

            //return values
            NpgsqlParameter returnValueParam = new NpgsqlParameter("returnVal", SqlDbType.Int);
            returnValueParam.Direction = ParameterDirection.ReturnValue;
            parameterCollection.Add(returnValueParam);

            //spracujeme dommand do SQL
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                command.CommandText = ((TableAttribute)properties.Attributes[0]).InsertProcedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameterCollection.ToArray());
                this.ExecuteNonQuery(command);
                return returnValueParam.Value != null ? (int)returnValueParam.Value : -1;
            }
        }
        */
        /// <summary>
        /// Vlozi pozadovany objekt do SQL
        /// </summary>
        /// <param name="item">objekt ktorych chceme vlozit</param>
        private int InternalInsertObject(Object item)
        {
            //objekt musi byt zadany
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            //najdeme informacie o datovom type
            ReflectionModelItem properties = this.mReflectionPropertyCollection.FindPropertyCollection(item.GetType());

            //vytvorime command
            List<NpgsqlParameter> parameterCollection = new List<NpgsqlParameter>();
            StringBuilder builder = new StringBuilder();
            StringBuilder values = new StringBuilder();
            builder.AppendFormat("INSERT INTO [{0}] (", properties.TableAttribute.Name);
            foreach (var value in properties.Values)
            {
                if (value.ColumnAttribute != null)
                {
                    ColumnAttribute attribude = value.ColumnAttribute;
                    if (attribude.CanBeInserted)
                    {
                        builder.AppendFormat("[{0}], ", value.Property.Name);
                        values.AppendFormat("@{0}, ", value.Property.Name);
                        NpgsqlParameter parameter = new NpgsqlParameter(value.Property.Name, attribude.Type);
                        parameter.Value = this.InternalPrepareValue(value.Property.GetValue(item, null), attribude);
                        parameterCollection.Add(parameter);
                    }
                }
            }

            //chyba, nie su ziadne SET parametre
            if (parameterCollection == null || parameterCollection.Count == 0)
            {
                throw new Exception("The model does not contain enough column for an insert.");
            }

            builder.Remove(builder.Length - 2, 2);
            values.Remove(values.Length - 2, 2);
            builder.AppendFormat(") VALUES ({0})", values.ToString());

            //spracujeme dommand do SQL
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                command.CommandText = builder.ToString();
                command.Parameters.AddRange(parameterCollection.ToArray());
                return this.ExecuteNonQuery(command);
            }
        }
        /// <summary>
        /// Insertne kolekciu dat do DB
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme insertnut</typeparam>
        /// <param name="collection">Kolekcia dat</param>
        /// <returns>Pocet riadkov ovplyvnenych insertom</returns>
        private int InternalInsertCollection<T>(List<T> collection)
        {
            //objekt musi byt zadany
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (collection.Count == 0)
                throw new ArgumentException("Collection is empty!");

            //najdeme informacie o datovom type
            ReflectionModelItem properties = this.mReflectionPropertyCollection.FindPropertyCollection(typeof(T));

            //vytvorime command
            List<NpgsqlParameter> parameterCollection = new List<NpgsqlParameter>();
            StringBuilder builder = new StringBuilder();
            var count = collection.Count;
            for (int i = 0; i < count; i++)
			{
                //ziskame polozku
			    T item = collection[i];
     
                //vytvorime command pre aktualny objekt
                StringBuilder values = new StringBuilder();
                builder.AppendFormat("INSERT INTO [{0}] (", properties.TableAttribute.Name);
                foreach (var value in properties.Values)
                {
                    if (value.ColumnAttribute != null)
                    {
                        ColumnAttribute attribude = value.ColumnAttribute;
                        if (attribude.CanBeUpdated)
                        {
                            //ziskame propertyName
                            var propertyName = value.Property.Name;

                            //vytvorime zaznam
                            builder.AppendFormat("[{0}], ", propertyName);

                            //upravime meno
                            propertyName = String.Format("{0}{1}", value.Property.Name, i);

                            //vytvorime parameter
                            values.AppendFormat("@{0}, ", propertyName);
                            NpgsqlParameter parameter = new NpgsqlParameter(propertyName, attribude.Type);
                            parameter.Value = this.InternalPrepareValue(value.Property.GetValue(item, null), attribude);
                            parameterCollection.Add(parameter);
                        }
                    }
                }

                //chyba, nie su ziadne SET parametre
                if (parameterCollection == null || parameterCollection.Count == 0)
                {
                    throw new Exception("The model does not contain enough column for an insert.");
                }

                builder.Remove(builder.Length - 2, 2);
                values.Remove(values.Length - 2, 2);
                builder.AppendFormat(") VALUES ({0});", values.ToString());
            }

            //spracujeme dommand do SQL
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                command.CommandText = builder.ToString();
                command.Parameters.AddRange(parameterCollection.ToArray());
                return this.ExecuteNonQuery(command);
            }
        }
        /// <summary>
        /// Aktualizuje pozadovany objekt v SQL
        /// </summary>
        /// <param name="item">objekt ktorych chceme aktualizovat</param>
        /// <param name="whereClause">Podmienka na aktualizaciu objektu</param>
        private int InternalUpdateObject(Object item, String whereClause = null)
        {
            //objekt musi byt zadany
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            //najdeme informacie o datovom type
            ReflectionModelItem properties = this.mReflectionPropertyCollection.FindPropertyCollection(item.GetType());

            // check primary key
            ReflectionPropertyItem primaryKey = properties.FindPrimaryKeyProperty();
            if (primaryKey == null)
            {
                throw new Exception("Update requires column with primary key.");
            }
 
            //vytvorime command
            List<NpgsqlParameter> parameterCollection = new List<NpgsqlParameter>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("UPDATE [{0}] SET ", properties.TableAttribute.Name);
            foreach (var property in properties.Values)
            {
                ColumnAttribute attribude = property.ColumnAttribute;
                if (attribude.CanBeUpdated)
                {
                    builder.AppendFormat("[{0}] = @{0}, ", property.Property.Name);
                    NpgsqlParameter parameter = new NpgsqlParameter(property.Property.Name, attribude.Type);
                    parameter.Value = this.InternalPrepareValue(property.Property.GetValue(item, null), attribude);
                    parameterCollection.Add(parameter);
                }
            }

            //chyba, nie su ziadne SET parametre
            if (parameterCollection == null || parameterCollection.Count == 0)
            {
                throw new Exception("The model does not contain enough column for an update.");
            }
            builder.Remove(builder.Length - 2, 2);
            builder.AppendFormat(" WHERE [{0}] = @{0}", primaryKey.Property.Name);

            //ak je zadana podmienka
            if (!String.IsNullOrWhiteSpace(whereClause))
            {
                builder.AppendFormat(" AND {0}", whereClause);
            }
            NpgsqlParameter primaryKeyParameter = new NpgsqlParameter(primaryKey.Property.Name, primaryKey.ColumnAttribute.Type);
            primaryKeyParameter.Value = primaryKey.Property.GetValue(item, null);
            parameterCollection.Add(primaryKeyParameter);

            //vykoname command
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                command.CommandText = builder.ToString();
                command.Parameters.AddRange(parameterCollection.ToArray());
                return this.ExecuteNonQuery(command);
            }
        }
        /// <summary>
        /// Overi hodnotu a vrati jej spravny format
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="attribute">Atributy stlpca v tabulke</param>
        /// <returns>Hodnota</returns>
        private Object InternalPrepareValue(Object value, ColumnAttribute attribute)
        {
            if (value != null)
            {
                switch (attribute.Type)
                { 
                    /*
                    case SqlDbType.Date:
                        if (value is DateTime)
                        {
                            return ((DateTime)value).ToLocalTime();
                        }
                        return value;
                    case SqlDbType.VarChar:
                    case SqlDbType.NVarChar:
                    case SqlDbType.Text:
                        return this.InternalTruncateValue(value, attribute);
                    */
                    default:
                        return value;
                }
            }
            return DBNull.Value;
        }
        /// <summary>
        /// Vykona skratenie hodnoty ak je dostupna a je to povolene
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme skratit</param>
        /// <param name="attribute">Atributy stlpca v tabulke</param>
        /// <returns>Skratena hodnota alebo povodna</returns>
        private Object InternalTruncateValue(Object value, ColumnAttribute attribute)
        {
            if (value != null && this.TruncateValue && attribute.MaxSize != int.MaxValue)
            {
                if (value is String)
                {
                    value = ((String)value).TruncateLongString(attribute.MaxSize);
                }
            }
            return value;
        }
        /// <summary>
        /// Vyhlada property ktore obsahuju pozadovany typ atributu
        /// </summary>
        /// <param name="propertyInfo">Kolekcia v ktorej chceme vyhladat</param>
        /// <returns>Pole najdenych property ktore obsahuju pozadovany atribut</returns>
        private PropertyInfo[] InternalFindPropertiesWithColumnAttribute(PropertyInfo[] propertyInfo)
        {
            List<PropertyInfo> collection = new List<PropertyInfo>();
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                PropertyInfo info = propertyInfo[i];
                if (info.GetCustomAttributes(typeof(ColumnAttribute), true).Length == 1)
                {
                    collection.Add(info);
                }
            }
            return collection.ToArray();
        }
        #endregion
    }
}
