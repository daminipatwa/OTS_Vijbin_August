using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Models
{
    public class VendorModel
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string emailid { get; set; }
        public string phoneno { get; set; }
        public string state { get; set; }
        public string gstno { get; set; }
        public string location { get; set; }
        public string region { get; set; }
        public int servicecategory { get; set; }
        public int createdby { get; set; }
        public int updatedby { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}
