using Abp;
using Abp.Dependency;

namespace Magicodes.AppSession
{
    /// <summary>
    /// APP Session
    /// </summary>
    public interface IAppSession : IShouldInitialize, ITransientDependency
    {
        IAppSessionInfo AppSessionInfo { get; set; }
        /// <summary>
        /// 获取用户信息，需要在AppSessionManager中注册
        /// WeChat:获取粉丝信息
        /// Wap：获取用户信息
        /// </summary>
        /// <returns></returns>
        T GetUserInfo<T>() where T : class;

        /// <summary>
        /// 获取扩展信息
        /// Wap：null
        /// WeChat：获取用户信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetUserExtendInfo<T>() where T : class;

        /// <summary>
        /// 设置值
        /// </summary>
        void SetAppSession(IAppSessionInfo appSessionInfo);
    }
}
