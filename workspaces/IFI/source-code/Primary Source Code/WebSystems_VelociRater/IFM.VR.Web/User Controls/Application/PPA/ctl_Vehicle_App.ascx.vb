Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions

Public Class ctl_Vehicle_App
    Inherits VRControlBase

    Public Event RequestPageRefresh()

    Public Property VehicleIndex As Int32
        Get
            Return ViewState.GetInt32("vs_vehicleNum", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_vehicleNum") = value
            Me.ctlCoverage_PPA_ScheduledItemsList.VehicleIndex = Me.VehicleIndex
            Me.ctlVehicleAdditionalInterestList.VehicleIndex = Me.VehicleIndex
        End Set

    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.VehicleIndex
        End Get
    End Property

    Public ReadOnly Property MyVehicle As QuickQuote.CommonObjects.QuickQuoteVehicle
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Vehicles.GetItemAtIndex(Me.VehicleIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateJSBinding(Me.txtVinNumber, ctlPageStartupScript.JsEventType.onblur, "VinConfirm('" + Me.txtVinNumber.ClientID + "','" + Me.divConfirmResults.ClientID + "'); return false;")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If MyVehicle IsNot Nothing Then
            Dim compSymbol = If(MyVehicle.VehicleSymbols IsNot Nothing, (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "1" Select s).FirstOrDefault(), Nothing)
            Dim colliSymbol = If(MyVehicle.VehicleSymbols IsNot Nothing, (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "2" Select s).FirstOrDefault(), Nothing)
            'Dim liabSymbol = If(MyVehicle.VehicleSymbols IsNot Nothing, (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "3" Select s).FirstOrDefault(), Nothing)

            If compSymbol IsNot Nothing AndAlso colliSymbol IsNot Nothing Then
                Dim performanceText As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.PerformanceTypeId, MyVehicle.PerformanceTypeId)
                Me.lblAccordHeader.Text = String.Format("Vehicle #{0} - {1} {2} {3} ({4})", VehicleIndex + 1, MyVehicle.Year, MyVehicle.Make, MyVehicle.Model, performanceText)
                'If liabSymbol IsNot Nothing Then
                '    Me.lblAccordHeader.Text = String.Format("Vehicle #{0} - {1} {2} {3} ({6} {4}/{5}/{7})", VehicleIndex + 1, MyVehicle.Year, MyVehicle.Make, MyVehicle.Model, compSymbol.UserOverrideSymbol, colliSymbol.UserOverrideSymbol, performanceText, liabSymbol.UserOverrideSymbol)
                'Else
                '    Me.lblAccordHeader.Text = String.Format("Vehicle #{0} - {1} {2} {3} ({6} {4}/{5})", VehicleIndex + 1, MyVehicle.Year, MyVehicle.Make, MyVehicle.Model, compSymbol.UserOverrideSymbol, colliSymbol.UserOverrideSymbol, performanceText)
                'End If

                If MyVehicle.Vin.ToLower().Trim().StartsWith("none") Then
                    Me.txtVinNumber.Enabled = False
                    'Me.txtVinNumber.Attributes.Remove("onblur")
                    Me.txtVinNumber.ToolTip = "No VIN edits allowed for VINs of 'NONE'."
                End If
            Else
                If MyVehicle.Vin.ToLower().Trim().StartsWith("none") And Not (MyVehicle.NonOwnedNamed Or MyVehicle.NonOwned) Then
                    Me.txtVinNumber.Enabled = False
                    'Me.txtVinNumber.Attributes.Remove("onblur")
                    Me.txtVinNumber.ToolTip = "No VIN edits allowed for VINs of 'NONE'."
                End If
                If Not (MyVehicle.NonOwnedNamed Or MyVehicle.NonOwned) Then
                    Me.lblAccordHeader.Text = String.Format("Vehicle #{0} - {1} {2} {3} ({4})", VehicleIndex + 1, MyVehicle.Year, MyVehicle.Make, MyVehicle.Model, String.Format("{0:C0}", MyVehicle.CostNew))
                Else
                    Me.lblAccordHeader.Text = String.Format("Vehicle #{0} - {1} {2} {3}", VehicleIndex + 1, MyVehicle.Year, MyVehicle.Make, MyVehicle.Model)
                End If

            End If

            Me.lblAccordHeader.Text = IFM.Common.InputValidation.InputHelpers.EllipsisText(Me.lblAccordHeader.Text, 60)

            If MyVehicle.Vin.ToLower().Trim().StartsWith("none") Then
                Me.txtVinNumber.Text = "NONE"
            Else
                Me.txtVinNumber.Text = MyVehicle.Vin
            End If

            If MyVehicle.NonOwnedNamed Or MyVehicle.NonOwned Then
                Me.divContent.Visible = False
                Me.divNoContent.Visible = True
                Me.lblNoContent.Text = "No additional information is required."
                Me.ctlVehicleAdditionalInterestList.Visible = False
                Me.ctlCoverage_PPA_ScheduledItemsList.Visible = False
                Me.txtVinNumber.Enabled = False
                'Me.txtVinNumber.Attributes.Remove("onblur")
                Me.txtVinNumber.ToolTip = "No VIN edits allowed for Non-Named/Non-Owned Vehicles."
            Else

                'Me.ctlVehicleAdditionalInterestList.VehicleItemsAccordID = If(Me.ParentVrControl IsNot Nothing, Me.ParentVrControl.MainAccordionDivId, "")
                Me.ctlVehicleAdditionalInterestList.VehicleIndex = VehicleIndex

                'Me.ctlCoverage_PPA_ScheduledItemsList.VehicleItemsAccordID = If(Me.ParentVrControl IsNot Nothing, Me.ParentVrControl.MainAccordionDivId, "")
                Me.ctlCoverage_PPA_ScheduledItemsList.VehicleIndex = VehicleIndex
                Me.PopulateChildControls()
            End If

        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If MyVehicle IsNot Nothing Then
            If Me.txtVinNumber.Text.ToLower().Trim() = "none" AndAlso MyVehicle.NonOwned = False AndAlso MyVehicle.NonOwnedNamed = False Then
                Me.txtVinNumber.Text = "NONE" + Me.VehicleIndex.ToString()
            Else
                MyVehicle.Vin = Me.txtVinNumber.Text
            End If

            Me.SaveChildControls()
            Return True
        End If
        Return False
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = String.Format("Vehicle #{0}", Me.VehicleIndex + 1)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim valItems = VehicleValidator.VehicleValidation(Me.VehicleIndex, Me.Quote, valArgs.ValidationType)
        If valItems.Any() Then
            For Each v In valItems
                Select Case v.FieldId
                    Case VehicleValidator.VehicleVIN
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtVinNumber, v, accordList)
                    Case VehicleValidator.VehicleSymbols
                        If Not IsQuoteEndorsement() Then
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                End Select
            Next
        End If
        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Private Sub ctlCoverage_PPA_ScheduledItemsList_RequestPageRefresh() Handles ctlCoverage_PPA_ScheduledItemsList.RequestPageRefresh
        RaiseEvent RequestPageRefresh()
    End Sub

    Public Overrides Sub ClearControl()
        Me.txtVinNumber.Text = ""

        MyBase.ClearControl()
    End Sub

End Class