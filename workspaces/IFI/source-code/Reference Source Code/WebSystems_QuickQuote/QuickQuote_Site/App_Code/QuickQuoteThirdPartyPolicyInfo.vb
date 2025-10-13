Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store policy information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()>
    Public Class QuickQuoteThirdPartyPolicyInfo 'added 9/18/2013; renamed from QuickQuotePolicyInfo 4/20/2020
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Classification As String
        Private _GroupUsageIndicator As String
        Private _IssuingCompanyName As String
        Private _PolicyEffectiveDateFrom As String
        Private _PolicyEffectiveDateTo As String
        Private _PolicyNumber As String
        Private _PolicyRatingState As String
        Private _PolicyType As String
        Private _PolicyTypeDescription As String
        Private _PolicyVendorOverride As String
        Private _UnitNumber As String

        Public Property Classification As String
            Get
                Return _Classification
            End Get
            Set(value As String)
                _Classification = value
            End Set
        End Property
        Public Property GroupUsageIndicator As String
            Get
                Return _GroupUsageIndicator
            End Get
            Set(value As String)
                _GroupUsageIndicator = value
            End Set
        End Property
        Public Property IssuingCompanyName As String
            Get
                Return _IssuingCompanyName
            End Get
            Set(value As String)
                _IssuingCompanyName = value
            End Set
        End Property
        Public Property PolicyEffectiveDateFrom As String
            Get
                Return _PolicyEffectiveDateFrom
            End Get
            Set(value As String)
                _PolicyEffectiveDateFrom = value
                qqHelper.ConvertToShortDate(_PolicyEffectiveDateFrom)
            End Set
        End Property
        Public Property PolicyEffectiveDateTo As String
            Get
                Return _PolicyEffectiveDateTo
            End Get
            Set(value As String)
                _PolicyEffectiveDateTo = value
                qqHelper.ConvertToShortDate(_PolicyEffectiveDateTo)
            End Set
        End Property
        Public Property PolicyNumber As String
            Get
                Return _PolicyNumber
            End Get
            Set(value As String)
                _PolicyNumber = value
            End Set
        End Property
        Public Property PolicyRatingState As String
            Get
                Return _PolicyRatingState
            End Get
            Set(value As String)
                _PolicyRatingState = value
            End Set
        End Property
        Public Property PolicyType As String
            Get
                Return _PolicyType
            End Get
            Set(value As String)
                _PolicyType = value
            End Set
        End Property
        Public Property PolicyTypeDescription As String
            Get
                Return _PolicyTypeDescription
            End Get
            Set(value As String)
                _PolicyTypeDescription = value
            End Set
        End Property
        Public Property PolicyVendorOverride As String
            Get
                Return _PolicyVendorOverride
            End Get
            Set(value As String)
                _PolicyVendorOverride = value
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
            _GroupUsageIndicator = ""
            _IssuingCompanyName = ""
            _PolicyEffectiveDateFrom = ""
            _PolicyEffectiveDateTo = ""
            _PolicyNumber = ""
            _PolicyRatingState = ""
            _PolicyType = ""
            _PolicyTypeDescription = ""
            _PolicyVendorOverride = ""
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
                    If _GroupUsageIndicator IsNot Nothing Then
                        _GroupUsageIndicator = Nothing
                    End If
                    If _IssuingCompanyName IsNot Nothing Then
                        _IssuingCompanyName = Nothing
                    End If
                    If _PolicyEffectiveDateFrom IsNot Nothing Then
                        _PolicyEffectiveDateFrom = Nothing
                    End If
                    If _PolicyEffectiveDateTo IsNot Nothing Then
                        _PolicyEffectiveDateTo = Nothing
                    End If
                    If _PolicyNumber IsNot Nothing Then
                        _PolicyNumber = Nothing
                    End If
                    If _PolicyRatingState IsNot Nothing Then
                        _PolicyRatingState = Nothing
                    End If
                    If _PolicyType IsNot Nothing Then
                        _PolicyType = Nothing
                    End If
                    If _PolicyTypeDescription IsNot Nothing Then
                        _PolicyTypeDescription = Nothing
                    End If
                    If _PolicyVendorOverride IsNot Nothing Then
                        _PolicyVendorOverride = Nothing
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
