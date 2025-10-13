<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WCP_Classification.ascx.vb" Inherits="IFM.VR.Web.ctl_WCP_Classification" %>
<%@ Register src="~/User Controls/QuoteEdit/ctlRiskGradeSearch.ascx" TagPrefix="uc1" TagName="ctl_WCP_ClassCodeLookup" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Class Code #"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Delete Classification" >Delete</asp:LinkButton>
        <asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Classification Information">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
</h3>

<div id="divWCPClass" runat="server">
    <style type="text/css">
        .WCPClassColumn {
            width:50%;
            text-align:left;
        }
    </style>
    <uc1:ctl_WCP_ClassCodeLookup id="ctl_ClassCodeLookup" runat="server"></uc1:ctl_WCP_ClassCodeLookup>
    <table id="tblClassification" runat="server" style="width:100%;">
        <tr>
            <td class="WCPClassColumn">
                *Class Code:
                <br />
                <asp:TextBox ID="txtClassCode" runat="server" ReadOnly="true"></asp:TextBox>
            </td>
            <td style="width:50%;text-align:right;">
                <asp:Button ID="btnClassCodeLookup" CssClass="standardSaveButton" runat="server" Text="Class Code Lookup" />
            </td>
        </tr>
        <tr>
            <td class="WCPClassColumn">
                *Payroll:
                <br />
                <asp:TextBox ID="txtEmployeePayroll" runat="server"></asp:TextBox>
            </td>
            <td style="display:none;">
                <asp:TextBox ID="txtID" runat="server" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="WCPClassColumn">
                Description:
                <br />
                <asp:TextBox ID="txtDescription" runat="server" Width="100%" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="color:red;text-align:center;font-size:larger;">
                You <b><u>MUST</u></b> click save after entering each classification!
            </td>
        </tr>
        <tr></tr>
    </table>
    <asp:HiddenField ID="hdnStateId" runat="server" />
    <asp:Label ID="lblClassCodeId" runat="server" style="display:none;"></asp:Label>
    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red"></asp:Label>
    </div>
