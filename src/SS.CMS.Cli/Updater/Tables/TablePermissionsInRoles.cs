﻿using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SS.CMS.Framework;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TablePermissionsInRoles
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("generalPermissions")]
        public string GeneralPermissions { get; set; }
    }

    public partial class TablePermissionsInRoles
    {
        public const string OldTableName = "bairong_PermissionsInRoles";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.PermissionsInRolesRepository.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.PermissionsInRolesRepository.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict = null;

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
