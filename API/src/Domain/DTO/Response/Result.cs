namespace Domain.DTO.Response
{
    public record Result<T>
    {
        public bool IsSucceed { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
