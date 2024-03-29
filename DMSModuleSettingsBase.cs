﻿/*
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
using DotNetNuke.Entities.Modules;

namespace Gafware.Modules.DMS
{
    public class DMSModuleSettingsBase : ModuleSettingsBase
    {
        public int FileNotificationsRole
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.FileNotificationsRoleId : 0);
            }
        }

        public int UserRole
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.UserRoleId : 0);
            }
        }

        public string NewFileMsg
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.NewFileMsg : "A [FILETYPENAME] file named '[FILENAME]' as been uploaded to the Document Management System by [UPLOADER] to the document named '[DOCUMENT]'");
            }
        }

        public string NewFileSubject
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.NewFileSubject : "A new file has been uploaded to the Document Management System");
            }
        }

        public bool ForceHttps
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.ForceHttps : false);
            }
        }

        public bool UseThumbnails
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.UseThumbnails : true);
            }
        }

        public bool CreatePDF
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.CreatePDF : true);
            }
        }

        public bool CreateWord
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.CreateWord : true);
            }
        }

        public bool CreateExcel
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.CreateExcel : true);
            }
        }

        public bool CreatePowerPoint
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.CreatePowerPoint : true);
            }
        }

        public bool CreateImage
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.CreateImage : true);
            }
        }

        public bool CreateAudio
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.CreateAudio : true);
            }
        }

        public bool CreateVideo
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.CreateVideo : true);
            }
        }

        public string CategoryName
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? (String.IsNullOrEmpty(portal.CategoryName) ? "Category" : portal.CategoryName) : "Category");
            }
        }

        public string RepositoryName
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? (String.IsNullOrEmpty(portal.Name) ? String.Empty : portal.Name) : String.Empty);
            }
        }

        public bool SaveLocalFile
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.SaveLocalFile : false);
            }
        }

        public bool ShowTips
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.ShowTips : true);
            }
        }

        public bool ShowInstructions
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.ShowInstructions : true);
            }
        }

        public string Instructions
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? (String.IsNullOrEmpty(portal.Instructions) ? "To view all documents, click \"Go!\" without typing a keyword." : portal.Instructions) : "To view all documents, click \"Go!\" without typing a keyword.");
            }
        }

        public string ThumbnailType
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? (String.IsNullOrEmpty(portal.ThumbnailType) ? "classic" : portal.ThumbnailType) : "classic");
            }
        }

        public int ThumbnailSize
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.ThumbnailSize : 128);
            }
        }

        public int PageSize
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? portal.PageSize : 20);
            }
        }

        public string Theme
        {
            get
            {
                Components.Repository portal = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                return (portal != null ? (String.IsNullOrEmpty(portal.Theme) ? "990000" : portal.Theme) : "990000");
            }
        }

        public bool PortalWideRepository
        {
            get
            {
                Components.DMSPortalSettings settings = Components.DocumentController.GetPortalSettings(PortalId);
                return (settings != null ? settings.PortalWideRepository : true);
            }
        }

        public bool EnableDNNSearch
        {
            get
            {
                Components.DMSPortalSettings settings = Components.DocumentController.GetPortalSettings(PortalId);
                return (settings != null ? settings.EnableDNNSearch : true);
            }
        }
    }
}