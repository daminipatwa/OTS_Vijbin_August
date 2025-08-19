using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Models
{
    public class CartModel
    {
        public int Id { get; set; }
        public int productid { get; set; }
        public int qty { get; set; }
        public decimal rate { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Make { get; set; }
        public string Category { get; set; }
        public string HSN { get; set; }
        public string createdate { get; set; }
        public string stock { get; set; }
        public decimal totalamount { get; set; }
        public string gst { get; set; }
        public decimal gst_amt { get; set; }
    }
}
