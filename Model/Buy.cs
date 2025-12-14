using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

[Table("Buy")]
public class Buy
{
    [Key]
    public int id;
}
