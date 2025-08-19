using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Models
{
    public class NotificationModel
    {
            public int id { get; set; }
            public int fromid { get; set; }
        public int toid { get; set; }
        public string from_type { get; set; }
        public string to_type { get; set; }
        public string msg { get; set; }
        public string url { get; set; }
        public string is_read { get; set; }
    }
}
