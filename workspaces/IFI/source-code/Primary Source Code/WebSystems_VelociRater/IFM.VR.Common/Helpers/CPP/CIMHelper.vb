Imports Microsoft.VisualBasic
Imports QuickQuote.CommonObjects 'added 4/5/2017

    Public Class CIMHelper


        Public Shared Function BuildersRiskRateTable() As SuggestedRateTable
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(0.75, Double.MinValue, 100000))
            table.Add(New SuggestedRateTableItem(0.6, 100001, 250000))
            table.Add(New SuggestedRateTableItem(0.5, 250001, 500000))
            table.Add(New SuggestedRateTableItem(0.45, 500001, 1000000))
            table.Add(New SuggestedRateTableItem(0.4, 1000001, Double.MaxValue))
            Return table
        End Function


        Public Shared Function ComputerHardwareRateTable() As SuggestedRateTable
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(0.65, Double.MinValue, 10000))
            table.Add(New SuggestedRateTableItem(0.6, 10001, 25000))
            table.Add(New SuggestedRateTableItem(0.5, 25001, 50000))
            table.Add(New SuggestedRateTableItem(0.45, 50001, 100000))
            table.Add(New SuggestedRateTableItem(0.4, 100001, Double.MaxValue))
            Return table
        End Function

        Public Shared Function ComputerProgramsMediaRateTable() As SuggestedRateTable
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(0.55, Double.MinValue, 10000))
            table.Add(New SuggestedRateTableItem(0.5, 10001, 25000))
            table.Add(New SuggestedRateTableItem(0.4, 25001, 50000))
            table.Add(New SuggestedRateTableItem(0.35, 50001, 100000))
            table.Add(New SuggestedRateTableItem(0.3, 100001, Double.MaxValue))
            Return table
        End Function

        Public Shared Function ComputerBusinessIncomeRateTable() As SuggestedRateTable
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(0.55, Double.MinValue, 10000))
            table.Add(New SuggestedRateTableItem(0.5, 10001, 25000))
            table.Add(New SuggestedRateTableItem(0.45, 25001, 50000))
            table.Add(New SuggestedRateTableItem(0.4, 50001, 100000))
            table.Add(New SuggestedRateTableItem(0.35, 100001, Double.MaxValue))
            Return table
        End Function


        Public Shared Function ContractorEquipmentScheduledItemsRateTable() As SuggestedRateTable
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(1.5, Double.MinValue, 10000))
            table.Add(New SuggestedRateTableItem(1.35, 10001, 25000))
            table.Add(New SuggestedRateTableItem(1.25, 25001, 50000))
            table.Add(New SuggestedRateTableItem(1.15, 50001, 100000))
            table.Add(New SuggestedRateTableItem(1, 100001, Double.MaxValue))
            Return table
        End Function

        Public Shared Function ContractorEquipmentUnScheduledToolsRateTable() As SuggestedRateTable
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(2.5, Double.MinValue, Double.MaxValue)) ' will always send back 2.5
            Return table
        End Function

        Public Shared Function ContractorsEquipmentLeasedRentedFromOthersRateTable() As SuggestedRateTable ' Added 7-15-15 Matt A Bug#5093
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(1.0, Double.MinValue, Double.MaxValue)) ' will always send back 2.5
            Return table
        End Function



        Public Shared Function FineArtsRateTable() As SuggestedRateTable
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(1.2, Double.MinValue, 10000))
            table.Add(New SuggestedRateTableItem(1.0, 10001, 25000))
            table.Add(New SuggestedRateTableItem(0.75, 25001, 50000))
            table.Add(New SuggestedRateTableItem(0.6, 50001, 100000))
            table.Add(New SuggestedRateTableItem(0.4, 100001, Double.MaxValue))
            Return table
        End Function


        Public Shared Function InstallationFloaterRateTable() As SuggestedRateTable
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(1.6, Double.MinValue, 10000))
            table.Add(New SuggestedRateTableItem(1.1, 10001, 25000))
            table.Add(New SuggestedRateTableItem(1.0, 25001, 50000))
            table.Add(New SuggestedRateTableItem(0.7, 50001, 100000))
            table.Add(New SuggestedRateTableItem(0.55, 100001, Double.MaxValue))
            Return table
        End Function


        Public Shared Function OwnerCargoRateTable() As SuggestedRateTable
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(2.0, Double.MinValue, 10000))
            table.Add(New SuggestedRateTableItem(1.75, 10001, 25000))
            table.Add(New SuggestedRateTableItem(1.5, 25001, 50000))
            table.Add(New SuggestedRateTableItem(1.25, 50001, 100000))
            table.Add(New SuggestedRateTableItem(1.0, 100001, Double.MaxValue))
            Return table
        End Function


        Public Shared Function ScheduledPropertyRateTable() As SuggestedRateTable
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(2.0, Double.MinValue, 10000))
            table.Add(New SuggestedRateTableItem(1.5, 10001, 25000))
            table.Add(New SuggestedRateTableItem(1.4, 25001, 50000))
            table.Add(New SuggestedRateTableItem(1.25, 50001, 100000))
            table.Add(New SuggestedRateTableItem(1.0, 100001, Double.MaxValue))
            Return table
        End Function

        Public Shared Function TransportationRateTable() As SuggestedRateTable
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(2.0, Double.MinValue, 10000))
            table.Add(New SuggestedRateTableItem(1.75, 10001, 25000))
            table.Add(New SuggestedRateTableItem(1.5, 25001, 50000))
            table.Add(New SuggestedRateTableItem(1.25, 50001, 100000))
            table.Add(New SuggestedRateTableItem(1.0, 100001, Double.MaxValue))
            Return table
        End Function

        Public Shared Function MotorTruckCargoRateTable() As SuggestedRateTable
            Dim table As New SuggestedRateTable
            table.Add(New SuggestedRateTableItem(2.0, Double.MinValue, 10000))
            table.Add(New SuggestedRateTableItem(1.75, 10001, 25000))
            table.Add(New SuggestedRateTableItem(1.5, 25001, 50000))
            table.Add(New SuggestedRateTableItem(1.25, 50001, 100000))
            table.Add(New SuggestedRateTableItem(1.0, 100001, Double.MaxValue))
            Return table
        End Function


    End Class

    Public Class SuggestedRateTable
        Inherits List(Of SuggestedRateTableItem)

        '

        Public Function GetRateForLimit(limit As Double) As Double
            For Each range In Me
                If limit >= range.LowerBound AndAlso limit <= range.UpperBound Then
                    Return range.SuggestedRate
                End If
            Next
            Return 0.0
        End Function

    End Class

    Public Class SuggestedRateTableItem
        Public Property SuggestedRate As Double
        Public Property LowerBound As Double
        Public Property UpperBound As Double
        Public Sub New()

        End Sub
        Public Sub New(rate As Double, lowerbound As Double, upperbound As Double)
            Me.SuggestedRate = rate
            Me.LowerBound = lowerbound
            Me.UpperBound = upperbound
        End Sub
    End Class

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

