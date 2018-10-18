using System.Collections.Generic;
using Magicodes.Admin.Chat.Dto;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Chat.Exporting
{
    public interface IChatMessageListExcelExporter
    {
        FileDto ExportToFile(List<ChatMessageExportDto> messages);
    }
}
