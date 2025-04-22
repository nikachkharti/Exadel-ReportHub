namespace ReportHub.Application.Common.Models
{
    public sealed class EndpointResponse
    {
        public string Message { get; set; }
        public object Result { get; set; }
        public bool IsSuccess { get; set; }
        public int HttpStatusCode { get; set; }

        public EndpointResponse(object result, string message, bool isSuccess, int httpStatusCode)
        {
            Message = message;
            Result = result;
            IsSuccess = isSuccess;
            HttpStatusCode = httpStatusCode;
        }

        public EndpointResponse()
        {
        }
    }

    public static class EndpointMessage
    {
        public static string successMessage = "Request completed successfully";
    };

}
