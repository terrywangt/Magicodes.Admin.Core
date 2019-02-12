using System.Collections.Generic;
using Magicodes.Admin.Contents;
using Magicodes.Admin.Dto;

namespace Cms.Host.Common
{
    public static class NavHelper
    {
        public static ICollection<TreeTableRowDto<ColumnInfo>> HeaderNav { get; set; }

        public static ICollection<TreeTableRowDto<ColumnInfo>> FooterNav { get; set; }
    }
}