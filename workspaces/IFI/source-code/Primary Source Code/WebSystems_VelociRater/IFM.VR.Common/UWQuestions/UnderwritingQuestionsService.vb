
Imports System.Configuration
Imports IFM.Configuration.Extensions
Imports Microsoft.Extensions.Caching.Memory

Imports Newtonsoft.Json
Imports UnderwritingQuestion = IFM.VR.Common.UWQuestions.VRUWQuestion
Imports QuestionList = System.Collections.Generic.List(Of IFM.VR.Common.UWQuestions.VRUWQuestion)
Imports CachedList = System.Collections.Immutable.ImmutableList(Of IFM.VR.Common.UWQuestions.VRUWQuestion)
Imports CachedListBuilder = System.Collections.Immutable.ImmutableList(Of IFM.VR.Common.UWQuestions.VRUWQuestion).Builder
Imports AnswerList = System.Collections.Generic.List(Of IFM.VR.Common.UWQuestions.VRUWQuestion)
Imports Answer = IFM.VR.Common.UWQuestions.VRUWQuestion
Imports Microsoft.Extensions.FileProviders
Imports System.IO
Imports Microsoft.Extensions.Primitives
Imports System.Threading
Imports System.Runtime.CompilerServices
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports Diamond.Common.StaticDataManager.Objects.SystemData
Imports System.Collections.Immutable
Imports Diamond.Common.Enums.ThirdParty.Metro
Imports Diamond.Common.Services.Messages.AccountingService.AddAgencyPayments
Imports Diamond.Common.Objects.Policy
Imports Diamond.Common.Objects.ThirdParty.ISOClaimSearch.ACORDClaimsInvestigation

