Imports IFM.PrimativeExtensions
Public Class ctl_App_AdditionalPolicyholder
    Inherits VRControlBase

    Public Property AdditionalPolicyholderIndex As Int32
        Get
            Return ViewState.GetInt32("vs_AdditionalPolicyholderIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_AdditionalPolicyholderIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyAdditionalPolicyholder As QuickQuote.CommonObjects.QuickQuoteAdditionalPolicyholder
        Get
            'Return Quote.AdditionalPolicyholders(AdditionalPolicyholderIndex)
            If Quote.IsNotNull AndAlso Quote.AdditionalPolicyholders IsNot Nothing AndAlso Quote.AdditionalPolicyholders.Count > 0 Then
                Return Quote.AdditionalPolicyholders(AdditionalPolicyholderIndex)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return AdditionalPolicyholderIndex
        End Get
    End Property

    Public Event DeleteAddlPHRequested(ByVal AdditionalPolicyholderIndex As Integer)

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.divAddlPH.ClientID, Me.accordActive, "0")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkRemove.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Remove Additional Policyholder?")

        Me.txtSSN.CreateMask("000-00-0000")
        Me.txtFEIN.CreateMask("00-0000000")

        Me.ddTaxIdType.Attributes.Add("onchange", "var ddtyp = document.getElementById('" & ddTaxIdType.ClientID & "');var txtSSN = document.getElementById('" & txtSSN.ClientID & "');var txtFEIN = document.getElementById('" & txtFEIN.ClientID & "');var lblSSN = document.getElementById('" & lblSSN.ClientID & "');var lblFEIN = document.getElementById('" & lblFEIN.ClientID & "');if (ddtyp.selectedIndex == '1') {txtSSN.style.display = 'none';lblSSN.style.display='none';txtFEIN.style.display = '';lblFEIN.style.display='';} else {if (ddtyp.selectedIndex == '2') {txtSSN.style.display = '';lblSSN.style.display='';txtFEIN.style.display = 'none';lblFEIN.style.display='none';} else {txtSSN.style.display = 'none';lblSSN.style.display='none';txtFEIN.style.display = 'none';lblFEIN.style.display='none';}}")

    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()

        If Me.Quote.IsNotNull Then
            If MyAdditionalPolicyholder IsNot Nothing Then
                If MyAdditionalPolicyholder.Name.DisplayName IsNot Nothing AndAlso MyAdditionalPolicyholder.Name.DisplayName <> "" AndAlso MyAdditionalPolicyholder.Name.DisplayName.ToUpper <> "NEW999" Then
                    Dim phnm As String = MyAdditionalPolicyholder.Name.DisplayName
                    If phnm.Length > 28 Then phnm = phnm.Substring(0, 28).Trim & "..."
                    'lblAccordHeader.Text = "Additional Policyholder #" & AdditionalPolicyholderIndex + 1.ToString & " - " & MyAdditionalPolicyholder.Name.DisplayName
                    lblAccordHeader.Text = "Additional Policyholder #" & AdditionalPolicyholderIndex + 1.ToString & " - " & phnm
                Else
                    lblAccordHeader.Text = "Additional Policyholder #" & AdditionalPolicyholderIndex + 1.ToString
                End If

                ddTaxIdType.SelectedValue = MyAdditionalPolicyholder.Name.TaxTypeId
                If ddTaxIdType.SelectedValue = "1" Then
                    txtSSN.Attributes.Add("style", "display:''")
                    lblSSN.Attributes.Add("style", "display:''")
                    txtFEIN.Attributes.Add("style", "display:none")
                    lblFEIN.Attributes.Add("style", "display:none")
                    txtSSN.Text = MyAdditionalPolicyholder.Name.TaxNumber
                ElseIf ddTaxIdType.SelectedValue = "2" Then
                    txtSSN.Attributes.Add("style", "display:none")
                    lblSSN.Attributes.Add("style", "display:none")
                    txtFEIN.Attributes.Add("style", "display:''")
                    lblFEIN.Attributes.Add("style", "display:''")
                    txtFEIN.Text = MyAdditionalPolicyholder.Name.TaxNumber
                Else
                    txtSSN.Attributes.Add("style", "display:none")
                    lblSSN.Attributes.Add("style", "display:none")
                    txtFEIN.Attributes.Add("style", "display:none")
                    lblFEIN.Attributes.Add("style", "display:none")
                End If
                If MyAdditionalPolicyholder.Name.DisplayName.ToUpper <> "NEW999" Then
                    txtName.Text = MyAdditionalPolicyholder.Name.DisplayName
                Else
                    txtName.Text = ""
                End If
            End If
        End If
        Me.PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote.IsNotNull AndAlso MyAdditionalPolicyholder IsNot Nothing Then
            MyAdditionalPolicyholder.Name.TaxTypeId = ddTaxIdType.SelectedValue
            If ddTaxIdType.SelectedValue = "1" Then
                MyAdditionalPolicyholder.Name.TaxNumber = txtSSN.Text
            Else
                MyAdditionalPolicyholder.Name.TaxNumber = txtFEIN.Text
            End If
            MyAdditionalPolicyholder.Name.CommercialName1 = txtName.Text
        End If

        Me.SaveChildControls()
        Populate()

        Return True
    End Function

    Private Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        RaiseEvent DeleteAddlPHRequested(AdditionalPolicyholderIndex)
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        ValidationHelper.GroupName = "Additional Policyholder #" & AdditionalPolicyholderIndex + 1

        If ddTaxIdType.SelectedIndex <= 0 Then
            Me.ValidationHelper.AddError(ddTaxIdType, "Missing Tax ID Type", accordList)
        End If

        If ddTaxIdType.SelectedValue = "1" Then
            If txtSSN.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtSSN, "Missing TIN", accordList)
            Else
                If Not IFM.Common.InputValidation.CommonValidations.IsValidSSN(txtSSN.Text) Then
                    Me.ValidationHelper.AddError(txtFEIN, "TIN is invalid", accordList)
                End If
            End If
        Else
            If ddTaxIdType.SelectedValue = "2" Then
                If txtFEIN.Text.Trim = "" Then
                    Me.ValidationHelper.AddError(txtFEIN, "Missing TIN", accordList)
                Else
                    If Not IFM.Common.InputValidation.CommonValidations.IsValidSSN(txtFEIN.Text) Then
                        Me.ValidationHelper.AddError(txtFEIN, "TIN is invalid", accordList)
                    End If
                End If
            End If
        End If

        If txtName.Text.Trim = "" Then
            Me.ValidationHelper.AddError(txtName, "Missing Name", accordList)
        End If

        Me.ValidateChildControls(valArgs)
    End Sub


End Class