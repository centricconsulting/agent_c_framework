Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports PopupMessageClass

Public Class VRHomeTEST_MGB
    Inherits VRControlBase

    'Protected ReadOnly Property QuoteId As String
    '    Get
    '        If Request.QueryString("quoteid") IsNot Nothing Then
    '            Return Request.QueryString("quoteid")
    '        End If
    '        If Page.RouteData.Values("quoteid") IsNot Nothing Then
    '            Return Page.RouteData.Values("quoteid").ToString()
    '        End If
    '        Return ""
    '    End Get
    'End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Public Overrides Sub Populate()

    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Private Sub VRHomeTEST_MGB_Load(sender As Object, e As EventArgs) Handles Me.Load
        'If Request("QuoteID") IsNot Nothing Then ctlBOPApp.Populate()
        If Request("QuoteID") IsNot Nothing Then
            CPPQuoteSummary.Populate()
        End If
    End Sub

    'Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject
    '    Get
    '        Dim errCreateQSO As String = ""
    '        Return VR.Common.QuoteSave.QuoteSaveHelpers.SetQSOById(QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuoteId, Session, errCreateQSO)
    '    End Get
    'End Property


    'Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    '    Using popup As New PopupMessageObject(Me.Page, "Hello World!", "Classification Name")
    '        With popup
    '            .addCSS = False
    '            .isModal = True
    '            '.isModal = False
    '            .Image = PopupMessageObject.ImageOptions.None
    '            .hideCloseButton = True
    '            .AddButton("OK", True)
    '            .CreateDynamicPopUpWindow()
    '        End With
    '    End Using
    'End Sub

End Class