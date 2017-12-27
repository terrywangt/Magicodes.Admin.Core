using System.Diagnostics;

namespace Magicodes.Admin.Core.Exception
{
    public static class ExceptionHandler
    {
        /// <summary>
        /// Catch unhandled exceptions. You can use -HockeyApp Crash Reporting- service to send your errors.
        /// http://hockeyapp.net
        /// </summary>
        /// <param name="exception"></param>
        public static void LogException(System.Exception exception)
        {
            try
            {
                if (exception == null)
                {
                    return;
                }

                var exceptionMsg = exception.ToString();
                Debug.WriteLine(exceptionMsg, "* Unhandled Exception *");
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }
    }
}