Imports IFM.PrimativeExtensions
Public Class ctl_CPR_App_Location
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
            Me.ctl_CPR_App_BuildingList.LocationIndex = value
        End Set
    End Property

    Private ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.LocationIndex
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Dim title As String = Nothing
        Dim titleLen As Integer = 40

        If Me.MyLocation.IsNotNull Then
            ' Set the title bar
            If MyLocation.Address IsNot Nothing AndAlso MyLocation.Address.DisplayAddress IsNot Nothing AndAlso MyLocation.Address.DisplayAddress <> String.Empty Then
                title = "Location #" & (Me.LocationIndex + 1).ToString & " - "
                If MyLocation.Address.DisplayAddress.Length > titleLen Then
                    title += MyLocation.Address.DisplayAddress.Substring(0, titleLen) & "..."
                Else
                    title += MyLocation.Address.DisplayAddress
                End If
            Else
                title = "Location #" & (Me.LocationIndex + 1).ToString
            End If
            lblAccordHeader.Text = title

        End If

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If MyLocation IsNot Nothing Then
            'If need preprocessing
        End If

        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        ValidationHelper.GroupName = "Location #" & LocationIndex + 1

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    '6/8/2017 note: shouldn't be needed anymore; copied to ctl_CPR_App_Building
    Private Function ValidYear(ByVal testYear As String) As Boolean
        If Not IsNumeric(testYear) Then Return False
        Dim yr As Integer = CInt(testYear)
        If yr > DateTime.Now.Year Then Return False
        If yr < 1900 Then Return False
        Return True
    End Function

End Class