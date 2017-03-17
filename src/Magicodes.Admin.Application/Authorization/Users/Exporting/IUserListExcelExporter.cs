using System.Collections.Generic;
using Magicodes.Admin.Authorization.Users.Dto;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos);
    }
}