using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace project858.Reflection
{
    /// <summary>
    /// Reflekcia property objektu
    /// </summary>
    public class ReflectionProperty
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="property">Property</param>
        public ReflectionProperty(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            this.Property = property;
            this.Name = property.Name;
            this.m_customAttributes = new Dictionary<Type, Object[]>();
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Meno property
        /// </summary>
        public String Name { get; private set; }
        /// <summary>
        /// Property ktoru reprezentuje typ
        /// </summary>
        public PropertyInfo Property { get; private set; }
        #endregion

        #region - Variables -
        /// <summary>
        /// Cache for custom attribute
        /// </summary>
        private Dictionary<Type, Object[]> m_customAttributes = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Vrati pozadovany vlastny atribut propety
        /// </summary>
        /// <param name="type">Typ atributu ktory ziadame</param>
        /// <returns>Atribut alebo null</returns>
        public Object[] GetCustomAttributes(Type type)
        {
            if (this.m_customAttributes.ContainsKey(type))
            {
                return this.m_customAttributes[type];
            }
            Object[] result = this.Property.GetCustomAttributes(type, true);
            this.m_customAttributes.Add(type, result);
            return result;
        }
        /// <summary>
        /// This function return first custom attribute
        /// </summary>
        /// <typeparam name="T">Type of attribute</typeparam>
        /// <returns>Attribute or null</returns>
        public T[] GetCustomAttributes<T>()
        {
            if (this.m_customAttributes.ContainsKey(typeof(T)))
            {
                Object[] result = this.m_customAttributes[typeof(T)];
                return result == null ? default : result.OfType<T>().ToArray();
            }
            else
            {
                //get all attribute
                Object[] result = this.Property.GetCustomAttributes(typeof(T), true);
                this.m_customAttributes.Add(typeof(T), result);
                return result == null ? default : result.OfType<T>().ToArray();
            }
        }
        /// <summary>
        /// This function return first custom attribute
        /// </summary>
        /// <typeparam name="T">Type of attribute</typeparam>
        /// <returns>Attribute or null</returns>
        public T GetCustomAttribute<T>()
        {
            T[] result = this.GetCustomAttributes<T>();
            return result != null && result.Length > 0x00 ? result[0] : default;
        }
        #endregion
    }
}
