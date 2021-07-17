using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace ClassLibrary.Exceptions
{
    [Serializable]
    public class FriendlyException : Exception
    {

        public int Code { get; set; } = 500;

        [JsonProperty("data")]
        public object Data { get; set; }

        public FriendlyException()
        {
        }


        public FriendlyException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }


        public FriendlyException(string message, int code = 500, object data = null)
            : base(message)
        {
            this.Code = code;
            this.Data = data;
        }

        public FriendlyException(string message, Exception innerException, int code = 500)
            : base(message, innerException)
        {
            this.Code = code;
        }
    }
}
