Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Serialization

Public Class VehicleClassCodeLookup
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache)

        ' Year
        Dim VehYr As String = ""
        If context.Request.QueryString("yr") IsNot Nothing Then
            VehYr = context.Request.QueryString("yr")
        End If

        ' Make
        Dim VehMake As String = ""
        If context.Request.QueryString("mk") IsNot Nothing Then
            VehMake = context.Request.QueryString("mk")
        End If

        ' Model
        Dim VehModel As String = ""
        If context.Request.QueryString("md") IsNot Nothing Then
            VehModel = context.Request.QueryString("md")
        End If

        ' Rating Type
        Dim RatingTypeId As String = ""
        If context.Request.QueryString("rtId") IsNot Nothing Then
            RatingTypeId = context.Request.QueryString("rtId")
        End If

        ' Use Code
        Dim UseCodeId As String = ""
        If context.Request.QueryString("ucId") IsNot Nothing Then
            UseCodeId = context.Request.QueryString("ucId")
        End If

        ' Operator 
        Dim OperatorId As String = ""
        If context.Request.QueryString("opId") IsNot Nothing Then
            OperatorId = context.Request.QueryString("opId")
        End If

        ' Operator Type
        Dim OperatorTypeId As String = ""
        If context.Request.QueryString("optypId") IsNot Nothing Then
            OperatorTypeId = context.Request.QueryString("optypId")
        End If

        ' Size
        Dim SizeId As String = ""
        If context.Request.QueryString("szId") IsNot Nothing Then
            SizeId = context.Request.QueryString("szId")
        End If

        ' Trailer Type
        Dim TrailerTypeId As String = ""
        If context.Request.QueryString("ttId") IsNot Nothing Then
            TrailerTypeId = context.Request.QueryString("ttId")
        End If

        ' Radius
        Dim RadiusId As String = ""
        If context.Request.QueryString("rdId") IsNot Nothing Then
            RadiusId = context.Request.QueryString("rdId")
        End If

        ' Secondary Class
        Dim SecondaryClassId As String = ""
        If context.Request.QueryString("scId") IsNot Nothing Then
            SecondaryClassId = context.Request.QueryString("scId")
        End If

        ' Secondary Class Type
        Dim SecondaryClassTypeId As String = ""
        If context.Request.QueryString("sctId") IsNot Nothing Then
            SecondaryClassTypeId = context.Request.QueryString("sctId")
        End If

        context.Response.Write(GetjSon(IFM.VR.Common.Helpers.VehicleClassCodeLookup.GetVehicleClassCode(VehYr, VehMake, VehModel, RatingTypeId, UseCodeId, OperatorId, OperatorTypeId, SizeId, TrailerTypeId, RadiusId, SecondaryClassId, SecondaryClassTypeId)))

        context.Response.End()

    End Sub

End Class