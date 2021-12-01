<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PacketView.ascx.cs" Inherits="Gafware.Modules.DMS.PacketView" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/desktopmodules/Gafware/DMS/DocumentSearchResults.ascx" TagPrefix="uc1" TagName="DocumentSearchResults" %>
<div class="dms">
    <uc1:DocumentSearchResults runat="server" id="documentSearchResults" Search="true" IsLink="false" />
</div>
