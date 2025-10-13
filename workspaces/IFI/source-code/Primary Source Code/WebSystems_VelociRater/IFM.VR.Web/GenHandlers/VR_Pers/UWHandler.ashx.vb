Imports System.Web
Imports System.Web.Services
Imports QuickQuote.CommonMethods

Public Class UWHandler
    Implements System.Web.IHttpHandler, IRequiresSessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim quoteid As String = Nothing

        context.Response.Clear()
        context.Response.ContentType = "application/json"

        If context.Request.QueryString("quoteId") IsNot Nothing Then quoteid = context.Request.QueryString("quoteId")

        If quoteid IsNot Nothing Then
            If UserHasAgencyIdAccess(quoteid) Then
                Dim ok As String = ArchiveQuote(quoteid)
                context.Response.Write(ok)
            End If
        End If
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Function UserHasAgencyIdAccess(quoteid As String) As Boolean
        Dim qqHelper As New QuickQuoteHelperClass()
        Dim qqxml As New QuickQuoteXML()
        Dim qo As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        Dim err As String = Nothing
        Dim agencyid As String = Nothing

        Try
            qqxml.GetQuote(quoteid, qo, err)
            If err IsNot Nothing AndAlso err.Trim() <> "" Then Return False

            agencyid = qo.AgencyId
            Return qqHelper.IsAgencyIdOkayForUser(agencyID)

        Catch ex As Exception
            Return False
        Finally
            qo.Dispose()
        End Try

    End Function

    Private Function ArchiveQuote(ByVal quoteId As String) As String
        Dim QQXML As New QuickQuoteXML()
        Dim errMsg As String = ""
        Dim jsSerializer As New System.Web.Script.Serialization.JavaScriptSerializer()

        Try
            ' TESTING - DON'T EXECUTE THE ARCHIVE
            'Return jsSerializer.Serialize(True)

            ' Archive the quote
            QQXML.ArchiveOrUnarchiveQuote(quoteId, QuickQuoteXML.QuickQuoteArchiveType.Archive, errMsg)
            If errMsg Is Nothing OrElse errMsg.Trim = "" Then
                Return jsSerializer.Serialize(True)
            Else
                Return jsSerializer.Serialize(False)
            End If
        Catch ex As Exception
            Return jsSerializer.Serialize(False)
        End Try
    End Function

End Class