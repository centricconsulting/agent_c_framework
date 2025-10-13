Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.FarmLines
Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports IFM.PolicyLoader.QuickQuote
Imports IFM.PolicyLoader

Public Class ctlUmbrellaPolicyCoverages
    Inherits VRControlBase

    Public Event QuoteRateRequested()

    Public ReadOnly UmbrellaDictionaryName As String = "UmbrellaUnderlyingDetails"
    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If _devDictionaryHelper Is Nothing Then
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(UmbrellaDictionaryName) = False Then
                    _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, UmbrellaDictionaryName)
                End If
            End If
            Return _devDictionaryHelper
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(dvUmbrellaPolicyCoverages.ClientID, hdnAccordGenInfo, "0", False)

        VRScript.StopEventPropagation(lnkClearGeneralInfo.ClientID, False)
        VRScript.StopEventPropagation(lnkSaveGeneralInfo.ClientID, False)

        btnGetPolicyInfo.Attributes.Add("onclick", "tester();")
        btnSaveAndRate.Attributes.Add("onclick", "tester();")

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        'If Quote IsNot Nothing Then
        btnSaveAndRate.Enabled = False
        'End If

        PopulateChildControls()
    End Sub




    Public Overrides Function Save() As Boolean
        SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        ValidateChildControls(valArgs)
        'PopulateChildControls()
    End Sub

    Protected Sub btnGetPolicyInfo_Click(sender As Object, e As EventArgs) Handles btnGetPolicyInfo.Click
        ' Clearing Both Lists
        SubQuoteFirst?.UnderlyingPolicies.Clear()
        ddh.ClearMasterDictionaryValue()

        ctl_FUPPUP_UnderlyingPolicies.VerifyPolicies()
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(DefaultValidationType)))
        Populate_FirePopulateEvent()
        If Me.ValidationSummmary.HasErrors() = False AndAlso GoverningStateQuote.UnderlyingPolicies.Count > 0 Then
            btnSaveAndRate.Enabled = True
        End If

    End Sub

    Protected Sub lnkSaveGeneralInfo_Click(sender As Object, e As EventArgs) Handles lnkSaveGeneralInfo.Click, btnSaveAndRate.Click
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(DefaultValidationType)))

        If sender Is btnSaveAndRate Then
            If ValidationSummmary.HasErrors = False Then
                'Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
                RaiseEvent QuoteRateRequested()
            Else
                Populate()
            End If
        Else
            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(DefaultValidationType)))
        End If
    End Sub

    Protected Sub lnkClearGeneralInfo_Click(sender As Object, e As EventArgs) Handles lnkClearGeneralInfo.Click
        ClearControl()
        Populate()
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        ClearChildControls()
    End Sub

    Public Sub VerifyUnderlyingPolicies()
        ctl_FUPPUP_UnderlyingPolicies.VerifyPolicies()
    End Sub



End Class