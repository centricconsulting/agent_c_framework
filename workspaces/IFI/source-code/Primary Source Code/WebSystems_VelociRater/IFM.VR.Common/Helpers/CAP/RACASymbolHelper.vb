Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.CAP
    Public Class RACASymbolHelper
        Inherits FeatureFlagBase

        Public Const notFoundSymbolCode As String = "EX"
        Private Const VehicleCompSymbolTypeId = "1"
        Private Const VehicleCollSymbolTypeId = "2"
        Private Const VehicleLiabSymbolTypeId = "3"
        Private Shared _RACASymbolsSettings As NewFlagItem
        Public Shared ReadOnly Property RACASymbolsSettings() As NewFlagItem
            Get
                If _RACASymbolsSettings Is Nothing Then
                    _RACASymbolsSettings = New NewFlagItem("VR_CAP_RACASymbols_Settings")
                End If
                Return _RACASymbolsSettings
            End Get
        End Property

        Public Shared Function RACASymbolsEnabled() As Boolean
            Return RACASymbolsSettings.EnabledFlag
        End Function

        Public Shared Function RACASymbolsEffDate() As Date
            Return RACASymbolsSettings.GetStartDateOrDefault("2/1/2026")
        End Function

        Public Shared Sub UpdateRACASymbols(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            If Quote IsNot Nothing Then
                Dim SubQuotes As List(Of QuickQuoteObject) = MultiState.General.SubQuotes(Quote)
                Select Case CrossDirection
                    Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                        If Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                            For Each vehicle In Quote.Vehicles
                                If vehicle.Vin IsNot Nothing AndAlso vehicle.Vin.Length = 17 Then
                                    Dim cih As New Helpers.IntegrationHelper
                                    Dim RACASymbolsResults As New List(Of RACASymbolsLookupResult)
                                    RACASymbolsResults = cih.GetRACASymbolsForVin(vehicle.Vin)
                                    If RACASymbolsResults IsNot Nothing AndAlso RACASymbolsResults.Count > 0 Then
                                        If vehicle.VehicleSymbols Is Nothing Then
                                            vehicle.VehicleSymbols = New List(Of QuickQuoteVehicleSymbol)
                                        End If
                                        For Each symbol In RACASymbolsResults
                                            RACASymbolHelper.AddUpdateRACASymbols(vehicle, VehicleCompSymbolTypeId, symbol.ComprehensiveSymbol, symbol.ComprehensiveSymbol)
                                            RACASymbolHelper.AddUpdateRACASymbols(vehicle, VehicleCollSymbolTypeId, symbol.CollisionSymbol, symbol.CollisionSymbol)
                                            RACASymbolHelper.AddUpdateRACASymbols(vehicle, VehicleLiabSymbolTypeId, symbol.LiabilitySymbol, symbol.LiabilitySymbol)
                                            Exit For
                                        Next
                                    Else
                                        'Symbols are EX
                                        RACASymbolHelper.AddUpdateRACASymbols(vehicle, VehicleCompSymbolTypeId, notFoundSymbolCode, notFoundSymbolCode)
                                        RACASymbolHelper.AddUpdateRACASymbols(vehicle, VehicleCollSymbolTypeId, notFoundSymbolCode, notFoundSymbolCode)
                                        RACASymbolHelper.AddUpdateRACASymbols(vehicle, VehicleLiabSymbolTypeId, notFoundSymbolCode, notFoundSymbolCode)
                                    End If
                                Else
                                    'Symbols are EX
                                    RACASymbolHelper.AddUpdateRACASymbols(vehicle, VehicleCompSymbolTypeId, notFoundSymbolCode, notFoundSymbolCode)
                                    RACASymbolHelper.AddUpdateRACASymbols(vehicle, VehicleCollSymbolTypeId, notFoundSymbolCode, notFoundSymbolCode)
                                    RACASymbolHelper.AddUpdateRACASymbols(vehicle, VehicleLiabSymbolTypeId, notFoundSymbolCode, notFoundSymbolCode)
                                End If
                            Next
                        End If

                    Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                        'Do Nothing
                End Select
            End If
        End Sub

        Public Shared Function IsRACASymbolsAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                RACASymbolsSettings.OtherQualifiers = IsCorrectLOB

                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, RACASymbolsSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

        Public Shared Sub AddUpdateRACASymbols(vehicle As QuickQuoteVehicle, typeId As String, symbolValue As String, overrideValue As String)
            Dim symbol = (From s In vehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = typeId Select s).FirstOrDefault()
            If symbol Is Nothing Then
                symbol = vehicle.VehicleSymbols.AddNew()
                symbol.VehicleSymbolCoverageTypeId = typeId
            End If
            If IsNullEmptyorWhitespace(vehicle.Vin) OrElse vehicle.Vin.Length <> 17 OrElse (vehicle.ClassCode IsNot Nothing AndAlso Left(vehicle.ClassCode, 1) = "6") OrElse vehicle.Year.TryToGetInt32() < 1981 Then
                'Vin lookup will not return results (and therefore not show the select button that triggers the RACA symbols call) for vehicles without a vin, short vin, 1980 or older, and trailers/class codes begin with 6)
                symbolValue = notFoundSymbolCode
                overrideValue = notFoundSymbolCode
            End If
            symbol.SystemGeneratedSymbol = symbolValue
            symbol.UserOverrideSymbol = overrideValue
        End Sub

    End Class
End Namespace
