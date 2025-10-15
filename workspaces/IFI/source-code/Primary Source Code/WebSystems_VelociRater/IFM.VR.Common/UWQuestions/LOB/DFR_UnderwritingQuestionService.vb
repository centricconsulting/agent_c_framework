Imports IFM.VR.Common.Underwriting
Imports Microsoft.Extensions.Caching.Memory
Imports Microsoft.Extensions.FileProviders
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Interop
Imports UnderwritingQuestion = IFM.VR.Common.UWQuestions.VRUWQuestion

Namespace IFM.VR.Common.Underwriting.LOB
    Public Class DFR_UnderwritingQuestionService
        Inherits UnderwritingQuestionsService
        Implements IUnderwritingQuestionsService
        Public Sub New(memoryCache As IMemoryCache,
                       fileProvider As IFileProvider)
            MyBase.New(memoryCache, fileProvider)
            setupCustomFilters()
        End Sub
        Public Sub New(memoryCache As IMemoryCache,
                       fileProvider As IFileProvider,
                       qqHelper As QuickQuoteHelperClass)
            MyBase.New(memoryCache, fileProvider, qqHelper)
            setupCustomFilters()
        End Sub

        Private Const HIDE_ON_EXCLUDE = True
        Private Const OCCUPANCY_CODE_OWNER_OCCUPIED = "14"
        Private QUESTION_NUMBERS_TO_HIDE_ON_OWNER_OCCUPIED As Integer() = {15, 16, 17}

        Private Sub setupCustomFilters()
            Me.CustomFilters.Add(Function(r As UnderwritingQuestionRequest, q As UnderwritingQuestion)
                                     Dim include = True

                                     If r.Quote.Locations.First().OccupancyCodeId = OCCUPANCY_CODE_OWNER_OCCUPIED AndAlso
                                        QUESTION_NUMBERS_TO_HIDE_ON_OWNER_OCCUPIED.Contains(q.QuestionNumber) Then
                                         include = False
                                     End If

                                     Return (include, HIDE_ON_EXCLUDE)
                                 End Function)
        End Sub
        Protected Overrides Function SaveAnswerToQuote(answer As QuickQuotePolicyUnderwriting, quote As QuickQuoteObject) As Boolean
            Dim retval = False

            If quote.Locations.Any() Then
                For Each qLoc In quote.Locations
                    If qLoc.PolicyUnderwritings Is Nothing Then
                        qLoc.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                    Else
                        Dim puwIndex = qLoc.PolicyUnderwritings.FindIndex(Function(puw) puw.PolicyUnderwritingCodeId = answer.PolicyUnderwritingCodeId)
                        If puwIndex > -1 Then
                            qLoc.PolicyUnderwritings.RemoveAt(puwIndex)
                        End If
                    End If
                    qLoc.PolicyUnderwritings.Add(answer)
                Next
                retval = True
            End If
            Return retval
        End Function

        Protected Overrides Function RetrieveAnswerValueFromQuote(policyUnderwritingCodeId As String, quote As QuickQuoteObject) As QuickQuotePolicyUnderwriting
            Dim retval As QuickQuotePolicyUnderwriting = Nothing

            If quote.Locations?.FirstOrDefault()?.PolicyUnderwritings?.Any() Then
                retval = quote.Locations.First().PolicyUnderwritings.FirstOrDefault(Function(puw) puw.PolicyUnderwritingCodeId = policyUnderwritingCodeId)
            End If

            Return retval
        End Function

        Protected Overrides Function DefaultAnswerPrototype() As QuickQuotePolicyUnderwriting
            Dim retval = MyBase.DefaultAnswerPrototype()

            retval.PolicyUnderwritingLevelId = "3"
            Return retval
        End Function
    End Class
End Namespace
