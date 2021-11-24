<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LetterFilter.ascx.cs" Inherits="Gafware.Modules.DMS.LetterFilter" %>
<table cellspacing="0" CellPadding="3" CellSpacing="3" width="100%" border="0">
    <tr>
        <td>
            <asp:repeater id="rptLetters" runat="server" OnItemCommand="rptLetters_ItemCommand" OnItemDataBound="rptLetters_ItemDataBound">
                <itemtemplate>
                    <asp:linkbutton id="lnkLetter" runat="server" CssClass="linkButton" commandname="Filter" commandargument='<%# DataBinder.Eval(Container, "DataItem.Letter")%>' OnClientClick="return showBlockingScreen()">
                        <%# DataBinder.Eval(Container, "DataItem.Letter")%>
                    </asp:linkbutton>
                    <%# GetSpacer(Convert.ToInt32(Container.ItemIndex)) %>
                </itemtemplate>
            </asp:repeater>
        </td>
    </tr>
</table>