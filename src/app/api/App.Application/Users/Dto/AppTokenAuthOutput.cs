namespace Magicodes.App.Application.Users.Dto
{
    /// <summary>
    ///     授权访问 输出参数
    /// </summary>
    public class AppTokenAuthOutput
    {
        /// <summary>
        ///     用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///     手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        ///     访问AccessToken
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        ///     过期时间
        /// </summary>
        public int ExpireInSeconds { get; set; }
    }
}