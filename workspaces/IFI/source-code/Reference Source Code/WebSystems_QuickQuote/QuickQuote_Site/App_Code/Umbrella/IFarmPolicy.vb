Namespace QuickQuote.CommonObjects.Umbrella
    Public Interface IFarmPolicy
        Inherits IWorkersCompensationPolicy 'for Stop Gap (OH)
        Property PersonalLiabilities As List(Of PersonalLiability)
        Property PersonalLiabilityLimit As String
        Property FarmLiabilities As List(Of FarmLiability)               
        'Property OccurrenceLiabilityLimit As String   'not needed 5/20/2021
        'Property AggregateLiabilityLimit As String    'not needed 5/20/2021
    End Interface
End Namespace
