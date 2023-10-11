using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.Common
{
    public class Response<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
        public T Data { get; set; }
        public int Count { get; set; }
    }
}
