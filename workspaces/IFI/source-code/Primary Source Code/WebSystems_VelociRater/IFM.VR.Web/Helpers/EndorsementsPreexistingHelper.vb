Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

'Added 10/18/2021 for BOP Endorsements task 61660 MLW
Public Class EndorsementsPreexistingHelper
    Private _quote As QuickQuoteObject
    Public Property Quote() As QuickQuoteObject
        Get
            Return _quote
        End Get
        Set(ByVal value As QuickQuoteObject)
            _quote = value
        End Set
    End Property

    Public Sub New(ByRef quote As QuickQuoteObject)
        _quote = quote
    End Sub

    Public Overloads Function IsPreexistingLocation(locationIndex As Integer) As Boolean
        Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
        Dim PreExistingFlag As Boolean = False
        AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
        If Me.Quote IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(locationIndex) AndAlso AllPreExistingItems IsNot Nothing AndAlso AllPreExistingItems.PreExisting_Locations IsNot Nothing Then
            Dim MyLocation = Quote.Locations.GetItemAtIndex(locationIndex)
            PreExistingFlag = AllPreExistingItems.PreExisting_Locations.isPreExistingLocationByLocationObject(MyLocation)
        End If
        Return PreExistingFlag
    End Function

    Public Overloads Function IsPreexistingLocation(myLocation As QuickQuoteLocation) As Boolean
        Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
        Dim PreExistingFlag As Boolean = False
        AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
        If Me.Quote IsNot Nothing AndAlso AllPreExistingItems IsNot Nothing AndAlso AllPreExistingItems.PreExisting_Locations IsNot Nothing Then
            PreExistingFlag = AllPreExistingItems.PreExisting_Locations.isPreExistingLocationByLocationObject(myLocation)
        End If
        Return PreExistingFlag
    End Function
End Class
