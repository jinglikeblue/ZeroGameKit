//
//  UniWebViewAuthenticationFlowGoogle.cs
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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A predefined authentication flow for Google Identity.
///
/// This implementation follows the flow described here:
/// https://developers.google.com/identity/protocols/oauth2/native-app
///
/// Google authentication flow is a bit different from the other standard authentication flows. Please read the link
/// above carefully to understand it.
///
/// See https://docs.uniwebview.com/guide/oauth2.html for a more detailed guide of authentication in UniWebView.
/// </summary>
public class UniWebViewAuthenticationFlowGoogle : UniWebViewAuthenticationCommonFlow, IUniWebViewAuthenticationFlow<UniWebViewAuthenticationGoogleToken> {
    /// <summary>
    /// The client ID of your Google application.
    /// </summary>
    public string clientId = "";
    
    /// <summary>
    /// The redirect URI of your Google application.
    ///
    /// It might be something like "com.googleusercontent.apps.${clientId}:${redirect_uri_path}". Be caution that the URI does not
    /// contain regular double slashes `//`, but should be only one.
    /// </summary>
    public string redirectUri = "";
    
    /// <summary>
    /// The scope of your Google application.
    ///
    /// It might be some full URL in recent Google services, such as "https://www.googleapis.com/auth/userinfo.profile" 
    /// </summary>
    public string scope = "";
    
    /// <summary>
    /// Optional to control this flow's behaviour.
    /// </summary>
    public UniWebViewAuthenticationFlowGoogleOptional optional;
    
    private const string responseType = "code";
    private const string grantType = "authorization_code";

    private readonly UniWebViewAuthenticationConfiguration config = 
        new UniWebViewAuthenticationConfiguration(
            "https://accounts.google.com/o/oauth2/v2/auth", 
            "https://oauth2.googleapis.com/token"
        );

    /// <summary>
    /// Starts the authentication flow with the standard OAuth 2.0.
    /// This implements the abstract method in `UniWebViewAuthenticationCommonFlow`.
    /// </summary>
    public override void StartAuthenticationFlow() {
        var flow = new UniWebViewAuthenticationFlow<UniWebViewAuthenticationGoogleToken>(this);
        flow.StartAuth();
    }

    /// <summary>
    /// Starts the refresh flow with the standard OAuth 2.0.
    /// This implements the abstract method in `UniWebViewAuthenticationCommonFlow`.
    /// </summary>
    /// <param name="refreshToken">The refresh token received with a previous access token response.</param>
    public override void StartRefreshTokenFlow(string refreshToken) {
        var flow = new UniWebViewAuthenticationFlow<UniWebViewAuthenticationGoogleToken>(this);
        flow.RefreshToken(refreshToken);
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public UniWebViewAuthenticationConfiguration GetAuthenticationConfiguration() {
        return config;
    }
    
    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public string GetCallbackUrl() {
        return redirectUri;
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public Dictionary<string, string> GetAuthenticationUriArguments() {
        var authorizeArgs = new Dictionary<string, string> {
            { "client_id", clientId },
            { "redirect_uri", redirectUri },
            { "scope", scope },
            { "response_type", responseType }
        };
        if (optional != null) {
            if (optional.enableState) {
                var state = GenerateAndStoreState();
                authorizeArgs.Add("state", state);
            }

            if (optional.PKCESupport != UniWebViewAuthenticationPKCE.None) {
                var codeChallenge = GenerateCodeChallengeAndStoreCodeVerify(optional.PKCESupport);
                authorizeArgs.Add("code_challenge", codeChallenge);

                var method = UniWebViewAuthenticationUtils.ConvertPKCEToString(optional.PKCESupport);
                authorizeArgs.Add("code_challenge_method", method);
            }

            if (!String.IsNullOrEmpty(optional.loginHint)) {
                authorizeArgs.Add("login_hint", optional.loginHint);
            }
        }

        return authorizeArgs;
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public Dictionary<string, string> GetAccessTokenRequestParameters(string authResponse) {
        if (!authResponse.StartsWith(redirectUri, StringComparison.InvariantCultureIgnoreCase)) {
            throw AuthenticationResponseException.UnexpectedAuthCallbackUrl;
        }
        
        var uri = new Uri(authResponse);
        var response = UniWebViewAuthenticationUtils.ParseFormUrlEncodedString(uri.Query);
        if (!response.TryGetValue("code", out var code)) {
            throw AuthenticationResponseException.InvalidResponse(authResponse);
        }
        if (optional.enableState) {
            VerifyState(response);
        }
        var parameters = new Dictionary<string, string> {
            { "client_id", clientId },
            { "code", code },
            { "redirect_uri", redirectUri },
            { "grant_type", grantType },
        };
        if (CodeVerify != null) {
            parameters.Add("code_verifier", CodeVerify);
        }

        return parameters;
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public Dictionary<string, string> GetRefreshTokenRequestParameters(string refreshToken) {
        return new Dictionary<string, string> {
            { "client_id", clientId },
            { "refresh_token", refreshToken },
            { "grant_type", "refresh_token" }
        };
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public UniWebViewAuthenticationGoogleToken GenerateTokenFromExchangeResponse(string exchangeResponse) {
        return UniWebViewAuthenticationTokenFactory<UniWebViewAuthenticationGoogleToken>.Parse(exchangeResponse);
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<UniWebViewAuthenticationGoogleToken> OnAuthenticationFinished { get; set; }
    
    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<long, string> OnAuthenticationErrored { get; set; }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<UniWebViewAuthenticationGoogleToken> OnRefreshTokenFinished { get; set; }
    
    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<long, string> OnRefreshTokenErrored { get; set; }
}

/// <summary>
/// The authentication flow's optional settings for Google.
/// </summary>
[Serializable]
public class UniWebViewAuthenticationFlowGoogleOptional {
    /// <summary>
    /// Whether to enable PKCE when performing authentication. Default is `S256`.
    /// </summary>
    public UniWebViewAuthenticationPKCE PKCESupport = UniWebViewAuthenticationPKCE.S256;
    /// <summary>
    /// Whether to enable the state verification. If enabled, the state will be generated and verified in the
    /// authentication callback. Default is `true`.
    /// </summary>
    public bool enableState = true;
    /// <summary>
    /// If your application knows which user is trying to authenticate, it can use this parameter to provide a hint to
    /// the Google Authentication Server. 
    /// </summary>
    public string loginHint = "";
}

/// <summary>
/// The token object from Google. Check `UniWebViewAuthenticationStandardToken` for more.
/// </summary>
public class UniWebViewAuthenticationGoogleToken : UniWebViewAuthenticationStandardToken { }

