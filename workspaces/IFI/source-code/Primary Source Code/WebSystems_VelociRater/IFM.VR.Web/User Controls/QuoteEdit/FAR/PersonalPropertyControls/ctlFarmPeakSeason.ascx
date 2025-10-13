<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFarmPeakSeason.ascx.vb" Inherits="IFM.VR.Web.ctlFarmPeakSeason" %>

<%@ Register Assembly="BasicFrame.WebControls.BasicDatePicker" Namespace="BasicFrame.WebControls" TagPrefix="BDP" %>

<div id="dvPeakSeason" runat="server" class="div">
    <table style="width: 100%" class="tableBorder">
        <tr>
            <td style="width: 25px"></td>
            <td style="vertical-align: central">
                <asp:TextBox ID="txtPeak_LimitData" runat="server" CssClass="CovTableItem" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));'></asp:TextBox>
            </td>
            <td style="vertical-align: central">
                <asp:TextBox ID="txtPeak_Start" runat="server" CssClass="CovTableItem"></asp:TextBox>
            </td>
            <td style="vertical-align: central">
                <asp:TextBox ID="txtPeak_End" runat="server" CssClass="CovTableItem"></asp:TextBox>
            </td>
            <td style="vertical-align: central; width: 100px">
                <asp:TextBox ID="txtPeak_Desc" runat="server"></asp:TextBox>
            </td>
            <td style="width: 100px">
                <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" OnClick="OnConfirm" OnClientClick="ConfirmPersPropDialog(this.id, 'False', 0)">Delete </asp:LinkButton>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hiddenIncreasedLimit" runat="server" Value="0" />
    <asp:HiddenField ID="hiddenType" runat="server" />
</div>