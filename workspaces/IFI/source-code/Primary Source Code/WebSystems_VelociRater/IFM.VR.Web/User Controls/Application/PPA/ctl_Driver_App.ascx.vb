Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions

Public Class ctl_Driver_App
    Inherits VRControlBase

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

    Public Property DriverIndex As Int32
        Get
            Return ViewState.GetInt32("vs_driverNum", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_driverNum") = value
            Me.ctl_LossViolations_App.DriverIndex = value
            Me.ctl_Employment_Info_PPA.DriverIndex = value
        End Set

    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.DriverIndex
        End Get
    End Property

    Public ReadOnly Property IsRatedDriver As Boolean
        Get
            If Me.Quote IsNot Nothing Then
                Dim driver As QuickQuote.CommonObjects.QuickQuoteDriver = Me.Quote.Drivers.GetItemAtIndex(DriverIndex)
                If driver IsNot Nothing Then
                    Dim DriverExcludeTypeId_Rated As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteDriver, QuickQuotePropertyName.DriverExcludeTypeId, "Rated", Me.Quote.LobType)
                    If driver.DriverExcludeTypeId = DriverExcludeTypeId_Rated Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property IsNonRatedDriver As Boolean
        Get
            If Me.Quote IsNot Nothing Then
                Dim driver As QuickQuote.CommonObjects.QuickQuoteDriver = Me.Quote.Drivers.GetItemAtIndex(DriverIndex)
                If driver IsNot Nothing Then
                    Dim DriverExcludeTypeId_NonRated As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteDriver, QuickQuotePropertyName.DriverExcludeTypeId, "NonRated", Me.Quote.LobType)
                    If driver.DriverExcludeTypeId = DriverExcludeTypeId_NonRated Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property MyDriver As QuickQuote.CommonObjects.QuickQuoteDriver
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Drivers.GetItemAtIndex(DriverIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            LoadStaticData()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID, True)
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddDLState.Items.Count < 1 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddDLState, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            LoadStaticData()

            If MyDriver IsNot Nothing Then
                Me.lblAccordHeader.Text = String.Format("Driver #{0} - {1} {2}", Me.DriverIndex + 1, MyDriver.Name.FirstName, MyDriver.Name.LastName)
                If String.IsNullOrWhiteSpace(MyDriver.Name.MiddleName) = False Then
                    Me.lblAccordHeader.Text = String.Format("Driver #{0} - {1} {2} {3}", Me.DriverIndex + 1, MyDriver.Name.FirstName, MyDriver.Name.MiddleName, MyDriver.Name.LastName)
                End If
                Me.divNoContent.Visible = False
                Dim DriverExcludeTypeId_Rated As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteDriver, QuickQuotePropertyName.DriverExcludeTypeId, "Rated", Me.Quote.LobType)
                If IsNonRatedDriver = False AndAlso IsRatedDriver = False Then
                    Me.txtDLDate.Enabled = False
                    Me.txtDLNumber.Enabled = False
                    Me.ddDLState.Enabled = False
                    Me.lblAccordHeader.Text = Me.lblAccordHeader.Text + " - Unrated Driver"
                    Me.ctl_LossViolations_App.Visible = False
                    'disable the accordion panel
                    Me.divContent.Visible = False
                    Me.divNoContent.Visible = True
                    Me.lbl_No_Content.Text = "No additional information required for unrated drivers."
                End If
                If IsNonRatedDriver Then
                    Me.lblAccordHeader.Text = Me.lblAccordHeader.Text + " - Unrated Driver"
                End If

                Me.lblAccordHeader.Text = IFM.Common.InputValidation.InputHelpers.EllipsisText(Me.lblAccordHeader.Text, 70)

                Me.txtDLNumber.Text = MyDriver.Name.DriversLicenseNumber

                Dim _dl As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, "IN", Me.Quote.LobType)
                If String.IsNullOrWhiteSpace(MyDriver.Name.DriversLicenseStateId) Or MyDriver.Name.DriversLicenseStateId = "0" Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddDLState, "16")
                Else
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddDLState, MyDriver.Name.DriversLicenseStateId)
                End If

                If String.IsNullOrWhiteSpace(MyDriver.Name.DriversLicenseDate) OrElse IsDate(MyDriver.Name.DriversLicenseDate) = False Then
                    If DateTime.TryParse(MyDriver.Name.BirthDate, Nothing) Then
                        Me.txtDLDate.Text = CDate(MyDriver.Name.BirthDate).AddYears(16)
                    Else
                        Me.txtDLDate.Text = MyDriver.Name.DriversLicenseDate
                    End If
                Else
                    Me.txtDLDate.Text = MyDriver.Name.DriversLicenseDate
                End If

                Me.ctl_LossViolations_App.DriverIndex = Me.DriverIndex

                Me.ctl_Employment_Info_PPA.DriverIndex = Me.DriverIndex

                Me.PopulateChildControls()

            End If

        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = String.Format("Driver #{0}", Me.DriverIndex + 1)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim valItems = DriverValidator.ValidateDriver(Me.DriverIndex, Me.Quote, valArgs.ValidationType)
        If valItems.Any() Then

            For Each v In valItems
                Select Case v.FieldId
                    Case DriverValidator.DriverDLNumber
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDLNumber, v, accordList)
                    Case DriverValidator.DriverDLState
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddDLState, v, accordList)
                    Case DriverValidator.DriverDLDate
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDLDate, v, accordList)
                End Select
            Next
        End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Visible Then
            If Quote IsNot Nothing Then
                If MyDriver IsNot Nothing Then
                    If IsRatedDriver OrElse IsNonRatedDriver Then
                        If MyDriver.Name IsNot Nothing Then
                            MyDriver.Name.DriversLicenseNumber = Me.txtDLNumber.Text.Replace("-", "").Trim()
                            MyDriver.Name.DriversLicenseStateId = Me.ddDLState.Text 'straight text no ID
                            MyDriver.Name.DriversLicenseDate = Me.txtDLDate.Text.Trim()
                        End If
                    End If
                    Me.SaveChildControls()
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Public Overrides Sub ClearControl()
        Me.txtDLDate.Text = ""
        Me.txtDLNumber.Text = ""
        MyBase.ClearControl()
    End Sub

End Class