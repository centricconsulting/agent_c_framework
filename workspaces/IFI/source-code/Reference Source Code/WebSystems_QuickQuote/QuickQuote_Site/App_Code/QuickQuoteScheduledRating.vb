Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store scheduled rating information (IRPM)
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteScheduledRating 'added 8/7/2012 for IRPM
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _Description As String
        Private _Maximum As String
        Private _Minimum As String
        Private _Remark As String
        Private _RiskCharacteristicTypeId As String
        Private _RiskFactor As String
        Private _ScheduleRatingTypeId As String

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
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        Public Property Maximum As String
            Get
                Return _Maximum
            End Get
            Set(value As String)
                _Maximum = value
            End Set
        End Property
        Public Property Minimum As String
            Get
                Return _Minimum
            End Get
            Set(value As String)
                _Minimum = value
            End Set
        End Property
        Public Property Remark As String
            Get
                Return _Remark
            End Get
            Set(value As String)
                _Remark = value
            End Set
        End Property
        Public Property RiskCharacteristicTypeId As String
            Get
                Return _RiskCharacteristicTypeId
            End Get
            Set(value As String)
                _RiskCharacteristicTypeId = value
            End Set
        End Property
        Public Property RiskFactor As String
            Get
                Return _RiskFactor
            End Get
            Set(value As String)
                _RiskFactor = value
            End Set
        End Property
        Public Property ScheduleRatingTypeId As String
            Get
                Return _ScheduleRatingTypeId
            End Get
            Set(value As String)
                _ScheduleRatingTypeId = value
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
            _Description = ""
            _Maximum = ""
            _Minimum = ""
            _Remark = ""
            _RiskCharacteristicTypeId = ""
            _RiskFactor = ""
            _ScheduleRatingTypeId = ""

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.ScheduleRatingTypeId <> "" Then
                    Dim sr As String = ""
                    sr = "ScheduleRatingTypeId: " & Me.ScheduleRatingTypeId
                    Dim srType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteScheduledRating, QuickQuoteHelperClass.QuickQuotePropertyName.ScheduleRatingTypeId, Me.ScheduleRatingTypeId)
                    If srType <> "" Then
                        sr &= " (" & srType & ")"
                    End If
                    str = qqHelper.appendText(str, sr, vbCrLf)
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
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _Maximum IsNot Nothing Then
                        _Maximum = Nothing
                    End If
                    If _Minimum IsNot Nothing Then
                        _Minimum = Nothing
                    End If
                    If _Remark IsNot Nothing Then
                        _Remark = Nothing
                    End If
                    If _RiskCharacteristicTypeId IsNot Nothing Then
                        _RiskCharacteristicTypeId = Nothing
                    End If
                    If _RiskFactor IsNot Nothing Then
                        _RiskFactor = Nothing
                    End If
                    If _ScheduleRatingTypeId IsNot Nothing Then
                        _ScheduleRatingTypeId = Nothing
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
