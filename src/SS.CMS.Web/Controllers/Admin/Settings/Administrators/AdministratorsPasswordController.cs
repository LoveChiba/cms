﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsPassword")]
    public partial class AdministratorsPasswordController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public AdministratorsPasswordController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            var userId = request.UserId;
            if (userId == 0) userId = auth.AdminId;
            if (!auth.IsAdminLoggin) return Unauthorized();
            var administrator = await DataProvider.AdministratorRepository.GetByUserIdAsync(userId);
            if (administrator == null) return NotFound();
            if (auth.AdminId != userId &&
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                Administrator = administrator
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            var userId = request.UserId;
            if (userId == 0) userId = auth.AdminId;
            if (!auth.IsAdminLoggin) return Unauthorized();
            var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(userId);
            if (adminInfo == null) return NotFound();
            if (auth.AdminId != userId &&
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var password = request.Password;
            var valid = await DataProvider.AdministratorRepository.ChangePasswordAsync(adminInfo, password);
            if (!valid.IsValid)
            {
                return this.Error($"更改密码失败：{valid.ErrorMessage}");
            }

            await auth.AddAdminLogAsync("重设管理员密码", $"管理员:{adminInfo.UserName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
