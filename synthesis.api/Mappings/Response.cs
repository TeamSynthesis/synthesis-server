namespace synthesis.api.Mappings;

public class Response<T>
{
  public Response
    (bool isSuccess, string? message = null, T? value = default, List<string>? errors = null)
  {
    if (isSuccess && errors != null || !isSuccess && errors == null)
    {
      throw new ArgumentException("invalid response");
    };

    IsSuccess = isSuccess;
    Message = message;
    Value = value;
    Errors = errors;
  }


  public bool IsSuccess { get; set; }
  public string? Message { get; set; }
  public T? Value { get; set; }
  public List<string>? Errors { get; set; }
}