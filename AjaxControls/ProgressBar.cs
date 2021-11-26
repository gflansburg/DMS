using System;
using System.Web.UI;
using System.Collections.Generic;

namespace Gafware.Modules.DMS.AjaxControls
{
	public class ProgressBar : ScriptControl
	{
		public int PollInterval { get; set; }
        public string FinishLink { get; set; }
        public int PortalId { get; set; }
        public int TabModuleId { get; set; }
        public bool PortalWideRepository { get; set; }
        public string ControlPath { get; set; }
        public string FilePath { get; set; }
        public bool SubFolderIsDocumentName { get; set; }
        public bool SubFolderIsTag { get; set; }
        public bool PrependSubFolderName { get; set; }
        public string Seperator { get; set; }
        public int FirstLevel { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int OwnerId { get; set; }
        public bool Searchable { get; set; }
        public bool UseCategorySecurityRoles { get; set; }
        public int SecurityRoleId { get; set; }
        public int[] Categories { get; set; }
        public int FilesImported { get; set; }

        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Gafware.Modules.DMS.ProgressBar", ClientID);

			descriptor.AddProperty("pollInterval", PollInterval);

            descriptor.AddProperty("finishLink", FinishLink);

            descriptor.AddProperty("portalId", PortalId);
            
            descriptor.AddProperty("tabModuleId", TabModuleId);
            
            descriptor.AddProperty("portalWideRepository", PortalWideRepository);
            
            descriptor.AddProperty("controlPath", ControlPath);
            
            descriptor.AddProperty("filePath", FilePath);
            
            descriptor.AddProperty("subFolderIsDocumentName", SubFolderIsDocumentName);
            
            descriptor.AddProperty("subFolderIsTag", SubFolderIsTag);
            
            descriptor.AddProperty("prependSubFolderName", PrependSubFolderName);
            
            descriptor.AddProperty("seperator", Seperator);
            
            descriptor.AddProperty("firstLevel", FirstLevel);
            
            descriptor.AddProperty("activationDate", ActivationDate);
            
            descriptor.AddProperty("expirationDate", ExpirationDate);
            
            descriptor.AddProperty("ownerId", OwnerId);
            
            descriptor.AddProperty("searchable", Searchable);
            
            descriptor.AddProperty("useCategorySecurityRoles", UseCategorySecurityRoles);
            
            descriptor.AddProperty("securityRoleId", SecurityRoleId);
            
            descriptor.AddProperty("categories", Categories);

            descriptor.AddProperty("filesImported", FilesImported);

            yield return descriptor;
		}

		protected override IEnumerable<ScriptReference> GetScriptReferences()
		{
            yield return new ScriptReference("Gafware.Modules.DMS.AjaxControls.ProgressBar.js", "Gafware.DMS");
		}
	}
}
