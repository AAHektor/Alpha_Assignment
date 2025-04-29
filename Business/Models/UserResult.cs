namespace Business.Models;
using Data.Models;

public class UserResult : ServiceResult
{
    public IEnumerable<User>? Result { get; set; }
}
