<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_PropertyInOpenItem.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_PropertyInOpenItem" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/ClassCode/ctl_CPR_ClassCodeLookup.ascx" TagPrefix="uc1" TagName="CPR_PIO_ClassCodeLookup" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Property #"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Delete Property" >Delete</asp:LinkButton>
        <asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Property Information">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
</h3>

<div>
    <style type="text/css">
        .PIO_LeftColumn {
            width:50%;
            text-align:left;
        }
        .PIO_RightColumn {
            width:50%;
            text-align:left;
        }
    </style>
    <script type="text/javascript">
        function preventBackspace(e) {
            var evt = e || window.event;
            if (evt) {
                var keyCode = evt.charCode || evt.keyCode;
                if (keyCode === 8 || keyCode == 46 || keyCode == 45) {
                    if (evt.preventDefault) {
                        evt.preventDefault();
                    } else {
                        evt.returnValue = false;
                    }
                }
            }
        }
    </script>
    <uc1:CPR_PIO_ClassCodeLookup runat="server" ID="ctl_PIOClassCodeLookup"></uc1:CPR_PIO_ClassCodeLookup>
    <table id="tblProperty" runat="server" style="width:100%;">
        <tr id="trPropertyInputRow" runat="server">
            <td colspan="2">
                <table ID="tblPropertyFields" runat="server" Style="width:100%;">
                    <tr>
                        <td colspan="2">
                            Special Class Code
                            <br />
                            <table style="width:100%;">
                                <tr>
                                    <td style="text-align:left;width:65%;">
                                        <asp:TextBox ID="txtSpecialClassCode" runat="server" onKeyDown="preventBackspace();" BackColor="#cccccc" onkeypress='return false' Width="100%"></asp:TextBox>
                                    </td>
                                    <td style="text-align:right;vertical-align:top;width:33%;">
                                        <asp:Button ID="btnClassCodeLookup" runat="server" Text="Class Code Lookup" />
                                    </td>
                                    <td style="text-align:left;width:1px;">
                                        <asp:TextBox ID="txtClassCodeId" runat="server" style="display:none" Width="1px"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            *Description
                            <br />
                            <asp:TextBox id="txtDescription" runat="server" TextMode="MultiLine" Width="100%" Height="30px" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="PIO_LeftColumn">
                            *Coverage Limit
                            <br />
                            <asp:TextBox ID="txtCoverageLimit" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                        </td>
                        <td class="PIO_RightColumn">
                            Valuation
                            <br />
                            <asp:DropDownList ID="ddValuation" runat="server" style="width:70%;"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="PIO_LeftColumn">
                            &nbsp;
                            <br />
                            <div id="divBlanketCheckbox" runat="server">
                                <asp:CheckBox ID="chkIncludedInBlanketRating" runat="server" />Included in Blanket Rating
                            </div>
                        </td>
                        <td id="tdDeductibleInTheOpen" runat="server" class="PIO_RightColumn">
                            Deductible
                            <br />
                            <asp:DropDownList ID="ddDeductible" runat="server" style="width:70%;"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr id="trBlanketInfoRow" runat="server" style="display:none;">
                        <td colspan="2" class="informationalText">
                            Blanket and/or Agreed Amount require a signed statement of values.  Please forward this to your underwriter upon binding coverage.
                        </td>
                    </tr>
                    <tr>
                        <td class="PIO_LeftColumn">
                            Cause of Loss
                            <asp:DropDownList ID="ddCauseOfLoss" runat="server" style="width:70%;" ></asp:DropDownList>
                            <br />
                        </td>
                        <td class="PIO_RightColumn">
                            &nbsp;
                            <br />
                            <asp:CheckBox ID="chkEarthquake" runat="server" />Earthquake
                        </td>
                    </tr>
                    <tr id="trEarthquakeDeductible" runat="server">
                        <td class="PIO_LeftColumn">
                            <br />
                        </td>
                        <td class="PIO_RightColumn">
                            Earthquake Deductible Options
                            <br />
                            <asp:DropDownList ID="ddEarthquakeDeductible" runat="server" style="width:70%;"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="PIO_LeftColumn">
                            Co-Insurance
                            <asp:DropDownList ID="ddCoinsurance" runat="server" style="width:70%;"></asp:DropDownList>
                            <br />
                        </td>
                        <td class="PIO_RightColumn">
                            &nbsp;
                            <br />
                            <asp:CheckBox ID="chkAgreedAmount" runat="server" />Agreed Amount
                            <div id="divWindHail" runat="server">
                                <asp:CheckBox ID="chkWindHail" runat="server" />Wind Hail Deductible %
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>
