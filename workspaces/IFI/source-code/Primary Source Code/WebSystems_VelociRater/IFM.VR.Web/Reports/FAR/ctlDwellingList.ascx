<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlDwellingList.ascx.vb" Inherits="IFM.VR.Web.ctlDwellingList" %>
<%@ Register Src="~/Reports/FAR/ctlDwelling.ascx" TagPrefix="uc1" TagName="ctlDwelling" %>

<div id="dvDwellingList" runat="server" class="div">
    <asp:DataList ID="dlAddlDwelling" runat="server" Width="100%">
        <%--<AlternatingItemStyle BackColor="LightGray" />--%>
        <ItemTemplate>
            <uc1:ctlDwelling runat="server" ID="ctlDwelling" />
            <br />
        </ItemTemplate>
        <AlternatingItemTemplate>
            <uc1:ctlDwelling runat="server" ID="ctlDwelling" />
            <br />
        </AlternatingItemTemplate>
    </asp:DataList>
</div>