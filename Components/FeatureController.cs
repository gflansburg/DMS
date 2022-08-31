/*
' Copyright (c) 2021 Gafware
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
//using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Search;
using DotNetNuke.Services.Search.Entities;

namespace Gafware.Modules.DMS.Components
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Controller class for DMS
    /// 
    /// The FeatureController class is defined as the BusinessController in the manifest file (.dnn)
    /// DotNetNuke will poll this class to find out which Interfaces the class implements. 
    /// 
    /// The IPortable interface is used to import/export content from a DNN module
    /// 
    /// The ISearchable interface is used by DNN to index the content of a module
    /// 
    /// The IUpgradeable interface allows module developers to execute code during the upgrade 
    /// process for a module.
    /// 
    /// Below you will find stubbed out implementations of each, uncomment and populate with your own data
    /// </summary>
    /// -----------------------------------------------------------------------------

    //uncomment the interfaces to add the support.
    public class FeatureController : ModuleSearchBase, IUpgradeable
    {
        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDateUtc)
        {
            List<SearchDocument> searchDocuments = new List<SearchDocument>();
            PortalInfo portalInfo = DotNetNuke.Entities.Portals.PortalController.Instance.GetPortal(moduleInfo.PortalID);
            if (portalInfo != null)
            {
                string portalAlias = String.Empty;
                PortalAliasController portalAliasController = new PortalAliasController();
                foreach (PortalAliasInfo portalAliasInfo in portalAliasController.GetPortalAliasesByPortalId(moduleInfo.PortalID))
                {
                    if (portalAliasInfo.IsPrimary)
                    {
                        portalAlias = portalAliasInfo.HTTPAlias;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(portalAlias))
                {
                    DMSPortalSettings portalSettings = DocumentController.GetPortalSettings(moduleInfo.PortalID);
                    if (portalSettings != null && portalSettings.EnableDNNSearch)
                    {
                        List<Document> documents = DocumentController.GetAllDocuments(moduleInfo.PortalID, portalSettings.PortalWideRepository ? 0 : moduleInfo.TabModuleID).Where(d => d.LastModifiedOnDate.ToUniversalTime() >= beginDateUtc).ToList();
                        foreach (Document document in documents)
                        {
                            SearchDocument searchDocument = new SearchDocument()
                            {
                                ModuleId = moduleInfo.ModuleID,
                                ModuleDefId = moduleInfo.ModuleDefID,
                                PortalId = moduleInfo.PortalID,
                                TabId = moduleInfo.TabID,
                                Title = document.DocumentName,
                                ModifiedTimeUtc = document.LastModifiedOnDate.ToUniversalTime(),
                                Description = document.DocumentDetails,
                                IsActive = (document.IsPublic && document.IsSearchable && (!document.ActivationDate.HasValue || document.ActivationDate <= DateTime.Now) && (!document.ExpirationDate.HasValue || document.ExpirationDate > DateTime.Now)),
                                Url = string.Format("{0}/ctl/GetDocuments/mid/{1}/q/{2}", moduleInfo.ParentTab.TabPath.Replace("//", "/"), moduleInfo.ModuleID, Generic.StringToHex(Generic.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("docids={0}", document.DocumentId))))),
                                Permissions = "Administrators;",
                                Tags = document.Tags.Select(t => t.Tag.TagName).ToList(),
                                UniqueKey = document.DocumentId.ToString(),
                                RoleId = -1,
                                AuthorUserId = (document.IsGroupOwner ? -1 : document.CreatedByUserID)
                            };
                            if (document.UseCategorySecurityRoles)
                            {
                                foreach (DocumentCategory category in document.Categories)
                                {
                                    RoleInfo role = Components.UserController.GetRoleById(moduleInfo.PortalID, category.Category.RoleId);
                                    if (!searchDocument.Permissions.Contains(role.RoleName + ";", StringComparison.OrdinalIgnoreCase))
                                    {
                                        searchDocument.Permissions += role.RoleName + ";";
                                    }
                                }
                                if (searchDocument.Permissions.EndsWith(";"))
                                {
                                    searchDocument.Permissions = searchDocument.Permissions.Substring(0, searchDocument.Permissions.Length - 1);
                                }
                            }
                            else
                            {
                                RoleInfo role = Components.UserController.GetRoleById(moduleInfo.PortalID, document.SecurityRoleId);
                                searchDocument.Permissions += role.RoleName;
                            }
                            searchDocuments.Add(searchDocument);
                        }
                    }
                }
            }
            return searchDocuments;
        }

        #region Optional Interfaces

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ExportModule implements the IPortable ExportModule Interface
        /// </summary>
        /// <param name="ModuleID">The Id of the module to be exported</param>
        /// -----------------------------------------------------------------------------
        //public string ExportModule(int ModuleID)
        //{
        //string strXML = "";

        //List<DMSInfo> colDMSs = GetDMSs(ModuleID);
        //if (colDMSs.Count != 0)
        //{
        //    strXML += "<DMSs>";

        //    foreach (DMSInfo objDMS in colDMSs)
        //    {
        //        strXML += "<DMS>";
        //        strXML += "<content>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDMS.Content) + "</content>";
        //        strXML += "</DMS>";
        //    }
        //    strXML += "</DMSs>";
        //}

        //return strXML;

        //	throw new System.NotImplementedException("The method or operation is not implemented.");
        //}

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ImportModule implements the IPortable ImportModule Interface
        /// </summary>
        /// <param name="ModuleID">The Id of the module to be imported</param>
        /// <param name="Content">The content to be imported</param>
        /// <param name="Version">The version of the module to be imported</param>
        /// <param name="UserId">The Id of the user performing the import</param>
        /// -----------------------------------------------------------------------------
        //public void ImportModule(int ModuleID, string Content, string Version, int UserID)
        //{
        //XmlNode xmlDMSs = DotNetNuke.Common.Globals.GetContent(Content, "DMSs");
        //foreach (XmlNode xmlDMS in xmlDMSs.SelectNodes("DMS"))
        //{
        //    DMSInfo objDMS = new DMSInfo();
        //    objDMS.ModuleId = ModuleID;
        //    objDMS.Content = xmlDMS.SelectSingleNode("content").InnerText;
        //    objDMS.CreatedByUser = UserID;
        //    AddDMS(objDMS);
        //}

        //	throw new System.NotImplementedException("The method or operation is not implemented.");
        //}

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSearchItems implements the ISearchable Interface
        /// </summary>
        /// <param name="ModInfo">The ModuleInfo for the module to be Indexed</param>
        /// -----------------------------------------------------------------------------
        //public DotNetNuke.Services.Search.SearchItemInfoCollection GetSearchItems(DotNetNuke.Entities.Modules.ModuleInfo ModInfo)
        //{
        //SearchItemInfoCollection SearchItemCollection = new SearchItemInfoCollection();

        //List<DMSInfo> colDMSs = GetDMSs(ModInfo.ModuleID);

        //foreach (DMSInfo objDMS in colDMSs)
        //{
        //    SearchItemInfo SearchItem = new SearchItemInfo(ModInfo.ModuleTitle, objDMS.Content, objDMS.CreatedByUser, objDMS.CreatedDate, ModInfo.ModuleID, objDMS.ItemId.ToString(), objDMS.Content, "ItemId=" + objDMS.ItemId.ToString());
        //    SearchItemCollection.Add(SearchItem);
        //}

        //return SearchItemCollection;

        //	throw new System.NotImplementedException("The method or operation is not implemented.");
        //}

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpgradeModule implements the IUpgradeable Interface
        /// </summary>
        /// <param name="Version">The current version of the module</param>
        /// -----------------------------------------------------------------------------
        public string UpgradeModule(string Version)
        {
            bool found = false;
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            HttpHandlersSection section = (HttpHandlersSection)config.GetSection("system.web/httpHandlers");
            foreach (HttpHandlerAction handler in section.Handlers)
            {
                //<add name="Telerik.ReportViewer.axd_*" path="Telerik.ReportViewer.axd" verb="*" type ="Telerik.ReportViewer.WebForms.HttpHandler, Telerik.ReportViewer.WebForms, Version=15.1.21.616, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" preCondition="integratedMode"/>
                if (handler.Path.Equals("Telerik.ReportViewer.axd"))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                section.Handlers.Add(new HttpHandlerAction("Telerik.ReportViewer.axd", "Telerik.ReportViewer.WebForms.HttpHandler, Telerik.ReportViewer.WebForms, Version=15.1.21.616, Culture=neutral, PublicKeyToken=a9d7983dfcc261be", "*"));
                config.Save();
            }
            found = false;
            ProtocolsSection protocolsSection = (ProtocolsSection)config.GetSection("system.web/webServices/protocols");
            foreach(ProtocolElement element in protocolsSection.Protocols)
            {
                if(element.Name.Equals("HttpGet"))
                {
                    found = true;
                }
            }
            if(!found)
            {
                protocolsSection.Protocols.Add(new ProtocolElement("HttpGet"));
                config.Save();
            }
            foreach (ProtocolElement element in protocolsSection.Protocols)
            {
                if (element.Name.Equals("HttpPost"))
                {
                    found = true;
                }
            }
            if (!found)
            {
                protocolsSection.Protocols.Add(new ProtocolElement("HttpPost"));
                config.Save();
            }
            return Version;
        }

        #endregion

    }

}
