<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CGL_Classcode.ascx.vb" Inherits="IFM.VR.Web.ctl_CGL_Classcode" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Class Code"></asp:Label>

    <span style="float: right;">
        <asp:LinkButton ID="lnkBtnAdd" runat="server" ToolTip="Add Class Code" CssClass="RemovePanelLink">Add Class Code</asp:LinkButton>
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" ToolTip="Delete this Class code" runat="server">Delete</asp:LinkButton>
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
</h3>
<div>
    <div runat="server" id="divContents">
        <div runat="server" id="divSearch" style="margin-bottom: 20px;">
            <label for="<%=ddearchType.ClientID%>">Search Type</label>
            <asp:DropDownList ID="ddearchType" runat="server" Width="120px">
                <asp:ListItem Value="0">Class Code Description</asp:ListItem>
                <asp:ListItem Value="1">Class Code Description Contains</asp:ListItem>
                <asp:ListItem Value="2">Class Code</asp:ListItem>
            </asp:DropDownList>
            <label for="<%=txtSearchClassCode.ClientID%>">Search Value</label>
            <asp:TextBox ID="txtSearchClassCode" runat="server" Width="120px"></asp:TextBox>
            <asp:Button ID="btnSearch" runat="server" Text="Search" />
            <div id="divSearchResults" runat="server">
                <table>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>
            </div>
        </div>

        <table id="tblClassInfo" runat="server" style="width: 100%">
            <tr>
                <td>Class Code
                <br />
                    <asp:TextBox ID="txtClassCode" runat="server"></asp:TextBox>
                </td>
                <td style="vertical-align: bottom;">Class Code Description
                <br />
                    <asp:TextBox ID="txtClassCodeDescription" Width="300" runat="server"></asp:TextBox>
                </td>
            </tr>
           <%-- <tr>
                <td colspan="2">*Class Code Assignment
                    <br />
                    <asp:DropDownList ID="ddAssignment" runat="server">
                        <asp:ListItem Value="1">Policy</asp:ListItem>
                        <asp:ListItem Value="2">Location</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddAssignmentLocation" runat="server"></asp:DropDownList>
                    <asp:Label ID="lblpremBaseShort" runat="server" Visible="False"></asp:Label>
                </td>

            </tr>--%>
            <tr>
                <td><label for="<%=txtExposure.ClientID%>">*Premium Exposure</label>
                <br />
                    <asp:TextBox ID="txtExposure" runat="server"></asp:TextBox>
                    <br />
                    <asp:Label ID="lblpremBaseShort" runat="server" Visible="False"></asp:Label>
                </td>
                <td style="vertical-align: top;">Class Code Basis
                <br />
                    <asp:TextBox ID="txtBasis" Width="300" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div runat="server" id="divArating" style="margin: 20px; display:none;">
                        <span class="informationalText">This class requires a manual rate. Contact the underwriter for the appropriate A-Rates. Risks with manual rates cannot be quoted or bound without Underwriter approval.
                        </span>
                        <table style="width: 100%">
                            <tr>
                                <td style="text-align: right; padding-right:20px;">
                                    <label for="<%=txtARatePrem.ClientID%>">*Premises/Ops Manual Rate</label>
                                <br />
                                 <asp:TextBox ID="txtARatePrem" runat="server"></asp:TextBox>
                                </td>
                                <td style="padding-left:20px;">
                                    <label for="<%=txtARateProd.ClientID%>">*Product/Comp Manual Rate</label>
                                    <br />
                                    <asp:TextBox ID="txtARateProd" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
           </tr>                    
        <tr>
            <td colspan="2">FootNote
                <br />
                <asp:TextBox ID="txtFootNote" TextMode="MultiLine" Style="text-transform: none;" Height="118px" Width="450px" runat="server"></asp:TextBox>
            </td>
        </tr>
   
        </table>
    </div>
         
</div>
