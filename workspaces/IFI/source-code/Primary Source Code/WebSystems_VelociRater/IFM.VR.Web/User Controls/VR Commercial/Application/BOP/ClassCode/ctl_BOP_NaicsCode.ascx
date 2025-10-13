<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_NaicsCode.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_NaicsCode" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/BOP/ClassCode/ctl_BOP_NaicsCodeLookup.ascx" TagPrefix="uc1" TagName="ctl_BOP_NaicsCodeLookup" %>

<script type="text/javascript">
    var VRNaicsCode = new function () {

        this.PerformNaicsCodeLookup = function (searchTypeId, searchTerm, divResults, Description, NaicsCodeId) {
            VRNaicsCode.SearchNaicsCodes($(searchTypeId).val(), $(searchTerm).val(), function (data) {
                $(divResults).html(html);
                var html = "<table style='width:100%;'>";
                html += "<thead>";

                html += "<th style='width:60px;'>";
                html += "</th>";
                html += "<th style='width:60px;text-align:left;'>";
                html += "NAICS Code";
                html += "</th>";
                html += "<th style='width:100%;text-align:left;'>";
                html += "Description"
                html += "</th>";

                html += "</thead>";
                html += "<tbody>";
                if (data != null && data.length > 0) {
                    for (var x = 0; x < data.length; x++) {
                        var result = data[x];
                        html += "<tr>";

                        // Need to update the id's for the current control
                        hdnNaicsDescription = Description;
                        hdnNaicsID = NaicsCodeId;

                        var naicscode = data[x].Code;
                        var rawdesc = data[x].Description;
                        var desc = rawdesc.replace("'", "&apos;");

                        html += "<td>";

                        html += "<input type='button' style='width:100%;' onclick='";
                        html += "$(\"" + NaicsCodeId + "\").val(\"" + naicscode + "\"); ";
                        html += "$(\"" + Description + "\").val(\"" + desc + "\");";
                        // html += "Cpr.SubmitNaicsCodeSelection();CloseNaicsLookupForm();' value='Select' />";
                        html += "CloseNaicsLookupForm();' value='Select' />";

                        html += "</td>";

                        html += "<td>";
                        html += naicscode.toString();
                        html += "</td>";

                        html += "<td>";
                        html += desc.toString();
                        html += "</td>";

                        html += "</tr>";
                    }
                }
                html += "</tbody>";
                html += "</table>";
                $(divResults).html(html);

                $(divResults + " table").DataTable({
                    columnDefs: [
                        { orderable: true, targets: [0, 1, 2] }
                    ],
                    order: [1, 'asc'],
                    searching: false,
                    //pageLength: 20
                }); //, jQueryUI: true 

            });
        }

        // Searches for NAICS Codes based on NAICS Code and a search-term.
        // Result: An array of IFM.VR.Common.Helpers.BOP.NaicsCodeLookupResult
        this.SearchNaicsCodes = function (SearchType, SearchText, callBack) {
            var genHandler = 'GenHandlers/Vr_Comm/NaicsCodeLookup.ashx?query=yes&SearchType=' + encodeURIComponent(SearchType) + '&searchText=' + encodeURIComponent(SearchText) + '&ppopo=' + Math.random().toString().replace('.', '');
            $.getJSON(genHandler, {
                dataType: "json",
                data: "",
                cache: false,
                format: "json"
            })
                .done(function (data) {
                    callBack(data);
                });
        }

    }; // END VRNaicsCode
</script>
<div ID="NAICSSection" runtat="server">
    <h3>
        <asp:Label ID="lblAccordHeader" runat="server" Text="NAICS Code"></asp:Label>

        <span style="float: right;">
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="divClassInfo" runat="server">
        <table id="tblClassInfo" runat="server" style="width: 100%">
            <tr>
                <td colspan="2">*NAICS Code
                    <asp:TextBox ID="txtNaicsCode" runat="server" Width="25%"></asp:TextBox>
                    <asp:Button ID="btnNaicsSearch" runat="server" Text="Find" Width="15%" />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="vertical-align: bottom;">*Description
                    <asp:TextBox ID="txtNaicsDescription" Width="80%" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hdnAccord" runat="server" />
</div>
<uc1:ctl_BOP_NaicsCodeLookup runat="server" ID="ctl_BOP_NaicsCodeLookup" />
