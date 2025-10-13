<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlBarnsBuildingsList.ascx.vb" Inherits="IFM.VR.Web.ctlBarnsBuildingsList" %>
<%@ Register Src="~/Reports/FAR/ctlBarnsBuildings.ascx" TagPrefix="uc1" TagName="ctlBarnsBuildings" %>

<div id="dvBuildingList" runat="server" class="div">
    <asp:DataList ID="dlBuilding" runat="server" Width="100%">
        <%--<AlternatingItemStyle BackColor="LightGray" />--%>
        <ItemTemplate>
            <uc1:ctlBarnsBuildings runat="server" ID="ctlBarnsBuildings" />
            <br />
        </ItemTemplate>
        <AlternatingItemTemplate>
            <uc1:ctlBarnsBuildings runat="server" ID="ctlBarnsBuildings" />
            <br />
        </AlternatingItemTemplate>
    </asp:DataList>
</div>