Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store RvWatercraftMotor information
    ''' </summary>
    ''' <remarks>currently used as list object under RvWatercraft object (<see cref="QuickQuoteRvWatercraft"/>)</remarks>
    <Serializable()> _
    Public Class QuickQuoteRvWatercraftMotor 'added 8/2/2013 for HOM
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _CostNew As String
        Private _Manufacturer As String
        Private _Model As String
        Private _MotorTypeId As String 'may need matching MotorType variable/property
        Private _SerialNumber As String
        Private _Year As String

        Private _RvWatercraftMotorNum As String 'added 10/14/2014 for reconciliation

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property PolicyId As String
            Get
                Return _PolicyId
            End Get
            Set(value As String)
                _PolicyId = value
            End Set
        End Property
        Public Property PolicyImageNum As String
            Get
                Return _PolicyImageNum
            End Get
            Set(value As String)
                _PolicyImageNum = value
            End Set
        End Property
        Public Property CostNew As String
            Get
                Return _CostNew
                'updated 8/25/2014; won't use for now
                'Return qqHelper.QuotedPremiumFormat(_CostNew)
            End Get
            Set(value As String)
                _CostNew = value
                qqHelper.ConvertToQuotedPremiumFormat(_CostNew)
            End Set
        End Property
        Public Property Manufacturer As String
            Get
                Return _Manufacturer
            End Get
            Set(value As String)
                _Manufacturer = value
            End Set
        End Property
        Public Property Model As String
            Get
                Return _Model
            End Get
            Set(value As String)
                _Model = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's MotorType table (0=None, 1=Inboard, 2=Outboard, 3=Inboard/Outboard, 4=Jetdrive, 5=Other, 6=Jet Drive, 7=N/A)</remarks>
        Public Property MotorTypeId As String '0=None; 1=Inboard; 2=Outboard; 3=Inboard/Outboard; 4=Jetdrive; 5=Other; 6=Jet Drive; 7=N/A
            Get
                Return _MotorTypeId
            End Get
            Set(value As String)
                _MotorTypeId = value
            End Set
        End Property
        Public Property SerialNumber As String
            Get
                Return _SerialNumber
            End Get
            Set(value As String)
                _SerialNumber = value
            End Set
        End Property
        Public Property Year As String
            Get
                Return _Year
            End Get
            Set(value As String)
                _Year = value
            End Set
        End Property

        Public Property RvWatercraftMotorNum As String 'added 10/14/2014 for reconciliation
            Get
                Return _RvWatercraftMotorNum
            End Get
            Set(value As String)
                _RvWatercraftMotorNum = value
            End Set
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _CostNew = ""
            _Manufacturer = ""
            _Model = ""
            _MotorTypeId = ""
            _SerialNumber = ""
            _Year = ""

            _RvWatercraftMotorNum = "" 'added 10/14/2014 for reconciliation

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Function HasValidRvWatercraftMotorNum() As Boolean 'added 10/14/2014 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_RvWatercraftMotorNum)
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
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _CostNew IsNot Nothing Then
                        _CostNew = Nothing
                    End If
                    If _Manufacturer IsNot Nothing Then
                        _Manufacturer = Nothing
                    End If
                    If _Model IsNot Nothing Then
                        _Model = Nothing
                    End If
                    If _MotorTypeId IsNot Nothing Then
                        _MotorTypeId = Nothing
                    End If
                    If _SerialNumber IsNot Nothing Then
                        _SerialNumber = Nothing
                    End If
                    If _Year IsNot Nothing Then
                        _Year = Nothing
                    End If

                    If _RvWatercraftMotorNum IsNot Nothing Then 'added 10/14/2014 for reconciliation
                        _RvWatercraftMotorNum = Nothing
                    End If

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

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
