Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Public Class ctlResidentName_App
    Inherits VRControlBase

    Public ReadOnly Property MyFarmLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public Property ResidentNameIndex As Int32
        Get
            Return ViewState.GetInt32("vs_residentIindex")
        End Get
        Set(value As Int32)
            ViewState("vs_residentIindex") = value
        End Set
    End Property

    Public ReadOnly Property ResidentName As QuickQuote.CommonObjects.QuickQuoteResidentName
        Get
            If Me.MyFarmLocation IsNot Nothing Then
                Return Me.MyFarmLocation.ResidentNames.GetItemAtIndex(Me.ResidentNameIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()
        If txtFirstName IsNot Nothing AndAlso txtLastName IsNot Nothing Then
            Me.VRScript.AddScriptLine("medicalPaymentNames_textboxIds.push(" + String.Format("new Array('{0}','{1}')", txtFirstName.ClientID, txtLastName.ClientID) + ");")
        End If

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateTextBoxFormatter(txtFirstName, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(txtLastName, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextboxWaterMark(txtDOB, "MM/DD/YYYY")
        Me.VRScript.CreateTextBoxFormatter(txtDOB, ctlPageStartupScript.FormatterType.DateFormat, ctlPageStartupScript.JsEventType.onblur)
        Me.VRScript.CreateTextBoxFormatter(txtRelationship, ctlPageStartupScript.FormatterType.AlphabeticOnly, ctlPageStartupScript.JsEventType.onkeyup)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.ResidentName IsNot Nothing Then
            If ResidentName.Name.FirstName <> "FIRSTN" Then
                Me.txtFirstName.Text = Me.ResidentName.Name.FirstName
            Else
                txtFirstName.Text = ""
            End If

            If ResidentName.Name.LastName <> "LASTN" Then
                Me.txtLastName.Text = Me.ResidentName.Name.LastName
            Else
                txtLastName.Text = ""
            End If

            If ResidentName.Name.BirthDate <> "1/1/1900" AndAlso ResidentName.Name.BirthDate <> "01/01/1900" Then
                Me.txtDOB.Text = Me.ResidentName.Name.BirthDate
            Else
                txtDOB.Text = ""
            End If

            If ResidentName.Name.FirstName <> "FIRSTN" AndAlso ResidentName.Name.LastName <> "LASTN" AndAlso ResidentName.Name.BirthDate <> "1/1/1900" Then
                Me.txtRelationship.Text = Me.ResidentName.Name.Salutation
            Else
                txtRelationship.Text = ""
            End If
        End If

        If IsQuoteEndorsement() Then
            If ResidentNameIndex = 0 Then
                lnkDelete.Visible = False
            End If
        End If

        Me.PopulateChildControls()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If Me.ResidentName IsNot Nothing Then
            Me.ResidentName.Name.FirstName = Me.txtFirstName.Text.Trim().ToUpper()
            Me.ResidentName.Name.LastName = Me.txtLastName.Text.Trim().ToUpper()
            Me.ResidentName.Name.BirthDate = Me.txtDOB.Text.Trim().ToUpper()
            Me.ResidentName.Name.Salutation = Me.txtRelationship.Text.Trim().ToUpper()
        End If
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)

        Me.ValidationHelper.GroupName = String.Format("Policy Level Coverages - Family Member #{0}", Me.ResidentNameIndex + 1)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Dim valItems = IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.ValidateResidentName(Me.Quote, 0, Me.ResidentNameIndex, Me.DefaultValidationType)
        If valItems.Any() Then
            For Each v In valItems
                Select Case v.FieldId
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_FirstName
                        Me.ValidationHelper.Val_BindValidationItemToControl(txtFirstName, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_LastName
                        Me.ValidationHelper.Val_BindValidationItemToControl(txtLastName, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_DOB
                        Me.ValidationHelper.Val_BindValidationItemToControl(txtDOB, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_Relationship
                        Me.ValidationHelper.Val_BindValidationItemToControl(txtRelationship, v, accordList)
                End Select
            Next
        End If

    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Me.Save_FireSaveEvent(False)
        If Me.MyFarmLocation.ResidentNames IsNot Nothing Then
            Try
                Me.MyFarmLocation.ResidentNames.Remove(Me.ResidentName)
                If MyFarmLocation.SectionIICoverages IsNot Nothing Then
                    '' update 'NumberOfPersonsReceivingCare' so that if they delete a record that will be reflected on the quote side
                    Dim famMed As QuickQuoteSectionIICoverage = (From cov In MyFarmLocation.SectionIICoverages Where cov.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments Select cov).FirstOrDefault()
                    If famMed IsNot Nothing Then
                        famMed.NumberOfPersonsReceivingCare = Me.MyFarmLocation.ResidentNames.Count.ToString()
                    End If
                End If
                Me.ParentVrControl.Populate()
                Me.Save_FireSaveEvent(False)

            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
            End Try
        End If
    End Sub
End Class