Public Class ctlQuoteSearchaJax
    Inherits System.Web.UI.UserControl

    Public Property PerPageCount_Starting As Int32

    Public Property SortColumn_Starting As String

    Public Property SortDesc_Starting As Boolean

    Public Property LobIds_Starting As String

    Public Property ExcludedLobIds_Starting As String

    Public Property ViewableLobs As String

    Public Property SearchPanelParmIndex As Int32

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager

        Dim test As String = "ifm_qSearch.PerformQSearch('" + divResultsHTML.ClientID + "'"
        test += "," + DirectCast(Me.Page.Master, VelociRater).AgencyID.ToString()
        test += ",0"  'page index
        test += "," + PerPageCount_Starting.ToString() 'per page count
        test += ",'" + SortColumn_Starting + "'" 'sort column
        test += ",'" + SortDesc_Starting.ToString().ToLower() + "'" 'sort desc
        test += ",''" 'quoteid
        test += ",''"  'quote number
        test += ",''" 'agentUsername
        test += ",''" 'client name
        test += ",''" 'statusIds CSV
        test += ",'" + ViewableLobs + "'" ' lobids to search for
        test += ",'" + ExcludedLobIds_Starting + "'" ' excluded LobIds CSV
        test += "," + SearchPanelParmIndex.ToString()
        test += ");"
        _script.AddScriptLine(test)
    End Sub

End Class