namespace api.ApiModel
{
    public class Response
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public object Message { get; set; }
    }
}
