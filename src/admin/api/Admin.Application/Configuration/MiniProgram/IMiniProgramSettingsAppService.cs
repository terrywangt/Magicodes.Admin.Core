using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Magicodes.Admin.Configuration.MiniProgram.Dto;
using System.Threading.Tasks;

namespace Magicodes.Admin.Configuration.MiniProgram
{
    public interface IMiniProgramSettingsAppService : IApplicationService
    {
        Task<MiniProgramSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(MiniProgramSettingsEditDto input);
    }
}
