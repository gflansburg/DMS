<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Gafware.Modules.DMS.Settings" %>
<%@ Register Assembly="Gafware.DMS" Namespace="Gafware.Modules.DMS" TagPrefix="cc1" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>

<asp:Literal ID="litCSS" runat="server"></asp:Literal>
<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblPortalWideRepository" runat="server" ControlName="chkPortalWideRepository" Suffix=":" /> 
        <div class="toggleButton" id="chkPortalWideRepositoryToggleButton" runat="server" style="width: 120px; display: inline-block; top: -5px;">
            <label for='<%= chkPortalWideRepository.ClientID %>'><asp:CheckBox ID="chkPortalWideRepository" AutoPostBack="true" runat="server" OnCheckedChanged="chkPortalWideRepository_CheckedChanged" /><span style="top: -7px; left: -7px;"></span></label>
        </div>
    </div>

    <div class="dnnFormItem">
        <dnn:Label ID="lblRole" runat="server" ControlName="ddlRole" Suffix=":" /> 
        <asp:DropDownList ID="ddlRole" DataValueField="RoleId" DataTextField="RoleName" runat="server"></asp:DropDownList>
        <asp:LinkButton ID="btnReload" runat="server" CssClass="dnnPrimaryAction" Text="Refresh List" OnClick="btnReload_Click" />
    </div>

    <div class="dnnFormItem">
        <dnn:Label ID="lblCategory" runat="server" ControlName="tbCategory" Suffix=":" /> 
        <asp:TextBox ID="tbCategory" runat="server" MaxLength="255"></asp:TextBox>
    </div>

    <div class="dnnFormItem">
        <dnn:Label ID="lblTheme" runat="server" ControlName="ddlTheme" Suffix=":" /> 
        <asp:DropDownList ID="ddlTheme" runat="server">
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
            <asp:listitem text="5" value="5"></asp:listitem>
            <asp:listitem text="10" value="10"></asp:listitem>
            <asp:listitem text="20" value="20"></asp:listitem>
            <asp:listitem text="25" value="25"></asp:listitem>
            <asp:listitem text="40" value="40"></asp:listitem>
            <asp:listitem text="50" value="50"></asp:listitem>
            <asp:listitem text="100" value="100"></asp:listitem>
        </asp:DropDownList>
    </div>

    <div class="dnnFormItem">
        <dnn:Label ID="lblSaveLocalFile" runat="server" ControlName="chkSaveLocalFile" Suffix=":" /> 
        <div class="toggleButton" id="chkSaveLocalFileToggleButton" runat="server" style="width: 120px; display: inline-block; top: -5px;">
            <label for='<%= chkSaveLocalFile.ClientID %>'><asp:CheckBox ID="chkSaveLocalFile" AutoPostBack="false" runat="server" /><span style="top: -7px; left: -7px;"></span></label>
        </div>
    </div>

    <div class="dnnFormItem">
        <dnn:Label ID="lblShowTips" runat="server" ControlName="chkShowTips" Suffix=":" /> 
        <div class="toggleButton" id="chkShowTipsToggleButton" runat="server" style="width: 120px; display: inline-block; top: -5px;">
            <label for='<%= chkShowTips.ClientID %>'><asp:CheckBox ID="chkShowTips" AutoPostBack="false" runat="server" /><span style="top: -7px; left: -7px;"></span></label>
        </div>
    </div>

    <div class="dnnFormItem">
        <dnn:Label ID="lblShowInstructions" runat="server" ControlName="chkShowInstructions" Suffix=":" /> 
        <div class="toggleButton" id="chkShowInstructionsToggleButton" runat="server" style="width: 120px; display: inline-block; top: -5px;">
            <label for='<%= chkShowInstructions.ClientID %>'><asp:CheckBox ID="chkShowInstructions" AutoPostBack="true" runat="server" OnCheckedChanged="chkShowInstructions_CheckedChanged" /><span style="top: -7px; left: -7px;"></span></label>
        </div>
    </div>

    <div class="dnnFormItem" id="pnlInstructions" runat="server" visible="true">
        <dnn:Label ID="lblInstructions" runat="server" ControlName="tbInstructions" Suffix=":" /> 
        <asp:TextBox ID="tbInstructions" runat="server" MaxLength="255"></asp:TextBox>
    </div>

    <div class="dnnFormItem">
        <dnn:Label ID="lblFileNotifications" runat="server" ControlName="ddFileNotifications" Suffix=":" /> 
        <asp:DropDownList ID="ddFileNotifications" DataValueField="RoleId" DataTextField="RoleName" runat="server"></asp:DropDownList>
        <asp:LinkButton ID="btnReload2" runat="server" CssClass="dnnPrimaryAction" Text="Refresh List" OnClick="btnReload2_Click" />
    </div>

    <div class="dnnFormItem">
        <dnn:Label ID="lblFileNotificationSubject" runat="server" ControlName="tbFileNotificationSubjexct" Suffix=":" /> 
        <asp:TextBox ID="tbFileNotificationSubject" runat="server"></asp:TextBox>
    </div>

    <div class="dnnFormItem">
        <dnn:Label ID="lblReplyEmail" runat="server" ControlName="txtReplyEmail" Suffix=":" />
        <span>You can use the following tokens in your email:<br />  [FILENAME] [FILETYPE] [FILETYPENAME] [UPLOADER] [UPLOADDATE] [DOCUMENT]</span>
    </div>
    <div style="margin-left: 20px; margin-right: 20px"><dnn:TextEditor id="txtReplyEmail" runat="server" Width="100%"></dnn:TextEditor></div>

