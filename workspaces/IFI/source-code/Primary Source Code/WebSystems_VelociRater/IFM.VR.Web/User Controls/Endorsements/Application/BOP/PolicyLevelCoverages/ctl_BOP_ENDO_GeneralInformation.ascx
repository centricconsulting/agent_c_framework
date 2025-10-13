<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_ENDO_GeneralInformation.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_ENDO_GeneralInformation" %>

<div runat="server" id="divMain">
    <h3>General Information
         <span style="float: right;">        
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
    </h3>
    <div>
        <style type="text/css">
            .LabelColumn {
                text-align:left;
                width:70%;
            }
            .DataColumn {
                text-align:left;
                width:30%;
            }
            .WideControl{width:95%;}
            .MediumControl{width:60%;}
            .NarrowControl{width:25%;}
        </style>
              <table>
                  <tr id="trProgramTypeRow" runat="server">
                      <td><label for="<%=Me.ddlProgramType.ClientID%>">*Program Type</label></td>
                      <td>
                          <asp:DropDownList ID="ddlProgramType" runat="server" CssClass="MediumControl"></asp:DropDownList>
                      </td>
                  </tr>
                  <tr>
                      <td><label for="<%=Me.ddlOccurrenceLiabilityLimit.ClientID%>">*Occurrence Liability Limit</label></td>
                      <td>
                          <asp:DropDownList ID="ddlOccurrenceLiabilityLimit" runat="server" CssClass="MediumControl">
                                <asp:ListItem Value="33">300,000</asp:ListItem>
                                <asp:ListItem Value="34">500,000</asp:ListItem>
                                <asp:ListItem Value="56">1,000,000</asp:ListItem>
                            </asp:DropDownList>
                      </td>
                  </tr>
                  <tr>
                      <td><label for="<%=ddlTenantsFireLiability.ClientID%>">*Tenants Fire Liability</label></td>
                      <td>
                          <asp:DropDownList ID="ddlTenantsFireLiability" runat="server" CssClass="MediumControl" >
                                <asp:ListItem Value="9">50,000</asp:ListItem>
                                <asp:ListItem Value="10">100,000</asp:ListItem>
                                <asp:ListItem Value="55">250,000</asp:ListItem>
                                <asp:ListItem Value="34">500,000</asp:ListItem>
                                <asp:ListItem Value="56">1,000,000</asp:ListItem>
                            </asp:DropDownList>
                      </td>
                  </tr>
                  <tr>
                      <td><label for="<%=ddlPropertyDamageLiabilityDeductible.ClientID%>">Liability Deductible</label></td>
                      <td>
                          <asp:DropDownList ID="ddlPropertyDamageLiabilityDeductible" runat="server" CssClass="MediumControl">
                                <asp:ListItem Value="0">N/A</asp:ListItem>
                                <asp:ListItem Value="21">250</asp:ListItem>
                                <asp:ListItem Value="22">500</asp:ListItem>
                                <asp:ListItem Value="24">1000</asp:ListItem>
                                <asp:ListItem Value="75">2500</asp:ListItem>
                            </asp:DropDownList>
                      </td>
                  </tr>
                  <tr>
                      <td><label id="lblApplies" for="<%=ddlPropDmgLiabLimitPerClaimOrOccurrence.ClientID %>">Applies</label></td>
                      <td>
                          <asp:DropDownList ID="ddlPropDmgLiabLimitPerClaimOrOccurrence" runat="server" CssClass="MediumControl" >
                                <asp:ListItem Value="0">N/A</asp:ListItem>
                                <asp:ListItem Value="1">Per Occurrence</asp:ListItem>
                                <asp:ListItem Value="2">Per Claim</asp:ListItem>
                            </asp:DropDownList>
                      </td>
                  </tr>
                  <tr>
                      <td>
                          <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_BusinessMasterSummary")%>"><font style="color:blue;" >BusinessMaster Enhancement Endorsement</font></a>
                      </td>
                      <td>
                          <asp:CheckBox ID="chkBusinessMasterEnhancedEndorsement" runat="server" Checked="true" />
                      </td>
                  </tr>
                  <tr>
                      <td><label for="<%=ddlBlanketRating.ClientID %>">Blanket Rating</label></td>
                      <td>
                           <asp:DropDownList ID="ddlBlanketRating" runat="server" CssClass="WideControl" AutoPostBack="true">
                                <asp:ListItem Value="0">N/A</asp:ListItem>
                                <asp:ListItem Value="1">Combined Building and Personal Property</asp:ListItem>
                                <asp:ListItem Value="2">Building Only</asp:ListItem>
                                <asp:ListItem Value="3">Personal Property Only</asp:ListItem>
                            </asp:DropDownList>
                      </td>
                  </tr>
              </table>                
        <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Text="&nbsp;"></asp:Label>
    </div>
</div>
<asp:HiddenField ID="hddAccord" runat="server" />