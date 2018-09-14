// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : ResetPasswordInput.cs
//           description :
//   
//           created by 雪雁 at  2018-09-13 10:58
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Web;
using Abp.Auditing;
using Abp.Runtime.Security;

namespace Magicodes.App.Application.Users.Dto
{
    public class ResetPasswordInput
    {
        public long UserId { get; set; }

        public string ResetCode { get; set; }

        [DisableAuditing] public string Password { get; set; }

        /// <summary>
        ///     Encrypted values for {TenantId}, {UserId} and {ResetCode}
        /// </summary>
        public string c { get; set; }

        public void Normalize()
        {
            ResolveParameters();
        }

        protected virtual void ResolveParameters()
        {
            if (!string.IsNullOrEmpty(c))
            {
                var parameters = SimpleStringCipher.Instance.Decrypt(c);
                var query = HttpUtility.ParseQueryString(parameters);

                if (query["userId"] != null) UserId = Convert.ToInt32(query["userId"]);

                if (query["resetCode"] != null) ResetCode = query["resetCode"];
            }
        }
    }
}