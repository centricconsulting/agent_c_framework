Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Public Class ctl_CAP_Driver
    Inherits VRControlBase

    Public Event RemoveDriver(index As Int32)
    Public Event AddDriver() 'Added 12/03/2020 for CAP Endorsements Task 52973 MLW

    Public ReadOnly CAPEndorsementsDictionaryName = "CAPEndorsementsDetails" 'Added 03/30/2021 for CAP Endorsements Task 52973 MLW

    'Added 03/30/2021 for CAP Endorsements Task 52973 MLW
    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If _devDictionaryHelper Is Nothing Then
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(CAPEndorsementsDictionaryName) = False Then
                    _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, CAPEndorsementsDictionaryName, Quote.LobType)
                End If
            End If
            Return _devDictionaryHelper
        End Get
    End Property

    Public Property DriverIndex As Int32
        Get
            If ViewState("vs_DriverIndex") Is Nothing Then
                ViewState("vs_DriverIndex") = -1
            End If
            Return CInt(ViewState("vs_DriverIndex"))
        End Get
        Set(value As Int32)
            ViewState("vs_DriverIndex") = value
        End Set
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.DriverIndex
        End Get
    End Property

    Public ReadOnly Property MyDriver As QuickQuoteDriver
        Get
            'updated 8/15/2018 from Quote.Drivers to use GoverningStateQuote; note: could set variable so it doesn't have to look it up every time
            If Quote IsNot Nothing AndAlso GoverningStateQuote.Drivers IsNot Nothing Then
                If GoverningStateQuote.Drivers.HasItemAtIndex(DriverIndex) Then Return GoverningStateQuote.Drivers(DriverIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Remove?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkBtnAdd.ClientID) 'Added 12/03/2020 for CAP Endorsements Task 52973 MLW

        Me.txtBirthDate.CreateMask("00/00/0000")

    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddlDLState.Items.Count = 0 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddlDLState, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.TextAscending, Me.Quote.LobType)

            ' Set the default state to IN
            ddlDLState.SelectedValue = "16"
        End If
    End Sub

    Private Sub UpdateAccordHeader()
        If MyDriver IsNot Nothing AndAlso MyDriver.Name IsNot Nothing Then
            lblAccordHeader.Text = "Driver #" & DriverIndex + 1.ToString & " " & MyDriver.Name.FirstName.ToUpper & " " & MyDriver.Name.LastName.ToUpper
        End If
    End Sub

    Public Overrides Sub Populate()
        If IsOnAppPage OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Driver") OrElse IsQuoteReadOnly() Then 'Added 12/16/2020 for CAP Endorsements Task 52973 MLW
            Dim bdt As String = ""
            LoadStaticData()

            If MyDriver IsNot Nothing AndAlso MyDriver.Name IsNot Nothing Then
                UpdateAccordHeader()
                'If DriverIndex = 0 Then lnkRemove.Visible = False Else lnkRemove.Visible = True 'moved to below IsQuoteEndorsement else
                txtFirstName.Text = MyDriver.Name.FirstName
                txtMiddleName.Text = MyDriver.Name.MiddleName
                txtLastName.Text = MyDriver.Name.LastName
                bdt = FormatDateMMDDYY(MyDriver.Name.BirthDate)
                'Updated 03/19/2021 for CAP Endorsements Task 52973 MLW
                txtBirthDate.Text = bdt.ReturnEmptyIfDefaultDiamondDate
                'txtBirthDate.Text = bdt
                'txtBirthDate.Text = MyDriver.Name.BirthDate
                txtDLNumber.Text = MyDriver.Name.DriversLicenseNumber
                ddlDLState.SelectedValue = MyDriver.Name.DriversLicenseStateId

                'Added 12/03/2020 for CAP Endorsements Task 52973 MLW
                If IsQuoteEndorsement() Then
                    If Not IsNewDriverOnEndorsement(MyDriver) Then
                        txtFirstName.Enabled = False
                        txtMiddleName.Enabled = False
                        txtLastName.Enabled = False
                        txtBirthDate.Enabled = False
                        txtDLNumber.Enabled = False
                        ddlDLState.Enabled = False
                    End If
                    Dim transactionCount As Integer = ddh.GetEndorsementTransactionCount()
                    If transactionCount >= 3 Then
                        If Not IsNewDriverOnEndorsement(MyDriver) Then
                            lnkRemove.Visible = False
                        End If
                        lnkBtnAdd.Visible = False
                    End If
                Else
                    If DriverIndex = 0 Then lnkRemove.Visible = False Else lnkRemove.Visible = True
                    lnkBtnAdd.Visible = False
                End If

            End If

            'Added 11/30/2020 for CAP Endorsements Tasks 52980 and 52973 MLW
            If IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
                Me.ctlViolationList.DriverIndex = Me.DriverIndex
                ctlViolationList.Visible = True
                Me.PopulateChildControls()
            Else
                ctlViolationList.Visible = False
            End If
        End If
    End Sub

    Private Function FormatDateMMDDYY(indate As String)
        If IsDate(indate) Then
            Dim dt As DateTime = CDate(indate)
            Return dt.Month.ToString.PadLeft(2, "0") & "/" & dt.Day.ToString.PadLeft(2, "0") & "/" & dt.Year.ToString()
        End If
        Return indate
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Added 12/23/2020 for CAP Endorsements Task 52973 MLW
        If IsOnAppPage OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Driver" AndAlso IsNewDriverOnEndorsement(MyDriver)) Then
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
            MyBase.ValidateControl(valArgs)

            Me.ValidationHelper.GroupName = String.Format("Driver #{0}", Me.DriverIndex + 1)

            If txtFirstName.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtFirstName, "Missing First Name", accordList)
            End If
            If txtLastName.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtLastName, "Missing Last Name", accordList)
            End If
            If txtBirthDate.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtBirthDate, "Missing Birth Date", accordList)
            End If
            If txtDLNumber.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtDLNumber, "Missing DL number", accordList)
            End If
            If ddlDLState.SelectedIndex <= 0 Then
                Me.ValidationHelper.AddError(ddlDLState, "Missing DL state", accordList)
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        'Added 12/23/2020 for CAP Endorsements Task 52973 MLW
        If IsOnAppPage OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Driver") Then
            If Quote IsNot Nothing Then
                'updated 8/15/2018 from Quote.Drivers to use GoverningStateQuote; note: could set variable so it doesn't have to look it up every time
                'If GoverningStateQuote.Drivers Is Nothing Then
                '    GoverningStateQuote.Drivers = New List(Of QuickQuoteDriver)
                '    GoverningStateQuote.Drivers.AddNew()
                'End If
                'updated 10/6/2018 to handle for QQ library's CloneObject turning list that is Nothing into list that is Something w/ Count 0
                If GoverningStateQuote.Drivers Is Nothing Then GoverningStateQuote.Drivers = New List(Of QuickQuoteDriver)
                'Updated 03/11/2021 for CAP Endorsements Task 52973 MLW
                'If GoverningStateQuote.Drivers.Count <= 0 Then
                If Not IsQuoteEndorsement() AndAlso GoverningStateQuote.Drivers.Count <= 0 Then
                    GoverningStateQuote.Drivers.AddNew()
                End If

                'Updated 12/03/2020 for CAP Endorsements Task 52973 MLW
                If MyDriver IsNot Nothing AndAlso (Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso IsNewDriverOnEndorsement(MyDriver))) Then
                    'If MyDriver IsNot Nothing Then
                    MyDriver.Name.FirstName = txtFirstName.Text
                    MyDriver.Name.MiddleName = txtMiddleName.Text
                    MyDriver.Name.LastName = txtLastName.Text
                    MyDriver.Name.BirthDate = txtBirthDate.Text
                    MyDriver.Name.DriversLicenseNumber = txtDLNumber.Text
                    MyDriver.Name.DriversLicenseStateId = ddlDLState.SelectedValue

                    ' Default license date, license status and driver exclude type
                    If IsDate(MyDriver.Name.BirthDate) Then
                        Dim licDt As Date = CDate(MyDriver.Name.BirthDate).AddYears(16)
                        MyDriver.Name.DriversLicenseDate = licDt.ToShortDateString()
                    End If
                    MyDriver.LicenseStatusId = "2"
                    MyDriver.DriverExcludeTypeId = "4"

                    'Added 02/25/2021 for CAP Endorsements Task 52973 MLW
                    If IsQuoteEndorsement() AndAlso IsNewDriverOnEndorsement(MyDriver) Then
                        ddh.UpdateDevDictionaryDriverList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, DriverIndex, MyDriver)
                        Dim endorsementsRemarksHelper = New EndorsementsRemarksHelper(ddh)
                        Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(EndorsementsRemarksHelper.RemarksType.Driver)
                        Quote.TransactionRemark = updatedRemarks
                    End If

                    UpdateAccordHeader()

                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        'Added 02/24/2021 for CAP Endorsements Task 52973 MLW
        If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Driver" Then
            If IsNewDriverOnEndorsement(MyDriver) Then
                ddh.UpdateDevDictionaryDriverList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.RemoveAdd, DriverIndex, MyDriver)
            Else
                ddh.UpdateDevDictionaryDriverList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, DriverIndex, MyDriver)
            End If
            Dim endorsementsRemarksHelper = New EndorsementsRemarksHelper(ddh)
            Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(EndorsementsRemarksHelper.RemarksType.Driver)
            Quote.TransactionRemark = updatedRemarks
        End If

        RaiseEvent RemoveDriver(Me.DriverIndex)
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    'Added 12/03/2020 for CAP Endorsements Task 52973 MLW
    Protected Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkBtnAdd.Click
        RaiseEvent AddDriver()
    End Sub

End Class