namespace Magicodes.App.Application.Users.Dto
{
    public class ResetPasswordOutput
    {
        /// <summary>
        /// 访问Token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public int ExpireInSeconds { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
