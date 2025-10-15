Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions

Public Class ctlCoverage_PPA_ScheduledItem
    Inherits VRControlBase

    Public Event ItemRemoved(itemIndex As Int32)

    Public Property VehicleIndex As Int32
        Get
            Return ViewState.GetInt32("vs_vehicleNum", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_vehicleNum") = value
            If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
                DoSICalcScriptSetup()
            End If
        End Set
    End Property

    Public Property ItemIndex As Int32
        Get
            Return ViewState.GetInt32("vs_scheduledItemIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_scheduledItemIndex") = value
        End Set
    End Property



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LoadStaticData()
        If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
            DoSICalcScriptSetup()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Private Sub DoSICalcScriptSetup() 'do not use this method for any other reason
        If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
            If Me.VehicleIndex >= 0 Then
                'CAH 8/14/2019 - Changed Me.QuoteId to Me.QuoteIdOrPolicyIdPipeImageNumberJavaScriptSafe - Endo's will not have QuoteId.  The new prop resolve to QuoteID if not Endo
                Me.VRScript.AddVariableLine("var txtAmountID_Array_" + Me.VehicleIndex.ToString() + "_" + Me.QuoteIdOrPolicyIdPipeImageNumberJavaScriptSafe + " = new Array();")
                Me.VRScript.AddVariableLine("txtAmountID_Array_" + Me.VehicleIndex.ToString() + "_" + Me.QuoteIdOrPolicyIdPipeImageNumberJavaScriptSafe + ".push('" + Me.txtAmount.ClientID + "');")
                'uses sevarl variables that are created by the list control to calc the remaining SI balance as the user types
                'not for sure this will only be added once so using the old way 'Me.VRScript.CreateJSBinding(Me.txtAmount, ctlPageStartupScript.JsEventType.onkeyup, "DoScheduledItemsRemainingMath(txtAmountID_Array_" + Me.VehicleIndex.ToString() + "_" + Me.QuoteId + ",originalSISum_" + Me.VehicleIndex.ToString() + "_" + Me.QuoteId + ",txtReaminingAmountID_" + Me.VehicleIndex.ToString() + "_" + Me.QuoteId + ");});")
                Me.VRScript.AddScriptLine("$(""#" + Me.txtAmount.ClientID + """).keyup(function () {DoScheduledItemsRemainingMath(txtAmountID_Array_" + Me.VehicleIndex.ToString() + "_" + Me.QuoteIdOrPolicyIdPipeImageNumberJavaScriptSafe + ",originalSISum_" + Me.VehicleIndex.ToString() + "_" + Me.QuoteIdOrPolicyIdPipeImageNumberJavaScriptSafe + ",txtReaminingAmountID_" + Me.VehicleIndex.ToString() + "_" + Me.QuoteIdOrPolicyIdPipeImageNumberJavaScriptSafe + ");});", False, False, True)
            End If
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddAdditionalEquipment.Items.Count < 1 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddAdditionalEquipment, QuickQuoteClassName.QuickQuoteScheduledItem, QuickQuotePropertyName.ScheduledItemsTypeId, SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            'Save()
            If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Any() Then
                Dim v As QuickQuote.CommonObjects.QuickQuoteVehicle = Me.Quote.Vehicles(Me.VehicleIndex)
                If ItemIndex >= 0 Then
                    LoadStaticData()
                    Dim si As QuickQuote.CommonObjects.QuickQuoteScheduledItem = v.ScheduledItems(ItemIndex)

                    si.Amount = si.Amount.TryToFormatAsCurreny(False) ' 6-3-14

                    Me.ddAdditionalEquipment.SetFromValue(si.ScheduledItemsTypeId)

                    Dim siType As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteScheduledItem, QuickQuotePropertyName.ScheduledItemsTypeId, si.ScheduledItemsTypeId)

                    Me.lblAccordTitle.Text = String.Format("Item #{0} - {1} {2}", ItemIndex + 1, si.Amount, siType)
                    Me.lblAccordTitle.Text = IFM.Common.InputValidation.InputHelpers.EllipsisText(Me.lblAccordTitle.Text, 40)

                    Dim bodyType_MotorCycle As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Motorcycle")
                    If v.BodyTypeId = bodyType_MotorCycle Then
                        ' If this is a motorcycle and the description is the default value from the quote side ("DESCRIPTION #1"),
                        ' don 't display the description in the description textbox, just display empty textbox and force the user to enter a description.
                        If si.Description.Contains("DESCRIPTION #") Then
                            Me.txtCoverageDescription.Text = ""
                        Else
                            Me.txtCoverageDescription.Text = si.Description
                        End If
                    Else
                        Me.txtCoverageDescription.Text = si.Description
                    End If

                    Me.txtAmount.Text = si.Amount

                End If
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = String.Format("Custom Equipment {1} - Vehicle #{0}", Me.VehicleIndex + 1, Me.ItemIndex + 1)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.ScheduledItemValidator_PPA.ScheduledItemViolation(VehicleIndex, ItemIndex, Me.Quote)

        For Each v In vals
            Select Case v.FieldId
                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.ScheduledItemValidator_PPA.EquipmentDescription
                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCoverageDescription, v, accordList)
                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.ScheduledItemValidator_PPA.EquipmentAmount
                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtAmount, v, accordList)
            End Select
        Next
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Visible Then
            If Quote IsNot Nothing Then
                If Me.Quote.Vehicles IsNot Nothing Then
                    Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Me.Quote.Vehicles.GetItemAtIndex(Me.VehicleIndex)
                    If vehicle IsNot Nothing Then
                        Dim schedItem As QuickQuote.CommonObjects.QuickQuoteScheduledItem = vehicle.ScheduledItems.GetItemAtIndex(Me.ItemIndex)
                        If schedItem IsNot Nothing Then
                            schedItem.ScheduledItemsTypeId = Me.ddAdditionalEquipment.SelectedValue
                            schedItem.ScheduledItemsCategoryId = "2"
                            schedItem.ItemDate = DateTime.Now
                            schedItem.Description = Me.txtCoverageDescription.Text
                            schedItem.Amount = Me.txtAmount.Text.Trim()
                        End If
                    End If
                End If

                Return True
            End If
        End If
        Return False
    End Function

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        RaiseEvent ItemRemoved(ItemIndex) ' repopulates list
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent(New IFM.VR.Web.VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

    Public Overrides Sub ClearControl()
        Me.ddAdditionalEquipment.SelectedIndex = -1
        Me.txtAmount.Text = ""
        Me.txtCoverageDescription.Text = ""

        MyBase.ClearControl()
    End Sub

End Class