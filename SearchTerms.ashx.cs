using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gafware.Modules.DMS.Components;

namespace Gafware.Modules.DMS
{
    /// <summary>
    /// Summary description for SearchTerms
    /// </summary>
    public class SearchTerms : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string term = (context.Request.QueryString["term"] ?? (context.Request.QueryString["q"] ?? String.Empty));
            int PortalId = (context.Request.QueryString["pid"] != null ? Convert.ToInt32(context.Request.QueryString["pid"]) : 0);
            int TabModuleId = (context.Request.QueryString["mid"] != null ? Convert.ToInt32(context.Request.QueryString["mid"]) : 0);
            Components.DMSPortalSettings settings = Components.DocumentController.GetPortalSettings(PortalId);
            if (settings == null)
            {
                settings = new Components.DMSPortalSettings();
            }
            IEnumerable<string> results = from tag in DocumentController.FindSearchTags(term, PortalId, settings.PortalWideRepository ? 0 : TabModuleId)
                                          select tag.TagName;
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            context.Response.Write(oSerializer.Serialize(results));
            context.Response.ContentType = "application/json";
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}