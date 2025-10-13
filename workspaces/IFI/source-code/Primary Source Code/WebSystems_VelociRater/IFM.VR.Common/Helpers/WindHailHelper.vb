Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers

    Public Class WindHailHelper
        Public Shared Function GetWindHailDeducID(limit As String, hailPercentSelectedID As String, minHaildedcId As String, standardDedcId As String) As String
            '  3-27-2013
            Dim returnDedutibleId As String = "0"
            If String.IsNullOrWhiteSpace(limit) = False Then
                ' need to determine this based on limit,property deduc, % , and min deduc
                Dim minDeducPerDDValAsPercent As Double = 0.0
                Select Case hailPercentSelectedID
                    Case "32"
                        minDeducPerDDValAsPercent = 0.01
                    Case "33"
                        minDeducPerDDValAsPercent = 0.02
                    Case "34"
                        minDeducPerDDValAsPercent = 0.05
                    Case Else
                        minDeducPerDDValAsPercent = 0.0 'N/A
                End Select

                Dim minWindHailDeducAsVal As Int32 = 0
                Select Case minHaildedcId
                    Case "0"
                        minWindHailDeducAsVal = 0 'N/A
                    Case "4"
                        minWindHailDeducAsVal = 250
                    Case "8"
                        minWindHailDeducAsVal = 500
                    Case "9"
                        minWindHailDeducAsVal = 1000
                    Case "15"
                        minWindHailDeducAsVal = 2500
                    Case "16"
                        minWindHailDeducAsVal = 5000
                    Case "17"
                        minWindHailDeducAsVal = 10000
                    Case "19"
                        minWindHailDeducAsVal = 25000
                    Case Else
                        minWindHailDeducAsVal = 0
                End Select

                Dim standardDeducVal As Int32 = 0
                Select Case standardDedcId
                    Case "0"
                        standardDeducVal = 0
                    Case "4"
                        standardDeducVal = 250
                    Case "8"
                        standardDeducVal = 500
                    Case "9"
                        standardDeducVal = 1000
                    Case "15"
                        standardDeducVal = 2500
                    Case "16"
                        standardDeducVal = 5000
                    Case "17"
                        standardDeducVal = 10000
                    Case "19"
                        standardDeducVal = 25000
                    Case Else
                        standardDeducVal = 0
                End Select

                Dim minPercentDeductibleVal As Int32 = CInt(CInt(limit) * minDeducPerDDValAsPercent)
                'If Not (minWindHailDeducAsVal = 0 Or standardDeducVal = 0 Or minPercentDeductibleVal = 0) Then
                Dim pairs As New List(Of KeyValuePair(Of Int32, String))
                pairs.Add(New KeyValuePair(Of Int32, String)(minWindHailDeducAsVal, minHaildedcId))
                pairs.Add(New KeyValuePair(Of Int32, String)(minPercentDeductibleVal, hailPercentSelectedID))
                pairs.Add(New KeyValuePair(Of Int32, String)(standardDeducVal, standardDedcId))

                'which is bigger ?
                'then determine the id of the largest and select that as the wind/hail deduc 
                returnDedutibleId = (From pair As KeyValuePair(Of Int32, String) In pairs Order By pair.Key Descending Select pair.Value).FirstOrDefault()
                'End If

            End If

            Return returnDedutibleId
        End Function


        '5-16-2013
        Public Shared Function GetFormatedBuildingWindHailDeductible(building As QuickQuoteBuilding) As String
            If building IsNot Nothing Then
                Dim WH_deductible As String = building.OptionalWindstormOrHailDeductible
                Select Case building.OptionalWindstormOrHailDeductibleId
                    Case "4"
                        WH_deductible = "$250"
                    Case "8"
                        WH_deductible = "$500"
                    Case "9"
                        WH_deductible = "$1,000"
                    Case "15"
                        WH_deductible = "$2,500"

                    Case "16"
                        WH_deductible = "$5,000"
                    Case "17"
                        WH_deductible = "$10,000"
                    Case "19"
                        WH_deductible = "$25,000"
                    Case "32"
                        WH_deductible = "1%"
                    Case "33"
                        WH_deductible = "2%"
                    Case "34"
                        WH_deductible = "5%"
                End Select
                Return WH_deductible
            End If
            Return "N/A"
        End Function

        Public Shared Function GetFormatedPitoWindHailDeductible(building As QuickQuotePropertyInTheOpenRecord) As String
            If building IsNot Nothing Then
                Dim WH_deductible As String = building.OptionalWindstormOrHailDeductible
                Select Case building.OptionalWindstormOrHailDeductibleId
                    Case "4"
                        WH_deductible = "$250"
                    Case "8"
                        WH_deductible = "$500"
                    Case "9"
                        WH_deductible = "$1,000"
                    Case "15"
                        WH_deductible = "$2,500"

                    Case "16"
                        WH_deductible = "$5,000"
                    Case "17"
                        WH_deductible = "$10,000"
                    Case "19"
                        WH_deductible = "$25,000"
                    Case "32"
                        WH_deductible = "1%"
                    Case "33"
                        WH_deductible = "2%"
                    Case "34"
                        WH_deductible = "5%"
                End Select
                Return WH_deductible
            End If
            Return "N/A"
        End Function
    End Class

End Namespace
