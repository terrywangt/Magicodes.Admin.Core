using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.Common.Dto
{
    public class SetCoverInputDto
    {
        /// <summary>
        /// 对象类型
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// 对象Id
        /// </summary>
        public long ObjectId { get; set; }

        /// <summary>
        /// 附件Url
        /// </summary>
        public string AttachmentUrl { get; set; }
    }
}
