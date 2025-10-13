<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlQuoteSearch.ascx.vb" Inherits="IFM.VR.Web.ctlQuoteSearch" %>

<script type="text/javascript">

    var agencyID = "<%=GetAgencyId()%>";
    var showed_ShowAllmessage = false;
    $(document).ready(function () {

        $("#<%=ddLob.ClientId%> option[value='<%=IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Commercial%>']").css('font-weight', 'bold');
        $("#<%=ddLob.ClientId%> option[value='<%=IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Personal%>']").css('font-weight', 'bold');
        $("#<%=ddLob.ClientId%> option[value='<%=IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Farm%>']").css('font-weight', 'bold');
        $("#<%=ddLob.ClientId%> option[value='<%=IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Umbrella%>']").css('font-weight', 'bold');

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

        $(".ctrl_searchOptions").keypress(function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            if (code == 13) {
                $('#<%= btnSearch.ClientID %>').click();
            }
        });

        $('#<%= btnSearch.ClientID%>').click(function (e) {
            var doOnPageValidation = true;
            var hasValidationErrors = false;
            var validationMessages = '<ul>';

            if (doOnPageValidation === true) {
                var customerName = $('#<%= txtClientName.ClientID%>').val();
                var policyNum = $('#<%= txtQuoteNum.ClientID%>').val();
                var searchType = $('#<%= ddType.ClientID%>').val();

                if (customerName.trim().length > 0 && customerName.trim().length < 4) {
                    validationMessages += '<li class="informationalTextRed">Customer Name must be 4 or more characters.</li>';
                    hasValidationErrors = true;
                }

                switch (searchType){
                    case "1":
                    case "5":
                        if (customerName.trim().length < 1 && policyNum.trim().length < 1){
                            validationMessages += '<li class="informationalTextRed">When searching with the type All or Policies, you must enter a Policy/Quote Number or Customer Name to search for.</li>';
                            hasValidationErrors = true;
                        }
                        break;
                    default:
                        break;
                }

                validationMessages += '</ul>';
                if(hasValidationErrors === true){
                    e.preventDefault();
                    $('<div>' + validationMessages + '</div>').dialog({
                        autoOpen: true,
                        modal: true,
                        title: "Search Validation",
                        closeOnEscape: false,
                        draggable: false,
                        buttons: {
                            'OK': function () { $(this).dialog('close'); }
                        }
                    });
                }
            }
        });

    });
</script>

<div style="margin-left: 5px; margin-top: 7px;" class="ctrl_searchOptions">

    <img alt="" src="images/VR-Search.png" />
    <span style="font-weight: bold; margin-left: 10px;">Search</span>
    <hr />
    <span class="label"><label for="<%=Me.txtQuoteNum.ClientID%>">Quote/Policy #</label></span>
    <br />
    <asp:TextBox ID="txtQuoteNum" runat="server" MaxLength="22"></asp:TextBox><br />    
    <span class="label"><label for="<%=Me.txtClientName.ClientID%>">Customer Name</label></span>
    <br />
    <asp:TextBox ID="txtClientName" runat="server" MaxLength="150"></asp:TextBox><br />
    <span class="label"><label for="<%=Me.ddStatus.ClientID%>">Status</label></span>
    <br />
    <asp:DropDownList ID="ddStatus" runat="server" Width="140px" />
    <br />
    <span class="label"><label for="<%=Me.ddType.ClientID%>">Type</label></span>
    <br />
    <asp:DropDownList ID="ddType" runat="server" Width="140px">
        <asp:ListItem Selected="True" Value="1">All</asp:ListItem>
        <asp:ListItem Value="2">Saved Quotes</asp:ListItem>
        <asp:ListItem Value="3">Saved Changes</asp:ListItem>
        <asp:ListItem Value="4">Saved Billing Updates</asp:ListItem>
        <asp:ListItem Value="5">Policies</asp:ListItem>
    </asp:DropDownList>
    <br />
    <span class="label"><label for="<%=Me.ddLob.ClientID%>">LOB</label></span>
    <br />
    <asp:DropDownList ID="ddLob" Width="140px" runat="server" />
    <br />
    <span class="label"><label for="<%=Me.ddAgent.ClientID%>">Agent User</label></span>
    <br />
    <asp:DropDownList ID="ddAgent" Width="140px" runat="server">
        <asp:ListItem Value="-1">All</asp:ListItem>
    </asp:DropDownList>
    <br />
    <label for="<%=Me.ddTimeFrame.ClientID%>">Time Frame</label><br />
    <asp:DropDownList ID="ddTimeFrame" ToolTip="Limits search to quotes with a last modified date within this range." runat="server" Width="140px">
        <asp:ListItem Selected="True" Value="0">All</asp:ListItem>
        <asp:ListItem Value="15">Last 15 Days</asp:ListItem>
        <asp:ListItem Value="30">Last 30 Days</asp:ListItem>
        <asp:ListItem Value="60">Last 60 Days</asp:ListItem>
        <asp:ListItem Value="90">Last 90 Days</asp:ListItem>
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
    <div runat="server" id="divSearchByQuoteId">
        <label for="<%=txtQuoteId.ClientID%>">Quote Id #</label>
        <br />
        <asp:TextBox ID="txtQuoteId" runat="server"></asp:TextBox>
    </div>
    
    <br />
    <asp:Button Width="100" Height="27" CssClass="StandardButton" Style="margin-top: 15px;" ID="btnSearch" runat="server" Text="Search" />
   
</div>