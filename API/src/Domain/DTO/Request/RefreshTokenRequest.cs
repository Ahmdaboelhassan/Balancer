namespace Domain.DTO.Request
{
    public record RefreshTokenRequest
    {
        public required string RefreshToken { get; set; }
    }
}
