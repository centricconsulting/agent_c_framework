Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports PopupMessageClass
Imports QuickQuote.CommonObjects

Public Class ctl_BOP_GeneralInformation
    Inherits VRControlBase

#Region "Declarations"

#End Region

#Region "Methods and Functions"
    Private Sub HandleError(ByVal RoutineName As String, ByVal ex As Exception)
        Dim str As String = RoutineName & ":  " & ex.Message
        If AppSettings("TestOrProd").ToUpper <> "PROD" Then lblMsg.Text = str Else Throw New Exception(ex.Message, ex)
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Try
            Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hddAccord, "0")
            'Me.VRScript.CreateJSBinding(ddlPropertyDamageLiabilityDeductible, ctlPageStartupScript.JsEventType.onchange, "if($(this).val() != '0'){$('#lblApplies').text('*Applies');}else{$('#lblApplies').text('Applies');}", True)
            Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)

            ' Handles changes to the Policy Type dropdown Preferred Option for BOP updated 2/4/21 
            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                'Updated 09/14/2021 for BOP Endorsements Task 61288 MLW
                If Not IsQuoteReadOnly() Then
                    'Me.ddlProgramType.SelectedValue = "61"
                    Me.ddlProgramType.CssClass = "MediumControl"
                    Dim preferredBopPopupMessage As String = "<div><b>To qualify for the preferred program, the risk must have the following criteria-</b></div><div>"
                    preferredBopPopupMessage = preferredBopPopupMessage & "• 3 year policy loss ratio of 55% or less.<br />"
                    preferredBopPopupMessage = preferredBopPopupMessage & "• Building age of 25 years or less.<br />"
                    preferredBopPopupMessage = preferredBopPopupMessage & "• Building age more than 25 years of age that have had major upgrades to roof, hvac, plumbing within the past 10 years and shown to be in better than average condition based on photos and/or loss control.<br />"
                    preferredBopPopupMessage = preferredBopPopupMessage & "• Business is managed by an experienced insured (minimum of 3 years of experience) in the same business.<br />"
                    preferredBopPopupMessage = preferredBopPopupMessage & "• Insured location has additional property safeguards than the average risk such as automatic sprinkler systems, central station alarms, fenced or otherwise better protected from losses.<br />"
                    preferredBopPopupMessage = preferredBopPopupMessage & "• Above average maintenance and housekeeping verified by agent, loss control inspection or other reliable source.<br />"
                    preferredBopPopupMessage = preferredBopPopupMessage & "• Property located in Protection Class 7 or better.<br />"
                    preferredBopPopupMessage = preferredBopPopupMessage & "• Exposure with Risk Grade 1 or 2.<br />"
                    preferredBopPopupMessage = preferredBopPopupMessage & "•Risk has a formal safety program in place as confirmed by agent or loss control.<br />"
                    preferredBopPopupMessage = preferredBopPopupMessage & "</div>"
                    'updated 2/2/21 
                    Using popupSpecial As New PopupMessageClass.PopupMessageObject(Me.Page, preferredBopPopupMessage, "Preferred Rating Program Guidelines")
                        With popupSpecial
                            .ControlEvent = PopupMessageClass.PopupMessageObject.ControlEvents.onchange
                            .DropDownValueToBindTo = "62"
                            .BindScript = PopupMessageClass.PopupMessageObject.BindTo.Control
                            .isModal = False
                            .AddButton("OK", True)
                            .width = 550
                            .height = 500
                            .AddControlToBindTo(ddlProgramType)
                            .divId = "ddlProgramTypePopup"
                            .CreateDynamicPopUpWindow()
                        End With
                    End Using
                End If
            End If
            Exit Sub
        Catch ex As Exception
            HandleError("AddScriptWhenRendered", ex)
        End Try
    End Sub

    Public Overrides Sub LoadStaticData()
        ' Program Type
        If ddlProgramType.Items Is Nothing OrElse ddlProgramType.Items.Count <= 0 Then
            QQHelper.LoadStaticDataOptionsDropDown(ddlProgramType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ProgramTypeId, , Quote.LobType)
        End If

        'Added 09/14/2021 for BOP Endorsements Task 61288 MLW
        QQHelper.LoadStaticDataOptionsDropDown(ddlTenantsFireLiability, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.TenantsFireLiabilityId, , Quote.LobType)
        If Not IsQuoteReadOnly() Then
            ddlTenantsFireLiability.Items.Remove("")
        End If

        Exit Sub
    End Sub

    Public Sub UpdateControlsAfterEffectiveDateChange(ByVal NewDate As String)
        ' Update any controls that rely on effective date
        If CDate(NewDate) >= IFM.VR.Common.Helpers.GenericHelper.GetOhioEffectiveDate() Then
            trProgramTypeRow.Attributes.Add("style", "display:''")
            ddlProgramType.SetFromValue(Me.SubQuoteFirst.ProgramTypeId)
        Else
            trProgramTypeRow.Attributes.Add("style", "display:none")
        End If

        Exit Sub
    End Sub

    Public Overrides Sub Populate()
        Try
            If Me.Quote IsNot Nothing Then
                If Me.SubQuoteFirst IsNot Nothing Then
                    LoadStaticData()
                    If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
                        trProgramTypeRow.Attributes.Add("style", "display:''")
                        'Updated 09/14/2021 for BOP Endorsements Task 61288 MLW
                        'ddlProgramType.SetFromValue(Me.SubQuoteFirst.ProgramTypeId)
                        If IsQuoteReadOnly() Then
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlProgramType, Me.SubQuoteFirst.ProgramTypeId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ProgramTypeId)
                        Else
                            ddlProgramType.SetFromValue(Me.SubQuoteFirst.ProgramTypeId)
                        End If
                    Else
                        trProgramTypeRow.Attributes.Add("style", "display:none")
                    End If
                    Me.ddlOccurrenceLiabilityLimit.SetFromValue(Me.SubQuoteFirst.OccurrenceLiabilityLimitId)
                    Me.chkBusinessMasterEnhancedEndorsement.Checked = Me.SubQuoteFirst.HasBusinessMasterEnhancement
                    'Updated 09/14/2021 for BOP Endorsements Task 61288 MLW
                    'Me.ddlTenantsFireLiability.SetFromValue(Me.SubQuoteFirst.TenantsFireLiabilityId)
                    If IsQuoteReadOnly() Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlTenantsFireLiability, Me.SubQuoteFirst.TenantsFireLiabilityId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.TenantsFireLiabilityId)
                    Else
                        Me.ddlTenantsFireLiability.SetFromValue(Me.SubQuoteFirst.TenantsFireLiabilityId)
                    End If
                    Me.ddlPropertyDamageLiabilityDeductible.SetFromValue(Me.SubQuoteFirst.PropertyDamageLiabilityDeductibleId)
                    Me.ddlPropDmgLiabLimitPerClaimOrOccurrence.SetFromValue(Me.SubQuoteFirst.PropertyDamageLiabilityDeductibleOptionId)
                    Me.ddlBlanketRating.SetFromValue(Me.SubQuoteFirst.BlanketRatingOptionId)

                    'Added 09/08/2021 for BOP Endorsements task 63910 MLW
                    If Not IsQuoteReadOnly() Then
                        lnkBusinessMaster.Attributes.Add("href", System.Configuration.ConfigurationManager.AppSettings("BOP_Help_BusinessMasterSummary"))
                    End If
                End If
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
            Exit Sub
        End Try
    End Sub

    Public Overrides Function Save() As Boolean
        Try
            If Me.Quote IsNot Nothing Then
                ' PROGRAM TYPE - enabled after OH effective date
                ' Set at the policy level
                If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
                    Quote.ProgramTypeId = ddlProgramType.SelectedValue
                Else
                    Quote.ProgramTypeId = ""
                End If

                For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In Me.SubQuotes
                    ' Set program type on the subquotes
                    If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
                        sq.ProgramTypeId = ddlProgramType.SelectedValue
                    Else
                        sq.ProgramTypeId = ""
                    End If
                    sq.OccurrenceLiabilityLimitId = ddlOccurrenceLiabilityLimit.SelectedValue
                    sq.TenantsFireLiabilityId = ddlTenantsFireLiability.SelectedValue
                    sq.PropertyDamageLiabilityDeductibleId = ddlPropertyDamageLiabilityDeductible.SelectedValue
                    sq.PropertyDamageLiabilityDeductibleOptionId = ddlPropDmgLiabLimitPerClaimOrOccurrence.SelectedValue
                    sq.HasBusinessMasterEnhancement = chkBusinessMasterEnhancedEndorsement.Checked
                    sq.BlanketRatingOptionId = ddlBlanketRating.SelectedValue
                Next
            End If
            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Try
            ' Note that all of the fields on this for have a default value that is acceptable
            MyBase.ValidateControl(valArgs)

            Me.ValidationHelper.GroupName = "General Information"

            Exit Sub
        Catch ex As Exception
            HandleError("ValidateControl", ex)
            Exit Sub
        End Try
    End Sub


#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If IsPostBack = False Then
                Me.MainAccordionDivId = Me.divMain.ClientID
            End If
        Catch ex As Exception
            HandleError("Page LOAD", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Private Sub ddlBlanketRating_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlBlanketRating.SelectedIndexChanged
        If ddlBlanketRating.SelectedIndex > 0 Then
            ' Use popup library to show the Use Code dialog
            Dim msg As String = "This risk contains coverage included on a blanket basis.  A signed Statement of Values is required.  Please forward to your underwriter upon binding coverage."
            Using popup As New PopupMessageObject(Me.Page, msg, "Blanket Rating Message")
                With popup
                    .isFixedPositionOnScreen = True
                    .ZIndexOfPopup = 2
                    .isModal = True
                    .Image = PopupMessageObject.ImageOptions.None
                    .hideCloseButton = True
                    .AddButton("OK", True)
                    .CreateDynamicPopUpWindow()
                End With
            End Using
        End If
    End Sub

#End Region

End Class