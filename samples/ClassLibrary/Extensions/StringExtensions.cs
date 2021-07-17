using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using QRCoder;
using System.Data;

namespace ClassLibrary.Extensions
{
    public static class StringExtensions
    {


        public static string TrimStart(this string source, string trim,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (source == null) return null;

            var s = source;
            while (s.StartsWith(trim, stringComparison)) s = s.Substring(trim.Length);

            return s;
        }

        /// <summary>
        ///     分割逗号的字符串为List<string>
        /// </summary>
        /// <param name="csvList"></param>
        /// <param name="nullOrWhitespaceInputReturnsNull">nullorwhitespace字符串是否返回空对象</param>
        /// <returns></returns>
        public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable()
                .Select(s => s.Trim())
                .ToList();
        }

        public static bool IsNullOrWhitespace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static Guid ToGuid(this string s)
        {
            return Guid.Parse(s);
        }

        public static int ToInt(this string s)
        {
            if (int.TryParse(s, out var r))
            {
                return r;
            }
            return 0;
        }

        /// <summary>
        /// TryToDouble
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static double TryToDouble(this string key)
        {
            double res = 0;
            double.TryParse(key, out res);
            return res;
        }

        /// <summary>
        /// TryToDecimal
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static decimal TryToDecimal(this string key)
        {
            decimal res = 0m;
            decimal.TryParse(key, out res);
            return res;
        }

        /// <summary>
        ///     Adds a char to end of given string if it does not ends with the char.
        /// </summary>
        public static string EnsureEndsWith(this string str, char c)
        {
            return EnsureEndsWith(str, c, StringComparison.Ordinal);
        }

        /// <summary>
        ///     Adds a char to end of given string if it does not ends with the char.
        /// </summary>
        public static string EnsureEndsWith(this string str, char c, StringComparison comparisonType)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            if (str.EndsWith(c.ToString(), comparisonType)) return str;

