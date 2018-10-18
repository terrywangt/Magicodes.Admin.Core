using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}
