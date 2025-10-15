<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_PropertyUpdates_HOM_App.ascx.vb" Inherits="IFM.VR.Web.ctl_PropertyUpdates_HOM_App" %>

<style>
    #tblUpdates tr {
        height: 60px;
    }

    #tblUpdates select {
        width: 175px;
    }

    #tblUpdates input[type="text"] {
        width: 80px;
    }

    .UpdatesCol3 {
        width: 125px;
    }

    .UpdatesRequiredSpans<%=MyLocationIndex.ToString()%> {
        display: inline;
    }
</style>

<script>
    // UPdates Control
    $(document).ready(function () {

        if (IsHouse30OrOver<%=MyLocationIndex.ToString()%>) {
            $(".UpdatesRequiredSpans<%=MyLocationIndex.ToString()%>").show();
        }
        else {
            $(".UpdatesRequiredSpans<%=MyLocationIndex.ToString()%>").hide();
        }

        $(".UpdatesCol3").each(function (t) {
            //TD
            $(this).children("input").each(function (i) {
                // checkboxes
                $(this).change(function () {
                    $(this).siblings("input").each(function (s) {
                        //siblings of input
                        $(this).attr('checked', false);
                    });
                });
            });
        });

        $("#tblUpdates").find("input").each(function (i) {
            if ($(this).attr("type") == "text") {
                $(this).keyup(function () {
                    $(this).val(FormatAsNumericDigitsOnly($(this).val())); // restrict to numeric only
                    if ($(this).val() == '')
                        DoInspectionDateDefault(CheckPropUpdatesFormForData());
                    else
                        DoInspectionDateDefault(true); // you already know form has some data
                });
            }

            if ($(this).attr("type") == "checkbox") {
                $(this).click(function () {
                    if ($(this).is(":checked") == false)
                        DoInspectionDateDefault(CheckPropUpdatesFormForData());
                    else
                        DoInspectionDateDefault(true); // you already know form has some data
                });
            }
        });

        $("#tblUpdates").find("select").each(function (s) {
            $(this).click(function () {
                if ($(this).val() == '')
                    DoInspectionDateDefault(CheckPropUpdatesFormForData());
                else
                    DoInspectionDateDefault(true); // you already know form has some data
            });
        });

    });

    function DoInspectionDateDefault(hasValue) {
        try {
            if ($("#" + InspectiondateClientId).val() != undefined) {
                if (hasValue) {
                    if ($("#" + InspectiondateClientId).val().toString().trim() == '')
                        $("#" + InspectiondateClientId).val(master_effectiveDate);
                }
                else {
                    $("#" + InspectiondateClientId).val('');
                }
            }
        }
        catch(err)
        {}
        
    }

    function CheckPropUpdatesFormForData() {
        var formHasValue = false;

        $("#tblUpdates").find("input").each(function (i) {
            if ($(this).attr("type") == "text") {
                if ($(this).val() != '' && $(this).attr('id') != 'InspectiondateClientId') {
                    formHasValue = true;
                }
            }

            if ($(this).attr("type") == "checkbox") {
                if ($(this).is(":checked")) {
                    formHasValue = true;
                }
            }
        });

        $("#tblUpdates").find("select").each(function (s) {
            if ($(this).val() != '') {
                formHasValue = true;
            }

        });

        return formHasValue;
    }

    // UPdates Control
</script>

