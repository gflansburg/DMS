<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BulkImport.ascx.cs" Inherits="Gafware.Modules.DMS.BulkImport" %>
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
</style>
<asp:Literal ID="litCSS" runat="server"></asp:Literal>
<div class="se-pre-con"></div>
<div class="dms" style="padding: 10px;">
    <h3><%=LocalizeString("BasicSettings")%></h3>
    <asp:LinkButton ID="btnBack" runat="server" Text="Back" CssClass="dnnSecondaryAction" OnClick="btnBack_Click" CausesValidation="false" />
    <p>Physical path to files must be on the local server or file share with applicable "read" permissions for the Identity used in the DNN App Pool.</p>
    <span class="FormText">
        <span class="RequiredField">*</span> Required field.
    </span>
    <br />
    <br />
    <asp:HiddenField ID="hidFilesImported" runat="server" Value="0" />
    <asp:HiddenField ID="hidFileImportComplete" runat="server" Value="false" />
    <div class="RecordDisplay">
        <label for="<%= tbFilePath.ClientID %>">
            <span class="FieldName"><span class="RequiredField">*</span> Folder:</span>
            <span class="FieldValue">
                <asp:TextBox ID="tbFilePath" runat="server" Width="790px" ValidationGroup="BulkImport"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbFilePath" Display="Dynamic" ErrorMessage="<br />File Path is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="BulkImport"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="valFilePath" runat="server" ErrorMessage="<br />Invalid Folder" ControlToValidate="tbFilePath" Display="Dynamic" CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="BulkImport"></asp:CustomValidator>
            </span>
        </label>
    </div>
    <br style="clear: both" />
    <div class="RecordDisplay">
        <label for="<%= cbSubFolderIsDocumentName.ClientID %>">
            <span class="FieldName"><span class="RequiredField">*</span> Folder/Sub-Folder(s) is/are Document Name(s):</span>
            <span class="FieldValue">
                <%--<asp:CheckBox ID="cbSubFolderIsDocumentName" runat="server" Checked="false" />--%>
                <div class="toggleButton" id="cbSubFolderIsDocumentNameToggleButton" runat="server" style="width: 120px">
                    <label><asp:CheckBox ID="cbSubFolderIsDocumentName" Checked="false" runat="server" ValidationGroup="BulkImport" /><span></span></label>
                </div>
            </span>
        </label>
    </div>
    <br style="clear: both" />
    <div class="RecordDisplay">
        <label for="<%= cbSubFolderIsTag.ClientID %>">
            <span class="FieldName"><span class="RequiredField">*</span> Folder/Sub-Folder(s) is/are Tag(s):</span>
            <span class="FieldValue">
                <%--<asp:CheckBox ID="cbSubFolderIsTag" runat="server" Checked="false" />--%>
                <div class="toggleButton" id="cbSubFolderIsTagToggleButton" runat="server" style="width: 120px">
                    <label><asp:CheckBox ID="cbSubFolderIsTag" Checked="false" runat="server" ValidationGroup="BulkImport" AutoPostBack="true" OnCheckedChanged="cbSubFolderIsTag_CheckedChanged" /><span></span></label>
                </div>
            </span>
        </label>
    </div>
    <br style="clear: both" />
    <div class="RecordDisplay">
        <label for="<%= cbPrependSubFolderName.ClientID %>">
            <span class="FieldName"><span class="RequiredField">*</span> Prepend Folder/Sub-Folder Name:</span>
            <span class="FieldValue">
                <%--<asp:CheckBox ID="cbPrependSubFolderName" runat="server" Checked="false" />--%>
                <div class="toggleButton" id="cbPrependSubFolderNameToggleButton" runat="server" style="width: 120px">
                    <label><asp:CheckBox ID="cbPrependSubFolderName" Checked="false" runat="server" ValidationGroup="BulkImport" AutoPostBack="true" OnCheckedChanged="cbPrependSubFolderName_CheckedChanged" /><span></span></label>
                </div>
            </span>
        </label>
    </div>
    <br style="clear: both" />
    <asp:Panel ID="pnlSeperator" runat="server" Visible="false">
        <div class="RecordDisplay">
            <label for="<%= lstSeperator.ClientID %>">
                <span class="FieldName"><span class="RequiredField">*</span> Seperator:</span>
                <span class="FieldValue">
                    <asp:DropDownList ID="lstSeperator" runat="server">
                        <asp:ListItem Value=" - " Text="-" Selected="True" />
                        <asp:ListItem Value=" | " Text="|" />
                        <asp:ListItem Value=", " Text="," />
                        <asp:ListItem Value=": " Text=":" />
                        <asp:ListItem Value="; " Text=";" />
                        <asp:ListItem Value="_" Text="_" />
                    </asp:DropDownList>
                </span>
            </label>
        </div>
        <br style="clear: both" />
    </asp:Panel>
    <asp:Panel ID="pnlLevel" runat="server" Visible="false">
        <div class="RecordDisplay">
            <label for="<%= lstLevel.ClientID %>">
                <span class="FieldName"><span class="RequiredField">*</span> Start Level:</span>
                <span class="FieldValue">
                    <asp:DropDownList ID="lstLevel" runat="server">
                        <asp:ListItem Value="0" Text="Root" />
                        <asp:ListItem Value="1" Text="1" />
                        <asp:ListItem Value="2" Text="2" />
                        <asp:ListItem Value="3" Text="3" />
                        <asp:ListItem Value="4" Text="4" />
                        <asp:ListItem Value="5" Text="5" />
                    </asp:DropDownList>
                </span>
            </label>
        </div>
        <br style="clear: both" />
    </asp:Panel>
    <div class="RecordDisplay">
        <label for="<%= dtActivation.ClientID %>">
            <span class="FieldName">Activation Date</span>
            <span class="FieldValue" style="position: relative; top: -5px;">
                <telerik:RadDatePicker ID="dtActivation" runat="server" Width="140px" DateInput-EmptyMessage="Activation Date" MinDate="01/01/1000" MaxDate="01/01/3000" ValidationGroup="BulkImport">
                    <Calendar ID="Calendar1" runat="server">
                        <SpecialDays>
                            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                        </SpecialDays>
                    </Calendar>
                </telerik:RadDatePicker>
            </span>
        </label>
    </div>
    <br style="clear: both" />
    <div class="RecordDisplay">
        <label for="<%= dtExpiration.ClientID %>">
            <span class="FieldName">Expiration Date</span>
            <span class="FieldValue" style="position: relative; top: -5px;">
                <telerik:RadDatePicker ID="dtExpiration" runat="server" Width="140px" DateInput-EmptyMessage="Expiration Date" MinDate="01/01/1000" MaxDate="01/01/3000" ValidationGroup="BulkImport">
                    <Calendar ID="Calendar2" runat="server">
                        <SpecialDays>
                            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                        </SpecialDays>
                    </Calendar>
                </telerik:RadDatePicker>
                <asp:CompareValidator ID="CompareValidator1" runat="server" Display="Dynamic" CssClass="red-text" ErrorMessage="<br />Expiration Date must be greater than Activation Date." ValidationGroup="BulkImport" ControlToCompare="dtActivation" ControlToValidate="dtExpiration" Operator="GreaterThanEqual"></asp:CompareValidator>
            </span>
        </label>
    </div>
    <br style="clear: both" />
    <div class="RecordDisplay">
        <label for="<%= dtExpiration.ClientID %>">
            <span class="FieldName"><span class="RequiredField">*</span> Owner</span>
            <span class="FieldValue">
                <asp:DropDownList ID="ddOwner" runat="server" DataTextField="DisplayName" DataValueField="UserId" ValidationGroup="BulkImport"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="ddOwner" InitialValue="0" Display="Dynamic" ErrorMessage="<br />Owner is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="BulkImport"></asp:RequiredFieldValidator>
            </span>
        </label>
    </div>
    <br style="clear: both" />
    <div class="RecordDisplay">
        <label for="<%= cbUseCategorySecurityRoles.ClientID %>">
            <span class="FieldName"><span class="RequiredField">*</span> Use Category Security Roles</span>
            <span class="FieldValue">
                <div class="toggleButton" id="cbUseCategorySecurityRolesToggleButton" runat="server" style="width: 120px">
                    <label for='<%= cbUseCategorySecurityRoles.ClientID %>'><asp:CheckBox ID="cbUseCategorySecurityRoles" Checked="true" AutoPostBack="true" runat="server" OnCheckedChanged="cbUseCategorySecurityRoles_CheckedChanged" /><span></span></label>
                </div>
            </span>
        </label>
    </div>
    <br style="clear: both" />
    <asp:Panel ID="pnlSecurityRole" runat="server" Visible="false">
        <div class="RecordDisplay">
            <label for="<%= cbUseCategorySecurityRoles.ClientID %>">
                <span class="FieldName"><span class="RequiredField">*</span> Required Role</span>
                <span class="FieldValue">
                    <asp:DropDownList ID="ddlSecurityRole" DataValueField="RoleId" DataTextField="RoleName" runat="server" ValidationGroup="BulkImport"></asp:DropDownList>
                    <asp:LinkButton ID="btnReloadSecurityRoles" runat="server" CssClass="dnnPrimaryAction" Text="Refresh List" OnClick="btnReloadSecurityRoles_Click" style="padding: 0px 9px 0px 9px; height: 33px;" />
                </span>
            </label>
        </div>
        <br style="clear: both" />
    </asp:Panel>
    <asp:Repeater ID="rptCategory" runat="server" OnItemDataBound="rptCategory_ItemDataBound">
        <ItemTemplate>
            <asp:HiddenField ID="hidCategoryId" runat="server" Value='<%# Eval("CategoryId") %>' />
            <div class="RecordDisplay">
                <span class="FieldName"><span class="RequiredField">*</span> <%# Eval("CategoryName") %></span>
                <span class="FieldValue">
                    <div class="toggleButton" id="cbCategoryToggleButton" runat="server" style="width: 120px">
                        <label><asp:CheckBox ID="cbCategory" Checked="true" AutoPostBack="false" runat="server" /><span></span></label>
                    </div>
                </span>
            </div>
            <br style="clear: both;" />
        </ItemTemplate>
    </asp:Repeater>
    <div class="RecordDisplay">
        <label for="<%= cbIsSearchable.ClientID %>">
            <span class="FieldName"><span class="RequiredField">*</span> Searchable</span>
            <span class="FieldValue">
                <div class="toggleButton" id="cbIsSearchableToggleButton" runat="server" style="width: 120px">
                    <label for='<%= cbIsSearchable.ClientID %>'><asp:CheckBox ID="cbIsSearchable" AutoPostBack="false" runat="server" Checked="true" /><span></span></label>
                </div>
            </span>
        </label>
    </div>
    <br style="clear: both" />
    <asp:LinkButton ID="btnImport" runat="server" Text="Import" CssClass="dnnPrimaryAction" OnClick="btnImport_Click" ValidationGroup="BulkImport" CausesValidation="true" />
    <asp:LinkButton ID="btnReset" runat="server" Text="Reset Page" CssClass="dnnSecondaryAction" OnClick="btnReset_Click" />
</div>
