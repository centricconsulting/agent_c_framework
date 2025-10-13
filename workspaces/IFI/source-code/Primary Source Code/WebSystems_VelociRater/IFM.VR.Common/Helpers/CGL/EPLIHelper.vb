Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Imports IFM.VR.Common.Helpers.MultiState

Namespace IFM.VR.Common.Helpers.CGL
    Public Class EPLIHelper

        Public Shared Function EPLI_Is_Applied(topQuote As QuickQuoteObject) As Boolean
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Dim sqf = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote).FirstOrDefault()
                If sqf IsNot Nothing Then
                    Return sqf.HasEPLI AndAlso sqf.EPLICoverageTypeID = "22"
                End If
            End If
            Return False
        End Function

        Public Shared Function EPLINotAvailableDoToClassCode(topQuote As QuickQuoteObject) As Boolean
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Or topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    Dim classCodes As List(Of QuickQuoteGLClassification) = ClassCodeHelper.GetAllPolicyAndLocationClassCodes(topQuote)
                    Dim ineligibleList = System.Configuration.ConfigurationManager.AppSettings("EPLIIneligibleCGLclasscodes").CSVtoList
                    For Each cc As QuickQuoteGLClassification In classCodes
                        If ineligibleList.Contains(cc.ClassCode.Trim()) Then
                            'can not have EPLI
                            Return True
                        End If
                    Next
                End If

                If topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                    Dim ineligibleList = System.Configuration.ConfigurationManager.AppSettings("EPLIIneligibleBOPclasscodeIds").CSVtoList
                    If topQuote.Locations.IsLoaded() Then
                        For Each l In topQuote.Locations
                            If l.Buildings.IsLoaded() Then
                                For Each b In l.Buildings
                                    If b.BuildingClassifications.IsLoaded() Then
                                        For Each c As QuickQuote.CommonObjects.QuickQuoteClassification In b.BuildingClassifications
                                            If ineligibleList.Contains(c.ClassCode) Then
                                                'can not have EPLI
                                                Return True
                                            End If
                                        Next
                                    End If
                                Next
                            End If
                        Next
                    End If
                End If

            End If
            Return False
        End Function

        Public Shared Sub Toggle_EPLI_Is_Applied(topQuote As QuickQuoteObject, applied As Boolean)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                For Each sq In IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote)
                    ' NOTE:  HasEPLI must always be set to true, regardless of whether the EPLI coverage
                    ' checkbox is checked or not.  The only time we would set HasEPLI to False is if the coverage
                    ' was not eligible due to effective date or something like that.  The way QuickQuote works is
                    ' that it applies 'Non-Underwitten" (22), "Opt-Out (19)", or "Ineligible" only if the 
                    ' HasEPLI flag is set to true.  It's a little odd but that is the way it works.  If you 
                    ' try and set the HasEPLI flag based on the checkbox, it'll cause problems in QuickQuote, see 
                    ' Bug 52222 for an example.
                    sq.HasEPLI = True
                    If sq.EPLICoverageTypeID = "" Then
                        If sq.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana OrElse sq.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                            sq.EPLICoverageTypeID = "22" 'non-underwritten
                        ElseIf sq.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                            sq.EPLICoverageTypeID = "19" 'opt-out
                        End If

                    Else
                        ' Re-coded the IF to make it less confusing MGB 11/12/2020
                        If EPLINotAvailableDoToClassCode(topQuote) Then
                            sq.EPLICoverageTypeID = "20" 'ineligible
                            sq.EPLICoverageLimitId = ""
                            sq.EPLIDeductibleId = ""
                        Else
                            If applied Then
                                sq.EPLICoverageTypeID = "22" 'non-underwritten
                            Else
                                sq.EPLICoverageTypeID = "19" 'opt-out
                                sq.EPLICoverageLimitId = ""
                                sq.EPLIDeductibleId = ""
                            End If
                        End If
                    End If

                Next
            End If
        End Sub

        Public Shared Function EPLI_Premium(topQuote As QuickQuoteObject) As String
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Return topQuote.SumPropertyValues(Function() topQuote.EPLIPremium).TryToFormatAsCurreny()
            End If
            Return "$0.00"
        End Function


    End Class
End Namespace
