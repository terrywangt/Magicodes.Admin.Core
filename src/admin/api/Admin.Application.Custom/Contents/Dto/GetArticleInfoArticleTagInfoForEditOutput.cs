using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;

namespace Admin.Application.Custom.Contents.Dto
{
    /// <summary>
    ///  ���༭���ģ��
    /// </summary>
    public class GetArticleTagInfoForEditOutput
    {
        public ArticleTagInfoEditDto ArticleTagInfo { get; set; }
    }
}