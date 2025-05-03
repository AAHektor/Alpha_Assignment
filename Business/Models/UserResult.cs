namespace Business.Models;

using Data.Entities;
using Data.Models;

public class UserResult : ServiceResult
{
    public IEnumerable<User>? Result { get; set; }
    public UserEntity? User { get; set; }

}
