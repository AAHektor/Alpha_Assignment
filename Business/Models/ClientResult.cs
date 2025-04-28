namespace Business.Models;
using Data.Models;

public class ClientResult : ServiceResult
{
    public IEnumerable<Client>? Result { get; set; }
}
