<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PacketSettings.ascx.cs" Inherits="Gafware.Modules.DMS.PacketSettings" %>
<!-- uncomment the code below to start using the DNN Form pattern to create and update settings -->
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Packet</a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblPacket" runat="server" /> 
        <asp:DropDownList ID="ddlPacket" DataValueField="Name" DataTextField="Name" runat="server"></asp:DropDownList>
        <asp:LinkButton ID="btnReload" runat="server" CssClass="dnnPrimaryAction" Text="Refresh List" OnClick="btnReload_Click" />
    </div>
</fieldset>
