Imports System.Collections
Imports System.Linq
Imports IFM.VR.Common.Underwriting
Imports IFM.VR.Common.Underwriting.LOB
Imports Microsoft.Extensions.Caching.Memory
Imports Microsoft.Extensions.FileProviders
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects.QuickQuoteObject

Namespace IFM.VR.Common.Underwriting
    ''' <summary>
    ''' Used for cases where constructor dependency injection using a framework is either not available
    ''' or decisions need to be made when constructing the instance    ''' 
    ''' </summary>
    ''' <remarks>
    ''' The factory can be used to support dependency injection as well
    ''' </remarks>
    Public NotInheritable Class UnderwritingQuestionServiceFactory
        Protected Shared qqHelper As New QuickQuoteHelperClass
        Protected Shared memCache As New MemoryCache(New MemoryCacheOptions())
        Protected Shared appDirectoryFileProvider As New PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory)

        Protected Shared services As New Dictionary(Of QuickQuoteLobType, IUnderwritingQuestionsService)

        Public Shared Function BuildFor(lobType As QuickQuoteLobType) As IUnderwritingQuestionsService

            Dim retval As IUnderwritingQuestionsService = Nothing

            If Not services.TryGetValue(lobType, retval) Then
                Select Case lobType
                    Case QuickQuoteLobType.AutoPersonal
                        retval = New UnderwritingQuestionsService(memCache, appDirectoryFileProvider, qqHelper)
                    Case QuickQuoteLobType.DwellingFirePersonal
                        retval = New DFR_UnderwritingQuestionService(memCache, appDirectoryFileProvider, qqHelper)
                    Case Else
                        'this code can be changed to default to 
                        'retval = New UnderwritingQuestionsService(memCache, appDirectoryFileProvider, qqHelper)
                        'unless an LOB needs special filters 
                End Select
                If retval IsNot Nothing Then
                    services.Add(lobType, retval)
                End If
            End If
            If retval Is Nothing Then
                Throw New ArgumentOutOfRangeException("A service for the specified LobType is not available", NameOf(lobType))
            End If
            Return retval
        End Function
    End Class
End Namespace
