﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Shared
{
    [Route("admin/shared/groupChannelLayerAdd")]
    public partial class GroupChannelLayerAddController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public GroupChannelLayerAddController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin) return Unauthorized();

            var group = await DataProvider.ChannelGroupRepository.GetAsync(request.SiteId, request.GroupId);

            return new GetResult
            {
                GroupName = group.GroupName,
                Description = group.Description
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<ListResult>> Add([FromBody] AddRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin) return Unauthorized();

            if (await DataProvider.ChannelGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return this.Error("保存失败，已存在相同名称的栏目组！");
            }

            var groupInfo = new ChannelGroup
            {
                SiteId = request.SiteId,
                GroupName = request.GroupName,
                Description = request.Description
            };

            await DataProvider.ChannelGroupRepository.InsertAsync(groupInfo);

            await auth.AddSiteLogAsync(request.SiteId, "新增栏目组", $"栏目组:{groupInfo.GroupName}");

            var groups = await DataProvider.ChannelGroupRepository.GetChannelGroupsAsync(request.SiteId);
            var groupNames = groups.Select(x => x.GroupName);

            return new ListResult
            {
                GroupNames = groupNames,
                Groups = groups
            };
        }

        [HttpPut, Route(Route)]
        public async Task<ActionResult<ListResult>> Edit([FromBody] EditRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin) return Unauthorized();

            var groupInfo = await DataProvider.ChannelGroupRepository.GetAsync(request.SiteId, request.GroupId);

            if (groupInfo.GroupName != request.GroupName && await DataProvider.ChannelGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return this.Error("保存失败，已存在相同名称的栏目组！");
            }

            groupInfo.GroupName = request.GroupName;
            groupInfo.Description = request.Description;

            await DataProvider.ChannelGroupRepository.UpdateAsync(groupInfo);

            await auth.AddSiteLogAsync(request.SiteId, "修改栏目组", $"栏目组:{groupInfo.GroupName}");

            var groups = await DataProvider.ChannelGroupRepository.GetChannelGroupsAsync(request.SiteId);
            var groupNames = groups.Select(x => x.GroupName);

            return new ListResult
            {
                GroupNames = groupNames,
                Groups = groups
            };
        }
    }
}
