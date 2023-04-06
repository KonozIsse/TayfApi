using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModel
{
    public class HomeCPVM
    {
        public int TotalOrders { get; set; }
        public int TotalOutStock { get; set; }
        public decimal TotalPurchased { get; set; }
        public decimal TotalTransactions { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomerRegistrations { get; set; }
        public int TotalCarts { get; set; }
        public double CartsPercentage { get; set; }
        public int PendingOrders{ get; set; }
        public double PendingPercentage { get; set; }
        public int CansalOrders { get; set; }
        public double CansalPercentage { get; set; }
        public int CompleteOrders { get; set; }
        public double CompletePercentage { get; set; }
    }
}