</fieldset>

<h2 style="font-size: 18px; padding: 5px;"><%=LocalizeString("Categories")%></h2>
<fieldset>
    <div style="padding:10px">
        <cc1:GridViewExtended ID="gv_Categories" runat="server" AutoGenerateColumns="False" DataKeyNames="CategoryId" Width="100%"
            OnRowEditing="gv_Categories_RowEditing" RowStyle-BackColor="#eeeeee" ShowFooterWhenEmpty="True" ShowHeaderWhenEmpty="True"
            RowStyle-Height="18" HeaderStyle-Height="30" GridLines="None" Font-Names="Arial" Font-Size="Small" CellPadding="4" ShowFooter="True" 
            ForeColor="#333333" OnRowUpdating="gv_Categories_RowUpdating" OnRowCancelingEdit="gv_Categories_RowCancelingEdit" 
            OnRowDataBound="gv_Categories_RowDataBound" OnRowDeleting="gv_Categories_RowDeleting" OnRowCommand="gv_Categories_RowCommand">
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle VerticalAlign="Top" Font-Names="Arial" Font-Size="Small" BackColor="#EFF3FB" />
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle Font-Names="Arial" Font-Size="Small" BackColor="#FFFFFF" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#507CD1" HorizontalAlign="Left" VerticalAlign="Middle" Wrap="False" Font-Size="Small" Font-Names="Arial" ForeColor="White" Font-Bold="True" />
            <AlternatingRowStyle BackColor="White" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="50px"> 
                    <ItemTemplate>
                        <asp:ImageButton ID="editButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/EditIcon1_16px.gif" AlternateText="Edit Category" ToolTip="Edit Category" CommandName="Edit" Text="Edit" /> 
                        <asp:ImageButton ID="deleteButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/DeleteIcon1_16px.gif" AlternateText="Delete Category" ToolTip="Delete Category" CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure you wish to delete this category?');" CausesValidation="false" /> 
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:ImageButton ID="saveButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/save.gif" AlternateText="Save Category" ToolTip="Save Category" CommandName="Update" Text="Update" /> 
                        <asp:ImageButton ID="cancelButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/DeleteIcon1_16px.gif" AlternateText="Cancel Edit" ToolTip="Cancel Edit" CommandName="Cancel" Text="Cancel" /> 
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:ImageButton ID="newButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/NewIcon1_16px.gif" AlternateText="New Category" ToolTip="New Category" CommandName="New" Text="New" CausesValidation="false" /> 
                        <asp:ImageButton ID="saveInsertButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/save.gif" AlternateText="Save Category" ToolTip="Save Category" CommandName="Insert" Text="Insert" Visible="false" CausesValidation="true" /> 
                        <asp:ImageButton ID="cancelInsertButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/DeleteIcon1_16px.gif" AlternateText="Cancel Edit" ToolTip="Cancel Insert" CommandName="Cancel" Text="Cancel" Visible="false" CausesValidation="false" /> 
                    </FooterTemplate>
                </asp:TemplateField> 
                <asp:TemplateField HeaderText="Category Name">
                    <ItemTemplate>
                        <%# Eval("CategoryName") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbCategoryName" runat="server" Text='<%# Eval("CategoryName") %>' MaxLength="50" Width="100%" style="font-weight: normal;"></asp:TextBox>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="tbCategoryName2" runat="server" Text='<%# Eval("CategoryName") %>' MaxLength="50" Width="100%" style="font-weight: normal;"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Required Role">
                    <ItemTemplate>
                        <%# Eval("Role.RoleName") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlRequiredRole" DataValueField="RoleId" DataTextField="RoleName" runat="server"></asp:DropDownList>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:DropDownList ID="ddlRequiredRole2" DataValueField="RoleId" DataTextField="RoleName" runat="server"></asp:DropDownList>
                    </FooterTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridViewExtended>
    </div>
</fieldset>

