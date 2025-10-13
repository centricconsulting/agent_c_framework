<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Vehicle_App.ascx.vb" Inherits="IFM.VR.Web.ctl_Vehicle_App" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlVehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctlVehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlCoverage_PPA_ScheduledItemsList.ascx" TagPrefix="uc1" TagName="ctlCoverage_PPA_ScheduledItemsList" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Label"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
</h3>

<div>
    <div runat="server" id="divContent">
        <table style="width: 100%;">
            <tr>
                <td>
                    <label for="<%=Me.txtVinNumber.ClientID%>">*VIN:</label>
                    <br />
                    <asp:TextBox ID="txtVinNumber" MaxLength="30" runat="server"></asp:TextBox>
                </td>
                <td>
                    <div runat="server" id="divConfirmResults">
                    </div>
                </td>
            </tr>
        </table>

        <uc1:ctlVehicleAdditionalInterestList runat="server" ID="ctlVehicleAdditionalInterestList" />
        <uc1:ctlCoverage_PPA_ScheduledItemsList runat="server" ID="ctlCoverage_PPA_ScheduledItemsList" />
    </div>
    <div runat="server" id="divNoContent" visible="false">
        <asp:Label runat="server" ID="lblNoContent"></asp:Label>
    </div>
</div>