using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Models
{
    public class SupplierDashboardModel
    {
        public string total_product { get; set; }
        public string complete_order { get; set; }
        public string incomplete_order { get; set; }
    }
}
