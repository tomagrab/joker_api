namespace joker_api.Models.Entities;

public class StonlyErrorResponseEntity
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
}