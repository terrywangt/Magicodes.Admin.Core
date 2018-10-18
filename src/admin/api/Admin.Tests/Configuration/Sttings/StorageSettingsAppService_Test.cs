using Abp.Configuration;
using Abp.Extensions;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Configuration.Storage;
using Magicodes.Storage.AliyunOss.Core;
using Magicodes.Storage.Core;
using Magicodes.Storage.Local.Core;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using Xunit;

namespace Magicodes.Admin.Tests.Configuration.Sttings
{
    public class StorageSettingsAppService_Test:AppTestBase
    {
        private readonly IStorageSettingAppService _storageSettingAppService;

        private readonly ISettingManager _settingManager;

        public IStorageProvider StorageProvider { get; set; }

        public StorageSettingsAppService_Test ()
        {
            _storageSettingAppService = Resolve<StorageSettingAppService>();
            _settingManager = Resolve<SettingManager>();
        }

        [Display(Name = "获取配置信息")]
        [Fact]
        public void GetAllSettings()
        {
           var query =  _storageSettingAppService.GetAllSettings();
           var s = query.Result;
           Assert.NotNull(query);
        }

    }
}
