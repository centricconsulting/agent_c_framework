Imports Diamond.Business.ThirdParty.tuxml
Imports Diamond.Business.ThirdParty.XactwareImportAssignmentService
Imports IndianaFarmersEntity
Imports QuickQuote.CommonMethods

Public Class CopyVrQuotes
    Inherits System.Web.UI.Page

    Private Property FromAgencyCode As String
        Get
            If ViewState("FromAgencyCode") IsNot Nothing AndAlso String.IsNullOrEmpty(ViewState("FromAgencyCode").ToString) = False Then
                Return ViewState("FromAgencyCode").ToString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            ViewState("FromAgencyCode") = value
        End Set
    End Property
    Private Property FromAgencyId As String
        Get
            If ViewState("FromAgencyId") IsNot Nothing AndAlso String.IsNullOrEmpty(ViewState("FromAgencyId").ToString) = False Then
                Return ViewState("FromAgencyId").ToString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            ViewState("FromAgencyId") = value
        End Set
    End Property
    Private Property FromAgencyName As String
        Get
            If ViewState("FromAgencyName") IsNot Nothing AndAlso String.IsNullOrEmpty(ViewState("FromAgencyName").ToString) = False Then
                Return ViewState("FromAgencyName").ToString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            ViewState("FromAgencyName") = value
        End Set
    End Property
    Private Property ToAgencyCode As String
        Get
            If ViewState("ToAgencyCode") IsNot Nothing AndAlso String.IsNullOrEmpty(ViewState("ToAgencyCode").ToString) = False Then
                Return ViewState("ToAgencyCode").ToString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            ViewState("ToAgencyCode") = value
        End Set
    End Property
    Private Property ToAgencyId As String
        Get
            If ViewState("ToAgencyId") IsNot Nothing AndAlso String.IsNullOrEmpty(ViewState("ToAgencyId").ToString) = False Then
                Return ViewState("ToAgencyId").ToString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            ViewState("ToAgencyId") = value
        End Set
    End Property
    Private Property ToAgencyName As String
        Get
            If ViewState("ToAgencyName") IsNot Nothing AndAlso String.IsNullOrEmpty(ViewState("ToAgencyName").ToString) = False Then
                Return ViewState("ToAgencyName").ToString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            ViewState("ToAgencyName") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            ResetLookupAndResults()
            SetFocus(Me.txtFromAgencyCode)
        End If
    End Sub

    Private Sub btnLookup_Click(sender As Object, e As EventArgs) Handles btnLookup.Click
        System.Threading.Thread.Sleep(1000) 'delay 1 second

        ResetLookupAndResults()
        Dim reqFields As List(Of String) = Nothing
        Dim qqHelper As New QuickQuoteHelperClass
        Dim chc As New CommonHelperClass
        If String.IsNullOrWhiteSpace(Me.txtFromAgencyCode.Text) = True Then
            QuickQuoteHelperClass.AddStringToList("from agency code", reqFields)
        End If
        If String.IsNullOrWhiteSpace(Me.txtToAgencyCode.Text) = True Then
            QuickQuoteHelperClass.AddStringToList("to agency code", reqFields)
        End If
        If reqFields IsNot Nothing AndAlso reqFields.Count > 0 Then
            Me.lblLookupText.Text = "Required Field" & If(reqFields.Count > 1, "s", "") & ": " & StringFromListOfString(reqFields)
        Else
            Dim invalidLengthCodes As List(Of String) = Nothing
            If Len(Me.txtFromAgencyCode.Text) < 4 Then
                QuickQuoteHelperClass.AddStringToList("from agency code", invalidLengthCodes)
            End If
            If Len(Me.txtToAgencyCode.Text) < 4 Then
                QuickQuoteHelperClass.AddStringToList("to agency code", invalidLengthCodes)
            End If
            If invalidLengthCodes IsNot Nothing AndAlso invalidLengthCodes.Count > 0 Then
                Me.lblLookupText.Text = "Invalid length" & If(invalidLengthCodes.Count > 1, "s", "") & ": " & StringFromListOfString(invalidLengthCodes)
            Else
                If QuickQuoteHelperClass.isTextMatch(Me.txtFromAgencyCode.Text, Me.txtToAgencyCode.Text, matchType:=QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing) = True Then
                    Me.lblLookupText.Text = "from code and to code cannot match"
                Else
                    Dim invalidCodes As List(Of String) = Nothing
                    Dim fromAgObj As New AgencyObject(Me.txtFromAgencyCode.Text, AgencyObject.SourceSystem.Diamond)
                    Dim toAgObj As New AgencyObject(Me.txtToAgencyCode.Text, AgencyObject.SourceSystem.Diamond)
                    If fromAgObj.HasAgencyInfo = False Then
                        QuickQuoteHelperClass.AddStringToList("from code" & If(fromAgObj.HasError, " (db error)", ""), invalidCodes)
                    Else
                        Me.FromAgencyCode = qqHelper.appendText(fromAgObj.Series6000Code, fromAgObj.AgencyCode, "-")
                        Me.FromAgencyId = fromAgObj.AgencyID
                        Me.FromAgencyName = fromAgObj.AgencyName
                    End If
                    If toAgObj.HasAgencyInfo = False Then
                        QuickQuoteHelperClass.AddStringToList("to code" & If(toAgObj.HasError, " (db error)", ""), invalidCodes)
                    Else
                        Me.ToAgencyCode = qqHelper.appendText(toAgObj.Series6000Code, toAgObj.AgencyCode, "-")
                        Me.ToAgencyId = toAgObj.AgencyID
                        Me.ToAgencyName = toAgObj.AgencyName
                    End If
                    fromAgObj.Dispose()
                    toAgObj.Dispose()
                    If invalidCodes IsNot Nothing AndAlso invalidCodes.Count > 0 Then
                        Me.lblLookupText.Text = "Unable to find: " & StringFromListOfString(invalidCodes)
                    Else
                        Me.lblLookupText.Text = "From Agency: " & Me.FromAgencyName & " - " & Me.FromAgencyCode & " (agencyId " & Me.FromAgencyId & ")"
                        Me.lblLookupText.Text &= "<br />To Agency: " & Me.ToAgencyName & " - " & Me.ToAgencyCode & " (agencyId " & Me.ToAgencyId & ")"
                        If QuickQuoteHelperClass.isTextMatch(Me.FromAgencyCode, Me.ToAgencyCode, matchType:=QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing) = True Then
                            Me.lblLookupText.Text &= "<br />from code and to code cannot match"
                        Else
                            If HasABT() = False Then
                                Me.lblLookupText.Text &= "<br />no ABT record found for fromCode/toCode in Diamond's AgencyBookTransfer table"
                            Else
                                Me.lblLookupText.Text &= "<br />Click 'Copy Quotes' to Continue or 'Reset' to Cancel"
                                Me.btnReset.Visible = True
                                Me.btnGo.Visible = True
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub
    Private Function StringFromListOfString(ByVal strs As List(Of String)) As String
        Dim s As String = ""

        If strs IsNot Nothing AndAlso strs.Count > 0 Then
            For Each str As String In strs
                If String.IsNullOrEmpty(str) = False Then
                    If String.IsNullOrEmpty(s) = False Then
                        s &= ", "
                    End If
                    s &= str
                End If
            Next
        End If

        Return s
    End Function

    Protected Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        System.Threading.Thread.Sleep(1000) 'delay 1 second

        ResetLookupAndResults()
        Me.txtFromAgencyCode.Text = ""
        Me.txtToAgencyCode.Text = ""
        SetFocus(Me.txtFromAgencyCode)
    End Sub
    Private Sub ResetLookupAndResults()
        Me.lblLookupText.Text = ""
        Me.btnReset.Visible = False
        Me.btnGo.Visible = False
        Me.ResultsSection.Visible = False
        Me.lblResults.Text = ""
    End Sub

    Private Sub btnGo_Click(sender As Object, e As EventArgs) Handles btnGo.Click
        System.Threading.Thread.Sleep(1000) 'delay 1 second

        Dim oldAgencyCode As String = Me.FromAgencyCode
        Dim oldAgencyId As String = Me.FromAgencyId
        Dim newAgencyCode As String = Me.ToAgencyCode
        Dim newAgencyId As String = Me.ToAgencyId
        Dim copyLog As String = ""
        Dim clearProducerInfoIfDifferentAgency As Boolean = True 'optional param defaulted to True
        Dim clearClientIdIfDifferentAgency As Boolean = False 'optional param defaulted to True
        Dim notePreviousInfoInDescriptionIfDifferentAgency As Boolean = True 'optional param defaulted to True
        Dim insertCopiedQuoteDatabaseRecords As Boolean = True
        Dim archiveOldQuotes As Boolean = True
        Dim emailToUse As String = "itwebdevelopmentgroup@indianafarmers.com"

        Dim qqXml As New QuickQuoteXML
        qqXml.CopyActiveQuotesFromAgencyToAgency(oldAgencyCode, oldAgencyId, newAgencyCode, newAgencyId, insertCopiedQuoteDatabaseRecords, archiveOldQuotes, copyLog, clearProducerInfoIfDifferentAgency:=clearProducerInfoIfDifferentAgency, clearClientIdIfDifferentAgency:=clearClientIdIfDifferentAgency, notePreviousInfoInDescriptionIfDifferentAgency:=notePreviousInfoInDescriptionIfDifferentAgency, copyLogConfirmationEmailAddress:=emailToUse)
        Me.ResultsSection.Visible = True
        Me.lblResults.Text = copyLog
    End Sub
    Private Function HasABT() As Boolean
        Dim hasIt As Boolean = False
        Dim qqHelper As New QuickQuoteHelperClass

        Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connDiamond"))
            sql.queryOrStoredProc = "select top 1 ABT.agencybooktransfer_id from AgencyBookTransfer as ABT with (nolock) where ABT.status = 1 and ABT.agency_id = " & qqHelper.IntegerForString(Me.FromAgencyId) & " and ABT.to_agency_id = " & qqHelper.IntegerForString(Me.ToAgencyId) & " order by ABT.agencybooktransfer_id desc"
            Dim dr As SqlClient.SqlDataReader = sql.GetDataReader
            If dr IsNot Nothing AndAlso dr.HasRows = True Then
                dr.Read()
                If qqHelper.IntegerForString(dr.Item("agencybooktransfer_id").ToString.Trim) > 0 Then
                    hasIt = True
                End If
            ElseIf sql.hasError = True Then
                'db error
            End If
        End Using

        Return hasIt
    End Function
End Class