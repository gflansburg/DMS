<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadReport.ascx.cs" Inherits="Gafware.Modules.DMS.UploadReport" %>


<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=15.2.21.1125, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>
<style type="text/css">
    .se-pre-con {
	    position: fixed;
	    left: 0px;
	    top: 0px;
	    width: 100%;
	    height: 100%;
	    z-index: 9999;
	    background-color: #000;
        opacity: 0.6;
    }
</style>
<div class="se-pre-con"></div>
<div style="padding: 10px;">
    <div class="dms" style="width: 900px; padding: 5px;">
        <h3><%=LocalizeString("BasicSettings")%></h3>
        <div id="pnlBack" runat="server" style="margin: 0px 0px 5px 0px;">
            <asp:LinkButton ID="btnBack" runat="server" Text="Back" CssClass="dnnSecondaryAction" OnClick="btnBack_Click" CausesValidation="false" />
        </div>
    </div>
    <div style="border: 1px solid black; padding: 5px; width: 100%; max-width: 912px; height: 712px; overflow: auto;">
        <telerik:reportviewer runat="server" id="report1" width="900px" height="700px"></telerik:reportviewer>
    </div>
</div>