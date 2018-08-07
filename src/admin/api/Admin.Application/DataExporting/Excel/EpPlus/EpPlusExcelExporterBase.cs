using System;
using System.Collections.Generic;
using Abp.AspNetZeroCore.Net;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Magicodes.Admin.Dto;
using OfficeOpenXml;

namespace Magicodes.Admin.DataExporting.Excel.EpPlus
{
    public abstract class EpPlusExcelExporterBase : AdminServiceBase, ITransientDependency
    {
        public IAppFolders AppFolders { get; set; }
        private readonly ICacheManager _cacheManager;

        protected EpPlusExcelExporterBase(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        protected FileDto CreateExcelPackage(string fileName, Action<ExcelPackage> creator)
        {
            var file = new FileDto(fileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);

            using (var excelPackage = new ExcelPackage())
            {
                creator(excelPackage);
                Save(excelPackage, file);
            }

            return file;
        }

        protected void AddHeader(ExcelWorksheet sheet, params string[] headerTexts)
        {
            if (headerTexts.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < headerTexts.Length; i++)
            {
                AddHeader(sheet, i + 1, headerTexts[i]);
            }
        }

        protected void AddHeader(ExcelWorksheet sheet, int columnIndex, string headerText)
        {
            sheet.Cells[1, columnIndex].Value = headerText;
            sheet.Cells[1, columnIndex].Style.Font.Bold = true;
        }

        protected void AddObjects<T>(ExcelWorksheet sheet, int startRowIndex, IList<T> items, params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                for (var j = 0; j < propertySelectors.Length; j++)
                {
                    sheet.Cells[i + startRowIndex, j + 1].Value = propertySelectors[j](items[i]);
                }
            }
        }

        protected void Save(ExcelPackage excelPackage, FileDto file)
        {
            _cacheManager.GetCache(AppConsts.ExcelFileCacheName).Set(file.FileName, excelPackage.GetAsByteArray());
        }
    }
}