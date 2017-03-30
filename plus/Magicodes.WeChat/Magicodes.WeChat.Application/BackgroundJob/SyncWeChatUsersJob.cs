using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Magicodes.WeChat.Core.User;
using Magicodes.WeChat.SDK.Apis.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeChat.Application.BackgroundJob
{
    public class SyncWeChatUsersJob : BackgroundJob<int>, ITransientDependency
    {
        private readonly INotificationPublisher _notiticationPublisher;
        private readonly IRepository<WeChatUser, string> _wechatUserRepository;
        public SyncWeChatUsersJob(IRepository<WeChatUser, string> wechatUserRepository, INotificationPublisher notiticationPublisher)
        {
            _wechatUserRepository = wechatUserRepository;
            _notiticationPublisher = notiticationPublisher;
        }
        public override void Execute(int args)
        {
            //TODO:等待ABP团队解决
            //SyncWeChatUsers(args);
        }

        private void ReportProgress(int progress, string message)
        {
            Logger.Debug("progress：" + progress + " \tmessage:" + message);
            //TODO:progress
        }

        private void SyncWeChatUsers(int tenantId)
        {
            try
            {
                ReportProgress(1, "正在初始化同步信息");
                var userApi = new UserApi();
                userApi.SetKey(tenantId);
                var opendIds = new List<string>();
                ReportProgress(3, "准备获取所有的粉丝信息");
                //递归获取所有的OPENID
                GetOpenIds(opendIds, userApi);
                if (opendIds.Count == 0)
                    return;

                var distinctOpendIds = opendIds.Distinct().ToList();
                if (distinctOpendIds.Count != opendIds.Count)
                {
                    opendIds = distinctOpendIds;
                }
                while (opendIds.Count > 0)
                {
                    var successList = new List<string>();
                    GetUserInfoList(userApi, opendIds, successList, tenantId);

                    var hs = new HashSet<string>(opendIds);
                    var successhs = new HashSet<string>(successList);
                    hs.RemoveWhere(p => successhs.Contains(p));
                    opendIds = hs.ToList();

                    ReportProgress(10, "已获取 " + successList.Count + " 个粉丝...");
                }
                ReportProgress(100, "同步成功！同步数量（" + opendIds.Count + "）。");

            }
            catch (Exception ex)
            {
                ReportProgress(100, "同步失败！" + ex.Message);
                throw ex;
            }
        }

        private void GetOpenIds(List<string> opendIds, UserApi userApi, string nextOpenId = null)
        {
            var result = userApi.GetOpenIdList(nextOpenId);
            if (result.IsSuccess() && (result.Data != null))
                opendIds.AddRange(result.Data.OpenIds);
            //最多一次只能获取10000
            if (!string.IsNullOrEmpty(result.NextOpenId) && (result.Count == 10000))
                GetOpenIds(opendIds, userApi, result.NextOpenId);
        }

        private void GetUserInfoList(UserApi userApi, List<string> opendIds, List<string> successList, int tenantId)
        {
            {
                var takeCount = opendIds.Count > 100 ? 100 : opendIds.Count;
                var openIdsToGet = opendIds.Take(takeCount).ToArray();
                if (openIdsToGet.Count() > 0)
                {
                    var debugStr = "";
                    //该接口最多支持获取100个粉丝的信息
                    try
                    {
                        debugStr = "准备获取以下粉丝信息：" + string.Join(",", openIdsToGet) + "。" + Environment.NewLine;

                        var batchResult = userApi.Get(openIdsToGet);
                        if (batchResult.IsSuccess())
                        {
                            debugStr += "已成功获取粉丝信息。";
                            successList.AddRange(openIdsToGet);
                            var users = _wechatUserRepository.GetAll().Where(p => p.TenantId == tenantId && openIdsToGet.Any(p1 => p1 == p.Id)).ToList();

                            var userList = batchResult.UserInfoList.Select(userInfo => new WeChatUser
                            {
                                City = userInfo.City,
                                Country = userInfo.Country,
                                GroupId = userInfo.GroupId,
                                HeadImgUrl = userInfo.Headimgurl,
                                Language = userInfo.Language,
                                NickName = userInfo.NickName,
                                Id = userInfo.OpenId,
                                Province = userInfo.Province,
                                Remark = userInfo.Remark,
                                Sex = (int)userInfo.Sex,
                                Subscribe = userInfo.Subscribe,
                                SubscribeTime =
                                    userInfo.SubscribeTime == default(DateTime)
                                        ? DateTime.Parse("2011-05-10")
                                        : userInfo.SubscribeTime,
                                UnionId = userInfo.Unionid,
                                TenantId = tenantId
                            }).ToList();

                            foreach (var item in userList)
                            {
                                var weChatUser = users.FirstOrDefault(p => p.Id == item.Id);
                                if (weChatUser != null)
                                {
                                    weChatUser.City = item.City;
                                    weChatUser.Country = item.Country;
                                    weChatUser.GroupId = item.GroupId;
                                    weChatUser.HeadImgUrl = item.HeadImgUrl;
                                    weChatUser.Language = item.Language;
                                    weChatUser.NickName = item.NickName;
                                    weChatUser.Id = item.Id;
                                    weChatUser.Province = item.Province;
                                    weChatUser.Remark = item.Remark;
                                    weChatUser.Sex = item.Sex;
                                    weChatUser.Subscribe = item.Subscribe;
                                    weChatUser.SubscribeTime = item.SubscribeTime;
                                    weChatUser.UnionId = item.UnionId;
                                }
                                else
                                    _wechatUserRepository.Insert(item);
                            }
                        }
                        else
                        {
                            debugStr += "粉丝信息获取失败：" + batchResult.DetailResult + "。";
                        }
                    }
                    catch (Exception ex)
                    {
                        debugStr += "粉丝信息获取异常：" + ex + "。";
                    }
                    finally
                    {
                    }
                }
            }
        }
    }
}
