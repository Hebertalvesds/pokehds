namespace api.ApiModel
{
    public class Response
    {
        public Pokemon Pokemon { get; set; }
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
