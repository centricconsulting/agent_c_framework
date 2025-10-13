Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions

Public Class ctl_Employment_Info_PPA
    Inherits VRControlBase

    'This control is only used for HOM, DFR, and PPA, so no multi state changes are needed 9/17/18 MLW

    Public Property DriverIndex As Int32
        Get
            Return ViewState.GetInt32("vs_driverNum", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_driverNum") = value
        End Set
    End Property

    Public ReadOnly Property EmployerNameID() As String
        Get
            Return Me.txtEmployerName.ClientID
        End Get
    End Property

    Public ReadOnly Property OccupationID() As String
        Get
            Return Me.ddIccupation.ClientID
        End Get
    End Property

    Public ReadOnly Property IsRatedDriver As Boolean
        Get
            If MyDriver IsNot Nothing Then
                Dim DriverExcludeTypeId_Rated As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteDriver, QuickQuotePropertyName.DriverExcludeTypeId, "Rated", Me.Quote.LobType)
                If MyDriver.DriverExcludeTypeId = DriverExcludeTypeId_Rated Then
                    Return True
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

    Public Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divMainEmployment.ClientID
        End If
        LoadStaticData()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(MainAccordionDivId, HiddenField1, "false")
        Me.VRScript.CreateConfirmDialog(Me.lnkClearBase.ClientID, "Clear Employment Information?")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddIccupation.Items.Count < 1 Then
            If Me.Quote IsNot Nothing Then
                QQHelper.LoadStaticDataOptionsDropDown(Me.ddIccupation, QuickQuoteClassName.QuickQuoteApplicant, QuickQuotePropertyName.OccupationTypeId, SortBy.TextAscending, Me.Quote.LobType)
            End If
        End If
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            Select Case Me.Quote.LobId
                Case "1" 'ppa
                    If MyDriver IsNot Nothing Then
                        If MyDriver.EmploymentInfo IsNot Nothing Then
                            If MyDriver.EmploymentInfo.Name IsNot Nothing Then
                                Me.txtEmployerName.Text = MyDriver.EmploymentInfo.Name.CommercialName1
                            End If
                            Me.ddIccupation.SetFromValue(MyDriver.EmploymentInfo.OccupationTypeId)
                        End If
                    End If
                Case "2", "3" 'hom, DFR
                    If Me.Quote.Applicants.IsLoaded() Then
                        Me.txtEmployerName.Text = Me.Quote.Applicants(0).Employer
                        Me.ddIccupation.SetFromValue(Me.Quote.Applicants(0).OccupationTypeId)
                    End If

            End Select

        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Employment Information"
        'Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Quote IsNot Nothing Then
            Select Case Me.Quote.LobId
                Case "1" ' ppa
                    LoadStaticData()
                    If MyDriver IsNot Nothing Then
                        If IsRatedDriver Then
                            MyDriver.EmploymentInfo.CreateIfNull().Name.CreateIfNull()
                            MyDriver.EmploymentInfo.Name.CommercialName1 = Me.txtEmployerName.Text
                            MyDriver.EmploymentInfo.OccupationTypeId = Me.ddIccupation.SelectedValue
                        Else
                            MyDriver.EmploymentInfo = Nothing
                        End If

                    End If
                Case "2", "3" ' hom, DFR
                    If Me.Quote.Applicants.IsLoaded Then
                        Me.Quote.Applicants(0).Employer = Me.txtEmployerName.Text.Trim()
                        Me.Quote.Applicants(0).OccupationTypeId = Me.ddIccupation.SelectedValue
                        '?? occupationType
                    End If
            End Select

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
        Me.txtEmployerName.Text = ""
        Me.ddIccupation.SelectedIndex = 0

        MyBase.ClearControl()
    End Sub

End Class