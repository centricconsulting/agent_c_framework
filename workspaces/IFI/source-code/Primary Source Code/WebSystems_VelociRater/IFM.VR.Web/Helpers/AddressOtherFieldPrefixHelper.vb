Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports System.Text.RegularExpressions
Imports Diamond.Business.ThirdParty.BMSAssignmentServiceObjects
Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Cryptography.Pkcs
Imports PublicQuotingLib.Models
Imports Diamond.Common.StaticDataManager.Objects.SystemData

Namespace Helpers
    Public Class AddressOtherFieldPrefixHelper

        Private Shared Property regEx As Regex

        Public Const CareOfRegex As String = "^\s*c(are|.|\/) ?o(\.|f)?:?\s*"
        Public Const AttnRegex As String = "^\s*a(ttn|ttention) ?:?\s*"

        Enum OtherFieldPrefix
            Other = 0
            CareOf = 1
            Attention = 2
        End Enum

        Public Shared Function GetPrefix(input As String) As OtherFieldPrefix
            If String.IsNullOrWhiteSpace(input) = False Then
                If isCareOf(input) Then
                    Return OtherFieldPrefix.CareOf
                ElseIf isAttn(input) Then
                    Return OtherFieldPrefix.Attention
                End If
            End If
            Return OtherFieldPrefix.Other
        End Function

        Public Shared Function GetPrefixType(input As String) As OtherFieldPrefix
            If String.IsNullOrWhiteSpace(input) = False Then
                Select Case input.TryToGetInt32
                    Case 1
                        Return OtherFieldPrefix.CareOf
                    Case 2
                        Return OtherFieldPrefix.Attention
                    Case Else
                        Return String.Empty
                End Select
            End If
        End Function

        Private Shared Function isAttn(input As String) As Boolean
            If RegexTest(AttnRegex, input) Then
                Return True
            End If
            Return False
        End Function

        Private Shared Function isCareOf(input As String) As Boolean
            If RegexTest(CareOfRegex, input) Then
                Return True
            End If
            Return False
        End Function

        Public Shared Function GetPrefixStringFromInput(input As String) As String
            Dim type As OtherFieldPrefix = GetPrefix(input)
            Return GetPrefixStringFromType(type)
        End Function

        Public Shared Function GetPrefixStringFromType(type As AddressOtherFieldPrefixHelper.OtherFieldPrefix)
            Select Case type
                Case OtherFieldPrefix.Attention
                    Return "ATTN "
                Case OtherFieldPrefix.CareOf
                    Return "C/O "
                Case Else
                    Return String.Empty
            End Select
        End Function


        Public Shared Function GetNameWithoutPrefix(input) As String
            Dim Type = GetPrefix(input)
            Select Case Type
                Case OtherFieldPrefix.Attention
                    Return RemovePrefix(AttnRegex, input)
                Case OtherFieldPrefix.CareOf
                    Return RemovePrefix(CareOfRegex, input)
                Case Else
                    Return input
            End Select
        End Function

        Public Shared Function GetNameWithPrefix(input) As String
            Dim Type = GetPrefix(input)
            Select Case Type
                Case OtherFieldPrefix.Attention
                    Return GetPrefixStringFromInput(input) & RemovePrefix(AttnRegex, input)
                Case OtherFieldPrefix.CareOf
                    Return GetPrefixStringFromInput(input) & RemovePrefix(CareOfRegex, input)
                Case Else
                    Return input
            End Select
        End Function

        Public Shared Function RemovePrefix(pattern As String, input As String) As String
            Dim options As RegexOptions = RegexOptions.IgnoreCase

            regEx = New Regex(pattern, options)
            Return Regex.Replace(input, String.Empty)
        End Function

        Private Shared Function RegexTest(pattern As String, input As String) As Boolean
            Dim options As RegexOptions = RegexOptions.IgnoreCase

            regEx = New Regex(pattern, options)
            Return Regex.IsMatch(input)
        End Function

        Public Shared Function ChangeType(input As String, type As OtherFieldPrefix) As String
            If String.IsNullOrWhiteSpace(input) = False Then
                Dim NameWithoutOldPrefix As String = GetNameWithoutPrefix(input)
                Dim Prefix As String = GetPrefixStringFromType(type)
                Return Prefix & NameWithoutOldPrefix
            End If
        End Function
    End Class
End Namespace
