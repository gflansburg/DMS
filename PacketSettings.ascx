﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PacketSettings.ascx.cs" Inherits="Gafware.Modules.DMS.PacketSettings" %>
<!-- uncomment the code below to start using the DNN Form pattern to create and update settings -->
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Packet</a></h2>
<fieldset>
    <div class="dms" style="padding: 10px;">
        <div class="dnnFormItem" runat="server" id="pnlRepository">
            <dnn:Label ID="lblRepository" runat="server" ControlName="ddlRepository" Suffix=":" /> 
            <asp:DropDownList ID="ddlRepository" DataValueField="TabModuleId" DataTextField="Name" AutoPostBack="true" OnSelectedIndexChanged="ddlRepository_SelectedIndexChanged" style="width: auto;" runat="server"></asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblPacket" runat="server" /> 
            <asp:DropDownList ID="ddlPacket" DataValueField="PacketId" DataTextField="Name" style="width: auto;" runat="server"></asp:DropDownList>
            <asp:LinkButton ID="btnReload" runat="server" CssClass="dnnPrimaryAction" Text="Refresh List" OnClick="btnReload_Click" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblTheme" runat="server" ControlName="ddlTheme" Suffix=":" /> 
            <asp:DropDownList ID="ddlTheme" runat="server" style="width: auto;">
                <asp:listitem text="Red" value="990000"></asp:listitem>
                <asp:listitem text="Green" value="008000"></asp:listitem>
                <asp:listitem text="Blue" value="2170CD"></asp:listitem>
                <asp:listitem text="Yellow" value="C8C800"></asp:listitem>
                <asp:listitem text="Cyan" value="00FFFF"></asp:listitem>
                <asp:listitem text="Magenta" value="FF00FF"></asp:listitem>
                <asp:listitem text="Orange" value="FF8000"></asp:listitem>
                <asp:listitem text="Gray" value="808080"></asp:listitem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblThumbnailType" runat="server" ControlName="ddlThumbnailType" Suffix=":" /> 
            <asp:DropDownList ID="ddlThumbnailType" runat="server" style="width: auto;">
                <asp:listitem text="Classic" value="classic"></asp:listitem>
                <asp:listitem text="High Contrast" value="high-contrast"></asp:listitem>
                <asp:listitem text="Square" value="square"></asp:listitem>
                <asp:listitem text="Vivid" value="vivid"></asp:listitem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblThumbnailSize" runat="server" ControlName="ddlThumbnailSize" Suffix=":" /> 
            <asp:DropDownList ID="ddlThumbnailSize" runat="server" style="width: auto;">
                <asp:listitem text="Large (128px)" value="128"></asp:listitem>
                <asp:listitem text="Medium (64px)" value="64"></asp:listitem>
                <asp:listitem text="Small (32px)" value="32"></asp:listitem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblPageSize" runat="server" ControlName="ddlPageSize" Suffix=":" /> 
            <asp:DropDownList ID="ddlPageSize" runat="server" style="width: auto;">
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
    </div>
</fieldset>

<asp:Panel ID="pnlUpdateSettings" runat="server" Visible="false">
    <hr />
    <div style="float:right;margin-bottom:10px;">
        <asp:linkbutton runat="server" id="updateSettings" causesvalidation="True" CssClass="dnnPrimaryAction" OnClick="updateSettings_Click"><asp:label runat="server" resourcekey="lblUpdateSettings" /></asp:linkbutton>
        <asp:linkbutton runat="server" id="cancelSettings" causesvalidation="False" CssClass="dnnSecondaryAction" OnClick="cancelSettings_Click"><asp:label runat="server" resourcekey="lblCancelSettings" /></asp:linkbutton>
    </div>
</asp:Panel>
