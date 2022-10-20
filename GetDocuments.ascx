<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GetDocuments.ascx.cs" Inherits="Gafware.Modules.DMS.GetDocuments" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/desktopmodules/Gafware/DMS/DocumentSearchResults.ascx" TagPrefix="uc1" TagName="DocumentSearchResults" %>
<asp:Literal ID="litCSS" runat="server"></asp:Literal>
<div class="dms">
    <h3><%=LocalizeString("BasicSettings")%></h3>
    <uc1:DocumentSearchResults runat="server" id="documentSearchResults" Search="true" TargetNew="false" />
</div>