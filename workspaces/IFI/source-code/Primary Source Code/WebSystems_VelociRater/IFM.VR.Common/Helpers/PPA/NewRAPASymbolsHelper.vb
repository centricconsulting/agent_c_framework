Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.PPA
    Public Class NewRAPASymbolsHelper
        Const symbolsForwardMsg As String = "Changing the effective date to January 1, 2025 or later requires you to select the lookup button for each quoted vehicle to ensure the correct rates and auto symbols are applied when re-rating your quote."

        Private Shared _NewRAPASymbolsSettings As NewFlagItem
        Public Shared ReadOnly Property NewRAPASymbolsSettings() As NewFlagItem
            Get
                If _NewRAPASymbolsSettings Is Nothing Then
                    _NewRAPASymbolsSettings = New NewFlagItem("VR_PPA_NewRAPASymbols_Settings")
                End If
                Return _NewRAPASymbolsSettings
            End Get
        End Property

        Public Shared Function NewRAPASymbolsEnabled() As Boolean
            Return NewRAPASymbolsSettings.EnabledFlag
        End Function

        Public Shared Function NewRAPASymbolsEffDate() As Date
            Return NewRAPASymbolsSettings.GetStartDateOrDefault("1/1/2025")
        End Function

        Public Shared Sub UpdateNewRAPASymbols(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'show message to redo vehicle lookup
                    If ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(symbolsForwardMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove symbols from Vehicle.VehicleSymbols
                    RemoveThreeNewRAPASymbols(Quote)
            End Select
        End Sub

        Public Shared Sub RemoveThreeNewRAPASymbols(Quote As QuickQuoteObject)
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            If Quote IsNot Nothing AndAlso Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                For Each vs In Quote.Vehicles
                    If vs.VehicleSymbols IsNot Nothing AndAlso vs.VehicleSymbols.Count > 0 Then
                        'Remove new vehicles symbols from VehicleSymbol list
                        Dim symbolBI As QuickQuoteVehicleSymbol = (From s In vs.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "8" Select s).FirstOrDefault()
                        If symbolBI IsNot Nothing Then
                            vs.VehicleSymbols.Remove(symbolBI)
                        End If
                        Dim symbolPD As QuickQuoteVehicleSymbol = (From s In vs.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "9" Select s).FirstOrDefault()
                        If symbolPD IsNot Nothing Then
                            vs.VehicleSymbols.Remove(symbolPD)
                        End If
                        Dim symbolMP As QuickQuoteVehicleSymbol = (From s In vs.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "14" Select s).FirstOrDefault()
                        If symbolMP IsNot Nothing Then
                            vs.VehicleSymbols.Remove(symbolMP)
                        End If
                    End If
                Next
            End If
        End Sub

        Public Shared Function IsNewRAPASymbolsAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, NewRAPASymbolsSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
