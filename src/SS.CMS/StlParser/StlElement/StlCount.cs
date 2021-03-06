﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.Utility;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.StlParser.StlElement
{
    [StlElement(Title = "显示数值", Description = "通过 stl:count 标签在模板中显示统计数字")]
    public class StlCount
	{
        private StlCount() { }
		public const string ElementName = "stl:count";

		[StlAttribute(Title = "需要获取值的类型")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "栏目索引")]
        private const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目名称")]
        private const string ChannelName = nameof(ChannelName);

        [StlAttribute(Title = "上级栏目的级别")]
        private const string UpLevel = nameof(UpLevel);

        [StlAttribute(Title = "从首页向下的栏目级别")]
        private const string TopLevel = nameof(TopLevel);

        [StlAttribute(Title = "内容范围")]
        private const string Scope = nameof(Scope);

        [StlAttribute(Title = "时间段")]
        private const string Since = nameof(Since);


        public const string TypeChannels = "Channels";
        public const string TypeContents = "Contents";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeChannels, "栏目数"},
            {TypeContents, "内容数"}
        };

        public static async Task<object> ParseAsync(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var type = string.Empty;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            var scope = ScopeType.Self;
            var since = string.Empty;

		    foreach (var name in contextInfo.Attributes.AllKeys)
		    {
		        var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
                {
                    channelIndex = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Scope))
                {
                    scope = TranslateUtils.ToEnum(value, ScopeType.Self);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Since))
                {
                    since = value;
                }
            }

            return await ParseImplAsync(pageInfo, contextInfo, type, channelIndex, channelName, upLevel, topLevel, scope, since);
		}

        private static async Task<string> ParseImplAsync(PageInfo pageInfo, ContextInfo contextInfo, string type, string channelIndex, string channelName, int upLevel, int topLevel, ScopeType scope, string since)
        {
            var count = 0;

            var sinceDate = Constants.SqlMinValue;
            if (!string.IsNullOrEmpty(since))
            {
                sinceDate = DateTime.Now.AddHours(-DateUtils.GetSinceHours(since));
            }

            if (string.IsNullOrEmpty(type) || StringUtils.EqualsIgnoreCase(type, TypeContents))
            {
                var channelId = await StlDataUtility.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);
                channelId = await StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(pageInfo.SiteId, channelId, channelIndex, channelName);

                var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(pageInfo.SiteId, channelId, scope);
                foreach (var theChannelId in channelIdList)
                {
                    var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(pageInfo.Site, theChannelId);
                    count += await DataProvider.ContentRepository.GetCountOfContentAddAsync(tableName, pageInfo.SiteId, theChannelId, ScopeType.Self, sinceDate, DateTime.Now.AddDays(1), 0, true);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeChannels))
            {
                var channelId = await StlDataUtility.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);
                channelId = await StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(pageInfo.SiteId, channelId, channelIndex, channelName);

                var nodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                count = nodeInfo.ChildrenCount;
            }

            return count.ToString();
        }
	}
}
