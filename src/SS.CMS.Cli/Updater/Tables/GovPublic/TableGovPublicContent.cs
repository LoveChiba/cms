﻿using System;
using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SS.CMS.Abstractions;
using SS.CMS.Framework;

namespace SS.CMS.Cli.Updater.Tables.GovPublic
{
    public partial class TableGovPublicContent
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("addUserName")]
        public string AddUserName { get; set; }

        [JsonProperty("lastEditUserName")]
        public string LastEditUserName { get; set; }

        [JsonProperty("lastEditDate")]
        public DateTime LastEditDate { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("contentGroupNameCollection")]
        public string ContentGroupNameCollection { get; set; }

        [JsonProperty("groupNameCollection")]
        public string GroupNameCollection { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("sourceID")]
        public long SourceId { get; set; }

        [JsonProperty("referenceID")]
        public long ReferenceId { get; set; }

        [JsonProperty("isChecked")]
        public string IsChecked { get; set; }

        [JsonProperty("checkedLevel")]
        public long CheckedLevel { get; set; }

        [JsonProperty("comments")]
        public long Comments { get; set; }

        [JsonProperty("hits")]
        public long Hits { get; set; }

        [JsonProperty("hitsByDay")]
        public long HitsByDay { get; set; }

        [JsonProperty("hitsByWeek")]
        public long HitsByWeek { get; set; }

        [JsonProperty("hitsByMonth")]
        public long HitsByMonth { get; set; }

        [JsonProperty("lastHitsDate")]
        public DateTime LastHitsDate { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }

        [JsonProperty("departmentID")]
        public long DepartmentId { get; set; }

        [JsonProperty("category1ID")]
        public long Category1Id { get; set; }

        [JsonProperty("category2ID")]
        public long Category2Id { get; set; }

        [JsonProperty("category3ID")]
        public long Category3Id { get; set; }

        [JsonProperty("category4ID")]
        public long Category4Id { get; set; }

        [JsonProperty("category5ID")]
        public long Category5Id { get; set; }

        [JsonProperty("category6ID")]
        public long Category6Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("publishDate")]
        public DateTime PublishDate { get; set; }

        [JsonProperty("effectDate")]
        public DateTime EffectDate { get; set; }

        [JsonProperty("isAbolition")]
        public string IsAbolition { get; set; }

        [JsonProperty("abolitionDate")]
        public DateTime AbolitionDate { get; set; }

        [JsonProperty("documentNo")]
        public string DocumentNo { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        [JsonProperty("keywords")]
        public string Keywords { get; set; }

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty("isRecommend")]
        public string IsRecommend { get; set; }

        [JsonProperty("isTop")]
        public string IsTop { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("addDate")]
        public DateTime AddDate { get; set; }
    }

    public partial class TableGovPublicContent
    {
        public static readonly string NewTableName = "ss_govpublic_content";

        private static List<TableColumn> NewColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = "Identifier",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "DocumentNo",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "DepartmentId",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "Publisher",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "Keywords",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "PublishDate",
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = "EffectDate",
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = "IsAbolition",
                DataType = DataType.VarChar,
                DataLength = 10
            },
            new TableColumn
            {
                AttributeName = "AbolitionDate",
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = "Description",
                DataType = DataType.VarChar,
                DataLength = 2000
            },
            new TableColumn
            {
                AttributeName = "ImageUrl",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "FileUrl",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "Body",
                DataType = DataType.Text
            }
        };

        private static List<TableColumn> GetNewColumns(List<TableColumn> oldColumns)
        {
            var columns = new List<TableColumn>();
            var repository =
                new Repository<Content>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
            columns.AddRange(repository.TableColumns);
            columns.AddRange(NewColumns);

            foreach (var tableColumnInfo in oldColumns)
            {
                if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(NodeId)))
                {
                    tableColumnInfo.AttributeName = nameof(SS.CMS.Abstractions.Content.ChannelId);
                }
                else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(PublishmentSystemId)))
                {
                    tableColumnInfo.AttributeName = nameof(SS.CMS.Abstractions.Content.SiteId);
                }
                else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(ContentGroupNameCollection)))
                {
                    tableColumnInfo.AttributeName = nameof(SS.CMS.Abstractions.Content.GroupNames);
                }
                else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(GroupNameCollection)))
                {
                    tableColumnInfo.AttributeName = nameof(SS.CMS.Abstractions.Content.GroupNames);
                }

                if (!columns.Exists(c => StringUtils.EqualsIgnoreCase(c.AttributeName, tableColumnInfo.AttributeName)))
                {
                    columns.Add(tableColumnInfo);
                }
            }

            return columns;
        }

        public static ConvertInfo GetConverter(List<TableColumn> oldColumns)
        {
            return new ConvertInfo
            {
                NewTableName = NewTableName,
                NewColumns = GetNewColumns(oldColumns),
                ConvertKeyDict = ConvertKeyDict,
                ConvertValueDict = ConvertValueDict
            };
        }

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(SS.CMS.Abstractions.Content.ChannelId), nameof(NodeId)},
                {nameof(SS.CMS.Abstractions.Content.SiteId), nameof(PublishmentSystemId)},
                {nameof(SS.CMS.Abstractions.Content.GroupNames), nameof(ContentGroupNameCollection)},
                {nameof(SS.CMS.Abstractions.Content.GroupNames), nameof(GroupNameCollection)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
