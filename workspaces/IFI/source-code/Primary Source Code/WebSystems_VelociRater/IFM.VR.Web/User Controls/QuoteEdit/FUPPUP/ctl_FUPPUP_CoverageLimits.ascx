<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_FUPPUP_CoverageLimits.ascx.vb" Inherits="IFM.VR.Web.ctl_FUPPUP_CoverageLimits" %>

    <div runat="server" id="FUPPUP_divPolicyCoverageLimits">
        <h3>
            Policy Coverage Limits
             <span style="float: right;">
                <asp:LinkButton ID="lnkClearGeneralInfo" runat="server" OnClientClick="var confirmed = confirm('Clear ALL Limits?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Policy Coverage Limits to Default Values" CssClass="RemovePanelLink">Clear</asp:LinkButton>
                <asp:LinkButton ID="lnkSaveGeneralInfo" runat="server" ToolTip="Save Policy Coverage" CssClass="RemovePanelLink">Save</asp:LinkButton>
             </span>
        </h3>
        <div>
            <table style="width:100%;">
                <tr>
                    <td style="width:15%;">
                        &nbsp;
                    </td>
                    <td style="width:40%;text-align:left;">
                        *Umbrella Limit
                    </td>
                    <td style="text-align:left;">
                        <asp:DropDownList ID="ddlUmbrellaLimit" runat="server" style="width:144px;"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width:15%;">
                        &nbsp;
                    </td>
                    <td style="width:40%;text-align:left;">
                        *UM/UIM Limit
                    </td>
                    <td style="text-align:left;">
                        <asp:DropDownList ID="ddlUmbrellaUmUimLimit" runat="server" style="width:144px;"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width:15%;">
                        &nbsp;
                    </td>
                    <td>
                        *Self Insured Retention
                    </td>
                    <td style="width:50%;text-align:left;">
                        <asp:TextBox ID="txtSelfInsuredRetention" runat="server" disabled="disabled"></asp:TextBox>                    
                    </td>
                </tr>
            </table>
        </div>
        <asp:HiddenField ID="hdnAccordGenInfo" runat="server" />
    </div>

