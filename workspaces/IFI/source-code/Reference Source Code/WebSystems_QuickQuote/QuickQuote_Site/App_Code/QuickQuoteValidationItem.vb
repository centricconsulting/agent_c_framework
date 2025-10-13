Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'added 4/2/2014

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store validation item information
    ''' </summary>
    ''' <remarks>holds any errors, validations, comments returned from Diamond on rating attempt</remarks>
    <Serializable()> _
    Public Class QuickQuoteValidationItem
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Enum QuickQuoteValidationSeverityType 'added 4/2/2014; uses Diamond's ValidationSeverityType table
            None
            NonApplicable 'N/A; validationseveritytype_id 0
            ValidationError 'Error; validationseveritytype_id 1
            ValidationWarning 'Warning; validationseveritytype_id 2
            Other 'Other; validationseveritytype_id 4
        End Enum

        Dim qqHelper As New QuickQuoteHelperClass 'added 4/2/2014

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _Message As String
        'added 4/2/2014
        Private _ValidationSeverityTypeId As String 'ValidationSeverityType in xml
        Private _ValidationSeverityType As QuickQuoteValidationSeverityType

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
        Public Property Message As String
            Get
                Return _Message
            End Get
            Set(value As String)
                _Message = value
            End Set
        End Property
        Public Property ValidationSeverityTypeId As String 'added 4/2/2014; ValidationSeverityType in xml; uses Diamond's ValidationSeverityType table w/ validationseveritytype_id
            Get
                Return _ValidationSeverityTypeId
            End Get
            Set(value As String)
                _ValidationSeverityTypeId = value
                If IsNumeric(_ValidationSeverityTypeId) = True Then ' AndAlso _ValidationSeverityTypeId <> "0" Then 'commented out <> "0" since that's one of the valid values; code originally copied from QuickQuoteInlandMarine class
                    'If System.Enum.IsDefined(GetType(QuickQuoteValidationSeverityType), CInt(_ValidationSeverityTypeId)) = True Then
                    '    _ValidationSeverityType = CInt(_ValidationSeverityTypeId)
                    'End If
                    'updated for static data list stuff
                    If System.Enum.TryParse(Of QuickQuoteValidationSeverityType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteValidationItem, QuickQuoteHelperClass.QuickQuotePropertyName.ValidationSeverityTypeId, _ValidationSeverityTypeId, QuickQuoteHelperClass.QuickQuotePropertyName.ValidationSeverityType), _ValidationSeverityType) = False Then
                        _ValidationSeverityType = QuickQuoteValidationSeverityType.None
                    End If
                    '12/5/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(QuickQuoteValidationSeverityType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteValidationItem, QuickQuoteHelperClass.QuickQuotePropertyName.ValidationSeverityTypeId, _ValidationSeverityTypeId, QuickQuoteHelperClass.QuickQuotePropertyName.ValidationSeverityType)) = True Then
                    '    _ValidationSeverityType = System.Enum.Parse(GetType(QuickQuoteValidationSeverityType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteValidationItem, QuickQuoteHelperClass.QuickQuotePropertyName.ValidationSeverityTypeId, _ValidationSeverityTypeId, QuickQuoteHelperClass.QuickQuotePropertyName.ValidationSeverityType))
                    'End If
                End If
            End Set
        End Property
        Public Property ValidationSeverityType As QuickQuoteValidationSeverityType
            Get
                Return _ValidationSeverityType
            End Get
            Set(value As QuickQuoteValidationSeverityType)
                _ValidationSeverityType = value
                If _ValidationSeverityType <> Nothing AndAlso _ValidationSeverityType <> QuickQuoteValidationSeverityType.None Then
                    '_ValidationSeverityTypeId = CInt(_ValidationSeverityType).ToString
                    'updated for static data list stuff
                    '_ValidationSeverityTypeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteValidationItem, QuickQuoteHelperClass.QuickQuotePropertyName.ValidationSeverityType, _ValidationSeverityType, QuickQuoteHelperClass.QuickQuotePropertyName.ValidationSeverityTypeId)
                    'updated 12/20/2013 to send enum text
                    _ValidationSeverityTypeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteValidationItem, QuickQuoteHelperClass.QuickQuotePropertyName.ValidationSeverityType, System.Enum.GetName(GetType(QuickQuoteValidationSeverityType), _ValidationSeverityType), QuickQuoteHelperClass.QuickQuotePropertyName.ValidationSeverityTypeId)
                End If
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _Message = ""
            _ValidationSeverityTypeId = "" 'added 4/2/2014
            _ValidationSeverityType = QuickQuoteValidationSeverityType.None 'added 4/2/2014
        End Sub
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.ValidationSeverityTypeId <> "" Then
                    Dim v As String = ""
                    v = "ValidationSeverityTypeId: " & Me.ValidationSeverityTypeId
                    Dim vType As String = Me.ValidationSeverityType
                    If vType <> "" Then
                        v &= " (" & vType & ")"
                    End If
                    str = qqHelper.appendText(str, v, vbCrLf)
                End If
                If Me.Message <> "" Then
                    str = qqHelper.appendText(str, "Message: " & Me.Message, vbCrLf)
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
                    If _Message IsNot Nothing Then
                        _Message = Nothing
                    End If
                    If _ValidationSeverityTypeId IsNot Nothing Then
                        _ValidationSeverityTypeId = Nothing
                    End If
                    If _ValidationSeverityType <> Nothing Then
                        _ValidationSeverityType = Nothing
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
