using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafware.Modules.DMS
{
    /// <summary>
    /// Summary description for SearchDocuments
    /// </summary>
    public class SearchDocuments : IHttpHandler
    {
        public class SearchResult
        {
            public int DocumentId { get; set; }
            public string DocumentName { get; set; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string term = (context.Request.QueryString["term"] ?? (context.Request.QueryString["q"] ?? String.Empty));
            int portalId = (context.Request.QueryString["pid"] != null ? Convert.ToInt32(context.Request.QueryString["pid"]) : 0);
            int tabModuleId = (context.Request.QueryString["mid"] != null ? Convert.ToInt32(context.Request.QueryString["mid"]) : 0);
            Components.DMSPortalSettings settings = Components.DocumentController.GetPortalSettings(portalId);
            if(settings == null)
            {
                settings = new Components.DMSPortalSettings();
            }
            List<Components.Document> docs = Components.DocumentController.GetAllPublicDocuments(portalId, settings.PortalWideRepository ? 0 : tabModuleId).Where(d => d.DocumentName.IndexOf(term, StringComparison.OrdinalIgnoreCase) != -1).ToList();
            List<SearchResult> results = (from doc in docs select new SearchResult { DocumentId = doc.DocumentId, DocumentName = doc.DocumentName }).ToList();
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = oSerializer.Serialize(results);
            context.Response.Write(json);
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