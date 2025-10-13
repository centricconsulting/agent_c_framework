Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store policy-level lob-specific information (that applies to individual states) for a quote
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteLobPolicyLevelInfo_AppliedToIndividualState 'added 8/16/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass


        'PolicyLevel
        'Private _ClassificationCodes As List(Of QuickQuoteClassificationCode) '8/19/2018 - moved to GoverningState because it's CRM; originally thought it was for WCP Classes, which are at Location.Classifications
        Private _Coverages As Generic.List(Of QuickQuoteCoverage)
        Private _GLClassifications As Generic.List(Of QuickQuoteGLClassification)
        Private _InclusionsExclusions As Generic.List(Of QuickQuoteInclusionExclusion)
        Private _ScheduledCoverages As List(Of QuickQuoteScheduledCoverage)


        'PolicyLevel
        'Public Property ClassificationCodes As List(Of QuickQuoteClassificationCode) '8/19/2018 - moved to GoverningState because it's CRM; originally thought it was for WCP Classes, which are at Location.Classifications
        '    Get
        '        Return _ClassificationCodes
        '    End Get
        '    Set(value As List(Of QuickQuoteClassificationCode))
        '        _ClassificationCodes = value
        '    End Set
        'End Property
        Public Property Coverages As Generic.List(Of QuickQuoteCoverage)
            Get
                Return _Coverages
            End Get
            Set(value As Generic.List(Of QuickQuoteCoverage))
                _Coverages = value
            End Set
        End Property
        Public Property GLClassifications As Generic.List(Of QuickQuoteGLClassification)
            Get
                Return _GLClassifications
            End Get
            Set(value As Generic.List(Of QuickQuoteGLClassification))
                _GLClassifications = value
            End Set
        End Property
        Public Property InclusionsExclusions As Generic.List(Of QuickQuoteInclusionExclusion)
            Get
                Return _InclusionsExclusions
            End Get
            Set(value As Generic.List(Of QuickQuoteInclusionExclusion))
                _InclusionsExclusions = value
            End Set
        End Property
        Public Property ScheduledCoverages As List(Of QuickQuoteScheduledCoverage)
            Get
                Return _ScheduledCoverages
            End Get
            Set(value As List(Of QuickQuoteScheduledCoverage))
                _ScheduledCoverages = value
            End Set
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        'Public Sub New(Parent As QuickQuoteObject) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types; removed 7/27/2018 in lieu of new generic constructor
        '    MyBase.New()
        '    SetDefaults()
        '    Me.SetParent = Parent
        'End Sub
        'Public Sub New(Parent As QuickQuotePackagePart) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types; removed 7/27/2018 in lieu of new generic constructor
        '    MyBase.New()
        '    SetDefaults()
        '    Me.SetParent = Parent
        'End Sub
        Public Sub New(Parent As Object) 'added 7/27/2018 to replace multiple constructors for different objects
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()


            'PolicyLevel
            '_ClassificationCodes = Nothing '8/19/2018 - moved to GoverningState because it's CRM; originally thought it was for WCP Classes, which are at Location.Classifications
            _Coverages = Nothing
            _GLClassifications = Nothing
            _InclusionsExclusions = Nothing
            _ScheduledCoverages = Nothing
        End Sub
        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then
                'If Me.PackagePartTypeId <> "" Then
                '    Dim t As String = ""
                '    t = "PackagePartTypeId: " & Me.PackagePartTypeId
                '    If Me.PackagePartType <> "" Then
                '        t &= " (" & Me.PackagePartType & ")"
                '    End If
                '    str = qqHelper.appendText(str, t, vbCrLf)
                'End If
                'If Me.FullTermPremium <> "" Then
                '    str = qqHelper.appendText(str, "FullTermPremium: " & Me.FullTermPremium, vbCrLf)
                'End If
                If Me.Coverages IsNot Nothing AndAlso Me.Coverages.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Coverages.Count.ToString & " Coverages", vbCrLf)
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
        'updated w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).


                    'PolicyLevel
                    'If _ClassificationCodes IsNot Nothing Then '8/19/2018 - moved to GoverningState because it's CRM; originally thought it was for WCP Classes, which are at Location.Classifications
                    '    If _ClassificationCodes.Count > 0 Then
                    '        For Each c As QuickQuoteClassificationCode In _ClassificationCodes
                    '            c.Dispose()
                    '            c = Nothing
                    '        Next
                    '        _ClassificationCodes.Clear()
                    '    End If
                    '    _ClassificationCodes = Nothing
                    'End If
                    If _Coverages IsNot Nothing Then
                        If _Coverages.Count > 0 Then
                            For Each cov As QuickQuoteCoverage In _Coverages
                                cov.Dispose()
                                cov = Nothing
                            Next
                            _Coverages.Clear()
                        End If
                        _Coverages = Nothing
                    End If
                    If _GLClassifications IsNot Nothing Then
                        If _GLClassifications.Count > 0 Then
                            For Each gl As QuickQuoteGLClassification In _GLClassifications
                                gl.Dispose()
                                gl = Nothing
                            Next
                            _GLClassifications.Clear()
                        End If
                        _GLClassifications = Nothing
                    End If
                    If _InclusionsExclusions IsNot Nothing Then
                        If _InclusionsExclusions.Count > 0 Then
                            For Each ie As QuickQuoteInclusionExclusion In _InclusionsExclusions
                                ie.Dispose()
                                ie = Nothing
                            Next
                            _InclusionsExclusions.Clear()
                        End If
                        _InclusionsExclusions = Nothing
                    End If
                    If _ScheduledCoverages IsNot Nothing Then
                        If _ScheduledCoverages.Count > 0 Then
                            For Each c As QuickQuoteScheduledCoverage In _ScheduledCoverages
                                c.Dispose()
                                c = Nothing
                            Next
                            _ScheduledCoverages.Clear()
                        End If
                        _ScheduledCoverages = Nothing
                    End If

                    MyBase.Dispose()
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
        'updated  w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace

