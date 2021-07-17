using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClassLibrary.Results
{

    internal interface ICallResultData
    {
        object Data { get; set; }
    }

    internal interface ICallResultData<T>
    {
        T Data { get; set; }
    }

    public abstract class CallResultBase
    {
        /// <summary>
        /// </summary>
        private string _msg;

        /// <summary>
        /// 构造函数
        /// </summary>
        protected CallResultBase()
        {
            Success = true;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_success"></param>
        protected CallResultBase(bool _success)
        {
            Success = _success;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// 业务代码
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }


        /// <summary>
        /// 返回信息
        /// </summary>
        [JsonProperty("msg")]
        public string Msg
        {
            get => _msg ?? string.Empty;
            set => _msg = value;
        }
    }

    /// <summary>
    /// 请求数据回调结果对象
    /// </summary>
    public class CallResult : CallResultBase, ICallResultData
    {
        [JsonProperty("data")] public object Data { get; set; }

        public static CallResult Ok(string msg = "", int code = 0)
        {
            CallResult result = new CallResult
            {
                Success = true,
                Code = code,
                Msg = msg
            };
            return result;
        }

        public static CallResult<T> Ok<T>(T data, string msg = "", int code = 0)
        {
            CallResult<T> result = new CallResult<T>
            {
                Data = data,
                Success = true,
                Code = code,
                Msg = msg
            };
            return result;
        }

        public static CallResult Fail(string msg, int code = 0, object data = null)
        {
            CallResult result = new CallResult
            {
                Success = false,
                Msg = msg,
                Code = code,
                Data = data
            };
            return result;
        }

        public static CallResult<T> Fail<T>(string msg, int code = 0, T data = default)
        {
            CallResult<T> result = new CallResult<T>
            {
                Success = false,
                Msg = msg,
                Code = code,
                Data = data
            };
            return result;
        }
    }

    /// <summary>
    /// 请求数据回调结果对象，传入返回类型
    /// </summary>
    public class CallResult<T> : CallResultBase, ICallResultData<T>
    {
        public CallResult()
        {
            Data = default;
        }

        /// <summary>
        /// 类型数据
        /// </summary>
        [JsonProperty("data")]
        public T Data { get; set; }
    }


    public class PagingCallResult<T> : CallResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PagingCallResult()
        {
            Success = true;
        }

        /// <summary>
        /// 分页后集合对象
        /// </summary>
        [JsonProperty("listData")]
        public List<T> ListData { get; set; }

        /// <summary>
        /// 数据总数
        /// </summary>
        [JsonProperty("count")]
        public long Count { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        [JsonProperty("totalPage")]
        public int TotalPage
        {
            get
            {
                if (this.Count == 0)
                {
                    return 0;
                }
                var page = (decimal)this.Count / this.Limit;
                return Convert.ToInt16(Math.Ceiling(page));
            }
        }

        /// <summary>
        /// 获取带底部分页数据
        /// </summary>
        /// <returns></returns>
        public FooterPagingCallResult<T> GetFooterPagingCallResult()
        {
            var result = new FooterPagingCallResult<T>
            {
                Code = this.Code,
                Count = this.Count,
                Data = this.Data,
                Limit = this.Limit,
                ListData = this.ListData,
                Msg = this.Msg,
                Page = this.Page,
                Success = this.Success,
                FooterData = new List<T>()
            };
            return result;
        }
    }

    /// <summary>
    /// 请求带底部表格分页数据回调结果对象，传入返回类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FooterPagingCallResult<T> : PagingCallResult<T>
    {
        /// <summary>
        /// 脚部数据对象
        /// </summary>
        [JsonProperty("footerData")]
        public List<T> FooterData { get; set; }

    }
}
