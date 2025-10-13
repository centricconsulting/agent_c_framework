<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPersonalPropertyList.ascx.vb" Inherits="IFM.VR.Web.ctlPersonalPropertyList" %>
<%@ Register Src="~/Reports/FAR/ctlPersonalProperty.ascx" TagPrefix="uc1" TagName="ctlPersonalProperty" %>

<div id="dvPersPropList" runat="server" class="div">
    <table id="tblPersPropHdr" runat="server" style="width: 100%" class="table">
        <tr style="vertical-align: bottom">
            <td style="width: 50%">
                <asp:Label ID="lblPropCoverage" runat="server" Text="Coverage" Font-Bold="true"></asp:Label>
            </td>
            <td>
                <table style="width: 100%" class="table">
                    <tr style="vertical-align: bottom">
                        <td align="right" style="width: 54%">
                            <asp:Label ID="Label1" runat="server" Text="Limits" Font-Bold="true"></asp:Label>
                        </td>
                        <td align="right" style="width: 23%">
                            <asp:Label ID="lblPropLimit" runat="server" Text="Deductible" Font-Bold="true"></asp:Label>
                        </td>
                        <td align="right" style="width: 23%">
                            <asp:Label ID="lblPropPrem" runat="server" Text="Premium" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:DataList ID="dlPersProp" runat="server">
        <%--<AlternatingItemStyle BackColor="LightGray" />--%>
        <ItemTemplate>
            <uc1:ctlPersonalProperty runat="server" ID="ctlPersonalProperty" />
        </ItemTemplate>
        <AlternatingItemTemplate>
            <uc1:ctlPersonalProperty runat="server" ID="ctlPersonalProperty" />
        </AlternatingItemTemplate>
    </asp:DataList>
    <hr style="border-color: black" />
    <table id="tblTotalPrem" runat="server" style="width: 100%" class="table">
        <tr>
            <td>
                <asp:Label ID="lblTotalPrem" runat="server" Text="Total Farm Personal Property Premium"></asp:Label>
            </td>
            <td align="right">
                <asp:Label ID="lblTotalPremData" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</div>