<h2 style="font-size: 18px; padding: 5px;"><%=LocalizeString("DocumentTypes")%></h2>
<fieldset>
    <div style="padding:10px">
        <cc1:GridViewExtended ID="gv_docTypes" runat="server" AutoGenerateColumns="False" DataKeyNames="FileTypeId" Width="100%"
            OnRowEditing="gv_docTypes_RowEditing" RowStyle-BackColor="#eeeeee" ShowFooterWhenEmpty="True" ShowHeaderWhenEmpty="True"
            RowStyle-Height="18" HeaderStyle-Height="30" GridLines="None" Font-Names="Arial" Font-Size="Small" CellPadding="4" ShowFooter="True" 
            ForeColor="#333333" OnRowUpdating="gv_docTypes_RowUpdating" OnRowCancelingEdit="gv_docTypes_RowCancelingEdit" 
            OnRowDataBound="gv_docTypes_RowDataBound" OnRowDeleting="gv_docTypes_RowDeleting" OnRowCommand="gv_docTypes_RowCommand">
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle VerticalAlign="Top" Font-Names="Arial" Font-Size="Small" BackColor="#EFF3FB" />
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle Font-Names="Arial" Font-Size="Small" BackColor="#FFFFFF" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#507CD1" HorizontalAlign="Left" VerticalAlign="Middle" Wrap="False" Font-Size="Small" Font-Names="Arial" ForeColor="White" Font-Bold="True" />
            <AlternatingRowStyle BackColor="White" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="50px"> 
                    <ItemTemplate>
                        <asp:ImageButton ID="editFileTypeButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/EditIcon1_16px.gif" AlternateText="Edit Document Type" ToolTip="Edit Document Type" CommandName="Edit" Text="Edit" /> 
                        <asp:ImageButton ID="deleteFileTypeButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/DeleteIcon1_16px.gif" AlternateText="Delete Document Type" ToolTip="Delete Document Type" CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure you wish to delete this document type?');" CausesValidation="false" /> 
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:ImageButton ID="saveFileTypeButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/save.gif" AlternateText="Save Document Type" ToolTip="Save Document Type" CommandName="Update" Text="Update" /> 
                        <asp:ImageButton ID="cancelFileTypeButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/DeleteIcon1_16px.gif" AlternateText="Cancel Edit" ToolTip="Cancel Edit" CommandName="Cancel" Text="Cancel" /> 
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:ImageButton ID="newFileTypeButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/NewIcon1_16px.gif" AlternateText="New Document Type" ToolTip="New Document Type" CommandName="New" Text="New" CausesValidation="false" /> 
                        <asp:ImageButton ID="saveInsertFileTypeButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/save.gif" AlternateText="Save Document Type" ToolTip="Save Document Type" CommandName="Insert" Text="Insert" Visible="false" CausesValidation="true" /> 
                        <asp:ImageButton ID="cancelInsertFileTypeButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/DeleteIcon1_16px.gif" AlternateText="Cancel Edit" ToolTip="Cancel Insert" CommandName="Cancel" Text="Cancel" Visible="false" CausesValidation="false" /> 
                    </FooterTemplate>
                </asp:TemplateField> 
                <asp:TemplateField HeaderText="Document Type Name">
                    <ItemTemplate>
                        <%# Eval("FileTypeName") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbFileTypeName" runat="server" Text='<%# Eval("FileTypeName") %>' MaxLength="100" Width="100%" style="font-weight: normal;"></asp:TextBox>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="tbFileTypeName2" runat="server" Text='<%# Eval("FileTypeName") %>' MaxLength="100" Width="100%" style="font-weight: normal;"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Short Name">
                    <ItemTemplate>
                        <%# Eval("FileTypeShortName") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbFileTypeShortName" runat="server" Text='<%# Eval("FileTypeShortName") %>' MaxLength="10" Width="100%" style="font-weight: normal;"></asp:TextBox>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="tbFileTypeShortName2" runat="server" Text='<%# Eval("FileTypeShortName") %>' MaxLength="10" Width="100%" style="font-weight: normal;"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Extension(s)">
                    <ItemTemplate>
                        <%# Eval("FileTypeExt") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbFileTypeExt" runat="server" Text='<%# Eval("FileTypeExt") %>' MaxLength="255" Width="100%" style="font-weight: normal;"></asp:TextBox>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="tbFileTypeExt2" runat="server" Text='<%# Eval("FileTypeExt") %>' MaxLength="255" Width="100%" style="font-weight: normal;"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridViewExtended>
    </div>
</fieldset>

<asp:Panel ID="pnlUpdateSettings" runat="server" Visible="false">
    <hr />
    <div style="float:right;margin-bottom:10px;">
        <asp:linkbutton runat="server" id="updateSettings" causesvalidation="True" CssClass="dnnPrimaryAction" OnClick="updateSettings_Click"><asp:label runat="server" resourcekey="lblUpdateSettings" /></asp:linkbutton>
        <asp:linkbutton runat="server" id="cancelSettings" causesvalidation="False" CssClass="dnnSecondaryAction" OnClick="cancelSettings_Click"><asp:label runat="server" resourcekey="lblCancelSettings" /></asp:linkbutton>
    </div>
</asp:Panel>