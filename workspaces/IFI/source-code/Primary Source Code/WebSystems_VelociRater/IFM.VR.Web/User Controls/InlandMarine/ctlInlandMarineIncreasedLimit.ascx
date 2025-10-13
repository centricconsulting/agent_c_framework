<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlInlandMarineIncreasedLimit.ascx.vb" Inherits="IFM.VR.Web.ctlInlandMarineIncreasedLimit" %>

<div id="dvIMLimit" runat="server" class="div">
    <table style="width: 100%" class="tableBorder">
        <tr>
            <td style="width: 15px"></td>
            <td style="width: 100px; vertical-align: top" class="CovTableColumn">
                <asp:TextBox ID="txtIM_LimitData" runat="server" CssClass="CovTableItem" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));'></asp:TextBox>
            </td>
            <td style="width: 100px; vertical-align: top">
                <asp:DropDownList ID="ddlIM_Deductible" runat="server" CssClass="CovTableItemDropDown"></asp:DropDownList>
            </td>
            <td style="vertical-align: top">
                <asp:TextBox ID="txtIM_Description" runat="server" TextMode="MultiLine" Width="175px" ToolTip="250 Max Characters"></asp:TextBox>
                <br />
                <asp:Label ID="lblMaxChar" runat="server" Text="Max Characters:" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>&nbsp;
                <asp:Label ID="lblMaxCharCount" runat="server" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>
            </td>
            <td style="vertical-align: top">
                <asp:TextBox ID="txtIM_StorageLocation" runat="server" TextMode="MultiLine" Width="100px" ToolTip="250 Max Characters"></asp:TextBox>
                <br />
                <asp:Label ID="lblIM_SL_MaxChar" runat="server" Text="Max Characters:" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>&nbsp;
                <asp:Label ID="lblIM_SL_MaxCharCount" runat="server" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>
            </td>
            <td style="width: 100px">
                <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" OnClick="OnConfirm" OnClientClick="ConfirmDialog()">Delete </asp:LinkButton>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hiddenIncreasedLimit" runat="server" Value="0" />
    <asp:HiddenField ID="hiddenMaxCharCount" runat="server" />
    <asp:HiddenField ID="hiddenSLMaxCharCount" runat="server" />
</div>