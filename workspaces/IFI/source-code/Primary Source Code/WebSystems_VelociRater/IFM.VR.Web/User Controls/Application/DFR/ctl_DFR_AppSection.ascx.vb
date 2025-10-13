Public Class ctl_DFR_AppSection
    Inherits VRControlBase

    Public Event App_Rate_ApplicationRatedSuccessfully()
    Public Event QuoteRated()
    Public Event UpdatesUWQuestions() 'Added 8/2/2022 for task 75911 MLW

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hiddendiv_master_hom_app, "0")

        Me.VRScript.AddTabIndexControl(Me.ctl_AppPolicyholder.PhoneTypeID, True, False)
        Me.VRScript.AddTabIndexControl(Me.ctl_PropertyUpdates_HOM_App.RoofUpdateID, False, True)
        Me.VRScript.AddTabIndexControl(Me.ctl_PropertyUpdates_HOM_App.RemarksID, True, False)
        Me.VRScript.AddTabIndexControl("select[id*='ddAiType']", False, True)
        Me.VRScript.AddTabIndexControl("input[id*='chkBillToMe']", True, False)
        Me.VRScript.AddTabIndexControl(Me.ctl_Employment_Info_PPA.EmployerNameID, False, True)
        Me.VRScript.AddTabIndexControl(Me.ctl_Employment_Info_PPA.OccupationID, True, False)
        Me.VRScript.AddTabIndexControl(Me.ctl_Billing_Info_PPA.MethodID, False, True)
        Me.VRScript.AddTabIndexControl(Me.ctl_Billing_Info_PPA.BillToID, True, False)
        Me.VRScript.AddTabIndexControl("input[id*='chkAgreeToEftTerms']", False, True)
        Me.VRScript.AddTabIndexControl("input[id*='chkDeclineEftTerms']", True, False)
        Me.VRScript.AddTabIndexControl("input[id*='ctl_Billing_Info_PPA_txtFirstName']", False, True)
        Me.VRScript.AddTabIndexControl("input[id*='ctl_Billing_Info_PPA_txtZipCode']", True, False)
        Me.VRScript.AddTabIndexControl(Me.ctl_Producer.ProducerID, True, True)
        Me.VRScript.AddTabIndexControl(Me.ctl_Prior_Carrier_PPA.PreviousInsurerID, False, True)
        Me.VRScript.AddTabIndexControl(Me.ctl_Prior_Carrier_PPA.ExpirationDateID, True, False)
        Me.VRScript.AddTabIndexControl(Me.ctl_App_Rate.SaveBtnID, False, True)

        Me.VRScript.BindTabIndexControls()
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = div_master_hom_app.ClientID
        End If

        'Added 1/11/2022 for Bug 67521 MLW - Esig Feature Flag
        If Me.hasEsigOption = False Then
            ctl_Esignature.Visible = False
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub ctl_App_Rate_ApplicationRatedSuccessfully() Handles ctl_App_Rate.ApplicationRatedSuccessfully
        RaiseEvent App_Rate_ApplicationRatedSuccessfully()
    End Sub

    Private Sub ctl_App_Rate_ApplicationRated() Handles ctl_App_Rate.ApplicationRated
        RaiseEvent QuoteRated()
    End Sub

    'Added 8/1/2022 for task 75911 MLW
    Private Sub UpdateUWQuestions() Handles ctl_App_Rate.UpdateUWQuestions
        RaiseEvent UpdatesUWQuestions()
    End Sub
End Class