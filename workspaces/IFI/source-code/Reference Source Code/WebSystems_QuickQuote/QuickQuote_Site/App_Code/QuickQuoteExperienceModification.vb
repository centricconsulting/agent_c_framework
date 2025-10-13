Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store payment option information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteExperienceModification 'added 9/16/2017
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _BureauTypeId As String
        Private _DetailStatusCode As String
        Private _ExperienceModificationNum As String
        Private _ExperienceRatingPeriodStatusTypeId As String
        Private _Factor As String
        Private _ModificationProductionDate As String
        Private _PolicyId As String
        Private _RatingEffectiveDate As String
        Private _RiskIdentifier As String
        'added 9/23/2017
        Private _BureauTypeId_Original As String
        Private _DetailStatusCode_Original As String
        Private _ExperienceRatingPeriodStatusTypeId_Original As String
        Private _Factor_Original As String
        Private _ModificationProductionDate_Original As String
        Private _RatingEffectiveDate_Original As String
        Private _RiskIdentifier_Original As String
        'Private _HasChange As Boolean

        Public Property BureauTypeId As String
            Get
                Return _BureauTypeId
            End Get
            Set(value As String)
                _BureauTypeId = value
            End Set
        End Property
        Public Property DetailStatusCode As String
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property
        Public Property ExperienceModificationNum As String
            Get
                Return _ExperienceModificationNum
            End Get
            Set(value As String)
                _ExperienceModificationNum = value
            End Set
        End Property
        Public Property ExperienceRatingPeriodStatusTypeId As String '-1="", 0=N/A, 1=Not Experience Rated, 2=Final, 3=Revised, 4=Contingent, 5=Preliminary
            Get
                Return _ExperienceRatingPeriodStatusTypeId
            End Get
            Set(value As String)
                _ExperienceRatingPeriodStatusTypeId = value
            End Set
        End Property
        Public Property Factor As String
            Get
                Return _Factor
            End Get
            Set(value As String)
                _Factor = value
            End Set
        End Property
        Public Property ModificationProductionDate As String
            Get
                Return _ModificationProductionDate
            End Get
            Set(value As String)
                _ModificationProductionDate = value
            End Set
        End Property
        Public Property PolicyId As String
            Get
                Return _PolicyId
            End Get
            Set(value As String)
                _PolicyId = value
            End Set
        End Property
        Public Property RatingEffectiveDate As String
            Get
                Return _RatingEffectiveDate
            End Get
            Set(value As String)
                _RatingEffectiveDate = value
            End Set
        End Property
        Public Property RiskIdentifier As String
            Get
                Return _RiskIdentifier
            End Get
            Set(value As String)
                _RiskIdentifier = value
            End Set
        End Property
        'added 9/23/2017
        Protected Friend Property BureauTypeId_Original As String
            Get
                Return _BureauTypeId_Original
            End Get
            Set(value As String)
                _BureauTypeId_Original = value
            End Set
        End Property
        Protected Friend Property DetailStatusCode_Original As String
            Get
                Return _DetailStatusCode_Original
            End Get
            Set(value As String)
                _DetailStatusCode_Original = value
            End Set
        End Property
        Protected Friend Property ExperienceRatingPeriodStatusTypeId_Original As String
            Get
                Return _ExperienceRatingPeriodStatusTypeId_Original
            End Get
            Set(value As String)
                _ExperienceRatingPeriodStatusTypeId_Original = value
            End Set
        End Property
        Protected Friend Property Factor_Original As String
            Get
                Return _Factor_Original
            End Get
            Set(value As String)
                _Factor_Original = value
            End Set
        End Property
        Protected Friend Property ModificationProductionDate_Original As String
            Get
                Return _ModificationProductionDate_Original
            End Get
            Set(value As String)
                _ModificationProductionDate_Original = value
            End Set
        End Property
        Protected Friend Property RatingEffectiveDate_Original As String
            Get
                Return _RatingEffectiveDate_Original
            End Get
            Set(value As String)
                _RatingEffectiveDate_Original = value
            End Set
        End Property
        Protected Friend Property RiskIdentifier_Original As String
            Get
                Return _RiskIdentifier_Original
            End Get
            Set(value As String)
                _RiskIdentifier_Original = value
            End Set
        End Property
        'Protected Friend Property HasChange As Boolean
        '    Get
        '        Return _HasChange
        '    End Get
        '    Set(value As Boolean)
        '        _HasChange = value
        '    End Set
        'End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _BureauTypeId = ""
            _DetailStatusCode = ""
            _ExperienceModificationNum = ""
            _ExperienceRatingPeriodStatusTypeId = ""
            _Factor = ""
            _ModificationProductionDate = ""
            _PolicyId = ""
            _RatingEffectiveDate = ""
            _RiskIdentifier = ""
            'added 9/23/2017
            _BureauTypeId_Original = ""
            _DetailStatusCode_Original = ""
            _ExperienceRatingPeriodStatusTypeId_Original = ""
            _Factor_Original = ""
            _ModificationProductionDate_Original = ""
            _RatingEffectiveDate_Original = ""
            _RiskIdentifier_Original = ""
            '_HasChange = False
        End Sub

        Public Function HasValidExperienceModificationNum() As Boolean 'added 9/18/2017 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ExperienceModificationNum)
        End Function

        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then
                If qqHelper.IsNumericString(Me.BureauTypeId) = True Then
                    str = qqHelper.appendText(str, "BureauTypeId: " & Me.BureauTypeId, vbCrLf)
                End If
                If String.IsNullOrWhiteSpace(Me.Factor) = False Then
                    str = qqHelper.appendText(str, "Factor: " & Me.Factor, vbCrLf)
                End If
                If qqHelper.IsDateString(Me.ModificationProductionDate) = True Then
                    str = qqHelper.appendText(str, "ModificationProductionDate: " & Me.ModificationProductionDate, vbCrLf)
                End If
                If qqHelper.IsDateString(Me.RatingEffectiveDate) = True Then
                    str = qqHelper.appendText(str, "RatingEffectiveDate: " & Me.RatingEffectiveDate, vbCrLf)
                End If

            Else
                str = "Nothing"
            End If
            Return str
        End Function

        'added 9/23/2017
        Protected Friend Sub SetOriginalProperties()
            _BureauTypeId_Original = _BureauTypeId
            _DetailStatusCode_Original = _DetailStatusCode
            _ExperienceRatingPeriodStatusTypeId_Original = _ExperienceRatingPeriodStatusTypeId
            _Factor_Original = _Factor
            _ModificationProductionDate_Original = _ModificationProductionDate
            _RatingEffectiveDate_Original = _RatingEffectiveDate
            _RiskIdentifier_Original = _RiskIdentifier
        End Sub
        Protected Friend Function HasChange(Optional ByVal checkDetailStatusCode As Boolean = False) As Boolean
            Dim changed As Boolean = False

            If BureauTypeId_HasChange() OrElse (checkDetailStatusCode = True AndAlso DetailStatusCode_HasChange()) OrElse ExperienceRatingPeriodStatusTypeId_HasChange() OrElse Factor_HasChange() OrElse ModificationProductionDate_HasChange() OrElse RatingEffectiveDate_HasChange() OrElse RiskIdentifier_HasChange() Then
                changed = True
            End If

            Return changed
        End Function
        Protected Friend Function BureauTypeId_HasChange() As Boolean
            Dim changed As Boolean = False

            'If qqHelper.IntegerForString(_BureauTypeId) <> qqHelper.IntegerForString(_BureauTypeId_Original) Then
            '    changed = True
            'End If
            If QuickQuoteHelperClass.isTextMatch(_BureauTypeId, _BureauTypeId_Original, matchType:=QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing) = False Then
                changed = True
            End If

            Return changed
        End Function
        Protected Friend Function DetailStatusCode_HasChange() As Boolean
            Dim changed As Boolean = False

            If QuickQuoteHelperClass.isTextMatch(_DetailStatusCode, _DetailStatusCode_Original, matchType:=QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing) = False Then
                changed = True
            End If

            Return changed
        End Function
        Protected Friend Function ExperienceRatingPeriodStatusTypeId_HasChange() As Boolean
            Dim changed As Boolean = False

            'If qqHelper.IntegerForString(_ExperienceRatingPeriodStatusTypeId) <> qqHelper.IntegerForString(_ExperienceRatingPeriodStatusTypeId_Original) Then
            '    changed = True
            'End If
            If QuickQuoteHelperClass.isTextMatch(_ExperienceRatingPeriodStatusTypeId, _ExperienceRatingPeriodStatusTypeId_Original, matchType:=QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing) = False Then
                changed = True
            End If

            Return changed
        End Function
        Protected Friend Function Factor_HasChange() As Boolean
            Dim changed As Boolean = False

            If QuickQuoteHelperClass.isTextMatch(_Factor, _Factor_Original, matchType:=QuickQuoteHelperClass.TextMatchType.DecimalOrText_IgnoreCasing) = False Then
                changed = True
            End If

            Return changed
        End Function
        Protected Friend Function ModificationProductionDate_HasChange() As Boolean
            Dim changed As Boolean = False

            If QuickQuoteHelperClass.isTextMatch(_ModificationProductionDate, _ModificationProductionDate_Original, matchType:=QuickQuoteHelperClass.TextMatchType.DateOrText_IgnoreCasing) = False Then
                changed = True
            End If

            Return changed
        End Function
        Protected Friend Function RatingEffectiveDate_HasChange() As Boolean
            Dim changed As Boolean = False

            If QuickQuoteHelperClass.isTextMatch(_RatingEffectiveDate, _RatingEffectiveDate_Original, matchType:=QuickQuoteHelperClass.TextMatchType.DateOrText_IgnoreCasing) = False Then
                changed = True
            End If

            Return changed
        End Function
        Protected Friend Function RiskIdentifier_HasChange() As Boolean
            Dim changed As Boolean = False

            If QuickQuoteHelperClass.isTextMatch(_RiskIdentifier, _RiskIdentifier_Original, matchType:=QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing) = False Then
                changed = True
            End If

            Return changed
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    qqHelper.DisposeString(_BureauTypeId)
                    qqHelper.DisposeString(_DetailStatusCode)
                    qqHelper.DisposeString(_ExperienceModificationNum)
                    qqHelper.DisposeString(_ExperienceRatingPeriodStatusTypeId)
                    qqHelper.DisposeString(_Factor)
                    qqHelper.DisposeString(_ModificationProductionDate)
                    qqHelper.DisposeString(_PolicyId)
                    qqHelper.DisposeString(_RatingEffectiveDate)
                    qqHelper.DisposeString(_RiskIdentifier)
                    'added 9/23/2017
                    qqHelper.DisposeString(_BureauTypeId_Original)
                    qqHelper.DisposeString(_DetailStatusCode_Original)
                    qqHelper.DisposeString(_ExperienceRatingPeriodStatusTypeId_Original)
                    qqHelper.DisposeString(_Factor_Original)
                    qqHelper.DisposeString(_ModificationProductionDate_Original)
                    qqHelper.DisposeString(RatingEffectiveDate_Original)
                    qqHelper.DisposeString(_RiskIdentifier_Original)
                    '_HasChange = Nothing

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
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace

