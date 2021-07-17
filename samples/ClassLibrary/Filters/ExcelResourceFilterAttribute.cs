using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Data;
using OfficeOpenXml;
using System.Linq;
using ClassLibrary.Extensions;
using ClassLibrary.Results;
using ClassLibrary.Exceptions;

namespace ClassLibrary.Filters
{
    /// <summary>
    /// Excel导出Filter
    /// </summary>
    public class ExcelResourceFilterAttribute : Attribute, IAsyncResultFilter, IResourceFilter
    {
        private const string ExportHeaderKey = "XH-EXPORT-EXCEL";


        /// <summary>
        /// 导出最大记录数
        /// </summary>
        public int Limit { get; set; } = 10000;

        /// <summary>
        /// 忽略的字段，以“,”分隔
        /// </summary>
        public string IgnoreFields { get; set; }

        ///// <summary>
        ///// 保留的字段，以“,”分隔
        ///// </summary>
        //public string RetainFields { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }


        private bool IsAction(HttpContext context)
        {
            var exportHeader = context.Request.GetHeader<string>(ExportHeaderKey);
            return exportHeader.IsNotNullOrEmpty() ? true : false;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!IsAction(context.HttpContext)) return;

            var request = context.HttpContext.Request;
            //启动倒带方式
            request.EnableBuffering();
            request.Body.Position = 0;

            var requestReader = new StreamReader(request.Body);
            var requestContent = requestReader.ReadToEnd();
            var jsonObject = JObject.Parse(requestContent);

            if (jsonObject["limit"] != null)
            {
                jsonObject["limit"] = this.Limit;
            }

            using (request.Body)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(jsonObject.ToString());
                request.Body = new MemoryStream(bytes);
            }

        }

        /// <summary>
        /// 操作context.Result无效
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!IsAction(context.HttpContext)) { await next(); }
            else
            {
                var result = context.Result;
                if (result is ObjectResult objectResult)
                {
                    var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                    var json = JsonConvert.SerializeObject(objectResult.Value, timeConverter);
                    DataTable dt;
                    if (objectResult.Value.GetType().Name.Contains("FooterPagingCallResult")) //  IsAssignableFrom(typeof(CallResult))
                    {
                        var res = json.JsonToObject<FooterPagingCallResult<object>>();
                        dt = res.ListData.ToJsonString().ToDataTable();
                    }
                    else if (objectResult.Value.GetType().Name.Contains("PagingCallResult"))
                    {
                        var res = json.JsonToObject<PagingCallResult<object>>();
                        dt = res.ListData.ToJsonString().ToDataTable();
                    }
                    else
                    {
                        var res = json.JsonToObject<IEnumerable<object>>();
                        dt = res.ToJsonString().ToDataTable();
                    }

                    if (dt == null || dt.Rows.Count <= 0)
                    {
                        throw new FriendlyException("暂无数据可导出");
                    }

                    var templateFilePath = Path.Combine("Export", this.TemplateName);
                    FileInfo newFile = new FileInfo(templateFilePath);


                    byte[] bytes;
                    using (ExcelPackage pk = new ExcelPackage(newFile))
                    {
                        var worksheet = pk.Workbook.Worksheets[0];

                        var _ignoreFields = IgnoreFields.Split(',');

                        if (_ignoreFields.Length > 0)
                        {
                            foreach (var field in _ignoreFields)
                            {
                                dt.Columns.Remove(field);
                            }
                        }

                        worksheet.Cells["A2"].LoadFromDataTable(dt, false);

                        //worksheet.Column(1).Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                        //worksheet.Column(4).Style.Numberformat.Format = "#";

                        bytes = pk.GetAsByteArray();
                    }

                    var filename = Guid.NewGuid().ToString().Replace("-", "");
                    var fileExt = Path.GetExtension(templateFilePath);

                    //var provider = new FileExtensionContentTypeProvider();
                    //var memi = provider.Mappings[fileExt];

                    //context.HttpContext.Response.Clear();
                    //context.HttpContext.Response.ContentType = memi;
                    //context.HttpContext.Response.BodyWriter.FlushAsync();


                    context.HttpContext.Response.Headers.Add("Content-Disposition", $"attachment;filename={filename + fileExt}");
                    context.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    if (bytes != null) context.HttpContext.Response.Body.Write(bytes, 0, bytes.Length);
                }
                else
                {
                    await next();
                }
            }
        }

    }



    public static class ExcelResourceFilterExtensions
    {
        public static T GetHeader<T>(this HttpRequest request, string key, T defalutValue = default)
        {
            var result = request.Headers[key].FirstOrDefault();
            if (result == null)
            {
                return defalutValue;
            }
            return result.CastTo<T>();
        }
    }
}
