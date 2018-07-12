using System.Threading.Tasks;
using Abp.Dependency;
using Castle.Core.Logging;

namespace Magicodes.Admin.Identity
{
    public class SmsSender : ISmsSender, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public SmsSender()
        {
            Logger = NullLogger.Instance;
        }

        public Task SendAsync(string number, string message)
        {
            /* Implement this service to send SMS to users (can be used for two factor auth). */

            Logger.Warn("Sending SMS is not implemented! Message content:");
            Logger.Warn("Number  : " + number);
            Logger.Warn("Message : " + message);

            return Task.FromResult(0);
        }
    }
}
