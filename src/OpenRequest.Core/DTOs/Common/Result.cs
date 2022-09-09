using OpenRequest.Core.Dtos.Errors;

namespace OpenRequest.Core.Dtos.Common;

public class Result<T>
{
    public T? Content { get; set; } 
    public Error? Error { get; set; }
    public bool IsSuccess => Error == null;
    public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
}