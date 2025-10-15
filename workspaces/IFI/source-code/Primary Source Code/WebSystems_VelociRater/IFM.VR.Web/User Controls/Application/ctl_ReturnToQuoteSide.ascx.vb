Public Class ctl_ReturnToQuoteSide
    Inherits VRControlBase

    Public ReadOnly Property ReturnToQuoteEvent() As String
        Get
            Return "ReturnToQuoteEvent" + "_" + Quote.PolicyId + "|" + Quote.PolicyImageNum
        End Get
    End Property

    Public Property ReturnToQuoteSession() As Boolean
        Get
            Dim ReturnToQuoteEventBool As Boolean
            If Session IsNot Nothing AndAlso Session(ReturnToQuoteEvent) IsNot Nothing Then
                If Boolean.TryParse(Session(ReturnToQuoteEvent).ToString, ReturnToQuoteEventBool) = False Then
                    ReturnToQuoteEventBool = False
                End If
            End If
            Return ReturnToQuoteEventBool
        End Get
        Set(ByVal value As Boolean)
            If Session IsNot Nothing AndAlso Session(ReturnToQuoteEvent) IsNot Nothing Then
                Session(ReturnToQuoteEvent) = value
            Else
                Session.Add(ReturnToQuoteEvent, value)
            End If
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.IsPostBack Then
            Dim eventTarget As String
            Dim eventArgument As String

            If Me.Request("__EVENTTARGET") Is Nothing Then
                eventTarget = String.Empty
            Else
                eventTarget = Me.Request("__EVENTTARGET")
            End If

            If Me.Request("__EVENTARGUMENT") Is Nothing Then
                eventArgument = String.Empty
            Else
                eventArgument = Me.Request("__EVENTARGUMENT")
            End If

            If eventTarget = ReturnToQuoteEvent Then
                Dim valuePassed As String = eventArgument
                CleanUpPartialAIs(valuePassed)
            End If
        End If

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
    End Sub

    Private Sub btnReturnToQuote_Click(sender As Object, e As EventArgs) Handles btnReturnToQuote.Click
        Dim ItemsWithFakeAIs As List(Of Integer) = New List(Of Integer)
        Dim ItemsWithFakeAIsString As String = String.Empty

        ItemsWithFakeAIs = IFM.VR.Common.Helpers.AdditionalInterest.GetFakeAIItemList(Me.Quote)
        ItemsWithFakeAIsString = String.Join(",", ItemsWithFakeAIs.ToArray)

        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

        If Me.Quote IsNot Nothing Then
            Select Case Me.Quote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    ' if partial AI then show prompt via script variable
                    If Common.Helpers.AdditionalInterest.HasIncompleteAi(Me.Quote) Then
                        'Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") + "?quoteid=" + QuoteId.ToString()
                        'Me.VRScript.AddScriptLine("var ai_return_to_Quote_proceed = confirm('Any incomplete additional interests will be removed. If all AIs are removed then Loan/Lease will be removed from the quote and unchecked. Continue to quote?'); if (ai_return_to_Quote_proceed) {DisableFormOnSaveRemoves(); window.location = '" + quotePath + "';} ")
                        Me.VRScript.AddScriptLine("var ai_return_to_Quote_proceed = confirm('Any incomplete additional interests will be removed. If all AIs are removed then Loan/Lease will be removed from the quote and unchecked. Continue to quote?'); if (ai_return_to_Quote_proceed) {DisableFormOnSaveRemoves(); __doPostBack('" + ReturnToQuoteEvent + "', '" + ItemsWithFakeAIsString + "');} ")
                        Return
                    Else
                        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") + "?quoteid=" + QuoteId.ToString())
                    End If
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    ' if BOP partial AI then show prompt
                    If Common.Helpers.AdditionalInterest.HasIncompleteAi(Me.Quote) Then
                        Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_BOP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                        Me.VRScript.AddScriptLine("var ai_return_to_Quote_proceed = confirm('Any incomplete additional interests will be removed. Continue to Quote?'); if (ai_return_to_Quote_proceed) {DisableFormOnSaveRemoves(); window.location = '" + quotePath + "';} ")
                        Return
                    Else
                        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_BOP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString())
                    End If
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    ' if CAP partial AI then show prompt
                    If Common.Helpers.AdditionalInterest.HasIncompleteAi(Me.Quote) Then
                        Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CAP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                        Me.VRScript.AddScriptLine("var ai_return_to_Quote_proceed = confirm('Any incomplete additional interests will be removed. Continue to Quote?'); if (ai_return_to_Quote_proceed) {DisableFormOnSaveRemoves(); window.location = '" + quotePath + "';} ")
                        Return
                    Else
                        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CAP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString())
                    End If
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    'Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_Input") + "?quoteid=" + QuoteId.ToString())
                    If Common.Helpers.AdditionalInterest.HasIncompleteAi(Me.Quote) Then
                        'Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") + "?quoteid=" + QuoteId.ToString()
                        'Me.VRScript.AddScriptLine("var ai_return_to_Quote_proceed = confirm('Any incomplete additional interests will be removed. If all AIs are removed then Loan/Lease will be removed from the quote and unchecked. Continue to quote?'); if (ai_return_to_Quote_proceed) {DisableFormOnSaveRemoves(); window.location = '" + quotePath + "';} ")
                        Me.VRScript.AddScriptLine("var ai_return_to_Quote_proceed = confirm('Any incomplete additional interests will be removed. If all AIs are removed then Loan/Lease will be removed from the quote and unchecked. Continue to quote?'); if (ai_return_to_Quote_proceed) {DisableFormOnSaveRemoves(); __doPostBack('" + ReturnToQuoteEvent + "', '" + ItemsWithFakeAIsString + "');} ")
                        Return
                    Else
                        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_Input") + "?quoteid=" + QuoteId.ToString())
                    End If
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
                    Dim HasIncompleteResidentNames As Boolean = IFM.VR.Common.Helpers.FARM.ResidentNameHelper.HasIncompleteResidentNames(Me.Quote)
                    Dim HasIncompleteAcreageOnlyRecords As Boolean = IFM.VR.Common.Helpers.FARM.AcresOnlyHelper.HasIncompleteAcreageOnlyRecords(Me.Quote)

                    If HasIncompleteResidentNames Or HasIncompleteAcreageOnlyRecords Then

                        If HasIncompleteResidentNames And HasIncompleteAcreageOnlyRecords Then
                            Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_Input") + "?quoteid=" + QuoteId.ToString()
                            Me.VRScript.AddScriptLine("var residentNames_return_to_Quote_proceed = confirm('Any incomplete Family Medical Payment Names and Acreage Only records will be removed. Continue to Quote?'); if (residentNames_return_to_Quote_proceed) {DisableFormOnSaveRemoves(); window.location = '" + quotePath + "';} ", True)
                            Return
                        Else
                            'going to be one or the other not both
                            If HasIncompleteResidentNames Then
                                Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_Input") + "?quoteid=" + QuoteId.ToString()
                                Me.VRScript.AddScriptLine("var residentNames_return_to_Quote_proceed = confirm('Any incomplete Family Medical Payment Names will be removed. Continue to Quote?'); if (residentNames_return_to_Quote_proceed) {DisableFormOnSaveRemoves(); window.location = '" + quotePath + "';} ", True)
                                Return
                            End If
                            If HasIncompleteAcreageOnlyRecords Then
                                Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_Input") + "?quoteid=" + QuoteId.ToString()
                                Me.VRScript.AddScriptLine("var residentNames_return_to_Quote_proceed = confirm('Any incomplete Acreage Only records will be removed. Continue to Quote?'); if (residentNames_return_to_Quote_proceed) {DisableFormOnSaveRemoves(); window.location = '" + quotePath + "';} ", True)
                                Return
                            End If
                        End If

                    Else
                        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_Input") + "?quoteid=" + QuoteId.ToString())
                    End If
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    ' if partial AI then show prompt via script variable
                    If Common.Helpers.AdditionalInterest.HasIncompleteAi(Me.Quote) Then
                        Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_Input") + "?quoteid=" + QuoteId.ToString()
                        Me.VRScript.AddScriptLine("var ai_return_to_Quote_proceed = confirm('Any incomplete additional interests will be removed. Continue to Quote?'); if (ai_return_to_Quote_proceed) {DisableFormOnSaveRemoves(); window.location = '" + quotePath + "';} ")
                        Return
                    Else
                        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_Input") + "?quoteid=" + QuoteId.ToString())
                    End If
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability 'CGL
                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CGL_Quote_NewLook") + "?quoteid=" + QuoteId.ToString())
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP 'BOP
                    ' If there are any incomplete loss histories, delete them or we'll get an error on the quote side when we try and re-rate
                    RemoveAnyIncompleteLossHistoryRecords()
                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_BOP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString())
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto ' CAP
                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CAP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString())
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation ' WCP
                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_WCP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString())
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty ' CPR
                    ' if CPR partial AI then show prompt
                    If Common.Helpers.AdditionalInterest.HasIncompleteAi(Me.Quote) Then
                        Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPR_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                        Me.VRScript.AddScriptLine("var ai_return_to_Quote_proceed = confirm('Any incomplete additional interests will be removed. Continue to Quote?'); if (ai_return_to_Quote_proceed) {DisableFormOnSaveRemoves(); window.location = '" + quotePath + "';} ")
                        Return
                    Else
                        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPR_Quote_NewLook") + "?quoteid=" + QuoteId.ToString())
                    End If
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    ' if CPP partial AI then show prompt
                    If Common.Helpers.AdditionalInterest.HasIncompleteAi(Me.Quote) Then
                        Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                        Me.VRScript.AddScriptLine("var ai_return_to_Quote_proceed = confirm('Any incomplete additional interests will be removed. Continue to Quote?'); if (ai_return_to_Quote_proceed) {DisableFormOnSaveRemoves(); window.location = '" + quotePath + "';} ")
                        Return
                    Else
                        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString())
                    End If
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal 'FUP/PUP
                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FUPPUP_Input") + "?quoteid=" + QuoteId.ToString())
                    Exit Select
                Case Else
