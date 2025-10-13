Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteScheduledGolfCourse 'added 5/4/2017 for CIM (Golf)
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _ScheduledCoverageNum As String 'for reconciliation
        Private _Address As QuickQuoteAddress
        Private _Description As String
        Private _CoveredHolesFairways As String 'covDetail; int (actually string)
        Private _CoveredHolesGreens As String 'covDetail; int (actually string)
        Private _CoveredHolesTees As String 'covDetail; int (actually string)
        Private _CoveredHolesTrees As String 'covDetail; int (actually string)
        Private _IsFairways As Boolean 'covDetail
        Private _IsGreens As Boolean 'covDetail
        Private _IsTees As Boolean 'covDetail
        Private _IsTrees As Boolean 'covDetail

        Public Property ScheduledCoverageNum As String
            Get
                Return _ScheduledCoverageNum
            End Get
            Set(value As String)
                _ScheduledCoverageNum = value
            End Set
        End Property
        Public Property Address As QuickQuoteAddress
            Get
                SetObjectsParent(_Address)
                Return _Address
            End Get
            Set(value As QuickQuoteAddress)
                _Address = value
                SetObjectsParent(_Address)
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        Public Property CoveredHolesFairways As String 'covDetail; int (actually string)
            Get
                Return _CoveredHolesFairways
            End Get
            Set(value As String)
                _CoveredHolesFairways = value
            End Set
        End Property
        Public Property CoveredHolesGreens As String 'covDetail; int (actually string)
            Get
                Return _CoveredHolesGreens
            End Get
            Set(value As String)
                _CoveredHolesGreens = value
            End Set
        End Property
        Public Property CoveredHolesTees As String 'covDetail; int (actually string)
            Get
                Return _CoveredHolesTees
            End Get
            Set(value As String)
                _CoveredHolesTees = value
            End Set
        End Property
        Public Property CoveredHolesTrees As String 'covDetail; int (actually string)
            Get
                Return _CoveredHolesTrees
            End Get
            Set(value As String)
                _CoveredHolesTrees = value
            End Set
        End Property
        Public Property IsFairways As Boolean 'covDetail
            Get
                Return _IsFairways
            End Get
            Set(value As Boolean)
                _IsFairways = value
            End Set
        End Property
        Public Property IsGreens As Boolean 'covDetail
            Get
                Return _IsGreens
            End Get
            Set(value As Boolean)
                _IsGreens = value
            End Set
        End Property
        Public Property IsTees As Boolean 'covDetail
            Get
                Return _IsTees
            End Get
            Set(value As Boolean)
                _IsTees = value
            End Set
        End Property
        Public Property IsTrees As Boolean 'covDetail
            Get
                Return _IsTrees
            End Get
            Set(value As Boolean)
                _IsTrees = value
            End Set
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()

            _ScheduledCoverageNum = "" 'for reconciliation
            _Address = Nothing
            _Description = ""
            _CoveredHolesFairways = "" 'covDetail; int (actually string)
            _CoveredHolesGreens = "" 'covDetail; int (actually string)
            _CoveredHolesTees = "" 'covDetail; int (actually string)
            _CoveredHolesTrees = "" 'covDetail; int (actually string)
            _IsFairways = False 'covDetail
            _IsGreens = False 'covDetail
            _IsTees = False 'covDetail
            _IsTrees = False 'covDetail
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).

                    qqHelper.DisposeString(_ScheduledCoverageNum) 'for reconciliation
                    If _Address IsNot Nothing Then
                        _Address.Dispose()
                        _Address = Nothing
                    End If
                    qqHelper.DisposeString(_Description)
                    qqHelper.DisposeString(_CoveredHolesFairways) 'covDetail; int (actually string)
                    qqHelper.DisposeString(_CoveredHolesGreens) 'covDetail; int (actually string)
                    qqHelper.DisposeString(_CoveredHolesTees) 'covDetail; int (actually string)
                    qqHelper.DisposeString(_CoveredHolesTrees) 'covDetail; int (actually string)
                    _IsFairways = Nothing 'covDetail
                    _IsGreens = Nothing 'covDetail
                    _IsTees = Nothing 'covDetail
                    _IsTrees = Nothing 'covDetail

                    MyBase.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        'Public Sub Dispose() Implements IDisposable.Dispose
        Public Overrides Sub Dispose()
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
