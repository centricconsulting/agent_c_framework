Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'added 4/27/2014

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store vehicle symbol information
    ''' </summary>
    ''' <remarks>related to PPA rating</remarks>
    <Serializable()> _
    Public Class QuickQuoteVehicleSymbol 'added 4/15/2014
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'added 4/27/2014

        'Private _PolicyId As String
        'Private _PolicyImageNum As String
        Private _DetailStatusCode As String 'added 4/23/2014 since VehicleSymbols always seem to stay on rated image after flagging for delete
        Private _PackagePartNum As String
        Private _SystemGeneratedSymbol As String
        Private _SystemGeneratedSymbolVehicleInfoLookupTypeId As String
        Private _UserOverrideSymbol As String
        'Private _VehicleNum As String
        Private _VehicleSymbolCoverageTypeId As String
        Private _VehicleSymbolNum As String 'uncommented 4/24/2014 for reconciliation

        'Public Property PolicyId As String
        '    Get
        '        Return _PolicyId
        '    End Get
        '    Set(value As String)
        '        _PolicyId = value
        '    End Set
        'End Property
        'Public Property PolicyImageNum As String
        '    Get
        '        Return _PolicyImageNum
        '    End Get
        '    Set(value As String)
        '        _PolicyImageNum = value
        '    End Set
        'End Property
        Public Property DetailStatusCode As String 'added 4/23/2014 since VehicleSymbols always seem to stay on rated image after flagging for delete
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property
        Public Property PackagePartNum As String
            Get
                Return _PackagePartNum
            End Get
            Set(value As String)
                _PackagePartNum = value
            End Set
        End Property
        Public Property SystemGeneratedSymbol As String
            Get
                Return _SystemGeneratedSymbol
            End Get
            Set(value As String)
                _SystemGeneratedSymbol = value
            End Set
        End Property
        Public Property SystemGeneratedSymbolVehicleInfoLookupTypeId As String
            Get
                Return _SystemGeneratedSymbolVehicleInfoLookupTypeId
            End Get
            Set(value As String)
                _SystemGeneratedSymbolVehicleInfoLookupTypeId = value
            End Set
        End Property
        Public Property UserOverrideSymbol As String
            Get
                Return _UserOverrideSymbol
            End Get
            Set(value As String)
                _UserOverrideSymbol = value
            End Set
        End Property
        'Public Property VehicleNum As String
        '    Get
        '        Return _VehicleNum
        '    End Get
        '    Set(value As String)
        '        _VehicleNum = value
        '    End Set
        'End Property
        Public Property VehicleSymbolCoverageTypeId As String
            Get
                Return _VehicleSymbolCoverageTypeId
            End Get
            Set(value As String)
                _VehicleSymbolCoverageTypeId = value
            End Set
        End Property
        Public Property VehicleSymbolNum As String 'uncommented 4/24/2014 for reconciliation
            Get
                Return _VehicleSymbolNum
            End Get
            Set(value As String)
                _VehicleSymbolNum = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            '_PolicyId = ""
            '_PolicyImageNum = ""
            _DetailStatusCode = "" 'added 4/23/2014 since VehicleSymbols always seem to stay on rated image after flagging for delete
            _PackagePartNum = ""
            _SystemGeneratedSymbol = ""
            _SystemGeneratedSymbolVehicleInfoLookupTypeId = ""
            _UserOverrideSymbol = ""
            '_VehicleNum = ""
            _VehicleSymbolCoverageTypeId = ""
            _VehicleSymbolNum = "" 'uncommented 4/24/2014 for reconciliation
        End Sub
        Public Function HasValidVehicleSymbolNum() As Boolean 'added 4/24/2014 for reconciliation purposes
            'If _VehicleSymbolNum <> "" AndAlso IsNumeric(_VehicleSymbolNum) = True AndAlso CInt(_VehicleSymbolNum) > 0 Then
            '    Return True
            'Else
            '    Return False
            'End If
            'updated 4/27/2014 to use common method
            Return qqHelper.IsValidQuickQuoteIdOrNum(_VehicleSymbolNum)
        End Function
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.VehicleSymbolCoverageTypeId <> "" Then
                    Dim s As String = ""
                    s = "VehicleSymbolCoverageTypeId: " & Me.VehicleSymbolCoverageTypeId
                    Dim sType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicleSymbol, QuickQuoteHelperClass.QuickQuotePropertyName.VehicleSymbolCoverageTypeId, Me.VehicleSymbolCoverageTypeId)
                    If sType <> "" Then
                        s &= " (" & sType & ")"
                    End If
                    str = qqHelper.appendText(str, s, vbCrLf)
                End If
                If Me.SystemGeneratedSymbolVehicleInfoLookupTypeId <> "" Then
                    Dim s As String = ""
                    s = "SystemGeneratedSymbolVehicleInfoLookupTypeId: " & Me.SystemGeneratedSymbolVehicleInfoLookupTypeId
                    Dim sType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicleSymbol, QuickQuoteHelperClass.QuickQuotePropertyName.SystemGeneratedSymbolVehicleInfoLookupTypeId, Me.SystemGeneratedSymbolVehicleInfoLookupTypeId)
                    If sType <> "" Then
                        s &= " (" & sType & ")"
                    End If
                    str = qqHelper.appendText(str, s, vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    'If _PolicyId IsNot Nothing Then
                    '    _PolicyId = Nothing
                    'End If
                    'If _PolicyImageNum IsNot Nothing Then
                    '    _PolicyImageNum = Nothing
                    'End If
                    If _DetailStatusCode IsNot Nothing Then 'added 4/23/2014 since VehicleSymbols always seem to stay on rated image after flagging for delete
                        _DetailStatusCode = Nothing
                    End If
                    If _PackagePartNum IsNot Nothing Then
                        _PackagePartNum = Nothing
                    End If
                    If _SystemGeneratedSymbol IsNot Nothing Then
                        _SystemGeneratedSymbol = Nothing
                    End If
                    If _SystemGeneratedSymbolVehicleInfoLookupTypeId IsNot Nothing Then
                        _SystemGeneratedSymbolVehicleInfoLookupTypeId = Nothing
                    End If
                    If _UserOverrideSymbol IsNot Nothing Then
                        _UserOverrideSymbol = Nothing
                    End If
                    'If _VehicleNum IsNot Nothing Then
                    '    _VehicleNum = Nothing
                    'End If
                    If _VehicleSymbolCoverageTypeId IsNot Nothing Then
                        _VehicleSymbolCoverageTypeId = Nothing
                    End If
                    If _VehicleSymbolNum IsNot Nothing Then 'uncommented 4/24/2014 for reconciliation
                        _VehicleSymbolNum = Nothing
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
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace