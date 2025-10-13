<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Driver_App.ascx.vb" Inherits="IFM.VR.Web.ctl_Driver_App" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_LossViolations_App.ascx" TagPrefix="uc1" TagName="ctl_LossViolations_App" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Employment_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Employment_Info_PPA" %>

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
                    <label for="<%=Me.txtDLNumber.ClientID%>">*Drivers License Number</label><br />
                    <asp:TextBox ID="txtDLNumber" runat="server" MaxLength="19" autofocus></asp:TextBox>
                </td>

                <td>
                    <label for="<%=Me.ddDLState.ClientID%>">*Drivers License State</label><br />
                    <asp:DropDownList ID="ddDLState" runat="server"></asp:DropDownList>

                <td>
                    <label for="<%=Me.txtDLDate.ClientID%>">*Drivers License Date</label><br />
                    <asp:TextBox ID="txtDLDate" MaxLength="19" onblur="$(this).val(dateFormat($(this).val()));" runat="server"></asp:TextBox></td>
            </tr>
        </table>
        <uc1:ctl_LossViolations_App runat="server" ID="ctl_LossViolations_App" />

        <uc1:ctl_Employment_Info_PPA runat="server" ID="ctl_Employment_Info_PPA" />
    </div>

    <div runat="server" id="divNoContent">
        <asp:Label runat="server" ID="lbl_No_Content">
        </asp:Label>
    </div>
</div>