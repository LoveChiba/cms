﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsTranslateController
    {
        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
            public List<Select<int>> TransSites { get; set; }
            public IEnumerable<Select<string>> TranslateTypes { get; set; }
        }

        public class GetOptionsRequest : SiteRequest
        {
            public int TransSiteId { get; set; }
        }

        public class GetOptionsResult
        {
            public Cascade<int> TransChannels { get; set; }
        }

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public IEnumerable<int> ChannelIds { get; set; }
            public int TransSiteId { get; set; }
            public int TransChannelId { get; set; }
            public TranslateType TranslateType { get; set; }
            public bool IsDeleteAfterTranslate { get; set; }
        }

        public async Task TranslateAsync(Site site, int targetSiteId, int targetChannelId, TranslateType translateType, IEnumerable<int> channelIds, bool isDeleteAfterTranslate, int adminId)
        {
            var channelIdList = new List<int>();//需要转移的栏目ID
            foreach (var channelId in channelIds)
            {
                if (translateType != TranslateType.Content)//需要转移栏目
                {
                    if (!await DataProvider.ChannelRepository.IsAncestorOrSelfAsync(site.Id, channelId, targetChannelId))
                    {
                        channelIdList.Add(channelId);
                    }
                }

                if (translateType == TranslateType.Content)//转移内容
                {
                    await TranslateContentAsync(site, channelId, targetSiteId, targetChannelId, isDeleteAfterTranslate);
                }
            }

            if (translateType != TranslateType.Content)//需要转移栏目
            {
                var channelIdListToTranslate = new List<int>(channelIdList);
                foreach (var channelId in channelIdList)
                {
                    var subChannelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(site.Id, channelId, ScopeType.Descendant);

                    if (subChannelIdList != null && subChannelIdList.Count > 0)
                    {
                        foreach (var channelIdToDelete in subChannelIdList)
                        {
                            if (channelIdListToTranslate.Contains(channelIdToDelete))
                            {
                                channelIdListToTranslate.Remove(channelIdToDelete);
                            }
                        }
                    }
                }

                var nodeInfoList = new List<Channel>();
                foreach (var channelId in channelIdListToTranslate)
                {
                    var nodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                    nodeInfoList.Add(nodeInfo);
                }

                await TranslateChannelAndContentAsync(site, nodeInfoList, targetSiteId, targetChannelId, translateType, null, null, isDeleteAfterTranslate);

                if (isDeleteAfterTranslate)
                {
                    foreach (var channelId in channelIdListToTranslate)
                    {
                        await DataProvider.ContentRepository.RecycleAllAsync(site, channelId, adminId);
                        await DataProvider.ChannelRepository.DeleteAsync(site, channelId, adminId);
                    }
                }
            }


        }

        private async Task TranslateChannelAndContentAsync(Site site, List<Channel> nodeInfoList, int targetSiteId, int parentId, TranslateType translateType, List<string> nodeIndexNameList, List<string> filePathList, bool isDeleteAfterTranslate)
        {
            if (nodeInfoList == null || nodeInfoList.Count == 0)
            {
                return;
            }

            if (nodeIndexNameList == null)
            {
                nodeIndexNameList = (await DataProvider.ChannelRepository.GetIndexNameListAsync(targetSiteId)).ToList();
            }

            if (filePathList == null)
            {
                filePathList = (await DataProvider.ChannelRepository.GetAllFilePathBySiteIdAsync(targetSiteId)).ToList();
            }

            foreach (var oldNodeInfo in nodeInfoList)
            {
                var nodeInfo = oldNodeInfo.Clone<Channel>();

                nodeInfo.SiteId = targetSiteId;
                nodeInfo.ParentId = parentId;
                nodeInfo.ChildrenCount = 0;
                nodeInfo.AddDate = DateTime.Now;

                if (isDeleteAfterTranslate)
                {
                    nodeIndexNameList.Add(nodeInfo.IndexName);
                }

                else if (!string.IsNullOrEmpty(nodeInfo.IndexName) && nodeIndexNameList.IndexOf(nodeInfo.IndexName) == -1)
                {
                    nodeIndexNameList.Add(nodeInfo.IndexName);
                }
                else
                {
                    nodeInfo.IndexName = string.Empty;
                }

                if (!string.IsNullOrEmpty(nodeInfo.FilePath) && filePathList.IndexOf(nodeInfo.FilePath) == -1)
                {
                    filePathList.Add(nodeInfo.FilePath);
                }
                else
                {
                    nodeInfo.FilePath = string.Empty;
                }

                var targetChannelId = await DataProvider.ChannelRepository.InsertAsync(nodeInfo);

                if (translateType == TranslateType.All)
                {
                    await TranslateContentAsync(site, oldNodeInfo.Id, targetSiteId, targetChannelId, isDeleteAfterTranslate);
                }

                if (targetChannelId != 0)
                {
                    //var orderByString = ETaxisTypeUtils.GetChannelOrderByString(ETaxisType.OrderByTaxis);
                    //var childrenNodeInfoList = DataProvider.ChannelRepository.GetChannelInfoList(oldNodeInfo, 0, "", EScopeType.Children, orderByString);

                    var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(site.Id, oldNodeInfo.Id, ScopeType.Children);
                    var childrenNodeInfoList = new List<Channel>();
                    foreach (var channelId in channelIdList)
                    {
                        childrenNodeInfoList.Add(await DataProvider.ChannelRepository.GetAsync(channelId));
                    }

                    if (channelIdList.Count > 0)
                    {
                        await TranslateChannelAndContentAsync(site, childrenNodeInfoList, targetSiteId, targetChannelId, translateType, nodeIndexNameList, filePathList, isDeleteAfterTranslate);
                    }

                    await _createManager.CreateChannelAsync(targetSiteId, targetChannelId);
                }
            }
        }

        private async Task TranslateContentAsync(Site site, int channelId, int targetSiteId, int targetChannelId, bool isDeleteAfterTranslate)
        {
            var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
            var contentIdList = await DataProvider.ContentRepository.GetContentIdsAsync(site, channel);

            var translateType = isDeleteAfterTranslate
                ? TranslateContentType.Cut
                : TranslateContentType.Copy;

            foreach (var contentId in contentIdList)
            {
                await ContentUtility.TranslateAsync(site, channelId, contentId, targetSiteId, targetChannelId, translateType, _createManager);
            }
        }
    }
}
