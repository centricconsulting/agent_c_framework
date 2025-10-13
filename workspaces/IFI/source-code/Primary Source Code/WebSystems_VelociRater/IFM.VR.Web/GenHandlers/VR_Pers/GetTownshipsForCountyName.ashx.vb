Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports IFM.PrimativeExtensions

Public Class GetTownshipsForCountyName
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"
        Dim countyName As String = context.Request.QueryString("countyname")
        Dim versionId As String = context.Request.QueryString("versionId")

        Dim stateId As String = context.Request.QueryString("stateId")
        Dim lobId As String = context.Request.QueryString("lobId")
        Dim effDate As String = context.Request.QueryString("effDate")

        Dim results As List(Of String())
        If String.IsNullOrWhiteSpace(versionId) = False Then
            results = IFM.VR.Common.Helpers.FARM.TownshipLookup.GetTownships(countyName, versionId.TryToGetInt32())
        Else
            results = IFM.VR.Common.Helpers.FARM.TownshipLookup.GetTownships(countyName, stateId.TryToGetInt32(), lobId.TryToGetInt32(), effDate)
        End If

        context.Response.Write(GetjSon(results))
        context.Response.End()
    End Sub



End Class