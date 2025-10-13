'Imports Microsoft.VisualBasic

'Public Class QuickQuoteAndDiamondGenericHolderClasses

'End Class

Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class DiamondLobAndVariousProperties 'added 9/11/2018

        Public Enum ImageOrPackagePartLevel
            None = 0
            ImageLevel = 1
            PackagePartLevel = 2
        End Enum

        Public Property VersionId As Integer = 0
        Public Property StateId As Integer = 0
        Public Property LobId As Integer = 0
        Public Property ActualLobId As Integer = 0
        Public Property LobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None
        Public Property DiamondLob As Diamond.Common.Objects.Policy.LOB = Nothing
        Public Property Level As ImageOrPackagePartLevel = ImageOrPackagePartLevel.None


        Public Sub Reset()
            _VersionId = 0
            _StateId = 0
            _LobId = 0
            _ActualLobId = 0
            _LobType = QuickQuoteObject.QuickQuoteLobType.None
            _DiamondLob = Nothing
            _Level = ImageOrPackagePartLevel.None
        End Sub
        Public Sub Dispose()
            _VersionId = Nothing
            _StateId = Nothing
            _LobId = Nothing
            _ActualLobId = Nothing
            _LobType = Nothing
            If _DiamondLob IsNot Nothing Then
                _DiamondLob.Dispose()
                _DiamondLob = Nothing
            End If
            _Level = Nothing
        End Sub
        Public Function AppearsToBeMasterLob() As Boolean
            If _LobId = _ActualLobId Then
                Return False
            Else
                Return True
            End If
        End Function
        Public Function QuickQuoteState() As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            Return QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteStateForDiamondStateId(_StateId)
        End Function
    End Class

    <Serializable()>
    Public Class QuickQuoteLocationAndOrderingProperties 'added 9/17/2018
        Implements IComparable(Of QuickQuoteLocationAndOrderingProperties)

        Public Property QQLocation As QuickQuoteLocation = Nothing
        Public Property AddressPositionMatches As List(Of Integer) = Nothing
        Public Property OriginalOrder As Integer = 0
        Public Property NewOrder As Integer = 0

        Public Sub Reset()
            _QQLocation = Nothing
            _AddressPositionMatches = Nothing
            _OriginalOrder = 0
            _NewOrder = 0
        End Sub
        Public Sub Dispose()
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            qqHelper.DisposeLocation(_QQLocation)
            qqHelper.DisposeIntegers(_AddressPositionMatches)
            _OriginalOrder = Nothing
            _NewOrder = Nothing
        End Sub

        Public Function CompareTo(other As QuickQuoteLocationAndOrderingProperties) As Integer Implements IComparable(Of QuickQuoteLocationAndOrderingProperties).CompareTo
            'Throw New NotImplementedException()

            If Me Is Nothing AndAlso other Is Nothing Then
                Return 0
            ElseIf Me Is Nothing Then
                Return -1
            ElseIf other Is Nothing Then
                Return 1
            Else
                If Me.NewOrder = other.NewOrder Then
                    If Me.OriginalOrder = other.OriginalOrder Then
                        Return 0
                    Else
                        Return Me.OriginalOrder.CompareTo(other.OriginalOrder)
                    End If
                Else
                    Return Me.NewOrder.CompareTo(other.NewOrder)
                End If
            End If
        End Function
    End Class
    Public Class QuickQuoteLocationAndOrderingPropertiesComparer 'added 9/18/2018
        Implements IComparer(Of QuickQuoteLocationAndOrderingProperties)

        Public Function Compare(x As QuickQuoteLocationAndOrderingProperties, y As QuickQuoteLocationAndOrderingProperties) As Integer Implements IComparer(Of QuickQuoteLocationAndOrderingProperties).Compare
            'Throw New NotImplementedException()

            If x Is Nothing AndAlso y Is Nothing Then
                Return 0
            ElseIf x Is Nothing Then
                Return -1
            ElseIf y Is Nothing Then
                Return 1
            Else
                If x.NewOrder = y.NewOrder Then
                    If x.OriginalOrder = y.OriginalOrder Then
                        Return 0
                    Else
                        Return x.OriginalOrder.CompareTo(y.OriginalOrder)
                    End If
                Else
                    Return x.NewOrder.CompareTo(y.NewOrder)
                End If
            End If
        End Function
    End Class

    <Serializable()>
    Public Class QuickQuoteVehicleAndOrderingProperties 'added 9/17/2018
        Implements IComparable(Of QuickQuoteVehicleAndOrderingProperties)

        Public Property QQVehicle As QuickQuoteVehicle = Nothing
        Public Property AddressPositionMatches As List(Of Integer) = Nothing
        Public Property YearMakeModelPositionMatches As List(Of Integer) = Nothing
        Public Property FullPositionMatches As List(Of Integer) = Nothing
        Public Property OriginalOrder As Integer = 0
        Public Property NewOrder As Integer = 0

        Public Sub Reset()
            _QQVehicle = Nothing
            _AddressPositionMatches = Nothing
            _YearMakeModelPositionMatches = Nothing
            _FullPositionMatches = Nothing
            _OriginalOrder = 0
            _NewOrder = 0
        End Sub
        Public Sub Dispose()
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            qqHelper.DisposeVehicle(_QQVehicle)
            qqHelper.DisposeIntegers(_AddressPositionMatches)
            qqHelper.DisposeIntegers(_YearMakeModelPositionMatches)
            qqHelper.DisposeIntegers(_FullPositionMatches)
            _OriginalOrder = Nothing
            _NewOrder = Nothing
        End Sub

        Public Function CompareTo(other As QuickQuoteVehicleAndOrderingProperties) As Integer Implements IComparable(Of QuickQuoteVehicleAndOrderingProperties).CompareTo
            'Throw New NotImplementedException()

            If Me Is Nothing AndAlso other Is Nothing Then
                Return 0
            ElseIf Me Is Nothing Then
                Return -1
            ElseIf other Is Nothing Then
                Return 1
            Else
                If Me.NewOrder = other.NewOrder Then
                    If Me.OriginalOrder = other.OriginalOrder Then
                        Return 0
                    Else
                        Return Me.OriginalOrder.CompareTo(other.OriginalOrder)
                    End If
                Else
                    Return Me.NewOrder.CompareTo(other.NewOrder)
                End If
            End If
        End Function
    End Class
    Public Class QuickQuoteVehicleAndOrderingPropertiesComparer 'added 9/18/2018
        Implements IComparer(Of QuickQuoteVehicleAndOrderingProperties)

        Public Function Compare(x As QuickQuoteVehicleAndOrderingProperties, y As QuickQuoteVehicleAndOrderingProperties) As Integer Implements IComparer(Of QuickQuoteVehicleAndOrderingProperties).Compare
            'Throw New NotImplementedException()

            If x Is Nothing AndAlso y Is Nothing Then
                Return 0
            ElseIf x Is Nothing Then
                Return -1
            ElseIf y Is Nothing Then
                Return 1
            Else
                If x.NewOrder = y.NewOrder Then
                    If x.OriginalOrder = y.OriginalOrder Then
                        Return 0
                    Else
                        Return x.OriginalOrder.CompareTo(y.OriginalOrder)
                    End If
                Else
                    Return x.NewOrder.CompareTo(y.NewOrder)
                End If
            End If
        End Function
    End Class

    <Serializable()>
    Public Class PackagePartTypeStateAndLobProperties 'added 12/5/2018

        Public Property PackagePartTypeId As Integer = 0
        Public Property PartStateId As Integer = 0
        Public Property PartLobId As Integer = 0
        Public Property ParentStateId As Integer = 0
        Public Property ParentLobId As Integer = 0


        Public Sub Reset()
            _PackagePartTypeId = 0
            _PartStateId = 0
            _PartLobId = 0
            _ParentStateId = 0
            _ParentLobId = 0
        End Sub
        Public Sub Dispose()
            _PackagePartTypeId = Nothing
            _PartStateId = Nothing
            _PartLobId = Nothing
            _ParentStateId = Nothing
            _ParentLobId = Nothing
        End Sub
        Public Function AppearsToBeMasterPart() As Boolean
            If _ParentStateId = 0 AndAlso _ParentLobId = 0 AndAlso _PartStateId > 0 AndAlso _PartLobId > 0 Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class
End Namespace
