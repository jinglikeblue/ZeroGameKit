//
//  UniWebViewAuthenticationFlowLine.cs
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
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A predefined authentication flow LINE Login.
///
/// This implementation follows the flow described here:
/// https://developers.line.biz/en/reference/line-login/
///
/// Google authentication flow is a bit different from the other standard authentication flows. Please read the link
/// above carefully to understand it.
///
/// See https://docs.uniwebview.com/guide/oauth2.html for a more detailed guide of authentication in UniWebView.
/// </summary>
public class UniWebViewAuthenticationFlowLine : UniWebViewAuthenticationCommonFlow, IUniWebViewAuthenticationFlow<UniWebViewAuthenticationLineToken> {
    /// <summary>
    /// The client ID (Channel ID) of your LINE Login application.
    /// </summary>
    public string clientId = "";
    
    /// <summary>
    /// The iOS bundle Id you set in LINE developer console.
    /// </summary>
    public string iOSBundleId = "";
    
    /// <summary>
    /// The Android package name you set in LINE developer console.
    /// </summary>
    public string androidPackageName = "";
    
    /// <summary>
    /// The scope of your LINE application.
    /// </summary>
    public string scope = "";
    
    /// <summary>
    /// Optional to control this flow's behaviour.
    /// </summary>
    public UniWebViewAuthenticationFlowLineOptional optional;
    
    private const string responseType = "code";
    private const string grantType = "authorization_code";

    private string RedirectUri {
        get {
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor) {
                return $"line3rdp.{iOSBundleId}://auth";
            }

            if (Application.platform == RuntimePlatform.Android) {
                return "intent://auth#Intent;package=" + androidPackageName + ";scheme=lineauth;end";
            }
            UniWebViewLogger.Instance.Critical("Not supported platform for LINE Login.");
            return "";
        }
    }

    private readonly UniWebViewAuthenticationConfiguration config = 
        new UniWebViewAuthenticationConfiguration(
            "https://access.line.me/oauth2/v2.1/login", 
            "https://api.line.me/oauth2/v2.1/token"
        );

    /// <summary>
    /// Starts the authentication flow with the standard OAuth 2.0.
    /// This implements the abstract method in `UniWebViewAuthenticationCommonFlow`.
    /// </summary>
    public override void StartAuthenticationFlow() {
        var flow = new UniWebViewAuthenticationFlow<UniWebViewAuthenticationLineToken>(this);
        flow.StartAuth();
    }

    /// <summary>
    /// Starts the refresh flow with the standard OAuth 2.0.
    /// This implements the abstract method in `UniWebViewAuthenticationCommonFlow`.
    /// </summary>
    /// <param name="refreshToken">The refresh token received with a previous access token response.</param>
    public override void StartRefreshTokenFlow(string refreshToken) {
        var flow = new UniWebViewAuthenticationFlow<UniWebViewAuthenticationLineToken>(this);
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
        return RedirectUri;
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public Dictionary<string, string> GetAuthenticationUriArguments() {
        var authorizeArgs = new Dictionary<string, string> {
            { "loginChannelId", clientId },
            { "returnUri", GenerateReturnUri() },
        };
        
        return authorizeArgs;
    }

    private string GenerateReturnUri() {
        var query = new Dictionary<string, string> {
            { "response_type", responseType },
            { "client_id", clientId },
            { "redirect_uri", RedirectUri }
        };

        // State is a must in LINE Login.
        var state = GenerateAndStoreState();
        query.Add("state", state);
        
        if (!String.IsNullOrEmpty(scope)) {
            query.Add("scope", scope);
        } else {
            query.Add("scope", "profile");
        }
        if (optional != null) {
            if (optional.PKCESupport != UniWebViewAuthenticationPKCE.None) {
                var codeChallenge = GenerateCodeChallengeAndStoreCodeVerify(optional.PKCESupport);
                query.Add("code_challenge", codeChallenge);
        
                var method = UniWebViewAuthenticationUtils.ConvertPKCEToString(optional.PKCESupport);
                query.Add("code_challenge_method", method);
            }
        }
        return "/oauth2/v2.1/authorize/consent?" + UniWebViewAuthenticationUtils.CreateQueryString(query);
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public Dictionary<string, string> GetAccessTokenRequestParameters(string authResponse) {
        var normalizedRedirectUri = UniWebViewAuthenticationUtils.ConvertIntentUri(RedirectUri);
        if (!authResponse.StartsWith(normalizedRedirectUri, StringComparison.InvariantCultureIgnoreCase)) {
            throw AuthenticationResponseException.UnexpectedAuthCallbackUrl;
        }
        
        var uri = new Uri(authResponse);
        var response = UniWebViewAuthenticationUtils.ParseFormUrlEncodedString(uri.Query);
        VerifyState(response);
        if (!response.TryGetValue("code", out var code)) {
            throw AuthenticationResponseException.InvalidResponse(authResponse);
        }
        var parameters = new Dictionary<string, string> {
            { "client_id", clientId },
            { "code", code },
            { "redirect_uri", RedirectUri },
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
    public UniWebViewAuthenticationLineToken GenerateTokenFromExchangeResponse(string exchangeResponse) {
        return UniWebViewAuthenticationTokenFactory<UniWebViewAuthenticationLineToken>.Parse(exchangeResponse);
    }

    [field: SerializeField]
    public UnityEvent<UniWebViewAuthenticationLineToken> OnAuthenticationFinished { get; set; }
    [field: SerializeField]
    public UnityEvent<long, string> OnAuthenticationErrored { get; set; }
    [field: SerializeField]
    public UnityEvent<UniWebViewAuthenticationLineToken> OnRefreshTokenFinished { get; set;  }
    [field: SerializeField]
    public UnityEvent<long, string> OnRefreshTokenErrored { get; set; }
}

/// <summary>
/// The authentication flow's optional settings for LINE.
/// </summary>
[Serializable]
public class UniWebViewAuthenticationFlowLineOptional {
    /// <summary>
    /// Whether to enable PKCE when performing authentication. Default is `S256`.
    /// </summary>
    public UniWebViewAuthenticationPKCE PKCESupport = UniWebViewAuthenticationPKCE.S256;
}

/// <summary>
/// The token object from LINE. Check `UniWebViewAuthenticationStandardToken` for more.
/// </summary>
public class UniWebViewAuthenticationLineToken : UniWebViewAuthenticationStandardToken { }
