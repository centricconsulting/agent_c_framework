Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.AllLines
    Public Class IRPM_ClasscodeCheck

        Private Shared Function GetCodesFromKey(key As String) As List(Of String)
            Dim chc As New CommonHelperClass
            Dim codeList As List(Of String)
            codeList = CSVtoList(chc.ConfigurationAppSettingValueAsString(key))
            If codeList IsNot Nothing Then
                Return codeList
            Else
                Return New List(Of String)
            End If
        End Function

        Private Shared Function GetClassCodesByLob(lob As QuickQuoteObject.QuickQuoteLobType) As List(Of String)
            Dim codeList As New List(Of String)
            Select Case lob
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    codeList = GetCodesFromKey("bop_NoIRPM_Classcodes")
                Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                    codeList = GetCodesFromKey("cpr_NoIRPM_Classcodes")
                Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    codeList = GetCodesFromKey("cgl_NoIRPM_Classcodes")
                Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    codeList = GetCodesFromKey("cpr_NoIRPM_Classcodes")
                    codeList.AddRange(GetCodesFromKey("cgl_NoIRPM_Classcodes"))
            End Select
            Return codeList
        End Function

        Public Shared Function IsUnwantedClassCodePresent(quote As QuickQuoteObject) As Boolean
            Dim result As Boolean = False
            If quote IsNot Nothing Then
                Dim UnwantedCCList As New List(Of String)
                UnwantedCCList = GetClassCodesByLob(quote.LobType)

                Select Case quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                        result = CheckBOPforBadCodes(quote, UnwantedCCList)
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                        result = CheckCPRforBadCodes(quote, UnwantedCCList)
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                        result = CheckCGLPolicyforBadCodes(quote, UnwantedCCList) OrElse
                            CheckCGLLocationsforBadCodes(quote, UnwantedCCList)
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        result = CheckCPRforBadCodes(quote, UnwantedCCList) OrElse
                            CheckCGLPolicyforBadCodes(quote, UnwantedCCList) OrElse
                            CheckCGLLocationsforBadCodes(quote, UnwantedCCList)
                End Select

            End If
            Return result
        End Function

        Private Shared Function CheckBOPforBadCodes(quote As QuickQuoteObject, UnwantedCCList As List(Of String)) As Boolean
            If quote.Locations IsNot Nothing Then
                For Each location As QuickQuoteLocation In quote.Locations
                    If location.Buildings IsNot Nothing Then
                        For Each building As QuickQuoteBuilding In location.Buildings
                            If building.BuildingClassifications IsNot Nothing Then
                                For Each bc As QuickQuoteClassification In building.BuildingClassifications
                                    If UnwantedCCList.Contains(bc.ClassCode) Then
                                        Return True
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next
            End If
            Return False
        End Function

        Private Shared Function CheckCPRforBadCodes(quote As QuickQuoteObject, UnwantedCCList As List(Of String)) As Boolean
            If quote.Locations IsNot Nothing Then
                For Each location As QuickQuoteLocation In quote.Locations
                    If location.Buildings IsNot Nothing Then
                        For Each building As QuickQuoteBuilding In location.Buildings
                            If building.ClassificationCode IsNot Nothing Then
                                If UnwantedCCList.Contains(building.ClassificationCode.ClassCode) Then
                                    Return True
                                End If
                            End If

                        Next
                    End If
                Next
            End If
            Return False
        End Function

        Private Shared Function CheckCGLPolicyforBadCodes(quote As QuickQuoteObject, UnwantedCCList As List(Of String)) As Boolean
            If quote.GLClassifications IsNot Nothing Then
                For Each cc As QuickQuoteGLClassification In quote.GLClassifications
                    If UnwantedCCList.Contains(cc.ClassCode) Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Function

        Private Shared Function CheckCGLLocationsforBadCodes(quote As QuickQuoteObject, UnwantedCCList As List(Of String)) As Boolean
            If quote.Locations IsNot Nothing Then
                For Each location As QuickQuoteLocation In quote.Locations
                    If location.GLClassifications IsNot Nothing Then
                        For Each cc As QuickQuoteGLClassification In location.GLClassifications
                            If UnwantedCCList.Contains(cc.ClassCode) Then
                                Return True
                            End If
                        Next
                    End If
                Next
            End If
            Return False
        End Function

        ''' <summary>
        ''' Convert's a CSV to a List and does not require the Comma (in case there is a single item in the list)
        ''' </summary>
        ''' <param name="csvText">Comma seperated string</param>
        ''' <returns>List of string items</returns>
        Private Shared Function CSVtoList(csvText As String) As List(Of String)
            If String.IsNullOrWhiteSpace(csvText) = False Then
                Return csvText.Split(Convert.ToChar(",")).ToList()
            End If

            Return New List(Of String)
        End Function






















    End Class
End Namespace
