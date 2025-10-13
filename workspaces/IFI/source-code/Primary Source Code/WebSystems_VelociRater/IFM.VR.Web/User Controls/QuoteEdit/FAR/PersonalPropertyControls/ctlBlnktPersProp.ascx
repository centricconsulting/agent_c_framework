<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlBlnktPersProp.ascx.vb" Inherits="IFM.VR.Web.ctlBlnktPersProp" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlFarmPeakSeasonList.ascx" TagPrefix="uc1" TagName="ctlFarmPeakSeasonList" %>

<table style="width: 100%">
    <tr>
        <td>
            <asp:Label ID="lblBlanketLimit" runat="server" Text="Limit (increments of $5,000)"></asp:Label>
        </td>
        <td></td>
    </tr>
    <tr>
        <td>
            <asp:TextBox ID="txtBlanketLimit" runat="server"></asp:TextBox>
        </td>
        <td>
            <asp:LinkButton ID="lnkDeleteBlanket" runat="server" OnClick="OnBlanketConfirm">Delete</asp:LinkButton>
        </td>
    </tr>
</table>
<asp:CheckBox ID="chkBlanketPeak" runat="server" AutoPostBack="true" />
<asp:Label ID="lblBlanketPeak" runat="server" Text=" Peak Season (G)"></asp:Label>
<div id="dvBlanketPeakSeason" runat="server" class="div" style="display: none">
    <table style="width: 100%">
        <tr>
            <td style="width: 50px"></td>
            <td>
                <asp:Label ID="lblPeakCovGLimit" runat="server" Text="Limit" Width="65px"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblPeakCovGStart" runat="server" Text="Start Date" Width="75px"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblPeakCovGEnd" runat="server" Text="End Date" Width="75px"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblPeakCovGDesc" runat="server" Text="Description"></asp:Label>
            </td>
            <td style="width: 120px"></td>
        </tr>
    </table>
    <uc1:ctlFarmPeakSeasonList runat="server" ID="ctlFarmPeakSeasonList" />
    <div align="right">
        <asp:LinkButton ID="lnkAddPeakCovG" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Peak Season</asp:LinkButton>
    </div>
    <div id="PeakNotice" class="informationalText" runat="server" visible="false">
        <p>
            Note: Existing and newly added Peak Season coverage will automatically be split if it is outside of the policy term after rating.
        </p>
    </div>
    <hr />

    <asp:HiddenField ID="hiddenIncreasedLimit" runat="server" Value="0" />
    <asp:HiddenField ID="hiddenPeakExists" runat="server" />
    <asp:HiddenField ID="hiddenPeakType" runat="server" />
    <asp:HiddenField ID="hiddenOldLimit" runat="server" />
</div>
<div id="divEarthquakeBlanket" runat="server" class="div">
    <asp:CheckBox ID="chkEarthquakeBlanket" runat="server" />
    <asp:Label ID="lblEarthquakeBlanket" runat="server" Text=" Earthquake"></asp:Label>
</div>