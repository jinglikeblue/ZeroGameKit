//
//  UniWebViewAuthenticationCommonFlow.cs
//  Created by Wang Wei (@onevcat) on 2022-06-25.
//
//  This file is a part of UniWebView Project (https://uniwebview.com)
//  By purchasing the asset, you are allowed to use this code in as many as projects 
//  you want, only if you publish the final products under the name of the same account
//  used for the purchase. 
//
//  This asset and all corresponding files (such as source code) are provided on an 
//  “as is” basis, without warranty of any kind, express of implied, including but not
//  limited to the warranties of merchantability, fitness for a particular purpose, and 
//  noninfringement. In no event shall the authors or copyright holders be liable for any 
//  claim, damages or other liability, whether in action of contract, tort or otherwise, 
//  arising from, out of or in connection with the software or the use of other dealing in the software.
//

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class and general control for other authentication flows. This class determines the global behaviors of the 
/// authentication flow, such as whether to start authentication as soon as the script `Start`s, and whether to use private
/// mode to authenticate the user.

/// This is a super and abstract class for all concrete auth flow. You are not expected to use this class directly.
/// Instead, to start a customized auth flow, you can use the `UniWebViewAuthenticationFlowCustomize` class.
/// </summary>
public abstract class UniWebViewAuthenticationCommonFlow: MonoBehaviour {
    /// <summary>
    /// Whether to start authentication as soon as the script `Start`s.
    /// </summary>
    public bool authorizeOnStart;
    /// <summary>
    /// Whether to use private mode to authenticate the user. If `true` and the device supports, the authentication
    /// will begin under the incognito mode.
    ///
    /// On iOS, this works on iOS 13 and later.
    ///
    /// On Android, it depends on the Chrome version and might require users to enable the incognito mode (and support
    /// for third-party use) in Chrome's settings. Check settings with `chrome://flags/#cct-incognito` and
    /// `chrome://flags/#cct-incognito-available-to-third-party` in Chrome to see the current status.
    /// </summary>
    public bool privateMode;
    
    // Security. Store the state.
    private string state;
    // Security. Store the code challenge verifier. 

    protected string CodeVerify { get; private set; }

    public void Start() {
        if (authorizeOnStart) {
            StartAuthenticationFlow();
        }
    }
    
    /// <summary>
    /// Subclass should override this method to start the authentication flow. Usually it starts
    /// a `UniWebViewAuthenticationFlow`. But you can also choose whatever you need to do. 
    /// </summary>
    public abstract void StartAuthenticationFlow();

    /// <summary>
    /// Subclass should override this method to start the authentication flow. Usually it starts
    /// a Unity Web Request against the authentication flow's token entry point to refresh the token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    public abstract void StartRefreshTokenFlow(string refreshToken);

    // Child classes are expected to call this method to request a `state` (and store it for later check) if the 
    // `state` verification is enabled.
    protected string GenerateAndStoreState() {
        state = UniWebViewAuthenticationUtils.GenerateRandomBase64URLString();
        return state;
    }

    // Child classes are expected to call this method to request a `code_challenge`. Later when exchanging the access
    // token, the `code_verifier` will be used to verify the `code_challenge`. Subclass can read it from `CodeVerify`.
    protected string GenerateCodeChallengeAndStoreCodeVerify(UniWebViewAuthenticationPKCE method) {
        CodeVerify = UniWebViewAuthenticationUtils.GenerateCodeVerifier();
        return UniWebViewAuthenticationUtils.CalculateCodeChallenge(CodeVerify, method);
    }
    
    // Perform verifying for `state`.
    protected void VerifyState(Dictionary<string, string> parameters, string key = "state") {
        if (state == null) {
            throw AuthenticationResponseException.InvalidState;
        }
        if (!parameters.TryGetValue(key, out var stateInResponse) || state != stateInResponse) {
            throw AuthenticationResponseException.InvalidState;
        }
    }
}
