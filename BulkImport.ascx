<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BulkImport.ascx.cs" Inherits="Gafware.Modules.DMS.BulkImport" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<%@ Register Src="~/desktopmodules/Gafware/DMS/DocumentSearchResults.ascx" TagPrefix="uc1" TagName="DocumentSearchResults" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style type="text/css">
    .nocontent {
        display: none;
    }
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
    .dms .dnnFormItem .toggleButton {
        top: -2px !important;
        left: -8px !important;
    }
</style>
<asp:Literal ID="litCSS" runat="server"></asp:Literal>
<div class="se-pre-con"></div>
<div class="dms" style="padding: 10px;">
    <h3><%=LocalizeString("BasicSettings")%></h3>
    <br />
    <asp:LinkButton ID="btnBack" runat="server" Text="Back" CssClass="dnnSecondaryAction" OnClick="btnBack_Click" CausesValidation="false" />
    <br /><br />
    <p><%= LocalizeString("BasicInfo") %></p>
    <br />
    <asp:HiddenField ID="hidFilesImported" runat="server" Value="0" />
    <asp:HiddenField ID="hidFileImportStatus" runat="server" Value="Idle" />
    <asp:HiddenField ID="hidProcessName" runat="server" Value="" />
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblFilePath" runat="server" ControlName="tbFilePath" Suffix=":" /> 
            <asp:TextBox ID="tbFilePath" runat="server" Width="100%" autofocus ValidationGroup="BulkImport"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbFilePath" Display="Dynamic" ErrorMessage="<br />File Path is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="BulkImport"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="valFilePath" runat="server" ErrorMessage="<br />Invalid Folder" ControlToValidate="tbFilePath" Display="Dynamic" CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="BulkImport"></asp:CustomValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSubFolderIsDocumentName" runat="server" ControlName="cbSubFolderIsDocumentName" Suffix=":" /> 
            <div class="toggleButton" id="cbSubFolderIsDocumentNameToggleButton" runat="server">
                <label for='<%= cbSubFolderIsDocumentName.ClientID %>'><asp:CheckBox ID="cbSubFolderIsDocumentName" Checked="false" runat="server" ValidationGroup="BulkImport" /><span></span></label>
            </div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSubFolderIsTag" runat="server" ControlName="cbSubFolderIsTag" Suffix=":" /> 
            <div class="toggleButton" id="cbSubFolderIsTagToggleButton" runat="server">
                <label for='<%= cbSubFolderIsTag.ClientID %>'><asp:CheckBox ID="cbSubFolderIsTag" Checked="false" runat="server" ValidationGroup="BulkImport" AutoPostBack="true" OnCheckedChanged="cbSubFolderIsTag_CheckedChanged" /><span></span></label>
            </div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblPrependSubFolderName" runat="server" ControlName="cbPrependSubFolderName" Suffix=":" /> 
            <div class="toggleButton" id="cbPrependSubFolderNameToggleButton" runat="server">
                <label for='<%= cbPrependSubFolderName.ClientID %>'><asp:CheckBox ID="cbPrependSubFolderName" Checked="false" runat="server" ValidationGroup="BulkImport" AutoPostBack="true" OnCheckedChanged="cbPrependSubFolderName_CheckedChanged" /><span></span></label>
            </div>
        </div>
        <asp:Panel ID="pnlSeperator" runat="server" Visible="false">
            <div class="dnnFormItem">
                <dnn:Label ID="lblSeperator" runat="server" ControlName="lstSeperator" Suffix=":" /> 
                <asp:DropDownList ID="lstSeperator" runat="server" style="width: auto;">
                    <asp:ListItem Value=" - " Text="-" Selected="True" />
                    <asp:ListItem Value=" | " Text="|" />
                    <asp:ListItem Value=", " Text="," />
                    <asp:ListItem Value=": " Text=":" />
                    <asp:ListItem Value="; " Text=";" />
                    <asp:ListItem Value="_" Text="_" />
                </asp:DropDownList>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlLevel" runat="server" Visible="false">
            <div class="dnnFormItem">
                <dnn:Label ID="lblLevel" runat="server" ControlName="lstLevel" Suffix=":" /> 
                <asp:DropDownList ID="lstLevel" runat="server" style="width: auto;">
                    <asp:ListItem Value="0" Text="Root" />
                    <asp:ListItem Value="1" Text="1" />
                    <asp:ListItem Value="2" Text="2" />
                    <asp:ListItem Value="3" Text="3" />
                    <asp:ListItem Value="4" Text="4" />
                    <asp:ListItem Value="5" Text="5" />
                </asp:DropDownList>
            </div>
        </asp:Panel>
        <div class="dnnFormItem">
            <dnn:Label ID="lblReplacePDFTitle" runat="server" ControlName="cbReplacePDFTitle" Suffix=":" /> 
            <div class="toggleButton" id="cbReplacePDFTitleToggleButton" runat="server">
                <label for='<%= cbReplacePDFTitle.ClientID %>'><asp:CheckBox ID="cbReplacePDFTitle" AutoPostBack="false" runat="server" Checked="true" /><span></span></label>
            </div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblActivation" runat="server" ControlName="dtActivation" Suffix=":" /> 
            <telerik:RadDatePicker ID="dtActivation" runat="server" DateInput-EmptyMessage="Activation Date" MinDate="01/01/1000" MaxDate="01/01/3000" ValidationGroup="BulkImport">
                <Calendar ID="Calendar1" runat="server">
                    <SpecialDays>
                        <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                    </SpecialDays>
                </Calendar>
            </telerik:RadDatePicker>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblExpiration" runat="server" ControlName="dtExpiration" Suffix=":" /> 
            <telerik:RadDatePicker ID="dtExpiration" runat="server" DateInput-EmptyMessage="Expiration Date" MinDate="01/01/1000" MaxDate="01/01/3000" ValidationGroup="BulkImport">
                <Calendar ID="Calendar2" runat="server">
                    <SpecialDays>
                        <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                    </SpecialDays>
                </Calendar>
            </telerik:RadDatePicker>
            <asp:CompareValidator ID="CompareValidator1" runat="server" Display="Dynamic" CssClass="red-text" ErrorMessage="<br />Expiration Date must be greater than Activation Date." ValidationGroup="BulkImport" ControlToCompare="dtActivation" ControlToValidate="dtExpiration" Operator="GreaterThanEqual"></asp:CompareValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblOwner" runat="server" ControlName="ddOwner" Suffix=":" /> 
            <asp:DropDownList ID="ddOwner" runat="server" DataTextField="DisplayName" DataValueField="UserId" ValidationGroup="BulkImport" style="width: auto;"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="ddOwner" InitialValue="0" Display="Dynamic" ErrorMessage="<br />Owner is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="BulkImport"></asp:RequiredFieldValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblUseCategorySecurityRoles" runat="server" ControlName="cbUseCategorySecurityRoles" Suffix=":" /> 
            <div class="toggleButton" id="cbUseCategorySecurityRolesToggleButton" runat="server">
                <label for='<%= cbUseCategorySecurityRoles.ClientID %>'><asp:CheckBox ID="cbUseCategorySecurityRoles" Checked="true" AutoPostBack="true" runat="server" OnCheckedChanged="cbUseCategorySecurityRoles_CheckedChanged" /><span></span></label>
            </div>
        </div>
        <asp:Panel ID="pnlSecurityRole" runat="server" Visible="false">
            <div class="dnnFormItem">
                <dnn:Label ID="lblSecurityRole" runat="server" ControlName="ddlSecurityRole" Suffix=":" /> 
                <asp:DropDownList ID="ddlSecurityRole" DataValueField="RoleId" DataTextField="RoleName" runat="server" style="width: auto;" ValidationGroup="BulkImport"></asp:DropDownList>
                <asp:LinkButton ID="btnReloadSecurityRoles" runat="server" CssClass="dnnPrimaryAction" Text="Refresh List" OnClick="btnReloadSecurityRoles_Click" style="padding: 0px 9px 0px 9px; height: 33px;" />
            </div>
        </asp:Panel>
        <asp:Repeater ID="rptCategory" runat="server" OnItemDataBound="rptCategory_ItemDataBound">
            <ItemTemplate>
                <asp:HiddenField ID="hidCategoryId" runat="server" Value='<%# Eval("CategoryId") %>' />
                <div class="dnnFormItem">
                    <div class="dnnLabel"><label id="label"><span id="lblLabel" runat="server"><%# Eval("CategoryName") %>:</span></label></div>
                    <div class="toggleButton" id="cbCategoryToggleButton" runat="server">
                        <label><asp:CheckBox ID="cbCategory" Checked="true" AutoPostBack="false" runat="server" /><span></span></label>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="dnnFormItem">
            <dnn:Label ID="lblIsPublic" runat="server" ControlName="cbIsPublic" Suffix=":" /> 
            <div class="toggleButton" id="cbIsPublicToggleButton" runat="server">
                <label for='<%= cbIsPublic.ClientID %>'><asp:CheckBox ID="cbIsPublic" AutoPostBack="false" runat="server" Checked="true" /><span></span></label>
            </div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblIsSearchable" runat="server" ControlName="cbIsSearchable" Suffix=":" /> 
            <div class="toggleButton" id="cbIsSearchableToggleButton" runat="server">
                <label for='<%= cbIsSearchable.ClientID %>'><asp:CheckBox ID="cbIsSearchable" AutoPostBack="false" runat="server" Checked="true" /><span></span></label>
            </div>
        </div>
        <br style="clear: both" />
    </fieldset>
    <hr />
    <div style="float:right;margin-bottom:10px;">
        <asp:LinkButton ID="btnImport" runat="server" Text="Import" CssClass="dnnPrimaryAction" OnClick="btnImport_Click" ValidationGroup="BulkImport" CausesValidation="true" />
        <asp:LinkButton ID="btnReset" runat="server" Text="Reset Page" CssClass="dnnSecondaryAction" OnClick="btnReset_Click" />
        <asp:LinkButton ID="lnkFinish" runat="server" OnClick="lnkFinish_Click" style="display: none;"></asp:LinkButton>
    </div>
    <br style="clear: both;" />
    <telerik:RadWindow runat="server" Width="400px" Height="160px" VisibleStatusbar="false" ShowContentDuringLoad="false" ID="bulkInsertWindow" Modal="true" Behaviors="None" Title="Importing Documents" ToolTip="Importing Documents" Animation="FlyIn" EnableShadow="True" AnimationDuration="200" Skin="Office2010Blue">
        <ContentTemplate>
            <div align="center" style="margin-top: 20px;">
                <%= LocalizeString("Importing") %><span id="progress">0%</span>
                <br /><br />
                <div style="width: 190px; height: 30px; background-color: #ddd; text-align: left;">
                    <div style="width: 1%; height: 30px;" id="progressBar" runat="server"></div>
                </div>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
</div>
