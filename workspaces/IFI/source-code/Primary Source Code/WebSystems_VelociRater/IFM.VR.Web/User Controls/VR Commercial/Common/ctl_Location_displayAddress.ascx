<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Location_displayAddress.ascx.vb" Inherits="IFM.VR.Web.ctl_Location_displayAddress" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Workplace #"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Remove Workplace" >Remove</asp:LinkButton>
        <%--<asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Workplace Information">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>--%>
    </span>
</h3>

<div id="divWCPWorkplace" runat="server">
    <style type="text/css">
        .WCPWPColumn {
            width:50%;
            text-align:left;
        }
    </style>
    <table id="tblWorkplace" runat="server" style="width:100%;">
        <tr>
            <td class="WCPWPColumn">
                *Street #:
                <br />
                <asp:TextBox ID="txtStreetNum" runat="server" TabIndex="1"></asp:TextBox>
            </td>
            <td class="WCPWPColumn">
                *ZIP:
                <br />
                <asp:TextBox ID="txtZipcode" runat="server" TabIndex="4"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="WCPWPColumn">
                *Street Name:
                <br />
                <asp:TextBox ID="txtStreetName" runat="server" TabIndex="2"></asp:TextBox>
            </td>
            <td class="WCPWPColumn">
                <label for="<%=Me.ddCityName.ClientID%>">*City:</label><br />
                <asp:DropDownList ID="ddCityName" CausesValidation="false" runat="server"></asp:DropDownList>
                <asp:TextBox ID="txtCity" runat="server" MaxLength="25" TabIndex="5"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="WCPWPColumn">
                Apt/Suite #:
                <br />
                <asp:TextBox ID="txtApartmentNumber" runat="server" TabIndex="3"></asp:TextBox>
            </td>
            <td class="WCPWPColumn">
                *State:
                <br />
                <asp:DropDownList ID="ddState" runat="server" TabIndex="6"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="WCPWPColumn">&nbsp;</td>
            <td class="WCPWPColumn">
                *County:
                <br />
                <asp:TextBox ID="txtCounty" runat="server" TabIndex="7"></asp:TextBox>
            </td>
        </tr>
    </table>
    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red"></asp:Label>    
</div>

