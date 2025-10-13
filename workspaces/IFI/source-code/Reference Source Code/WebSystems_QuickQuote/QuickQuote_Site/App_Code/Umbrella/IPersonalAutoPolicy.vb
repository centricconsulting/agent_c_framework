Namespace QuickQuote.CommonObjects.Umbrella
    Public Interface IPersonalAutoPolicy
        Property BodilyInjuryLimitA As String
        Property BodilyInjuryLimitB As String
        Property Drivers As List(Of Driver)
        Property PropertyDamageLimit As String
        Property CombinedSingleLimit As String
        Property Vehicles As List(Of Vehicle)
        Property YouthfulOperators As List(Of YouthfulOperator)
    End Interface
End Namespace
