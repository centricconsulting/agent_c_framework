Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions
Imports System.Linq

Public Class ctl_Prior_Carrier_PPA
    Inherits VRControlBase

    Public ReadOnly Property PreviousInsurerID() As String
        Get
            Return Me.ddName.ClientID
        End Get
    End Property

    Public ReadOnly Property ExpirationDateID() As String
        Get
            Return Me.txtExpirationDate.ClientID
        End Get
    End Property

    Public Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divMainPriorCarrier.ClientID
            LoadStaticData()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateDatePicker(Me.txtExpirationDate.ClientID, False)
        Me.VRScript.CreateTextboxMask(txtExpirationDate, "99/99/9999")
        Me.VRScript.CreateAccordion(MainAccordionDivId, hiddenAccordActive, "0")
        Me.VRScript.CreateConfirmDialog(Me.lnkClearBase.ClientID, "Clear Prior Carrier Information?")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            Me.VRScript.AddScriptLine("$('#" + Me.divMainPriorCarrier.ClientID + "').find('label').each(function(){$(this).text('*' + $(this).text());});")
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddName.Items.Count < 1 AndAlso SubQuoteFirst IsNot Nothing AndAlso SubQuoteFirst.PriorCarrier IsNot Nothing Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddName, QuickQuoteClassName.QuickQuotePriorCarrier, QuickQuotePropertyName.PreviousInsurerTypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddDurationScale, QuickQuoteClassName.QuickQuotePriorCarrier, QuickQuotePropertyName.PriorDurationTypeId, SortBy.None, Me.Quote.LobType)
            'updated 8/11/2018 for multi-state (Me.Quote replaced w/ SubQuoteFirst in one spot); note: existing calls would fail if Quote is not returned since there is no check
            If Not (Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal Or Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal) Or SubQuoteFirst.PriorCarrier.PreviousInsurerTypeId <> "85" Then ' BUG 7793
                Dim NotFoundItem = (From i As ListItem In Me.ddName.Items Where i.Value = "85" Or i.Text.ToLower().Trim() = "not found" Select i).FirstOrDefault() ' 'Not Found' is only for PAA and HOM - because of Comp Raters
                If NotFoundItem IsNot Nothing Then
                    Me.ddName.Items.Remove(NotFoundItem)
                End If
            End If

        End If
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        'updated 8/11/2018 for multi-state
        If SubQuoteFirst IsNot Nothing Then
            If SubQuoteFirst.PriorCarrier IsNot Nothing Then
                With SubQuoteFirst.PriorCarrier

                    If .PreviousInsurerTypeId = "" Then
                        Me.ddName.SetFromValue("0")
                    Else
                        Me.ddName.SetFromValue(.PreviousInsurerTypeId)
                    End If
                    '59745 BB Adding If condition to set the Previous Insurer to default blank value when it is not selected yet as per 
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP OrElse
                    Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialCrime OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGarage OrElse
                    Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine OrElse
                    Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty OrElse
                    Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialUmbrella OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation) AndAlso .PreviousInsurerTypeId = "" Then
                        Me.ddName.SetFromValue("")
                    Else
                        Me.ddName.SetFromValue(.PreviousInsurerTypeId)
                    End If

                    Me.txtDuration.Text = .PriorDurationWithCompany.ReturnEmptyIfEqualsAny("0")

                    Me.ddDurationScale.SetFromValue(.PriorDurationTypeId)
                    Me.txtPolicyNumber.Text = .PriorPolicy

                    If IsDate(.PriorExpirationDate) Then
                        Me.txtExpirationDate.Text = .PriorExpirationDate.ReturnEmptyIfDefaultDiamondDate
                    Else
                        'just keep whatever is there now
                    End If
                End With
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Prior Carrier Information"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        'updated 8/11/2018 for multi-state; note: currently passing in 1st stateQuote instead of Me.Quote, but may be better to drill down to state quote(s) from Validation method
        Dim valItems = PriorCarrierValidator.ValidatePriorCarrier(SubQuoteFirst, valArgs.ValidationType)
        If valItems.Any() Then
            Dim accordId As String = MainAccordionDivId
            For Each v In valItems
                Select Case v.FieldId
                    Case PriorCarrierValidator.PriorcarrierPreviousInsurer
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddName, v, accordList)
                    Case PriorCarrierValidator.PriorCarrierDuration
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDuration, v, accordList)
                    Case PriorCarrierValidator.PriorCarrierExpirationDate
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtExpirationDate, v, accordList)
                    Case PriorCarrierValidator.PriorCarrierPolicyNumber
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPolicyNumber, v, accordList)

                End Select
            Next
        End If

    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Quote IsNot Nothing Then
            'updated 8/11/2018 for multi-state; existing logic has just been placed in a loop, and Me.Quote has been replaced by current stateQuote
            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                For Each stateQuote As QuickQuoteObject In SubQuotes
                    If stateQuote IsNot Nothing Then 'this shouldn't be necessary
                        If stateQuote.PriorCarrier Is Nothing Then
                            stateQuote.PriorCarrier = New QuickQuotePriorCarrier()
                        End If
                        With stateQuote.PriorCarrier
                            .PreviousInsurerTypeId = Me.ddName.SelectedValue
                            .PriorDurationWithCompany = Me.txtDuration.Text
                            .PriorDurationTypeId = Me.ddDurationScale.SelectedValue
                            .PriorPolicy = Me.txtPolicyNumber.Text
                            .PriorExpirationDate = Me.txtExpirationDate.Text
                        End With
                    End If
                Next
            End If

        End If
        Return True
    End Function

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Protected Sub lnkClearBase_Click(sender As Object, e As EventArgs) Handles lnkClearBase.Click
        ClearControl()
    End Sub

    Public Overrides Sub ClearControl()
        Me.txtDuration.Text = ""
        Me.txtExpirationDate.Text = ""
        Me.txtPolicyNumber.Text = ""
        Me.ddDurationScale.SelectedIndex = 0
        WebHelper_Personal.SetdropDownFromText(Me.ddName, "NONE")
        MyBase.ClearControl()
    End Sub

End Class