using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exception
{
    public class ExceptionModel<T>
    {
        public ExceptionModel(T data, string message = "", bool success = true)
        {
            Success = success;
            Data = data;
            Message = message;
        }

        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
