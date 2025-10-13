Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Runtime.CompilerServices

Namespace Helpers
    Public Class Endorsement_ChangeBtnEnable
        'IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(PolicyNumber, ImageNum, PolicyID, ToolTip, val)

        Protected Shared _qqHelper As QuickQuoteHelperClass = Nothing
        ''' <summary>
        ''' This provides access to helper logic such as static data.
        ''' </summary>
        ''' <returns></returns>
        Protected Shared ReadOnly Property qqHelper As QuickQuoteHelperClass
            Get
                If _qqHelper Is Nothing Then
                    _qqHelper = New QuickQuoteHelperClass
                End If
                Return _qqHelper
            End Get
        End Property

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Queries if the change button should be enabled. </summary>
        '''
        ''' <remarks>   Chhaw, 05/30/2019. </remarks>
        '''
        ''' <param name="PolicyNumber"> [in,out] The policy number. </param>
        ''' <param name="ImageNum"> [in,out] The image number. </param>
        ''' <param name="PolicyId"> [in,out] Identifier for the policy. </param>
        ''' <param name="ToolTip">  [in,out] The tool tip of the button. </param>
        '''
        ''' <returns>   True if the change button should be enabled, false if not. </returns>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Shared Function IsChangeBtnEnabled(PolicyNumber As String, ByRef ImageNum As Integer, ByRef PolicyId As Integer, ByRef ToolTip As String, Optional ByVal okayToUseContext As Boolean = True) As Boolean
            'new 9/12/2019
            Dim strEnabled As String = ""
            Dim strTooltip As String = ""
            Dim strPolicyId As String = ""
            Dim strPolicyImageNum As String = ""
            If okayToUseContext = True Then
                GetEndorsementChangeButtonEnabledParamsFromContext(PolicyNumber, strEnabled, strTooltip, strPolicyId, strPolicyImageNum)
            End If

            If String.IsNullOrWhiteSpace(strEnabled) = False Then 'added IF 9/12/2019; original logic in ELSE
                PolicyId = qqHelper.IntegerForString(strPolicyId)
                ImageNum = qqHelper.IntegerForString(strPolicyImageNum)
                ToolTip = strTooltip
                Return qqHelper.BitToBoolean(strEnabled)
            Else
                Dim caughtDatabaseError As Boolean = False
                'We need the current PolicyID, the below returns the Latest PolicyID
                Dim policyResult As QuickQuotePolicyLookupInfo = QuickQuoteHelperClass.CurrentPolicyResultForPolicyNumber(PolicyNumber, caughtDatabaseError:=caughtDatabaseError)
                If policyResult IsNot Nothing Then
                    Dim enabledFlag As Boolean = False 'added 9/12/2019
                    PolicyId = policyResult.PolicyId
                    ImageNum = policyResult.PolicyImageNum
                    If policyResult.IsInforceOrFuture() Then
                        ToolTip = "Make a change to this policy"

                        'Dim polIdToCheck As Integer = 0
                        'If qqHelper.IsPositiveIntegerString(PolicyId) = True Then
                        '    polIdToCheck = CInt(PolicyId)
                        'End If
                        'If polIdToCheck > 0 Then 'note: could also just check for pending endorsement image for policy... a policy can have multiple policyIds... not sure if it's valid to enter endorsement on one policyId if there's already a pending endorsement on a different policyId
                        '    If QuickQuoteHelperClass.HasPendingEndorsementImage(policyId:=polIdToCheck) = True Then 'could pass in policyResult byref if we want to get the info for the pending endorsement image if it's found
                        '        ToolTip = "This policy record already has a pending endorsement"
                        '        Return False
                        '    End If
                        'End If
                        'updated 8/2/2019
                        If QuickQuoteHelperClass.HasPendingEndorsementImage(policyId:=PolicyId) = True Then 'could pass in policyResult byref if we want to get the info for the pending endorsement image if it's found
                            ToolTip = "This policy record already has a pending endorsement"
                            'Return False
                            'updated 9/12/2019
                            enabledFlag = False
                            'ElseIf qqHelper.HasAdvancedCancelWarning(polId:=PolicyId) = True Then
                            'updated 9/13/2019 based on prelimary findings where usp_AdvancedCancelWarnings executes better for polNum than policyId
                        ElseIf qqHelper.HasAdvancedCancelWarning(polNum:=PolicyNumber, polId:=PolicyId) = True Then
                            ToolTip = "Endorsements cannot be submitted on policies in notice of cancellation/expiration"
                            'Return False
                            'updated 9/12/2019
                            enabledFlag = False
                        Else 'added 9/12/2019
                            enabledFlag = True
                        End If
                        'Return True 'removed 9/12/2019
                    Else
                        ToolTip = "Endorsements can only be submitted on policies with a status of in-force or future"
                        'Return False
                        'updated 9/12/2019
                        enabledFlag = False
                    End If
                    'added 9/12/2019
                    If okayToUseContext = True Then
                        SetEndorsementChangeButtonEnabledParamsFromContext(PolicyNumber, enabledFlag.ToString, ToolTip, PolicyId.ToString, ImageNum.ToString)
                    End If
                    Return enabledFlag
                End If
            End If

            Return False
        End Function
        Public Shared Sub GetEndorsementChangeButtonEnabledParamsFromContext(ByVal polNum As String, ByRef strEnabled As String, ByRef strTooltip As String, ByRef strPolicyId As String, ByRef strPolicyImageNum As String)
            strEnabled = ""
            strTooltip = ""
            strPolicyId = ""
            strPolicyImageNum = ""

            If String.IsNullOrWhiteSpace(polNum) = False Then
                Dim context As HttpContext = System.Web.HttpContext.Current
                If context IsNot Nothing AndAlso context.Items.Keys IsNot Nothing Then
                    Dim endorsementChangeButtonEnabledPolNumText As String = "EndorsementChangeButtonEnabled_" & polNum
                    If context.Items(endorsementChangeButtonEnabledPolNumText & "_enabled") IsNot Nothing Then
                        strEnabled = DirectCast(context.Items(endorsementChangeButtonEnabledPolNumText & "_enabled"), String)
                        If context.Items(endorsementChangeButtonEnabledPolNumText & "_tooltip") IsNot Nothing Then
                            strTooltip = DirectCast(context.Items(endorsementChangeButtonEnabledPolNumText & "_tooltip"), String)
                        End If
                        If context.Items(endorsementChangeButtonEnabledPolNumText & "_policyid") IsNot Nothing Then
                            strPolicyId = DirectCast(context.Items(endorsementChangeButtonEnabledPolNumText & "_policyid"), String)
                        End If
                        If context.Items(endorsementChangeButtonEnabledPolNumText & "_policyimagenum") IsNot Nothing Then
                            strPolicyImageNum = DirectCast(context.Items(endorsementChangeButtonEnabledPolNumText & "_policyimagenum"), String)
                        End If
                    End If
                End If
            End If
        End Sub
        Public Shared Sub RemoveEndorsementChangeButtonEnabledParamsFromContext(ByVal polNum As String)
            If String.IsNullOrWhiteSpace(polNum) = False Then
                Dim context As HttpContext = System.Web.HttpContext.Current
                If context IsNot Nothing AndAlso context.Items.Keys IsNot Nothing Then
                    Dim endorsementChangeButtonEnabledPolNumText As String = "EndorsementChangeButtonEnabled_" & polNum
                    If context.Items(endorsementChangeButtonEnabledPolNumText & "_enabled") IsNot Nothing Then
                        context.Items.Remove(endorsementChangeButtonEnabledPolNumText & "_enabled")
                        If context.Items(endorsementChangeButtonEnabledPolNumText & "_tooltip") IsNot Nothing Then
                            context.Items.Remove(endorsementChangeButtonEnabledPolNumText & "_tooltip")
                        End If
                        If context.Items(endorsementChangeButtonEnabledPolNumText & "_policyid") IsNot Nothing Then
                            context.Items.Remove(endorsementChangeButtonEnabledPolNumText & "_policyid")
                        End If
                        If context.Items(endorsementChangeButtonEnabledPolNumText & "_policyimagenum") IsNot Nothing Then
                            context.Items.Remove(endorsementChangeButtonEnabledPolNumText & "_policyimagenum")
                        End If
                    End If
                End If
            End If
        End Sub
        Public Shared Sub SetEndorsementChangeButtonEnabledParamsFromContext(ByVal polNum As String, ByVal strEnabled As String, ByVal strTooltip As String, ByVal strPolicyId As String, ByVal strPolicyImageNum As String)
            If String.IsNullOrWhiteSpace(polNum) = False Then
                Dim context As HttpContext = System.Web.HttpContext.Current
                If context IsNot Nothing AndAlso context.Items.Keys IsNot Nothing Then
                    Dim endorsementChangeButtonEnabledPolNumText As String = "EndorsementChangeButtonEnabled_" & polNum
                    context.Items(endorsementChangeButtonEnabledPolNumText & "_enabled") = strEnabled
                    context.Items(endorsementChangeButtonEnabledPolNumText & "_tooltip") = strTooltip
                    context.Items(endorsementChangeButtonEnabledPolNumText & "_policyid") = strPolicyId
                    context.Items(endorsementChangeButtonEnabledPolNumText & "_policyimagenum") = strPolicyImageNum
                End If
            End If
        End Sub

    End Class
End Namespace

