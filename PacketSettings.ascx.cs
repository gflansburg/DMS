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
using System.Collections.Generic;
using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using Gafware.Modules.DMS.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Gafware.Modules.DMS
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Settings class manages Module Settings
    /// 
    /// Typically your settings control would be used to manage settings for your module.
    /// There are two types of settings, ModuleSettings, and TabModuleSettings.
    /// 
    /// ModuleSettings apply to all "copies" of a module on a site, no matter which page the module is on. 
    /// 
    /// TabModuleSettings apply only to the current module on the current page, if you copy that module to
    /// another page the settings are not transferred.
    /// 
    /// If you happen to save both TabModuleSettings and ModuleSettings, TabModuleSettings overrides ModuleSettings.
    /// 
    /// Below we have some examples of how to access these settings but you will need to uncomment to use.
    /// 
    /// Because the control inherits from DMSSettingsBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class PacketSettings : DMSModuleSettingsBase
    {
        private readonly INavigationManager _navigationManager;

        public PacketSettings()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        #region Base Method Implementations

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            try
            {
                if (Page.IsPostBack == false)
                {
                    //Check for existing settings and use those on this page
                    //Settings["SettingName"]

                    List<Repository> repositories = DocumentController.GetAllRepositories(PortalId);
                    ddlRepository.DataSource = repositories;
                    ddlRepository.DataBind();
                    pnlRepository.Visible = repositories.Count > 1;

                    if (Settings.Contains("PageSize"))
                        ddlPageSize.SelectedIndex = ddlPageSize.Items.IndexOf(ddlPageSize.Items.FindByValue(Settings["PageSize"].ToString()));
                    else
                        ddlPageSize.SelectedIndex = ddlPageSize.Items.IndexOf(ddlPageSize.Items.FindByValue(PageSize.ToString()));
                    if (Settings.Contains("ThumbnailSize"))
                        ddlThumbnailSize.SelectedIndex = ddlThumbnailSize.Items.IndexOf(ddlThumbnailSize.Items.FindByValue(Settings["ThumbnailSize"].ToString()));
                    else
                        ddlThumbnailSize.SelectedIndex = ddlThumbnailSize.Items.IndexOf(ddlThumbnailSize.Items.FindByValue(ThumbnailSize.ToString()));
                    if (Settings.Contains("ThumbnailType"))
                        ddlThumbnailType.SelectedIndex = ddlThumbnailType.Items.IndexOf(ddlThumbnailType.Items.FindByValue(Settings["ThumbnailType"].ToString()));
                    else
                        ddlThumbnailType.SelectedIndex = ddlThumbnailType.Items.IndexOf(ddlThumbnailType.Items.FindByValue(ThumbnailType));
                    if (Settings.Contains("RepositoryID"))
                        ddlRepository.SelectedIndex = ddlRepository.Items.IndexOf(ddlRepository.Items.FindByValue(Settings["RepositoryID"].ToString()));
                    else
                        ddlRepository.SelectedIndex = 0;
                    if (Settings.Contains("Theme"))
                        ddlTheme.SelectedIndex = ddlTheme.Items.IndexOf(ddlTheme.Items.FindByValue(Settings["Theme"].ToString()));
                    else
                        ddlTheme.SelectedIndex = ddlTheme.Items.IndexOf(ddlTheme.Items.FindByValue(Theme));

                    ddlPacket.DataSource = PacketController.GetAllPackets(PortalId, PortalWideRepository ? 0 : Convert.ToInt32(ddlRepository.SelectedValue));
                    ddlPacket.DataBind();

                    if (Settings.Contains("PacketID"))
                        ddlPacket.SelectedIndex = ddlPacket.Items.IndexOf(ddlPacket.Items.FindByText(Settings["PacketID"].ToString()));
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UpdateSettings()
        {
            try
            {
                var modules = new ModuleController();

                //module settings
                modules.UpdateTabModuleSetting(TabModuleId, "PacketID", ddlPacket.SelectedItem.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "ThumbnailType", ddlThumbnailType.SelectedValue);
                modules.UpdateTabModuleSetting(TabModuleId, "ThumbnailSize", ddlThumbnailSize.SelectedValue);
                modules.UpdateTabModuleSetting(TabModuleId, "PageSize", ddlPageSize.SelectedValue);
                modules.UpdateTabModuleSetting(TabModuleId, "Theme", ddlTheme.SelectedValue);
                Packet packet = PacketController.GetPacket(Convert.ToInt32(ddlPacket.SelectedValue));
                modules.UpdateTabModuleSetting(TabModuleId, "RepositoryID", packet.TabModuleId.ToString());
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        public void Page_Load()
        {
            if (Request.QueryString["ctl"].ToLower() == "editsettings" && !IsAdmin())
            {
                base.Response.Redirect(_navigationManager.NavigateURL(), true);
            }
            if (!IsPostBack && Request.QueryString["ctl"].ToLower() == "editsettings")
            {
                LoadSettings();
                pnlUpdateSettings.Visible = true;
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

        protected void btnReload_Click(object sender, EventArgs e)
        {
            int oldIndex = ddlPacket.SelectedIndex;
            ddlPacket.SelectedIndex = -1;
            ddlPacket.DataSource = PacketController.GetAllPackets(PortalId, PortalWideRepository ? 0 : Convert.ToInt32(ddlRepository.SelectedValue));
            ddlPacket.DataBind();
            if(oldIndex < ddlPacket.Items.Count)
            {
                ddlPacket.SelectedIndex = oldIndex;
            }
        }

        protected void ddlRepository_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnReload_Click(sender, e);
        }

        protected void updateSettings_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                UpdateSettings();
                cancelSettings_Click(sender, e);
            }
        }

        protected void cancelSettings_Click(object sender, EventArgs e)
        {
            Response.Redirect(_navigationManager.NavigateURL());
        }
    }
}