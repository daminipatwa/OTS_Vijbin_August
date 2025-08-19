using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OTS.Data.Models
{
    public class ProductModel
    {
        public int id { get; set; }
        public int wishlistid { get; set; }
        public string Description { get; set; }
            public string Make { get; set; }
        public string HSN { get; set; }
        public string Quantity { get; set; }
        public string Rate { get; set;  }
        public string Unit { get; set; }
        public string stock { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public string size { get; set; }
        public string gst { get; set; }
        public int? clientid { get; set; }
        public long totalrows { get; set; }

    }

   
}
