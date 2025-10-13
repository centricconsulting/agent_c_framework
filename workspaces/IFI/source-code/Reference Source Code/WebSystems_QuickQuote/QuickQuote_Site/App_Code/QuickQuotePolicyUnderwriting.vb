Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store policy underwriting question information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuotePolicyUnderwriting
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        'added 8/16/2012 PM
        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _PolicyUnderwritingAnswer As String
        Private _PolicyUnderwritingAnswerTypeId As String '-1 = Unanswered; 0 = N/A; 1 = Yes; 2 = No
        Private _PolicyUnderwritingCodeId As String 'Question; table joins to Version table for LOB
        Private _PolicyUnderwritingExtraAnswer As String
        Private _PolicyUnderwritingExtraAnswerTypeId As String '1 = Text; 2 = Date; 3 = Currency
        Private _PolicyUnderwritingLevelId As String '0 = N/A; 1 = Policy Image; 2 = Vehicle; 3 = Location; 4 = Watercraft; 8 = All Watercraft
        Private _PolicyUnderwritingTabId As String '1 = UW # 1; 2 = UW # 2; 3 = UW # 3
        Private _PolicyUnderwriterDate As String
        'also table for PolicyUnderwritingCodeType (id 0 = Policy; id 1 = Additional Policy)

        Private _PolicyUnderwritingNum As String 'added 10/14/2014 for reconciliation

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
        Public Property PolicyUnderwritingAnswer As String
            Get
                Return _PolicyUnderwritingAnswer
            End Get
            Set(value As String)
                _PolicyUnderwritingAnswer = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's PolicyUnderwritingAnswerType table (-1=Unanswered, 0=N/A, 1=Yes, 2=No)</remarks>
        Public Property PolicyUnderwritingAnswerTypeId As String
            Get
                Return _PolicyUnderwritingAnswerTypeId
            End Get
            Set(value As String)
                _PolicyUnderwritingAnswerTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's PolicyUnderwritingCode table; joins to PolicyUnderwritingCodeVersion to get questions specific to each LOB</remarks>
        Public Property PolicyUnderwritingCodeId As String
            Get
                Return _PolicyUnderwritingCodeId
            End Get
            Set(value As String)
                _PolicyUnderwritingCodeId = value
            End Set
        End Property
        Public Property PolicyUnderwritingExtraAnswer As String
            Get
                Return _PolicyUnderwritingExtraAnswer
            End Get
            Set(value As String)
                _PolicyUnderwritingExtraAnswer = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's PolicyUnderwritingExtraAnswerType table (1=Text, 2=Date, 3=Currency)</remarks>
        Public Property PolicyUnderwritingExtraAnswerTypeId As String
            Get
                Return _PolicyUnderwritingExtraAnswerTypeId
            End Get
            Set(value As String)
                _PolicyUnderwritingExtraAnswerTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's PolicyUnderwritingLevel table (0=N/A, 1=Policy Image, 2=Vehicle, 3=Location, 4=Watercraft, 8=All Watercraft)</remarks>
        Public Property PolicyUnderwritingLevelId As String
            Get
                Return _PolicyUnderwritingLevelId
            End Get
            Set(value As String)
                _PolicyUnderwritingLevelId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's PolicyUnderwritingTab table (1=UW # 1, 2=UW # 2, 3=UW # 3)</remarks>
        Public Property PolicyUnderwritingTabId As String
            Get
                Return _PolicyUnderwritingTabId
            End Get
            Set(value As String)
                _PolicyUnderwritingTabId = value
            End Set
        End Property
        Public Property PolicyUnderwriterDate As String
            Get
                Return _PolicyUnderwriterDate
            End Get
            Set(value As String)
                _PolicyUnderwriterDate = value
                qqHelper.ConvertToShortDate(_PolicyUnderwriterDate)
            End Set
        End Property

        Public Property PolicyUnderwritingNum As String 'added 10/14/2014 for reconciliation
            Get
                Return _PolicyUnderwritingNum
            End Get
            Set(value As String)
                _PolicyUnderwritingNum = value
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
            _PolicyUnderwritingAnswer = ""
            _PolicyUnderwritingAnswerTypeId = ""
            _PolicyUnderwritingCodeId = ""
            _PolicyUnderwritingExtraAnswer = ""
            _PolicyUnderwritingExtraAnswerTypeId = ""
            _PolicyUnderwritingLevelId = ""
            _PolicyUnderwritingTabId = ""
            _PolicyUnderwriterDate = ""

            _PolicyUnderwritingNum = "" 'added 10/14/2014 for reconciliation

            _DetailStatusCode = "" 'added 5/15/2019

        End Sub
        Public Function HasValidPolicyUnderwritingNum() As Boolean 'added 10/14/2014 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_PolicyUnderwritingNum)
        End Function
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.PolicyUnderwritingCodeId <> "" Then
                    str = qqHelper.appendText(str, "PolicyUnderwritingCodeId: " & Me.PolicyUnderwritingCodeId, vbCrLf)
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
                    If _PolicyUnderwritingAnswer IsNot Nothing Then
                        _PolicyUnderwritingAnswer = Nothing
                    End If
                    If _PolicyUnderwritingAnswerTypeId IsNot Nothing Then
                        _PolicyUnderwritingAnswerTypeId = Nothing
                    End If
                    If _PolicyUnderwritingCodeId IsNot Nothing Then
                        _PolicyUnderwritingCodeId = Nothing
                    End If
                    If _PolicyUnderwritingExtraAnswer IsNot Nothing Then
                        _PolicyUnderwritingExtraAnswer = Nothing
                    End If
                    If _PolicyUnderwritingExtraAnswerTypeId IsNot Nothing Then
                        _PolicyUnderwritingExtraAnswerTypeId = Nothing
                    End If
                    If _PolicyUnderwritingLevelId IsNot Nothing Then
                        _PolicyUnderwritingLevelId = Nothing
                    End If
                    If _PolicyUnderwritingTabId IsNot Nothing Then
                        _PolicyUnderwritingTabId = Nothing
                    End If
                    If _PolicyUnderwriterDate IsNot Nothing Then
                        _PolicyUnderwriterDate = Nothing
                    End If

                    If _PolicyUnderwritingNum IsNot Nothing Then 'added 10/14/2014 for reconciliation
                        _PolicyUnderwritingNum = Nothing
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
