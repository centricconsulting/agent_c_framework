Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports IFM.PrimativeExtensions

Public Class ProtectionClassLookup
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"
        Dim stateId As Int32 = If(context.Request.QueryString("stateId") IsNot Nothing AndAlso IsNumeric(context.Request.QueryString("stateId")), context.Request.QueryString("stateId").TryToGetInt32(), 16)
        context.Response.Write(GetjSon(VR.Common.Helpers.ProtectionClassLookupHelper.GetProtectionClassRawData(context.Request.QueryString("city"), CBool(context.Request.QueryString("isCity")), stateId, CBool(context.Request.QueryString("isComm")))))
        context.Response.End()
    End Sub


End Class