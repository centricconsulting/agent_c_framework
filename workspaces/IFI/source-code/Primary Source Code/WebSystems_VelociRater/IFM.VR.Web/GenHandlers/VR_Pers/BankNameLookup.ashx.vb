Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Serialization

Public Class BankNameLookup
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        Dim routingnumber = ""
        If context.Request.QueryString("routingnumber") IsNot Nothing Then
            routingnumber = context.Request.QueryString("routingnumber")
        End If

        Dim zipLookup As New IFM.VR.ZipCodeRef.ZipCode()
        '0 = zip
        '1 = county
        '2 = state
        '3 > = cities
        Dim listOfZips As List(Of String) = zipLookup.GetCityCnty(routingnumber)

        context.Response.Write(GetjSon(IFM.VR.Common.Helpers.EFTHelper.GetBankNameFromAbaLookUp(routingnumber)))

        context.Response.End()

    End Sub



End Class