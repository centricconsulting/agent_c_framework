Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Imports IFM.VR.Common.Helpers.MultiState

Namespace IFM.VR.Common.Helpers.CGL

    Public Class CLIHelper

        Public Shared Function CLI_Is_Applied(topQuote As QuickQuoteObject) As Boolean
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Dim sqf = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote).FirstOrDefault()
                'Dim sqf = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote)
                If sqf IsNot Nothing Then
                    Return sqf.CyberLiability And sqf.CyberLiabilityTypeId = "23" 'eligible 
                End If
            End If
            Return False
        End Function

        Public Shared Function CLINotAvailableDoToClassCode(topQuote As QuickQuoteObject) As Boolean
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If

                If topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    Dim classCodes As List(Of QuickQuoteGLClassification) = ClassCodeHelper.GetAllPolicyAndLocationClassCodes(topQuote)
                    Dim ineligibleList = System.Configuration.ConfigurationManager.AppSettings("CLIIneligibleCGLclasscodes").CSVtoList
                    For Each cc As QuickQuoteGLClassification In classCodes
                        If ineligibleList.Contains(cc.ClassCode.Trim()) Then
                            'can not have CLI
                            Return True
                        End If
                    Next
                End If

                If topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                    Dim ineligibleList = System.Configuration.ConfigurationManager.AppSettings("CLIIneligibleBOPclasscodeIds").CSVtoList
                    If topQuote.Locations.IsLoaded() Then
                        For Each l In topQuote.Locations
                            If l.Buildings.IsLoaded() Then
                                For Each b In l.Buildings
                                    If b.BuildingClassifications.IsLoaded() Then
                                        For Each c As QuickQuote.CommonObjects.QuickQuoteClassification In b.BuildingClassifications
                                            If ineligibleList.Contains(c.ClassCode) Then
                                                'can not have CLI
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

        Public Shared Sub Toggle_CLI_Is_Applied(topQuote As QuickQuoteObject, applied As Boolean)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                For Each sq In IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote)
                    sq.CyberLiability = True 'always true unless the coverage isn't available due to date of effective date
                    If sq.CyberLiabilityTypeId = "" Then
                        If topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage OrElse topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                            If sq.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana OrElse sq.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                                sq.CyberLiabilityTypeId = "23" 'eligible
                                sq.CyberLiabilityLimitId = "261" '50,000
                                sq.CyberLiabilityDeductibleId = "15" '2,500
                            ElseIf sq.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                                sq.CyberLiabilityTypeId = "24" 'opt-out
                            End If
                        End If
                    Else
                        If CLINotAvailableDoToClassCode(topQuote) = False Then
                            If applied Then
                                sq.CyberLiabilityTypeId = "23" 'eligible
                                sq.CyberLiabilityLimitId = "261" '50,000
                                sq.CyberLiabilityDeductibleId = "15" '2,500
                            Else
                                sq.CyberLiabilityTypeId = "24" 'opt-out
                                sq.CyberLiabilityLimitId = ""
                                sq.CyberLiabilityDeductibleId = ""
                            End If
                        Else
                            sq.CyberLiabilityTypeId = "20" 'ineligible
                            sq.CyberLiabilityLimitId = ""
                            sq.CyberLiabilityDeductibleId = ""
                        End If
                    End If
                Next
            End If
        End Sub

    End Class
End Namespace
