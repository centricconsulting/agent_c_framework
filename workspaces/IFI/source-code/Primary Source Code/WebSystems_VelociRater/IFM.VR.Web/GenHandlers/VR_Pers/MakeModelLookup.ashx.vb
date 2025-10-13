Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Serialization

Class AutoCompleteEntry
    Public Property id As Int32
    Public Property label As String
    Public Property value As String

    Public Sub New()

    End Sub

    Public Sub New(id As String, label As String, value As String)
        Me.id = id
        Me.label = label
        Me.value = value
    End Sub
End Class

Public Class MakeModelLookup
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)

        context.Response.Clear()
        context.Response.ContentType = "application/json"
        If context.Request.QueryString("term") IsNot Nothing Then
            If context.Request.QueryString("GetMakes") IsNot Nothing Then
                If context.Request.QueryString("Year") IsNot Nothing AndAlso Integer.TryParse(context.Request.QueryString("Year"), Nothing) Then ' added ability to only get actual makes for a given year - Matt A 5/11/2016
                    Dim makes = IFM.VR.Common.Helpers.PPA.MakeModelListLookup.GetMakes(Integer.Parse(context.Request.QueryString("Year")))
                    Dim l As New List(Of AutoCompleteEntry)
                    For Each make As String In From m In makes Where m.ToLower().StartsWith(context.Request.QueryString("term").ToLower()) Select m
                        l.Add(New AutoCompleteEntry(0, make, make))
                    Next
                    context.Response.Write(GetjSon(l))
                Else
                    Dim makes = IFM.VR.Common.Helpers.PPA.MakeModelListLookup.GetMakes()
                    Dim l As New List(Of AutoCompleteEntry)
                    For Each make As String In From m In makes Where m.ToLower().StartsWith(context.Request.QueryString("term").ToLower()) Select m
                        l.Add(New AutoCompleteEntry(0, make, make))
                    Next
                    context.Response.Write(GetjSon(l))
                End If
            End If

            If context.Request.QueryString("GetModels") IsNot Nothing Then
                Dim model As String = context.Request.QueryString("GetModels")
                If model.ToUpper() = "Nissan".ToUpper() Then
                    model = "Nissan/Datsun".ToUpper()
                End If
                If context.Request.QueryString("Year") IsNot Nothing AndAlso Integer.TryParse(context.Request.QueryString("Year"), Nothing) Then
                    Dim makes = IFM.VR.Common.Helpers.PPA.MakeModelListLookup.GetModels(model, context.Request.QueryString("Year"))
                    Dim l As New List(Of AutoCompleteEntry)
                    For Each make As String In From m In makes Where m.ToLower().StartsWith(context.Request.QueryString("term").ToLower()) Select m
                        l.Add(New AutoCompleteEntry(0, make, make))
                    Next
                    context.Response.Write(GetjSon(l))
                Else
                    Dim makes = IFM.VR.Common.Helpers.PPA.MakeModelListLookup.GetModels(model)
                    Dim l As New List(Of AutoCompleteEntry)
                    For Each make As String In From m In makes Where m.ToLower().StartsWith(context.Request.QueryString("term").ToLower()) Select m
                        l.Add(New AutoCompleteEntry(0, make, make))
                    Next
                    context.Response.Write(GetjSon(l))
                End If

            End If
        End If

        context.Response.End()

    End Sub


End Class