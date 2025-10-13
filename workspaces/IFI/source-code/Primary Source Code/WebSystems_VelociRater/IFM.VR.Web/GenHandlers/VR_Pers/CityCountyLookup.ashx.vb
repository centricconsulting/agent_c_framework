Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Serialization

Public Class CityCountyLookup
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        Dim zipcode = ""
        If context.Request.QueryString("zipcode") IsNot Nothing Then
            zipcode = context.Request.QueryString("zipcode")
        End If
        context.Response.Write(GetjSon(IFM.VR.Common.Helpers.GetCityCountyFromZipCode.GetCityCountyFromZipCode(zipcode)))

        context.Response.End()
    End Sub


End Class
