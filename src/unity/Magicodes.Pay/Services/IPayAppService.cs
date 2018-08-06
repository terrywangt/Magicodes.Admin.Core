using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Pay.Services.Dto;

namespace Magicodes.Pay.Services
{

    public interface IPayAppService : IApplicationService
    {
        Task<string> AliAppPay(AppPayInput input);
        Task<WeChat.Pay.Dto.AppPayOutput> WeChatAppPay(AppPayInput input);
    }
}