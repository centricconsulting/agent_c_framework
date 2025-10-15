Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Public Class Cov_CanineExclusionItem
    Inherits VRControlBase

    Dim _qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass

    Public Property CoverageIndex As Int32
        Get
            If ViewState("vs_Covindex") Is Nothing Then
                ViewState("vs_Covindex") = 0
            End If
            Return CInt(ViewState("vs_Covindex"))
        End Get
        Set(value As Int32)
            ViewState("vs_Covindex") = value
        End Set
    End Property
    Public ReadOnly Property MyLocationZero As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote?.Locations.GetItemAtIndex(0)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property MySectionIICoverages() As List(Of QuickQuoteSectionIICoverage)
        Get
            Return MyLocationZero?.SectionIICoverages
        End Get
    End Property

    Public ReadOnly Property MySectionCoverage As QuickQuoteSectionIICoverage
        Get
            Dim genericCov As QuickQuoteSectionIICoverage = Nothing
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(0) Then
                If MyLocationZero IsNot Nothing Then
                    If MyLocationZero.SectionIICoverages IsNot Nothing Then
                        Dim cov = (From sc In MyLocationZero.SectionIICoverages Where sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion Select sc).GetItemAtIndex(Me.CoverageIndex)
                        If cov IsNot Nothing Then
                            genericCov = cov
                        End If
                    End If
                End If
            End If

            Return genericCov
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If MySectionCoverage IsNot Nothing Then
            MySectionCoverage.Name.FirstName = Me.txtName.Text.Trim()
            MySectionCoverage.Description = Me.txtDescription.Text.Trim()
            MySectionCoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion
        Else
            Dim parentChkBox = CType(ParentVrControl.FindControl("chkCanine"), CheckBox)
            If parentChkBox.Checked Then
                If (txtName.Text.IsNullEmptyorWhitespace = False AndAlso txtDescription.Text.IsNullEmptyorWhitespace = False) OrElse Left(Me.txtDescription.Text.ToUpper, 8) = "CANINE #" Then
                    Dim newCoverage = New QuickQuoteSectionIICoverage
                    newCoverage.Name.FirstName = Me.txtName.Text.Trim()
                    newCoverage.Description = Me.txtDescription.Text.Trim()
                    newCoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion
                    MySectionIICoverages.Add(newCoverage)
                End If
            End If
        End If

            Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Canine Exclusion"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        If Me.MySectionCoverage IsNot Nothing Then
            Dim valList = IFM.VR.Validation.ObjectValidation.FarmLines.CanineExclusion.ValidateCanineExclusionCoverage(Me.Quote, Me.MySectionCoverage, Me.DefaultValidationType)
            If valList.Any() Then
                For Each v In valList
                    Select Case v.FieldId
                        Case IFM.VR.Validation.ObjectValidation.FarmLines.CanineExclusion.Description
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.FarmLines.CanineExclusion.Name
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtName, v, accordList)
                    End Select
                Next

            End If
        End If
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Me.Save_FireSaveEvent(False)
        MySectionIICoverages.Remove(MySectionCoverage)
        Me.Populate_FirePopulateEvent()
        Me.Save_FireSaveEvent(False)
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        Me.txtName.Text = ""
        Me.txtDescription.Text = ""
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If MySectionCoverage IsNot Nothing Then
            Me.txtName.Text = MySectionCoverage.Name.FirstName
            Me.txtDescription.Text = MySectionCoverage.Description
        Else
            Me.ClearControl()
        End If

        If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
            If Me.IsOnAppPage AndAlso (Left(Me.txtDescription.Text.ToUpper, 8) = "CANINE #" OrElse Me.txtDescription.Text = String.Empty) Then 'On app never add canine placeholder
                Me.txtDescription.Text = String.Empty
            Else
                If Me.txtDescription.Text = String.Empty Then 'on quote add canine # if empty string
                    Me.txtDescription.Text = "CANINE #" & (Me.CoverageIndex + 1).ToString()
                Else
                    If Left(Me.txtDescription.Text, 8) = "CANINE #" Then 'on quote renumber canine #
                        Me.txtDescription.Text = "CANINE #" & (Me.CoverageIndex + 1).ToString()
                    End If
                End If
            End If
        Else
            If Left(Me.txtDescription.Text.ToUpper, 8) = "CANINE #" Then 'on Endo or ReadOnly clear the placeholder
                Me.txtDescription.Text = String.Empty
            End If
        End If

        If Me.IsQuoteReadOnly Then
            lnkDelete.Visible = False
            lblMaxCharCount.Visible = False
            lblMaxChar.Visible = False
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()

        Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
        VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)

        txtDescription.Attributes.Add("onfocus", "this.select()")

        Me.VRScript.CreateJSBinding(Me.lnkDelete.ClientID, "click", "return confirm('Are you sure you want to delete this item?');")
        Me.VRScript.CreateTextBoxFormatter(txtName, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onkeyup)

    End Sub
End Class