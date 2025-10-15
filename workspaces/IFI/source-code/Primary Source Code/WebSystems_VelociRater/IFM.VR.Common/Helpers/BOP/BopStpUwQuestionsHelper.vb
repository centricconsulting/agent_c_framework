Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports Diamond.Business.ThirdParty.ChoicePoint.ThirdParty

Namespace IFM.VR.Common.Helpers
    Public Class BopStpUwQuestionsHelper
        Inherits FeatureFlagBase

        Private Shared _BopStpUwQuestionsSettings As NewFlagItem
        Const NewCoBopCalendarDateKey As String = "VR_NewCo_BOP_CalendarDate"
        Public Shared ReadOnly Property BopStpUwQuestionsSettings() As NewFlagItem
            Get
                If _BopStpUwQuestionsSettings Is Nothing Then
                    _BopStpUwQuestionsSettings = New NewFlagItem("VR_BOP_BopStpUwQuestions_Settings")
                End If
                Return _BopStpUwQuestionsSettings
            End Get
        End Property

        Const BopStpUwQuestionsWarningMsg As String = ""
        Const BopStpUwQuestionsRemovedMsg As String = ""

        Public Shared Sub UpdateBopStpUwQuestions(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Dim qqHelper = New QuickQuoteHelperClass
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Messages
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Messages
                    Dim subQuotes As List(Of QuickQuoteObject) = qqHelper.MultiStateQuickQuoteObjects(Quote)
                    If subQuotes.IsLoaded Then
                        subQuotes.Item(0)?.PolicyUnderwritings?.RemoveAll(Function(x) x.PolicyUnderwritingCodeId = "9607")
                    End If
            End Select
        End Sub

        Public Shared Function IsBopStpUwQuestionsAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then

                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP

                BopStpUwQuestionsSettings.OtherQualifiers = IsCorrectLOB _
                    AndAlso DoesQuoteQualifyByCalendarDate(quote, NewCoBopCalendarDateKey)

                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, BopStpUwQuestionsSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function

    End Class
End Namespace