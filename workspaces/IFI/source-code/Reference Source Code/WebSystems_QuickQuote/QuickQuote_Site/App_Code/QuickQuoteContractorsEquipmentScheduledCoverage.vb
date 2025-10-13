Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' objects used to store contractors equipment information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteObject object (<see cref="QuickQuoteObject"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuoteContractorsEquipmentScheduledCoverage 'added 1/22/2015
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        'ScheduledCoverage
        Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _ScheduledCoverageNum As String

        'Coverage
        Private _ManualLimitAmount As String
        Private _Description As String

        'CoverageDetail
        Private _ManufacturerName As String
        Private _Model As String
        Private _SerialNumber As String
        Private _ValuationMethodTypeId As String
        Private _Year As String

        'added 1/28/2015
        Private _QuotedPremium As String

        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A04191}")
                Return _AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A04191}")
            End Set
        End Property
        Public Property CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _CanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property ScheduledCoverageNum As String
            Get
                Return _ScheduledCoverageNum
            End Get
            Set(value As String)
                _ScheduledCoverageNum = value
            End Set
        End Property

        Public Property ManualLimitAmount As String
            Get
                Return _ManualLimitAmount
            End Get
            Set(value As String)
                _ManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_ManualLimitAmount)
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

        Public Property ManufacturerName As String
            Get
                Return _ManufacturerName
            End Get
            Set(value As String)
                _ManufacturerName = value
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
        Public Property SerialNumber As String
            Get
                Return _SerialNumber
            End Get
            Set(value As String)
                _SerialNumber = value
            End Set
        End Property
        Public Property ValuationMethodTypeId As String
            Get
                Return _ValuationMethodTypeId
            End Get
            Set(value As String)
                _ValuationMethodTypeId = value
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

        'added 1/28/2015
        Public Property QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_QuotedPremium)
            End Get
            Set(value As String)
                _QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_QuotedPremium)
            End Set
        End Property

        'added 5/22/2017 for Diamond Proposals
        Public ReadOnly Property TextDescription_Desc_or_SerialYearMakeModel As String
            Get
                Dim desc As String = ""

                If String.IsNullOrWhiteSpace(_Description) = False Then
                    desc = _Description
                Else
                    desc = TextDescription_SerialYearMakeModel
                End If

                Return desc
            End Get
        End Property
        Public ReadOnly Property TextDescription_SerialYearMakeModel As String
            Get
                Dim desc As String = ""

                If String.IsNullOrWhiteSpace(_SerialNumber) = False Then
                    desc = qqHelper.appendText(desc, _SerialNumber, splitter:=" ")
                End If
                'If String.IsNullOrWhiteSpace(_Year) = False Then
                'updated 5/25/2017
                If qqHelper.IsPositiveIntegerString(_Year) = True Then
                    desc = qqHelper.appendText(desc, _Year, splitter:=" ")
                End If
                If String.IsNullOrWhiteSpace(_ManufacturerName) = False Then
                    desc = qqHelper.appendText(desc, _ManufacturerName, splitter:=" ")
                End If
                If String.IsNullOrWhiteSpace(_Model) = False Then
                    desc = qqHelper.appendText(desc, _Model, splitter:=" ")
                End If

                Return desc
            End Get
        End Property
        Public ReadOnly Property TextDescription_SerialYearMakeModelDesc As String
            Get
                Dim desc As String = TextDescription_SerialYearMakeModel
                If String.IsNullOrWhiteSpace(_Description) = False Then
                    desc = qqHelper.appendText(desc, _Description, splitter:=" ")
                End If

                Return desc
            End Get
        End Property
        'added 5/25/2017
        Public ReadOnly Property TextDescription_YearMakeModelSerial As String
            Get
                Dim desc As String = ""

                'If String.IsNullOrWhiteSpace(_Year) = False Then
                'updated 5/25/2017
                If qqHelper.IsPositiveIntegerString(_Year) = True Then
                    desc = qqHelper.appendText(desc, _Year, splitter:=" ")
                End If
                If String.IsNullOrWhiteSpace(_ManufacturerName) = False Then
                    desc = qqHelper.appendText(desc, _ManufacturerName, splitter:=" ")
                End If
                If String.IsNullOrWhiteSpace(_Model) = False Then
                    desc = qqHelper.appendText(desc, _Model, splitter:=" ")
                End If
                If String.IsNullOrWhiteSpace(_SerialNumber) = False Then
                    desc = qqHelper.appendText(desc, _SerialNumber, splitter:=" ")
                End If

                Return desc
            End Get
        End Property
        Public ReadOnly Property TextDescription_YearMakeDescModelSerial As String
            Get
                Dim desc As String = ""

                'If String.IsNullOrWhiteSpace(_Year) = False Then
                'updated 5/25/2017
                If qqHelper.IsPositiveIntegerString(_Year) = True Then
                    desc = qqHelper.appendText(desc, _Year, splitter:=" ")
                End If
                If String.IsNullOrWhiteSpace(_ManufacturerName) = False Then
                    desc = qqHelper.appendText(desc, _ManufacturerName, splitter:=" ")
                End If
                If String.IsNullOrWhiteSpace(_Description) = False Then
                    desc = qqHelper.appendText(desc, _Description, splitter:=" ")
                End If
                If String.IsNullOrWhiteSpace(_Model) = False Then
                    desc = qqHelper.appendText(desc, _Model, splitter:=" ")
                End If
                If String.IsNullOrWhiteSpace(_SerialNumber) = False Then
                    desc = qqHelper.appendText(desc, _SerialNumber, splitter:=" ")
                End If

                Return desc
            End Get
        End Property
        Public ReadOnly Property TextDescription_Desc_or_YearMakeModelSerial As String
            Get
                Dim desc As String = ""

                If String.IsNullOrWhiteSpace(_Description) = False Then
                    desc = _Description
                Else
                    desc = TextDescription_YearMakeModelSerial
                End If

                Return desc
            End Get
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            '_AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing
            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _ScheduledCoverageNum = ""

            _ManualLimitAmount = ""
            _Description = ""

            _ManufacturerName = ""
            _Model = ""
            _SerialNumber = ""
            _ValuationMethodTypeId = ""
            _Year = ""

            'added 1/28/2015
            _QuotedPremium = ""
        End Sub
        Public Function HasValidScheduledCoverageNum() As Boolean
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ScheduledCoverageNum)
        End Function
        'added 1/22/2015 for additionalInterests reconciliation; method is here and in QuickQuoteScheduledCoverage... can be called from either and passed to the other
        Public Sub ParseThruAdditionalInterests()
            If _AdditionalInterests IsNot Nothing AndAlso _AdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False Then
                        If ai.HasValidAdditionalInterestNum = True Then
                            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _AdditionalInterests IsNot Nothing Then
                        If _AdditionalInterests.Count > 0 Then
                            For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                                ai.Dispose()
                                ai = Nothing
                            Next
                            _AdditionalInterests.Clear()
                        End If
                        _AdditionalInterests = Nothing
                    End If
                    _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    If _ScheduledCoverageNum IsNot Nothing Then
                        _ScheduledCoverageNum = Nothing
                    End If

                    If _ManualLimitAmount IsNot Nothing Then
                        _ManualLimitAmount = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If

                    If _ManufacturerName IsNot Nothing Then
                        _ManufacturerName = Nothing
                    End If
                    If _Model IsNot Nothing Then
                        _Model = Nothing
                    End If
                    If _SerialNumber IsNot Nothing Then
                        _SerialNumber = Nothing
                    End If
                    If _ValuationMethodTypeId IsNot Nothing Then
                        _ValuationMethodTypeId = Nothing
                    End If
                    If _Year IsNot Nothing Then
                        _Year = Nothing
                    End If

                    'added 1/28/2015
                    qqHelper.DisposeString(_QuotedPremium)

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
        Public Overrides Sub Dispose()
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
