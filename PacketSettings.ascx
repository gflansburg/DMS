<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PacketSettings.ascx.cs" Inherits="Gafware.Modules.DMS.PacketSettings" %>
<!-- uncomment the code below to start using the DNN Form pattern to create and update settings -->
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Packet</a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblRepository" runat="server" ControlName="ddlRepository" Suffix=":" /> 
        <asp:DropDownList ID="ddlRepository" DataValueField="TabModuleId" DataTextField="Name" OnSelectedIndexChanged="ddlRepository_SelectedIndexChanged" runat="server"></asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblPacket" runat="server" /> 
        <asp:DropDownList ID="ddlPacket" DataValueField="Name" DataTextField="Name" runat="server"></asp:DropDownList>
        <asp:LinkButton ID="btnReload" runat="server" CssClass="dnnPrimaryAction" Text="Refresh List" OnClick="btnReload_Click" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblThumbnailType" runat="server" ControlName="ddlThumbnailType" Suffix=":" /> 
        <asp:DropDownList ID="ddlThumbnailType" runat="server">
            <asp:listitem text="Classic" value="classic"></asp:listitem>
            <asp:listitem text="High Contrast" value="high-contrast"></asp:listitem>
            <asp:listitem text="Square" value="square"></asp:listitem>
            <asp:listitem text="Vivid" value="vivid"></asp:listitem>
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblThumbnailSize" runat="server" ControlName="ddlThumbnailSize" Suffix=":" /> 
        <asp:DropDownList ID="ddlThumbnailSize" runat="server">
            <asp:listitem text="Large (128px)" value="128"></asp:listitem>
            <asp:listitem text="Medium (64px)" value="64"></asp:listitem>
            <asp:listitem text="Small (32px)" value="32"></asp:listitem>
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblPageSize" runat="server" ControlName="ddlPageSize" Suffix=":" /> 
        <asp:DropDownList ID="ddlPageSize" runat="server">
            <asp:listitem text="Show All" value="0"></asp:listitem>
            <asp:listitem text="5" value="5"></asp:listitem>
            <asp:listitem text="10" value="10"></asp:listitem>
            <asp:listitem text="20" value="20"></asp:listitem>
            <asp:listitem text="25" value="25"></asp:listitem>
            <asp:listitem text="40" value="40"></asp:listitem>
            <asp:listitem text="50" value="50"></asp:listitem>
            <asp:listitem text="100" value="100"></asp:listitem>
        </asp:DropDownList>
    </div>
</fieldset>
