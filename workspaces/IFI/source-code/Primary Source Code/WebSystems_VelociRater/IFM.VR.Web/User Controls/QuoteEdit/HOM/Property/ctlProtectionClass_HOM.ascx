<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlProtectionClass_HOM.ascx.vb" Inherits="IFM.VR.Web.ctlProtectionClass_HOM" %>

<asp:HiddenField ID="hiddenSelectedProtectionClassId" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="HiddenSelectedMilesToFireDepartmentID" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="HiddenField1" runat="server" ClientIDMode="Static" />

<asp:Label ID="lblSelectedProtectionClassId" runat="server" ClientIDMode="Static" Text=""></asp:Label>

<div id="ProtectionClassDiv" runat="server" class="standardSubSection">
    <h3>Protection Class
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearProtectionClass" ToolTip="Clear Protection Class" runat="server" CssClass="RemovePanelLink">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveProtectionClass" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="ProtectionClassContentDiv">
        <table style="width: 100%">
            <tr>
                <td style="text-align: right; width: 150px;">
                    <label for="<%=Me.ddlProtectionClass.ClientID%>">*Protection Class</label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlProtectionClass" TabIndex="21" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="color: red;">
                    <div style="margin-left: 30px;">
                        Physical Address required on Policyholder Page or Property Page
                    </div>
                </td>
            </tr>
            <tr runat="server" id="trPC_Feet" style="display: none;">
                <td style="text-align: right;">
                    <label for="<%=Me.ddlFeetToHydrantA.ClientID%>">*Feet to Hydrant</label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlFeetToHydrantA" runat="server" TabIndex="22">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="Over 1,000 Feet" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Within 1,000 Feet" Value="4"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr runat="server" id="trPC_Miles" style="display: none;">
                <td style="text-align: right;">
                    <label for="<%=Me.ddlMilesToFireDepartmentA.ClientID%>">*Miles to Fire Department</label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlMilesToFireDepartmentA" runat="server">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="More than 5 Miles" Value="1"></asp:ListItem>
                        <asp:ListItem Text="5 Miles or Less" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>
</div>

<div id="ProtectionClassDivB" runat="server" class="standardSubSection" visible="false">
    <h3>Protection Class
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearProtectionClassB" ToolTip="Clear Protection Class" runat="server" CssClass="RemovePanelLink">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveProtectionClassB" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="ProtectionClassContentDivB">
        <table style="width: 100%">
            <tr>
                <td style="text-align: right; width: 150px;">
                    <label for="<%=Me.ddlProtectionClassB.ClientID%>">*Protection Class</label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlProtectionClassB" TabIndex="24" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="color: red;">
                    <div style="margin-left: 30px;">
                        Physical Address required on Policyholder Page or Property Page
                    </div>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">
<%--                    <label for="<%=Me.txtFeetToHydrantB.ClientID%>">*Feet to Hydrant</label></td>--%>
                    <label for="<%=Me.ddlFeetToHydrantB.ClientID%>">*Feet to Hydrant</label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlFeetToHydrantB" TabIndex="25" runat="server">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="Over 1,000 Feet" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Within 1,000 Feet" Value="4"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:HiddenField ID="HiddenField2" ClientIDMode="Static" runat="server" />
<%--                    <asp:TextBox ID="txtFeetToHydrantB" TabIndex="25" placeholder="0" onkeyup="$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));" MaxLength="9" runat="server"></asp:TextBox></td>--%>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">
<%--                    <label for="<%=Me.txtMilesToFireDepartmentB.ClientID%>">*Miles to Fire Department</label></td>--%>
                    <label for="<%=Me.ddlMilesToFireDepartmentB.ClientID%>">*Miles to Fire Department</label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlMilesToFireDepartmentB" runat="server" TabIndex="26"></asp:DropDownList>
<%--                    <asp:TextBox TabIndex="26" ID="txtMilesToFireDepartmentB" placeholder="0" onkeyup="$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));" MaxLength="5" runat="server"></asp:TextBox></td>--%>
                </td>
            </tr>
        </table>
    </div>
</div>

<div id="ProtectionClassDivC" runat="server" class="standardSubSection" visible="false">
    <h3>Protection Class
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearProtectionClassC" ToolTip="Clear Protection Class" runat="server" CssClass="RemovePanelLink">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveProtectionClassC" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="ProtectionClassContentDivC">
        <table style="width: 100%">
            <tr>
                <td style="text-align: right;">
                    <label for="<%=Me.ddlFeetToHydrantC.ClientID%>">*Feet to Hydrant</label></td>
                <td>
                    <asp:DropDownList ID="ddlFeetToHydrantC" runat="server" TabIndex="27">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="Over 1,000 Feet" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Within 1,000 Feet" Value="4"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:HiddenField ID="hdnSelectedFeetToHydrantC" ClientIDMode="Static" runat="server" />
                </td>
                <td style="text-align: right; width: 150px;">
                    <%--<label for="<%=Me.txtProtectionClassC.ClientID%>">System Generated Protection Class</label>--%>
                    <asp:Label id="lblProtectionClassC" runat="server" AssociatedControlID="txtProtectionClassC"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtProtectionClassC" runat="server" Width="100px" TabIndex="28" ReadOnly="true"></asp:TextBox>                    
                </td>
            </tr>
            <tr>
                <td style="text-align: right; width: 150px;">
                    <label for="<%=Me.txtMilesToFireDepartmentC.ClientID%>">Miles to Fire Department</label>
                </td>
                <td>
                    <asp:TextBox ID="txtMilesToFireDepartmentC" runat="server" Width="100px" ReadOnly="true" TabIndex="27"></asp:TextBox>                    
                </td>
                <td style="text-align: right; width: 150px;">
                    <label for="<%=Me.txtNameOfFireDepartmentC.ClientID%>">Name of Fire Department</label>
                </td>
                <td>
                    <asp:TextBox ID="txtNameOfFireDepartmentC" runat="server" ReadOnly="true" Width="100px" TabIndex="29"></asp:TextBox>                    
                </td>
            </tr>
        </table>
    </div>
</div>