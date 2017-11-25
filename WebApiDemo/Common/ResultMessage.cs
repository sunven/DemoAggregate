namespace WebApiDemo.Common
{
    public class ResultMessage
    {
        public ResultMessage(ResultState state, string message)
        {
            State = state;
            Message = message;
        }

        public ResultMessage(ResultState state, string message, object data)
        {
            State = state;
            Message = message;
            Data = data;
        }

        public ResultState State { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }

    public enum ResultState
    {
        Fail = 0,
        Success = 1
    }
}