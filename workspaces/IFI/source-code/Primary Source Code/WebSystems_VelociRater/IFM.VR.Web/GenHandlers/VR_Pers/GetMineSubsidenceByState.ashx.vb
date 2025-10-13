Imports System.Web
Imports System.Web.Services
Imports IFM.PrimativeExtensions

Public Class GetMineSubsidenceByState
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        ' can make it to use either or of these or whatever the client side has to send
        Dim stateAbbreviation As String = context.Request.QueryString("stateAbbreviation")
        Dim stateId As String = context.Request.QueryString("stateId")

        Dim results = New List(Of String)

        ' probably better to send nothing back if no state is sent rather than assume it is IN in here. If the client wants to assume that and send IN if it doesn't know for sure that is fine.
        If stateAbbreviation IsNot Nothing Then
            results = IFM.VR.Common.Helpers.MineSubsidenceHelper.MineSubCountiesByStateAbbreviation(stateAbbreviation)
        End If

        If stateId IsNot Nothing Then
            results = IFM.VR.Common.Helpers.MineSubsidenceHelper.MineSubCountiesByStateId(stateId.TryToGetInt32())
        End If

        context.Response.Write(GetjSon(results))
        context.Response.End()
    End Sub

End Class