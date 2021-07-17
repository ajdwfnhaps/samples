using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ClassLibrary.Extensions
{
    /// <summary>
    ///     基类型<see cref="object" />扩展辅助操作类
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ToDate(this object obj)
        {
            var date = Convert.ToDateTime("1900-01-01");
            if (obj != null)
            {
                if (obj.Equals(DBNull.Value)) return date;
                if (DateTime.TryParse(obj.ToString(), out date)) return date;
            }

            return date;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }


        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static DateTime ToDate(this object obj, DateTime defValue)
        {
            if (obj != null)
            {
                if (obj.Equals(DBNull.Value)) return defValue;
                if (DateTime.TryParse(obj.ToString(), out defValue)) return defValue;
            }

            return defValue;
        }

        /// <summary>
        ///     object强制转化为DateTime类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? ToDateNull(this object obj)
        {
            if (obj == null) return null;
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch
            {
                return null;
            }
        }

        #region 公共方法

        /// <summary>
        ///     把对象类型转换为指定类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object CastTo(this object value, Type conversionType)
        {
            if (value == null) return null;
            if (conversionType.IsNullableType()) conversionType = conversionType.GetUnNullableType();
            if (conversionType.IsEnum) return Enum.Parse(conversionType, value.ToString());
            if (conversionType == typeof(Guid)) return Guid.Parse(value.ToString());
            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        ///     把对象类型转化为指定类型
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败引发异常。 </returns>
        public static T CastTo<T>(this object value)
        {
            var result = CastTo(value, typeof(T));
            return (T)result;
        }

        /// <summary>
        ///     把对象类型转化为指定类型，转化失败时返回指定的默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <param name="defaultValue"> 转化失败返回的指定默认值 </param>
        /// <returns> 转化后的指定类型对象，转化失败时返回指定的默认值 </returns>
        public static T CastTo<T>(this object value, T defaultValue)
        {
            try
            {
                return CastTo<T>(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        ///     判断当前值是否介于指定范围内
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 动态类型对象 </param>
        /// <param name="start"> 范围起点 </param>
        /// <param name="end"> 范围终点 </param>
        /// <param name="leftEqual"> 是否可等于上限（默认等于） </param>
        /// <param name="rightEqual"> 是否可等于下限（默认等于） </param>
        /// <returns> 是否介于 </returns>
        public static bool IsBetween<T>(this IComparable<T> value, T start, T end, bool leftEqual = false,
            bool rightEqual = false) where T : IComparable
        {
            var flag = leftEqual ? value.CompareTo(start) >= 0 : value.CompareTo(start) > 0;
            return flag && (rightEqual ? value.CompareTo(end) <= 0 : value.CompareTo(end) < 0);
        }

        /// <summary>
        ///     将对象序列化为JSON字符串，不支持存在循环引用的对象
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="value">动态类型对象</param>
        /// <returns>JSON字符串</returns>
        public static string ToJsonString<T>(this T value)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };
            return ToJsonString(value, jsonSerializerSettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static string ToJsonString<T>(this T value, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(value, settings);
        }

        #endregion
    }
}
