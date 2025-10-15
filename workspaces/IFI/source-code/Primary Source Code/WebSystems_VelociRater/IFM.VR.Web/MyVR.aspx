<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" CodeBehind="MyVR.aspx.vb" Inherits="IFM.VR.Web.MyVR" %>

<%@ Import Namespace="IFM.VR.Web" %>

<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/MyVelocirater/ctlQuoteSearchaJax.ascx" TagPrefix="uc1" TagName="ctlQuoteSearchaJax" %>
<%@ Register Src="~/User Controls/MyVelocirater/ctlVr3Stats.ascx" TagPrefix="uc1" TagName="ctlVr3Stats" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctlUWQuestionsPopup runat="server" ID="ctlUWQuestionsPopup" />
    <table style="width: 100%;">
        <tr>
            <td style="width: 200px; vertical-align: top;">
                <div style="height: 8px;"></div>

                <div style="margin-left: 5px; margin-top: 7px;">

                    <img alt="" src="images/VR-Search.png" />
                    <span style="font-weight: bold; margin-left: 10px;">Search My Quotes</span>
                    <hr />
                    <span class="label">
                        <label for="<%=Me.txtQuoteNum.ClientID%>">Quote #</label></span>
                    <br />
                    <asp:TextBox ID="txtQuoteNum" runat="server" MaxLength="22"></asp:TextBox><br />
                    <span class="label">
                        <label for="<%=Me.txtClientName.ClientID%>">Customer Name</label></span>
                    <br />
                    <asp:TextBox ID="txtClientName" runat="server" MaxLength="150"></asp:TextBox><br />
                    <span class="label">
                        <label for="<%=Me.ddStatus.ClientID%>">Status</label></span>
                    <br />

                    <asp:DropDownList ID="ddStatus" runat="server" Width="140px">
                    </asp:DropDownList>
                    <br />
                    <span class="label">
                        <label for="<%=Me.ddLob.ClientID%>">LOB</label></span>
                    <br />

                    <asp:DropDownList ID="ddLob" Width="140px" runat="server">
                    </asp:DropDownList>
                    <br />
                    <span class="label">
                        <label for="<%=Me.ddAgent.ClientID%>">Agent User</label></span>
                    <br />
                    <asp:DropDownList ID="ddAgent" Width="140px" runat="server">
                        <asp:ListItem Value="-1">All</asp:ListItem>
                    </asp:DropDownList>
                    <br />
                    <label for="<%=Me.ddTimeFrame.ClientID%>">Time Frame</label><br />
                    <asp:DropDownList ID="ddTimeFrame" runat="server">
                        <asp:ListItem Value="0">All</asp:ListItem>
                        <asp:ListItem Value="30">Last 30 Days</asp:ListItem>
                        <asp:ListItem Value="60">Last 60 Days</asp:ListItem>
                        <asp:ListItem Selected="True" Value="90">Last 90 Days</asp:ListItem>
                    </asp:DropDownList>
                    <br />
                    <label for="<%=Me.chkShowAll.ClientID%>">Show All on One Page</label>
                    <br />
                    <asp:CheckBox ID="chkShowAll" runat="server" Text="" />

                    <div runat="server" id="divShowArchived">
                        <label for="<%=Me.chkShowArchived.ClientID%>">Show Archived</label>
                        <br />
                        <asp:CheckBox ID="chkShowArchived" Checked="true" runat="server" />
                        <br />
                    </div>
                    <br />
                    <input class="StandardButton" onclick='var esc = $.Event("keypress", { keyCode: 13 }); $(".findTopLevelResults").trigger(esc); event.stopPropagation();' id="btnDoSearch" type="button" value="Search" />
                </div>
            </td>
            <td style="vertical-align: top;">
                <uc1:ctlQuoteSearchaJax runat="server" SearchPanelParmIndex="1" ViewableLobs="1,1001" PerPageCount_Starting="8" ID="ctlQuoteSearchaJax" />
                <uc1:ctlQuoteSearchaJax runat="server" SearchPanelParmIndex="2" ViewableLobs="2,1001" PerPageCount_Starting="8" ID="ctlQuoteSearchaJax1" />
                <uc1:ctlQuoteSearchaJax runat="server" SearchPanelParmIndex="3" ViewableLobs="17,1003" PerPageCount_Starting="8" ID="ctlQuoteSearchaJax2" />
                <uc1:ctlQuoteSearchaJax runat="server" SearchPanelParmIndex="4" ViewableLobs="9,20,25,23,21,28,1002" PerPageCount_Starting="8" ID="ctlQuoteSearchaJax3" />
            </td>
        </tr>
    </table>
    <uc1:ctlVr3Stats runat="server" ID="ctlVr3Stats" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/QuoteSearchResults.js")%>"></script>
    <script src="<%=ResolveClientUrl("~/js/qSearch.js")%>"></script>

    <script type="text/javascript">

    var agencyID = "<%=GetAgencyId()%>";
    var showed_ShowAllmessage = false;
    $(document).ready(function () {

        $("#<%=ddLob.ClientId%> option[value='<%=IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Commercial%>']").css('font-weight', 'bold');
        $("#<%=ddLob.ClientId%> option[value='<%=IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Personal%>']").css('font-weight', 'bold');
        $("#<%=ddLob.ClientId%> option[value='<%=IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Farm%>']").css('font-weight', 'bold');

        $("#<%=ddStatus.ClientID%>").change(function () {
            $(".findStatuses").val($(this).val());
        });

        $("#<%=txtClientName.ClientId%>").keyup(function () {
            $(".findCustomerName").val($(this).val());
        });

        $("#<%=txtQuoteNum.ClientID%>").keyup(function () {
            $(".findQuoteId").val($(this).val());
        });

        $("#<%=ddLob.ClientID%>").change(function () {
            $(".findLobs").val($(this).val());
        });

        $("#<%=ddAgent.ClientID%>").change(function () {
            $(".findAgentIds").val($(this).val());
        });

        $("#<%=Me.chkShowAll.ClientID%>").change(function(){
            if ($(this).is(':checked'))
            {
                if (showed_ShowAllmessage == false) { showed_ShowAllmessage = true; alert('Displaying all quotes on one page will cause page to load slowly.'); }
            };
        });

        if (agencyID != "-1") {
            $("#<%=Me.txtClientName.ClientID%>").autocomplete(
                {
                    source: 'GenHandlers/Vr_Pers/ClientNameAutoComplete.ashx?agencyId=' + agencyID
                    , minLength: 2
                    , delay: 300
	            , autofocus: true
	            , select: function (event, ui) {
	                var selectedObj = ui.item;
	                $("#<%=Me.txtClientName.ClientID%>").val(selectedObj.label);
	                //$('#txtSelectedUrl').val(selectedObj.value);
		            return false;
	            }
                });
        }
        if (agencyID != "-1") {
            $("#<%=Me.txtQuoteNum.ClientID%>").autocomplete(
           {
               source: 'GenHandlers/Vr_Pers/ClientNameAutoComplete.ashx?searchQuotes=y&agencyId=' + agencyID
               , minLength: 2
               , delay: 300
           , autofocus: true
           , select: function (event, ui) {
               var selectedObj = ui.item;
               $("#<%=Me.txtQuoteNum.ClientID%>").val(selectedObj.label);
               //$('#txtSelectedUrl').val(selectedObj.value);
               return false;
           }
           });
   }

    });
    </script>
</asp:Content>