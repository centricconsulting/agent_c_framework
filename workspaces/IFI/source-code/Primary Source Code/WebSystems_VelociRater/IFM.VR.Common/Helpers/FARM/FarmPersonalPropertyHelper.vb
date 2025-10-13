Namespace IFM.VR.Common.Helpers.FARM
    Public Class FarmPersonalPropertyHelper

        Public Shared Function GetDefaultTextForFarmOptionalCoverage(type As QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType) As String
            Select Case type
                Case QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_4_H_and_FFAAnimals
                    Return "4H and FFA Animals".ToUpper()

            End Select
            Return ""
        End Function

        Public Shared Function GetDefaultTextForFarmPersonalCoverage(type As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType) As String
            Select Case type
                Case QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_DescribedMachinery
                    Return "Farm Machinery Described".ToUpper()
                Case QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.FarmMachineryDescribed_OpenPerils
                    Return "Farm Machinery Described - Open Perils".ToUpper()
                Case QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Irrigation_Equipment
                    Return "Irrigation Equipment".ToUpper()
                Case QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Livestock
                    Return "Livestock".ToUpper()
                Case QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.MiscellaneousFarmPersonalProperty
                    Return "Miscellaneous Farm Personal Property".ToUpper()
                Case QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_Rented_or_borrowed_Equipment
                    Return "Rented or Borrowed Equipment".ToUpper()
                Case QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_MachineryNotDescribed
                    Return "Farm Machinery - Not Described".ToUpper()
                Case QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Grain
                    Return "Grain in Buildings".ToUpper()
                Case QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.GrainintheOpen
                    Return "Grain in the Open".ToUpper()
                Case QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_Barn
                    Return "Hay in Buildings".ToUpper()
                Case QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_the_Open
                    Return "Hay in the Open".ToUpper()
                Case QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.ReproductiveMaterials
                    Return "Reproductive Equipment".ToUpper()
            End Select
            Return ""
        End Function

    End Class
End Namespace