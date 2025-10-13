Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Web.UI.WebControls
Namespace IFM.VR.Common.Helpers.HOM
    'Added 10/10/2022 for bug 76006 MLW
    Public Class StructureTypeManufactureHelper
        Private Shared _StructureTypeManufacturedSettings As NewFlagItem
        Public Shared ReadOnly Property StructureTypeManufacturedSettings() As NewFlagItem
            Get
                If _StructureTypeManufacturedSettings Is Nothing Then
                    _StructureTypeManufacturedSettings = New NewFlagItem("VR_HOM_StructureTypeManufactured_Settings")
                End If
                Return _StructureTypeManufacturedSettings
            End Get
        End Property

        Public Shared Function StructureTypeManufacturedEnabled() As Boolean
            Return StructureTypeManufacturedSettings.EnabledFlag
        End Function

        Public Shared Function StructureTypeManufacturedEffDate() As Date
            Return StructureTypeManufacturedSettings.GetStartDateOrDefault("2/1/2023")
        End Function

        Public Shared Sub UpdateStructureType(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                Select Case CrossDirection
                    Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                        'Set Structure Type to Manufactured if it is already set to Mobile Manufactured
                        If Quote.Locations(0).StructureTypeId = "14" Then
                            Quote.Locations(0).StructureTypeId = "20" 'Manufactured
                        End If
                    Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                        'Set Structure Type to Mobile Manufactured if already set to Manufactured
                        Dim NeedsWarningMessage As Boolean = False
                        If Quote.Locations(0).StructureTypeId = "20"
                            Quote.Locations(0).StructureTypeId = "14" 'Mobile Manufactured                          
                        End If
                End Select
            End if
        End Sub

        Public Shared Function IsStructureTypeManufacturedAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, StructureTypeManufacturedSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

    End Class
End Namespace