<div id="divUpdatesMain" runat="server">
    <h3>
        <asp:Label ID="lblMainAccord" runat="server" Text="Updates"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClear" runat="server" ToolTip="Clear Property Updates" CssClass="RemovePanelLink">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" runat="server" ToolTip="Save Page" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <div id="divMobileWidthHeight" runat="server">
            <table style="width: 100%">
                <tr>
                    <td>
                        <label for="<%= txtMobileHomeLength.ClientID%>">*Mobile Home Length</label>
                        <br />
                        <asp:TextBox ID="txtMobileHomeLength" ToolTip="In Feet" MaxLength="3" runat="server" autofocus></asp:TextBox>ft
                    </td>
                    <td>
                        <label for="<%= txtMobileHomeWidth.ClientID%>">*Mobile Home Width</label>
                        <br />
                        <asp:TextBox ID="txtMobileHomeWidth" ToolTip="In Feet" MaxLength="3" runat="server"></asp:TextBox>ft
                    </td>
                </tr>
            </table>
        </div>
        <table id="tblUpdates" style="width: 100%">
            <colgroup>
                <col />
                <col />
                <col class="UpdatesCol3" />
                <col class="" />
            </colgroup>
            <tr>
                <td><span class="UpdatesRequiredSpans<%=MyLocationIndex.ToString()%>">*</span>Roof Information</td>
                <td>
                    <label for="<%=Me.txtRoofUpdateYear.ClientID%>">Year Updated</label><br />
                    <asp:TextBox ID="txtRoofUpdateYear" runat="server" MaxLength="4" autofocus></asp:TextBox>
                </td>
                <td class="UpdatesCol3">
                    <asp:CheckBox ID="radRoofPartial" runat="server" Text="Partial" />
                    <br />
                    <asp:CheckBox ID="radRoofComplete" Text="Complete" runat="server" />
                </td>
                <td>Type<br />
                    <asp:DropDownList ID="ddRoofType" runat="server" Width="160px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td><span class="UpdatesRequiredSpans<%=MyLocationIndex.ToString()%>">*</span>Central Heat Information</td>
                <td>
                    <label for="<%=Me.txtCentralAirUpdated.ClientID%>">Year Updated</label><br />
                    <asp:TextBox ID="txtCentralAirUpdated" runat="server" MaxLength="4"></asp:TextBox>
                </td>
                <td class="UpdatesCol3">
                    <asp:CheckBox ID="radCentralPartial" runat="server" Text="Partial" />
                    <br />
                    <asp:CheckBox ID="radCentralComplete" runat="server" Text="Complete" />
                </td>
                <td>
                    <asp:DropDownList ID="ddCentralAirType" runat="server" Width="160px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td><span class="UpdatesRequiredSpans<%=MyLocationIndex.ToString()%>">*</span>Electric Service Information</td>
                <td>
                    <label for="<%=Me.txtElectricUpdated.ClientID%>">Year Updated</label><br />
                    <asp:TextBox ID="txtElectricUpdated" runat="server" MaxLength="4"></asp:TextBox>
                </td>
                <td class="UpdatesCol3">
                    <asp:CheckBox ID="radElectricPartial" runat="server" Text="Partial" />
                    <br />
                    <asp:CheckBox ID="radElectricComplete" runat="server" Text="Complete" />
                </td>
                <td>
                    <asp:DropDownList ID="ddElectricType" runat="server" Width="160px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td><span class="UpdatesRequiredSpans<%=MyLocationIndex.ToString()%>">*</span>Plumbing Information</td>
                <td>
                    <label for="<%=Me.txtPlumbingUpdated.ClientID%>">Year Updated</label><br />
                    <asp:TextBox ID="txtPlumbingUpdated" runat="server" MaxLength="4"></asp:TextBox>
                </td>
                <td class="UpdatesCol3">
                    <asp:CheckBox ID="radPlumbingPartial" runat="server" Text="Partial" />
                    <br />
                    <asp:CheckBox ID="radPlumbingComplete" runat="server" Text="Complete" />
                </td>
                <td>
                    <asp:DropDownList ID="ddPlumbingType" runat="server" Width="160px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr runat="server" id="trInspection">
                <td><span class="UpdatesRequiredSpans<%=MyLocationIndex.ToString()%>">*</span>Inspection</td>
                <td>
                    <label for="<%=Me.txtInspectionDate.ClientID%>">Date Updated</label><br />
                    <asp:TextBox ID="txtInspectionDate" runat="server" MaxLength="10"></asp:TextBox>
                </td>
                <td class="UpdatesCol3">
                    <asp:CheckBox ID="radInspectionPartial" runat="server" Text="Partial" />
                    <br />
                    <asp:CheckBox ID="radInspectionComplete" runat="server" Text="Complete" />
                </td>
                <td>
                    <label for="<%=txtInspectionRemarks.ClientID%>">Remarks</label><br />
                    <asp:TextBox ID="txtInspectionRemarks" TextMode="MultiLine" MaxLength="300" ToolTip="Inspection Remarks" runat="server" Height="53px" Width="150px"></asp:TextBox>
                </td>
            </tr>
        </table>

        <div runat="server" id="divFarmAdditionalQuestions">
            <div>
                <asp:CheckBox ID="chkBoxCentralHeat" runat="server" Text="Central Heat" />
            </div>
            <div>
                <label for="<%=ddOccupants.ClientID%>">*Who Occupies this dwelling</label>
                <asp:DropDownList ID="ddOccupants" runat="server"></asp:DropDownList>
                <asp:HiddenField ID="hdnOccupant" runat="server" />
            </div>
        </div>
        <asp:HiddenField ID="hiddenUpdatesAccordActive" runat="server" />
    </div>
</div>