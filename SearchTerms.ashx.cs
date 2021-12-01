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
            int portalId = (context.Request.QueryString["pid"] != null ? Convert.ToInt32(context.Request.QueryString["pid"]) : 0);
            int tabModuleId = (context.Request.QueryString["mid"] != null ? Convert.ToInt32(context.Request.QueryString["mid"]) : 0);
            int categoryId = (context.Request.QueryString["cid"] != null ? Convert.ToInt32(context.Request.QueryString["cid"]) : 0);
            int userId = (context.Request.QueryString["uid"] != null ? Convert.ToInt32(context.Request.QueryString["uid"]) : 0);
            bool searchPrivate = (context.Request.QueryString["p"] != null ? Generic.ToBoolean(context.Request.QueryString["p"]) : false);
            bool searchAdmin = (context.Request.QueryString["a"] != null ? Generic.ToBoolean(context.Request.QueryString["a"]) : false);
            int searchUser = (context.Request.QueryString["u"] != null ? Convert.ToInt32(context.Request.QueryString["u"]) : 0);
            Components.DMSPortalSettings settings = Components.DocumentController.GetPortalSettings(portalId);
            if (settings == null)
            {
                settings = new Components.DMSPortalSettings();
            }
            IEnumerable<string> tags = from tag in DocumentController.FindSearchTags(term, portalId, settings.PortalWideRepository ? 0 : tabModuleId) select tag.TagName;
            IEnumerable<string> docs;
            if (searchAdmin)
            {
                List<DocumentView> viewDocs = DocumentController.GetDocumentList(categoryId, term, userId, portalId, settings.PortalWideRepository ? 0 : tabModuleId);
                if(searchUser > 0)
                {
                    viewDocs = viewDocs.FindAll(p => p.CreatedByUserID == searchUser);
                }
                docs = from doc in viewDocs select doc.DocumentName;
            }
            else
            {
                docs = from doc in DocumentController.Search(categoryId, term, searchPrivate, portalId, tabModuleId, userId) select doc.DocumentName;
            }
            /*DotNetNuke.Entities.Users.UserInfo userInfo = DotNetNuke.Entities.Users.UserController.Instance.GetUser(portalId, userId);
            DotNetNuke.Entities.Users.UserInfo userInfo = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
            List<Components.Document> docs = DocumentController.Search(categoryId, term, searchPrivate, portalId, tabModuleId, userId);
            List<string> filteredDocs = new List<string>();
            foreach (Document doc in docs)
            {
                foreach (DocumentCategory docCat in doc.Categories)
                {
                    if (userInfo.IsInRole(docCat.Category.RoleName))
                    {
                        filteredDocs.Add(doc.DocumentName);
                        break;
                    }
                }
            }*/
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            context.Response.Write(oSerializer.Serialize(tags.Concat(docs)));
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