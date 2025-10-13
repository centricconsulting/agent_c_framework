Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Public Class VinHandler
    Inherits VRGenericHandlerBase


    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        Dim Vin As String = ""
        Dim Make As String = ""
        Dim Model As String = ""
        Dim Year As String = ""
        Dim EffectiveDate As DateTime = Date.Today
        Dim VersionId As Int32 = 128
        Dim PolicyId As Integer = 0 'Added 10/13/2022 for task 75263 MLW        
        Dim PolicyImageNum As Integer = 0 'Added 10/13/2022 for task 75263 MLW 
        Dim VehicleNum As Integer = 0 'Added 10/13/2022 for task 75263 MLW 
        Dim IsNewBusiness As Boolean = True 'Added 10/18/2022 for task 75263 MLW 
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

        Dim hasVersionId As Boolean = False 'Added 10/18/2022 for task 75263 MLW 
      
        If context.Request.QueryString("VersionId") IsNot Nothing Then
            VersionId = context.Request.QueryString("VersionId").TryToGetInt32()
            hasVersionId = True 'Added 10/18/2022 for task 75263 MLW 
        End If

        If Int32.TryParse(Year, Nothing) = False Then
            Year = "0"
        End If

        'Added 10/13/2022 for task 75263 MLW
        If context.Request.QueryString("PolicyId") IsNot Nothing Then
            PolicyId = qqh.IntegerForString(context.Request.QueryString("PolicyId"))
        End If

        If context.Request.QueryString("PolicyImageNum") IsNot Nothing Then
            PolicyImageNum = qqh.IntegerForString(context.Request.QueryString("PolicyImageNum"))
        End If

        If context.Request.QueryString("VehicleNum") IsNot Nothing Then
            VehicleNum = qqh.IntegerForString(context.Request.QueryString("VehicleNum"))
        End If

        If context.Request.QueryString("IsNewBusiness") IsNot Nothing Then
            IsNewBusiness = qqh.BitToBoolean(context.Request.QueryString("IsNewBusiness"))
        End If

        'Added 10/18/2022 for task 75263 MLW
        If (IsNewBusiness = True OrElse hasVersionId = False) AndAlso EffectiveDate >= CDate("12/01/2022") Then
            VersionId = 245
        End If

        'Updated 10/13/2022 for task 75263 MLW
        'context.Response.Write(GetjSon(IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo(Vin, Make, Model, Year, EffectiveDate, VersionId)))
        Dim lookupType As Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelISORAPA
        If IFM.VR.Common.Helpers.PPA.VinLookup.IsNewModelISORAPALookupTypeAvailable(VersionId, EffectiveDate, IsNewBusiness) Then
            lookupType = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelIsoRapaApi        
        End If     
        context.Response.Write(GetjSon(IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo_OptionalLookupType(Vin, Make, Model, Year, EffectiveDate, VersionId, lookupType, PolicyId, PolicyImageNum, VehicleNum)))
        context.Response.End()
    End Sub


End Class