Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.FARM
    Public Class WoodburningStoveHelper
        Private Shared _WoodburningSettings As NewFlagItem
        Public Shared ReadOnly Property WoodburningSettings() As NewFlagItem
            Get
                If _WoodburningSettings Is Nothing Then
                    _WoodburningSettings = New NewFlagItem("VR_Far_WoodburningUnits_Settings")
                End If
                Return _WoodburningSettings
            End Get
        End Property

        'Added 6/14/2022 for task 72947 MLW
        Const WoodburningWarningMsg As String = "The number of Woodburning Stoves for location one have been set to one. Please update the quote locations page, for all locations, if more than one are present."
        Const WoodBurningRemovedMsg As String = "Rating for Woodburning Stove Surcharge has changed based on the effective date."
        Public Shared Function WoodburningNumOfUnitsEnabled() As Boolean
            Return WoodburningSettings.EnabledFlag
        End Function

        Public Shared Function WoodburningNumOfUnitsEffDate() As Date
            Return WoodburningSettings.GetStartDateOrDefault("9/1/2022")
        End Function

        Public Shared Sub UpdateWoodburningStove(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    For Each l In Quote.Locations
                        If l.WoodOrFuelBurningApplianceSurcharge = True Then
                            NeedsWarningMessage = True
                            If l.WoodOrFuelBurningApplianceSurcharge_NumberOfUnits < 1 Then
                                l.WoodOrFuelBurningApplianceSurcharge_NumberOfUnits = 1
                            End If
                        End If
                    Next
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(WoodburningWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove Number of units, but still keep checked if checked before
                    For Each l In Quote.Locations

                        If Quote.Locations.IndexOf(l) > 0 Then
                            l.WoodOrFuelBurningApplianceSurcharge = False
                        End If

                        l.WoodOrFuelBurningApplianceSurcharge_NumberOfUnits = 0

                        If l.WoodOrFuelBurningApplianceSurcharge Then
                            NeedsWarningMessage = True
                        End If

                    Next
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(WoodBurningRemovedMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
            End Select
        End Sub

        Public Shared Function IsWoodburningNumOfUnitsAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then
                Dim qqh As New QuickQuoteHelperClass
                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)

                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm
                Dim IsCorrectProgramType As Boolean = SubQuoteFirst?.ProgramTypeId = "6" 'FO only

                WoodburningSettings.OtherQualifiers = IsCorrectProgramType AndAlso IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, WoodburningSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function


    End Class
End Namespace
