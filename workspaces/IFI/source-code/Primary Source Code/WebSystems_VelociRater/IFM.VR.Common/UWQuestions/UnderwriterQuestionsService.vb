
Imports System.Configuration
Imports System.IO
Imports IFM.Configuration.Extensions
Imports Newtonsoft.Json
Imports UnderwritingQuestion = IFM.VR.Common.UWQuestions.VRUWQuestion


Public Interface IUnderwriterQuestionsProvider
    Function GetQuestions(request As QuestionRequest) As IEnumerable(Of UnderwritingQuestion)
End Interface

Public Class UnderwriterQuestionsService
    Implements IUnderwriterQuestionsProvider

    Const VR_UNDERWRITING_QUESTIONS_PATH = "VR_UnderwritingQuestionsPath"

    Public Function GetQuestions(request As QuestionRequest) As IEnumerable(Of UnderwritingQuestion) Implements IUnderwriterQuestionsProvider.GetQuestions
        Dim retval As New List(Of UnderwritingQuestion)
        Try
            If request.LobType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.None Then
                Dim uwFile = ConfigurationManager.AppSettings.ConstructWith(Of FileInfo)(VR_UNDERWRITING_QUESTIONS_PATH, Nothing)

                If uwFile IsNot Nothing AndAlso uwFile.Exists() Then
                    Using textStream As StreamReader = uwFile.OpenText()
                        Dim questions = JsonConvert.DeserializeObject(Of IEnumerable(Of UnderwritingQuestion))(textStream.ReadToEnd())

                        questions = questions.Where(Function(q) q.LobType = request.LobType)

                        If request.OverrideState <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                            questions = questions.Where(Function(q) q.OverrideState = request.OverrideState)
                        End If
                        If request.KillQuestionOnly Then
                            questions = questions.Where(Function(q) q.IsTrueKillQuestion)
                        End If

                        retval.AddRange(questions)
                    End Using
                End If
            End If
        Catch

        End Try
        Return retval
    End Function

End Class

Public Class QuestionRequest
    Public Property LobType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType
    Public Property KillQuestionOnly As Boolean
    Public Property OverrideState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
End Class