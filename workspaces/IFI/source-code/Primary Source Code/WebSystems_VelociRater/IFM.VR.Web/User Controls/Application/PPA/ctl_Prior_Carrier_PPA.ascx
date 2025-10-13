<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Prior_Carrier_PPA.ascx.vb" Inherits="IFM.VR.Web.ctl_Prior_Carrier_PPA" %>

<div runat="server" id="divMainPriorCarrier" class="standardSubSection">
    <h3>Prior Carrier Information
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearBase" runat="server" CssClass="RemovePanelLink" Style="display: none;" ToolTip="Clear Prior Carrier Information">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <table style="width: 100%;">
            <tr>
                <td>
                    <label for="<%=Me.ddName.ClientID%>">*Previous Insurer</label></td>
                <td>
                    <asp:DropDownList ID="ddName" runat="server"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.txtDuration.ClientID%>">Duration w/Company</label></td>
                <td>
                    <asp:TextBox ID="txtDuration" MaxLength="3" onkeyup='$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));' runat="server"></asp:TextBox>
                    <asp:DropDownList ID="ddDurationScale" runat="server"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.txtPolicyNumber.ClientID%>">Policy Number</label></td>
                <td>
                    <asp:TextBox ID="txtPolicyNumber" MaxLength="30" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.txtExpirationDate.ClientID%>">*Expiration Date</label></td>
                <td>
                    <asp:TextBox ID="txtExpirationDate" MaxLength="10" runat="server"></asp:TextBox></td>
            </tr>
        </table>
    </div>
</div>
<asp:HiddenField ID="hiddenAccordActive" runat="server" />