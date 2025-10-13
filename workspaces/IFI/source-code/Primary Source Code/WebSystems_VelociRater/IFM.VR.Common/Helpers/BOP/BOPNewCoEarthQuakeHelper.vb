Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.MultiState
'Imports Diamond.Business.ThirdParty.XactwareImportAssignmentService

Namespace IFM.VR.Common.Helpers
    Public Class BopNewCoEarthQuakeHelper
        Inherits FeatureFlagBase

        Private Shared _BopNewCoEarthQuakeSettings As NewFlagItem
        Public Shared ReadOnly Property BopNewCoEarthQuakeSettings() As NewFlagItem
            Get
                If _BopNewCoEarthQuakeSettings Is Nothing Then
                    _BopNewCoEarthQuakeSettings = New NewFlagItem("VR_NewCo_BOP_EarthQuake_Settings")
                End If
                Return _BopNewCoEarthQuakeSettings
            End Get
        End Property

        Const BopNewCoEarthQuakeWarningMsg As String = ""
        Const BopNewCoEarthQuakeRemovedMsg As String = ""

        Public Shared Sub UpdateBopNewCoEarthQuake(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Dim qqHelper = New QuickQuoteHelperClass
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Messages
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Messages
            End Select
        End Sub

        Public Shared Function IsBopNewCoEarthQuakeAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then

                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                Dim IsNewCo As Boolean = NewCompanyIdHelper.IsNewCompany(quote)

                BopNewCoEarthQuakeSettings.OtherQualifiers = IsCorrectLOB + IsNewCo

                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, BopNewCoEarthQuakeSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function

        Public Shared Sub ProcessEarthQuake(ByRef Quote As QuickQuoteObject)
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing Then
                For Each location In Quote.Locations
                    If location IsNot Nothing AndAlso location.Buildings IsNot Nothing Then
                        For Each Building In location.Buildings
                            Dim EQSettings = GetEarthQuakeValues(Building.ConstructionId) 'Tuple
                            If IsBopNewCoEarthQuakeAvailable(Quote) Then
                                If SubQuoteFirst(Quote).HasEarthquake Then
                                    Building.EarthquakeBuildingClassificationTypeId = EQSettings.classTypeId
                                    Building.EarthquakeDeductibleId = EQSettings.deductibleId
                                Else
                                    Building.EarthquakeBuildingClassificationTypeId = ""
                                    Building.EarthquakeDeductibleId = ""
                                End If
                            End If
                        Next
                    End If
                Next
            End If

        End Sub

        Public Shared Function GetEarthQuakeValues(constructionType As String) As (classTypeId As String, deductibleId As String)
            Dim EQSettings = (classTypeId:="", deductibleId:="")  'Tuple
            If Not String.IsNullOrWhiteSpace(constructionType) Then
                Select Case constructionType
                    Case "7"
                        EQSettings.classTypeId = "11"
                        EQSettings.deductibleId = "34"
                    Case "12"
                        EQSettings.classTypeId = "24"
                        EQSettings.deductibleId = "36"
                    Case "13"
                        EQSettings.classTypeId = "15"
                        EQSettings.deductibleId = "34"
                    Case "14"
                        EQSettings.classTypeId = "20"
                        EQSettings.deductibleId = "36"
                    Case "15"
                        EQSettings.classTypeId = "23"
                        EQSettings.deductibleId = "36"
                    Case "16"
                        EQSettings.classTypeId = "17"
                        EQSettings.deductibleId = "34"
                End Select
            End If
            'DSD - QuickQuoteBuilding.EarthquakeBuildingClassificationTypeId - Descriptions
            'DSD - Resolves to Location.DeductibleId - 34 - 5%; 36 - 10% 
            Return EQSettings 'Returns a Tuple
        End Function


    End Class

End Namespace