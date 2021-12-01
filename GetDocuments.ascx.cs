/*
' Copyright (c) 2021  Gafware
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
using System.Reflection;
using System.Web.UI.WebControls;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Tabs;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Xml.Serialization;
using Gafware.Modules.DMS.Components;
using System.Web;
using DotNetNuke.Common;
using System.Collections;
using DotNetNuke.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Gafware.Modules.DMS
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from DMSModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class GetDocuments : DMSModuleBase, IActionable
    {
        private readonly INavigationManager _navigationManager;

        public GetDocuments()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                documentSearchResults.NavigationManager = _navigationManager;
                documentSearchResults.CategoryName = CategoryName;
                documentSearchResults.Theme = Theme;
                documentSearchResults.PageSize = PageSize;
                documentSearchResults.ThumbnailSize = ThumbnailSize;
                documentSearchResults.ThumbnailType = ThumbnailType;
                documentSearchResults.UseLocalFile = SaveLocalFile;
                documentSearchResults.PortalId = PortalId;
                documentSearchResults.PortalWideRepository = PortalWideRepository;
                documentSearchResults.UserId = UserId;
                documentSearchResults.TabModuleId = TabModuleId;
                documentSearchResults.ModuleId = ModuleId;
                documentSearchResults.IsAdmin = IsAdmin();
                documentSearchResults.ControlPath = ControlPath;
                if (!IsPostBack)
                {
                    string strQuery = (Request.QueryString["q"] ?? String.Empty).Trim();
                    try
                    {
                        Gafware.Modules.DMS.Cryptography.CryptographyUtil.Decrypt(strQuery);
                        documentSearchResults.QueryString = Request.QueryString.ToString();
                    }
                    catch
                    {
                        try
                        {
                            byte[] aQuery = Generic.StringToByteArray(strQuery);
                            string strNewQuery = HttpUtility.UrlDecode(System.Text.Encoding.ASCII.GetString(aQuery));
                            Gafware.Modules.DMS.Cryptography.CryptographyUtil.Decrypt(strNewQuery);
                            documentSearchResults.QueryString = String.Format("q={0}", HttpUtility.UrlEncode(strNewQuery));
                        }
                        catch
                        {
                            documentSearchResults.QueryString = Request.QueryString.ToString();
                        }
                    }
                    Page.Title = documentSearchResults.Title;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public bool IsAdmin()
        {
            if ((new ModuleSecurity((new ModuleController()).GetTabModule(this.TabModuleId))).HasEditPermissions)
            {
                return true;
            }
            return false;
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                return new ModuleActionCollection();
            }
        }
    }
}