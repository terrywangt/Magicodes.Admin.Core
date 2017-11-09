using System.Collections.Generic;
using Magicodes.Admin.Caching.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Maintenance
{
    public class MaintenanceViewModel
    {
        public IReadOnlyList<CacheDto> Caches { get; set; }
    }
}