using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;

namespace Admin.Application.Custom.Contents.Dto
{
    /// <summary>
    ///  文章���༭���ģ��
    /// </summary>
    public class GetArticleInfoForEditOutput
    {
        public ArticleInfoEditDto ArticleInfo { get; set; }
    }
}