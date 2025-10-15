Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'added 6/30/2015

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store auto symbol information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteAutoSymbol 'added 9/25/2012
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        ''' <summary>
        ''' valid auto symbol coverage types
        ''' </summary>
        ''' <remarks>value corresponds to coveragecode_id in Diamond</remarks>
        Enum QuickQuoteAutoSymbolCoverageType 'added 9/28/2012 to make easier for developer (so he doesn't have to know coverage code ids); 12/23/2013 note: will need to update to use static data file
            None = 0
            CombinedSingleLimitLiability = 2
            MedicalPayments = 60006
            UninsuredMotoristLiability = 8
            UnderinsuredMotoristBodilyInjuryLiability = 30013
            ComprehensiveCoverage = 3
            CollisionCoverage = 5
            NonOwnershipLiability = 10066
            HiredBorrowedLiability = 10062
            TowingAndLabor = 60008 'added 4/16/2013 for CAP
        End Enum

        Dim qqHelper As New QuickQuoteHelperClass 'added 6/30/2015

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _AutoSymbolNum As String
        Private _AutoSymbolTypeId As String
        Private _CoverageCodeId As String
        Private _CoverageType As QuickQuoteAutoSymbolCoverageType

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
        Public Property AutoSymbolNum As String
            Get
                Return _AutoSymbolNum
            End Get
            Set(value As String)
                _AutoSymbolNum = value
            End Set
        End Property
        Public Property AutoSymbolTypeId As String
            Get
                Return _AutoSymbolTypeId
            End Get
            Set(value As String)
                _AutoSymbolTypeId = value
            End Set
        End Property
        Public Property CoverageCodeId As String '12/23/2013 note: will need to update to use static data file
            Get
                Return _CoverageCodeId
            End Get
            Set(value As String)
                _CoverageCodeId = value
                If IsNumeric(_CoverageCodeId) = True AndAlso _CoverageCodeId <> "0" Then
                    If System.Enum.IsDefined(GetType(QuickQuoteAutoSymbolCoverageType), CInt(_CoverageCodeId)) = True Then
                        _CoverageType = CInt(_CoverageCodeId)
                    End If
                End If
            End Set
        End Property
        Public Property CoverageType As QuickQuoteAutoSymbolCoverageType '12/23/2013 note: will need to update to use static data file
            Get
                Return _CoverageType
            End Get
            Set(value As QuickQuoteAutoSymbolCoverageType)
                _CoverageType = value
                If _CoverageType <> Nothing AndAlso _CoverageType <> QuickQuoteAutoSymbolCoverageType.None Then
                    _CoverageCodeId = CInt(_CoverageType).ToString
                End If
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
            _AutoSymbolNum = ""
            _AutoSymbolTypeId = ""
            _CoverageCodeId = ""
            _CoverageType = Nothing

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.CoverageCodeId <> "" Then
                    Dim a As String = ""
                    a = "CoverageCodeId: " & Me.CoverageCodeId
                    If Me.CoverageType <> QuickQuoteAutoSymbolCoverageType.None Then
                        a &= " (" & System.Enum.GetName(GetType(QuickQuoteAutoSymbolCoverageType), Me.CoverageType) & ")"
                    End If
                    str = qqHelper.appendText(str, a, vbCrLf)
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
                    If _AutoSymbolNum IsNot Nothing Then
                        _AutoSymbolNum = Nothing
                    End If
                    If _AutoSymbolTypeId IsNot Nothing Then
                        _AutoSymbolTypeId = Nothing
                    End If
                    If _CoverageCodeId IsNot Nothing Then
                        _CoverageCodeId = Nothing
                    End If
                    If _CoverageType <> Nothing Then
                        _CoverageType = Nothing
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
