using Abp.Application.Services.Dto;
using Abp.AspNetZeroCore.Net;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using Admin.Application.Custom.LogInfos.Dto;
using Admin.Application.Custome.LogInfos.Dto;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Core.Custom.LogInfos;
using Magicodes.Admin.Dto;
using Magicodes.ExporterAndImporter.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Magicodes.Admin.Storage;

namespace Admin.Application.Custom.LogInfos
{
    /// <summary>
    /// 交易日志
    /// </summary>
    [AbpAuthorize(AppPermissions.Pages_TransactionLog)]
    public partial class TransactionLogAppService : AppServiceBase, ITransactionLogAppService
    {
        private readonly IRepository<TransactionLog, long> _transactionLogRepository;
        private readonly IExporter _excelExporter;
        private readonly ISettingManager _settingManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;

        /// <summary>
        /// 
        /// </summary>
        public TransactionLogAppService(
            IRepository<TransactionLog, long> transactionLogRepository
            , ISettingManager settingManager
            , IExporter excelExporter, ITempFileCacheManager tempFileCacheManager) : base()
        {
            _transactionLogRepository = transactionLogRepository;
            _settingManager = settingManager;
            _excelExporter = excelExporter;
            _tempFileCacheManager = tempFileCacheManager;
        }

        /// <summary>
        /// 获取交易日志列表
        /// </summary>
        public async Task<PagedResultDto<TransactionLogListDto>> GetTransactionLogs(GetTransactionLogsInput input)
        {
            async Task<PagedResultDto<TransactionLogListDto>> getListFunc(bool isLoadSoftDeleteData)
            {
                var query = CreateTransactionLogsQuery(input);
                
				
				var resultCount = await query.CountAsync();
                var results = await query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .Select(i => new TransactionLogListDto
                    {
                        Id = i.Id,
                        ClientName = i.ClientName,
                        ClientIpAddress = i.ClientIpAddress,
                        CustomData = i.CustomData,
                        CreationTime = i.CreationTime,
                        CultureValue = i.Currency.ToString(),
                        PayChannel = i.PayChannel,
                        Exception = i.Exception,
                        IsFreeze = i.IsFreeze,
                        OutTradeNo = i.OutTradeNo,
                        PayTime = i.PayTime,
                        Terminal = i.Terminal,
                        TransactionState = i.TransactionState
                    })
                    .ToListAsync();

                return new PagedResultDto<TransactionLogListDto>(resultCount, results);
            }

            //是否仅加载回收站数据
            if (input.IsOnlyGetRecycleData)
            {
                using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    return await getListFunc(true);
                }
            }
            return await getListFunc(false);
        }

