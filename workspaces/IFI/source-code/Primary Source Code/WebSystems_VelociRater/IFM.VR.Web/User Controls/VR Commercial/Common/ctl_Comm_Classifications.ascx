<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Comm_Classifications.ascx.vb" Inherits="IFM.VR.Web.ctl_Comm_Classifications" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Building Classification #"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Delete Location" >Delete</asp:LinkButton>
        <asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Location Information">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
</h3>

<div>
    <style type="text/css">
        .LabelColumn {
            width:35%;
            text-align:left;
        }
        .DataColumn {
            width:65%;
            text-align:left;
        }
    </style>
    <table id="tblClassifications" runat="server" style="width:100%;">
        <tr id="trClassInputRow" runat="server">
            <td colspan="2">
                <table ID="tblClassificationFields" runat="server" Style="width:100%;">
                    <tr>
                        <td class="LabelColumn">
                            Program
                        </td>
                        <td class="DataColumn">
                            <asp:DropDownList ID="ddlProgram" runat="server" Width="60%" AutoPostBack="true">
                                <asp:ListItem Value=""></asp:ListItem>
                                <asp:ListItem Value="AP">Apartment</asp:ListItem>
                                <asp:ListItem Value="CTO">Contractors - Office</asp:ListItem>
                                <asp:ListItem Value="CTS">Contractors - Shop</asp:ListItem>
                                <asp:ListItem Value="MO">Motel</asp:ListItem>
                                <asp:ListItem Value="OF">Office</asp:ListItem>
                                <asp:ListItem Value="RS">Restaurant</asp:ListItem>
                                <asp:ListItem Value="RE">Retail</asp:ListItem>
                                <asp:ListItem Value="SE">Service</asp:ListItem>
                                <asp:ListItem Value="WH">Wholesale</asp:ListItem>
                            </asp:DropDownList>
    <%--                        &nbsp;&nbsp;--%>
                            <div style="float:right;">
                                <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_BOPEligibility")%>">Eligibility</a>
                            </div>
                        </td>
                    </tr>
                    <tr id="trProgramMessageRow" runat="server" visible="false">
                        <td colspan="2" style="text-align:left;">
                            <asp:Label ID="lblProgramMessage" runat="server" CssClass="informationalText"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="LabelColumn">
                            Classification
                        </td>
                        <td class="DataColumn">
                            <asp:DropDownList ID="ddlClassification" runat="server" Width="90%" AutoPostBack="true"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="LabelColumn">
                            Class Code
                        </td>
                        <td class="DataColumn">
                            <asp:TextBox ID="txtClassCode" runat="server" Width="50%" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="trAnnualReceiptsRow" runat="server" visible="false">
                        <td class="LabelColumn">
                            Annual Receipts
                        </td>
                        <td class="DataColumn">
                            <asp:TextBox ID="txtAnnualReceipts" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="trMotelInfoRow" runat="server">
                        <td colspan="2" class="informationalText">
                            If the motel also includes a restaurant, enter the combined sales for both.
                        </td>
                    </tr>
                    <tr id="trEmployeePayrollRow" runat="server" visible="false">
                        <td class="LabelColumn">
                            Employee Payroll
                        </td>
                        <td class="DataColumn">
                            <asp:TextBox ID="txtEmployeePayroll" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="trOfficersRow" runat="server" visible="false">
                        <td class="LabelColumn">
                            # of Officers, Partners, Individual Insureds
                        </td>
                        <td class="DataColumn">
                            <asp:TextBox ID="txtNumOfficersPartnersIndInsureds" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="LabelColumn">
                            Primary Classification
                        </td>
                        <td class="DataColumn">
                            <asp:CheckBox ID="chkPrimaryClassification" runat="server" Text="&nbsp;" AutoPostBack="true" CssClass="chkPrimaryClass" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red"></asp:Label>
    </div>






