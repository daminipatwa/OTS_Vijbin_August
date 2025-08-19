using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Models
{
    public class VisitingCardModel
    {
        public int id { get; set; }

        public int productid { get; set; }
  
    public string company_name { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string contactno1 { get; set; }
    public string contactno2 { get; set; }
    public string emailid { get; set; }
    public string designation { get; set; }
    public string address { get; set; }
    public int qty { get; set; }
        public string ImageUrl { get; set; }
    }

    public class PrintingModel
    {
        public int id { get; set; }

        public int productid { get; set; }

        public string location { get; set; }
        public string companyname { get; set; }
        public string address { get; set; }
        public string telno { get; set; }
        public string mobileno { get; set; }
        public string website { get; set; }
        public string designation { get; set; }
       
    }
}
