Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteIsoTransmissionLookupInfo 'added 10/22/2016
        Enum AddressMatchType
            None = 0
            Normal = 1
            Alternate = 2
        End Enum
        Public Property IsoTransmissionId As Integer = 0
        Public Property PolicyId As Integer = 0
        Public Property PolicyImageNum As Integer = 0
        Public Property UnitNum As Integer = 0
        Public Property OrderedAddress As String = String.Empty
        Public Property ReturnedAddress As String = String.Empty
        Public ReadOnly Property HasAnyDistinguishableInfo As Boolean
            Get
                If IsoTransmissionId > 0 OrElse _PolicyId > 0 OrElse String.IsNullOrWhiteSpace(_OrderedAddress) = False OrElse String.IsNullOrWhiteSpace(_ReturnedAddress) = False Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property
        'Public Property GetXmlData As Boolean = False 'removed 10/22/2016 since ProcessPPC appears to handle importXml from isoTransmission (xml string) or db query result (encrypted string)
        Public Property ExportXml As String = String.Empty
        Public Property ImportXml As String = String.Empty

        'added 10/23/2016
        Public Property AlternateOrderedAddress As String = String.Empty
        Public Property AlternateReturnedAddress As String = String.Empty
        Public ReadOnly Property OrderedAddressIsDifferentThanAlternate As Boolean
            Get
                If UCase(_OrderedAddress) <> UCase(_AlternateOrderedAddress) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property
        Public ReadOnly Property ReturnedAddressIsDifferentThanAlternate As Boolean
            Get
                If UCase(_ReturnedAddress) <> UCase(_AlternateReturnedAddress) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property
        Public ReadOnly Property HasUsableAlternateOrderedAddress As Boolean
            Get
                If String.IsNullOrWhiteSpace(_AlternateOrderedAddress) = False AndAlso OrderedAddressIsDifferentThanAlternate = True Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property
        Public ReadOnly Property HasUsableAlternateReturnedAddress As Boolean
            Get
                If String.IsNullOrWhiteSpace(_AlternateReturnedAddress) = False AndAlso ReturnedAddressIsDifferentThanAlternate = True Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property
        Public Property OrderedAddressMatchType As AddressMatchType = AddressMatchType.None
        Public Property ReturnedAddressMatchType As AddressMatchType = AddressMatchType.None
    End Class
End Namespace
