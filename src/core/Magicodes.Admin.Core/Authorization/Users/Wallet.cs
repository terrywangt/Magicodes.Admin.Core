using Microsoft.EntityFrameworkCore;

namespace Magicodes.Admin.Authorization.Users
{
    /// <summary>
    /// 钱包
    /// </summary>
    [Owned]
    public class Wallet
    {
        /// <summary>
        /// 余额（以分为单位）
        /// </summary>
        public int Balance { get; set; }

        /// <summary>
        /// 冻结金额（以分为单位）
        /// </summary>
        public int FrozenAmount { get; set; }
    }
}