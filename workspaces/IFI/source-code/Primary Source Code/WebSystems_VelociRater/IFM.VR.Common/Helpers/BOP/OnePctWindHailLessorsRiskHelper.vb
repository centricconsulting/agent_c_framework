Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports PopupMessageClass

Namespace IFM.VR.Common.Helpers
    Public Class OnePctWindHailLessorsRiskHelper

        Private Shared _OnePctWindHailLessorsRiskSettings As NewFlagItem
        Public Shared ReadOnly Property OnePctWindHailLessorsRiskSettings() As NewFlagItem
            Get
                If _OnePctWindHailLessorsRiskSettings Is Nothing Then
                    _OnePctWindHailLessorsRiskSettings = New NewFlagItem("VR_BOP_WindHail1PctWithLROAndMinBldgDeductible_Settings")
                End If
                Return _OnePctWindHailLessorsRiskSettings
            End Get
        End Property

        Public Shared Function OnePctWindHailLessorsRiskEnabled() As Boolean
            Return OnePctWindHailLessorsRiskSettings.EnabledFlag
        End Function

        Public Shared Function OnePctWindHailLessorsRiskEffDate() As Date
            Return OnePctWindHailLessorsRiskSettings.GetStartDateOrDefault("12/8/2024")
        End Function

        Const OnePctWindHailLessorsRiskMsg As String = "A 1% wind hail deductible applies to locations with lessors' risk exposures."

        Public Shared Sub UpdateOnePctWindHailLessorsRisk(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'If Lessors Risk Occupancy, Wind Hail is N/A, and building min limits met, then assign Wind Hail as 1% and show message
                    If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                        Dim i As Integer = 0
                        For Each l In Quote.Locations
                            Dim locationQualifiesForLessorsRiskWindHail1Pct As Boolean = CheckWindHailLessorsRisk(Quote, i)
                            NeedsWarningMessage = Set1PercentWindHailDeductibleAndShowMessageFlagNeeded(l, locationQualifiesForLessorsRiskWindHail1Pct)
                            i += 1
                        Next
                    End If
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(OnePctWindHailLessorsRiskMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No change
            End Select
        End Sub

        Public Shared Function IsOnePctWindHailLessorsRiskAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                OnePctWindHailLessorsRiskSettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, OnePctWindHailLessorsRiskSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

        Public Shared Function CheckWindHailLessorsRisk(Quote As QuickQuoteObject, locIndex As Integer) As Boolean
            'Used in multiple places to see if the location qualifies to have 1% Wind Hail Deductible on a Lessors Risk Occupancy - button clicks, cross date, and to remove N/A from the drop down
            Dim fullyQualifies As Boolean = False
            Dim locPrequalifies As Boolean = False
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                If Quote.Locations.HasItemAtIndex(locIndex) Then
                    Dim loc As QuickQuoteLocation = Quote.Locations(locIndex)
                    Dim qqHelper = New QuickQuoteHelperClass
                    If loc IsNot Nothing AndAlso (Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote OrElse (Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso qqHelper.IsQuickQuoteLocationNewToImage(loc, Quote))) Then
                        Dim locationLimitTotal As Integer = 0
                        If loc.Buildings IsNot Nothing AndAlso loc.Buildings.Count > 0 Then
                            For Each b As QuickQuoteBuilding In loc.Buildings
                                locPrequalifies = CheckForLessorsRiskLocPrequal(locPrequalifies, b)
                                locationLimitTotal += GetLocationBldgLimitTotal(b)
                            Next
                        End If
                        fullyQualifies = LocationFullyQualifiesForLessorsRiskWindHail(locPrequalifies, loc, locationLimitTotal)
                    End If
                End If
            End If
            Return fullyQualifies
        End Function

        Private Shared Function LocationFullyQualifiesForLessorsRiskWindHail(locPrequalifies As Boolean, loc As QuickQuoteLocation, locationLimitTotal As Integer) As Boolean
            Dim fullyQualifies As Boolean = False
            If locPrequalifies Then
                'A 1% Minimum total limit must be met by the deductible selected
                Select Case loc.PropertyDeductibleId
                    Case "22"
                        '$500 Property Deductible, limit total must be >= $50,000
                        If locationLimitTotal >= 50000 Then
                            fullyQualifies = True
                        End If
                    Case "24"
                        '$1,000 Property Deductible, limit total must be >= $100,000
                        If locationLimitTotal >= 100000 Then
                            fullyQualifies = True
                        End If
                    Case "75"
                        '$2,500 Property Deductible, limit total must be >= $250,000
                        If locationLimitTotal >= 250000 Then
                            fullyQualifies = True
                        End If
                    Case "76"
                        '$5,000 Property Deductible, limit total must be >= $500,000
                        If locationLimitTotal >= 500000 Then
                            fullyQualifies = True
                        End If
                    Case "333"
                        '$7,500 Property Deductible, limit total must be >= $750,000
                        If locationLimitTotal >= 750000 Then
                            fullyQualifies = True
                        End If
                    Case "157"
                        '$10,000 Property Deductible, limit total must be >= $1,000,000
                        If locationLimitTotal >= 1000000 Then
                            fullyQualifies = True
                        End If
                End Select
            End If
            Return fullyQualifies
        End Function

        Private Shared Function GetLocationBldgLimitTotal(b As QuickQuoteBuilding) As Integer
            Dim locationLimitTotal As Integer
            Dim buildingLimit As Integer = b.Limit.TryToGetInt32
            Dim personalPropertyLimit As Integer = b.PersonalPropertyLimit.TryToGetInt32
            If buildingLimit > 0 Then
                locationLimitTotal += buildingLimit
            End If
            If personalPropertyLimit > 0 Then
                locationLimitTotal += personalPropertyLimit
            End If
            Return locationLimitTotal
        End Function

        Private Shared Function CheckForLessorsRiskLocPrequal(locPrequalifies As Boolean, b As QuickQuoteBuilding) As Boolean
            'If (b.OccupancyId = "16" OrElse b.OccupancyId = "20") AndAlso isAllowableBldgClassCode(b) Then
            If (b.OccupancyId = "16" OrElse b.OccupancyId = "20") Then
                'Now allowing apartments, condos, and townhouses - isAllowableBldgClassCode removed for now, keeping code in comments case they change their minds.
                'Lessors Risks Occupancies - (16) Non-Owner Occupied Bldg / Lessor's, (20) Owner Occupied Bldg 10% or Less / Lessor's
                locPrequalifies = True
            End If
            Return locPrequalifies
        End Function

        'Now allowing apartments, condos, and townhouses - isAllowableBldgClassCode removed for now, keeping code in comments case they change their minds.
        'Private Shared Function isAllowableBldgClassCode(b As QuickQuoteBuilding) As Boolean
        '    Dim isIt As Boolean = False
        '    If b.BuildingClassifications IsNot Nothing AndAlso b.BuildingClassifications.Count > 0 Then
        '        For Each bc As QuickQuoteClassification In b.BuildingClassifications
        '            If bc.ClassCode IsNot Nothing AndAlso IsNullEmptyorWhitespace(bc.ClassCode) = False Then
        '                If bc.ClassCode.NotEqualsAny("65146", "65147", "65145", "65142", "65141", "65133", "65132") Then
        '                    'Do not want it to be one of these class codes
        '                    '65146 - Apartment Building - 5 to 12 units with no office occupancy
        '                    '65147 - Apartment Building -5 to 12 units with office occupancy
        '                    '69145 - Condominiums - Residential Condominium (Association Risk Only)
        '                    '65142 - Townhouses Or Similar Associations- 4 families Or fewer, with mercantile Or office occupancy-includes 3 Or 4 family lessors risk only And garden apartments
        '                    '65141 - Townhouses Or Similar Associations- 4 families Or fewer, with no mercantile Or office occupancy-includes 3 Or 4 family lessors risk only
        '                    '65133 - Townhouses Or Similar Associations - Over 4 families with mercantile Or office occupancy
        '                    '65132 - Townhouses Or Similar Associations - Over 4 families with no mercantile Or office occupanc
        '                    isIt = True
        '                    Exit For
        '                End If
        '            End If
        '        Next
        '    End If
        '    Return isIt
        'End Function

        Public Shared Sub SetWindHailLessorsRisk(Quote As QuickQuoteObject, locIndex As Integer, myPage As Web.UI.Page)
            'For Button clicks on bottom of Location page - Add New Location, Save, Rate - also for Add Location link in accordion header - New Business and Endorsements
            Dim locationQualifiesForLessorsRiskWindHail1Pct As Boolean = CheckWindHailLessorsRisk(Quote, locIndex)
            Dim l As QuickQuoteLocation = Quote.Locations(locIndex)
            Dim showPopupMessage As Boolean = Set1PercentWindHailDeductibleAndShowMessageFlagNeeded(l, locationQualifiesForLessorsRiskWindHail1Pct)

            If showPopupMessage Then
                Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
                Using popup As New PopupMessageObject(myPage, OnePctWindHailLessorsRiskMsg)
                    With popup
                        .Title = "Wind Hail Deductible"
                        .isFixedPositionOnScreen = True
                        .ZIndexOfPopup = 2
                        .isModal = True
                        .Image = PopupMessageObject.ImageOptions.None
                        .hideCloseButton = True
                        .AddButton("OK", True)
                        .CreateDynamicPopUpWindow()
                    End With
                End Using
            End If
        End Sub

        Private Shared Function Set1PercentWindHailDeductibleAndShowMessageFlagNeeded(l As QuickQuoteLocation, locationQualifiesForLessorsRiskWindHail1Pct As Boolean) As Boolean
            'If Wind Hail Deductible set to N/A (or not yet set) and location has already fully qualified for 1% Wind Hail Deductible to be applied, apply it here and set a flag to later show a message
            Dim showMessage As Boolean = False
            'if Wind Hail Deductible N/A or not yet set - allows for previously selected 1%, 2% or 5% selection so we do not show the popup unnecessarily
            If (IsNullEmptyorWhitespace(l.WindHailDeductibleLimitId) OrElse l.WindHailDeductibleLimitId = "0") AndAlso locationQualifiesForLessorsRiskWindHail1Pct Then
                showMessage = True
                l.WindHailDeductibleLimitId = "222" '1% Wind Hail Deductible
            End If
            Return showMessage
        End Function
    End Class
End Namespace