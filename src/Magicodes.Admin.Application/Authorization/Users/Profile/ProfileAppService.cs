using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Abp;
using Abp.Auditing;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Extensions;
using Abp.IO;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Abp.Zero.Configuration;
using Magicodes.Admin.Authorization.Users.Dto;
using Magicodes.Admin.Authorization.Users.Profile.Dto;
using Magicodes.Admin.Friendships;
using Magicodes.Admin.Security;
using Magicodes.Admin.Storage;
using Magicodes.Admin.Timing;

namespace Magicodes.Admin.Authorization.Users.Profile
{
    [AbpAuthorize]
    public class ProfileAppService : AdminAppServiceBase, IProfileAppService
    {
        private readonly IAppFolders _appFolders;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IFriendshipManager _friendshipManager;

        public ProfileAppService(
            IAppFolders appFolders,
            IBinaryObjectManager binaryObjectManager,
            ITimeZoneService timezoneService,
            IFriendshipManager friendshipManager)
        {
            _appFolders = appFolders;
            _binaryObjectManager = binaryObjectManager;
            _timeZoneService = timezoneService;
            _friendshipManager = friendshipManager;
        }

        [DisableAuditing]
        public async Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit()
        {
            var user = await GetCurrentUserAsync();
            var userProfileEditDto = ObjectMapper.Map<CurrentUserProfileEditDto>(user);

            if (Clock.SupportsMultipleTimezone)
            {
                userProfileEditDto.Timezone = await SettingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);

                var defaultTimeZoneId = await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, AbpSession.TenantId);
                if (userProfileEditDto.Timezone == defaultTimeZoneId)
                {
                    userProfileEditDto.Timezone = string.Empty;
                }
            }

            return userProfileEditDto;
        }

        public async Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input)
        {
            var user = await GetCurrentUserAsync();
            ObjectMapper.Map(input, user);
            CheckErrors(await UserManager.UpdateAsync(user));

            if (Clock.SupportsMultipleTimezone)
            {
                if (input.Timezone.IsNullOrEmpty())
                {
                    var defaultValue = await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, AbpSession.TenantId);
                    await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), TimingSettingNames.TimeZone, defaultValue);
                }
                else
                {
                    await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), TimingSettingNames.TimeZone, input.Timezone);
                }
            }
        }

        public async Task ChangePassword(ChangePasswordInput input)
        {
            await UserManager.InitializeOptionsAsync(AbpSession.TenantId);

            var user = await GetCurrentUserAsync();
            CheckErrors(await UserManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword));
        }

        public async Task UpdateProfilePicture(UpdateProfilePictureInput input)
        {
            var tempProfilePicturePath = Path.Combine(_appFolders.TempFileDownloadFolder, input.FileName);

            byte[] byteArray;

            using (var fsTempProfilePicture = new FileStream(tempProfilePicturePath, FileMode.Open))
            {
                using (var bmpImage = new Bitmap(fsTempProfilePicture))
                {
                    var width = input.Width == 0 ? bmpImage.Width : input.Width;
                    var height = input.Height == 0 ? bmpImage.Height : input.Height;
                    var bmCrop = bmpImage.Clone(new Rectangle(input.X, input.Y, width, height), bmpImage.PixelFormat);

                    using (var stream = new MemoryStream())
                    {
                        bmCrop.Save(stream, bmpImage.RawFormat);
                        byteArray = stream.ToArray();
                    }
                }
            }

            if (byteArray.Length > 102400) //100 KB
            {
                throw new UserFriendlyException(L("ResizedProfilePicture_Warn_SizeLimit"));
            }

            var user = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            if (user.ProfilePictureId.HasValue)
            {
                await _binaryObjectManager.DeleteAsync(user.ProfilePictureId.Value);
            }

            var storedFile = new BinaryObject(AbpSession.TenantId, byteArray);
            await _binaryObjectManager.SaveAsync(storedFile);

            user.ProfilePictureId = storedFile.Id;

            FileHelper.DeleteIfExists(tempProfilePicturePath);
        }

        [AbpAllowAnonymous]
        public async Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting()
        {
            var passwordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit),
                RequireLowercase = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase),
                RequiredLength = await SettingManager.GetSettingValueAsync<int>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength)
            };

            return new GetPasswordComplexitySettingOutput
            {
                Setting = passwordComplexitySetting
            };
        }

        [DisableAuditing]
        public async Task<GetProfilePictureOutput> GetProfilePicture()
        {
            var user = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            if (user.ProfilePictureId == null)
            {
                return new GetProfilePictureOutput(string.Empty);
            }

            return await GetProfilePictureById(user.ProfilePictureId.Value);
        }

        public async Task<GetProfilePictureOutput> GetFriendProfilePictureById(GetFriendProfilePictureByIdInput input)
        {
            if (!input.ProfilePictureId.HasValue || await _friendshipManager.GetFriendshipOrNullAsync(AbpSession.ToUserIdentifier(), new UserIdentifier(input.TenantId, input.UserId)) == null)
            {
                return new GetProfilePictureOutput(string.Empty);
            }

            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                var bytes = await GetProfilePictureByIdOrNull(input.ProfilePictureId.Value);
                if (bytes == null)
                {
                    return new GetProfilePictureOutput(string.Empty);
                }

                return new GetProfilePictureOutput(Convert.ToBase64String(bytes));
            }
        }

        public async Task<GetProfilePictureOutput> GetProfilePictureById(Guid profilePictureId)
        {
            return await GetProfilePictureByIdInternal(profilePictureId);
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        private async Task<byte[]> GetProfilePictureByIdOrNull(Guid profilePictureId)
        {
            var file = await _binaryObjectManager.GetOrNullAsync(profilePictureId);
            if (file == null)
            {
                return null;
            }

            return file.Bytes;
        }

        private async Task<GetProfilePictureOutput> GetProfilePictureByIdInternal(Guid profilePictureId)
        {
            var bytes = await GetProfilePictureByIdOrNull(profilePictureId);
            if (bytes == null)
            {
                return new GetProfilePictureOutput(string.Empty);
            }

            return new GetProfilePictureOutput(Convert.ToBase64String(bytes));
        }
    }
}