Imports IFM.VR.Common.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports IFM.VR.Common.Helpers.CPR
Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption


Namespace IFM.VR.Common.Helpers.CPP
    Public Class OwnerOccupiedPercentageFieldHelper
        Private Shared _OwnerOccupiedPercentageFieldSettings As NewFlagItem
        Public Shared ReadOnly Property OwnerOccupiedPercentageFieldSettings() As NewFlagItem
            Get
                If _OwnerOccupiedPercentageFieldSettings Is Nothing Then
                    _OwnerOccupiedPercentageFieldSettings = New NewFlagItem("VR_CPP_CPR_OwnerOccupiedPercentageField_Settings")
                End If
                Return _OwnerOccupiedPercentageFieldSettings
            End Get
        End Property

        Const OwnerOccupiedPercentageFieldUpdatedMsg As String = ""

        Public Shared Function OwnerOccupiedPercentageFieldEnabled() As Boolean
            Return OwnerOccupiedPercentageFieldSettings.EnabledFlag
        End Function

        Public Shared Function OwnerOccupiedPercentageFieldEffDate() As Date
            Return OwnerOccupiedPercentageFieldSettings.GetStartDateOrDefault("3/15/2025")
        End Function

        Public Shared Sub UpdateOwnerOccupiedPercentageField(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            If Quote IsNot Nothing Then
                Select Case CrossDirection
                    Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    ' Not necessary in this scenario
                    Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                        ' Loop through all the buildings on the quote
                        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                            For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                                If L.Buildings IsNot Nothing AndAlso L.Buildings.Count > 0 Then
                                    For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                                        If B.OwnerOccupiedPercentageId IsNot Nothing Then
                                            B.OwnerOccupiedPercentageId = "-1"
                                        End If
                                    Next
                                End If
                            Next
                        End If
                End Select
            End If
        End Sub

        Public Shared Function IsOwnerOccupiedPercentageFieldAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage _
                    OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                OwnerOccupiedPercentageFieldSettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, OwnerOccupiedPercentageFieldSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace


