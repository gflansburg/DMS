<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagList.ascx.cs" Inherits="Gafware.Modules.DMS.TagList" %>
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
</style>
<div class="se-pre-con"></div>
<div style="padding: 10px;">
    <div class="dms" style="padding: 5px;">
        <asp:Panel ID="pnlDetails" runat="server" Visible="false">
            <h3><%=LocalizeString("EditSettings")%></h3>
            <br />
            <span class="FormText">
                <span class="RequiredField">*</span> Required field.
            </span>
            <br /><br />
            <div class="RecordDisplay">
                <label for="<%= tbTagName.ClientID %>" >
                    <span class="FieldNameShort"><span class="RequiredField">*</span> Tag Name:</span>
                    <span class="FieldValue">
                        <asp:TextBox ID="tbTagName" runat="server" MaxLength="50" ValidationGroup="TagEditor" Width="890px"></asp:TextBox>
                        <asp:RequiredFieldValidator Font-Italic="true" ForeColor="Red" ID="valTagName" runat="server" CssClass="red-text" Font-Bold="true" ErrorMessage=" Invalid entry." ValidationGroup="TagEditor" ControlToValidate="tbTagName" Display="Dynamic"></asp:RequiredFieldValidator>
                    </span>
                </label>
            </div>
            <%--<br clear="all" />
            <div class="RecordDisplay">
                <label for="<%= rblIsPrivate.ClientID %>" >
                    <span class="FieldNameShort"><span class="RequiredField">*</span> Is Private:</span>
                    <span class="FieldValueShort">
                        <asp:RadioButtonList ID="rblIsPrivate" runat="server" RepeatDirection="Horizontal" ValidationGroup="TagEditor" CellPadding="0" CellSpacing="0">
                            <asp:ListItem Text="Yes" Value="1" />
                            <asp:ListItem Text="No" Value="0" Selected="True" />
                        </asp:RadioButtonList>
                    </span>
                    <span class="FormInstructions"> Denotes whether public website users can see this tag.</span>
                </label>
            </div>
            <br clear="all" />
            <div class="RecordDisplay">
                <label for="<%= tbTagName.ClientID %>" >
                    <span class="FieldNameShort"><span class="RequiredField">*</span> Weight:</span>
                    <span class="FieldValueShort">
                        <telerik:RadNumericTextBox ID="tbWeight" runat="server" DataType="System.Integer" Value="0" NumberFormat-DecimalDigits="0" ValidationGroup="TagEditor"></telerik:RadNumericTextBox>
                        <asp:RequiredFieldValidator Font-Italic="true" ForeColor="Red" ID="RequiredFieldValidator1" runat="server" CssClass="red-text" Font-Bold="true" ErrorMessage=" Invalid entry." ValidationGroup="TagEditor" ControlToValidate="tbWeight" Display="Dynamic"></asp:RequiredFieldValidator>
                    </span>
                    <span class="FormInstructions">Requires numeric values; Higher numbers appear in search dropdown menu first</span>
                </label>
            </div>--%>
            <br clear="all" style="clear: both" />
            <asp:LinkButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit" ValidationGroup="TagEditor" CssClass="dnnPrimaryAction" />
            <asp:LinkButton ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel" CausesValidation="false" CssClass="dnnSecondaryAction" />
        </asp:Panel>
        <asp:Panel ID="pnlGrid" runat="server">
            <h3><%=LocalizeString("BasicSettings")%></h3>
            <div style="float: left; text-align: left; margin: 0px 0px 5px 1px;">
                <uc1:LetterFilter ID="filter" runat="server" OnClick="filter_OnClick" />
            </div>
            <div style="float:right; margin:0px 1px 5px 0px;">
                <asp:LinkButton runat="server" id="btnBack" causesvalidation="False" CssClass="secondaryButton dmsButton" OnClick="btnBack_Click"><asp:label runat="server" resourcekey="btnBack" /></asp:LinkButton>
                <asp:LinkButton runat="server" id="btnAddNewTag" causesvalidation="False" CssClass="secondaryButton dmsButton" OnClick="btnAddNewTag_Click"><asp:label runat="server" resourcekey="btnAddNewTag" /></asp:LinkButton>
            </div>
            <br style="clear: both" />
            <asp:GridView ID="gv" runat="server" BorderWidth="1px" AutoGenerateColumns="False" AllowPaging="True" AllowSorting="True" 
                RowStyle-BackColor="#eeeeee" RowStyle-Height="18" HeaderStyle-Height="30" OnSorting="gv_Sorting" CssClass="filesList"
                OnPageIndexChanging="gv_PageIndexChanging" Width="100%" PageSize="15" GridLines="None" Font-Names="Arial" 
                Font-Size="Small" CellPadding="3" CellSpacing="3" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" 
                ForeColor="Black" DataKeyNames="TagId" OnRowEditing="gv_RowEditing" OnRowDeleting="gv_RowDeleting" 
                EmptyDataText="No tags found for the selected filter." OnDataBound="gv_DataBound" OnRowDataBound="gv_RowDataBound">
                <PagerSettings Position="Bottom" PageButtonCount="15" />
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
                <RowStyle VerticalAlign="Top" Font-Names="Arial" Font-Size="Small" BackColor="#F7F7F7" />
                <FooterStyle Font-Names="Arial" Font-Size="X-Small" BackColor="White" />
                <HeaderStyle BackColor="#666666" HorizontalAlign="Left" VerticalAlign="Middle" Wrap="False" Font-Size="Small" Font-Names="Arial" ForeColor="White" Font-Bold="False" Font-Underline="false" />
                <AlternatingRowStyle BackColor="#E0E0E0" />
                <Columns>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px"> 
                        <ItemTemplate>
                            <asp:LinkButton ID="editButton" runat="server" ToolTip="Edit Tag" CommandName="Edit" Text="Edit" /> 
                            &nbsp;
                            <asp:LinkButton ID="deleteButton" runat="server" ToolTip="Delete Tag" CommandName="Delete" Text="Delete" OnClientClick='<%# GetDeleteJavascript((int)Eval("DocumentCount")) %>' /> 
                        </ItemTemplate> 
                    </asp:TemplateField> 
                    <asp:TemplateField HeaderText="Tag Name <img src='/DesktopModules/Gafware/DMS/Images/sortneutral.png' border='0' alt='Sort by Full Name' />" SortExpression="TagName"> 
                        <ItemTemplate>
                            <a class="documentView" data-id="<%# Eval("TagId") %>" style="color: #<%# Theme %>"><%# Eval("TagName") %></a>
                        </ItemTemplate> 
                    </asp:TemplateField> 
                    <asp:TemplateField HeaderText="Document Count <img src='/DesktopModules/Gafware/DMS/Images/sortneutral.png' border='0' alt='Sort by Document Count' />" SortExpression="DocumentCount" ItemStyle-Width="50px"> 
                        <ItemTemplate>
                            <%# Eval("DocumentCount") %>
                        </ItemTemplate> 
                    </asp:TemplateField> 
                </Columns>
                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            </asp:GridView>
        </asp:Panel>
        <div id="documentListDialog" class="nocontent">
            <div id="documentList-content" class="dialog-content">
                <div class="dms" style="padding: 10px; overflow: auto; height: 560px; width: 100%;">
                    <asp:GridView ID="gvDocuments" runat="server" AutoGenerateColumns="False" CellPadding="3" CellSpacing="3"  EmptyDataText="<br /><strong>No documents found.</strong>"  
                        ForeColor="Black" GridLines="None" DataKeyNames="DocumentId" BackColor="White" BorderColor="#DEDFDE" PageSize="20" BorderStyle="None" BorderWidth="1px" 
                        AllowPaging="False" AllowSorting="False" Width="100%" ShowHeader="True" OnRowDataBound="gvDocuments_RowDataBound" CssClass="filesList">
		                <Columns>
                            <asp:TemplateField HeaderText="ID" HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%# Eval("DocumentId") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Document Name" HeaderStyle-Wrap="false">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="lnkDocumentName" CommandArgument='<%# Eval("DocumentId") %>' CommandName="Details" OnCommand="lnkDocumentName_Command"><%# Eval("DocumentName") %></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Last Modified" HeaderStyle-Wrap="false" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%# ((DateTime)Eval("LastModifiedOnDate")).ToString("MM/dd/yyyy") %>
                                </ItemTemplate>
                            </asp:TemplateField>
		                </Columns>
		                <FooterStyle BackColor="White" />
		                <RowStyle BackColor="#F7F7F7" VerticalAlign="Top" />
		                <EditRowStyle VerticalAlign="Top" />
		                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" VerticalAlign="Top" />
		                <HeaderStyle BackColor="#666666" Font-Bold="False" ForeColor="White" HorizontalAlign="Left" Font-Size="10pt" VerticalAlign="Top" Font-Underline="false" />
		                <AlternatingRowStyle BackColor="White" VerticalAlign="Top" />
	                </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</div>