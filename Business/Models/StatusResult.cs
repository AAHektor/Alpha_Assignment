namespace Business.Models;
using Data.Models;

public class StatusResult : ServiceResult
{

    public IEnumerable<Status>? Result { get; set; }
}
