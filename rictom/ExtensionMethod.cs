using Newtonsoft.Json.Linq;
using project858.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;


namespace project858
{
    /// <summary>
    /// Metody na rozsirenie vlastnosti objektov
    /// </summary>
    public static class ExtensionMethod
    {
        #region - Public Static DateTime Methods -
        /// <summary>
        /// This method selects min value from current and MinValue 
        /// </summary>
        /// <param name="value">Current value</param>
        /// <param name="minValue">Min value</param>
        /// <returns>Min value</returns>
        public static DateTime Min(this DateTime value, DateTime minValue)
        {
            return value < minValue ? value : minValue;
        }
        /// <summary>
        /// This method selects min value from current and new date which is combine current value and days 
        /// </summary>
        /// <param name="value">Current value</param>
        /// <param name="days">
        /// A number of whole and fractional days. The value parameter can be negative or positive.
        /// </param>
        /// <returns>Min value</returns>
        public static DateTime MinAddDays(this DateTime value, int days)
        {
            DateTime minValue = DateTime.Now.AddDays(days);
            return value.Min(minValue);
        }
        /// <summary>
        /// This method selects max value from current and MinValue 
        /// </summary>
        /// <param name="value">Current value</param>
        /// <param name="maxValue">Max value</param>
        /// <returns>Min value</returns>
        public static DateTime Max(this DateTime value, DateTime maxValue)
        {
            return value < maxValue ? maxValue : value;
        }
        /// <summary>
        /// This method selects max value from current and new date which is combine current value and days 
        /// </summary>
        /// <param name="value">Current value</param>
        /// <param name="days">
        /// A number of whole and fractional days. The value parameter can be negative or positive.
        /// </param>
        /// <returns>Min value</returns>
        public static DateTime MaxAddDays(this DateTime value, int days)
        {
            DateTime maxValue = DateTime.Now.AddDays(days);
            return value.Max(maxValue);
        }
        #endregion

        #region - Public Static Type Methods -
        /// <summary>
        /// Overi ci je typ typu list
        /// </summary>
        /// <param name="type">Typ ktory chceme overit</param>
        /// <returns>Boolean typ je List inak false</returns>
        public static Boolean IsList(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }
        /// <summary>
        /// Overi ci je typ typu list
        /// </summary>
        /// <param name="type">Typ ktory chceme overit</param>
        /// <param name="itemType">Typ itemu</param>
        /// <returns>Boolean typ je List inak false</returns>
        public static Boolean IsList(this Type type, Type itemType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type argumentType = type.GetGenericArguments().Single();
                return itemType.IsAssignableFrom(argumentType);
            }
            return false;
        }
        #endregion

        #region - Public Static Byte Methods -
        /// <summary>
        /// This function returns bit value from position
        /// </summary>
        /// <param name="value">byte</param>
        /// <param name="position">Position</param>
        /// <returns>True = bit = 1, False = bit = 0</returns>
        public static Boolean GetBitValue(this Byte value, int position)
        {
            return Utility.GetBitValue(value, position);
        }
        #endregion

        #region - Public Static Guid Methods -
        /// <summary>
        /// Skonvertuje guid to stringu bez pomlciek
        /// </summary>
        /// <param name="value">Guid ktory chceme konvertovat</param>
        /// <returns>Konvertovany guid</returns>
        public static String ToStringWithoutDash(this Guid value)
        {
            return value.ToString("N");
        }
        /// <summary>
        /// Overi ci je guid empty
        /// </summary>
        /// <param name="value">Guid ktory chceme overit</param>
        /// <returns>True = guid je empty, inak false</returns>
        public static Boolean IsEmpty(this Guid value)
        {
            return Utility.IsEmpty(value);
        }
        /// <summary>
        /// Vrati hodnotu urcenu pre databazu. Ak je hodnota null vrati DBNull.Value
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <returns>Hodnota pre databazu</returns>
        public static Object ToDbValue(this Nullable<Guid> value)
        {
            return value == null ? DBNull.Value : (Object)value;
        }
        /// <summary>
        /// Vrati DB null ak je hodnota Guid.Empty, alebo vrati aktualnu hodnotu
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <returns>DBNull alebo aktualna hodnota</returns>
        public static Object GetDbValue(this Guid value)
        {
            return value.CompareTo(Guid.Empty) == 0 ? DBNull.Value : (Object)value;
        }

