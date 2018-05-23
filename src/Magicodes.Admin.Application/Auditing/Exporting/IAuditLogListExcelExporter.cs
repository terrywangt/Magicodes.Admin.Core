using System.Collections.Generic;
using Magicodes.Admin.Auditing.Dto;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Auditing.Exporting
{
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);

        FileDto ExportToFile(List<EntityChangeListDto> entityChangeListDtos);
    }
}
