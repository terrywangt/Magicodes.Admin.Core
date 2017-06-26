using System;
using System.Collections.Generic;

namespace Magicodes.AppSession
{
    public class AppSessionManager : IAppSessionManager
    {
        public Dictionary<RequestTypes, Func<IAppSessionInfo, object>> GetUserInfoFuncs = new Dictionary<RequestTypes, Func<IAppSessionInfo, object>>();

        public Dictionary<RequestTypes, Func<IAppSessionInfo, object>> GetUserExtendInfoFuncs = new Dictionary<RequestTypes, Func<IAppSessionInfo, object>>();

        public void RegisterFuncForGetUserInfo(RequestTypes requestType, Func<IAppSessionInfo, object> func)
        {
            if (GetUserInfoFuncs.ContainsKey(requestType))
            {
                throw new Exception("当前key已存在：" + requestType);
            }
            GetUserInfoFuncs.Add(requestType, func);
        }

        public Func<IAppSessionInfo, object> GetUserInfoFunc(RequestTypes requestType)
        {
            return GetUserInfoFuncs.ContainsKey(requestType) ? GetUserInfoFuncs[requestType] : (GetUserInfoFuncs.ContainsKey(RequestTypes.Default) ? GetUserInfoFuncs[RequestTypes.Default] : null);
        }

        public void RegisterFuncForGetUserExtendInfo(RequestTypes requestType, Func<IAppSessionInfo, object> func)
        {
            if (GetUserExtendInfoFuncs.ContainsKey(requestType))
            {
                throw new Exception("当前key已存在：" + requestType);
            }
            GetUserExtendInfoFuncs.Add(requestType, func);
        }

        public Func<IAppSessionInfo, object> GetUserInfoExtendFunc(RequestTypes requestType)
        {
            return GetUserExtendInfoFuncs.ContainsKey(requestType) ? GetUserExtendInfoFuncs[requestType] : (GetUserExtendInfoFuncs.ContainsKey(RequestTypes.Default) ? GetUserExtendInfoFuncs[RequestTypes.Default] : null);
        }

    }
}