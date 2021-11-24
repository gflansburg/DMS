<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentList.ascx.cs" Inherits="Gafware.Modules.DMS.DocumentList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/desktopmodules/Gafware/DMS/LetterFilter.ascx" TagPrefix="uc1" TagName="LetterFilter" %>
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
    .nocontent {
        display: none;
    }
</style>
<div class="se-pre-con"></div>
<div class="dms" style="padding: 10px;">
    <asp:Panel ID="pnlDetails" runat="server" style="display: none">
        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <h3>The document you are trying to retrieve is no longer available or you do not have sufficient privileges.</h3><br />
            If you reached this page through a bookmarked link or from another website, please use our document search to find a new version of the document you requested.
        </asp:Panel>
        <asp:Panel ID="pnlFound" runat="server">
            <h2><asp:Label ID="lblDocumentName" runat="server" /></h2>
            <div id="pnlBack" runat="server" style="float: left; text-align: right; margin: 0px 0px 5px 0px;">
                <asp:LinkButton ID="btnBack" runat="server" Text="Back" CssClass="dnnSecondaryAction" OnClick="btnBack_Click" CausesValidation="false" />
            </div>
            <div id="pnlControl" runat="server" style="float: right; text-align: right; margin: 0px 1px 5px 0px;">
                <asp:Panel ID="pnlDetails3" runat="server">
                    <asp:LinkButton ID="btnEdit" runat="server" Text="Edit Document" OnClick="btnEdit_Click" CausesValidation="false" CssClass="dnnPrimaryAction" /> <asp:LinkButton ID="btnDelete" runat="server" Text="Delete Document" CssClass="dnnSecondaryAction" OnClientClick="return confirm('Are you sure you wish to delete this document?');" OnClick="btnDelete_Click" CausesValidation="false" />
                </asp:Panel>
                <asp:Panel ID="pnlEdit" runat="server" Visible="false">
                    <asp:LinkButton ID="btnSave" runat="server" Text="Save Document" OnClick="btnSave_Click" CausesValidation="true" CssClass="dnnPrimaryAction" ValidationGroup="DocumentControl" /> <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CssClass="dnnSecondaryAction" OnClick="btnCancel_Click" CausesValidation="false" />
                </asp:Panel>
            </div>
            <br style="clear: both;" />
            <div class="RecordDisplay">
                <span class="FieldName">Document ID</span>
                <span class="FieldValue FieldValueSpan">
                    <asp:Label ID="lblDocumentID" runat="server">&nbsp;</asp:Label>
                </span>
            </div>
            <br style="clear: both;" />
            <div class="RecordDisplay">
                <asp:Panel ID="pnlOwnerDetails" runat="server">
                    <span class="FieldName">Owner</span>
                    <span class="FieldValue FieldValueSpan">
                        <asp:Label ID="lblOwner" runat="server">&nbsp;</asp:Label>
                    </span>
                </asp:Panel>
                <asp:Panel ID="pnlOwnerEdit" runat="server" Visible="false">
                    <span class="FieldName"><span class="RequiredField">*</span> Owner</span>
                    <span class="FieldValue">
                        <asp:DropDownList ID="ddOwner2" runat="server" DataTextField="DisplayName" DataValueField="UserId" ValidationGroup="DocumentControl"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="ddOwner2" InitialValue="0" Display="Dynamic" ErrorMessage="<br />Owner is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="DocumentControl"></asp:RequiredFieldValidator>
                    </span>
                </asp:Panel>
            </div>
            <br style="clear: both;" />
            <asp:Panel ID="pnlDocumentNameEdit" runat="server" Visible="false">
                <div class="RecordDisplay">
                    <span class="FieldName"><span class="RequiredField">*</span> Document Name</span>
                    <span class="FieldValue">
                        <asp:TextBox ID="tbDocumentName" runat="server" Width="790px" MaxLength="255"  ValidationGroup="DocumentControl"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbDocumentName" Display="Dynamic" ErrorMessage="<br />Document Name is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="DocumentControl"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="valExists" runat="server" ErrorMessage="<br />A document with this name already exists!" Display="Dynamic" CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="DocumentControl"></asp:CustomValidator>
                    </span>
                </div>
                <br style="clear: both;" />
            </asp:Panel>
            <div class="RecordDisplay">
                <asp:Panel ID="pnlDocumentDetailsDetails" runat="server">
                    <span class="FieldName">Document Details</span>
                    <span class="FieldValue FieldValueSpan">
                        <asp:Label ID="lblDetails" runat="server">&nbsp;</asp:Label>
                    </span>
                </asp:Panel>
                <asp:Panel ID="pnlDocumentDetailsEdit" runat="server" Visible="false">
                    <span class="FieldName"><span class="RequiredField">*</span> Document Details</span>
                    <span class="FieldValue">
                        <asp:TextBox ID="tbDocumentDetails" runat="server" TextMode="MultiLine" MaxLength="4000" Rows="1" Width="790px"  ValidationGroup="DocumentControl"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbDocumentDetails" Display="Dynamic" ErrorMessage="<br />Document Details is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="DocumentControl"></asp:RequiredFieldValidator>
                    </span>
                </asp:Panel>
            </div>
            <br style="clear: both;" />
            <div class="RecordDisplay">
                <asp:Panel ID="pnlAdminCommentsDetails" runat="server">
                    <span class="FieldName">Admin Comments</span>
                    <span class="FieldValue FieldValueSpan">
                        <asp:Label ID="lblAdminComments" runat="server">&nbsp;</asp:Label>
                    </span>
                </asp:Panel>
                <asp:Panel ID="pnlAdminCommentsEdit" runat="server" Visible="false">
                    <span class="FieldName">Admin Comments</span>
                    <span class="FieldValue">
                        <asp:TextBox ID="tbAdminComments" runat="server" TextMode="MultiLine" MaxLength="2500" Rows="1" Width="790px"  ValidationGroup="DocumentControl"></asp:TextBox>
                    </span>
                </asp:Panel>
            </div>
            <br style="clear: both;" />
