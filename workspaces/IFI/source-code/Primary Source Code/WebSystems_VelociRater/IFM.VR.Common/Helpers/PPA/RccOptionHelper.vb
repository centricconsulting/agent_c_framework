Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Web.UI.WebControls
Imports Diamond.C0044.Common.Library.Enumerations

Namespace IFM.VR.Common.Helpers.PPA
    Public Class RccOptionHelper
        Inherits FeatureFlagBase
        'Added 8/8/2022 for task 76030 MLW
        Const RccOptionWarningMsg As String = "Not Used"
        Const RccRemovalKey As String = "RemoveRCCBillingPayPlanCalendarDateKey"
        Public Shared Function RccOptionEnabled() As Boolean
            Dim RccOptionSettings As NewFlagItem = New NewFlagItem("VR_PPA_AddRccOption")
            Return RccOptionSettings.EnabledFlag
        End Function

        Public Shared Function RccOptionEffDate() As Date
            Dim RccOptionSettings As NewFlagItem = New NewFlagItem("VR_PPA_AddRccOption")
            Return RccOptionSettings.GetStartDateOrDefault("1/1/2022")
        End Function

        Public Shared Sub UpdateRccOption(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Work Here                 
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Dim NeedsWarningMessage As Boolean = False
                    'Dim MyLocation As QuickQuoteLocation = Quote.Locations(0)

                    'If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                    '    Dim i = New WebValidationItem(RccOptionWarningMsg)
                    '    i.IsWarning = True
                    '    ValidationErrors.Add(i)
                    'End If
            End Select
        End Sub

        Public Shared Function IsShowRCCBillingPayPlanDate() As Boolean
            Dim RccBillingPayPlan As Boolean = False
            Dim qqHelper As New QuickQuoteHelperClass
            If qqHelper.IsValidDateString(GetConfigStringForKey(RccRemovalKey), mustBeGreaterThanDefaultDate:=True) = False OrElse Date.Today < CalendarEffectiveDate(RccRemovalKey) Then
                RccBillingPayPlan = True
            End If
            Return RccBillingPayPlan
        End Function

        Public Shared Function ShowRccBillingPayPlanDateEndorsement(quote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            Dim RccBillingPayPlanEndo As Boolean = False
            If quote IsNot Nothing Then
                If quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso quote.BillingPayPlanId = "18" Then
                    RccBillingPayPlanEndo = True
                End If
            End If
            Return RccBillingPayPlanEndo
        End Function

        Public Shared Function IsRccOptionAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then 
                Dim RccOptionSettings As NewFlagItem = New NewFlagItem("VR_PPA_AddRccOption")
                Dim isCommercial As Boolean = CommercialQuoteHelper.IsCommercialLob(Quote.LobType)
                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                Dim IsCorrectLOBComm As Boolean = isCommercial
                Dim ShowRccForPPA As Boolean = IsCorrectLOB AndAlso IsShowRCCBillingPayPlanDate()
                Dim ShowRccForComm As Boolean = IsCorrectLOBComm AndAlso IsShowRCCBillingPayPlanDate()
                Dim RccQualifiedByPayPlan As Boolean = quote.CurrentPayplanId = "18" 'AndAlso DoesQuoteQualifyByCreatedDate(quote, RccRemovalKey)
                'Dim RccQualifiedByStartDate As Boolean = quote.BillingPayPlanId = "18" AndAlso DoesQuoteQualifyByCreatedDate(quote, RccRemovalKey)
                Dim RccQualifyForEndorsementDate As Boolean = ShowRccBillingPayPlanDateEndorsement(quote) AndAlso RccQualifiedByPayPlan

                If quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    RccOptionSettings.OtherQualifiers = RccQualifyForEndorsementDate OrElse ShowRccForPPA OrElse ShowRccForComm
                ElseIf IsCorrectLOB Then
                    RccOptionSettings.OtherQualifiers = ShowRccForPPA OrElse RccQualifiedByPayPlan
                ElseIf IsCorrectLOBComm Then
                    RccOptionSettings.OtherQualifiers = ShowRccForComm OrElse RccQualifiedByPayPlan
                End If
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, RccOptionSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
