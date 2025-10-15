Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.CIM

    Public Class CIM_Building
        Public Property LocationIndex As Int32
        Public Property BuildingIndex As Int32
        Public Property Building As QuickQuoteBuilding
        Public Property Address As String

        Public Sub New(locationIndex As Int32, buildingIndex As Int32, address As String, building As QuickQuoteBuilding)
            Me.LocationIndex = locationIndex
            Me.BuildingIndex = buildingIndex
            Me.Address = address
            Me.Building = building
        End Sub
    End Class


End Namespace