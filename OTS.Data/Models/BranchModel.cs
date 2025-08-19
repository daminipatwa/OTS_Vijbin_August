using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Models
{
    public class BranchModel
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? createdon { get; set; }
        public string? updatedon { get; set; }
        public int createdby { get; set; }
        public int updatedby { get; set; }

    }
}
