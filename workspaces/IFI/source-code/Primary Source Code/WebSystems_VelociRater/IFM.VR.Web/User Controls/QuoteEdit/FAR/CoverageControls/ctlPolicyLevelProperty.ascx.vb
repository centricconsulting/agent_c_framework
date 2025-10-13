Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
'Imports IFM.VR.Validation.ObjectValidation.AllLines

Public Class ctlPolicyLevelProperty
    Inherits VRControlBase

    Public Property MyLocationIndex As Int32
        Get
            If ViewState("vs_farmLocationIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_farmLocationIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_farmLocationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyFarmLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() AndAlso Me.Quote.Locations.Count > MyLocationIndex Then
                Return Me.Quote.Locations(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            MainAccordionDivId = dvFarmPolicyPropertyCoverage.ClientID
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(MainAccordionDivId, hiddenFarmPropertyCoverage, "0")

        'save event
        VRScript.StopEventPropagation(lnkBtnSave.ClientID)

        'remove event
        VRScript.StopEventPropagation(lnkBtnClear.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function
End Class