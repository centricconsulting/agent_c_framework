'5/29/2013 for SQL stuff
Imports System.Data.SqlClient

'added 3/24/2017
Imports QuickQuote.CommonMethods

Partial Class DiamondQuoteProposalLoader
    Inherits System.Web.UI.Page

    Dim qqHelper As New QuickQuoteHelperClass 'added 5/29/2013 to validate user has access to quote

    Dim proposalBytes As Byte() = Nothing '5/29/2013 - renamed from htmlBytes

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            LoadProposal()
        End If
    End Sub
    Private Sub LoadProposal() '5/29/2013 - updated for pdfBytes and proposalId
        If Request.QueryString("proposalId") IsNot Nothing AndAlso Request.QueryString("proposalId").ToString <> "" AndAlso IsNumeric(Request.QueryString("proposalId").ToString) = True Then
            LoadProposalFromDb(Request.QueryString("proposalId").ToString)
        ElseIf Request.QueryString("diamondProposalBinaryId") IsNot Nothing AndAlso qqHelper.IsPositiveIntegerString(Request.QueryString("diamondProposalBinaryId").ToString) = True Then 'added 4/4/2017
            LoadDiamondProposalFromDb(diaProposalBinaryId:=Request.QueryString("diamondProposalBinaryId").ToString)
        ElseIf Request.QueryString("diamondProposalId") IsNot Nothing AndAlso qqHelper.IsPositiveIntegerString(Request.QueryString("diamondProposalId").ToString) = True Then 'added 4/4/2017
            LoadDiamondProposalFromDb(diaProposalId:=Request.QueryString("diamondProposalId").ToString)
        ElseIf Session("htmlBytes") IsNot Nothing Then
            proposalBytes = CType(Session("htmlBytes"), Byte())
            'Response.Clear()
            Response.ContentType = "text/html"
            Response.BinaryWrite(proposalBytes)
            Response.Flush()
            Session("htmlBytes") = Nothing
        ElseIf Session("pdfBytes") IsNot Nothing Then
            proposalBytes = CType(Session("pdfBytes"), Byte())
            'Response.Clear()
            Response.ContentType = "application/pdf"
            Response.BinaryWrite(proposalBytes)
            Response.Flush()
            Session("pdfBytes") = Nothing
        Else
            Me.lblMessage.Text = "The parameters passed to this page were insufficient."
        End If
    End Sub
    Private Sub LoadProposalFromDb(ByVal proposalId As String)
        If IsNumeric(proposalId) = True Then
            Dim isOkayToView As Boolean = False
            Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connQQ"))
                sql.queryOrStoredProc = "usp_Get_ProposalAndQuoteLinks"
                sql.parameter = New SqlParameter("@proposalId", CInt(proposalId))

                Dim dr As SqlDataReader = sql.GetDataReader
                If dr IsNot Nothing AndAlso dr.HasRows = True Then
                    Dim agencyCodes As New List(Of String)
                    While dr.Read
                        If proposalBytes Is Nothing Then
                            If dr.Item("proposalBytes") IsNot DBNull.Value Then
                                proposalBytes = dr.Item("proposalBytes")
                            Else
                                proposalBytes = Nothing
                            End If
                        End If
                        agencyCodes.Add(dr.Item("agencyCode").ToString.Trim)
                    End While
                    If qqHelper.IsHomeOfficeStaffUser = True Then
                        'okay to view
                        isOkayToView = True
                    Else
                        If agencyCodes IsNot Nothing AndAlso agencyCodes.Count > 0 Then
                            For Each ac As String In agencyCodes
                                If qqHelper.IsAgencyOkayForUser(ac) = True Then
                                    isOkayToView = True 'will still check all and overwrite if one is not okay
                                Else
                                    isOkayToView = False
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                Else
                    Me.lblMessage.Text = "There was a problem locating your proposal in the database."
                End If
            End Using
            If isOkayToView = True AndAlso proposalBytes IsNot Nothing Then
                'show it
                Response.ContentType = "application/pdf"
                Response.BinaryWrite(proposalBytes)
                Response.Flush()
            End If
        Else
            Me.lblMessage.Text = "The parameters passed to this page were insufficient."
        End If
    End Sub

    'added 4/4/2017
    Private Sub LoadDiamondProposalFromDb(Optional ByVal diaProposalBinaryId As String = "", Optional ByVal diaProposalId As String = "")
        If qqHelper.IsPositiveIntegerString(diaProposalBinaryId) = True OrElse qqHelper.IsPositiveIntegerString(diaProposalId) = True Then
            Dim isOkayToView As Boolean = False
            Dim qqDiaProposal As QuickQuote.CommonObjects.QuickQuoteDiamondProposal = Nothing
            Dim loadErrorMessage As String = ""
            If qqHelper.IsPositiveIntegerString(diaProposalId) = True Then
                qqDiaProposal = qqHelper.DiamondProposalForLookup(diamondProposalId:=CInt(diaProposalId), errorMessage:=loadErrorMessage)
            Else
                'diaProposalBinaryId should be positive integer string
                qqDiaProposal = qqHelper.DiamondProposalForBinaryId(CInt(diaProposalBinaryId), errorMessage:=loadErrorMessage)
            End If

            If qqDiaProposal IsNot Nothing Then
                If qqDiaProposal.DiamondProposalBytes IsNot Nothing Then
                    proposalBytes = qqDiaProposal.DiamondProposalBytes
                    If qqHelper.IsHomeOfficeStaffUser = True Then 'may not be able to use this (i.e. because it may also allow access to Home Office quotes/policies)
                        'okay to view
                        isOkayToView = True
                    Else
                        Dim agCodes As List(Of String) = qqDiaProposal.AgencyCodes
                        If agCodes IsNot Nothing AndAlso agCodes.Count > 0 Then
                            For Each ac As String In agCodes
                                If qqHelper.IsAgencyOkayForUser(ac) = True Then
                                    isOkayToView = True 'will still check all and overwrite if one is not okay
                                Else
                                    isOkayToView = False
                                    loadErrorMessage = "You do not have access to view this proposal."
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                Else
                    If String.IsNullOrWhiteSpace(loadErrorMessage) = True Then
                        loadErrorMessage = "Unable to find binary for your proposal."
                    End If
                End If
            Else
                If String.IsNullOrWhiteSpace(loadErrorMessage) = True Then
                    loadErrorMessage = "There was a problem locating your proposal in the database."
                End If
            End If

            If isOkayToView = True AndAlso proposalBytes IsNot Nothing Then
                'show it
                Response.ContentType = "application/pdf"
                Response.BinaryWrite(proposalBytes)
                Response.Flush()
            Else
                If String.IsNullOrWhiteSpace(loadErrorMessage) = True Then
                    loadErrorMessage = "There was a problem loading your proposal."
                End If
                Me.lblMessage.Text = loadErrorMessage
            End If
        Else
            Me.lblMessage.Text = "The parameters passed to this page were insufficient."
        End If
    End Sub
End Class
