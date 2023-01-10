using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModel
{
    public class IndexCartVM
    {
        public decimal Tax { get; set; }
        public List<CartVM> Cart { get; set; }
    }
}
