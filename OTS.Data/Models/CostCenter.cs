using System.ComponentModel.DataAnnotations;

namespace OTS.Data.Models;
public class CostCenter
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    public string? createdon { get; set; }
    public string? updatedon { get; set; }
    public int createdby  { get; set; }
    public int updatedby { get; set; }
}
