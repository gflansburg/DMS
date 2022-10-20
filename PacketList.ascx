<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PacketList.ascx.cs" Inherits="Gafware.Modules.DMS.PacketList" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<%@ Register Src="~/desktopmodules/Gafware/DMS/LetterFilter.ascx" TagPrefix="uc1" TagName="LetterFilter" %>
<%@ Register Src="~/desktopmodules/Gafware/DMS/DocumentSearchResults.ascx" TagPrefix="uc1" TagName="DocumentSearchResults" %>
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
    .dms .nocontent {
        display: none;
    }
    .dms .dnnFormItem .toggleButton {
        top: -2px !important;
        left: -8px !important;
    }
</style>
<div class="se-pre-con"></div>
<asp:Literal ID="litCSS" runat="server"></asp:Literal>
<asp:HiddenField ID="preview" runat="server" Value="0" />
<div class="dms" style="padding: 10px;">
    <asp:Panel ID="pnlDetails" runat="server" style="display: none">
        <h3><%=LocalizeString("EditSettings")%></h3>
        <br />
        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <h3><%= LocalizeString("PacketError") %></h3><br />
            <%= LocalizeString("PacketErrorInfo") %>
        </asp:Panel>
        <asp:Panel ID="pnlFound" runat="server">
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblName" runat="server" ControlName="tbName" Suffix=":" /> 
                    <asp:TextBox ID="tbName" runat="server" MaxLength="50" autofocus ValidationGroup="PacketEditor" style="width: calc(100% - 310px);"></asp:TextBox>
                    <asp:LinkButton ID="btnEditName" CssClass="dnnSecondaryAction" runat="server" CausesValidation="false" Text="Edit Name" OnClick="btnEditName_Click" style="line-height: 20px;" />
                    <asp:HiddenField ID="hidCancelRename" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbName" Display="Dynamic" ErrorMessage="<br />Name is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="PacketEditor"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="CustomFieldValidator1" runat="server" ControlToValidate="tbName" Display="Dynamic" ErrorMessage="<br />This Packet Name is already in use." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="PacketEditor"></asp:CustomValidator>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblDescription" runat="server" ControlName="tbDescription" Suffix=":" /> 
                    <asp:TextBox ID="tbDescription" runat="server" TextMode="MultiLine" Rows="1" ValidationGroup="PacketEditor"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbDescription" Display="Dynamic" ErrorMessage="<br />Description is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="PacketEditor"></asp:RequiredFieldValidator>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblIsGroupOwner" runat="server" ResourceKey="lblIsGroupOwner" ControlName="cbIsGroupOwner" Suffix=":" /> 
                    <div class="toggleButton" id="cbIsGroupOwnerToggleButton" runat="server">
                        <label for='<%= cbIsGroupOwner.ClientID %>'><asp:CheckBox ID="cbIsGroupOwner" AutoPostBack="true" runat="server" OnCheckedChanged="cbIsGroupOwner_CheckedChanged" /><span></span></label>
                    </div>
                </div>
                <div class="dnnFormItem" runat="server" id="pnlOwnerEdit">
                    <dnn:Label ID="lblOwner" runat="server" ResourceKey="lblOwner" ControlName="ddOwner2" Suffix=":" /> 
                    <asp:DropDownList ID="ddOwner2" runat="server" DataTextField="DisplayName" DataValueField="UserId" ValidationGroup="DocumentControl" style="width: auto;"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="ddOwner2" InitialValue="0" Display="Dynamic" ErrorMessage="<br />Owner is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="DocumentControl"></asp:RequiredFieldValidator>
                </div>
                <div class="dnnFormItem" runat="server" id="pnlGroupEdit" visible="false">
                    <dnn:Label ID="lblGroup" runat="server" ResourceKey="lblGroup" ControlName="ddGroup" Suffix=":" /> 
                    <asp:DropDownList ID="ddGroup" runat="server" DataTextField="RoleName" DataValueField="RoleId" ValidationGroup="DocumentControl" style="width: auto;"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="ddGroup" InitialValue="0" Display="Dynamic" ErrorMessage="<br />Group is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="DocumentControl"></asp:RequiredFieldValidator>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblShowPacketDescription" runat="server" ControlName="cbShowPacketDescription" Suffix=":" /> 
                    <div class="toggleButton" id="cbShowPacketDescriptionToggleButton" runat="server">
                        <label for='<%= cbShowPacketDescription.ClientID %>'><asp:CheckBox ID="cbShowPacketDescription" AutoPostBack="false" runat="server" Checked="true" /><span></span></label>
                    </div>
                </div>
                <asp:Panel ID="pnlAdminComments" runat="server">
                    <div class="dnnFormItem">
                        <dnn:Label ID="lblAdminComments" runat="server" ControlName="tbAdminComments" Suffix=":" /> 
                        <asp:TextBox ID="tbAdminComments" runat="server" TextMode="MultiLine" Rows="1" ValidationGroup="PacketEditor"></asp:TextBox>
                    </div>
                </asp:Panel>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblCustomHeader" runat="server" ControlName="tbCustomHeader" Suffix=":" /> 
                    <asp:TextBox ID="tbCustomHeader" MaxLength="100" runat="server" ValidationGroup="PacketEditor"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblShowDescription" runat="server" ControlName="cbShowDescription" Suffix=":" /> 
                    <div class="toggleButton" id="cbShowDescriptionToggleButton" runat="server">
                        <label for='<%= cbShowDescription.ClientID %>'><asp:CheckBox ID="cbShowDescription" AutoPostBack="false" runat="server" Checked="true" /><span></span></label>
                    </div>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblDocuments" runat="server" ControlName="ddDocuments" Suffix=":" /> 
                    <asp:DropDownList ID="ddDocuments" runat="server" DataTextField="DocumentName" DataValueField="DocumentID" ValidationGroup="AddDocument" style="width: calc(100% - 327px);"></asp:DropDownList>
                    <asp:LinkButton ID="btnAddDocument" runat="server" OnClick="btnAddDocument_Click" Text="Add Document" ValidationGroup="AddDocument" CssClass="dnnSecondaryAction" style="line-height: 20px;" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="ddDocuments" InitialValue="0" Display="Dynamic" ErrorMessage=" Document Required" Font-Bold="true" ForeColor="Red" ValidationGroup="AddDocument"></asp:RequiredFieldValidator>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblTags" runat="server" ControlName="ddTags" Suffix=":" /> 
                    <asp:DropDownList ID="ddTags" runat="server" DataTextField="TagName" DataValueField="TagID" ValidationGroup="AddTag" style="width: calc(100% - 284px);"></asp:DropDownList>
                    <asp:LinkButton ID="btnAddTag" runat="server" OnClick="btnAddTag_Click" Text="Add Tag" ValidationGroup="AddTag" CssClass="dnnSecondaryAction" style="line-height: 20px;" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="ddTags" InitialValue="0" Display="Dynamic" ErrorMessage=" Tag Required" Font-Bold="true" ForeColor="Red" ValidationGroup="AddTag"></asp:RequiredFieldValidator>
                </div>
            </fieldset>
            <span style="font-weight: bold"><%= LocalizeString("DocumentsSelected") %></span><br />
            <asp:HiddenField ID="selectedDocuments" runat="server" />
            <div style="background-color: #EEEEEE; border: 1px solid #999; text-align: left; margin: 5px 1px 5px 0px; width: 100%; overflow: auto;">
                <asp:GridView ID="gvPackets" runat="server" BorderWidth="1px" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" 
                    RowStyle-BackColor="#eeeeee" RowStyle-Height="18" Width="100%" GridLines="None" ShowHeader="false"
                    Font-Names="Arial" Font-Size="Small" CellPadding="3" CellSpacing="3" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" 
                    ForeColor="Black" OnRowDeleting="gvPackets_RowDeleting" OnRowDataBound="gvPackets_RowDataBound">
                    <RowStyle VerticalAlign="Top" Font-Names="Arial" Font-Size="Small" BackColor="#F7F7F7" />
                    <FooterStyle Font-Names="Arial" Font-Size="X-Small" BackColor="White" />
                    <HeaderStyle BackColor="#666666" HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" Font-Size="Small" Font-Names="Arial" ForeColor="White" Font-Bold="False" Font-Underline="false" />
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Left" ItemStyle-Width="0px" ItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate>
                                <%# (PayloadType)Eval("PayloadType") == PayloadType.Document ? Eval("PacketDocument.DocumentID") : Eval("PacketTag.TagId")  %>
                            </ItemTemplate> 
                        </asp:TemplateField> 
                        <asp:TemplateField ItemStyle-CssClass="dont-break-out" ItemStyle-Wrap="true" ItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate>
                                <asp:HiddenField ID="payloadType" runat="server" Value='<%# Eval("PayloadType").ToString()  %>' />
                                <asp:HiddenField ID="itemID" runat="server" Value='<%# (PayloadType)Eval("PayloadType") == PayloadType.Document ? Eval("PacketDocument.PacketDocId") : Eval("PacketTag.PacketTagId")  %>' />
                                <asp:HiddenField ID="subItemID" runat="server" Value='<%# (PayloadType)Eval("PayloadType") == PayloadType.Document ? Eval("PacketDocument.DocumentId") : Eval("PacketTag.TagId")  %>' />
                                <%# (PayloadType)Eval("PayloadType") == PayloadType.Document ? Eval("PacketDocument.DocumentName") : Eval("PacketTag.TagName")  %>
                            </ItemTemplate> 
                        </asp:TemplateField> 
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="150px" ItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate>
                                <asp:DropDownList ID="ddFileType" DataTextField="FileType" DataValueField="FileID" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddFileType_SelectedIndexChanged" Visible="false" style="margin-bottom: 0 !important" />
                                <asp:Label ID="lblType" runat="server" Text="Tag" />
                            </ItemTemplate> 
                        </asp:TemplateField> 
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="24px" ItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate>
                                <asp:LinkButton ID="deleteButton" runat="server" ToolTip="Delete" CommandName="Delete" CausesValidation="false" OnClientClick='<%# GetConfirmDeletePayload((PayloadType)Eval("PayloadType")) %>'><asp:Image runat="server" ID="deleteImage" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/DeleteIcon1_16px.gif" AlternateText="Delete" ToolTip="Delete" onMouseOut="MM_swapImgRestore()" /></asp:LinkButton>
                            </ItemTemplate> 
                        </asp:TemplateField> 
                        <asp:TemplateField ItemStyle-CssClass="dragHandle" ItemStyle-Width="20px" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate> 
                                <asp:Image ID="imgAnchor" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/row-anchor.png" AlternateText="Row Anchor" ToolTip="Drag Me" Visible='<%# GetDocCount() > 1 %>' /> 
                            </ItemTemplate> 
                        </asp:TemplateField> 
                    </Columns>
                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                </asp:GridView>
            </div>
            <hr />
            <div style="float:right;margin-bottom:10px;">
                <asp:LinkButton ID="btnReset" runat="server" Text="Reset Page" CausesValidation="false" OnClick="btnReset_Click" CssClass="dnnSecondaryAction" />
                <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" OnClick="btnCancel_Click" CssClass="dnnSecondaryAction" />
                <asp:LinkButton ID="btnSave" runat="server" Text="Save" ValidationGroup="PacketEditor" OnClick="btnSave_Click" CssClass="dnnPrimaryAction" />
            </div>
            <br style="clear: both" />
            <asp:HiddenField ID="hidFileCount" runat="server" Value="0" />
            <asp:LinkButton ID="lnkCustomHeaderPostback" runat="server" style="display: none;" CausesValidation="false" OnClick="lnkCustomHeaderPostback_Click"></asp:LinkButton>
            <asp:HiddenField ID="hidBaseUrl" runat="server" Value="" />
            <asp:Panel ID="pnlLink" runat="server" Visible="false">
                <p><strong><%= LocalizeString("LinkURL") %></strong> <span><%= LocalizeString("BasicHelp") %></span></p>
                <asp:TextBox ID="tbLinkURL" TextMode="MultiLine" ReadOnly="true" Width="100%" Rows="3" runat="server"></asp:TextBox>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
    <asp:Panel ID="pnlGrid" runat="server" style="display: none">
        <div class="searchBox" id="searchBox" runat="server">
            <div style="text-align: left; display: inline-block; width: calc(100% - 200px); min-width: 295px;">
    	        <strong>Enter search term(s): </strong><br style="clear: none" />
                <div style="width: 100%; padding: 1px 0">
                    <asp:TextBox ID="tbKeywords" style="min-width: 220px; width: calc(100% - 105px)" runat="server" autofocus placeholder="Search Terms ..."></asp:TextBox>
					<asp:LinkButton ID="btnSearch" Width="100px" runat="server" Text="Go!" OnClick="btnSearch_Click" CssClass="dnnPrimaryAction" />
                </div>
                <asp:Label ID="lblInstructions" runat="server" Text='To view all packets, click "Go!" without typing a keyword.' CssClass="SearchText"></asp:Label>
            </div>
            <br style="clear: both" />
       	    <br />
        </div>
        <br />
        <h3><%=LocalizeString("BasicSettings")%></h3>
        <div style="float: left; text-align: left; margin: 0px 0px 5px 1px;">
            <uc1:LetterFilter runat="server" OnClick="letterFilter_Click" ID="letterFilter" />
            <asp:Panel ID="pnlOwner" runat="server" style="margin-bottom: 10px;"><span style="vertical-align: middle;"><%= LocalizeString("Owner") %></span> <asp:DropDownList ID="ddOwner" DataValueField="UserId" DataTextField="DisplayName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddOwner_SelectedIndexChanged" style="margin-bottom: 0 !important; vertical-align: middle;"></asp:DropDownList></asp:Panel>
        </div>
        <div style="float:right;margin-bottom:10px;" id="pnlAdmin" runat="server">
            <asp:LinkButton runat="server" id="backCommandButton" causesvalidation="False" CssClass="secondaryButton backButton" OnClick="backCommandButton_Click"><asp:label runat="server" resourcekey="backCommandButton" /></asp:LinkButton>
            <asp:LinkButton runat="server" id="newPacketCommandButton" causesvalidation="False" CssClass="secondaryButton addButton" OnClick="newPacketCommandButton_Click"><asp:label runat="server" resourcekey="newPacketCommandButton" /></asp:LinkButton>
            <asp:LinkButton runat="server" id="changeOwnershipCommandButton" causesvalidation="False" CssClass="secondaryButton ownerButton"><asp:label runat="server" resourcekey="changeOwnershipCommandButton" /></asp:LinkButton>
        </div>
        <br style="clear: both" />
		<div style="width: 100%; overflow: auto;">
			<asp:GridView ID="gv" runat="server" AutoGenerateColumns="False" CellPadding="3" CellSpacing="3" 
				EmptyDataText="<br /><strong>No packets found.</strong>" CssClass="filesList"
				ForeColor="Black" GridLines="None" DataKeyNames="PacketId" BackColor="White" BorderColor="#DEDFDE" PageSize="20" 
				BorderStyle="None" BorderWidth="1px" AllowPaging="True" AllowSorting="True" Width="100%" OnDataBound="gv_DataBound"
				OnPageIndexChanging="gv_PageIndexChanging" ShowHeader="True" OnSorting="gv_Sorting" PagerSettings-PageButtonCount="5"
				OnRowDeleting="gv_RowDeleting" OnRowEditing="gv_RowEditing" OnRowDataBound="gv_RowDataBound">
				<Columns>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Left" ItemStyle-Width="50px" HeaderText="Actions" HeaderStyle-Wrap="false" ItemStyle-Wrap="false"> 
						<ItemTemplate>
                            <asp:LinkButton ID="editButton" runat="server" ToolTip="Edit Packet" CommandName="Edit"><asp:Image runat="server" ID="editImage" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/EditIcon1_16px.gif" AlternateText="Edit Packet" ToolTip="Edit Packet" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage(this.id,'','/DesktopModules/Gafware/DMS/Images/icons/EditIcon2_16px.gif',1)" /></asp:LinkButton>
                            &nbsp;
                            <asp:LinkButton ID="deleteButton" runat="server" ToolTip="Delete Packet" CommandName="Delete" OnClientClick='<%# "confirmDelete(this, \"" + JSEncode(Eval("Name").ToString()) + "\");  return false;" %>'><asp:Image runat="server" ID="deleteImage" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/DeleteIcon1_16px.gif" AlternateText="Delete Packet" ToolTip="Delete Packet" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage(this.id,'','/DesktopModules/Gafware/DMS/Images/icons/DeleteIcon2_16px.gif',1)" /></asp:LinkButton>
						</ItemTemplate> 
					</asp:TemplateField> 
					<asp:TemplateField HeaderText="ID <img src='/desktopmodules/Gafware/DMS/Images/sortneutral.png' border='0' alt='Sort by ID' />" HeaderStyle-Wrap="false" SortExpression="PacketId" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="50px">
						<ItemTemplate>
							<%# Eval("PacketId") %>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Packet Name <img src='/desktopmodules/Gafware/DMS/Images/sortneutral.png' border='0' alt='Sort by Packet Name' />" HeaderStyle-Wrap="false" SortExpression="Name">
						<ItemTemplate>
							<asp:LinkButton ID="lnkPreview" runat="server" CommandName="Preview" CommandArgument='<%# Eval("PacketId") %>' OnCommand="lnkPreview_Command"><%# Eval("Name") %></asp:LinkButton>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Comments <img src='/desktopmodules/Gafware/DMS/Images/sortneutral.png' border='0' alt='Sort by Comments' />" HeaderStyle-Wrap="false" SortExpression="AdminComments" ItemStyle-Width="300px">
						<ItemTemplate>
							<%# GetComments(Eval("AdminComments").ToString()) %>
						</ItemTemplate>
					</asp:TemplateField>
				</Columns>
				<FooterStyle BackColor="White" />
				<RowStyle BackColor="#F7F7F7" VerticalAlign="Top" Font-Names="Arial" Font-Size="14px" />
				<EditRowStyle VerticalAlign="Top" />
				<SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" VerticalAlign="Top" />
				<PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
				<PagerTemplate>
					<table>
						<tr>
							<td>
								<asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>
							</td>
						</tr>
					</table>
				</PagerTemplate>		            
				<HeaderStyle BackColor="#666666" Font-Bold="False" ForeColor="White" HorizontalAlign="Left" Font-Size="10pt" VerticalAlign="Top" Font-Underline="false" />
				<AlternatingRowStyle BackColor="White" VerticalAlign="Top" />
			</asp:GridView>
		</div>
    </asp:Panel>
    <div id="changeOwnershipDialog" class="nocontent">
        <div id="changeOwnership-content" class="dialog-content">
            <div class="dms body_padding">
                <fieldset>
                    <div class="dnnFormItem">
                        <dnn:Label ID="lblIsGroupOwner3" runat="server" ControlName="cbIsGroupOwner3" Suffix=":" /> 
                        <div class="toggleButton" id="cbIsGroupOwner3ToggleButton" runat="server">
                            <label for='<%= cbIsGroupOwner3.ClientID %>'><asp:CheckBox ID="cbIsGroupOwner3" AutoPostBack="false" runat="server" onclick="toggleCurrentGroup(this);" /><span></span></label>
                        </div>
                    </div>
                    <div class="dnnFormItem">
                        <div id="pnlCurrentOwner" runat="server">
                            <dnn:Label ID="lblCurrentOwner" runat="server" ControlName="ddCurrentOwner" Suffix=":" /> 
                            <asp:DropDownList ID="ddCurrentOwner" runat="server" DataTextField="DisplayName" Width="100%" style="float: right; min-width: auto;" DataValueField="UserID" ValidationGroup="NewOwnership"></asp:DropDownList>
                            <asp:CustomValidator ID="CustomValidator1" runat="server" ClientValidationFunction="currentOwnerRequired" ControlToValidate="ddCurrentOwner" InitialValue="0" Display="Dynamic" ErrorMessage="<br />Current Owner is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="NewOwnership"></asp:CustomValidator>
                        </div>
                        <div id="pnlCurrentGroup" runat="server" style="display: none;">
                            <dnn:Label ID="lblCurrentGroup" runat="server" ControlName="ddCurrentGroup" Suffix=":" /> 
                            <asp:DropDownList ID="ddCurrentGroup" runat="server" DataTextField="RoleName" Width="100%" style="float: right; min-width: auto;" DataValueField="RoleId" ValidationGroup="NewOwnership"></asp:DropDownList>
                            <asp:CustomValidator ID="CustomValidator2" runat="server" ClientValidationFunction="currentGroupRequired" ControlToValidate="ddCurrentGroup" InitialValue="0" Display="Dynamic" ErrorMessage="<br />Current Group is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="NewOwnership"></asp:CustomValidator>
                        </div>
                    </div>
                    <div class="dnnFormItem">
                        <dnn:Label ID="lblIsGroupOwner2" runat="server" ControlName="cbIsGroupOwner2" Suffix=":" /> 
                        <div class="toggleButton" id="cbIsGroupOwner2ToggleButton" runat="server">
                            <label for='<%= cbIsGroupOwner2.ClientID %>'><asp:CheckBox ID="cbIsGroupOwner2" AutoPostBack="false" runat="server" onclick="toggleNewGroup(this);" /><span></span></label>
                        </div>
                    </div>
                    <div class="dnnFormItem">
                        <div id="pnlNewOwner" runat="server">
                            <dnn:Label ID="lblNewOwner" runat="server" ControlName="ddNewOwner" Suffix=":" /> 
                            <asp:DropDownList ID="ddNewOwner" runat="server" DataTextField="DisplayName" Width="100%" style="float: right; min-width: auto;" DataValueField="UserID" ValidationGroup="NewOwnership"></asp:DropDownList>
                            <asp:CustomValidator ID="RequiredFieldValidator2" runat="server" ClientValidationFunction="newOwnerRequired" ControlToValidate="ddNewOwner" InitialValue="0" Display="Dynamic" ErrorMessage="<br />New Owner is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="NewOwnership"></asp:CustomValidator>
                        </div>
                        <div id="pnlNewGroup" runat="server" style="display: none;">
                            <dnn:Label ID="lblNewGroup" runat="server" ControlName="ddNewGroup" Suffix=":" /> 
                            <asp:DropDownList ID="ddNewGroup" runat="server" DataTextField="RoleName" Width="100%" style="float: right; min-width: auto;" DataValueField="RoleId" ValidationGroup="NewOwnership"></asp:DropDownList>
                            <asp:CustomValidator ID="RequiredFieldValidator8" runat="server" ClientValidationFunction="newGroupRequired" ControlToValidate="ddNewGroup" InitialValue="0" Display="Dynamic" ErrorMessage="<br />New Group is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="NewOwnership"></asp:CustomValidator>
                        </div>
                    </div>
                </fieldset>
                <div style="float: right; text-align: right; margin: 5px 1px 0px 0px;">
                    <a class="dnnSecondaryAction" id="btnCancelChange" runat="server"><%= LocalizeString("Cancel") %></a>
                    <asp:LinkButton ID="btnSaveChange" runat="server" Text="Save" ValidationGroup="NewOwnership" CssClass="dnnPrimaryAction" OnClick="btnSaveChange_Click" />
                </div>
            </div>
        </div>
    </div>
</div>
<div id="previewDialog" class="nocontent dms">
    <div id="preview-content" class="dialog-content" style="overflow: auto; height: 100%; width: 100%;">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always" ChildrenAsTriggers="true" >
            <ContentTemplate>
                <uc1:DocumentSearchResults runat="server" id="documentSearchResults" Search="true" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
