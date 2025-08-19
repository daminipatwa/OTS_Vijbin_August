using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Models
{
    public class AgreementModel
    {
        public int id { get; set; }
        public int clientid { get; set; }
        public string file_path { get; set; }
        public string createdon { get; set; }
        public string? client_name { get; set; }
    }
}
