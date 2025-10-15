Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption

Public Class ctl_Farm_Location_Description
    Inherits VRControlBase

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public Property MyAcresIndex As Int32
        Get
            Return ViewState.GetInt32("vs_MyAcresIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_MyAcresIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Locations.GetItemAtIndex(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property MyAcresObject As QuickQuote.CommonObjects.QuickQuoteAcreage
        Get
            If MyLocation IsNot Nothing Then
                Return MyLocation.Acreages.GetItemAtIndex(MyAcresIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Function TxtAcresControl() As TextBox
        Return Me.txtAcres
    End Function

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Delete?")
        Me.VRScript.AddScriptLine("$(""#" + Me.txtCounty.ClientID + """).autocomplete({ source: indiana_Counties });")
        'Todo - Matt - MS - I'd be surprised if this works because the location probably hasn't been moved to the state part yet to get the proper versionId. Probably need to lookup versionId from lob, effective, and whatever the location thinks its current stateid is.
        Dim versionIdForLocation As String = If(String.IsNullOrWhiteSpace(Me.SubQuoteForLocation(Me.MyLocation)?.VersionId) = False, Me.SubQuoteForLocation(Me.MyLocation).VersionId, Me.Quote.VersionId)
        'Dim townshipLookup As String = $"VRData.TownShip.GetTownshipsByCountyNameBindToDropdown('{Me.ddTownshipName.ClientID}',$('#{Me.txtCounty.ClientID}').val(),'{versionIdForLocation}','{Me.hdnTownshipName.ClientID}');"
        'Updated 10/24/2019 for bug 41036 MLW
        'Dim townshipLookup As String = $"VRData.TownShip.GetTownshipsByCountyNameBindToDropdown('{Me.ddTownshipName.ClientID}',$('#{Me.txtCounty.ClientID}').val(),null,'{Me.hdnTownshipName.ClientID}','{Me.MyLocation.Address.StateId}', master_LobId, master_effectiveDate);"
        Dim townshipLookup As String = ""
        If ddStateAbbrev.SelectedValue = "" Or ddStateAbbrev.SelectedValue = "0" Then
            If Me.MyAcresObject IsNot Nothing AndAlso (Me.MyAcresObject.StateId = "" Or Me.MyAcresObject.StateId = "0") Then
                townshipLookup = $"VRData.TownShip.GetTownshipsByCountyNameBindToDropdown('{Me.ddTownshipName.ClientID}',$('#{Me.txtCounty.ClientID}').val(),null,'{Me.hdnTownshipName.ClientID}','{Me.MyLocation.Address.StateId}', master_LobId, master_effectiveDate);"
            Else
                townshipLookup = $"VRData.TownShip.GetTownshipsByCountyNameBindToDropdown('{Me.ddTownshipName.ClientID}',$('#{Me.txtCounty.ClientID}').val(),null,'{Me.hdnTownshipName.ClientID}','{Me.MyAcresObject.StateId}', master_LobId, master_effectiveDate);"
            End If
        Else
            townshipLookup = $"VRData.TownShip.GetTownshipsByCountyNameBindToDropdown('{Me.ddTownshipName.ClientID}',$('#{Me.txtCounty.ClientID}').val(),null,'{Me.hdnTownshipName.ClientID}',$('#{Me.ddStateAbbrev.ClientID}').val(), master_LobId, master_effectiveDate);"
        End If

        Me.VRScript.AddScriptLine(townshipLookup) ' do a lookup on startup of page
        Me.VRScript.CreateJSBinding(Me.txtCounty, ctlPageStartupScript.JsEventType.onblur, townshipLookup)
        'Added 10/24/2019 for bug 41036 MLW
        If MyAcresObject IsNot Nothing AndAlso (MyAcresObject.LocationAcreageTypeId = "3" OrElse MyAcresObject.LocationAcreageTypeId = "") Then
            Me.VRScript.CreateJSBinding(Me.ddStateAbbrev, ctlPageStartupScript.JsEventType.onchange, townshipLookup)
            'Added 11/13/2019 for bug 41224 MLW
            Me.VRScript.CreateJSBinding(Me.ddStateAbbrev, ctlPageStartupScript.JsEventType.onchange, "$('#" + Me.hdnTownshipName.ClientID + "').val($('#" + Me.ddTownshipName.ClientID + "').val());")
        End If
        Me.VRScript.CreateJSBinding(Me.ddTownshipName, ctlPageStartupScript.JsEventType.onchange, "$('#" + Me.hdnTownshipName.ClientID + "').val($('#" + Me.ddTownshipName.ClientID + "').val());")
        'Added 11/13/2019 for bug 41224 MLW
        Me.VRScript.CreateJSBinding(Me.txtCounty, ctlPageStartupScript.JsEventType.onblur, "$('#" + Me.hdnTownshipName.ClientID + "').val($('#" + Me.ddTownshipName.ClientID + "').val());")


        Me.VRScript.CreateTextBoxFormatter(Me.txtAcres, ctlPageStartupScript.FormatterType.NumericNoCommas, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtSection, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtTownshipNum, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtRange, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onkeyup)

        If IsQuoteReadOnly() OrElse IsQuoteEndorsement() Then
            If Me.txtAcres.Enabled Then
                Me.VRScript.CreateJSBinding(Me.txtAcres.ClientID, ctlPageStartupScript.JsEventType.onkeyup, String.Format("AlwaysRoundToNextNumber(""" + Me.txtAcres.ClientID + """);"))
            End If
        Else
            If IsOnAppPage Then
                If Me.txtAcres.Enabled Then ' adjusts the primary acres when you modify acreage only locations
                    ' is acresOnly
                    'Dim scriptRoundAcre As String = "AlwaysRoundToNextNumber(""" + txtAcres.ClientID + """);"
                    'txtAcres.Attributes.Add("onblur", scriptRoundAcre)

                    Me.VRScript.AddScriptLine(String.Format("acresLocation_{0}.push(""{1}"");", Me.MyLocationIndex, Me.txtAcres.ClientID))
                    Me.VRScript.CreateJSBinding(Me.txtAcres.ClientID, ctlPageStartupScript.JsEventType.onkeyup, String.Format("AlwaysRoundToNextNumber(""" + txtAcres.ClientID + """); AdjustPrimaryAcres(acresLocation_{0}, {1}, acresLocation_{0}_starting);", Me.MyLocationIndex, Me.MyAcresIndex))
                    'VRScript.CreateJSBinding(txtAcres.ClientID, ctlPageStartupScript.JsEventType.onblur, String.Format("AlwaysRoundToNextNumber(""" + txtAcres.ClientID + """); AdjustPrimaryAcres(acresLocation_{0}, {1}, acresLocation_{0}_starting);", MyLocationIndex, MyAcresIndex))
                Else
                    ' is Primary
                    Me.VRScript.AddScriptLine(String.Format("var acresLocation_{0} = new Array();", Me.MyLocationIndex, Me.txtAcres.ClientID))
                    Me.VRScript.AddScriptLine(String.Format("acresLocation_{0}.push(""{1}"");", Me.MyLocationIndex, Me.txtAcres.ClientID))
                    Dim myLocControl As ctl_Farm_Location_App = Me.FindFirstVRParentOfType(Of ctl_Farm_Location_App)()
                    Me.VRScript.AddScriptLine(String.Format("var acresLocation_{0}_starting = {1};", Me.MyLocationIndex, myLocControl.StartingAcreage))
                End If
            Else
                If Me.txtAcres.Enabled Then
                    Me.VRScript.AddScriptLine(String.Format("acresLocation_{0}.push(""{1}"");", Me.MyLocationIndex, Me.txtAcres.ClientID))
                    Me.VRScript.CreateJSBinding(Me.txtAcres.ClientID, ctlPageStartupScript.JsEventType.onchange, String.Format("AlwaysRoundToNextNumber(""" + txtAcres.ClientID + """); AdjustPrimaryAcres(acresLocation_{0}, {1}, acresLocation_{0}_starting);", Me.MyLocationIndex, Me.MyAcresIndex))
                End If
            End If
        End If

    End Sub

    Public Overrides Sub LoadStaticData()
        'Added 10/24/2019 for bug 41036 MLW
        If Me.ddStateAbbrev.Items.Count = 0 AndAlso (MyAcresObject.LocationAcreageTypeId = "3" OrElse MyAcresObject.LocationAcreageTypeId = "") Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        If Me.MyAcresObject IsNot Nothing Then
            LoadStaticData() 'Added 10/24/2019 for bug 41036 MLW
            Me.ValidationHelper.GroupName = String.Format("Location #{0} - Acreage", MyLocationIndex + 1)
            If Not Me.MyAcresObject.LocationAcreageTypeId = "4" Then
                Me.txtAcres.Text = MyAcresObject.Acreage
                If IsQuoteReadOnly() = False AndAlso IsQuoteEndorsement() = False Then
                    Me.txtAcres.Enabled = Not (Me.MyAcresObject.LocationAcreageTypeId = "1" Or Me.MyAcresObject.LocationAcreageTypeId = "2" Or Me.MyAcresObject.LocationAcreageTypeId = "4")
                    If txtAcres.Enabled = False Then
                        'primary
                        Me.txtAcres.ToolTip = "Save or Rate to show updated acres. As you add/remove/modify Acreage Only Locations this Primary Acres will adjust accordingly."
                    End If
                End If

                'If IsQuoteReadOnly() OrElse IsQuoteEndorsement() Then
                '    Me.txtAcres.Text = MyAcresObject.Acreage
                'Else
                '    Me.txtAcres.Enabled = Not (Me.MyAcresObject.LocationAcreageTypeId = "1" Or Me.MyAcresObject.LocationAcreageTypeId = "2")
                '    If txtAcres.Enabled = False Then
                '        'primary
                '        Me.txtAcres.ToolTip = "Save or Rate to show updated acres. As you add/remove/modify Acreage Only Locations this Primary Acres will adjust accordingly."

                '        Dim myLocControl As ctl_Farm_Location_App = Me.FindFirstVRParentOfType(Of ctl_Farm_Location_App)()
                '        Dim primAcreage As Int32 = myLocControl.StartingAcreage - IFM.VR.Common.Helpers.FARM.AcresOnlyHelper.CurrentTotalAcres(Me.MyLocation, True)
                '        If primAcreage < 1 Then
                '            Me.txtAcres.Text = MyAcresObject.Acreage ' just stay the same
                '        Else
                '            Me.txtAcres.Text = primAcreage
                '            If primAcreage <> MyAcresObject.Acreage Then
                '                'Me.txtAcres.ToolTip = String.Format("Adjusted from {0} do to additional acreage locations.", primAcreage)
                '                Me.ValidationHelper.AddWarning(String.Format("Primary Acres adjusted from {1} to {0} due to additional location acres entered/modified.", primAcreage, MyAcresObject.Acreage))
                '            End If
                '        End If

                '    Else
                '        Me.txtAcres.Text = MyAcresObject.Acreage
                '    End If
                'End If

                Me.txtSection.Text = MyAcresObject.Section
                Me.txtRange.Text = MyAcresObject.Range
                Me.txtTownshipNum.Text = MyAcresObject.Twp
                hdnTownshipName.Value = MyAcresObject.TownshipCodeTypeId
                Me.txtDescription.Text = MyAcresObject.Description
                Me.txtCounty.Text = MyAcresObject.County
                'Added 10/24/2019 for bug 41036 MLW
                If MyAcresObject.LocationAcreageTypeId = "3" OrElse MyAcresObject.LocationAcreageTypeId = "" OrElse IsQuoteEndorsement() Then
                    If Me.MyAcresObject.StateId = "" Or Me.MyAcresObject.StateId = "0" Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, Me.MyLocation.Address.StateId)
                    Else
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, MyAcresObject.StateId)
                    End If
                Else
                    Me.tdStateAbbrev.Style.Add("display", "none")
                End If
                Me.PopulateChildControls()
                Me.lnkDelete.Visible = Me.MyAcresIndex <> 0
            End If

        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Function AcresText() As String
        Dim a As String = String.Empty

        If IsQuoteReadOnly() OrElse IsQuoteEndorsement() Then
            a = Me.txtAcres.Text.Trim()
        Else
            If txtAcres.Enabled = False Then
                'primary
                'Me.txtAcres.ToolTip = "Save or Rate to show updated acres. As you add/remove/modify Acreage Only Locations this Primary Acres will adjust accordingly."

                Dim myLocControl As ctl_Farm_Location_App = Me.FindFirstVRParentOfType(Of ctl_Farm_Location_App)()
                Dim primAcreage As Int32 = myLocControl.StartingAcreage - IFM.VR.Common.Helpers.FARM.AcresOnlyHelper.CurrentTotalAcres(Me.MyLocation, True)
                If primAcreage < 1 Then
                    a = Me.txtAcres.Text.Trim() ' just stay the same
                Else
                    a = primAcreage
                    If primAcreage <> MyAcresObject.Acreage Then
                        'Me.txtAcres.ToolTip = String.Format("Adjusted from {0} do to additional acreage locations.", primAcreage)
                        Me.ValidationHelper.AddWarning(String.Format("Primary Acres adjusted from {1} to {0} due to additional location acres entered/modified.", primAcreage, MyAcresObject.Acreage))
                    End If
                End If
            Else
                a = Me.txtAcres.Text.Trim()
            End If
        End If
        Return a
    End Function

    Public Sub SaveAcresAfterListSave()
        If MyAcresObject IsNot Nothing Then
            MyAcresObject.Acreage = AcresText()
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.MyAcresObject IsNot Nothing AndAlso Me.MyAcresObject.LocationAcreageTypeId <> "4" Then
            MyAcresObject.Acreage = Me.txtAcres.Text.Trim()
            MyAcresObject.Section = Me.txtSection.Text.Trim()
            MyAcresObject.Range = Me.txtRange.Text.Trim()
            MyAcresObject.Twp = Me.txtTownshipNum.Text.Trim()
            'Added 11/13/2019 for bug 41224 MLW - if 0, will not trigger validation, but quote rate returns 0 when township not selected before quote rate
            If hdnTownshipName.Value = "0" Then
                hdnTownshipName.Value = ""
            End If
            MyAcresObject.TownshipCodeTypeId = hdnTownshipName.Value
            MyAcresObject.Description = Me.txtDescription.Text.Trim()
            MyAcresObject.County = Me.txtCounty.Text.Trim()
            If String.IsNullOrWhiteSpace(MyAcresObject.LocationAcreageTypeId) OrElse IsQuoteEndorsement() Then
                MyAcresObject.LocationAcreageTypeId = "3" ' only change if it was nothing
            End If
            'Updated 10/24/2019 for bug 41036 MLW
            If MyAcresObject.LocationAcreageTypeId = "3" Then
                MyAcresObject.StateId = ddStateAbbrev.SelectedValue
            Else
                MyAcresObject.StateId = Me.MyLocation.Address.StateId
            End If

            Me.SaveChildControls()
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        'Me.ValidationHelper.GroupName = String.Format("Location #{0} - Acreage", MyLocationIndex + 1)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Dim valItems = IFM.VR.Validation.ObjectValidation.FarmLines.AcreageOnlyValidator.ValidateAcreages(Me.Quote, MyLocationIndex, Me.MyAcresIndex, Me.DefaultValidationType)
        If valItems.Any() Then
            For Each v In valItems
                Select Case v.FieldId
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.AcreageOnlyValidator.Acres
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtAcres, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.AcreageOnlyValidator.Section
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSection, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.AcreageOnlyValidator.TownShipNum
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtTownshipNum, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.AcreageOnlyValidator.Range
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtRange, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.AcreageOnlyValidator.County
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCounty, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.AcreageOnlyValidator.Description
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.AcreageOnlyValidator.TownshipName
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddTownshipName, v, accordList)
                End Select
            Next
        End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Me.Save_FireSaveEvent(False)
        Me.MyLocation.Acreages.RemoveAt(Me.MyAcresIndex)
        Me.ParentVrControl.Populate()
        Me.Save_FireSaveEvent(False)
    End Sub

End Class