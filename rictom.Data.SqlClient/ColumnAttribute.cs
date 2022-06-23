using NpgsqlTypes;
using System;

namespace project858.Data.SqlClient
{
    /// <summary>
    /// Nastavenie strlca tabulky
    /// </summary>
    public sealed class ColumnAttribute : Attribute
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public ColumnAttribute()
        {
            this.CanBeInserted = false;
            this.CanBeUpdated = false;
            this.CanBeNull = false;
            this.MaxSize = int.MaxValue;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Definuje dlzku contextu
        /// </summary>
        public Int32 MaxSize { get; set; }
        /// <summary>
        /// Definuje ci hodnotu potrebne pripojit do Insert
        /// </summary>
        public Boolean CanBeInserted { get; set; }
        /// <summary>
        /// Definuje ci hodnotu potrebne pripojit do Update
        /// </summary>
        public Boolean CanBeUpdated { get; set; }
        /// <summary>
        /// Definuje ci moze byt hodnota null
        /// </summary>
        public Boolean CanBeNull { get; set; }
        /// <summary>
        /// Typ objektu v databaze
        /// </summary>
        public NpgsqlDbType Type { get; set; }
        #endregion
    }
}
