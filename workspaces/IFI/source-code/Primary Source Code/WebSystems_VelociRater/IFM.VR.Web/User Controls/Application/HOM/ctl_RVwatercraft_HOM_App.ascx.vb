Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers

Public Class ctl_RVwatercraft_HOM_App
    Inherits VRControlBase

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property RequiresYouthfulOperatorInformation As Boolean
        Get
            Return Not YouthfulDriverObject Is Nothing
        End Get
    End Property

    Private ReadOnly Property YouthfulDriverObject As QuickQuote.CommonObjects.QuickQuoteOperator
        Get
            'Updated 8/23/18 for multi state MLW
            'If Me.Quote IsNot Nothing AndAlso Me.Quote.Operators IsNot Nothing Then
            If Me.Quote IsNot Nothing AndAlso Me.GoverningStateQuote IsNot Nothing AndAlso Me.GoverningStateQuote.Operators IsNot Nothing Then
                For Each op In Me.GoverningStateQuote.Operators
                    If op.Name IsNot Nothing Then
                        If IsDate(op.Name.BirthDate) Then
                            Dim effectiveDate As DateTime = If(IsDate(Me.Quote.EffectiveDate), CDate(Me.Quote.EffectiveDate), DateTime.Now)
                            If CDate(op.Name.BirthDate).AddYears(25) > effectiveDate Then
                                ' ok you have a operator under 25 but is it assigned to a watercraft
                                If MyLocation.RvWatercrafts IsNot Nothing Then
                                    For Each rv In MyLocation.RvWatercrafts
                                        If rv.AssignedOperatorNums IsNot Nothing Then
                                            If rv.AssignedOperatorNums.Contains(op.OperatorNum) Then
                                                ' yes this operator is under 25 and is assigned to a watercraft
                                                Return op
                                            End If
                                        End If
                                    Next
                                End If

                            End If
                        End If
                    End If
                Next
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divRVWatercraft.ClientID
        Me.ListAccordionDivId = Me.divRVItems.ClientID
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddScriptLine("$(""#" + Me.txtBirthDate.ClientID + """).watermark(""MM/DD/YYYY"");")
        Me.txtBirthDate.Attributes.Add("onblur", "$(this).val(dateFormat($(this).val()));")

        Me.VRScript.StopEventPropagation(Me.lnkSaveRVWater.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkClearYouth.ClientID, "Clear Operator?")
        Me.VRScript.StopEventPropagation(Me.lnkSaveYouth.ClientID)

        Me.VRScript.CreateAccordion(divYouthFul.ClientID, Nothing, "0")

        If MyLocation Is Nothing OrElse MyLocation.RvWatercrafts Is Nothing OrElse MyLocation.RvWatercrafts.Any() = False Then
            Me.VRScript.CreateAccordion(Me.divRVWatercraft.ClientID, Nothing, "false")
            divInLandMarineContent.Visible = False
        Else
            divInLandMarineContent.Visible = True
            Me.VRScript.CreateAccordion(Me.divRVWatercraft.ClientID, Me.hiddenActiveRVWatercraft, "0")
        End If

        Me.VRScript.CreateAccordion(Me.divRVItems.ClientID, Me.hiddenActiveRVWater, "0")

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

        rvWaterRepeater.DataSource = If(MyLocation IsNot Nothing AndAlso MyLocation.RvWatercrafts IsNot Nothing, MyLocation.RvWatercrafts, Nothing)
        rvWaterRepeater.DataBind()

        Me.FindChildVrControls() ' finds the just added controls do to the binding
        Dim index As Int32 = 0
        For Each child In Me.ChildVrControls
            If TypeOf child Is ctl_RVwatercraft_Hom_App_Item Then
                Dim c As ctl_RVwatercraft_Hom_App_Item = child
                c.RvWatercraftIndex = index
                c.Populate()
                index += 1
            End If
        Next

        If RequiresYouthfulOperatorInformation Then
            Me.divYouthFul.Visible = True
            Me.txtFirstName.Text = Me.YouthfulDriverObject.Name.FirstName
            Me.txtLastName.Text = Me.YouthfulDriverObject.Name.LastName
            Me.txtBirthDate.Text = Me.YouthfulDriverObject.Name.BirthDate
        Else
            Me.divYouthFul.Visible = False
        End If

    End Sub

    Public Overrides Function Save() As Boolean

        If RequiresYouthfulOperatorInformation Then
            Me.YouthfulDriverObject.Name.TypeId = "1" ' Added 12-12-2015 Matt A - Needed because of validation change

            Me.YouthfulDriverObject.Name.FirstName = Me.txtFirstName.Text.Trim()
            Me.YouthfulDriverObject.Name.LastName = Me.txtLastName.Text.Trim()
            'you have to make sure this is valid because the logic that finds the youthful operator is looking for someone number 25 years old so it must always be a date less than 25
            If IsDate(Me.txtBirthDate.Text) AndAlso CDate(Me.txtBirthDate.Text).AddYears(25) > DateTime.Now AndAlso CDate(Me.txtBirthDate.Text) < DateTime.Now Then
                Me.YouthfulDriverObject.Name.BirthDate = Me.txtBirthDate.Text.Trim()
            Else
                Me.ValidationHelper.AddWarning("Operator must be between 15 and 25 years of age. ", Me.txtBirthDate.ClientID)
                Dim lastWarning = Me.ValidationHelper.GetLastWarning()
                lastWarning.ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(Me.divYouthFul.ClientID, "0"))
                lastWarning.ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(Me.txtBirthDate.ClientID))
                'Me.ValidationHelper.Val_BindWarningToControl(Me.txtBirthDate, "Operator must be between 15 and 25 years of age. ", Me.divYouthFul.ClientID, "0")
            End If

        End If

        Me.SaveChildControls()

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
        Me.ValidationHelper.GroupName = "Youngest Operator"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim accordListPlusYouthful As New List(Of VRAccordionTogglePair)
        accordListPlusYouthful.AddRange(accordList)
        accordListPlusYouthful.Add(New VRAccordionTogglePair(Me.divYouthFul.ClientID, "0"))

        If Me.YouthfulDriverObject IsNot Nothing Then
            Dim vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftOperatorsValidator.ValidateRvWaterCraftOperator(Me.YouthfulDriverObject, Validation.ObjectValidation.ValidationItem.ValidationType.appRate)
            For Each v In vals
                Select Case v.FieldId
                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftOperatorsValidator.FirstName
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName.ClientID, v, accordListPlusYouthful)
                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftOperatorsValidator.LastName
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastName.ClientID, v, accordListPlusYouthful)
                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftOperatorsValidator.BirthDate
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBirthDate.ClientID, v, accordListPlusYouthful)
                End Select
            Next

            If IsDate(Me.txtBirthDate.Text) Then
                If CDate(Me.txtBirthDate.Text).AddYears(25) < DateTime.Now Or CDate(Me.txtBirthDate.Text).AddYears(15) > DateTime.Now Then
                    Me.ValidationHelper.AddError("Invalid Birth Date", Me.txtBirthDate.ClientID)
                    Dim lastError = Me.ValidationHelper.GetLastError()
                    lastError.ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(Me.divYouthFul.ClientID, "0"))
                    lastError.ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(Me.txtBirthDate.ClientID))
                    'Me.ValidationHelper.Val_BindErrorToControl(Me.txtBirthDate, "Invalid Birth Date", Me.divYouthFul.ClientID, "0")
                End If
            End If

        End If

    End Sub

    Protected Sub lnkSaveRVWater_Click(sender As Object, e As EventArgs) Handles lnkSaveRVWater.Click
        Save_FireSaveEvent(New IFM.VR.Web.VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

    Protected Sub lnkSaveYouth_Click(sender As Object, e As EventArgs) Handles lnkSaveYouth.Click
        Save_FireSaveEvent(New IFM.VR.Web.VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

    Protected Sub lnkClearYouth_Click(sender As Object, e As EventArgs) Handles lnkClearYouth.Click
        Me.txtFirstName.Text = ""
        Me.txtLastName.Text = ""
        Me.txtBirthDate.Text = ""
    End Sub
End Class