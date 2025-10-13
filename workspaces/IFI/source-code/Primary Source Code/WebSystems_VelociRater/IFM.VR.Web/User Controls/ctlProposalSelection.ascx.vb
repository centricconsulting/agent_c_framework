Imports System.Data.SqlClient
Imports QuickQuote.CommonMethods 'add 3/3/17 for switch from DiamondQuickQuote to QuickQuote
Public Class ctlProposalSelection
    Inherits VRControlBase
    Dim conn As String = System.Configuration.ConfigurationManager.AppSettings("conn")
    Dim connDiamond As String = System.Configuration.ConfigurationManager.AppSettings("connDiamond")
    Dim connQQ As String = System.Configuration.ConfigurationManager.AppSettings("connQQ")

    'Dim printForms As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()

        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        _script.AddScriptLine("$(""#PropPrintDiv"").accordion({collapsible: false, heightStyle: ""content""});")
        _script.AddScriptLine("$("".PropPrintSection"").accordion({collapsible: false, heightStyle: ""content""});")

        'End If
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()

        Page.MaintainScrollPositionOnPostBack = True

        If Quote.Database_QuoteId IsNot Nothing Then
            Me.rptPropPrint.DataSource = getQuotes(Quote.Database_QuoteId)
            Me.rptPropPrint.DataBind()
        Else
            ShowMessage("No Quote ID found!")
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean

        Return True

    End Function

    Private Sub Print_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        If Me.Visible Then
            Dim quoteIDhash As New Hashtable
            Dim queryStr As New StringBuilder
            Dim r As Integer = 0

            For Each ri As RepeaterItem In rptPropPrint.Items
                Dim chkSelected As CheckBox = ri.FindControl("CheckBox")
                Dim quoteID As Label = ri.FindControl("quoteID")

                If (chkSelected.Checked) Then
                    quoteIDhash.Add(r, quoteID.Text)
                    r += 1
                End If
            Next

            Dim j = 0
            Do While j < quoteIDhash.Values.Count
                If j = quoteIDhash.Values.Count - 1 Then
                    queryStr.Append(quoteIDhash.Values(j).ToString)
                    OpenNewWindow(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_QuoteProposal") & queryStr.ToString)
                Else
                    queryStr.Append(quoteIDhash.Values(j).ToString & "|")
                End If
                j += 1
            Loop
        End If
    End Sub

    Private Sub Cancel_Click(sender As Object, e As EventArgs) Handles btnReturn.Click
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "0")
    End Sub

    Private Function getQuotes(ByRef quoteid As String) As DataTable
        Dim dt As New DataTable
        Dim dr As SqlDataReader
            Using sq As New SQLselectObject
                Dim param As New SqlClient.SqlParameter("@quoteId", quoteid)
                dr = sq.GetDataReader(connQQ, "usp_Get_Quote", param)

                If dr IsNot Nothing AndAlso dr.HasRows Then

                    While dr.Read
                        If dr.Item("quotestatusid").ToString = "3" Then

                            Using polNum As New PolicyNumberObject
                                polNum.connection = conn
                                Dim qqh As New QuickQuoteHelperClass

                            If qqh.IsAgencyOkayForUser(dr.Item("agencyCode").ToString) Then

                                Dim drAll As SqlDataReader
                                Using sqAll As New SQLselectObject
                                    Dim clientIdParam As New SqlClient.SqlParameter("@clientId", dr.Item("clientID").ToString)
                                    drAll = sqAll.GetDataReader(connQQ, "usp_Get_ProposalQuotes", clientIdParam)
                                    dt.Load(drAll)
                                End Using
                                Return dt
                            Else
                                ShowMessage("This quote is not in your agency or associate agencies.")
                                Return Nothing
                            End If
                        End Using
                        Else

                            ShowMessage("This quote has not been rated.")
                        End If
                    End While
                End If
            End Using
        Return Nothing
    End Function


    Public Sub ShowMessage(ByVal message As String)
        Dim strScript As String = "<script language=JavaScript>alert(""" & message & """);</script>"
        If Me.Visible Then
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "clientScript", strScript)
        End If
    End Sub

    Private Sub OpenNewWindow(ByVal url As String)
        If url <> "" Then
            Dim strScript As String = "<script language=JavaScript>"
            strScript &= " window.open(""" & url & """);"
            strScript &= "</script>"
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "clientScript", strScript)
        End If
    End Sub

    Public Function getLobType(ByRef LobType As String) As String
        Dim LOB As String = ""

        If InStr(UCase(LobType), "CPP") Then
            Return "CPP"
        ElseIf InStr(UCase(LobType), "CPR") Then
            Return "CPR"
        ElseIf InStr(UCase(LobType), "CGL") Then
            Return "CGL"
        ElseIf InStr(UCase(LobType), "BOP") Then
            Return "BOP"
        ElseIf InStr(UCase(LobType), "WCP") Then
            Return "WCP"
        ElseIf InStr(UCase(LobType), "CAP") Then
            Return "CAP"
        End If

        Return ""
    End Function

End Class