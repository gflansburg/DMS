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

using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
//using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search;

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
    public class FeatureController : IUpgradeable
    {


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
            return Version;
        }

        #endregion

    }

}
