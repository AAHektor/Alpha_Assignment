using Data.Entities;
using Data.Models;

namespace Business.Models;

public class AuthResult : ServiceResult
{
    public UserEntity? User { get; set; }
}

public class AuthResult<T> : ServiceResult
{
    public T? Result { get; set; }
}