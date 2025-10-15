Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store top-level premium information for a quote; includes properties that were previously on QuickQuote only
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteTopLevelQuotePremiums 'added 7/27/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _TotalQuotedPremium As String

        Private _AnnualPremium As String 'PolicyImage.premium_annual
        Private _ChangeInFullTermPremium As String 'PolicyImage.premium_chg_fullterm
        Private _ChangeInWrittenPremium As String 'PolicyImage.premium_chg_written
        Private _DifferenceChangeInFullTermPremium As String 'PolicyImage.premium_diff_chg_fullterm
        Private _DifferenceChangeInWrittenPremium As String 'PolicyImage.premium_diff_chg_written
        Private _FullTermPremium As String 'PolicyImage.premium_fullterm
        Private _FullTermPremiumOffsetForPreviousImage As String 'PolicyImage.ftp_offset_for_prev_image
        Private _FullTermPremiumOnsetForCurrent As String 'PolicyImage.ftp_onset_for_current
        Private _OffsetPremiumForPreviousImage As String 'PolicyImage.offset_for_prev_image
        Private _OnsetPremiumForCurrentImage As String 'PolicyImage.onset_for_current
        Private _PreviousWrittenPremium As String 'PolicyImage.premium_previous_written
        Private _WrittenPremium As String 'PolicyImage.premium_written
        Private _PriorTermAnnual As String 'PolicyImage.prior_term_annual_premium
        Private _PriorTermFullterm As String 'PolicyImage.prior_term_fullterm

        Private _TotalQuotedPremiumType As QuickQuoteHelperClass.PremiumType 'added 7/28/2018


        Public Property TotalQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalQuotedPremium)
            End Get
            Set(value As String)
                _TotalQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalQuotedPremium)
            End Set
        End Property
        Public ReadOnly Property AnnualPremium As String 'PolicyImage.premium_annual
            Get
                Return qqHelper.QuotedPremiumFormat(_AnnualPremium)
            End Get
        End Property
        Public ReadOnly Property ChangeInFullTermPremium As String 'PolicyImage.premium_chg_fullterm
            Get
                Return qqHelper.QuotedPremiumFormat(_ChangeInFullTermPremium)
            End Get
        End Property
        Public ReadOnly Property ChangeInWrittenPremium As String 'PolicyImage.premium_chg_written
            Get
                Return qqHelper.QuotedPremiumFormat(_ChangeInWrittenPremium)
            End Get
        End Property
        Public ReadOnly Property DifferenceChangeInFullTermPremium As String 'PolicyImage.premium_diff_chg_fullterm
            Get
                Return qqHelper.QuotedPremiumFormat(_DifferenceChangeInFullTermPremium)
            End Get
        End Property
        Public ReadOnly Property DifferenceChangeInWrittenPremium As String 'PolicyImage.premium_diff_chg_written
            Get
                Return qqHelper.QuotedPremiumFormat(_DifferenceChangeInWrittenPremium)
            End Get
        End Property
        Public ReadOnly Property FullTermPremium As String 'PolicyImage.premium_fullterm
            Get
                Return qqHelper.QuotedPremiumFormat(_FullTermPremium)
            End Get
        End Property
        Public ReadOnly Property FullTermPremiumOffsetForPreviousImage As String 'PolicyImage.ftp_offset_for_prev_image
            Get
                Return qqHelper.QuotedPremiumFormat(_FullTermPremiumOffsetForPreviousImage)
            End Get
        End Property
        Public ReadOnly Property FullTermPremiumOnsetForCurrent As String 'PolicyImage.ftp_onset_for_current
            Get
                Return qqHelper.QuotedPremiumFormat(_FullTermPremiumOnsetForCurrent)
            End Get
        End Property
        Public ReadOnly Property OffsetPremiumForPreviousImage As String 'PolicyImage.offset_for_prev_image
            Get
                Return qqHelper.QuotedPremiumFormat(_OffsetPremiumForPreviousImage)
            End Get
        End Property
        Public ReadOnly Property OnsetPremiumForCurrentImage As String 'PolicyImage.onset_for_current
            Get
                Return qqHelper.QuotedPremiumFormat(_OnsetPremiumForCurrentImage)
            End Get
        End Property
        Public ReadOnly Property PreviousWrittenPremium As String 'PolicyImage.premium_previous_written
            Get
                Return qqHelper.QuotedPremiumFormat(_PreviousWrittenPremium)
            End Get
        End Property
        Public ReadOnly Property WrittenPremium As String 'PolicyImage.premium_written
            Get
                Return qqHelper.QuotedPremiumFormat(_WrittenPremium)
            End Get
        End Property
        Public ReadOnly Property PriorTermAnnual As String 'PolicyImage.prior_term_annual_premium
            Get
                Return qqHelper.QuotedPremiumFormat(_PriorTermAnnual)
            End Get
        End Property
        Public ReadOnly Property PriorTermFullterm As String 'PolicyImage.prior_term_fullterm
            Get
                Return qqHelper.QuotedPremiumFormat(_PriorTermFullterm)
            End Get
        End Property

        Public ReadOnly Property TotalQuotedPremiumType As QuickQuoteHelperClass.PremiumType 'added 7/28/2018
            Get
                Return _TotalQuotedPremiumType
            End Get
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Public Sub New(Parent As Object) 'generic, but Parent will likely be TopLevelQuoteInfo
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()
            _TotalQuotedPremium = ""

            _AnnualPremium = "" 'PolicyImage.premium_annual
            _ChangeInFullTermPremium = "" 'PolicyImage.premium_chg_fullterm
            _ChangeInWrittenPremium = "" 'PolicyImage.premium_chg_written
            _DifferenceChangeInFullTermPremium = "" 'PolicyImage.premium_diff_chg_fullterm
            _DifferenceChangeInWrittenPremium = "" 'PolicyImage.premium_diff_chg_written
            _FullTermPremium = "" 'PolicyImage.premium_fullterm
            _FullTermPremiumOffsetForPreviousImage = "" 'PolicyImage.ftp_offset_for_prev_image
            _FullTermPremiumOnsetForCurrent = "" 'PolicyImage.ftp_onset_for_current
            _OffsetPremiumForPreviousImage = "" 'PolicyImage.offset_for_prev_image
            _OnsetPremiumForCurrentImage = "" 'PolicyImage.onset_for_current
            _PreviousWrittenPremium = "" 'PolicyImage.premium_previous_written
            _WrittenPremium = "" 'PolicyImage.premium_written
            _PriorTermAnnual = "" 'PolicyImage.prior_term_annual_premium
            _PriorTermFullterm = "" 'PolicyImage.prior_term_fullterm

            _TotalQuotedPremiumType = QuickQuoteHelperClass.PremiumType.None 'added 7/28/2018

        End Sub
        Protected Friend Sub Set_AnnualPremium(ByVal prem As String) 'PolicyImage.premium_annual
            _AnnualPremium = prem
        End Sub
        Protected Friend Sub Set_ChangeInFullTermPremium(ByVal prem As String) 'PolicyImage.premium_chg_fullterm
            _ChangeInFullTermPremium = prem
        End Sub
        Protected Friend Sub Set_ChangeInWrittenPremium(ByVal prem As String) 'PolicyImage.premium_chg_written
            _ChangeInWrittenPremium = prem
        End Sub
        Protected Friend Sub Set_DifferenceChangeInFullTermPremium(ByVal prem As String) 'PolicyImage.premium_diff_chg_fullterm
            _DifferenceChangeInFullTermPremium = prem
        End Sub
        Protected Friend Sub Set_DifferenceChangeInWrittenPremium(ByVal prem As String) 'PolicyImage.premium_diff_chg_written
            _DifferenceChangeInWrittenPremium = prem
        End Sub
        Protected Friend Sub Set_FullTermPremium(ByVal prem As String) 'PolicyImage.premium_fullterm
            _FullTermPremium = prem
        End Sub
        Protected Friend Sub Set_FullTermPremiumOffsetForPreviousImage(ByVal prem As String) 'PolicyImage.ftp_offset_for_prev_image
            _FullTermPremiumOffsetForPreviousImage = prem
        End Sub
        Protected Friend Sub Set_FullTermPremiumOnsetForCurrent(ByVal prem As String) 'PolicyImage.ftp_onset_for_current
            _FullTermPremiumOnsetForCurrent = prem
        End Sub
        Protected Friend Sub Set_OffsetPremiumForPreviousImage(ByVal prem As String) 'PolicyImage.offset_for_prev_image
            _OffsetPremiumForPreviousImage = prem
        End Sub
        Protected Friend Sub Set_OnsetPremiumForCurrentImage(ByVal prem As String) 'PolicyImage.onset_for_current
            _OnsetPremiumForCurrentImage = prem
        End Sub
        Protected Friend Sub Set_PreviousWrittenPremium(ByVal prem As String) 'PolicyImage.premium_previous_written
            _PreviousWrittenPremium = prem
        End Sub
        Protected Friend Sub Set_WrittenPremium(ByVal prem As String) 'PolicyImage.premium_written
            _WrittenPremium = prem
        End Sub
        Protected Friend Sub Set_PriorTermAnnual(ByVal prem As String) 'PolicyImage.prior_term_annual_premium
            _PriorTermAnnual = prem
        End Sub
        Protected Friend Sub Set_PriorTermFullterm(ByVal prem As String) 'PolicyImage.prior_term_fullterm
            _PriorTermFullterm = prem
        End Sub

        Protected Friend Sub Set_TotalQuotedPremiumType(ByVal premType As QuickQuoteHelperClass.PremiumType) 'added 7/28/2018
            _TotalQuotedPremiumType = premType
        End Sub


        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.TotalQuotedPremium <> "" Then 'added 6/30/2015
                    str = qqHelper.appendText(str, "TotalQuotedPremium: " & Me.TotalQuotedPremium, vbCrLf)
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

                    qqHelper.DisposeString(_TotalQuotedPremium)
                    qqHelper.DisposeString(_AnnualPremium) 'PolicyImage.premium_annual
                    qqHelper.DisposeString(_ChangeInFullTermPremium) 'PolicyImage.premium_chg_fullterm
                    qqHelper.DisposeString(_ChangeInWrittenPremium) 'PolicyImage.premium_chg_written
                    qqHelper.DisposeString(_DifferenceChangeInFullTermPremium) 'PolicyImage.premium_diff_chg_fullterm
                    qqHelper.DisposeString(_DifferenceChangeInWrittenPremium) 'PolicyImage.premium_diff_chg_written
                    qqHelper.DisposeString(_FullTermPremium) 'PolicyImage.premium_fullterm
                    qqHelper.DisposeString(_FullTermPremiumOffsetForPreviousImage) 'PolicyImage.ftp_offset_for_prev_image
                    qqHelper.DisposeString(_FullTermPremiumOnsetForCurrent) 'PolicyImage.ftp_onset_for_current
                    qqHelper.DisposeString(_OffsetPremiumForPreviousImage) 'PolicyImage.offset_for_prev_image
                    qqHelper.DisposeString(_OnsetPremiumForCurrentImage) 'PolicyImage.onset_for_current
                    qqHelper.DisposeString(_PreviousWrittenPremium) 'PolicyImage.premium_previous_written
                    qqHelper.DisposeString(_WrittenPremium) 'PolicyImage.premium_written
                    qqHelper.DisposeString(_PriorTermAnnual) 'PolicyImage.prior_term_annual_premium
                    qqHelper.DisposeString(_PriorTermFullterm) 'PolicyImage.prior_term_fullterm

                    _TotalQuotedPremiumType = Nothing 'added 7/28/2018

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


