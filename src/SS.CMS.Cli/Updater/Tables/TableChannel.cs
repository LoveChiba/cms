﻿using System;
using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SS.CMS.Abstractions;
using SS.CMS.Framework;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TableChannel
    {
        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("nodeName")]
        public string NodeName { get; set; }

        [JsonProperty("nodeType")]
        public string NodeType { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("contentModelID")]
        public string ContentModelId { get; set; }

        [JsonProperty("parentID")]
        public long ParentId { get; set; }

        [JsonProperty("parentsPath")]
        public string ParentsPath { get; set; }

        [JsonProperty("parentsCount")]
        public long ParentsCount { get; set; }

        [JsonProperty("childrenCount")]
        public long ChildrenCount { get; set; }

        [JsonProperty("isLastNode")]
        public string IsLastNode { get; set; }

        [JsonProperty("nodeIndexName")]
        public string NodeIndexName { get; set; }

        [JsonProperty("nodeGroupNameCollection")]
        public string NodeGroupNameCollection { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("contentNum")]
        public long ContentNum { get; set; }

        [JsonProperty("commentNum")]
        public long CommentNum { get; set; }

        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("channelFilePathRule")]
        public string ChannelFilePathRule { get; set; }

        [JsonProperty("contentFilePathRule")]
        public string ContentFilePathRule { get; set; }

        [JsonProperty("linkUrl")]
        public string LinkUrl { get; set; }

        [JsonProperty("linkType")]
        public string LinkType { get; set; }

        [JsonProperty("channelTemplateID")]
        public long ChannelTemplateId { get; set; }

        [JsonProperty("contentTemplateID")]
        public long ContentTemplateId { get; set; }

        [JsonProperty("keywords")]
        public string Keywords { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("extendValues")]
        public string ExtendValues { get; set; }
    }

    public partial class TableChannel
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_Node",
            "wcm_Node"
        };

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict,
            Process = Process
        };

        private static readonly string NewTableName = DataProvider.ChannelRepository.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.ChannelRepository.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(Channel.Id), nameof(NodeId)},
                {nameof(Channel.ChannelName), nameof(NodeName)},
                {nameof(Channel.SiteId), nameof(PublishmentSystemId)},
                {nameof(Channel.IndexName), nameof(NodeIndexName)},
                {"GroupNameCollection", nameof(NodeGroupNameCollection)},
                {nameof(Channel.ContentModelPluginId), nameof(ContentModelId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = new Dictionary<string, string>
        {
            {UpdateUtils.GetConvertValueDictKey(nameof(Channel.ContentModelPluginId), "GovInteract"), "SS.GovInteract"},
            {UpdateUtils.GetConvertValueDictKey(nameof(Channel.ContentModelPluginId), "GovPublic"), "SS.GovPublic"},
            {UpdateUtils.GetConvertValueDictKey(nameof(Channel.ContentModelPluginId), "Job"), "SS.Jobs"},
        };

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue(nameof(Channel.Content), out var contentObj))
            {
                var content = contentObj.ToString();
                content = content.Replace("@upload", "@/upload");
                row[nameof(Channel.Content)] = content;
            }

            return row;
        }
    }
}
