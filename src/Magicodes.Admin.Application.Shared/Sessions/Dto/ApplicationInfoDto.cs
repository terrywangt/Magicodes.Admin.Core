using System;
using System.Collections.Generic;

namespace Magicodes.Admin.Sessions.Dto
{
    public class ApplicationInfoDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 应用程序版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>

        public DateTime ReleaseDate { get; set; }

        public Dictionary<string, bool> Features { get; set; }
    }
}