        #endregion

        #region - Public Static String Methods -
        /// <summary>
        /// Determines whether an element is in the collection
        /// </summary>
        /// <param name="values">
        /// Collection to check
        /// </param>
        /// <param name="value">
        /// The object to locate in the collection. The value can be null for reference types.
        /// </param>
        /// <param name="ignoreCase">
        /// true to ignore case during the comparison; otherwise, false.
        /// </param>
        /// <returns>true | fakse</returns>
        public static Boolean Contains(this List<String> values, String value, Boolean ignoreCase = false)
        {
            if (values != null && value.Length > 0)
            {
                foreach (String item in values)
                {
                    if (String.Compare(item, value, ignoreCase) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Determines whether an element is in the array
        /// </summary>
        /// <param name="values">
        /// Array to check
        /// </param>
        /// <param name="value">
        /// The object to locate in the array. The value  can be null for reference types.
        /// </param>
        /// <param name="ignoreCase">
        /// true to ignore case during the comparison; otherwise, false.
        /// </param>
        /// <returns>true | fakse</returns>
        public static Boolean Contains(this String[] values, String value, Boolean ignoreCase = false)
        {
            if (values != null && value.Length > 0)
            {
                foreach (String item in values)
                {
                    if (String.Compare(item, value, ignoreCase) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// This method converts list of bytes to hexa string
        /// </summary>
        /// <param name="value">List with data</param>
        /// <returns>Hexa string | null</returns>
        public static String ToHexaString(this Byte[] value)
        {
            if (value != null && value.Length > 0)
            {
                return Utility.ConvertByteArrayToHexString(value);
            }
            return null;
        }
        /// <summary>
        /// This method converts list of bytes to hexa string
        /// </summary>
        /// <param name="value">List with data</param>
        /// <returns>Hexa string | null</returns>
        public static String ToHexaString(this List<Byte> value)
        {
            if (value != null)
            {
                return Utility.ConvertByteArrayToHexString(value.ToArray());
            }
            return null;
        }
        /// <summary>
        /// Skonvertuje string na DateTimeOffset
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme parsovat</param>
        /// <returns>DateTimeOffset</returns>
        public static DateTimeOffset ToDateTimeOffset(this string value) 
        {
            return DateTimeOffset.ParseExact(value, "yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Skonvertuje string na DateTime
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme parsovat</param>
        /// <returns>DateTimeOffset</returns>
        public static DateTime ToDateTime(this string value)
        {
            return DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Skonvertuje string do guid, alebo vrati Empty
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme konvertovat</param>
        /// <returns>Guid alebo Guid.Empty</returns>
        public static Nullable<Guid> ToGuid(this string value)
        {
            return Utility.ParseGuid(value);
        }
        /// <summary>
        /// Skonvertuje string do guid, alebo vrati Empty
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme konvertovat</param>
        /// <returns>Guid alebo Guid.Empty</returns>
        public static Nullable<Guid> ToGuidWithoutDash(this string value)
        {
            return Utility.ParseGuidWithoutDash(value);
        }
        #endregion

        #region - Public Static Methods -
        /// <summary>
        /// Vrati hodnotu property
        /// </summary>
        /// <typeparam name="T">Typ obejktu ktory ziadame</typeparam>
        /// <param name="data">Data z ktorych chceme hodnotu citat</param>
        /// <param name="name">Meno property ktorej hodnotu chceme nacitat</param>
        /// <returns>Hodnota typu T alebo jej default</returns>
        public static T GetPropertyValue<T>(this JObject data, String name)
        {
            if (data != null)
            {
                JToken token = data.GetValue(name);
                return token.Value<T>();
            }
            return default(T);
        }
        /// <summary>
        /// Vrati hodnotu property
        /// </summary>
        /// <typeparam name="T">Typ obejktu ktory ziadame</typeparam>
        /// <param name="data">Data z ktorych chceme hodnotu citat</param>
        /// <param name="name">Meno property ktorej hodnotu chceme nacitat</param>
        /// <returns>Hodnota typu T alebo jej default</returns>
        public static T GetPropertyValue<T>(this ExpandoObject data, String name)
        {
            if (data != null)
            {
                PropertyInfo property = data.GetType().GetProperty(name);
                if (property != null)
                {
                    Object value = property.GetValue(data, null);
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }
            return default(T);
        }
        /// <summary>
        /// Odstrani diakritiku z textu
        /// </summary>
        /// <param name="value">Text z ktoreho chceme odstranit diakritiku</param>
        /// <returns>Text s odstranenou diakritikou</returns>
        public static String RemoveDiacritics(this String value)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                var normalizedString = value.Normalize(NormalizationForm.FormD);
                var stringBuilder = new StringBuilder();
                foreach (var c in normalizedString)
                {
                    var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                    if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    {
                        stringBuilder.Append(c);
                    }
                }
                value = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            }
            return value;
        }
        /// <summary>
        /// Vrati json string z objektu
        /// </summary>
        /// <param name="value">Objekt ktory chceme konverotvat</param>
        /// <returns>Text alebo null</returns>
        public static String ToJsonString(this Object value)
        {
            return Utility.ConvertObjectToJson(value);
        }
        /// <summary>
        /// Vytvori string s informaciami o vsetkych polozkach
        /// </summary>
        /// <param name="collection">Kolekcia dat ktore chceme vypisat</param>
        /// <returns>Vypisane hodnoty</returns>
        public static String ToStringAllValues(this NameValueCollection collection)
        {
            StringBuilder builder = new StringBuilder();
            int count = collection.Count;
            foreach (String key in collection)
            {
                builder.AppendFormat("{0}={1};", key, collection[key]);
            }
            return builder.ToString();
        }
        /// <summary>
        /// Skonvertuje datum a cas na iso 8601 format
        /// </summary>
        /// <param name="value">DateTimeOffset</param>
        /// <returns>ISO 8601 format</returns>
        public static String ToIso8601String(this DateTime value)
        {
            return value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
        /// <summary>
        /// Skonvertuje datum a cas na iso 8601 format
        /// </summary>
        /// <param name="value">DateTimeOffset</param>
        /// <returns>ISO 8601 format</returns>
        public static String ToIso8601String(this DateTimeOffset value)
        {
            return value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
        /// <summary>
        /// Overi ci je hodnota typu alpha
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <returns>True hodnota je alpha, inak false</returns>
        public static Boolean IsAlpha(this string value)
        {
            return Utility.IsAlpha(value, int.MaxValue);
        }
        /// <summary>
        /// Overi ci je hodnota typu alpha
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="length">Dlzka stringu ktory chceme overit</param>
        /// <returns>True hodnota je alpha, inak false</returns>
        public static Boolean IsAlpha(this string value, int length)
        {
            return Utility.IsAlpha(value, length);
        }
        /// <summary>
        /// Overi ci je objekt typu DB Null
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <returns>True = objekt je DB null, inak false</returns>
        public static Boolean IsDBNull(this object value)
        {
            return value is DBNull;
        }
        /// <summary>
        /// Overi ci je hodnota typu numeric
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <returns>True hodnota je numeric, inak false</returns>
        public static Boolean IsNumeric(this string value)
        {
            return Utility.IsNumeric(value, 1);
        }
        /// <summary>
        /// Overi ci je hodnota typu numeric
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="length">Dlzka stringu ktory chceme overit</param>
        /// <returns>True hodnota je numeric, inak false</returns>
        public static Boolean IsNumeric(this string value, int length)
        {
            return Utility.IsNumeric(value, length);
        }
        /// <summary>
        /// Overi ci je hodnota typu alphanumeric
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <returns>True hodnota je alphanumeric, inak false</returns>
        public static Boolean IsAlphaNumeric(this string value)
        {
            return Utility.IsAlphaNumeric(value, int.MaxValue);
        }
        /// <summary>
        /// Overi ci je hodnota typu alphanumeric
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="length">Dlzka stringu ktory chceme overit</param>
        /// <returns>True hodnota je alphanumeric, inak false</returns>
        public static Boolean IsAlphaNumeric(this string value, int length)
        {
            return Utility.IsAlphaNumeric(value, length);
        }
        /// <summary>
        /// Vrati zaciatocnu cast stringu o pozadovanej dlzke
        /// </summary>
        /// <param name="value">String</param>
        /// <param name="maxLength">Maximalna dlzka stringu ktoru ziadame</param>
        /// <returns>Orezany string, alebo povodny string</returns>
        public static string TruncateLongString(this string value, int maxLength)
        {
            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
        /// <summary>
        /// Zacriptuje string do SHA1
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme zacryptovat</param>
        /// <returns>Zacryptovany string</returns>
        public static String ToSHA1(this string value)
        {
            return Utility.EncryptoSHA1(value);
        }
        /// <summary>
        /// Overi ci zadany text obsahuje pozadovane slova alebo ich modifikaciu s medziznakmi
        /// </summary>
        /// <param name="value">Text ktory chceme overit</param>
        /// <param name="words">Slova na overenie</param>
        /// <returns>True = slovo je zahrnute v texte inak false</returns>
        public static Boolean ValidateContainsWords(this string value, params string[] words)
        {
            if (words.Length > 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (String word in words)
                {
                    builder.Clear();
                    builder.Append("(?=(");
                    foreach (Char wordChar in word)
                    {
                        builder.Append(String.Format("{0}*", wordChar));
                    }
                    builder.Append("))");

                    Regex regex = new Regex(builder.ToString(), RegexOptions.IgnoreCase);
                    MatchCollection matches = regex.Matches(value);
                    foreach (Match match in matches)
                    {
                        foreach (Group group in match.Groups)
                        {
                            if (String.Compare(group.Value, word, true) == 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// Vrati hodnotu urcenu pre databazu. Ak je hodnota null vrati DBNull.Value
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <returns>Hodnota pre databazu</returns>
        public static Object ToDbValue(this string value)
        {
            return String.IsNullOrWhiteSpace(value) ? DBNull.Value : (Object)value;
        }
        /// <summary>
        /// Vrati hodnotu urcenu pre databazu. Ak je hodnota null vrati DBNull.Value
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <returns>Hodnota pre databazu</returns>
        public static Object ToDbValue(this Nullable<int> value)
        {
            return value == null ? DBNull.Value : (Object)value;
        }
        /// <summary>
        /// Overi string na jeho minimalnu dlzku
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="minLength">Minimalna dlzka ktoru ma mat string, pricom nemoze byt zlozeny len z white space znakov</param>
        /// <returns>True = string ma minimalnu dlzku, inak false</returns>
        public static Boolean Validate(this string value, int minLength)
        {
            return value.Validate(minLength, int.MaxValue);
        }
        /// <summary>
        /// Overi string na jeho minimalnu dlzku
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="minLength">Minimalna dlzka ktoru ma mat string, pricom nemoze byt zlozeny len z white space znakov</param>
        /// <param name="maxLength">Maximalna dlzka ktory moze mt string</param>
        /// <returns>True = string ma minimalnu dlzku, inak false</returns>
        public static Boolean Validate(this string value, int minLength, int maxLength)
        {
            return !String.IsNullOrWhiteSpace(value) && value.Length >= minLength && value.Length <= maxLength;
        }
        /// <summary>
        /// Odstrani opakovane znaky new line
        /// </summary>
        /// <param name="value">String z ktoreho chceme ordstranit znaky</param>
        /// <returns>Upraveny string</returns>
        public static String RemoveReplyNewLine(this string value)
        {
            return String.IsNullOrWhiteSpace(value) ? value : Regex.Replace(value, "(\r\n){2,}", "\r\n");
        }
        /// <summary>
        /// Vrati DB null ak je hodnota Empty, alebo vrati aktualnu hodnotu
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <returns>DBNull alebo aktualna hodnota</returns>
        public static Object GetDbValue(this string value)
        {
            return String.IsNullOrWhiteSpace(value) ? DBNull.Value : (Object)value;
        }
        /// <summary>
        /// Zacryptuje aktualnu hodnotu stringu do SHA1
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme zacrpytovat</param>
        /// <returns>Zacryptovana hodnota alebo Empty</returns>
        public static String GetSHA1Value(this string value)
        {
            return Utility.EncryptoSHA1(value);
        }
        #endregion
    }
}
