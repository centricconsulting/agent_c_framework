Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports DevDictionaryHelper

Public Class EndorsementsCppHelper
    Private _quote As QuickQuoteObject
    Public Property Quote() As QuickQuoteObject
        Get
            Return _quote
        End Get
        Set(ByVal value As QuickQuoteObject)
            _quote = value
        End Set
    End Property

    Private Property _CppDictItems As DevDictionaryHelper.AllCommercialDictionary
    Public ReadOnly Property CppDictItems As DevDictionaryHelper.AllCommercialDictionary
        Get
            If Quote IsNot Nothing Then
                If _CppDictItems Is Nothing Then
                    _CppDictItems = New DevDictionaryHelper.AllCommercialDictionary(Quote)
                End If
            End If
            Return _CppDictItems
        End Get
    End Property

    Public Sub New(ByRef quote As QuickQuoteObject)
        _quote = quote
        CppDictItems.GetAllCommercialDictionary()
    End Sub

    Public Function ShowAddClassCodeLink() As Boolean
        Return CppDictItems.AreAnyLocationsAdded
    End Function

    Public Function ShouldDisableSubmit() As Boolean
        If ShowDeleteMessage() OrElse ShowAddMessage() Then
            Return True
        End If
        Return False
    End Function

    Public Function ShowDeleteMessage() As Boolean
        If CppDictItems.AreAnyLocationsDeleted _
            AndAlso CppDictItems.AreAnyLocationsAdded = False _
            AndAlso AnyClassCodesAtLocationLevel() = False Then
            Return True
        End If
        Return False
    End Function

    Public Function ShowAddMessage() As Boolean
        If CppDictItems.AreAnyLocationsAdded _
            AndAlso AnyClassCodesAtPolicyLevel() = False Then
            Return True
        End If
        Return False
    End Function

    Public Function AnyClassCodesAtPolicyLevel() As Boolean
        Return Me.Quote?.GLClassifications?.Any()
    End Function

    Public Function AnyClassCodesAtLocationLevel() As Boolean
        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
            For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                If LOC.GLClassifications.Any() Then
                    Return True
                End If
            Next
        End If
        Return False
    End Function

End Class
