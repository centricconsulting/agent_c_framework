<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Endo_AppliedAdditionalInterest.ascx.vb" Inherits="IFM.VR.Web.ctl_Endo_AppliedAdditionalInterest" %>



<h3>
    <asp:Label ID="lblExpanderText" runat="server" Text="Assigned Additional Interest"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" runat="server">Remove</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" runat="server">Save</asp:LinkButton>
    </span>
</h3>

<div runat="server" id="divAiEntry">
    <style>
        /*re-named from BLDLabelColumn 6/8/2017... so we could use that for the stuff that came over from ctl_BOP_ENDO_App_Location*/
        .BLDAILabelColumn {
            width: 30%;
            text-align: right;
        }

        .BldDDL {
            width: 50%;
        }
        /*added 6/8/2017; originally came from ctl_BOP_ENDO_App_Location and then re-named*/
        .BLDLabelColumn {
            width: 32%;
            text-align: left;
        }

        .BLDDataColumn {
            /*width:10%;*/
            text-align: left;
        }

        .BLDUITextBox {
            width: 75%;
        }
    </style>
    <table style="width: 100%">
        <tr>
            <td>
                <table style="width: 100%">
                    <tr>
                        <td class="BLDAILabelColumn">&nbsp;&nbsp;
                            Location
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlGAILocation" runat="server" CssClass="BldDDL" />
                        </td>
                    </tr>

                    <tr>
                        <td class="BLDAILabelColumn">&nbsp;&nbsp;
                            Description
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlGAIDescriptions" runat="server" CssClass="BldDDL">
                                <asp:ListItem Value="">N/A</asp:ListItem>
                            </asp:DropDownList>
                            <asp:HiddenField ID="hdnAiDescription" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="BLDAILabelColumn">&nbsp;&nbsp;
                            Loss Payee Name
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlGAILimitLossPayeeName" runat="server" CssClass="BldDDL">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="BLDAILabelColumn">&nbsp;&nbsp;
                            Loss Payee Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlGAILimitLossPayeeType" runat="server" CssClass="BldDDL">
                                <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                                <asp:ListItem Value="78">Loss Payable - Building Owner</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="BLDAILabelColumn">&nbsp;&nbsp;
                            ATIMA
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlGAILimitATMA" runat="server" CssClass="BldDDL">
                                <asp:ListItem Value="0">None</asp:ListItem>
                                <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>
