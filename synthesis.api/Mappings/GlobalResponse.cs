namespace synthesis.api.Mappings;

public class GlobalResponse<T>
{
    public GlobalResponse
      (bool isSuccess, string? message = null, T? data = default, List<string>? errors = null)
    {
        if (isSuccess && errors != null || !isSuccess && errors == null)
        {
            throw new ArgumentException("invalid response");
        };

        IsSuccess = isSuccess;
        Message = message;
        Data = data;
        Errors = errors;
    }


    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}