Namespace IFM.VR.Common.Underwriting
    Public Interface IUnderwritingQuestionsService
        Function GetQuestions(request As UnderwritingQuestionRequest) As IEnumerable(Of UnderwritingQuestion)
        Function SaveAnswers(request As UnderwritingSaveRequest) As Boolean
    End Interface

    Public Class UnderwritingQuestionsService
        Implements IUnderwritingQuestionsService

        Private Const VR_UNDERWRITING_QUESTIONS_PATH_KEY = "VR_UnderwritingQuestionsPath"
        Protected Friend Const UWQ_SVC_CACHE_KEY = "UWQ_SVC_ALL"
        Private ReadOnly _cache As IMemoryCache
        Private ReadOnly _fileProvider As IFileProvider
        Private ReadOnly _qqHelper As QuickQuoteHelperClass
        Private ReadOnly _changeHandles As New List(Of CancellationTokenSource)
        Private _uwFilePath As String

        ''' <summary>
        ''' List of custom tests that are used to include or exclude questions
        ''' each test takes a <see cref="UnderwritingQuestionRequest"/> and a <see cref="UnderwritingQuestion"/> as its arguments and returns true to include or false to exclude
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CustomFilters As New List(Of System.Func(Of UnderwritingQuestionRequest, UnderwritingQuestion, (include As Boolean, hideOnExclude As Boolean)))

        Public Sub New(memoryCache As IMemoryCache,
                       fileProvider As IFileProvider)
            _cache = memoryCache
            _fileProvider = fileProvider
            _qqHelper = New QuickQuoteHelperClass()
            init()
        End Sub
        Public Sub New(memoryCache As IMemoryCache,
                       fileProvider As IFileProvider,
                       qqHelper As QuickQuoteHelperClass)
            _cache = memoryCache
            _fileProvider = fileProvider
            _qqHelper = qqHelper
            init()
        End Sub

        Protected Sub init()
            _uwFilePath = ConfigurationManager.AppSettings.Get(VR_UNDERWRITING_QUESTIONS_PATH_KEY)

        End Sub

        Protected Function getCacheOptions() As MemoryCacheEntryOptions
            Dim cacheOptions As New MemoryCacheEntryOptions
            cacheOptions.AddExpirationToken(_fileProvider.Watch(_uwFilePath))

            Return cacheOptions
        End Function

        ''' <summary>
        ''' Retrieves <see cref="UnderwritingQuestion"/> Underwriting Questions from the underlying source based on the submitted <paramref name="request"/>
        ''' </summary>
        ''' <param name="request">
        '''     <list type="table">
        '''         <item>
        '''             <term>LobType</term>
        '''             <description>
        '''             The line of business (LOB) to search for. 
        '''             If None is specified, only questions with LobType set to None will be returned</description>
        '''         </item>
        '''         <item>
        '''             <term>GoverningState</term>
        '''             <description>
        '''             The governing state to search for. 
        '''             Questions with a GoverningState of None are also included in the search, 
        '''             but will only be returned if a question for the QuestionNumber but mathcing the GoverningState of the request does not exist.
        '''             If None is specified, only questions with a GoverningState of None will be returned</description>
        '''         </item>
        '''         <item>
        '''             <term>TypeFilter</term>
        '''             <description>An extra filter on the results. 
        '''                 KillOnly - only return Kill Questions
        '''                 ExcludeUnmapped - will not return questions that are not mapped to Diamond
        '''                 GoverningStateOnly - only return questions that match the supplied GoverningState, do not consider those with GoverningState = None
        '''                 None - (default) no extra filtering
        '''             </description>
        '''         </item>
        '''         <item>
        '''             <term>Quote</term>
        '''             <description>The quote used to pull existing answers from</description>
        '''         </item>
        '''     </list>
        ''' </param>
        ''' <returns></returns>
        Public Function GetQuestions(request As UnderwritingQuestionRequest) As IEnumerable(Of UnderwritingQuestion) Implements IUnderwritingQuestionsService.GetQuestions
            Dim retval As QuestionList = Nothing

            If request Is Nothing Then Throw New ArgumentNullException(NameOf(request))

            If request.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.None Then
                Throw New ArgumentException("LobType cannot be None", NameOf(request))
            End If

            Dim tmpList As CachedList = Nothing
            If Not _cache.TryGetValue(Of CachedList)(request.ToKey(), tmpList) OrElse
               Not tmpList.Any() Then 'check cache for the request object
                'Dim tmpList As CachedList = Nothing

                'not in cache, load it from the source
                If Not _cache.TryGetValue(Of CachedList)($"{UWQ_SVC_CACHE_KEY}", tmpList) OrElse
                   Not tmpList.Any() Then
                    tmpList = loadQuestionsList(True)?.ToImmutableList()
                    _cache.Set(Of CachedList)($"{UWQ_SVC_CACHE_KEY}", tmpList, getCacheOptions())
                    'Else
                    '    tmpList = qList
                End If

                If tmpList?.Any() Then
                    ''only need questions for the specified LOB
                    Dim builder As CachedListBuilder = tmpList.ToBuilder()
                    builder.RemoveAll(Function(q) q.LobType <> request.LobType)

                    'filters
                    If request.TypeFilter.HasFlag(UnderwritingQuestionRequest.QuestionTypeFilter.KillOnly) Then Filter_KillOnly(builder)
                    If request.TypeFilter.HasFlag(UnderwritingQuestionRequest.QuestionTypeFilter.ExcludeUnmapped) Then Filter_ExcludeUnmapped(builder)

                    Filter_GoverningState(builder, request)
                    Filter_MinimumEffectiveDate(builder, request)
                    builder.Sort(Function(q1, q2) q1.QuestionNumber.CompareTo(q2.QuestionNumber))
                    Filter_Subset(builder, request)

                    tmpList = builder.ToImmutable()
                    _cache.Set(Of CachedList)(request.ToKey(), tmpList, getCacheOptions())
                    'retval = tmpList
                End If
                'Else
                '    retval = qList.ToImmutableList.ToList()
            End If
            retval = tmpList?.Select(Of UnderwritingQuestion)(Function(uwq) CType(uwq.Clone(), UnderwritingQuestion))?.ToList()

            If retval IsNot Nothing AndAlso
               request.Quote IsNot Nothing Then
                Filter_Special(retval, request)
                MatchQuestionsToExistingResponses(retval, request.Quote)
            End If

            Return retval
        End Function

        Public Function SaveAnswers(request As UnderwritingSaveRequest) As Boolean Implements IUnderwritingQuestionsService.SaveAnswers
            Dim retval As Boolean = True

            If request Is Nothing OrElse
               request.Quote Is Nothing OrElse
               request.Answers Is Nothing OrElse
               request.Answers.Any() = False OrElse
               request.LobType = QuickQuoteObject.QuickQuoteLobType.None OrElse
               request.GoverningState = QuickQuoteHelperClass.QuickQuoteState.None Then
                Throw New ArgumentException("request or one of its required properties is null", NameOf(request))
            End If

            For Each ans In request.Answers
                'initialize policyUnderwriting object
                Dim puw = DefaultAnswerPrototype()

                If ans.QuestionAnswerYes OrElse
                   ans.QuestionAnswerNo OrElse
                   ans.HideFromDisplay Then

                    With puw
                        .PolicyUnderwritingCodeId = ans.PolicyUnderwritingCodeId
                        .PolicyUnderwritingAnswer = IIf(ans.QuestionAnswerYes, QUESTION_ANSWER_YES, IIf(ans.QuestionAnswerNo, QUESTION_ANSWER_NO, String.Empty))
                        'May need to make this code more extensible based on the ExtraAnswerType
                        .PolicyUnderwritingExtraAnswerTypeId = IIf(Not String.IsNullOrWhiteSpace(ans.DetailText), "1", String.Empty)
                        .PolicyUnderwritingExtraAnswer = ans.DetailText

                        If Not String.IsNullOrWhiteSpace(ans.PolicyUnderwritingTabId) Then
                            .PolicyUnderwritingTabId = ans.PolicyUnderwritingTabId
                        End If

                        If Not String.IsNullOrWhiteSpace(ans.PolicyUnderwritingLevelId) Then
                            .PolicyUnderwritingLevelId = ans.PolicyUnderwritingLevelId
                        End If

                        If ans.HideFromDisplay Then
                            puw.PolicyUnderwritingAnswer = If(ans.DefaultValueIfHidden, QUESTION_ANSWER_NO)
                        End If
                    End With
                    If Not SaveAnswerToQuote(puw, request.Quote) Then
                        'if we have an issue saving the answer that isn't an exception
                        retval = False
                        Exit For
                    End If
                End If
            Next ans

            Return retval
        End Function

        Private Const QUESTION_ANSWER_YES = "1"
        Private Const QUESTION_ANSWER_NO = "-1"

        Protected Overridable Function RetrieveAnswerValueFromQuote(policyUnderwritingCodeId As String, quote As QuickQuoteObject) As QuickQuotePolicyUnderwriting
            'default based on PPA, may change later
            Dim retval As QuickQuotePolicyUnderwriting = Nothing
            Dim multiStateQuotes = _qqHelper.MultiStateQuickQuoteObjects(quote)

            If multiStateQuotes?.FirstOrDefault()?.PolicyUnderwritings?.Any() Then
                retval = multiStateQuotes.First().PolicyUnderwritings.FirstOrDefault(Function(puw) puw.PolicyUnderwritingCodeId = policyUnderwritingCodeId)
            End If

            Return retval
        End Function

        ''' <summary>
        ''' returns a default QuickQuotePolicyUnderwriting object to be used when saving answers.
        ''' overridable in derived classes in case this needs to be changed for whatever reason
        ''' </summary>
        ''' <returns></returns>
        Protected Overridable Function DefaultAnswerPrototype() As QuickQuotePolicyUnderwriting
            'May need to make this code more extensible in the future
            Dim retval As New QuickQuotePolicyUnderwriting() With
               {
                   .PolicyUnderwritingAnswerTypeId = "0",
                   .PolicyUnderwritingTabId = "1",
                   .PolicyUnderwritingLevelId = "1"
               }
            Return retval
        End Function

        Protected Overridable Function SaveAnswerToQuote(answer As QuickQuotePolicyUnderwriting, quote As QuickQuoteObject) As Boolean
            Dim retval As Boolean = False
            'default is to follow pattern for PPA, this may change in the future
            Dim multiStateQuotes As List(Of QuickQuoteObject) = _qqHelper.MultiStateQuickQuoteObjects(quote)

            If (multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Any) AndAlso
               (quote.QuoteStates IsNot Nothing AndAlso quote.QuoteStates.Any) Then

                'save to each state quote
                For Each msq As QuickQuoteObject In multiStateQuotes
                    If msq.PolicyUnderwritings Is Nothing Then
                        msq.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                    Else
                        Dim puwIndex = msq.PolicyUnderwritings.FindIndex(Function(puw) puw.PolicyUnderwritingCodeId = answer.PolicyUnderwritingCodeId)
                        If puwIndex > -1 Then
                            msq.PolicyUnderwritings.RemoveAt(puwIndex)
                        End If
                    End If

                    msq.PolicyUnderwritings.Add(answer)
                Next
                retval = True
            End If
            Return retval
        End Function
        'filter routines

        ''' <summary>
        ''' Processes the <see cref="CustomFilters"/> list to excluded or include questions based on custom criteria.        ''' 
        ''' Can be overriden in derived classes.
        ''' </summary>
        ''' <param name="questions"></param>
        ''' <param name="request"></param>
        ''' <remarks>
        ''' The filters are not evaluated in any particular order, so an item may be exlucded before it can be included and vice versa.
        ''' The worst case execution is every filter is evaluated for every question that has not been filtered by <see cref="UnderwritingQuestionRequest.TypeFilter"/> or <see cref="QuickQuoteObject.QuickQuoteLobType"/>
        ''' It is advisable to subclass <see cref="UnderwritingQuestionsService"/> and provide custom filters or override this method rather than using a single instance with a diverse and potentially clashing set of filters
        ''' </remarks>
        Protected Overridable Sub Filter_Special(ByRef questions As QuestionList,
                                                request As UnderwritingQuestionRequest)
            For Each questionFilter In CustomFilters
                For Each q In questions
                    Dim response = questionFilter(request, q)
                    If Not response.include Then
                        If response.hideOnExclude Then
                            q.HideFromDisplay = True
                        Else
                            questions.Remove(q)
                        End If
                    End If
                Next
            Next
        End Sub
        Private Sub Filter_KillOnly(ByRef questions As CachedListBuilder)
            questions.RemoveAll(Function(q) q.IsTrueKillQuestion = False)
        End Sub
        Private Sub Filter_ExcludeUnmapped(ByRef questions As CachedListBuilder)
            questions.RemoveAll(Function(q) q.IsUnmapped)
        End Sub
        ''' <summary>
        ''' return all questions by where GoverningState matches the request.
        ''' </summary>
        ''' <remarks>
        ''' If a question from <paramref name="questions"/> does not have a GoverningState specified (None),
        ''' it will be included unless <paramref name="request"/>'s TypeFilter property includes GoverningStateOnly
        ''' If the <paramref name="questions"/> has a two questions with the same QuestionNumber,one with GoverningState set and the other None, 
        ''' the question with the GoverningState set is kept and the other excluded
        ''' </remarks>
        ''' <param name="questions"></param>
        ''' <param name="request"></param>
        Private Sub Filter_GoverningState(ByRef questions As CachedListBuilder,
                                          request As UnderwritingQuestionRequest)

            If request.TypeFilter.HasFlag(UnderwritingQuestionRequest.QuestionTypeFilter.GoverningStateOnly) Then
                questions.RemoveAll(Function(q) q.GoverningState <> request.GoverningState)
            Else
                'if the filter is not specified and multiple questions with the same Question Number,
                'we should return the questions with this filter value set
                Dim grouped = questions.GroupBy(Of Integer)(Function(q) q.QuestionNumber)

                For Each grp In grouped
                    If grp.Any(Function(q) q.GoverningState = request.GoverningState) Then
                        questions.RemoveAll(Function(q) q.QuestionNumber = grp.Key AndAlso q.GoverningState <> request.GoverningState)
                    Else
                        questions.RemoveAll(Function(q) q.QuestionNumber = grp.Key AndAlso q.GoverningState <> QuickQuoteHelperClass.QuickQuoteState.None)
                    End If
                Next
            End If
        End Sub
        Private Sub Filter_MinimumEffectiveDate(ByRef questions As CachedListBuilder,
                                                request As UnderwritingQuestionRequest)
            If request.TypeFilter.HasFlag(UnderwritingQuestionRequest.QuestionTypeFilter.MinimumEffectiveDate) AndAlso
               request.MinimumEffectiveDate.HasValue Then
                'remove all questions with the specified filter when the quote effective is earlier
                questions.RemoveAll(Function(q)
                                        Dim dval As Date

                                        Return q.MinimumEffectiveDate.HasValue AndAlso
                                               Date.TryParse(request.Quote.EffectiveDate, dval) AndAlso
                                               dval < q.MinimumEffectiveDate.Value
                                    End Function)
                'we may have questions with the same QuestionNumber, prefer those one with the filter value set
                Dim grouped = questions.GroupBy(Of Integer)(Function(q) q.QuestionNumber)

                For Each grp In grouped
                    If grp.Any(Function(q) q.MinimumEffectiveDate.HasValue) Then
                        questions.RemoveAll(Function(q) q.QuestionNumber = grp.Key AndAlso Not q.MinimumEffectiveDate.HasValue)
                    Else
                        questions.RemoveAll(Function(q) q.QuestionNumber = grp.Key AndAlso q.MinimumEffectiveDate.HasValue)
                    End If
                Next
            End If
        End Sub

        Private Sub Filter_Subset(ByRef questions As CachedListBuilder,
                                  request As UnderwritingQuestionRequest)
            'this function does not handle for cases of duplicate question numbers
            If request.TypeFilter.HasFlag(UnderwritingQuestionRequest.QuestionTypeFilter.Subset) Then
                If request.SubsetToFind_QuestionNumbers?.Any() Then
                    'flexing the LINQ query syntax muscles because I tire of typing Function for lambda expressions in VB
                    'questions = (From questionNumber In request.SubsetToFind_QuestionNumbers
                    '             Join q In questions On questionNumber Equals q.QuestionNumber
                    '             Select q).ToList()
                    questions.RemoveAll(Function(uwq) Not request.SubsetToFind_QuestionNumbers.Contains(uwq.QuestionNumber))
                ElseIf request.SubsetToFind_PolicyUnderwritingCodeIds?.Any() Then
                    'questions = (From codeId In request.SubsetToFind_PolicyUnderwritingCodeIds
                    '             Join q In questions On codeId Equals q.PolicyUnderwritingCodeId
                    '             Select q).ToList()
                    questions.RemoveAll(Function(uwq) Not request.SubsetToFind_PolicyUnderwritingCodeIds.Contains(uwq.QuestionNumber))
                End If
            End If
        End Sub

        Private Sub MatchQuestionsToExistingResponses(questions As QuestionList, quote As QuickQuoteObject)
            'get the responses from the quote, if they exist, and update the question list

            'match question and existing response by PolicyUnderwritingCodeId
            'return the question updated with the response
            For Each q In questions
                Dim answer = RetrieveAnswerValueFromQuote(q.PolicyUnderwritingCodeId, quote)

                If answer Is Nothing Then Continue For 'couldn't find an answer, then skip this question

                If answer.PolicyUnderwritingAnswer = QUESTION_ANSWER_YES Then
                    q.QuestionAnswerYes = True
                ElseIf answer.PolicyUnderwritingAnswer = QUESTION_ANSWER_NO Then
                    q.QuestionAnswerNo = True
                End If

                'May need to make this code more extensible based on the ExtraAnswerType
                If Not String.IsNullOrWhiteSpace(answer.PolicyUnderwritingExtraAnswer) Then
                    q.PolicyUnderwritingExtraAnswerTypeId = 1
                    q.PolicyUnderwritingExtraAnswer = answer.PolicyUnderwritingExtraAnswer
                End If
            Next
        End Sub
        'probably need to separate this into it's own service (Repository) to aid with Unit Testing 
        Private Function loadQuestionsList(Optional ByVal forceLoadFromSource As Boolean = False) As QuestionList
            'inner function implemented as a lambda
            '*only used in this function
            '*not useful anywhere else
            '*only exists to prevent duplication of code
            Dim readValuesFromFile As Func(Of IFileInfo, QuestionList) =
                Function(uwFile As IFileInfo) As IEnumerable(Of UnderwritingQuestion)
                    Dim qList As IEnumerable(Of UnderwritingQuestion) = Enumerable.Empty(Of UnderwritingQuestion)

                    Using textStream As New StreamReader(uwFile.CreateReadStream())
                        qList = JsonConvert.DeserializeObject(Of IEnumerable(Of UnderwritingQuestion))(textStream.ReadToEnd())
                    End Using
                    Return qList
                End Function

            Dim retval As New QuestionList
            Dim uwFilePathInfo As IFileInfo = _fileProvider.GetFileInfo(_uwFilePath)

            If Not uwFilePathInfo?.Exists Then
                For Each uwf In _fileProvider.GetDirectoryContents(_uwFilePath)
                    retval.AddRange(readValuesFromFile(uwf))
                Next
            Else
                retval.AddRange(readValuesFromFile(uwFilePathInfo))
            End If

            Return retval
        End Function
    End Class

    Public Class UnderwritingQuestionRequest
        'New types must be shifted to the next binary position: newEnumValue = 1 << nextPowerOf2
        <Flags>
        Public Enum QuestionTypeFilter
            None = 0
            KillOnly = 1 << 0
            ''' <summary>
            ''' Exclude questions that are not explicitly mapped to a <see cref="QuickQuotePolicyUnderwriting.PolicyUnderwritingCodeId"/>.
            ''' This is used in most cases
            ''' </summary>
            ExcludeUnmapped = 1 << 1
            ''' <summary>
            ''' Exclude questions that don't have the same governing state as the quote
            ''' </summary>
            GoverningStateOnly = 1 << 2
            ''' <summary>
            ''' Exclude questions with an explicitly set <see cref="UWQuestions.VRUWQuestion.MinimumEffectiveDate"/> the later than the Quote's <see cref="QuickQuoteObject.EffectiveDate"/>.
            ''' It may be desired to configure a question with the same <see cref="UWQuestions.VRUWQuestion.QuestionNumber"/> that does not have MinimumEffectiveDate set
            ''' </summary>
            MinimumEffectiveDate = 1 << 3
            ''' <summary>
            ''' instructs the service to limit the returned questions based on values set within <see cref="SubsetToFind"/>
            ''' </summary>
            Subset = 1 << 4
        End Enum
        ''' <summary>
        ''' Required. <see cref="IUnderwritingQuestionsService"/> uses this to initially determine which questions to consider
        ''' </summary>        ''' 
        ''' <returns></returns>
        Public Property LobType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType
        ''' <summary>
        ''' used to filter by GoverningState
        ''' </summary>
        ''' <returns></returns>
        Public Property GoverningState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None

        Public Property MinimumEffectiveDate As Date?
        ''' <summary>
        ''' Required. Used to retrieve exisitng answers and save results
        ''' </summary>
        ''' <returns></returns>
        <JsonIgnore>
        Public Property Quote As QuickQuote.CommonObjects.QuickQuoteObject
        ''' <summary>
        ''' Directs the <see cref="IUnderwritingQuestionsService"/> to include or exclude specific questions.        
        ''' Combine filters using bitwise OR
        ''' </summary>
        ''' <returns></returns>
        Public Property TypeFilter As QuestionTypeFilter
        ''' <summary>
        ''' The range of either QuestionNumbers or PolicyUnderwritingCodeIds to return.
        ''' Filtering based on SubsetFind is performed last.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>        
        ''' Set only one property per search to an Array or other IEnumerable
        ''' Literal expressions can be used to specify values for either property.
        ''' </remarks>
        Public Property SubsetToFind_QuestionNumbers As IEnumerable(Of Integer)
        ''' <summary>
        ''' The range of either QuestionNumbers or PolicyUnderwritingCodeIds to return.
        ''' Filtering based on SubsetFind is performed last.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Set only one property per search to an Array or other IEnumerable
        ''' Literal expressions can be used to specify values for either property.
        ''' </remarks>
        Public Property SubsetToFind_PolicyUnderwritingCodeIds As IEnumerable(Of String)
        Public Overrides Function ToString() As String
            Return $"{{""{NameOf(UnderwritingQuestionRequest)}"":{JsonConvert.SerializeObject(Me)}}}"
        End Function
        ''' <summary>
        ''' support function for caching 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>properties that likely change on every request need to have the JSonIgnore attribute applied to avoid unnecessary cache bloating</remarks>
        Protected Friend Function ToKey() As String
            Return $"{UnderwritingQuestionsService.UWQ_SVC_CACHE_KEY}{Me}"
        End Function
    End Class

    Public Class UnderwritingSaveRequest
        Public Property LobType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType
        Public Property GoverningState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
        Public Property Quote As QuickQuote.CommonObjects.QuickQuoteObject
        Public Property Answers As New AnswerList
    End Class

End Namespace