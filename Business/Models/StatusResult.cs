namespace Business.Models;
using Data.Models;

public class StatusResult<T> : ServiceResult
{

    public string Id { get; set; } = null!;
    public string StatusName { get; set; } = null!;
    public T? Result { get; set; }
}

public class StatusResult : ServiceResult
{

}
