Namespace QuickQuote.CommonObjects.Umbrella
    Public Interface IHomePolicy
        Property InvestmentProperties As List(Of Location)
        Property MiscellaneousLiabilities As List(Of MiscellaneousLiability)
        Property PersonalLiabilities As List(Of PersonalLiability)
        Property PersonalLiabilityLimit As String
        Property ProfessionalLiabilities As List(Of ProfessionalLiability)
        Property RecreationalVehicles As List(Of RecreationalVehicle)
        Property Watercrafts As List(Of Watercraft)
    End Interface
End Namespace