<%--            <div class="RecordDisplay">
                <asp:Panel ID="pnlManagerToolkitDetails" runat="server">
                    <span class="FieldName">Manager Toolkit</span>
                    <span class="FieldValue FieldValueSpan">
                        <asp:Label ID="lblManagerToolkit" runat="server">&nbsp;</asp:Label>
                    </span>
                </asp:Panel>
                <asp:Panel ID="pnlManagerToolkitEdit" runat="server" Visible="false">
                    <span class="FieldName"><span class="RequiredField">*</span> Manager Toolkit</span>
                    <span class="FieldValue">
                        <div class="toggleButton" id="cbManagerToolkitToggleButton" runat="server" style="width: 120px">
                            <label for='<%= cbManagerToolkit.ClientID %>'><asp:CheckBox ID="cbManagerToolkit" AutoPostBack="false" runat="server" /><span></span></label>
                        </div>
                    </span>
                </asp:Panel>
            </div>
            <br style="clear: both;" />--%>
            <div class="RecordDisplay" style="position: relative;">
                <asp:Panel ID="pnlActivationDateDetails" runat="server">
                    <span class="FieldName">Activation Date</span>
                    <span class="FieldValue FieldValueSpan">
                        <asp:Label ID="lblActivationDate" runat="server">&nbsp;</asp:Label>
                    </span>
                </asp:Panel>
                <asp:Panel ID="pnlActivationDateEdit" runat="server" Visible="false">
                    <span class="FieldName">Activation Date</span>
                    <span class="FieldValue" style="position: relative; top: -5px;">
                        <telerik:RadDatePicker ID="dtActivation" runat="server" Width="140px" DateInput-EmptyMessage="Activation Date" MinDate="01/01/1000" MaxDate="01/01/3000" ValidationGroup="DocumentControl">
                            <Calendar ID="Calendar1" runat="server">
                                <SpecialDays>
                                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                                </SpecialDays>
                            </Calendar>
                        </telerik:RadDatePicker>
                    </span>
                </asp:Panel>
            </div>
            <br style="clear: both;" />
            <div class="RecordDisplay" style="position: relative;">
                <asp:Panel ID="pnlExpirationDateDetails" runat="server">
                    <span class="FieldName">Expiration Date</span>
                    <span class="FieldValue FieldValueSpan">
                        <asp:Label ID="lblExpirationDate" runat="server">&nbsp;</asp:Label>
                    </span>
                </asp:Panel>
                <asp:Panel ID="pnlExpirationDateEdit" runat="server" Visible="false">
                    <span class="FieldName">Expiration Date</span>
                    <span class="FieldValue" style="position: relative; top: -5px;">
                        <telerik:RadDatePicker ID="dtExpiration" runat="server" Width="140px" DateInput-EmptyMessage="Expiration Date" MinDate="01/01/1000" MaxDate="01/01/3000" ValidationGroup="DocumentControl">
                            <Calendar ID="Calendar2" runat="server">
                                <SpecialDays>
                                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                                </SpecialDays>
                            </Calendar>
                        </telerik:RadDatePicker>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" Display="Dynamic" CssClass="red-text" ErrorMessage="<br />Expiration Date must be greater than Activation Date." ValidationGroup="DocumentControl" ControlToCompare="dtActivation" ControlToValidate="dtExpiration" Operator="GreaterThanEqual"></asp:CompareValidator>
                    </span>
                </asp:Panel>
            </div>
            <br style="clear: both;" />
            <div class="RecordDisplay">
                <asp:Panel ID="pnlUseCategorySecurityRolesDetails" runat="server">
                    <span class="FieldName">Use Category Security Roles</span>
                    <span class="FieldValue FieldValueSpan">
                        <asp:Label ID="lblUseCategorySecurityRoles" runat="server">&nbsp;</asp:Label>
                    </span>
                </asp:Panel>
                <asp:Panel ID="pnlUseCategorySecurityRolesEdit" runat="server" Visible="false">
                    <span class="FieldName"><span class="RequiredField">*</span> Use Category Security Roles</span>
                    <span class="FieldValue">
                        <div class="toggleButton" id="cbUseCategorySecurityRolesToggleButton" runat="server" style="width: 120px">
                            <label for='<%= cbUseCategorySecurityRoles.ClientID %>'><asp:CheckBox ID="cbUseCategorySecurityRoles" AutoPostBack="true" runat="server" OnCheckedChanged="cbUseCategorySecurityRoles_CheckedChanged" /><span></span></label>
                        </div>
                    </span>
                </asp:Panel>
            </div>
            <br style="clear: both;" />

            <asp:Panel ID="pnlSecurityRole" runat="server" Visible="false">
                <div class="RecordDisplay">
                    <asp:Panel ID="pnlSecurityRoleDetails" runat="server">
                        <span class="FieldName">Required Role</span>
                        <span class="FieldValue FieldValueSpan">
                            <asp:Label ID="lblSecurityRole" runat="server">&nbsp;</asp:Label>
                        </span>
                    </asp:Panel>
                    <asp:Panel ID="pnlSecurityRoleEdit" runat="server" Visible="false">
                        <span class="FieldName"><span class="RequiredField">*</span> Required Role</span>
                        <span class="FieldValue">
                            <asp:DropDownList ID="ddlSecurityRole" DataValueField="RoleId" DataTextField="RoleName" runat="server" ValidationGroup="DocumentControl"></asp:DropDownList>
                            <asp:LinkButton ID="btnReloadSecurityRoles" runat="server" CssClass="dnnPrimaryAction" Text="Refresh List" OnClick="btnReloadSecurityRoles_Click" style="padding: 0px 9px 0px 9px; height: 33px;" />
                        </span>
                    </asp:Panel>
                </div>
                <br style="clear: both;" />
            </asp:Panel>
            <asp:Repeater ID="rptCategory" runat="server" OnItemDataBound="rptCategory_ItemDataBound">
                <ItemTemplate>
                    <asp:HiddenField ID="hidCategoryId" runat="server" Value='<%# Eval("CategoryId") %>' />
                    <div class="RecordDisplay">
                        <asp:Panel ID="pnlCategoryDetails" runat="server" Visible="<%# ViewMode == Gafware.Modules.DMS.ViewMode.Details %>">
                            <span class="FieldName"><%# Eval("CategoryName") %></span>
                            <span class="FieldValue FieldValueSpan">
                                <asp:Label ID="lblCategory" runat="server">No</asp:Label>
                            </span>
                        </asp:Panel>
                        <asp:Panel ID="pnlCategoryEdit" runat="server" Visible="<%# ViewMode == Gafware.Modules.DMS.ViewMode.Edit %>">
                            <span class="FieldName"><span class="RequiredField">*</span> <%# Eval("CategoryName") %></span>
                            <span class="FieldValue">
                                <div class="toggleButton" id="cbCategoryToggleButton" runat="server" style="width: 120px">
                                    <label><asp:CheckBox ID="cbCategory" AutoPostBack="false" runat="server" /><span></span></label>
                                </div>
                            </span>
                        </asp:Panel>
                    </div>
                    <br style="clear: both;" />
                </ItemTemplate>
            </asp:Repeater>
            <div class="RecordDisplay">
                <asp:Panel ID="pnlSearchableDetails" runat="server">
                    <span class="FieldName">Searchable</span>
                    <span class="FieldValue FieldValueSpan">
                        <asp:Label ID="lblIsSearchable" runat="server">&nbsp;</asp:Label>
                    </span>
                </asp:Panel>
                <asp:Panel ID="pnlSearchableEdit" runat="server" Visible="false">
                    <span class="FieldName"><span class="RequiredField">*</span> Searchable</span>
                    <span class="FieldValue">
                        <div class="toggleButton" id="cbIsSearchableToggleButton" runat="server" style="width: 120px">
                            <label for='<%= cbIsSearchable.ClientID %>'><asp:CheckBox ID="cbIsSearchable" AutoPostBack="false" runat="server" /><span></span></label>
                        </div>
                    </span>
                </asp:Panel>
            </div>
            <br style="clear: both;" />
            <asp:Panel ID="pnlDetails2" runat="server">
                <div class="RecordDisplay">
                    <span class="FieldName">Date Created</span>
                    <span class="FieldValue FieldValueSpan">
                        <asp:Label ID="lblDateCreated" runat="server">&nbsp;</asp:Label>
                    </span>
                </div>
                <br style="clear: both;" />
                <div class="RecordDisplay">
                    <span class="FieldName">Date Last Modified</span>
                    <span class="FieldValue FieldValueSpan">
                        <asp:Label ID="lblDateLastModified" runat="server">&nbsp;</asp:Label>
                    </span>
                </div>
                <br style="clear: both;" />
        <%--        <div class="RecordDisplay">
                    <span class="FieldName">IP Address</span>
                    <span class="FieldValue FieldValueSpan">
                        <asp:Label ID="lblIPAddress" runat="server">&nbsp;</asp:Label>
                    </span>
                </div>
                <br style="clear: both;" />--%>
            </asp:Panel>
        </asp:Panel>
        <br style="clear: both;" />
        <div id="pnlBack2" runat="server" style="float: left; text-align: right; margin: 0px 0px 5px 0px;" visible="false">
            <asp:LinkButton ID="btnBack2" runat="server" Text="Back" CssClass="dnnSecondaryAction" OnClick="btnBack_Click" CausesValidation="false" />
        </div>
        <div id="pnlControl2" runat="server" style="float: right; text-align: right; margin: 0px 1px 5px 0px;" visible="false">
            <asp:LinkButton ID="btnSave2" runat="server" Text="Save Document" CssClass="dnnPrimaryAction" OnClick="btnSave_Click" CausesValidation="true" ValidationGroup="DocumentControl" /> <asp:LinkButton ID="btnCancel2" runat="server" Text="Cancel" CssClass="dnnSecondaryAction" OnClick="btnCancel_Click" CausesValidation="false" />
        </div>
        <br style="clear: both; line-height: 0px; height: 0px;" />
        <asp:Panel ID="pnlPackets" runat="server">
            <div style="margin: 5px 0px 5px 4px;">
                <span style="font-size: 12pt; font-weight: bold;">Packet Listings</span>
            </div>
            <asp:GridView ID="gvPackets" runat="server" AutoGenerateColumns="False" CellPadding="3" CellSpacing="3"  EmptyDataText="<br /><strong>No packets found.</strong>" 
                ForeColor="Black" GridLines="None" DataKeyNames="PacketId" BackColor="White" BorderColor="#DEDFDE" PageSize="20" BorderStyle="None" BorderWidth="1px" 
                AllowPaging="False" AllowSorting="False" Width="100%" ShowHeader="True" CssClass="filesList" OnRowDataBound="gvPackets_RowDataBound">
		        <Columns>
                    <asp:TemplateField HeaderText="ID" HeaderStyle-Wrap="false" SortExpression="DocumentID" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="50px">
                        <ItemTemplate>
                            <%# Eval("PacketID") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Packet Name" HeaderStyle-Wrap="false" SortExpression="Name">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkPacketView" CommandArgument='<%# Eval("PacketID") %>' CommandName="EditPacket" OnCommand="lnkPacketView_Command" runat="server"><%# Eval("Name") %></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Comments" HeaderStyle-Wrap="false" SortExpression="AdminComments" ItemStyle-Width="300px">
                        <ItemTemplate>
                            <%# GetComments(Eval("AdminComments").ToString()) %>
                        </ItemTemplate>
                    </asp:TemplateField>
		        </Columns>
		        <FooterStyle BackColor="White" />
		        <RowStyle BackColor="#F7F7F7" VerticalAlign="Top" Font-Names="Arial" Font-Size="14px" />
		        <EditRowStyle VerticalAlign="Top" />
		        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" VerticalAlign="Top" />
		        <HeaderStyle BackColor="#666666" Font-Bold="False" ForeColor="White" HorizontalAlign="Left" Font-Size="10pt" VerticalAlign="Top" Font-Underline="false" />
		        <AlternatingRowStyle BackColor="White" VerticalAlign="Top" />
	        </asp:GridView>
            <br style="clear: both;" />
        </asp:Panel>
        <asp:Panel ID="pnlTags" runat="server">
            <div style="margin: 5px 0px 5px 0px;">
                <span style="font-size: 12pt; font-weight: bold;">Tags</span>
            </div>
            <div style="background-color:#CCC; padding: 5px; margin: 0 0 5px 0px; width: 99%; position: relative">	
                <strong>New Document Tag:</strong>
                <asp:DropDownList ID="ddTags" DataTextField="TagName" DataValueField="TagID" CssClass="offset" runat="server" ValidationGroup="DocumentTags"></asp:DropDownList>
                <strong>OR</strong> 
                <telerik:RadTextBox ID="tbTag" ValidationGroup="DocumentTags" runat="server" EmptyMessage="Create New Tag" Style="top: 3px; position: relative"></telerik:RadTextBox>
                <asp:LinkButton ID="btnAddTag" runat="server" Text="Add Tag" ValidationGroup="DocumentTags" CssClass="dnnSecondaryAction" OnClick="btnAddTag_Click" style="top: -1px; position: relative; left: 40px;" />
                <span ID="pnlSaveMessage" runat="server" CssClass="statusMessage" style="color: red; font-weight:bold; display: none;">Saved</span>
                <asp:HiddenField ID="hidTagToRemove" runat="server" /><asp:LinkButton ID="lnkRemoveTag" runat="server" style="display: none;" OnClick="lnkRemoveTag_Click" />
            </div>
            <br style="clear: both; line-height: 0px;" />
            <telerik:RadAutoCompleteBox ID="tbTags" runat="server" Delimiter=",;" Width="100%" DataTextField="TagName" DataValueField="TagName" Filter="StartsWith" AllowCustomEntry="True" OnEntryRemoved="tbTags_EntryRemoved" OnEntryAdded="tbTags_EntryAdded"></telerik:RadAutoCompleteBox>
            <br style="clear: both;" />
        </asp:Panel>
        <asp:Panel ID="pnlFiles" runat="server">
            <div style="float: left; text-align: left; margin: 5px 0px 0px 4px;">
                <span style="font-size: 12pt; font-weight: bold;">Files</span>
            </div>
            <div style="float: right; text-align: right; margin: 0px 1px 5px 0px;">
                <a id="btnNewFile" runat="server" class="dnnSecondaryAction">New Files</a> <a id="btnNewLink" runat="server" class="dnnSecondaryAction">New Link</a>
            </div>
            <br style="clear: both; line-height: 0px;" />
            <asp:GridView ID="gvFiles" runat="server" AutoGenerateColumns="False" CellPadding="3" CellSpacing="3"  EmptyDataText="<br /><strong>No files found.</strong>" ForeColor="Black" 
                GridLines="None" DataKeyNames="FileId" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" AllowPaging="False" AllowSorting="False" Width="100%"
                ShowHeader="True" OnRowDeleting="gvFiles_RowDeleting" OnRowDataBound="gvFiles_RowDataBound" CssClass="filesList">
	            <Columns>
                    <asp:TemplateField ItemStyle-VerticalAlign="Middle" ItemStyle-Width="84px" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:ImageButton ID="btnDelete" CommandName="Delete" runat="server" Style="padding-top: 5px;" ImageUrl="~/desktopmodules/Gafware/DMS/images/icons/DeleteIcon1.gif" onmouseout="MM_swapImgRestore()" OnClientClick='<%# "return confirm(\"Are you sure you wish to delete this " + (Eval("FileType").ToString().Equals("url", StringComparison.OrdinalIgnoreCase) ?  "link" : "file") + "?\");" %>' />
                            <asp:ImageButton ID="historyButton" CommandName="History" CssClass="history" CommandArgument='<%# Eval("FileId") %>' OnCommand="historyButton_Command" runat="server" Style="padding-top: 5px;" ImageUrl="~/desktopmodules/Gafware/DMS/images/icons/HistoryIcon1.gif" onmouseout="MM_swapImgRestore()" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-CssClass="itemStatus" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="120px" HeaderText="Status" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <div class="toggleButton" id="cbActiveToggleButton" runat="server" style="width: 120px" data-uid='<%# Eval("FileId") %>'>
                                <label><asp:CheckBox ID="cbActive" Checked='<%# ((int)Eval("StatusId")) == 1 %>' AutoPostBack="false" runat="server" onclick='<%# "toggleStatus(this," + Eval("FileId").ToString() + ")" %>' /><span></span></label>
                            </div>
                            <asp:Panel ID="pnlSaveMessage" runat="server" CssClass="statusMessage" style="color: red; font-weight:bold; display: none;" data-uid='<%# Eval("FileId") %>'>Saved</asp:Panel>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-Wrap="true" SortExpression="FileName" ItemStyle-VerticalAlign="Middle" ItemStyle-CssClass="dont-break-out">
                        <ItemTemplate>
                            <%# GetUrl(Container.DataItem) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Version" HeaderStyle-Wrap="false" SortExpression="Version" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                        <ItemTemplate>
                            <%# GetVersion(Container.DataItem) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="File Type" HeaderStyle-Wrap="false" SortExpression="FileType" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                        <ItemTemplate>
                            <%# Eval("FileType") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="File Size" HeaderStyle-Wrap="false" SortExpression="Filesize" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                        <ItemTemplate>
                            <%# GetFilesize(Container.DataItem) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Uploader" HeaderStyle-Wrap="false" SortExpression="CreatedByUser.DisplayName" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle">
                        <ItemTemplate>
                            <%# Eval("CreatedByUser.DisplayName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Date Uploaded" HeaderStyle-Wrap="false" SortExpression="CreatedOnDate" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                        <ItemTemplate>
                            <%# ((DateTime)Eval("CreatedOnDate")).ToString("MM/dd/yyyy") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Download" HeaderStyle-Wrap="false" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                        <ItemTemplate>
                            <a href="<%# ResolveUrl("~/desktopmodules/Gafware/DMS/GetFile.ashx") %>?id=<%# Eval("FileId") %>" title="Download <%# System.IO.Path.GetFileName(Eval("Filename").ToString()) %>" target="_blank" style="color: #<%# Theme %>;"><%# Eval("FileType").ToString().Equals("url") ? String.Empty : "Download" %></a>
                        </ItemTemplate>
                    </asp:TemplateField>
	            </Columns>
	            <FooterStyle BackColor="White" />
	            <RowStyle BackColor="#C0C0C0" VerticalAlign="Top" />
	            <EditRowStyle VerticalAlign="Top" />
	            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" VerticalAlign="Top" />
	            <HeaderStyle BackColor="#666666" Font-Bold="False" ForeColor="White" HorizontalAlign="Left" Font-Size="10pt" VerticalAlign="Top" Font-Underline="false" />
	            <AlternatingRowStyle BackColor="White" VerticalAlign="Top" />
            </asp:GridView>
        </asp:Panel>
        <asp:Panel ID="pnlLink" runat="server" Visible="false">
            <br />
            <p><strong>Link URL</strong> <span>Use this link to display the document above.</span></p>
            <asp:TextBox ID="tbLinkURL" TextMode="MultiLine" ReadOnly="true" Width="98%" Rows="3" runat="server"></asp:TextBox>
        </asp:Panel>
        <asp:HiddenField ID="history" runat="server" Value="0" />
        <asp:LinkButton ID="lnkReload" runat="server" CausesValidation="false" style="display: none;" OnClick="lnkReload_Click">Reload</asp:LinkButton>
        <div id="newHyperlinkDialog" class="nocontent dms">
            <div id="newHyperlink-content" class="dialog-content">
                <div class="body_padding">
                    <div class="RecordDisplay">
                        <span class="FieldNamePopup">Web Page URL</span>
                        <span class="FieldValuePopup">
                            <asp:TextBox ID="tbURL" runat="server" Width="400px" ValidationGroup="NewHyperlink"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="tbURL" Display="Dynamic" ErrorMessage="<br />Web Page URL is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="NewHyperlink"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator Font-Italic="true" ForeColor="Red" ID="RegularExpressionValidator1" runat="server" CssClass="FormInstructions" Font-Bold="true" ErrorMessage="<br />Not a properly formated url." ControlToValidate="tbURL" Display="Dynamic" ValidationExpression="(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?" ValidationGroup="NewHyperlink"></asp:RegularExpressionValidator>
                        </span>
                    </div>
                    <br style="clear: both;" />
                    <div style="float: right; text-align: right; margin: 5px 1px 0px 0px;">
                        <a class="dnnSecondaryAction" id="btnCancelLink" runat="server">Cancel</a>
                        <asp:LinkButton ID="btnSaveLink" runat="server" Text="Save" ValidationGroup="NewHyperlink" CssClass="dnnPrimaryAction" OnClick="btnSaveLink_Click" OnClientClick="$('#newHyperlinkDialog').dialog('close')" />
                    </div>
                </div>
            </div>
        </div>
        <div id="newFileDialog" class="nocontent dms">
            <div id="newFile-content" class="dialog-content">
                <div class="body_padding">
                    <div class="RecordDisplay" style="overflow: hidden">
                        <span class="FieldNamePopup">File</span>
                        <span class="FieldValuePopup">
                            <asp:FileUpload ID="upDocument" runat="server" CssClass="inputfile" />
                            <label for="<%= upDocument.ClientID %>"><span></span> <strong><svg xmlns="http://www.w3.org/2000/svg" width="20" height="17" viewBox="0 0 20 17"><path d="M10 0l-5.2 4.9h3.3v5.1h3.8v-5.1h3.3l-5.2-4.9zm9.3 11.5l-3.2-2.1h-2l3.4 2.6h-3.5c-.1 0-.2.1-.2.1l-.8 2.3h-6l-.8-2.2c-.1-.1-.1-.2-.2-.2h-3.6l3.4-2.6h-2l-3.2 2.1c-.4.3-.7 1-.6 1.5l.6 3.1c.1.5.7.9 1.2.9h16.3c.6 0 1.1-.4 1.3-.9l.6-3.1c.1-.5-.2-1.2-.7-1.5z"/></svg></strong></label>
                            <asp:Panel ID="pnlUploadMsg" runat="server" CssClass="statusMessage" style="color: red; font-weight:bold; visibility: hidden; overflow: auto; width: 100%; height: 165px;"></asp:Panel>
                        </span>
                    </div>
                    <br style="clear: both;" />
                    <div style="float: right; text-align: right; margin: 5px 1px 0px 0px;">
                        <a class="dnnSecondaryAction" id="btnCancelFile" runat="server">Cancel</a>
                        <asp:LinkButton ID="btnSaveFile" runat="server" Text="Save" CausesValidation="false" OnClick="btnSaveFile_Click" Enabled="true" CssClass="dnnPrimaryAction" OnClientClick="return OnUpload();" />
                    </div>
                </div>
            </div>
        </div>
        <div id="versionDialog" class="nocontent dms">
            <div id="version-content" class="dialog-content">
                <div class="body_padding">
                    <div class="RecordDisplay" style="overflow: hidden">
                        <span class="FieldNamePopup">Version</span>
                        <span class="FieldValuePopup" style="width: 203px;">
                            <table style="margin-top: 2px;">
                                <tr>
                                    <td valign="bottom"><asp:TextBox runat="server" type="number" ID="tbMajor" min="0" max="999" Width="50px" ValidationGroup="Version" /></td>
                                    <td valign="bottom">.</td>
                                    <td valign="bottom"><asp:TextBox runat="server" type="number" ID="tbMinor" min="0" max="999" Width="50px" ValidationGroup="Version" /></td>
                                    <td valign="bottom">.</td>
                                    <td valign="bottom"><asp:TextBox runat="server" type="number" ID="tbBuild" min="0" max="999" Width="50px" ValidationGroup="Version" /></td>
                                </tr>
                            </table>
                            <asp:Panel ID="pnlVersionMsg" runat="server" CssClass="statusMessage" style="color: red; font-weight:bold; display: none;"></asp:Panel>
                        </span>
                    </div>
                    <br style="clear: both;" />
                    <div style="float: right; text-align: right; margin: 5px 1px 0px 0px;">
                        <asp:HiddenField Value="0" ID="hidFileVersionId" runat="server" />
                        <a class="dnnSecondaryAction" onclick="$('#versionDialog').dialog('close'); return false;">Cancel</a>
                        <a class="dnnPrimaryAction" onclick="saveVersion(); return false;">Save</a>
                    </div>
                </div>
            </div>
        </div>
        <div id="historyDialog" class="nocontent dms">
            <div id="historylink-content" class="dialog-content">
                <div class="body_padding" style="overflow: auto; height: 650px; width: 100%;">
                    <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="False" CellPadding="3" CellSpacing="3"  EmptyDataText="<br /><strong>No history found.</strong>" ForeColor="Black" 
                        GridLines="None" DataKeyNames="FileVersionId" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" AllowPaging="False" AllowSorting="False" Width="100%"
                        ShowHeader="True" OnRowDeleting="gvHistory_RowDeleting" OnRowDataBound="gvHistory_RowDataBound">
	                    <Columns>
                            <asp:TemplateField ItemStyle-VerticalAlign="Middle" ItemStyle-Width="84px">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnDelete" CommandName="Delete" runat="server" Style="padding-top: 5px;" ImageUrl="~/desktopmodules/Gafware/DMS/images/icons/DeleteIcon1.gif" onmouseout="MM_swapImgRestore()" OnClientClick='<%# "return confirm(\"Are you sure you wish to delete this " + (IsUrl(Container.DataItem) ?  "link" : "file") + "?\");" %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Version" HeaderStyle-Wrap="false" SortExpression="Version" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                <ItemTemplate>
                                    <%# GetVersion(Container.DataItem) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="File Size" HeaderStyle-Wrap="false" SortExpression="Filesize" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                <ItemTemplate>
                                    <%# GetFilesize(Container.DataItem) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Uploader" HeaderStyle-Wrap="false" SortExpression="CreatedByUser.DisplayName" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle">
                                <ItemTemplate>
                                    <%# Eval("CreatedByUser.DisplayName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date Uploaded" HeaderStyle-Wrap="false" SortExpression="CreatedOnDate" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                <ItemTemplate>
                                    <%# ((DateTime)Eval("CreatedOnDate")).ToString("MM/dd/yyyy") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Download" HeaderStyle-Wrap="false" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                <ItemTemplate>
                                    <a href="<%# IsUrl(Container.DataItem) ? Eval("WebpageUrl") : ResolveUrl("~/desktopmodules/Gafware/DMS/GetFile.ashx?vid=" + Eval("FileVersionId").ToString()) %>" title="<%# IsUrl(Container.DataItem) ? Eval("WebpageUrl") : "Download" %>" target="_blank" style="color: #<%# Theme %>;"><%# IsUrl(Container.DataItem) ? Eval("WebpageUrl") : "Download" %></a>
                                </ItemTemplate>
                            </asp:TemplateField>
	                    </Columns>
	                    <FooterStyle BackColor="White" />
	                    <RowStyle BackColor="#C0C0C0" VerticalAlign="Top" />
	                    <EditRowStyle VerticalAlign="Top" />
	                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" VerticalAlign="Top" />
	                    <HeaderStyle BackColor="#666666" Font-Bold="False" ForeColor="White" HorizontalAlign="Left" Font-Size="10pt" VerticalAlign="Top" Font-Underline="false" />
	                    <AlternatingRowStyle BackColor="White" VerticalAlign="Top" />
                    </asp:GridView>
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlGrid" runat="server" style="display: none">
        <h3><%=LocalizeString("BasicSettings")%></h3>
        <div class="searchBox" id="searchBox" runat="server">
            <div style="width: 180px; float: left;">
                <strong><asp:Label ID="lblCategory" runat="server" Text="Label"></asp:Label></strong><br clear="none"/> 
                <asp:DropDownList ID="ddCategory" runat="server" DataTextField="CategoryName" DataValueField="CategoryId" AutoPostBack="true" OnSelectedIndexChanged="ddCategory_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div style="margin-left: 10px; float: right; text-align: left; padding-right: 30px;">
    	        <strong>Enter search term(s): </strong><br clear="none" />
                <div style="float: left; width: 700px;">
                    <asp:TextBox ID="tbKeywords" Width="100%" runat="server" autofocus placeholder="Search Terms ..."></asp:TextBox>
                    <br />To view all documents, click "Go!" without typing a keyword.
                </div>
                <div style="float: right; width: 100px; margin-left: 10px;">
                    <asp:LinkButton ID="btnSearch" Width="100px" Height="35px" runat="server" Text="Go!" OnClick="btnSearch_Click" CssClass="dnnPrimaryAction" />
                </div>
            </div>
            <br clear="all"/>
       	    <br />
        </div>
        <br />
        <p>Click on the document name link to see the details </p>
        <div style="float: left; text-align: left; margin: 0px 0px 5px 1px;">
            <uc1:LetterFilter runat="server" OnClick="letterFilter_Click" ID="letterFilter" /><br />
            <asp:Panel ID="pnlOwner" runat="server" style="margin-bottom: 10px;"><span style="vertical-align: middle;">Owner:</span> <asp:DropDownList ID="ddOwner" DataValueField="UserId" DataTextField="DisplayName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddOwner_SelectedIndexChanged" style="margin-bottom: 0 !important; vertical-align: middle;"></asp:DropDownList></asp:Panel>
        </div>
        <div style="float:right;margin-bottom:10px;" id="pnlAdmin" runat="server" visible="false">
            <asp:LinkButton runat="server" id="backCommandButton" causesvalidation="False" CssClass="secondaryButton dmsButton" OnClick="backCommandButton_Click"><asp:label runat="server" resourcekey="backCommandButton" /></asp:LinkButton>
            <asp:LinkButton runat="server" id="newDocumentCommandButton" causesvalidation="False" CssClass="secondaryButton dmsButton" OnClick="newDocumentCommandButton_Click"><asp:label runat="server" resourcekey="newDocumentCommandButton" /></asp:LinkButton>
            <asp:LinkButton runat="server" id="delAllCommandButton" causesvalidation="False" CssClass="secondaryButton dmsButton" OnClick="delAllCommandButton_Click" OnClientClick="return confirm('Are you sure you wish to delete all the documents in this repository?');"><asp:label runat="server" resourcekey="delAllCommandButton" /></asp:LinkButton>
            <input type="button" id="changeOwnershipCommandButton" runat="server" value="Change Ownership" class="secondaryButton dmsButton" resourcekey="changeOwnershipCommandButton" />
        </div>
        <div style="clear: both"></div>
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="False" CellPadding="3" CellSpacing="3" 
            EmptyDataText="<br /><strong>No documents found.</strong>" 
            ForeColor="Black" GridLines="None" DataKeyNames="DocumentID" BackColor="White" BorderColor="#DEDFDE" PageSize="20" CssClass="filesList"
            BorderStyle="None" BorderWidth="1px" AllowPaging="True" AllowSorting="True" Width="100%" OnDataBound="gv_DataBound" OnRowDataBound="gv_RowDataBound"
            OnPageIndexChanging="gv_PageIndexChanging" ShowHeader="True" OnSorting="gv_Sorting" PagerSettings-PageButtonCount="5">
		    <Columns>
                <asp:TemplateField HeaderText="ID <img src='/DesktopModules/Gafware/DMS/Images/sortneutral.png' border='0' alt='Sort by ID' />" HeaderStyle-Wrap="false" SortExpression="DocumentId" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# Eval("DocumentId") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Document Name <img src='/DesktopModules/Gafware/DMS/Images/sortneutral.png' border='0' alt='Sort by Document Name' />" HeaderStyle-Wrap="false" SortExpression="DocumentName">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lnkDocumentName" CommandArgument='<%# Eval("DocumentId") %>' CommandName="Details" OnCommand="lnkDocumentName_Command"><%# Eval("DocumentName") %></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Last Modified <img src='/DesktopModules/Gafware/DMS/Images/sortneutral.png' border='0' alt='Sort by Date Last Modified' />" HeaderStyle-Wrap="false" SortExpression="LastModifiedOnDate" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# ((DateTime)Eval("LastModifiedOnDate")).ToString("MM/dd/yyyy") %>
                    </ItemTemplate>
                </asp:TemplateField>
		    </Columns>
		    <FooterStyle BackColor="White" />
		    <RowStyle BackColor="#F7F7F7" VerticalAlign="Top" />
		    <EditRowStyle VerticalAlign="Top" />
		    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" VerticalAlign="Top" />
		    <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
            <PagerTemplate>
                <table class="pager">
                    <tr>
                        <td>
                            <asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>
                        </td>
                    </tr>
                </table>
            </PagerTemplate>		            
		    <HeaderStyle BackColor="#666666" Font-Bold="False" ForeColor="White" HorizontalAlign="Left" Font-Size="10pt" VerticalAlign="Top" Font-Underline="false" />
		    <AlternatingRowStyle BackColor="#D0D0D0" VerticalAlign="Top" />
	    </asp:GridView>
    </asp:Panel>
    <div id="changeOwnershipDialog" class="nocontent">
        <div id="changeOwnership-content" class="dialog-content">
            <div class="dms body_padding">
                <div class="RecordDisplay">
                    <span class="FieldNamePopup">Current Owner</span>
                    <span class="FieldValuePopup">
                        <asp:DropDownList ID="ddCurrentOwner" runat="server" DataTextField="DisplayName" DataValueField="UserID"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddCurrentOwner" InitialValue="0" Display="Dynamic" ErrorMessage="<br />Current Owner is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="NewOwnership"></asp:RequiredFieldValidator>
                    </span>
                </div>
                <br style="clear: both;" />
                <div class="RecordDisplay">
                    <span class="FieldNamePopup">New Owner</span>
                    <span class="FieldValuePopup">
                        <asp:DropDownList ID="ddNewOwner" runat="server" DataTextField="DisplayName" DataValueField="UserID" ValidationGroup="NewOwnership"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddNewOwner" InitialValue="0" Display="Dynamic" ErrorMessage="<br />New Owner is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="NewOwnership"></asp:RequiredFieldValidator>
                    </span>
                </div>
                <br style="clear: both;" />
                <div style="float: right; text-align: right; margin: 5px 1px 0px 0px;">
                    <asp:LinkButton Text="Cancel" CssClass="dnnSecondaryAction" ID="btnCancelChange" runat="server" />
                    <asp:LinkButton ID="btnSaveChange" runat="server" Text="Save" ValidationGroup="NewOwnership" CssClass="dnnPrimaryAction" OnClick="btnSaveChange_Click" />
                </div>
            </div>
        </div>
    </div>
</div>
