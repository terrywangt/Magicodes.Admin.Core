using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Magicodes.Admin.Core.Custom.Contents;

namespace Admin.Application.Custom.Contents.Dto
{
    /// <summary>
    ///  栏目创建或者编辑Dto
    /// </summary>
    public partial class CreateOrUpdateColumnInfoDto
    {
        [Required]
        public ColumnInfoEditDto ColumnInfo { get; set; }
    }
}