		/// <summary>
		/// 导出交易日志
		/// </summary>
		public async Task<FileDto> GetTransactionLogsToExcel(GetTransactionLogsInput input)
        {
            async Task<List<TransactionLogExportDto>> getListFunc(bool isLoadSoftDeleteData)
            {
                var query = CreateTransactionLogsQuery(input);
                var results = await query
                    .OrderBy(input.Sorting)
                    .ToListAsync();

                var exportListDtos = results.MapTo<List<TransactionLogExportDto>>();
                if (exportListDtos.Count == 0)
                    throw new UserFriendlyException(L("NoDataToExport"));
                return exportListDtos;
            }

            List<TransactionLogExportDto> exportData = null;

			
            exportData = await getListFunc(false);
            var fileDto = new FileDto(L("TransactionLog") + L("ExportData") + ".xlsx", MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
            var byteArray = await _excelExporter.ExportAsByteArray(exportData);
            _tempFileCacheManager.SetFile(fileDto.FileToken, byteArray);
            return fileDto;
        }

		/// <summary>
		/// 
		/// </summary>
        private IQueryable<TransactionLog> CreateTransactionLogsQuery(GetTransactionLogsInput input)
        {
            var query = _transactionLogRepository.GetAllIncluding(p => p.Currency);
			
			//关键字搜索
			query = query
					.WhereIf(
                    !input.Filter.IsNullOrEmpty(),
					p => p.Name.Contains(input.Filter) || p.ClientIpAddress.Contains(input.Filter) || p.ClientName.Contains(input.Filter) || p.CustomData.Contains(input.Filter) || p.OutTradeNo.Contains(input.Filter) || p.TransactionId.Contains(input.Filter) || p.Exception.Contains(input.Filter));
			
			
			//创建时间范围搜索
			query = query
                .WhereIf(input.CreationDateStart.HasValue, t => t.CreationTime >= input.CreationDateStart.Value)
                .WhereIf(input.CreationDateEnd.HasValue, t => t.CreationTime <= input.CreationDateEnd.Value);
			
			
            return query;
        }

		/// <summary>
		/// 获取交易日志
		/// </summary>
        [AbpAuthorize(AppPermissions.Pages_TransactionLog_Create, AppPermissions.Pages_TransactionLog_Edit)]
        public async Task<GetTransactionLogForEditOutput> GetTransactionLogForEdit(NullableIdDto<long> input)
        {
            TransactionLogEditDto editDto;
            if (input.Id.HasValue)
            {
                var info = await _transactionLogRepository.GetAsync(input.Id.Value);
                editDto = info.MapTo<TransactionLogEditDto>();
            }
            else
            {
                editDto = new TransactionLogEditDto();

            }
            return new GetTransactionLogForEditOutput
            {
                TransactionLog = editDto
            };
        }

		/// <summary>
		/// 创建或者编辑交易日志
		/// </summary>
        [AbpAuthorize(AppPermissions.Pages_TransactionLog_Create, AppPermissions.Pages_TransactionLog_Edit)]
        public async Task CreateOrUpdateTransactionLog(CreateOrUpdateTransactionLogDto input)
        {
            if (!input.TransactionLog.Id.HasValue)
            {
                await CreateTransactionLogAsync(input);
            }
            else
            {
                await UpdateTransactionLogAsync(input);
            }
        }

		/// <summary>
		/// 删除交易日志
		/// </summary>
        [AbpAuthorize(AppPermissions.Pages_TransactionLog_Delete)]
        public async Task DeleteTransactionLog(EntityDto<long> input)
        {
            var transactionLog = await _transactionLogRepository.GetAsync(input.Id);
            await _transactionLogRepository.DeleteAsync(transactionLog);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_TransactionLog_Create)]
        protected virtual async Task CreateTransactionLogAsync(CreateOrUpdateTransactionLogDto input)
        {
            var CurrencyName = _settingManager.GetSettingValueAsync("Abp.Localization.DefaultLanguageName").Result;
            var transactionLog = new TransactionLog()
            {
                Currency = new Currency(CurrencyName, input.TransactionLog.Amount),
                ClientIpAddress = input.TransactionLog.ClientIpAddress,
                ClientName = input.TransactionLog.ClientName,
                IsFreeze = input.TransactionLog.IsFreeze,
                PayChannel = input.TransactionLog.PayChannel,
                Terminal = input.TransactionLog.Terminal,
                TransactionState = input.TransactionLog.TransactionState,
                CustomData = input.TransactionLog.CustomData,
                OutTradeNo = input.TransactionLog.OutTradeNo,
                PayTime = input.TransactionLog.PayTime,
                Exception = input.TransactionLog.Exception,
                CreatorUserId = AbpSession.UserId,
                CreationTime = Clock.Now,
                TenantId = AbpSession.TenantId
            };
            await _transactionLogRepository.InsertAsync(transactionLog);

        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_TransactionLog_Edit)]
        protected virtual async Task UpdateTransactionLogAsync(CreateOrUpdateTransactionLogDto input)
        {
            var CurrencyName = _settingManager.GetSettingValueAsync("Abp.Localization.DefaultLanguageName").Result;
            Debug.Assert(input.TransactionLog.Id != null, "必须设置input.TransactionLog.Id的值");

            var transactionLog = await _transactionLogRepository.GetAsync(input.TransactionLog.Id.Value);
            transactionLog.Currency = new Currency(CurrencyName, input.TransactionLog.Amount);
            transactionLog.ClientIpAddress = input.TransactionLog.ClientIpAddress;
            transactionLog.ClientName = input.TransactionLog.ClientName;
            transactionLog.IsFreeze = input.TransactionLog.IsFreeze;
            transactionLog.PayChannel = input.TransactionLog.PayChannel;
            transactionLog.Terminal = input.TransactionLog.Terminal;
            transactionLog.TransactionState = input.TransactionLog.TransactionState;
            transactionLog.CustomData = input.TransactionLog.CustomData;
            transactionLog.OutTradeNo = input.TransactionLog.OutTradeNo;
            transactionLog.PayTime = input.TransactionLog.PayTime;
            transactionLog.Exception = input.TransactionLog.Exception;
        }



        ///// <summary>
        ///// 获取选择列表
        ///// </summary>
        //public async Task<List<GetDataComboItemDto<long>>> GetCurrencyDataComboItems()
        //{
        //    var list = await _transactionLogRepository.GetAll()
        //    //.Where(p => !p.IsActive)
        //    .OrderByDescending(p => p.Id)
        //    .Select(p => new { p.Id, p.Currency.CultureName }).ToListAsync();
        //    return list.Select(p => new GetDataComboItemDto<long>()
        //    {
        //        DisplayName = p.CultureName,
        //        Value = p.Id
        //    }).ToList();
        //}

		/// <summary>
        /// IsFreeze开关服务
        /// </summary>
        /// <param name="input">开关输入参数</param>
        /// <returns></returns>
		[AbpAuthorize(AppPermissions.Pages_TransactionLog_Edit)]
        public async Task UpdateIsFreezeSwitchAsync(SwitchEntityInputDto<long> input)
		{
            var transactionLog = await _transactionLogRepository.GetAsync(input.Id);
			transactionLog.IsFreeze = input.SwitchValue;
		}


    }
}