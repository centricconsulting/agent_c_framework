Imports IFM.PrimativeExtensions

Public Class ctl_LossViolations_App
    Inherits VRControlBase

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

    Public Property DriverIndex As Int32
        Get
            Return ViewState.GetInt32("vs_driverNum", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_driverNum") = value
            Me.ctlAccidentHistoryList.DriverIndex = value
            Me.ctlViolationList.DriverIndex = value
        End Set
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.DriverIndex
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
            Me.MainAccordionDivId = Me.divAccidentsAndViolations_App.ClientID
        End If

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(MainAccordionDivId, hidden_divAccidentsAndViolations_App, "0")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If MyDriver IsNot Nothing Then
            Dim avCount As Integer = 0
            If MyDriver.LossHistoryRecords IsNot Nothing Then
                avCount += MyDriver.LossHistoryRecords.Count 'dont use CountEvenIfNull()
            End If
            If MyDriver.AccidentViolations IsNot Nothing Then
                avCount += MyDriver.AccidentViolations.Count 'dont use CountEvenIfNull()
            End If
            Me.lblAccordHeader.Text = String.Format("Loss History/Violations ({0})", avCount)
        End If
        Me.PopulateChildControls()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = String.Format("Loss History/Violations")
        'Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it ' readonly on App so no need
    End Sub

    Public Overrides Function Save() As Boolean
        'Me.SaveChildControls() ' readonly on App so no need
        Return True
    End Function

    Protected Sub lnkMVRReport_Click(sender As Object, e As EventArgs) Handles lnkMVRReport.Click
        Dim Err As String = Nothing
        If MyDriver IsNot Nothing Then
            Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.PERSONAL_AUTO_GetMVRReport(Me.Quote, MyDriver.DriverNum, Err, True)
            If ReportFile IsNot Nothing Then
                Response.ContentType = "application/pdf"
                Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("MVR_{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
                Response.BinaryWrite(ReportFile)
            Else
                Me.VRScript.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
            End If
        Else
            Me.VRScript.AddScriptLine("alert('Driver number not found.');")
        End If
    End Sub

    Protected Sub lnkClueReport_Click(sender As Object, e As EventArgs) Handles lnkClueReport.Click
        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.PERSONAL_AUTO_GetCLUEReport(Me.Quote, Err, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("CLUE_{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
            Response.BinaryWrite(ReportFile)
        Else
            Me.VRScript.AddScriptLine("alert('No Loss Records Found.');")
        End If
    End Sub

End Class