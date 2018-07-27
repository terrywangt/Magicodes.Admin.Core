using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Magicodes.Admin.Core.Custom.Contents;

namespace Admin.Application.Custom.Contents.Dto
{
    /// <summary>
    ///  创建或者编辑Dto
    /// </summary>
    public partial class CreateOrUpdateArticleInfoArticleTagInfoDto
    {
        [Required]
        public ArticleTagInfoEditDto ArticleTagInfo { get; set; }
    }
}