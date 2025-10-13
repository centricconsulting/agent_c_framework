<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="billingAccountSummary.ascx.vb" Inherits="IFM.VR.Web.billingAccountSummary" %>
<header>
    <style type="text/css">

        .BillAccountSum {
            clear: both;
            box-sizing: border-box;
        }

        .BillAccountSum table tr td.left{
           padding: 10px 0px 10px 40px;
           width: 65%; 
           margin: 10px;
        }
        .BillAccountSum table tr td.right{
           padding: 10px 40px 10px 0px;
           margin: 10px;
        }
         
    </style>
</header>

<div id="BillAccountSum" class="BillAccountSum" runat="server">
    <h3>
        <asp:Label ID="lblAccountSum" runat="server" Text="Account Summary"></asp:Label>
        <span style="float: right;">
                <asp:LinkButton ID="lnkPrint" CssClass="RemovePanelLink" Style="margin-left: 20px;" runat="server">Printer Friendly</asp:LinkButton>
        </span>
        <br />
        <asp:Label ID="lblTranEffDate" runat="server"></asp:Label>
    </h3>
    <table id="tblMain" runat="server" style="border-collapse: collapse; width: 100%; display: table;">
        <tr>
            <td class="left">
                Pay Plan:<br />
                <asp:Label id="txtPayPlanResult" runat="server" />
            </td>
            <td class="right">
                Bill Method:<br />
                <asp:Label id="txtBillMethod" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="left">
                Balance Due on <asp:Label id="txtBalanceDueDate" runat="server" />:<br />
                <asp:Label id="txtBalanceDueAmount" runat="server" />
            </td>
            <td class="right">
                Current Bill To:<br />
                <asp:Label id="txtBillTo" runat="server" />
            </td>
        </tr>
        <tr>
            <td  class="left">
                Next Activity on <asp:Label id="txtActivityDate" runat="server" />:<br />
                <asp:Label id="txtActivityType" runat="server" /><asp:Label id="txtAmount" runat="server" /> due on <asp:Label id="txtNextDueDate" runat="server" />
            </td>
            <td class="right">
                To Pay In Full:<br />
                <asp:Label id="txtPayInFull" runat="server" />       
            </td>
        </tr>
    </table> 
    <asp:HiddenField ID="hdnAccordGenInfo" runat="server" />
</div>
