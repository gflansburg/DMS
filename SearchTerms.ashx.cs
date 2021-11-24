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
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            List<Tag> tags = DocumentController.FindSearchTags(term, PortalId, settings.PortalWideRepository ? 0 : TabModuleId);
            if (tags.Count > 0)
            {
                sb.Append("[");
                bool first = true;
                foreach (Tag tag in tags)
                {
                    if (!first)
                    {
                        sb.Append(",");
                    }
                    //sb.Append("{" + String.Format("\"id\":{0},\"value\":\"{1}\"", tag.TagId, tag.TagName.Replace("\"", "&quot;")) + "}");
                    sb.Append(String.Format("\"{0}\"", tag.TagName.Replace("\"", "&quot;")));
                    first = false;
                }
                sb.Append("]");
            }
            context.Response.Write(sb.ToString());
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