using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exception
{
    public class BussnessResultModel
    {
        public BussnessResultModel(object data, string message = "", bool success = true)
        {
            Success = success;
            Data = data;
            Message = message;
        }

        public bool Success { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }
}
