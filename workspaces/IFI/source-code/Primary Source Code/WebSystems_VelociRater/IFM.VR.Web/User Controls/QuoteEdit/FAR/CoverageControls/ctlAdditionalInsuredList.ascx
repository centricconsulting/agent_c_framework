<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlAdditionalInsuredList.ascx.vb" Inherits="IFM.VR.Web.ctlAdditionalInsuredList" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/CoverageControls/ctlAdditionalInsured.ascx" TagPrefix="uc1" TagName="ctlAdditionalInsured" %>

<div id="dvAdditionalInsuredList" runat="server" class="standardSubSection">
    <asp:Repeater ID="aiRepeater" runat="server">
        <ItemTemplate>
            <table style="width: 100%">
                <tr>
                    <td>
                        <uc1:ctlAdditionalInsured runat="server" ID="ctlAdditionalInsured" />
                    </td>
                </tr>
            </table>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <table style="width: 100%">
                <tr>
                    <td style="background-color: lightgray">
                        <uc1:ctlAdditionalInsured runat="server" ID="ctlAdditionalInsured" />
                    </td>
                </tr>
            </table>
        </AlternatingItemTemplate>
    </asp:Repeater>
</div>