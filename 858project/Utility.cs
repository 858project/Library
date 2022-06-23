using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace project858
{
    /// <summary>
    /// Utility for library
    /// </summary>
    public static class Utility
    {
        #region - Constants -
        /// <summary>
        /// Regex na overenie stringu ci obsahuje len alpha znaky.
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka") alebo vyuzit Utlity.GetRegexPattern(regexConstant, length)
        /// </summary>
        public const String REGEX_ALPHA = @"^[a-zA-Z]{LENGTH,}$";
        /// <summary>
        /// Regex na overenie dlzky stringu
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka") alebo vyuzit Utlity.GetRegexPattern(regexConstant, length)
        /// </summary>
        public const String REGEX_LENGTH = @"^.{LENGTH,}$";
        /// <summary>
        /// Regex na overenie stringu ci obsahuje len ciselne znaky.
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka") alebo vyuzit Utlity.GetRegexPattern(regexConstant, length)
        /// </summary>
        public const String REGEX_NUMERIC = @"^[0-9]{LENGTH,}$";
        /// <summary>
        /// Regex na overenie stringu ci obsahuje len alpha numericke znaky
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka") alebo vyuzit Utlity.GetRegexPattern(regexConstant, length)
        /// </summary>
        public const String REGEX_ALPHANUMERIC = @"^[0-9a-zA-Z]{LENGTH,}$";
        /// <summary>
        /// Regex na overenie stringu ci obsahuje len alpha numericke znaky, pricom alphanumericke su len male
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka") alebo vyuzit Utlity.GetRegexPattern(regexConstant, length)
        /// </summary>
        public const String REGEX_ALPHANUMERIC_SMALL = @"^[0-9a-z]{LENGTH,}$";
        /// <summary>
        /// Kryptovacie heslo na ulozenie konfiguracie
        /// </summary>
        public const String CONFIGURATION_PASSWORD = "39dare54m48ndgdf543rf684t540fjetr4n444f9fds9";
        /// <summary>
        /// Kryptovacie heslo na ulozenie konfiguracie sluzby
        /// </summary>
        public const String WINDOWS_SERVICE_CONFIGURATION_PASSWORD = "390fmn40fm48nd943fh48450fje8434nf944f94f9";
        #endregion

        #region - Public Static Method -
        /// <summary>
        /// This function returns true if object is generic list
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns>True | false</returns>
        public static Boolean IsGenericListType(this Type type)
        {
           return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)));
        }
        /// <summary>
        /// This function returns true if object is generic list
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <returns>True | false</returns>
        public static Boolean IsGenericList(this Object item)
        {
            var type = item.GetType();
            if (type != null)
            {
                return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)));
            }
            return false;
        }
        /// <summary>
        /// Vrati string dat ktore sa prijali alebo odoslali
        /// </summary>
        /// <param name="data">Data ktore chceme spracovat</param>
        /// <returns>String dat</returns>
        public static String ConvertByteArrayToHexString(Byte[] data)
        {
            //pomocny string na ulozenie obrazu dat
            StringBuilder builder = new StringBuilder();

            if (data != null)
            {
                //prejdeme vsetky byty a urobime z nich string
                foreach (Byte b in data)
                {
                    builder.AppendFormat("0x{0:X2} ", b);
                }

                //odstranime posledny prazdny znak
                builder.Remove(builder.Length - 1, 1);
            }

            //vratime data
            return builder.ToString();
        }
        /// <summary>
        /// This function returns bit value from position
        /// </summary>
        /// <param name="data">byte</param>
        /// <param name="position">Position</param>
        /// <returns>True = bit = 1, False = bit = 0</returns>
        public static Boolean GetBitValue(Byte data, int position)
        {
            if ((((data) >> (position)) & 1) == 0)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Prekonvertuje objekt do json stringu
        /// </summary>
        /// <param name="value">Objekt ktory chceme konverotvat</param>
        /// <returns>Json text alebo null</returns>
        public static String ConvertObjectToJson(Object value)
        {
            if (value != null)
            {
                return JsonConvert.SerializeObject(value, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            return null;
        }
        /// <summary>
        /// Reverzne string
        /// </summary>
        /// <param name="value">String ktory chceme reverznut</param>
        /// <returns>Reverznuty string alebo null</returns>
        public static String ReverseString(String value)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                char[] array = value.ToCharArray();
                Array.Reverse(array);
                return new String(array);
            }
            return null;
        }
        /// <summary>
        /// Overi ci je guid empty
        /// </summary>
        /// <param name="value">Guid ktory chceme overit</param>
        /// <returns>True = guid je empty, inak false</returns>
        public static Boolean IsEmpty(Guid value)
        {
            return Guid.Empty.CompareTo(value) == 0;
        }
        /// <summary>
        /// Overi ci je hodnota typu alpha
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="length">Dlzka stringu ktory chceme overit</param>
        /// <returns>True hodnota je alpha, inak false</returns>
        public static Boolean IsAlpha(string value, int length)
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[a-zA-Z]{" + length + ",}$");
        }
        /// <summary>
        /// Overi ci je hodnota typu anumeric
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="length">Dlzka stringu ktory chceme overit</param>
        /// <returns>True hodnota je numeric, inak false</returns>
        public static Boolean IsNumeric(string value, int length)
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[0-9]{" + length + ",}$");
        }
        /// <summary>
        /// Overi ci je hodnota typu alphanumeric
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="length">Dlzka stringu ktory chceme overit</param>
        /// <returns>True hodnota je alphanumeric, inak false</returns>
        public static Boolean IsAlphaNumeric(string value, int length)
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[0-9a-zA-Z]{"+ length + ",}$");
        }
        /// <summary>
        /// Zacriptuje string do SHA1
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme zacryptovat</param>
        /// <returns>Zacryptovany string</returns>
        public static String EncryptoSHA1(String value)
        {
            return Utility.EncryptoSHA1(value, Encoding.UTF8);
        }
        /// <summary>
        /// Zacriptuje string do SHA1
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme zacryptovat</param>
        /// <param name="encoding">Encoding v akom chceme kodovat</param>
        /// <returns>Zacryptovany string</returns>
        public static String EncryptoSHA1(String value, Encoding encoding)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                using (SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider())
                {
                    if (encoding == null)
                    {
                        encoding = Encoding.UTF8;
                    }
                    var result = BitConverter.ToString(provider.ComputeHash(encoding.GetBytes(value))).Replace("-", "");
                    return result.ToLower();
                }
            }
            return String.Empty;
        }
        /// <summary>
        /// Vyparsuje guid z textu ktory neobsahuje pomlcky
        /// </summary>
        /// <param name="guid">String ktory obsahuje guid bez pomlciek</param>
        /// <returns>Guid alebo null</returns>
        public static Nullable<Guid> ParseGuidWithoutDash(String guid)
        {
            //overie string
            if (String.IsNullOrWhiteSpace(guid) || guid.Length != 32)
            {
                return null;
            }

            //parse guid value
            Guid value = Guid.Empty;
            if (Guid.TryParseExact(guid, "N", out value))
            {
                return value;
            }

            //no guid
            return null;
        }
        /// <summary>
        /// Vyparsuje jedinecny identifikator
        /// </summary>
        /// <param name="guid">String-ova hodnota v ktorej sa ma Guid nachadzat</param>
        /// <returns>Vyparsovany Guid alebo Guid.Empty</returns>
        public static Nullable<Guid> ParseGuid(String guid)
        {
            if (!String.IsNullOrWhiteSpace(guid))
            {
                Guid key = Guid.Empty;
                if (Utility.ValidateGuid(guid))
                {
                    if (!Guid.TryParse(guid, out key))
                    {
                        return null;
                    }
                }
                return key;
            }
            return null;
        }
        /// <summary>
        /// Overi spravnost stringu ako guid
        /// </summary>
        /// <param name="guid">Guid hodnota v stringu</param>
        /// <returns>True = hodnota je guid, inak false</returns>
        public static Boolean ValidateGuid(String guid)
        {
            if (String.IsNullOrWhiteSpace(guid))
                return false;

            return Regex.IsMatch(guid, @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$");
        }
        /// <summary>
        /// This function check whether type is from nullable type
        /// </summary>
        /// <param name="type">Value to check</param>
        /// <returns></returns>
        public static Boolean IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        /// <summary>
        /// Overi ci je objekt ciselneho typu
        /// </summary>
        /// <param name="value">Objekt ktory chceme overit</param>
        /// <returns>True = objekt je ciselneho typu, inak false</returns>
        public static Boolean IsNumbericType(Object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return value is int || value is uint || value is float || value is String || value is double;
        }
        /// <summary>
        /// Odstrani zo stringu biele znaky
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Ak string neobsahuje ine znaky ako biele, pripadne je null
        /// </exception>
        /// <param name="str">String ktory chceme upravit</param>
        /// <returns>Upraveny string</returns>
        public static String StringTrim(String str)
        { 
            if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException("str");
            }

            //inicializujeme
            Regex regex = new Regex(@"\s");

            //odstranime znaky
            return regex.Replace(str, String.Empty);
        }
        /// <summary>
        /// Overi spravnost telefonneho cisla. Minimalna dlzka cislic je 8
        /// </summary>
        /// <param name="phone">Cislo ktore chceme overit</param>
        /// <returns>True = cislo je spravne</returns>
        public static Boolean ValidatePhoneNumber(String phone)
        {
            // Check for exactly 10 numbers left over
            return Regex.IsMatch(phone, @"^\+?\d{3,15}$"); 
        }
        /// <summary>
        /// Vrati Regex string s upravou na pozadovanu dlzku pri kontrole
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Argument mimo rozsah. Min value is 1
        /// </exception>
        /// <param name="regexConstant">Konstanta z Utility.REGEX ktoru chceme modifikovat</param>
        /// <param name="length">Pozadovana dlzka pri kontrole</param>
        /// <returns>Regex string</returns>
        public static String GetRegexPattern(String regexConstant, int length)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            return regexConstant.Replace("LENGTH", length.ToString());
        }
        /// <summary>
        /// Overi meno serioveho portu
        /// </summary>
        /// <param name="time">Cas ktory chceme overit</param>
        /// <returns>True = cas je ok, inak false</returns>
        public static Boolean ValidateTime(String time)
        {
            //osetrenies
            if (String.IsNullOrEmpty(time))
                return false;

            //regularny vyraz na overnie mena portu
            String pattern = @"^([01]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$";

            //overime meno
            return Regex.IsMatch(time, pattern);
        }
        /// <summary>
        /// method for validating a url with regular expressions
        /// </summary>
        /// <param name="url">url we're validating</param>
        /// <returns>true if valid, otherwise false</returns>
        public static Boolean ValidateUrl(String url)
        {
            //pattern na validaciu
            String pattern = @"^(((http|https|ftp)\://)|(www\.))[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}$";
            
            //nastavenie regex
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            //validacia
            return reg.IsMatch(url);
        }
        /// <summary>
        /// Overi meno serioveho portu
        /// </summary>
        /// <param name="portName">Meno portu ktore chceme voerit</param>
        /// <returns>True = meno portu je spravne, inak false</returns>
        public static Boolean ValidatePortName(String portName)
        {
            //osetrenies
            if (String.IsNullOrEmpty(portName))
                return false;

            //regularny vyraz na overnie mena portu
            String pattern = @"^[Cc][Oo][Mm][1-9]\d?\d?$";

            //overime meno
            return Regex.IsMatch(portName, pattern);
        }
        /// <summary>
        /// Vrati cas vytvorenia Buildu z verzie ktora bola vytvorena sposobom [X.X.*]
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Ak nebol vstupny argument inicializovany
        /// </exception>
        /// <param name="version">Verzia assembly</param>
        /// <returns>Datum a cas vytvorenia buildu</returns>
        public static DateTime GetDateTimeFromVersion(Version version)
        {
            //osetrenie
            if (version == null)
                throw new ArgumentNullException("version");

            //inicializacia
            DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);

            //Build - pocet dni od zaciatku roka 2000
            //Revision - pocet dvoj sekundoviek aktualneho dna, podla UTC casu
            dateTime = dateTime.AddDays(version.Build).AddSeconds(version.Revision * 2);

            //vratime datum
            return dateTime;
        }
        /// <summary>
        /// Overi spravnost zadanej emailovej adresy
        /// </summary>
        /// <param name="address">Adresa ktoru chceme overit</param>
        /// <returns>True = adresa je spravna</returns>
        public static Boolean ValidateMailAddress(String address)
        {
            //osetrenies
            if (String.IsNullOrEmpty(address))
                return false;

            //regularny vyraz na overenie addresy
            String pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|" +
                             @"(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
           
            //overenei adresy
            return Regex.IsMatch(address, pattern);
        }
        /// <summary>
        /// method to validate an IP address
        /// using regular expressions. The pattern
        /// being used will validate an ip address
        /// with the range of 1.0.0.0 to 255.255.255.255
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns></returns>
        public static Boolean ValidateIpAddress(String address)
        {
            //create our match pattern
            string pattern = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
            //create our Regular Expression object
            Regex check = new Regex(pattern);
            //boolean variable to hold the status
            bool valid = false;
            //check to make sure an ip address was provided
            if (String.IsNullOrEmpty(address))
            {
                //no address provided so return false
                valid = false;
            }
            else
            {
                //address provided so use the IsMatch Method
                //of the Regular Expression object
                valid = check.IsMatch(address, 0);
            }
            //return the results
            return valid;
        }
        /// <summary>
        /// Na zaklade vstupneho DateTime vrati UnixTime
        /// </summary>
        /// <param name="date">Vstupny datum z ktoreho ratame unixTime</param>
        /// <returns>UnixTime ohraniceny na processingTimeout</returns>
        public static Int32 ConvertDateTimeToUnixTime(DateTime date)
        {
            //vypocitame unix time
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return (Int32)(diff.TotalSeconds);
        }
        /// <summary>
        /// Prepocita UnixTime na DateTime foramt
        /// </summary>
        /// <param name="Seconds">UnixTime</param>
        /// <returns>DateTime</returns>
        public static DateTime ConvertUnixTimeToDateTime(Int32 Seconds)
        {
            //vypocitame unix time
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            origin = origin.AddSeconds(Seconds);
            return origin;
        }
        #endregion
    }
}
