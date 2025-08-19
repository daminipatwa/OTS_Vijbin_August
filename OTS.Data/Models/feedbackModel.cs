using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Models
{
    public class feedbackModel
    {
        public int? id { get; set; }
        public string msg { get; set; }
        public int clientid { get; set; }
        public int userid { get; set; }
        public int rate { get; set; }
    }
}
