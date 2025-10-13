Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption

Public Class ctl_Location_displayAddress
    Inherits VRControlBase

#Region "Declarations"

    Public Property WorkplaceIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocAddressIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocAddressIndex") = value
        End Set
    End Property

    Private ReadOnly Property MyWorkplace As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(WorkplaceIndex) Then
                Return Quote.Locations(WorkplaceIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.WorkplaceIndex
        End Get
    End Property


    Public Event AddWorkplaceRequested()
    Public Event DeleteWorkplaceRequested(ByVal WPIndex As Integer)
    Public Event ClearWorkplaceRequested(ByVal WPIndex As Integer)

#End Region

#Region "Methods and Functions"

    Private Sub HandleError(ByVal RoutineName As String, ByVal ex As Exception)
        Dim str As String = RoutineName & ":  " & ex.Message
        If AppSettings("TestOrProd").ToUpper <> "PROD" Then lblMsg.Text = str Else Throw New Exception(ex.Message, ex)
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkDelete.ClientID)
        'Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Delete?")
        'Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear?")

        Me.VRScript.CreateJSBinding(Me.txtZipcode, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + Me.txtZipcode.ClientID + "','" + Me.ddCityName.ClientID + "','" + Me.txtCity.ClientID + "','" + Me.txtCounty.ClientID + "','" + Me.ddState.ClientID + "');")
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddState.Items.Count = 0 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddState, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Private Sub UpdateAccordHeader()
        If MyWorkplace IsNot Nothing AndAlso MyWorkplace.Address IsNot Nothing Then
            Dim dsc As String = Nothing
            If MyWorkplace.Address.DisplayAddress.Trim.Length <= 23 Then
                dsc = MyWorkplace.Address.DisplayAddress.Trim.ToUpper
            Else
                dsc = MyWorkplace.Address.DisplayAddress.Trim.ToUpper.Substring(0, 24) & "...".ToUpper
            End If
            lblAccordHeader.Text = "Location # " & WorkplaceIndex + 1.ToString & " " & dsc
        Else
            lblAccordHeader.Text = "Location # " & WorkplaceIndex + 1.ToString
        End If
    End Sub

    Public Overrides Sub Populate()
        Dim err As String = Nothing

        Try
            LoadStaticData()

            ' Only show the remove button on Workplace #2 and greater
            If WorkplaceIndex = 0 Then
                Me.lnkDelete.Visible = False
            Else
                Me.lnkDelete.Visible = True
            End If

            ' Never show the city dropdown, only there for the zip code lookup
            ddCityName.Attributes.Add("style", "display:none")

            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(WorkplaceIndex) Then
                If Quote.Locations(WorkplaceIndex).Address IsNot Nothing Then
                    txtStreetNum.Text = Quote.Locations(WorkplaceIndex).Address.HouseNum
                    txtStreetName.Text = Quote.Locations(WorkplaceIndex).Address.StreetName
                    txtApartmentNumber.Text = Quote.Locations(WorkplaceIndex).Address.ApartmentNumber
                    txtCity.Text = Quote.Locations(WorkplaceIndex).Address.City
                    txtCounty.Text = Quote.Locations(WorkplaceIndex).Address.County
                    ddState.SelectedValue = Quote.Locations(WorkplaceIndex).Address.StateId
                    txtZipcode.Text = Quote.Locations(WorkplaceIndex).Address.Zip
                End If
                UpdateAccordHeader()
            End If

            Me.PopulateChildControls()

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
        End Try
    End Sub

    Public Overrides Function Save() As Boolean
        Dim cls As QuickQuote.CommonObjects.QuickQuoteClassification = Nothing
        Dim dia_id As String = Nothing
        Dim err As String = Nothing
        Dim pn As String = Nothing
        Dim pa As String = Nothing

        Try
            lblMsg.Text = "&nbsp;"

            If MyWorkplace IsNot Nothing Then
                If MyWorkplace.Address Is Nothing Then MyWorkplace.Address = New QuickQuote.CommonObjects.QuickQuoteAddress
                MyWorkplace.Address.HouseNum = txtStreetNum.Text
                MyWorkplace.Address.StreetName = txtStreetName.Text
                MyWorkplace.Address.City = txtCity.Text
                MyWorkplace.Address.StateId = ddState.SelectedValue
                MyWorkplace.Address.County = txtCounty.Text
                MyWorkplace.Address.ApartmentNumber = txtApartmentNumber.Text
                MyWorkplace.Address.Zip = txtZipcode.Text
                UpdateAccordHeader()
            End If

            Me.SaveChildControls()

            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Try
            MyBase.ValidateControl(valArgs)
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
            Me.ValidationHelper.GroupName = "Workplace #" & WorkplaceIndex + 1

            If txtStreetNum.Text = "" Then
                Me.ValidationHelper.AddError(txtStreetNum, "Missing Street Number", accordList)
            End If
            If txtStreetName.Text = "" Then
                Me.ValidationHelper.AddError(txtStreetName, "Missing Street Name", accordList)
            End If
            If txtCity.Text = "" Then
                Me.ValidationHelper.AddError(txtCity, "Missing City", accordList)
            End If
            If txtZipcode.Text = "" Then
                Me.ValidationHelper.AddError(txtZipcode, "Missing Zipcode", accordList)
            End If
            If txtCounty.Text = "" Then
                Me.ValidationHelper.AddError(txtCounty, "Missing County", accordList)
            End If

            Me.ValidateChildControls(valArgs)

            Exit Sub
        Catch ex As Exception
            HandleError("ValidateControls", ex)
            Exit Sub
        End Try
    End Sub


    Private Sub ClearInputFields()
        Try
            txtStreetNum.Text = ""
            txtStreetName.Text = ""
            txtCity.Text = ""
            ddState.SelectedIndex = 0
            txtCounty.Text = ""
            txtZipcode.Text = ""

            Exit Sub
        Catch ex As Exception
            HandleError("ClearInputFields", ex)
            Exit Sub
        End Try
    End Sub

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not IsPostBack Then
                Me.MainAccordionDivId = Me.divWCPWorkplace.ClientID
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("Page LOAD", ex)
            Exit Sub
        End Try
    End Sub

    'Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
    '    Try
    '        Me.Save_FireSaveEvent()
    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("Save", ex)
    '        Exit Sub
    '    End Try
    'End Sub

    'Private Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
    '    RaiseEvent ClearWorkplaceRequested(WorkplaceIndex)
    'End Sub

    Private Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        RaiseEvent DeleteWorkplaceRequested(WorkplaceIndex)
    End Sub

#End Region

End Class