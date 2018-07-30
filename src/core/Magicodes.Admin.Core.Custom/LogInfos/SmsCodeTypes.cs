using System.ComponentModel;

namespace Magicodes.Admin.Core.Custom.LogInfos
{
    [Description("用户状态")]
    public enum SmsCodeTypes
    {
        /// <summary>
        /// 登陆
        /// </summary>
        [Description("登陆")]
        Login = 0,
        /// <summary>
        /// 绑定手机
        /// </summary>
        [Description("绑定手机")]
        BindPhone = 1,
        /// <summary>
        /// 注册
        /// </summary>
        [Description("注册")]
        Register = 2
    }
}