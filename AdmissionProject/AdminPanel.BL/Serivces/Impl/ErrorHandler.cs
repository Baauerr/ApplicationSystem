using EasyNetQ.SystemMessages;

namespace AdminPanel.BL.Serivces.Impl
{
    public class ErrorMessageHandler
    {
        public Exception HandleErrorMessage(Error exception)
        {
            throw new Exception(exception.Message);
        }
    }
}
