Imports System.Data.SqlClient

Public Class VR3QuoteSummaryLoader
    Inherits System.Web.UI.Page

    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass 'added 5/29/2013 to validate user has access to quote

    Dim proposalBytes As Byte() = Nothing '5/29/2013 - renamed from htmlBytes

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            LoadProposal()
        End If
    End Sub
    Private Sub LoadProposal() '5/29/2013 - updated for pdfBytes and proposalId
        If Request.QueryString("quoteid") IsNot Nothing AndAlso Request.QueryString("quoteid").ToString <> "" AndAlso IsNumeric(Request.QueryString("quoteid").ToString) = True Then
            LoadProposalFromDb(Request.QueryString("quoteid").ToString)
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
    Private Sub LoadProposalFromDb(ByVal QuoteId As String)
        If IsNumeric(QuoteId) = True Then
            Dim isOkayToView As Boolean = False
            Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connQQ"))
                sql.queryOrStoredProc = "usp_Get_Quote"
                sql.parameter = New SqlParameter("@QuoteId", CInt(QuoteId))

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
                    Me.lblMessage.Text = "There was a problem locating your summary in the database."
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
End Class