#If DEBUG Then
                    Debugger.Break() ' you need a case for this LOB
#End If
                    Exit Select
            End Select
        End If
    End Sub

    Private Sub RemoveAnyIncompleteLossHistoryRecords()
        If Quote IsNot Nothing Then
CheckLH:
            'Updated 9/4/18 for multi state MLW
            If Me.GoverningStateQuote IsNot Nothing Then
                'If Quote.LossHistoryRecords IsNot Nothing AndAlso Quote.LossHistoryRecords.Count > 0 Then
                '    Dim deleterec As Boolean = False
                '    Dim ndx As Integer = -1
                '    For Each lhr As QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord In Quote.LossHistoryRecords
                '        ndx += 1
                '        deleterec = False
                '        If Not IsDate(lhr.LossDate) Then deleterec = True
                '        If lhr.TypeOfLossId.Trim = "" Then deleterec = True
                '        If lhr.Amount.Trim = "" Then deleterec = True
                '        If lhr.LossDescription = "" Then deleterec = True
                '        If deleterec Then
                '            Quote.LossHistoryRecords.RemoveAt(ndx)
                '            Save_FireSaveEvent(False)
                '            GoTo CheckLH
                '        End If
                '    Next
                'End If
                If GoverningStateQuote.LossHistoryRecords IsNot Nothing AndAlso GoverningStateQuote.LossHistoryRecords.Count > 0 Then
                    Dim deleterec As Boolean = False
                    Dim ndx As Integer = -1
                    For Each lhr As QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord In GoverningStateQuote.LossHistoryRecords
                        ndx += 1
                        deleterec = False
                        If Not IsDate(lhr.LossDate) Then deleterec = True
                        If lhr.TypeOfLossId.Trim = "" Then deleterec = True
                        If lhr.Amount.Trim = "" Then deleterec = True
                        If lhr.LossDescription = "" Then deleterec = True
                        If deleterec Then
                            GoverningStateQuote.LossHistoryRecords.RemoveAt(ndx)
                            Save_FireSaveEvent(False)
                            GoTo CheckLH
                        End If
                    Next
                End If
            End If
        End If
    End Sub

    Private Sub CleanUpPartialAIs(ItemNumsWithFakeAI As String)
        If Me.Quote IsNot Nothing Then
            Dim quotePath As String = String.Empty
            Dim arrItemNumsWithFakeAI As String() = ItemNumsWithFakeAI?.Split(",")

            Select Case Me.Quote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    quotePath = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") + "?quoteid=" + QuoteId.ToString()
                    ' if partial AI then Remove Them
                    IFM.VR.Common.Helpers.AdditionalInterest.RemoveIncompleteAis_Auto(Me.Quote)
                    ' Restore Fakes if necessary
                    IFM.VR.Common.Helpers.AdditionalInterest.AddFakeAIByItem(Me.Quote, arrItemNumsWithFakeAI)
                    ReturnToQuoteSession = True
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    quotePath = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_Input") + "?quoteid=" + QuoteId.ToString()
                    ' if partial AI then Remove Them
                    IFM.VR.Common.Helpers.AdditionalInterest.RemoveIncompleteAis_HOM(Me.Quote)
                    ' Restore Fakes if necessary
                    IFM.VR.Common.Helpers.AdditionalInterest.AddFakeAIByItem(Me.Quote, arrItemNumsWithFakeAI)
                    ReturnToQuoteSession = True
                    Exit Select
                    'Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    '    ' if BOP partial AI then show prompt
                    '    Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_BOP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                    '    Exit Select
                    'Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    '    ' if CAP partial AI then show prompt
                    '    Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CAP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                    '    Exit Select
                    'Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    '    ' if partial AI then show prompt via script variable
                    '    Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_Input") + "?quoteid=" + QuoteId.ToString()
                    '    Exit Select
                    'Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                    '    ' if CPR partial AI then show prompt
                    '    Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPR_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                    '    Exit Select
                    'Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    '    ' if CPP partial AI then show prompt
                    '    Dim quotePath As String = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                    '    Exit Select
                Case Else
                    Exit Select
            End Select
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

            ReturnToQuoteSession = False

            If String.IsNullOrWhiteSpace(quotePath) = False Then
                Response.Redirect(quotePath)
            End If

        End If
    End Sub

End Class