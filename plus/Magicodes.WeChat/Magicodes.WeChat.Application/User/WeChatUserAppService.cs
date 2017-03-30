using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Magicodes.Admin.Auditing.Dto;
using Magicodes.Admin.Auditing.Exporting;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Dto;
using Magicodes.Admin;
using Magicodes.WeChat.Application.User.Dto;
using System;
using Magicodes.WeChat.Core.User;
using Magicodes.WeChat.Core.Authorization;
using System.Diagnostics;
using Abp.BackgroundJobs;
using Abp.Runtime.Session;
using Magicodes.WeChat.Application.BackgroundJob;

namespace Magicodes.WeChat.Application.User
{
    [AbpAuthorize(WeChatPermissions.WeChatPermissions_Pages_Tenants_WeChatUsers)]
    public class WeChatUserAppService : AdminAppServiceBase, IWeChatUserAppService
    {
        private readonly IRepository<WeChatUser, string> _wechatUserRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;
        public WeChatUserAppService(IRepository<WeChatUser, string> wechatUserRepository, IBackgroundJobManager backgroundJobManager)
        {
            _wechatUserRepository = wechatUserRepository;
            _backgroundJobManager = backgroundJobManager;

        }

        public async Task<PagedResultDto<WeChatUserListDto>> GetWeChatUsers(GetWeChatUsersInput input)
        {
            var query = CreateWeChatUsersQuery(input);

            var resultCount = await query.CountAsync();
            var results = await query
                .AsNoTracking()
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<WeChatUserListDto>(resultCount, results.MapTo<List<WeChatUserListDto>>());
        }

        
        public async Task Sync()
        {
            await _backgroundJobManager.EnqueueAsync<SyncWeChatUsersJob, int>(AbpSession.TenantId.HasValue ? AbpSession.TenantId.Value : 1);
        }

        [AbpAuthorize(WeChatPermissions.WeChatPermissions_Pages_Tenants_WeChatUsers_Create)]
        protected virtual async Task CreateWeChatUserAsync(CreateOrUpdateWeChatUserInput input)
        {
            var weChatUser = input.WeChatUser.MapTo<WeChatUser>();
            await _wechatUserRepository.InsertAsync(weChatUser);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [AbpAuthorize(WeChatPermissions.WeChatPermissions_Pages_Tenants_WeChatUsers_Edit)]
        protected virtual async Task EditWeChatUserAsync(CreateOrUpdateWeChatUserInput input)
        {
            var weChatUser = input.WeChatUser.MapTo<WeChatUser>();
            await _wechatUserRepository.UpdateAsync(weChatUser);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [AbpAuthorize(WeChatPermissions.WeChatPermissions_Pages_Tenants_WeChatUsers_Create, WeChatPermissions.WeChatPermissions_Pages_Tenants_WeChatUsers_Edit)]
        public async Task<GetWeChatUserForEditOutput> GetWeChatUserForEdit(IdDto input)
        {
            WeChatUserEditDto editDto;
            if (!string.IsNullOrEmpty(input.Id)) //Editing existing edition?
            {
                var info = await _wechatUserRepository.GetAsync(input.Id);
                editDto = info.MapTo<WeChatUserEditDto>();
            }
            else
            {
                editDto = new WeChatUserEditDto();

            }
            return new GetWeChatUserForEditOutput
            {
                WeChatUser = editDto
            };
        }

        //public async Task CreateOrUpdateWeChatUser(CreateOrUpdateWeChatUserInput input)
        //{
        //    //if (!string.IsNullOrEmpty(input.WeChatUser.Id))
        //    //{
        //    //    await up(input);
        //    //}
        //    //else
        //    //{
        //    //    await CreateRoleAsync(input);
        //    //}
        //}

        //[AbpAuthorize(WeChatPermissions.WeChatPermissions_Pages_Tenants_WeChatUsers_Delete)]
        //public async Task DeleteRole(IdDto input)
        //{
        //    await _wechatUserRepository.DeleteAsync(input.Id);
        //}

        //[AbpAuthorize(WeChatPermissions.WeChatPermissions_Pages_Tenants_WeChatUsers_Edit)]
        //protected virtual async Task UpdateRoleAsync(CreateOrUpdateWeChatUserInput input)
        //{
        //    Debug.Assert(input.WeChatUser.Id != null, "input.WeChatUser.Id should be set.");

        //    var model = await _wechatUserRepository.FirstOrDefaultAsync(input.WeChatUser.Id);
        //   //_wechatUserRepository.UpdateAsync()

        //    //await UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);
        //}

        //public async Task<FileDto> GetAuditLogsToExcel(GetAuditLogsInput input)
        //{
        //    var auditLogs = await CreateAuditLogAndUsersQuery(input)
        //                .AsNoTracking()
        //                .OrderByDescending(al => al.AuditLog.ExecutionTime)
        //                .ToListAsync();

        //    var auditLogListDtos = ConvertToAuditLogListDtos(auditLogs);

        //    return _auditLogListExcelExporter.ExportToFile(auditLogListDtos);
        //}


        public Task<FileDto> GetWeChatUsersToExcel(GetWeChatUsersInput input)
        {
            throw new NotImplementedException();
        }



        private IQueryable<WeChatUser> CreateWeChatUsersQuery(GetWeChatUsersInput input)
        {
            var query = _wechatUserRepository.GetAll().WhereIf(
                !input.Filter.IsNullOrEmpty(),
                p => p.NickName.Contains(input.Filter) ||
                        p.City.Contains(input.Filter) ||
                        p.Province.Contains(input.Filter)
                );

            //query = query
            //    .WhereIf(!input.UserName.IsNullOrWhiteSpace(), item => item.User.UserName.Contains(input.UserName))
            //    .WhereIf(!input.ServiceName.IsNullOrWhiteSpace(), item => item.AuditLog.ServiceName.Contains(input.ServiceName))
            //    .WhereIf(!input.MethodName.IsNullOrWhiteSpace(), item => item.AuditLog.MethodName.Contains(input.MethodName))
            //    .WhereIf(!input.BrowserInfo.IsNullOrWhiteSpace(), item => item.AuditLog.BrowserInfo.Contains(input.BrowserInfo))
            //    .WhereIf(input.MinExecutionDuration.HasValue && input.MinExecutionDuration > 0, item => item.AuditLog.ExecutionDuration >= input.MinExecutionDuration.Value)
            //    .WhereIf(input.MaxExecutionDuration.HasValue && input.MaxExecutionDuration < int.MaxValue, item => item.AuditLog.ExecutionDuration <= input.MaxExecutionDuration.Value)
            //    .WhereIf(input.HasException == true, item => item.AuditLog.Exception != null && item.AuditLog.Exception != "")
            //    .WhereIf(input.HasException == false, item => item.AuditLog.Exception == null || item.AuditLog.Exception == "");
            return query;
        }
    }
}
