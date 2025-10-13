Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Public Class VinHandlerCAP
    Inherits VRGenericHandlerBase

    'Added 06/30/2021 for CAP Endorsements Tasks 53028 and 53030 MLW - to use 3rd party company VINtelligence

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        Dim Vin As String = ""
        Dim Make As String = ""
        Dim Model As String = ""
        Dim Year As String = ""
        Dim EffectiveDate As DateTime = Date.Today
        Dim VersionId As Int32 = 210
        Dim PolicyId As Integer = 0
        Dim PolicyImageNum As Integer = 0
        Dim VehicleNum As Integer = 0
        Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass()

        If context.Request.QueryString("Vin") IsNot Nothing Then
            Vin = context.Request.QueryString("Vin").ToUpper()
        End If

        If context.Request.QueryString("Make") IsNot Nothing Then
            Make = context.Request.QueryString("Make").Trim()
        End If

        If context.Request.QueryString("Model") IsNot Nothing Then
            Model = context.Request.QueryString("Model").Trim()
        End If

        If context.Request.QueryString("Year") IsNot Nothing Then
            Year = context.Request.QueryString("Year")
        End If

        If context.Request.QueryString("EffectiveDate") IsNot Nothing AndAlso IsDate(context.Request.QueryString("EffectiveDate")) Then
            EffectiveDate = CDate(context.Request.QueryString("EffectiveDate"))
        End If

        If Int32.TryParse(Year, Nothing) = False Then
            Year = "0"
        End If

        If context.Request.QueryString("VersionId") IsNot Nothing Then
            VersionId = context.Request.QueryString("VersionId").TryToGetInt32()
        End If

        If context.Request.QueryString("PolicyId") IsNot Nothing Then
            PolicyId = qqh.IntegerForString(context.Request.QueryString("PolicyId"))
        End If

        If context.Request.QueryString("PolicyImageNum") IsNot Nothing Then
            PolicyImageNum = qqh.IntegerForString(context.Request.QueryString("PolicyImageNum"))
        End If

        If context.Request.QueryString("VehicleNum") IsNot Nothing Then
            VehicleNum = qqh.IntegerForString(context.Request.QueryString("VehicleNum"))
        End If
        
        context.Response.Write(GetjSon(IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo_OptionalLookupType(Vin, Make, Model, Year, EffectiveDate, VersionId, Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.VINtelligence, PolicyId, PolicyImageNum, VehicleNum)))

        context.Response.End()
    End Sub


End Class