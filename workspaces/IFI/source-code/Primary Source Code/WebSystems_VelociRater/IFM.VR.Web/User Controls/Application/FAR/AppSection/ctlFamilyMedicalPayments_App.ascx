<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFamilyMedicalPayments_App.ascx.vb" Inherits="IFM.VR.Web.ctlFamilyMedicalPayments_App" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/ctlResidentName_App.ascx" TagPrefix="uc1" TagName="ctlResidentName_App" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/ctlResidentName_Commercial_App.ascx" TagPrefix="uc1" TagName="ctlResidentName_Commercial_App" %>

<div>
    <span id="titleText" runat="server">Family Medical Payments</span>
    <div style="margin-left: 30px;">
        <div runat="server" id="divRecords">
            <table>
                <tr>
                    <td>*First Name</td>
                    <td>*Last Name</td>
                    <td>*DOB</td>
                    <td>*Relationship</td>
                    <td></td>
                </tr>
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>
                        <uc1:ctlResidentName_App runat="server" ID="ctlResidentName_App" />
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
        <div id="CommData" runat="server" visible="false">
            <table>
            <tr>
                <td>Commercial Name 1</td>
                <td>Commercial Name 2</td>
                <td></td>
            </tr>
            <asp:Repeater ID="RepeaterComm" runat="server">
                <itemtemplate>
                    <uc1:ctlResidentName_Commercial_App runat="server" ID="ctlResidentName_Commercial_App" />
                </itemtemplate>
            </asp:Repeater>
            </table>
        </div>

        <div id="dvFamMedPayAddNew" style="float: right;">
            <asp:LinkButton ID="lnkAdd" ToolTip="Add New Family Medical Payments Record" runat="server">Add New</asp:LinkButton>
        </div>
    </div>
</div>