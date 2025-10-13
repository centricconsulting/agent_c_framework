<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFamFarmCorpList.ascx.vb" Inherits="IFM.VR.Web.ctlFamFarmCorpList" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/FamFarmCorpControls/ctlFamFarmCorpItem.ascx" TagPrefix="uc1" TagName="ctlFamFarmCorpItem" %>


<div id="dvFamFarmCorpList" runat="server" class="standardSubSection">
    <asp:Repeater ID="aiRepeater" runat="server">
        <ItemTemplate>
            <table style="width: 100%">
                <tr>
                    <td>
                        <uc1:ctlFamFarmCorpItem runat="server" id="ctlFamFarmCorpItem" />
                    </td>
                </tr>
            </table>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <table style="width: 100%" class="table">
                <tr>
                    <td style="background-color: lightgray">
                        <uc1:ctlFamFarmCorpItem runat="server" ID="ctlFamFarmCorpItem" />
                    </td>
                </tr>
            </table>
        </AlternatingItemTemplate>
    </asp:Repeater>
    <span style="float: right;">
        <asp:LinkButton ID="lnkAddAI" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Another</asp:LinkButton>
    </span>
</div>