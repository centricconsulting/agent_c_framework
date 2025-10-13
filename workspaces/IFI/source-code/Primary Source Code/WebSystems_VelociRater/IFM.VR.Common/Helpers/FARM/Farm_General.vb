Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.FARM
    Public Class Farm_General
        Public Shared Function hasAdditionalQuestionsForFarmItemNumberOfUnitsUpdate(Quote As QuickQuoteObject) As Boolean
            'If SwimmingPoolUnitsHelper.isSwimmingPoolUnitsAvailable(Quote) OrElse TrampolineUnitsHelper.isTrampolineUnitsAvailable(Quote) OrElse isWoodStoveEnabled Then Return True
            If SwimmingPoolUnitsHelper.isSwimmingPoolUnitsAvailable(Quote) OrElse TrampolineUnitsHelper.isTrampolineUnitsAvailable(Quote) OrElse WoodburningStoveHelper.IsWoodburningNumOfUnitsAvailable(Quote) Then Return True
            Return False
        End Function

    End Class

End Namespace
