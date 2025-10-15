Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store drivers license information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteDriversLicenseInfo 'added 9/18/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Classification As String
        Private _DLDate As String
        Private _DriversLicenseNumber As String
        Private _DriversLicenseState As String
        Private _UnitNumber As String

        Public Property Classification As String
            Get
                Return _Classification
            End Get
            Set(value As String)
                _Classification = value
            End Set
        End Property
        Public Property DLDate As String
            Get
                Return _DLDate
            End Get
            Set(value As String)
                _DLDate = value
                qqHelper.ConvertToShortDate(_DLDate)
            End Set
        End Property
        Public Property DriversLicenseNumber As String
            Get
                Return _DriversLicenseNumber
            End Get
            Set(value As String)
                _DriversLicenseNumber = value
            End Set
        End Property
        Public Property DriversLicenseState As String
            Get
                Return _DriversLicenseState
            End Get
            Set(value As String)
                _DriversLicenseState = value
            End Set
        End Property
        Public Property UnitNumber As String
            Get
                Return _UnitNumber
            End Get
            Set(value As String)
                _UnitNumber = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Classification = ""
            _DLDate = ""
            _DriversLicenseNumber = ""
            _DriversLicenseState = ""
            _UnitNumber = ""
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _Classification IsNot Nothing Then
                        _Classification = Nothing
                    End If
                    If _DLDate IsNot Nothing Then
                        _DLDate = Nothing
                    End If
                    If _DriversLicenseNumber IsNot Nothing Then
                        _DriversLicenseNumber = Nothing
                    End If
                    If _DriversLicenseState IsNot Nothing Then
                        _DriversLicenseState = Nothing
                    End If
                    If _UnitNumber IsNot Nothing Then
                        _UnitNumber = Nothing
                    End If

                    MyBase.Dispose() 'added 8/4/2014

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
        'Public Sub Dispose() Implements IDisposable.Dispose
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