            return str + c;
        }

        /// <summary>
        ///     Adds a char to end of given string if it does not ends with the char.
        /// </summary>
        public static string EnsureEndsWith(this string str, char c, bool ignoreCase, CultureInfo culture)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            if (str.EndsWith(c.ToString(culture), ignoreCase, culture)) return str;

            return str + c;
        }

        /// <summary>
        ///     Adds a char to beginning of given string if it does not starts with the char.
        /// </summary>
        public static string EnsureStartsWith(this string str, char c)
        {
            return EnsureStartsWith(str, c, StringComparison.Ordinal);
        }

        /// <summary>
        ///     Adds a char to beginning of given string if it does not starts with the char.
        /// </summary>
        public static string EnsureStartsWith(this string str, char c, StringComparison comparisonType)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            if (str.StartsWith(c.ToString(), comparisonType)) return str;

            return c + str;
        }

        /// <summary>
        ///     Adds a char to beginning of given string if it does not starts with the char.
        /// </summary>
        public static string EnsureStartsWith(this string str, char c, bool ignoreCase, CultureInfo culture)
        {
            if (str == null) throw new ArgumentNullException("str");

            if (str.StartsWith(c.ToString(culture), ignoreCase, culture)) return str;

            return c + str;
        }

        /// <summary>
        ///     Indicates whether this string is null or an System.String.Empty string.
        /// </summary>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            return !str.IsNullOrEmpty();
        }

        /// <summary>
        ///     indicates whether this string is null, empty, or consists only of white-space characters.
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        ///     Gets a substring of a string from beginning of the string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str" /> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="len" /> is bigger that string's length</exception>
        public static string Left(this string str, int len)
        {
            if (str == null) throw new ArgumentNullException("str");

            if (str.Length < len)
                throw new ArgumentException("len argument can not be bigger than given string's length!");

            return str.Substring(0, len);
        }

        /// <summary>
        ///     Converts line endings in the string to <see cref="Environment.NewLine" />.
        /// </summary>
        public static string NormalizeLineEndings(this string str)
        {
            return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
        }

        /// <summary>
        ///     Gets index of nth occurence of a char in a string.
        /// </summary>
        /// <param name="str">source string to be searched</param>
        /// <param name="c">Char to search in <see cref="str" /></param>
        /// <param name="n">Count of the occurence</param>
        public static int NthIndexOf(this string str, char c, int n)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            var count = 0;
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] != c) continue;

                if (++count == n) return i;
            }

            return -1;
        }

        ///// <summary>
        /////     Removes first occurrence of the given postfixes from end of the given string.
        /////     Ordering is important. If one of the postFixes is matched, others will not be tested.
        ///// </summary>
        ///// <param name="str">The string.</param>
        ///// <param name="postFixes">one or more postfix.</param>
        ///// <returns>Modified string or the same string if it has not any of given postfixes</returns>
        //public static string RemovePostFix(this string str, params string[] postFixes)
        //{
        //    if (str == null) return null;

        //    if (str == string.Empty) return string.Empty;

        //    if (postFixes.IsNullOrEmpty()) return str;

        //    foreach (var postFix in postFixes)
        //        if (str.EndsWith(postFix))
        //            return str.Left(str.Length - postFix.Length);

        //    return str;
        //}

        ///// <summary>
        /////     Removes first occurrence of the given prefixes from beginning of the given string.
        /////     Ordering is important. If one of the preFixes is matched, others will not be tested.
        ///// </summary>
        ///// <param name="str">The string.</param>
        ///// <param name="preFixes">one or more prefix.</param>
        ///// <returns>Modified string or the same string if it has not any of given prefixes</returns>
        //public static string RemovePreFix(this string str, params string[] preFixes)
        //{
        //    if (str == null) return null;

        //    if (str == string.Empty) return string.Empty;

        //    if (preFixes.IsNullOrEmpty()) return str;

        //    foreach (var preFix in preFixes)
        //        if (str.StartsWith(preFix))
        //            return str.Right(str.Length - preFix.Length);

        //    return str;
        //}

        /// <summary>
        ///     Gets a substring of a string from end of the string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str" /> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="len" /> is bigger that string's length</exception>
        public static string Right(this string str, int len)
        {
            if (str == null) throw new ArgumentNullException("str");

            if (str.Length < len)
                throw new ArgumentException("len argument can not be bigger than given string's length!");

            return str.Substring(str.Length - len, len);
        }

        /// <summary>
        ///     Uses string.Split method to split given string by given separator.
        /// </summary>
        public static string[] Split(this string str, string separator)
        {
            return str.Split(new[] { separator }, StringSplitOptions.None);
        }

        /// <summary>
        ///     Uses string.Split method to split given string by given separator.
        /// </summary>
        public static string[] Split(this string str, string separator, StringSplitOptions options)
        {
            return str.Split(new[] { separator }, options);
        }

        /// <summary>
        ///     Uses string.Split method to split given string by <see cref="Environment.NewLine" />.
        /// </summary>
        public static string[] SplitToLines(this string str)
        {
            return str.Split(Environment.NewLine);
        }

        /// <summary>
        ///     Uses string.Split method to split given string by <see cref="Environment.NewLine" />.
        /// </summary>
        public static string[] SplitToLines(this string str, StringSplitOptions options)
        {
            return str.Split(Environment.NewLine, options);
        }

        /// <summary>
        ///     Converts PascalCase string to camelCase string.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="invariantCulture">Invariant culture</param>
        /// <returns>camelCase of the string</returns>
        public static string ToCamelCase(this string str, bool invariantCulture = true)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            if (str.Length == 1) return invariantCulture ? str.ToLowerInvariant() : str.ToLower();

            return (invariantCulture ? char.ToLowerInvariant(str[0]) : char.ToLower(str[0])) + str.Substring(1);
        }

        /// <summary>
        ///     Converts PascalCase string to camelCase string in specified culture.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="culture">An object that supplies culture-specific casing rules</param>
        /// <returns>camelCase of the string</returns>
        public static string ToCamelCase(this string str, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            if (str.Length == 1) return str.ToLower(culture);

            return char.ToLower(str[0], culture) + str.Substring(1);
        }

        /// <summary>
        ///     Converts given PascalCase/camelCase string to sentence (by splitting words by space).
        ///     Example: "ThisIsSampleSentence" is converted to "This is a sample sentence".
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <param name="invariantCulture">Invariant culture</param>
        public static string ToSentenceCase(this string str, bool invariantCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            return Regex.Replace(
                str,
                "[a-z][A-Z]",
                m => m.Value[0] + " " +
                     (invariantCulture ? char.ToLowerInvariant(m.Value[1]) : char.ToLower(m.Value[1]))
            );
        }

        /// <summary>
        ///     Converts given PascalCase/camelCase string to sentence (by splitting words by space).
        ///     Example: "ThisIsSampleSentence" is converted to "This is a sample sentence".
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        public static string ToSentenceCase(this string str, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1], culture));
        }

        /// <summary>
        ///     Converts string to enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="value">String value to convert</param>
        /// <returns>Returns enum object</returns>
        public static T ToEnum<T>(this string value)
            where T : struct
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        ///     Converts string to enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="value">String value to convert</param>
        /// <param name="ignoreCase">Ignore case</param>
        /// <returns>Returns enum object</returns>
        public static T ToEnum<T>(this string value, bool ignoreCase)
            where T : struct
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static string ToMd5(this string str)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(str);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var hashByte in hashBytes) sb.Append(hashByte.ToString("X2"));

                return sb.ToString();
            }
        }

        /// <summary>
        ///     Converts camelCase string to PascalCase string.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="invariantCulture">Invariant culture</param>
        /// <returns>PascalCase of the string</returns>
        public static string ToPascalCase(this string str, bool invariantCulture = true)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            if (str.Length == 1) return invariantCulture ? str.ToUpperInvariant() : str.ToUpper();

            return (invariantCulture ? char.ToUpperInvariant(str[0]) : char.ToUpper(str[0])) + str.Substring(1);
        }

        /// <summary>
        ///     Converts camelCase string to PascalCase string in specified culture.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="culture">An object that supplies culture-specific casing rules</param>
        /// <returns>PascalCase of the string</returns>
        public static string ToPascalCase(this string str, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            if (str.Length == 1) return str.ToUpper(culture);

            return char.ToUpper(str[0], culture) + str.Substring(1);
        }

        /// <summary>
        ///     Gets a substring of a string from beginning of the string if it exceeds maximum length.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str" /> is null</exception>
        public static string Truncate(this string str, int maxLength)
        {
            if (str == null) return null;

            if (str.Length <= maxLength) return str;

            return str.Left(maxLength);
        }

        /// <summary>
        ///     Gets a substring of a string from beginning of the string if it exceeds maximum length.
        ///     It adds a "..." postfix to end of the string if it's truncated.
        ///     Returning string can not be longer than maxLength.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str" /> is null</exception>
        public static string TruncateWithPostfix(this string str, int maxLength)
        {
            return TruncateWithPostfix(str, maxLength, "...");
        }

        /// <summary>
        ///     Gets a substring of a string from beginning of the string if it exceeds maximum length.
        ///     It adds given <paramref name="postfix" /> to end of the string if it's truncated.
        ///     Returning string can not be longer than maxLength.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str" /> is null</exception>
        public static string TruncateWithPostfix(this string str, int maxLength, string postfix)
        {
            if (str == null) return null;

            if (str == string.Empty || maxLength == 0) return string.Empty;

            if (str.Length <= maxLength) return str;

            if (maxLength <= postfix.Length) return postfix.Left(maxLength);

            return str.Left(maxLength - postfix.Length) + postfix;
        }

        /// <summary>
        ///     Json反序列化
        /// </summary>
        /// <typeparam name="T">T类型</typeparam>
        /// <param name="value">反序列化后得到的对象</param>
        /// <returns></returns>
        public static T JsonToObject<T>(this string value)
        {
            var type = typeof(T);
            var tReturn = type.Assembly.CreateInstance(type.FullName);
            try
            {
                tReturn = JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
            }

            return (T)tReturn;
        }

        /// <summary>
        ///     将字符串转换成Base64字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static string StringToBase64(this string value)
        {
            var byteArray = Encoding.Default.GetBytes(value);
            //先转成byte[];
            return Convert.ToBase64String(byteArray);
        }

        /// <summary>
        ///     将Base64字符转成String
        /// </summary>
        /// <param name="value">Base64字符串</param>
        /// <returns></returns>
        public static string Base64ToString(this string value)
        {
            var byteArray = Convert.FromBase64String(value);
            return Encoding.Default.GetString(byteArray);
        }

        #region AES加密解密 

        /// <summary>
        /// 128位处理key
        /// </summary>
        /// <param name="keyArray">原字节</param>
        /// <param name="key">处理key</param>
        /// <returns></returns>
        private static byte[] GetAesKey(byte[] keyArray, string key)
        {
            var newArray = new byte[16];
            if (keyArray.Length < 16)
            {
                for (var i = 0; i < newArray.Length; i++)
                {
                    if (i >= keyArray.Length)
                    {
                        newArray[i] = 0;
                    }
                    else
                    {
                        newArray[i] = keyArray[i];
                    }
                }
            }
            return newArray;
        }

        /// <summary>
        /// 使用AES加密字符串,按128位处理key
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <param name="autoHandle"></param>
        /// <returns>Base64字符串结果</returns>
        public static string ToAESEncrypt(this string content, string key, bool autoHandle = true)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            if (autoHandle) keyArray = GetAesKey(keyArray, key);
            var toEncryptArray = Encoding.UTF8.GetBytes(content);

            SymmetricAlgorithm des = Aes.Create();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            var cTransform = des.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }

        /// <summary>
        /// 使用AES解密字符串,按128位处理key
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <param name="autoHandle"></param>
        /// <returns>UTF8解密结果</returns>
        public static string ToAESDecrypt(this string content, string key, bool autoHandle = true)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            if (autoHandle) keyArray = GetAesKey(keyArray, key);
            var toEncryptArray = Convert.FromBase64String(content);

            SymmetricAlgorithm des = Aes.Create();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            var cTransform = des.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// 使用AES加密字符串,按32位处理key
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="key">秘钥，需要32位</param>
        /// <param name="iv">需要16位</param>
        /// <returns>Base64字符串结果</returns>
        public static string ToAESEncrypt(this string content, string key, string iv)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 32));
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.IV = Encoding.UTF8.GetBytes(iv.Substring(0, 16));
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(content);
                        }
                        byte[] bytes = msEncrypt.ToArray();
                        return ByteArrayToHexString(bytes);
                    }
                }
            }
        }

        /// <summary>
        /// 使用AES解密字符串,按32位处理key
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="key">秘钥，需要32位</param>
        /// <param name="iv">需要16位</param>
        /// <returns>UTF8解密结果</returns>
        public static string ToAESDecrypt(this string content, string key, string iv)
        {
            byte[] inputBytes = HexStringToByteArray(content);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 32));
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.IV = Encoding.UTF8.GetBytes(iv.Substring(0, 16));

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream(inputBytes))
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srEncrypt = new StreamReader(csEncrypt))
                        {
                            return srEncrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将指定的16进制字符串转换为byte数组
        /// </summary>
        /// <param name="s">16进制字符串(如：“7F 2C 4A”或“7F2C4A”都可以)</param>
        /// <returns>16进制字符串对应的byte数组</returns>
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            }
            return buffer;
        }

        /// <summary>
        /// 将一个byte数组转换成一个格式化的16进制字符串
        /// </summary>
        /// <param name="data">byte数组</param>
        /// <returns>格式化的16进制字符串</returns>
        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                //16进制数字
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                //16进制数字之间以空格隔开
                //sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            return sb.ToString().ToLower();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private const string ImageExtNames = ".jpeg|.jpg|.gif|.bmp|.png";

        /// <summary>
        /// 
        /// </summary>
        private const string AudioExtNames = ".mp3|.ogg|.wav|.ape|.cda|.au|.midi|.mac|.aac";

        /// <summary>
        /// 
        /// </summary>
        private const string VideoExtNames = ".rmvb|.wmv|.asf|.avi|.3gp|.mpg|.mkv|.mp4|.dvd|.ogm|.mov|.mpeg2|.mpeg4";

        ///// <summary>
        ///// 扩展名返回文件类型
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static EnumFileType ExtNameToFileType(this string value)
        //{
        //    value = value.ToLower();
        //    if (ImageExtNames.Contains(value)) { return EnumFileType.Image; }
        //    else if (AudioExtNames.Contains(value)) { return EnumFileType.Audio; }
        //    else if (VideoExtNames.Contains(value)) { return EnumFileType.Video; }

        //    return EnumFileType.None;
        //}

        ///// <summary>
        ///// 文件名返回文件类型
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static EnumFileType FileNameToFileType(this string value)
        //{
        //    var extensionName = Path.GetExtension(value);
        //    return extensionName.ExtNameToFileType();
        //}

        static string LASTRADNSTRING = string.Empty;

        /// <summary>
        /// 随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandString(this int length)
        {

            var chars = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            Random r = new Random();
            StringBuilder stringBuilder = new StringBuilder();
            //生成一个8位长的随机字符，具体长度可以自己更改
            int i = 0;
            while (i < length)
            {
                int m = r.Next(0, 62);//这里下界是0，随机数可以取到，上界应该是75，因为随机数取不到上界，也就是最大74，符合我们的题意
                if (LASTRADNSTRING.Contains(chars[m]))
                {
                    continue;
                }
                i++;
                stringBuilder.Append(chars[m]);

            }
            LASTRADNSTRING = stringBuilder.ToString();
            return LASTRADNSTRING;
        }

        /// <summary>
        /// 随机整型字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandIntString(this int length)
        {
            Random rd = new Random();
            StringBuilder stringBuilder = new StringBuilder();
            int i = 0;
            while (i < length)
            {
                var temp = rd.Next(0, 10).ToString();
                if (LASTRADNSTRING.Contains(temp))
                {
                    continue;
                }
                i++;
                stringBuilder.Append(temp);
            }

            LASTRADNSTRING = stringBuilder.ToString();
            return stringBuilder.ToString();
        }

        #region 生成二维码

        /// <summary>
        /// 将字符串转成二维码
        /// </summary>
        /// <param name="value">需要转换的值</param>
        /// <returns>转换完成的Bitmap</returns>
        public static Bitmap ToQrCode(this string value)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(value, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return qrCodeImage;
        }


        /// <summary>
        /// 将字符串转成二维码Base64String
        /// </summary>
        /// <param name="value">需要转换的值</param>
        /// <returns>转换完成的Bitmap</returns>
        public static string ToQrCodeBase64String(this string value)
        {
            MemoryStream ms = new MemoryStream();
            var bmp = value.ToQrCode();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            return Convert.ToBase64String(arr);
        }

        #endregion

        #region PINYIN

        ///// <summary>
        ///// 将字符串转成拼音
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static string ToPinyin(this string value)
        //{
        //    if (value.IsNullOrEmpty())
        //    {
        //        throw new FriendlyException("待转换字符不可为空");
        //    }

        //    return NPinyin.Pinyin.GetPinyin(value, Encoding.Default);
        //}

        ///// <summary>
        ///// 将字符串转成拼音首字母（大写）
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static string ToPinyinInitials(this string value)
        //{
        //    if (value.IsNullOrEmpty())
        //    {
        //        throw new FriendlyException("待转换字符不可为空");
        //    }
        //    return NPinyin.Pinyin.GetInitials(value, Encoding.Default);
        //}

        #endregion

        /// <summary>
        /// HtmlEncode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return "";
            return System.Web.HttpUtility.HtmlEncode(value);
        }

        /// <summary>
        /// HtmlDecode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return "";
            return System.Web.HttpUtility.HtmlDecode(value);
        }


        public static DataTable ToDataTable(this string json)
        {
            return JsonConvert.DeserializeObject<DataTable>(json);
        }


        /// <summary>
        /// 判断输入的字符串是否是一个合法的手机号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(this string str)
        {
            if (str.IsNullOrWhiteSpace()) return false;

            return Regex.IsMatch(str, @"^[1]+\d{10}");
        }
    }
}
