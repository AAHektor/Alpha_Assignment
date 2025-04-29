using Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class Client
{
    public string Id { get; set; } = null!;
    public string ClientId { get; set; } = null!;
}
