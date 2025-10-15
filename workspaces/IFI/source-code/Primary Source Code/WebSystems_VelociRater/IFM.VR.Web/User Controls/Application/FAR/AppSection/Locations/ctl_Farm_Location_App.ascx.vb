Imports QuickQuote.CommonObjects

Public Class ctl_Farm_Location_App
    Inherits VRControlBase

    Public Property MyLocationIndex As Int32
        Get
            If ViewState("vs_locationIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_locationIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return MyLocationIndex
        End Get
    End Property

    Public Property StartingAcreage As Int32
        Get
            If ViewState("vs_startingAcrege") IsNot Nothing Then
                Return CInt(ViewState("vs_startingAcrege"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_startingAcrege") = value
        End Set
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)

        If Me.Quote IsNot Nothing AndAlso Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            Dim scriptAcreChange As String = "TxtAcerageChange (""" + ctl_Farm_Location_Description.TxtAcresControl.ClientID + """, """ + divBlanketAcreage.ClientID + """,""" + chkBlanketAcreage.ClientID + """, """ + divtxtTotalBlanketAcreage.ClientID + """);"
            Me.VRScript.CreateJSBinding(Me.ctl_Farm_Location_Description.TxtAcresControl, ctlPageStartupScript.JsEventType.onblur, scriptAcreChange, True)
            ctl_Farm_Location_Description.TxtAcresControl.Attributes.Add("onblur", scriptAcreChange)

            Dim scriptRoundBlanketAcre As String = "AlwaysRoundToNextNumber (""" + txtTotalBlanketAcreage.ClientID + """);"
            txtTotalBlanketAcreage.Attributes.Add("onblur", scriptRoundBlanketAcre)

            'Toggle BlanketAcarageTextbox
            Dim scriptchkBlanketAcreage As String = "ToggleBlanketAcreageTxtbox (""" + chkBlanketAcreage.ClientID + """, """ + divtxtTotalBlanketAcreage.ClientID + """);"
            chkBlanketAcreage.Attributes.Add("onclick", scriptchkBlanketAcreage)
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.MyLocation IsNot Nothing Then
            Me.lblHeader.Text = String.Format("Location #{0} - {1} {2} {3}", MyLocationIndex + 1, MyLocation.Address.HouseNum, MyLocation.Address.StreetName, MyLocation.Address.City)
        End If
        Me.ctl_Farm_Location_Description.MyLocationIndex = Me.MyLocationIndex ' 0 'always zero
        Me.ctl_PropertyUpdates_HOM_App.MyLocationIndex = Me.MyLocationIndex
        Me.ctl_Farm_Structures_App.MyLocationIndex = Me.MyLocationIndex
        Me.ctl_Farm_Location_Description_List.MyLocationIndex = Me.MyLocationIndex
        Me.lblPrimary.Visible = MyLocationIndex.Equals(0)

        If Not IsPostBack Then
            StartingAcreage = IFM.VR.Common.Helpers.FARM.AcresOnlyHelper.CurrentTotalAcres(Me.MyLocation, False)
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.Farm

                    If Not IFM.VR.Common.Helpers.FARM.FarmBlanketAcreageHelper.IsFarmBlanketAcreageAvailable(Quote) Then
                        Me.divBlanketAcreage.Visible = False
                    Else
                        Me.divBlanketAcreage.Visible = True
                    End If

                    If Me.MyLocationIndex > 0 Then
                        Me.divBlanketAcreage.Visible = False
                    End If

                    If IFM.VR.Common.Helpers.FARM.FarmBlanketAcreageHelper.IsFarmBlanketAcreageAvailable(Quote) AndAlso MyLocation.Acreages.Any(Function(a) a.LocationAcreageTypeId = "4") Then
                        Dim blanketAcreage As QuickQuoteAcreage = MyLocation.Acreages.FirstOrDefault(Function(a) a.LocationAcreageTypeId = "4")
                        chkBlanketAcreage.Checked = True
                        txtTotalBlanketAcreage.Text = blanketAcreage.Acreage
                    End If

                    Exit Select

                Case Else
                    Exit Select
            End Select
        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If MyLocation IsNot Nothing Then

            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    Dim blanketAcreage As QuickQuoteAcreage = MyLocation.Acreages.FirstOrDefault(Function(a) a.LocationAcreageTypeId = "4")
                    If IFM.VR.Common.Helpers.FARM.FarmBlanketAcreageHelper.IsFarmBlanketAcreageAvailable(Quote) AndAlso chkBlanketAcreage.Checked AndAlso MyLocationIndex = 0 Then
                        If blanketAcreage Is Nothing Then
                            blanketAcreage = New QuickQuoteAcreage()

                            blanketAcreage.Acreage = Me.txtTotalBlanketAcreage.Text.Trim()
                            blanketAcreage.LocationAcreageTypeId = "4"

                            MyLocation.Acreages.Add(blanketAcreage)
                        Else

                            blanketAcreage.Acreage = Me.txtTotalBlanketAcreage.Text.Trim()
                            blanketAcreage.LocationAcreageTypeId = "4"

                        End If
                    Else
                        If blanketAcreage IsNot Nothing Then
                            MyLocation.Acreages.Remove(blanketAcreage)
                        End If

                    End If

                    Exit Select

                Case Else
                    Exit Select
            End Select
        End If

        Me.SaveChildControls()
        Me.PopulateChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Select Case Me.Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.Farm
                Dim txtAcerageInput As Integer
                Dim txtBlanketAcerageInput As Integer
                ctl_Farm_Location_Description.AcresText()
                Dim hasTxtAcerage As Boolean = Integer.TryParse(ctl_Farm_Location_Description.AcresText(), txtAcerageInput)

                If txtAcerageInput > 0 AndAlso chkBlanketAcreage.Checked Then
                    If Not Integer.TryParse(txtTotalBlanketAcreage.Text, txtBlanketAcerageInput) Then
                        Me.ValidationHelper.AddError("Missing Total Blanket Acreage.", txtTotalBlanketAcreage.ClientID)
                    ElseIf txtBlanketAcerageInput <= 1 Then
                        Me.ValidationHelper.AddError("Total Blanket Acreage must be greater than 1.", txtTotalBlanketAcreage.ClientID)
                    End If
                End If
                Exit Select

            Case Else
                Exit Select
        End Select

        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click

        If MyLocation IsNot Nothing Then
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    Dim blanketAcreage As QuickQuoteAcreage = MyLocation.Acreages.FirstOrDefault(Function(a) a.LocationAcreageTypeId = "4")
                    If IFM.VR.Common.Helpers.FARM.FarmBlanketAcreageHelper.IsFarmBlanketAcreageAvailable(Quote) AndAlso chkBlanketAcreage.Checked AndAlso MyLocationIndex = 0 Then
                        If blanketAcreage Is Nothing Then
                            blanketAcreage = New QuickQuoteAcreage()
                            blanketAcreage.Acreage = Me.txtTotalBlanketAcreage.Text.Trim()
                            blanketAcreage.LocationAcreageTypeId = "4"
                            MyLocation.Acreages.Add(blanketAcreage)
                        Else

                            blanketAcreage.Acreage = Me.txtTotalBlanketAcreage.Text.Trim()
                            blanketAcreage.LocationAcreageTypeId = "4"
                        End If
                    Else
                        If blanketAcreage IsNot Nothing Then
                            MyLocation.Acreages.Remove(blanketAcreage)
                        End If
                    End If
                    Exit Select

                Case Else
                    Exit Select
            End Select
        End If

        Me.Save_FireSaveEvent()
    End Sub

    Private Sub ctl_Farm_Location_Description_List_SaveComplete() Handles ctl_Farm_Location_Description_List.SaveComplete
        Me.ctl_Farm_Location_Description.SaveAcresAfterListSave()
    End Sub
End Class