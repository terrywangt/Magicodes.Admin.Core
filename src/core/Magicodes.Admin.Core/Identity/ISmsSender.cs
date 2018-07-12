using System.Threading.Tasks;

namespace Magicodes.Admin.Identity
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);
    }
}