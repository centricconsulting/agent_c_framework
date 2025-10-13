Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
Imports IFM.PrimativeExtensions

Public Class ctlAccidentHistoryList
    Inherits VRControlBase

    Public Property DriverIndex As Int32
        Get
            Return ViewState.GetInt32("vs_driverNum", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_driverNum") = value
        End Set
    End Property

    'added 6/18/2020
    Public ReadOnly Property ShouldControlBeUsed As Boolean
        Get
            If IsOnAppPage = False AndAlso Me.Quote IsNot Nothing AndAlso Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso QuickQuoteHelperClass.PPA_CheckDictionaryKeyToOrderClueAtQuoteRate() = True AndAlso Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = divAccidetHistoryList.ClientID
        Me.ListAccordionDivId = divLossHistories.ClientID
        If Not IsPostBack Then
            If IsOnAppPage Then
                Select Case Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        Exit Select
                    Case Else
                        'If Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                        Me.HiddenFieldMainAccord.Value = "false"
                        'End If
                        Exit Select
                End Select
            End If
        End If
        AttachLossHistoryControlEvents()

        If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                Me.lnkAdd.Attributes.Add("style", "display:'';")
                Me.lnkSave.Attributes.Add("style", "display:'';")
            Else
                Me.lnkAdd.Attributes.Add("style", "display: none;")
                Me.lnkSave.Attributes.Add("style", "display: none;")
                Me.divAccidetHistoryList.Attributes.Add("title", "No changes to Loss History permitted.")
            End If
        Else
            Me.divHOMClueLink.Visible = False
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'added IF 6/18/2020
        If ShouldControlBeUsed = True Then
            Me.VRScript.StopEventPropagation(Me.lnkAdd.ClientID)

            Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)

            If Me.Quote IsNot Nothing Then

                Select Case Me.Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                        'updated 8/15/2018 from Quote.Drivers to use GoverningStateQuote; note: could set variable so it doesn't have to look it up every time
                        If Me.GoverningStateQuote.Drivers IsNot Nothing Then
                            If Me.GetData() IsNot Nothing Then
                                Me.VRScript.CreateAccordion(MainAccordionDivId, HiddenFieldMainAccord, "0")
                                Me.VRScript.CreateAccordion(ListAccordionDivId, HiddenField1, "0")
                            Else
                                Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Nothing, Nothing, True)
                            End If
                            divHOMClueLink.Visible = False
                        End If
                        Exit Select
                    Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                        If IsOnAppPage Then
                            Dim homLosses = GetData()
                            ' have the Clue LINK so this will always be expandable on the app side
                            Me.VRScript.CreateAccordion(MainAccordionDivId, HiddenFieldMainAccord, "0")
                            Me.VRScript.CreateAccordion(ListAccordionDivId, HiddenField1, "0")
                            Me.VRScript.AddScriptLine(String.Format("HOMLossListClueReportLink('{1}','{0}'," + If(homLosses IsNot Nothing AndAlso homLosses.Any(), "true", "false") + "); ", Me.divLossHistories.ClientID, divHOMClueLink.ClientID), True)
                        Else

                            If GetData() IsNot Nothing Then
                                Me.VRScript.CreateAccordion(MainAccordionDivId, HiddenFieldMainAccord, "0")
                                Me.VRScript.CreateAccordion(ListAccordionDivId, HiddenField1, "0")
                            Else
                                Me.VRScript.CreateAccordion(MainAccordionDivId, Nothing, Nothing, True)
                            End If
                        End If
                        Exit Select
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        If GetData() IsNot Nothing Then
                            Me.VRScript.CreateAccordion(MainAccordionDivId, HiddenFieldMainAccord, "0")
                            Me.VRScript.CreateAccordion(ListAccordionDivId, HiddenField1, "0")
                        Else
                            Me.VRScript.CreateAccordion(MainAccordionDivId, Nothing, Nothing, True)
                        End If

                        Exit Select
                End Select

            End If
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Private Function GetData() As List(Of QuickQuoteLossHistoryRecord)
        Dim data As List(Of QuickQuoteLossHistoryRecord) = Nothing
        If Me.Quote IsNot Nothing Then

            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    'updated 8/15/2018 from Quote.Drivers to use GoverningStateQuote; note: could set variable so it doesn't have to look it up every time
                    If Me.GoverningStateQuote.Drivers IsNot Nothing Then
                        Dim driver As QuickQuoteDriver = GoverningStateQuote.Drivers.GetItemAtIndex(Me.DriverIndex)

                        If driver IsNot Nothing AndAlso driver.LossHistoryRecords.IsLoaded Then
                            data = driver.LossHistoryRecords
                        End If
                    End If
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    data = IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetAllHOMLosses(Me.Quote)
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    data = GetAllCommercialLosses()
                    Exit Select
            End Select
        End If
        Return data
    End Function

    Public Function GetAllCommercialLosses() As List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord)
        If Quote IsNot Nothing Then
            'If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
            Dim lossList As New List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord)()
            'updated 8/15/2018 from Quote to GoverningStateQuote; note: could set variable so it doesn't have to look it up every time
            If GoverningStateQuote.LossHistoryRecords IsNot Nothing Then
                lossList.AddRange(GoverningStateQuote.LossHistoryRecords)
            End If

            Return If(lossList.Any(), lossList, Nothing)
            'End If
        End If
        Return Nothing
    End Function


    Public Overrides Sub Populate()
        'added IF 6/18/2020
        If ShouldControlBeUsed = True Then
            Dim data = GetData()

            Me.Repeater1.DataSource = data
            Me.Repeater1.DataBind()
            Me.FindChildVrControls() ' finds the just added controls do to the binding
            Dim index As Int32 = 0
            For Each child In Me.GatherChildrenOfType(Of ctlAccidentHistoryItem)
                child.AccidentIndex = index
                child.DriverIndex = Me.DriverIndex
                child.Populate()
                index += 1
            Next

            If Me.Quote IsNot Nothing Then
                Select Case Me.Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                        Me.lnkClueReport.Visible = False
                        'updated 8/15/2018 from Quote.Drivers to use GoverningStateQuote
                        If Me.GoverningStateQuote.Drivers IsNot Nothing Then
                            Me.divLossHistories.Visible = data IsNot Nothing
                            If data IsNot Nothing Then
                                Me.lblHeader.Text = String.Format("Loss History ({0})", data.Count().ToString())
                            Else
                                Me.lblHeader.Text = "Loss History"
                            End If
                        Else
                            Me.divLossHistories.Visible = False
                        End If
                    Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                        Me.lnkClueReport.Visible = False
                        If IsOnAppPage Then
                            Me.lnkClueReport.Visible = True
                            Me.divLossHistories.Visible = True
                        Else
                            Me.divLossHistories.Visible = If(data IsNot Nothing, data.Any(), False)
                        End If
                        Me.lblHeader.Text = String.Format("Loss History ({0})", data.CountEvenIfNull)
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        Me.lnkClueReport.Visible = False
                        Me.divLossHistories.Visible = True
                        Me.lblHeader.Text = String.Format("Loss History ({0})", data.CountEvenIfNull)
                End Select
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'added IF 6/18/2020
        If ShouldControlBeUsed = True Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = "Loss History"
            Dim processingStatusCode As String = Nothing

            Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    For Each v In LossListValidator.ValidateLossList(Me.DriverIndex, Nothing, Me.Quote, Me.DefaultValidationType)
                        Select Case v.FieldId
                            Case LossListValidator.LossDateDuplicatedInList
                                Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                        End Select
                    Next
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    For Each v In LossListValidator.ValidateLossList(Me.DriverIndex, Nothing, Me.Quote, Me.DefaultValidationType)
                        Select Case v.FieldId
                            Case LossListValidator.LossDateDuplicatedInList
                                Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                        End Select
                    Next
                    Exit Select
            End Select

            Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Me.ValidateChildControls(valArgs)
                    Exit Select
                Case Else
                    If Not IsOnAppPage Then
                        Me.ValidateChildControls(valArgs)
                    End If
                    Exit Select
            End Select
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        'added IF 6/18/2020
        If ShouldControlBeUsed = True Then
            For Each child In Me.ChildVrControls
                child.Save()
                child.Populate()
            Next
        End If
        Return True
    End Function

    Private Sub AttachLossHistoryControlEvents()
        ' need to wire up the remove events for each control every single time
        For Each child In Me.GatherChildrenOfType(Of ctlAccidentHistoryItem)
            AddHandler child.ItemRemoveRequest, AddressOf ItemRemoveRequest
        Next
    End Sub

    Private Sub ItemRemoveRequest(itemIndex As Int32)
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                Me.Populate()
                Me.Save_FireSaveEvent(False)
                Me.LockTree()
            Case Else
                If Me.IsOnAppPage = False Then
                    Me.Populate()
                    Me.Save_FireSaveEvent(False)
                    Me.LockTree()
                End If
        End Select
    End Sub

    Protected Sub lnkAdd_Click(sender As Object, e As EventArgs) Handles lnkAdd.Click
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                AddNewLossItem()
                Exit Select
            Case Else
                If Not IsOnAppPage Then
                    AddNewLossItem()
                End If
                Exit Select
        End Select
    End Sub

    Public Sub AddNewLossItem()
        If Quote IsNot Nothing Then
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    If Not IsOnAppPage Then
                        'updated 8/15/2018 from Quote.Drivers to use GoverningStateQuote
                        If Me.GoverningStateQuote.Drivers IsNot Nothing Then

                            Dim MyDriver = Me.GoverningStateQuote.Drivers.GetItemAtIndex(Me.DriverIndex)

                            If MyDriver.IsNotNull Then
                                MyDriver.LossHistoryRecords.AddNew()
                                Me.Save_FireSaveEvent(False)

                                Me.LockTree()
                                Me.Populate()

                                Try
                                    Me.HiddenField1.Value = CInt(MyDriver.LossHistoryRecords.Count() - 1).ToString()
                                    Me.VRScript.AddScriptLine("$(""#" + Me.divLossHistories.ClientID + """).accordion({heightStyle: ""content"", active: " + Me.HiddenField1.Value + ", collapsible: true, activate: function(event, ui) { $(""#" + Me.HiddenField1.ClientID + """).val($(""#" + Me.divLossHistories.ClientID + """).accordion('option','active'));    } });")
                                Catch ex As Exception

                                End Try
                            End If

                        End If

                    End If
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    If Not IsOnAppPage Then
                        ' Note: If you have been to app and back it is possible that when adding a new record it could appear to be adding it in the middle of the list
                        ' THis would happen if there where existing policy level and applicant level losses
                        ' Since I add to policy level it might appear that I added the new loss in the middle of the list because we combine both loss lists into one
                        'updated 8/15/2018 to use GoverningStateQuote instead of Quote
                        Me.GoverningStateQuote.LossHistoryRecords.AddNew()

                        Me.Save_FireSaveEvent(False)

                        Me.LockTree()
                        Me.Populate()

                        Try
                            Me.HiddenField1.Value = CInt(Me.GoverningStateQuote.LossHistoryRecords.Count() - 1).ToString()
                            Me.VRScript.AddScriptLine("$(""#" + Me.divLossHistories.ClientID + """).accordion({heightStyle: ""content"", active: " + Me.HiddenField1.Value + ", collapsible: true, activate: function(event, ui) { $(""#" + Me.HiddenField1.ClientID + """).val($(""#" + Me.divLossHistories.ClientID + """).accordion('option','active'));    } });")
                        Catch ex As Exception

                        End Try

                    End If
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    ' Note: If you have been to app and back it is possible that when adding a new record it could appear to be adding it in the middle of the list
                    ' This would happen if there where existing policy level and applicant level losses
                    ' Since I add to policy level it might appear that I added the new loss in the middle of the list because we combine both loss lists into one
                    'updated 8/15/2018 to use GoverningStateQuote instead of Quote
                    Me.GoverningStateQuote.LossHistoryRecords.AddNew()

                    Me.Save_FireSaveEvent(False)

                    Me.LockTree()
                    Me.Populate()

                    Try
                        Me.HiddenField1.Value = CInt(Me.GoverningStateQuote.LossHistoryRecords.Count() - 1).ToString()
                        Me.VRScript.AddScriptLine("$(""#" + Me.divLossHistories.ClientID + """).accordion({heightStyle: ""content"", active: " + Me.HiddenField1.Value + ", collapsible: true, activate: function(event, ui) { $(""#" + Me.HiddenField1.ClientID + """).val($(""#" + Me.divLossHistories.ClientID + """).accordion('option','active'));    } });")
                    Catch ex As Exception

                    End Try

                    Exit Select
            End Select
        End If
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                Me.Save_FireSaveEvent(True)
                Exit Select
            Case Else
                If IsOnAppPage = False Then
                    Me.Save_FireSaveEvent(True)
                End If
                Exit Select
        End Select
    End Sub

    Protected Sub lnkClueReport_Click(sender As Object, e As EventArgs) Handles lnkClueReport.Click

        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.PERSONAL_HOME_GetCLUEReport(Me.Quote, Err, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("CLUE_{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
            Response.BinaryWrite(ReportFile)
        Else
            Me.VRScript.AddScriptLine("alert('No Loss Records Found.');")
        End If
    End Sub

End Class