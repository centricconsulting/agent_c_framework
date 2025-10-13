<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFarmLocationList.ascx.vb" Inherits="IFM.VR.Web.ctlFarmLocationList" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctlFarmLocation.ascx" TagPrefix="uc1" TagName="ctlFarmLocation_item" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %><%--added 1/7/2020 (Interoperability)--%>

<div id="FarmLocationListControlDivTopMost" runat="server">
    <asp:Panel ID="pnlFarmLocations" runat="server" DefaultButton="btnRateLocation">
        <div id="dvFarmLocations" runat="server">
            <asp:Repeater ID="FarmLocationRepeater" runat="server">
                <ItemTemplate>
                    <uc1:ctlFarmLocation_item runat="server" ID="ctlFarmLocationItem" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div align="center">
            <br />
            <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change" visible="false" />
            <asp:Button ID="btnPersonalProperty" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text=" Farm Personal Property Page" />
            <asp:Button ID="btnIMRVPage" runat="server" Text="IM and RV Watercraft Page" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="IM/RV Watercraft Page" />
            <asp:Button ID="btnRateLocation" runat="server" Text="Rate this Quote" CssClass="StandardSaveButton" Style="height: 26px" />
            <div style="margin-top:5px;"><%--<br /><br />--%><uc1:ctl_RouteToUw runat="server" id="ctl_RouteToUw" Visible="false" /></div><%--added 1/7/2020 (Interoperability)--%>
        </div>
    </asp:Panel>
</div>
<asp:HiddenField ID="hiddenLocationListActive" runat="server" />