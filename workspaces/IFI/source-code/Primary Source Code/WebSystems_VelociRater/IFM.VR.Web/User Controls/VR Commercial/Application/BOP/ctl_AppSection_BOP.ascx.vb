Imports QuickQuote.CommonObjects

Public Class ctl_AppSection_BOP
    Inherits VRControlBase

    Public Event QuoteRated()
    Public Event App_Rate_ApplicationRatedSuccessfully()
    Public Event AIChanged()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.div_master_bop_app.ClientID
        AddHandler Me.ctl_AdditionalInterestList.AIChange, AddressOf HandleAIChange
        AttachControlEvents()

        'Added 1/11/2022 for Bug 67521 MLW - Esig Feature Flag
        If Me.hasEsigOption = False Then
            ctl_Esignature.Visible = False
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.CreateAccordion(MainAccordionDivId, hiddendiv_master_bop_app, "0")
        'Me.VRScript.CreateJSBinding(chkIssuedBound.ClientID, "onchange", "Bop.IssuedBoundOnChanged('" & chkIssuedBound.ClientID & "', '" & bdpIssuedBoundDate.ClientID & "');")
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Quote IsNot Nothing Then
            If GoverningStateQuote.HasPhotographyCoverageScheduledCoverages Then
                Me.ctl_PhotogEquip.Visible = True
            Else
                Me.ctl_PhotogEquip.Visible = False
            End If
            'Added 6/26/2019 for Bug 33828 MLW
            If SubQuotes IsNot Nothing Then
                For Each subquote As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                    If subquote.HasBeauticiansProfessionalLiability Then
                        Me.ctl_BOP_App_AdditionalServices.Visible = True
                        Exit For
                    Else
                        Me.ctl_BOP_App_AdditionalServices.Visible = False
                    End If
                Next
            End If
            'If SubQuoteFirst.HasBeauticiansProfessionalLiability Then
            '    Me.ctl_BOP_App_AdditionalServices.Visible = True
            'Else
            '    Me.ctl_BOP_App_AdditionalServices.Visible = False
            'End If
        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub AttachControlEvents()
        AddHandler ctl_AdditionalInterestList.AIChange, AddressOf HandleAIChange
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Protected Sub HandleAIChange()
        'Save_FireSaveEvent(False)
        'ctl_LocationList.Populate()
        'ctl_ContractorsEQList.Populate()
        'updated 9/22/2017
        RaiseEvent AIChanged()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Application"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        ' Issued/Bound is not required...
        'If Me.bdpIssuedBoundDate.SelectedValue Is Nothing Then
        '    Me.ValidationHelper.AddError(bdpIssuedBoundDate, "Issued/Bound Date is required", accordList)
        'End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub ctl_App_Rate_ApplicationRated() Handles ctlApp_Rate.ApplicationRated
        RaiseEvent QuoteRated()
    End Sub

    Private Sub ctl_App_Rate_ApplicationRatedSuccessfully() Handles ctlApp_Rate.ApplicationRatedSuccessfully
        RaiseEvent App_Rate_ApplicationRatedSuccessfully()
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Save_FireSaveEvent()
    End Sub

    'added 9/22/2017
    Public Sub PopulateLocationAndContractorsEquipmentLists()
        ctl_LocationList.Populate()
        ctl_ContractorsEQList.Populate()
    End Sub

End Class