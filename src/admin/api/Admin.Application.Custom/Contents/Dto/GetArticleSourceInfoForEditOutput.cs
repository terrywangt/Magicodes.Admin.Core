using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;

namespace Admin.Application.Custom.Contents.Dto
{
    /// <summary>
    ///  文章来源���༭���ģ��
    /// </summary>
    public class GetArticleSourceInfoForEditOutput
    {
        public ArticleSourceInfoEditDto ArticleSourceInfo { get; set; }
    }
}