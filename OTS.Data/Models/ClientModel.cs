using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Models
{
    public class ClientModel
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? emailid { get; set; }
        public string? mobileno { get; set; }
        public string? state { get; set; }
        public string? gst { get; set; }
        public string? location { get; set; }
        public string? category { get; set; }
        public string? region { get; set; }
        public string? createdon { get; set; }
        public string? updatedon { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
        public int createdby { get; set; }
        public int updatedby { get; set; }
    }
}
