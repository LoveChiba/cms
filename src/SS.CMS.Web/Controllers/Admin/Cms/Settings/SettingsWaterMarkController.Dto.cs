﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsWaterMarkController
    {
        public class GetResult
        {
            public Site Site { get; set; }
            public IEnumerable<string> Families { get; set; }
            public string ImageUrl { get; set; }
        }

        public class UploadResult
        {
            public string ImageUrl { get; set; }
            public string VirtualUrl { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public bool IsWaterMark { get; set; }
            public int WaterMarkPosition { get; set; }
            public int WaterMarkTransparency { get; set; }
            public int WaterMarkMinWidth { get; set; }
            public int WaterMarkMinHeight { get; set; }
            public bool IsImageWaterMark { get; set; }
            public string WaterMarkFormatString { get; set; }
            public string WaterMarkFontName { get; set; }
            public int WaterMarkFontSize { get; set; }
            public string WaterMarkImagePath { get; set; }
        }

        public class UploadRequest : SiteRequest
        {
            public IFormFile File { get; set; }
        }
    }
}