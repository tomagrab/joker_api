namespace joker_api.Models.Common;

public class CommonApiResponseModel<T>
{
    public CommonApiResponseModel()
    {
    }

    public CommonApiResponseModel(bool success, string? message = null, T? data = default)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}