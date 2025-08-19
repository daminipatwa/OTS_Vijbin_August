using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Models
{
    public class CorpUserModel
    {
        public int Id { get; set; }
        public int vendorid { get; set; }
        public string firstname { get; set; } 
        public string lastname { get; set; }
        public string contactno { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string emailid { get; set; }
        public int usertype { get; set; }
        public int department { get; set; }

        public string? department_name { get; set; }
        public int approval_type { get; set; }
        public int approveby { get; set; }
        public int branchid { get; set; }
        public string? branch_name { get; set; }
        public string? tat { get; set; }
        public string? limitamount { get; set; }
        public string? regions { get; set; }

        public string billingaddress { get; set; }
        public string shippingaddress1 { get; set; }
        public string? shippingaddress2 { get; set; }
        public int clientid { get; set; }

        public string? client_name { get; set; }
        public int createdby { get; set; }
        public int updatedby { get; set;}

        public string? approvalby { get; set; }

    }
}
