Imports QuickQuote.CommonMethods

Public Class ctl_RVwatercraft_Hom_App_Item
    Inherits VRControlBase

    Public Property RvWatercraftIndex As Int32
        Get
            If (ViewState("vs_RVindex") IsNot Nothing) Then
                Return CInt(ViewState("vs_RVindex"))
            End If
            Return -1
        End Get
        Set(value As Int32)
            ViewState("vs_RVindex") = value
        End Set
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.RvWatercraftIndex
        End Get
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() AndAlso Me.Quote.Locations(0) IsNot Nothing Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property MyRvWatercraftItem As QuickQuote.CommonObjects.QuickQuoteRvWatercraft
        Get
            If MyLocation IsNot Nothing Then
                If MyLocation.RvWatercrafts IsNot Nothing AndAlso RvWatercraftIndex < MyLocation.RvWatercrafts.Count Then
                    Return MyLocation.RvWatercrafts(RvWatercraftIndex)
                End If
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property RequiresInformation As Boolean
        Get
            If Me.Quote IsNot Nothing Then
                If Me.MyRvWatercraftItem IsNot Nothing Then
                    If Me.MyRvWatercraftItem.RvWatercraftTypeId <> "5" Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Get
    End Property

    Private ReadOnly Property RequiresBoatMotorInformation As Boolean
        Get
            Return Not MyBoatMotor Is Nothing
        End Get
    End Property

    Private ReadOnly Property MyBoatMotor As QuickQuote.CommonObjects.QuickQuoteRvWatercraftMotor
        Get
            If Me.MyRvWatercraftItem.RvWatercraftMotors IsNot Nothing Then
                Return (From m In Me.MyRvWatercraftItem.RvWatercraftMotors Where m.MotorTypeId = "2" Select m).FirstOrDefault()
            End If
            Return Nothing
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

        Me.VRScript.CreateConfirmDialog(Me.lnkClearRVWater.ClientID, "Clear?")
        Me.VRScript.StopEventPropagation(Me.lnkSaveRVWater.ClientID)

        Me.VRScript.CreateConfirmDialog(Me.lnkClearMotor.ClientID, "Clear?")
        Me.VRScript.StopEventPropagation(Me.lnkSaveMotor.ClientID)

        Me.VRScript.AddScriptLine("$(""#" + Me.divMotor.ClientID + """).accordion({ heightStyle: ""content"", active: 0, collapsible: true });  ")

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = String.Format("RV Watercraft #{0}", CInt(RvWatercraftIndex) + 1)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim valItems = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RvWaterCraftValidator.ValidateRvWaterCraft(MyRvWatercraftItem, valArgs.ValidationType, Me.Quote.LobId, quote:=Quote)

        For Each v In valItems
            Select Case v.FieldId
                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RvWaterCraftValidator.SerialNumberMissing, IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RvWaterCraftValidator.SerialNumberInvalid
                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSerial.ClientID, v, accordList)

                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RvWaterCraftValidator.ManufacturerMissing, IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RvWaterCraftValidator.ManufacturerInvalid
                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtManufacturer.ClientID, v, accordList)

                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RvWaterCraftValidator.ModelMissing, IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RvWaterCraftValidator.ModelInvalid
                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtModel.ClientID, v, accordList)

            End Select
        Next

        If Me.RequiresBoatMotorInformation Then
            'do motor validation
            Dim motorValItems = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftBoatMotorValidtor.ValidateRvWaterCraftMotor(MyBoatMotor, valArgs.ValidationType)
            For Each v In motorValItems
                Select Case v.FieldId
                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftBoatMotorValidtor.SerialNumberMissing, IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftBoatMotorValidtor.SerialNumberInvalid
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtMotorSerialNumber.ClientID, v, accordList)

                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftBoatMotorValidtor.ManufacturerMissing, IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftBoatMotorValidtor.ManufacturerInvalid
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtMotorManufacturer.ClientID, v, accordList)

                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftBoatMotorValidtor.ModelMissing, IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftBoatMotorValidtor.ModelInvalid
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtMotorModel.ClientID, v, "", "0")

                End Select
            Next
        End If

    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If Me.Quote IsNot Nothing Then
            If Me.MyRvWatercraftItem IsNot Nothing Then

                If String.IsNullOrWhiteSpace(Me.MyRvWatercraftItem.Year) And String.IsNullOrWhiteSpace(Me.MyRvWatercraftItem.Manufacturer) And String.IsNullOrWhiteSpace(Me.MyRvWatercraftItem.Model) Then
                    ' no info so show type text
                    '        Dim bodyType_MotorCycle As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Motorcycle")
                    Dim typeText As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, Me.MyRvWatercraftItem.RvWatercraftTypeId)
                    Me.lblHeader.Text = IFM.Common.InputValidation.InputHelpers.EllipsisText(String.Format("RV/Watercraft #{0} - {1}", CInt(RvWatercraftIndex) + 1, typeText).ToUpper(), 50)
                Else
                    Me.lblHeader.Text = IFM.Common.InputValidation.InputHelpers.EllipsisText(String.Format("RV/Watercraft #{0} - {1} {2} {3}", CInt(RvWatercraftIndex) + 1, Me.MyRvWatercraftItem.Year, Me.MyRvWatercraftItem.Manufacturer, Me.MyRvWatercraftItem.Model).ToUpper(), 50)
                End If



                Me.txtSerial.Text = Me.MyRvWatercraftItem.SerialNumber.Trim()
                Me.txtManufacturer.Text = Me.MyRvWatercraftItem.Manufacturer.Trim()
                Me.txtModel.Text = Me.MyRvWatercraftItem.Model.Trim()

                Me.tblFormData.Visible = False
                Me.divMotor.Visible = False
                Me.lblNoInputNeeded.Visible = False

                If Not Me.RequiresInformation Then
                    lnkClearRVWater.Enabled = False
                    lnkClearRVWater.ToolTip = "You can not clear this item"
                    'hiddenAddressIsActive.Value = "false"
                    Me.lblSerial.Text = "Serial Number"
                    Me.lblManufacturer.Text = "Manufacturer"
                    Me.lblModel.Text = "Model"
                    Me.lblNoInputNeeded.Visible = True
                Else
                    Me.lblSerial.Text = "*Serial Number"
                    Me.lblManufacturer.Text = "*Manufacturer"
                    Me.lblModel.Text = "*Model"
                    Me.tblFormData.Visible = True

                    If Me.RequiresBoatMotorInformation Then
                        divMotor.Visible = True
                        Me.txtMotorSerialNumber.Text = MyBoatMotor.SerialNumber
                        Me.txtMotorManufacturer.Text = MyBoatMotor.Manufacturer
                        Me.txtMotorModel.Text = MyBoatMotor.Model
                        Me.tblFormData.Visible = Me.MyRvWatercraftItem.RvWatercraftTypeId <> "3"
                    End If
                End If

            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean

        If Me.RequiresInformation Then
            If Me.Quote IsNot Nothing Then
                If Me.MyRvWatercraftItem IsNot Nothing Then
                    Me.MyRvWatercraftItem.SerialNumber = Me.txtSerial.Text.Trim()
                    Me.MyRvWatercraftItem.Manufacturer = Me.txtManufacturer.Text.Trim()
                    Me.MyRvWatercraftItem.Model = Me.txtModel.Text.Trim()
                End If
            End If
        End If

        If Me.RequiresBoatMotorInformation Then
            MyBoatMotor.SerialNumber = Me.txtMotorSerialNumber.Text.Trim
            MyBoatMotor.Manufacturer = Me.txtMotorManufacturer.Text
            MyBoatMotor.Model = Me.txtMotorModel.Text
        End If

        Me.Populate()
        Return True
    End Function

    Protected Sub lnkSaveRVWater_Click(sender As Object, e As EventArgs) Handles lnkSaveRVWater.Click
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

    Protected Sub lnkClearRVWater_Click(sender As Object, e As EventArgs) Handles lnkClearRVWater.Click
        Me.txtSerial.Text = ""
        Me.txtManufacturer.Text = ""
        Me.txtSerial.Text = ""
    End Sub

    Protected Sub lnkSaveMotor_Click(sender As Object, e As EventArgs) Handles lnkSaveMotor.Click
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

    Protected Sub lnkClearMotor_Click(sender As Object, e As EventArgs) Handles lnkClearMotor.Click
        Me.txtMotorSerialNumber.Text = ""
        Me.txtMotorManufacturer.Text = ""
        Me.txtMotorModel.Text = ""
    End Sub
End Class