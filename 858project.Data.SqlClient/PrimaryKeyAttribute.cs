using NpgsqlTypes;
using System;

namespace project858.Data.SqlClient
{
    /// <summary>
    /// Nastavenie strlca tabulky
    /// </summary>
    public sealed class PrimaryKeyAttribute : Attribute
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public PrimaryKeyAttribute()
        {

        }
        #endregion
    }
}
