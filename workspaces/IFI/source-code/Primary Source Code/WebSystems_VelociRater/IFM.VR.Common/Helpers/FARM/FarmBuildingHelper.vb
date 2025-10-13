Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.MultiState

Namespace IFM.VR.Common.Helpers.FARM

    Public Class FarmBuildingHelper

        Public Shared Sub RemoveFarmBuilding(qq As QuickQuote.CommonObjects.QuickQuoteObject, locationIndex As Int32, buildingIndex As Int32)
            If qq.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If qq IsNot Nothing And qq.Locations.HasItemAtIndex(locationIndex) AndAlso qq.Locations.GetItemAtIndex(locationIndex).Buildings.HasItemAtIndex(buildingIndex) Then
                'Dim dwellingIdx As Integer = 1
                Dim selectedLossIncome = qq.Locations(locationIndex).IncomeLosses.Find(Function(p) p.Description.Trim = String.Format("LOC{0}BLD{1}", (locationIndex + 1), (buildingIndex + 1)))
                ' Check to see if ANY Loss Income records exist for the location
                If qq.Locations(locationIndex).IncomeLosses.Count > 0 Then
                    ' Check to see if the specific location as a Loss Income record
                    If selectedLossIncome IsNot Nothing Then
                        qq.Locations(locationIndex).IncomeLosses.Remove(selectedLossIncome)

                        ' Renumber remaining coverages with proper building number
                        For Each lossIncome In qq.Locations(locationIndex).IncomeLosses
                            Dim bldNum As Integer = lossIncome.Description.Substring(lossIncome.Description.Length - 1, 1)
                            If bldNum > (buildingIndex + 1) Then
                                lossIncome.Description = String.Format("LOC{0}BLD{1}", (locationIndex + 1), (bldNum - 1))
                            End If
                        Next
                    Else
                        'dwellingIdx += 1

                        ' Renumber remaining coverages with proper building number
                        For Each lossIncome In qq.Locations(locationIndex).IncomeLosses
                            Dim bldNum As Integer = lossIncome.Description.Substring(lossIncome.Description.Length - 1, 1)
                            If bldNum > (buildingIndex + 1) Then
                                If buildingIndex = 0 Then
                                    lossIncome.Description = String.Format("LOC{0}BLD{1}", (locationIndex + 1), (bldNum - 1))
                                Else
                                    lossIncome.Description = String.Format("LOC{0}BLD{1}", (locationIndex + 1), (bldNum - buildingIndex))
                                End If
                            End If
                        Next
                    End If
                End If

                'Updated 9/10/18 for multi state MLW - QQHelper, parts, SubQuotes, sq - qq to sq
                Dim QQHelper As QuickQuote.CommonMethods.QuickQuoteHelperClass = Nothing
                If QQHelper Is Nothing Then
                    QQHelper = New QuickQuote.CommonMethods.QuickQuoteHelperClass
                End If
                Dim parts = QQHelper.MultiStateQuickQuoteObjects(qq)
                If parts IsNot Nothing Then
                    Dim SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = parts
                    If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                            If sq.OptionalCoverages IsNot Nothing Then
                                'remove suff cov for  this building if it exits
                                Dim removeCov = (From cov In sq.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock AndAlso cov.Description = String.Format("{0}.{1}", locationIndex + 1, buildingIndex + 1) Select cov).FirstOrDefault()
                                If removeCov IsNot Nothing Then
                                    sq.OptionalCoverages.Remove(removeCov)
                                End If
                                'update the other descriptions because the building numbers have changed
                                Dim thisLocationsSuffCovs = (From cov In sq.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock AndAlso cov.Description.StartsWith((locationIndex + 1).ToString() + ".") Select cov).ToList()

                                If thisLocationsSuffCovs IsNot Nothing AndAlso thisLocationsSuffCovs.Any() Then
                                    For Each cov In thisLocationsSuffCovs
                                        Dim covBuildingIndex As Int32 = If(cov.Description.Contains(".") AndAlso IsNumeric(cov.Description.Split(".")(1)), CInt(cov.Description.Split(".")(1)) - 1, -1)
                                        If covBuildingIndex > buildingIndex Then
                                            cov.Description = String.Format("{0}.{1}", locationIndex + 1, covBuildingIndex) ' can use covBuildingIndex because it is index and not num so it has already been decremented by 1
                                        End If
                                    Next
                                End If
                            End If
                        Next
                        ''if using quote for location - remove, not using - MLW
                        'Dim quoteToUse = QQHelper.StateQuoteForLocation(SubQuotes, qq.Locations(locationIndex), alwaysReturnQuoteIfPossibleOnNoMatch:=True)
                        ''remove suff cov for  this building if it exits
                        'Dim removeCov = (From cov In quoteToUse.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock AndAlso cov.Description = String.Format("{0}.{1}", locationIndex + 1, buildingIndex + 1) Select cov).FirstOrDefault()
                        'If removeCov IsNot Nothing Then
                        '    quoteToUse.OptionalCoverages.Remove(removeCov)
                        'End If
                        ''update the other descriptions because the building numbers have changed
                        'Dim thisLocationsSuffCovs = (From cov In quoteToUse.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock AndAlso cov.Description.StartsWith((locationIndex + 1).ToString() + ".") Select cov).ToList()

                        'If thisLocationsSuffCovs IsNot Nothing AndAlso thisLocationsSuffCovs.Any() Then
                        '    For Each cov In thisLocationsSuffCovs
                        '        Dim covBuildingIndex As Int32 = If(cov.Description.Contains(".") AndAlso IsNumeric(cov.Description.Split(".")(1)), CInt(cov.Description.Split(".")(1)) - 1, -1)
                        '        If covBuildingIndex > buildingIndex Then
                        '            cov.Description = String.Format("{0}.{1}", locationIndex + 1, covBuildingIndex) ' can use covBuildingIndex because it is index and not num so it has already been decremented by 1
                        '        End If
                        '    Next
                        'End If
                    End If
                End If

                'actually remove the building
                If qq.Locations IsNot Nothing Then
                    If locationIndex < qq.Locations.Count Then
                        If qq.Locations(locationIndex).Buildings IsNot Nothing AndAlso buildingIndex < qq.Locations(locationIndex).Buildings.Count Then
                            qq.Locations(locationIndex).Buildings.RemoveAt(buildingIndex)
                        End If
                    End If
                End If

            End If
        End Sub

        Public Shared Sub RemoveFarmLocation(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, locationIndex As Int32)

            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Dim subQuotes = IFM.VR.Common.Helpers.MultiState.SubQuotes(topQuote)
                'Updated 9/10/18 for multi state MLW - QQHelper, parts, SubQuotes, sq - qq to sq
                If subQuotes IsNot Nothing AndAlso subQuotes.Any() Then
                    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In subQuotes
                        'get the suff covs that are tied to this location and remove them
                        If sq.OptionalCoverages IsNot Nothing Then
                            Dim thisLocationsSuffCovs = (From cov In sq.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock AndAlso cov.Description.StartsWith((locationIndex + 1).ToString() + ".") Select cov).ToList()
                            If thisLocationsSuffCovs IsNot Nothing Then
                                For Each c In thisLocationsSuffCovs
                                    sq.OptionalCoverages.Remove(c)
                                Next
                            End If
                        End If
                    Next
                    ''if using quote for location - remove, not using - MLW
                    'Dim quoteToUse = QQHelper.StateQuoteForLocation(SubQuotes, qq.Locations(locationIndex), alwaysReturnQuoteIfPossibleOnNoMatch:=True) 'SubQuoteForLocation(qq.Locations(locationIndex))
                    ''get the suff covs that are tied to this location and remove them
                    'If quoteToUse.OptionalCoverages IsNot Nothing Then
                    '    Dim thisLocationsSuffCovs = (From cov In quoteToUse.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock AndAlso cov.Description.StartsWith((locationIndex + 1).ToString() + ".") Select cov).ToList()
                    '    If thisLocationsSuffCovs IsNot Nothing Then
                    '        For Each c In thisLocationsSuffCovs
                    '            quoteToUse.OptionalCoverages.Remove(c)
                    '        Next
                    '    End If
                    'End If

                End If

                'remove the location
                If topQuote.Locations IsNot Nothing Then
                        If locationIndex < topQuote.Locations.Count Then
                            topQuote.Locations.RemoveAt(locationIndex)
                        End If
                    End If
                End If

        End Sub

    End Class
End Namespace