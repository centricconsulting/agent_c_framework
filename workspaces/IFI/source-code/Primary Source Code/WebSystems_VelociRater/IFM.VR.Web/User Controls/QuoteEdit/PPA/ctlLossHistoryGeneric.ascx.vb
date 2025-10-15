Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Public Class ctlLossHistoryGeneric
    Inherits VRControlBase

    'added 6/18/2020
    Public ReadOnly Property ShouldControlBeUsed As Boolean
        Get
            If IsOnAppPage = False AndAlso Me.Quote IsNot Nothing AndAlso Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso QuickQuoteHelperClass.PPA_CheckDictionaryKeyToOrderClueAtQuoteRate() = True AndAlso Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Overrides Sub LoadStaticData()
        'nothing to do here
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            If ShouldControlBeUsed = True Then
                Dim keyIsPresent As Boolean = False
                Me.cbOrderClueAtRate.Checked = QuickQuoteHelperClass.QuoteIsOkayToOrderClueAtQuoteRate(Me.Quote, keyIsPresent:=keyIsPresent)
                If Me.cbOrderClueAtRate.Checked = False AndAlso keyIsPresent = False Then
                    Dim qqLossHists As List(Of QuickQuoteLossHistoryRecord) = QQHelper.QuickQuoteLossHistoriesFromAllLevels(Me.Quote)
                    If qqLossHists IsNot Nothing AndAlso qqLossHists.Count > 0 Then
                        For Each lh As QuickQuoteLossHistoryRecord In qqLossHists
                            If lh IsNot Nothing AndAlso QQHelper.IsValidDateString(lh.LossDate, mustBeGreaterThanDefaultDate:=True) = True Then
                                If CDate(lh.LossDate) >= DateAdd(DateInterval.Year, -3, Date.Today) Then 'note: may want to use quoteEffDate instead of current date
                                    Me.cbOrderClueAtRate.Checked = True
                                    Exit For
                                End If
                            End If
                        Next
                    End If
                End If
            End If
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
        'nothing to do here
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If ShouldControlBeUsed = True Then
            Me.VRScript.CreateAccordion(Me.divLossHistoryGenericParent.ClientID, Me.accordActive, "0")
            Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            'not sure if these are needed
            Me.MainAccordionDivId = Me.divLossHistoryGenericParent.ClientID
            'Me.ListAccordionDivId = Me.divLossHistoryGenericChild.ClientID
        End If
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Quote IsNot Nothing Then
            If ShouldControlBeUsed = True Then
                QuickQuoteHelperClass.Set_QuoteIsOkayToOrderClueAtQuoteRate(Me.Quote, Me.cbOrderClueAtRate.Checked)
            End If
        End If

        Return True
    End Function
End Class