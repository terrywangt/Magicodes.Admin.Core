using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Magicodes.AppSession
{
    public interface IAppSessionManager : ISingletonDependency
    {
        void RegisterFuncForGetUserInfo(RequestTypes requestType, Func<IAppSessionInfo, object> func);

        Func<IAppSessionInfo, object> GetUserInfoFunc(RequestTypes requestType);

        void RegisterFuncForGetUserExtendInfo(RequestTypes requestType, Func<IAppSessionInfo, object> func);

        Func<IAppSessionInfo, object> GetUserInfoExtendFunc(RequestTypes requestType);
    }
}
