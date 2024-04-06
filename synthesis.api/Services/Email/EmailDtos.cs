namespace synthesis.api.Services.Email;

public record ConfirmEmailRecepientDto
{
    public string? Email { get; set; }
    public string? Link { get; set; }
}

public record InviteRecepientDto
{
    public string? Email { get; set; }
    public string? Code { get; set; }

}