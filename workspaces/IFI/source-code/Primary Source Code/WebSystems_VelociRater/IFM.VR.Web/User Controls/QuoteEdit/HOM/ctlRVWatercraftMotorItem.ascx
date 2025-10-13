<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlRVWatercraftMotorItem.ascx.vb" Inherits="IFM.VR.Web.ctlRVWatercraftMotorItem" %>

<script type="text/javascript">
    $(function () {
        $("#MotorInputDiv").accordion({
            collapsible: false
        });
    });
</script>

<header>
    <style>
        .LabelColumn {
            width: 170px;
            text-align: right;
        }

        .DataColumn {
            width: 150px;
        }
    </style>
</header>

<h3>
    <asp:Label ID="lblMotorTitle" runat="server" Text="Motor"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkRemove" runat="server" CssClass="RemovePanelLink">Clear</asp:LinkButton>
    </span>
</h3>

<div id="MotorInputDiv">
    <h3>Motor</h3>
    <table>
        <tr>
            <td class="LabelColumn">
                <asp:Label ID="Label1" runat="server" Text="Type"></asp:Label>
            </td>
            <td class="DataColumn">
                <asp:DropDownList ID="ddlMotorType" runat="server" TabIndex="1"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="label2" runat="server" Text="Year"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtYear" runat="server" TabIndex="2"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="label3" runat="server" Text="Manufacturer"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtManufacturer" runat="server" TabIndex="3"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="label4" runat="server" Text="Model"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtModel" runat="server" TabIndex="4"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="label5" runat="server" Text="Serial Number"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtSerialNumber" runat="server" TabIndex="5"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="label6" runat="server" Text="Cost New"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtCostNew" runat="server" TabIndex="6"></asp:TextBox>
            </td>
        </tr>
    </table>
</div>
<div align="center">
    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>
</div>