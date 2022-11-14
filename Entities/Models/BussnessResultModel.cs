using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class BussnessResultModel<T>
    {
        public BussnessResultModel(T data, string message = "", bool success = true)
        {
            Success = success;
            Data = data;
            Message = message;
        }

        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message  { get; set; }
    }
}
