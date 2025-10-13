Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()> _
    Public Class QuickQuoteChoicePointUnitNumCollectionSet 'added 5/7/2014; 8/4/2014 note: will not be inheriting QuickQuoteBaseObject
        Implements IDisposable

        Private _UnitNum As Integer
        Private _LossHistories As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Policy.LossHistory)
        Private _AccidentsViolations As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Policy.AccidentViolation)
        Private _QuickQuoteNum As Integer
        'added 5/12/2014 so we can see what type of transmissions are included
        Private _HasMvrTransmission As Boolean
        Private _HasClueAutoTransmission As Boolean
        Private _HasCluePropertyTransmission As Boolean 'added 9/11/2014
        Private _PolicyHasAnyMvrTransmissions As Boolean
        Private _PolicyHasAnyClueAutoTransmissions As Boolean '9/11/2014 note: renamed from Clue to ClueAuto
        Private _PolicyHasAnyCluePropertyTransmissions As Boolean 'added 9/11/2014

        Public Property UnitNum As Integer
            Get
                Return _UnitNum
            End Get
            Set(value As Integer)
                _UnitNum = value
            End Set
        End Property
        Public Property LossHistories As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Policy.LossHistory)
            Get
                Return _LossHistories
            End Get
            Set(value As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Policy.LossHistory))
                _LossHistories = value
            End Set
        End Property
        Public Property AccidentsViolations As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Policy.AccidentViolation)
            Get
                Return _AccidentsViolations
            End Get
            Set(value As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Policy.AccidentViolation))
                _AccidentsViolations = value
            End Set
        End Property
        Public Property QuickQuoteNum As Integer
            Get
                Return _QuickQuoteNum
            End Get
            Set(value As Integer)
                _QuickQuoteNum = value
            End Set
        End Property
        'added 5/12/2014
        Public Property HasMvrTransmission As Boolean
            Get
                Return _HasMvrTransmission
            End Get
            Set(value As Boolean)
                _HasMvrTransmission = value
            End Set
        End Property
        Public Property HasClueAutoTransmission As Boolean
            Get
                Return _HasClueAutoTransmission
            End Get
            Set(value As Boolean)
                _HasClueAutoTransmission = value
            End Set
        End Property
        Public Property HasCluePropertyTransmission As Boolean 'added 9/11/2014
            Get
                Return _HasCluePropertyTransmission
            End Get
            Set(value As Boolean)
                _HasCluePropertyTransmission = value
            End Set
        End Property
        Public Property PolicyHasAnyMvrTransmissions As Boolean
            Get
                Return _PolicyHasAnyMvrTransmissions
            End Get
            Set(value As Boolean)
                _PolicyHasAnyMvrTransmissions = value
            End Set
        End Property
        Public Property PolicyHasAnyClueAutoTransmissions As Boolean '9/11/2014 note: renamed from Clue to ClueAuto
            Get
                Return _PolicyHasAnyClueAutoTransmissions
            End Get
            Set(value As Boolean)
                _PolicyHasAnyClueAutoTransmissions = value
            End Set
        End Property
        Public Property PolicyHasAnyCluePropertyTransmissions As Boolean 'added 9/11/2014
            Get
                Return _PolicyHasAnyCluePropertyTransmissions
            End Get
            Set(value As Boolean)
                _PolicyHasAnyCluePropertyTransmissions = value
            End Set
        End Property

        Public Sub New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _UnitNum = 0
            _LossHistories = Nothing
            _AccidentsViolations = Nothing
            _QuickQuoteNum = 0
            'added 5/12/2014
            _HasMvrTransmission = False
            _HasClueAutoTransmission = False
            _HasCluePropertyTransmission = False 'added 9/11/2014
            _PolicyHasAnyMvrTransmissions = False
            _PolicyHasAnyClueAutoTransmissions = False '9/11/2014 note: renamed from Clue to ClueAuto
            _PolicyHasAnyCluePropertyTransmissions = False 'added 9/11/2014
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    _UnitNum = Nothing
                    If _LossHistories IsNot Nothing Then
                        If _LossHistories.Count > 0 Then
                            For Each lh As Diamond.Common.Objects.Policy.LossHistory In _LossHistories
                                lh.Dispose()
                                lh = Nothing
                            Next
                            _LossHistories.Clear()
                        End If
                        _LossHistories = Nothing
                    End If
                    If _AccidentsViolations IsNot Nothing Then
                        If _AccidentsViolations.Count > 0 Then
                            For Each av As Diamond.Common.Objects.Policy.AccidentViolation In _AccidentsViolations
                                av.Dispose()
                                av = Nothing
                            Next
                            _AccidentsViolations.Clear()
                        End If
                        _AccidentsViolations = Nothing
                    End If
                    _QuickQuoteNum = Nothing
                    'added 5/12/2014
                    _HasMvrTransmission = Nothing
                    _HasClueAutoTransmission = Nothing
                    _HasCluePropertyTransmission = Nothing 'added 9/11/2014
                    _PolicyHasAnyMvrTransmissions = Nothing
                    _PolicyHasAnyClueAutoTransmissions = Nothing '9/11/2014 note: renamed from Clue to ClueAuto
                    _PolicyHasAnyCluePropertyTransmissions = Nothing 'added 9/11/2014
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
