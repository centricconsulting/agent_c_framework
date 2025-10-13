Imports System.Web
Imports System.Web.Services

Public Class GetCommClassCodes
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        If context.Request.QueryString("query") IsNot Nothing Then
            If context.Request.QueryString("searchType") IsNot Nothing AndAlso context.Request.QueryString("searchText") IsNot Nothing AndAlso context.Request.QueryString("lobId") IsNot Nothing AndAlso context.Request.QueryString("programTypeId") IsNot Nothing Then

                Dim searchType As IFM.VR.Common.Helpers.CGL.ClassCodeHelper.SearchType = Common.Helpers.CGL.ClassCodeHelper.SearchType.Class_Code_Description_Contains
                [Enum].TryParse(Of IFM.VR.Common.Helpers.CGL.ClassCodeHelper.SearchType)(CInt(context.Request.QueryString("searchType")), searchType)

                Dim searchText = HttpUtility.UrlDecode(context.Request.QueryString("searchText"))

                Dim lob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.None
                [Enum].TryParse(Of QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)(CInt(context.Request.QueryString("lobId")), lob)

                Dim programTypeId As Int32 = CInt(context.Request.QueryString("programTypeId"))

                context.Response.Write(GetjSon(IFM.VR.Common.Helpers.CGL.ClassCodeHelper.SearchDBForClassCodes(searchType, searchText, lob, programTypeId)))
            End If

            If context.Request.QueryString("classCodeId") IsNot Nothing AndAlso context.Request.QueryString("lobId") IsNot Nothing AndAlso context.Request.QueryString("programTypeId") IsNot Nothing Then

                Dim classCodeId As Int32 = CInt(context.Request.QueryString("classCodeId"))

                Dim lob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.None
                [Enum].TryParse(Of QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)(CInt(context.Request.QueryString("lobId")), lob)

                Dim programTypeId As Int32 = CInt(context.Request.QueryString("programTypeId"))

                context.Response.Write(GetjSon(IFM.VR.Common.Helpers.CGL.ClassCodeHelper.SearchDBForClassCode(classCodeId, Nothing, lob, programTypeId)))
            End If

            ' Added logic to search by class code (for populate from footnote) MGB 11-1-2017
            If context.Request.QueryString("classCode") IsNot Nothing AndAlso context.Request.QueryString("lobId") IsNot Nothing AndAlso context.Request.QueryString("programTypeId") IsNot Nothing Then

                Dim classCode As String = CInt(context.Request.QueryString("classCode"))

                Dim lob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.None
                [Enum].TryParse(Of QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)(CInt(context.Request.QueryString("lobId")), lob)

                Dim programTypeId As Int32 = CInt(context.Request.QueryString("programTypeId"))

                context.Response.Write(GetjSon(IFM.VR.Common.Helpers.CGL.ClassCodeHelper.SearchDBForClassCode(Nothing, classCode, lob, programTypeId)))
            End If

        End If






        context.Response.End()
    End Sub



End Class