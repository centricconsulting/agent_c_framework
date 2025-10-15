Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctl_FarmPolicyCoverage_AppSide
    Inherits VRControlBase

    Public ReadOnly Property MyFarmLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.HiddenField1, "0")
        Me.VRScript.CreateAccordion(Me.CanineSection.ClientID, Me.HiddenField2, "0")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkCanineBtnSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkCanineBtnClear.ClientID)

    End Sub

    Public Overrides Sub LoadStaticData()
        If ddlBPType.Items.Count < 1 Then
            QQHelper.LoadStaticDataOptionsDropDown(ddlBPType, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.BusinessPursuitTypeId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        Me.trIncidental.Visible = False
        trCanine.Visible = False

        Dim businessPursuits As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = Nothing
        Dim medicalPayments As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = Nothing
        Dim CanineExclusion As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = Nothing
        If Me.MyFarmLocation IsNot Nothing Then
            If Me.MyFarmLocation.SectionIICoverages IsNot Nothing AndAlso Me.MyFarmLocation.SectionIICoverages.Any() Then
                businessPursuits = (From cov In MyFarmLocation.SectionIICoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Incidental_Business_Exposures Select cov).FirstOrDefault()
                medicalPayments = (From cov In MyFarmLocation.SectionIICoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments Select cov).FirstOrDefault()
                CanineExclusion = (From cov In MyFarmLocation.SectionIICoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion Select cov).FirstOrDefault()
            End If
            If businessPursuits Is Nothing AndAlso medicalPayments Is Nothing AndAlso CanineExclusion Is Nothing Then
                Me.Visible = False
            Else
                'populate
                If businessPursuits IsNot Nothing Then
                    Me.trIncidental.Visible = True
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddlBPType, businessPursuits.BusinessPursuitTypeId)
                End If
                If CanineExclusion IsNot Nothing Then
                    trCanine.Visible = True
                End If
            End If
            Me.PopulateChildControls()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divMain.ClientID

        If Not IsPostBack Then
            If MyFarmLocation IsNot Nothing AndAlso MyFarmLocation.SectionIICoverages IsNot Nothing Then
                Dim medicalPayments As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = Nothing
                medicalPayments = (From cov In MyFarmLocation.SectionIICoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments Select cov).FirstOrDefault()
                If medicalPayments IsNot Nothing Then
                    Dim addedNewResidentName As Boolean = False
                    If Int32.TryParse(medicalPayments.NumberOfPersonsReceivingCare, Nothing) Then
                        If Me.MyFarmLocation.ResidentNames Is Nothing Then
                            Me.MyFarmLocation.ResidentNames = New List(Of QuickQuoteResidentName)()
                        End If
                        While (Me.MyFarmLocation.ResidentNames.Count < CInt(medicalPayments.NumberOfPersonsReceivingCare))
                            Me.MyFarmLocation.ResidentNames.Add(New QuickQuoteResidentName())
                            addedNewResidentName = True
                        End While
                    End If

                    If addedNewResidentName Then 'need to save these recently added records
                        Me.Save_FireSaveEvent(False)
                        Me.Populate()
                    End If
                End If
            End If

        End If
    End Sub

    Public Overrides Function Save() As Boolean

        If MyFarmLocation.SectionIICoverages IsNot Nothing Then

            Dim businessPursuits As QuickQuoteSectionIICoverage = (From cov In MyFarmLocation.SectionIICoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Incidental_Business_Exposures Select cov).FirstOrDefault()
            If businessPursuits IsNot Nothing Then
                businessPursuits.BusinessPursuitTypeId = Me.ddlBPType.SelectedValue
            End If

        End If
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
        Me.ValidationHelper.GroupName = "Policy Level Coverages"

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim valItems = IFM.VR.Validation.ObjectValidation.FarmLines.PolicyCoverageValidator.ValidateFARCoverages(Me.Quote, 0, Me.DefaultValidationType, False, False)
        If valItems.Any() Then
            For Each v In valItems
                Select Case v.FieldId
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.PolicyCoverageValidator.BusinessTypeRequired
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlBPType, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.PolicyCoverageValidator.InvalidValueForFarmPollutionLiabilityUpdate
                        Me.ValidationHelper.AddWarning(v.Message)
                End Select
            Next
        End If

    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click, lnkCanineBtnSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Protected Sub lnkCanineBtnClear_Clear(sender As Object, e As EventArgs) Handles lnkCanineBtnClear.Click
        If Cov_CanineExclusionList.Visible Then Cov_CanineExclusionList.ClearControl()
    End Sub
End Class