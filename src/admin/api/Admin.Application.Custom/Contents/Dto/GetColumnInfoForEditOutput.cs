using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;

namespace Admin.Application.Custom.Contents.Dto
{
    /// <summary>
    ///  栏目���༭���ģ��
    /// </summary>
    public class GetColumnInfoForEditOutput
    {
        public ColumnInfoEditDto ColumnInfo { get; set; }
    }
}