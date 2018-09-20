using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Magicodes.Admin.Configuration.Storage.Dto;

namespace Magicodes.Admin.Configuration.Storage
{
    public interface IStorageSettingAppService
    {
        Task<StorageSettingEditDto> GetAllSettings();

        Task UpdateAllSettings(StorageSettingEditDto input);
    }
}
