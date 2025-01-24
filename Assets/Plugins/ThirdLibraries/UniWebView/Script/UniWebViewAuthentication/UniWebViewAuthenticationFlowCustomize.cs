//
//  UniWebViewAuthenticationFlowCustomize.cs
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
/// A customizable authentication flow behavior.
/// </summary>
/// <remarks>
/// Besides of the predefined authentication flows, such as Twitter (`UniWebViewAuthenticationFlowTwitter`) or Google
/// (`UniWebViewAuthenticationFlowGoogle`), this class allows you to determine the details of the authentication flow,
/// such as entry points, grant types, scopes and more. But similar to other target-specified flows, it follows the same
/// OAuth 2.0 code auth pattern.
///
/// If you need to support other authentication flows for the platform targets other than the predefined ones, you can
/// use this class and set all necessary parameters. It runs the standard OAuth 2.0 flow and gives out a
/// `UniWebViewAuthenticationStandardToken` as the result.
///
/// If you need to support authentication flows other than `code` based OAuth 2.0, try to derive from
/// `UniWebViewAuthenticationCommonFlow` and implement `IUniWebViewAuthenticationFlow` interface, or even use the 
/// underneath `UniWebViewAuthenticationSession` to get a highly customizable flow.
/// 
/// </remarks>
public class UniWebViewAuthenticationFlowCustomize : UniWebViewAuthenticationCommonFlow, IUniWebViewAuthenticationFlow<UniWebViewAuthenticationStandardToken> {

    /// <summary>
    /// The config object which defines the basic information of the authentication flow.
    /// </summary>
    public UniWebViewAuthenticationFlowCustomizeConfig config = new UniWebViewAuthenticationFlowCustomizeConfig();

    /// <summary>
    /// The client Id of your OAuth application.
    /// </summary>
    public string clientId = "";

    /// <summary>
    /// The redirect URI of your OAuth application. The service provider is expected to call this URI to pass back the
    /// authorization code. It should be something also set to your OAuth application.
    ///
    /// Also remember to add it to the "Auth Callback Urls" field in UniWebView's preference panel. 
    /// </summary>
    public string redirectUri = "";
    
    /// <summary>
    /// The scope of the authentication request.
    /// </summary>
    public string scope = "";

    /// <summary>
    /// The optional object which defines some optional parameters of the authentication flow, such as whether supports
    /// `state` or `PKCE`.
    /// </summary>
    public UniWebViewAuthenticationFlowCustomizeOptional optional;

    /// <summary>
    /// Starts the authentication flow with the standard OAuth 2.0.
    /// This implements the abstract method in `UniWebViewAuthenticationCommonFlow`.
    /// </summary>
    public override void StartAuthenticationFlow() {
        var flow = new UniWebViewAuthenticationFlow<UniWebViewAuthenticationStandardToken>(this);
        flow.StartAuth();
    }

    /// <summary>
    /// Starts the refresh flow with the standard OAuth 2.0.
    /// This implements the abstract method in `UniWebViewAuthenticationCommonFlow`.
    /// </summary>
    /// <param name="refreshToken">The refresh token received with a previous access token response.</param>
    public override void StartRefreshTokenFlow(string refreshToken) {
        var flow = new UniWebViewAuthenticationFlow<UniWebViewAuthenticationStandardToken>(this);
        flow.RefreshToken(refreshToken);
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public virtual UniWebViewAuthenticationConfiguration GetAuthenticationConfiguration() {
        return new UniWebViewAuthenticationConfiguration(
            config.authorizationEndpoint, 
            config.tokenEndpoint
        );
    }
    
    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public virtual string GetCallbackUrl() {
        return redirectUri;
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public virtual Dictionary<string, string> GetAuthenticationUriArguments() {
        var authorizeArgs = new Dictionary<string, string> {
            { "client_id", clientId },
            { "redirect_uri", redirectUri },
            { "scope", scope },
            { "response_type", config.responseType }
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
        }

        return authorizeArgs;
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public virtual Dictionary<string, string> GetAccessTokenRequestParameters(string authResponse) {
        if (!authResponse.StartsWith(redirectUri, StringComparison.InvariantCultureIgnoreCase)) {
            throw AuthenticationResponseException.UnexpectedAuthCallbackUrl;
        }
        
        var uri = new Uri(authResponse);
        var response = UniWebViewAuthenticationUtils.ParseFormUrlEncodedString(uri.Query);
        if (!response.TryGetValue("code", out var code)) {
            throw AuthenticationResponseException.InvalidResponse(authResponse);
        }
        if (optional != null && optional.enableState) {
            VerifyState(response);
        }
        var parameters = new Dictionary<string, string> {
            { "client_id", clientId },
            { "code", code },
            { "redirect_uri", redirectUri },
            { "grant_type", config.grantType },
        };
        if (CodeVerify != null) {
            parameters.Add("code_verifier", CodeVerify);
        }
        if (optional != null && !String.IsNullOrEmpty(optional.clientSecret)) {
            parameters.Add("client_secret", optional.clientSecret);
        }

        return parameters;
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public virtual Dictionary<string, string> GetRefreshTokenRequestParameters(string refreshToken) {
        var parameters = new Dictionary<string, string> {
            { "client_id", clientId },
            { "refresh_token", refreshToken },
            { "grant_type", config.refreshTokenGrantType }
        };
        if (optional != null && !String.IsNullOrEmpty(optional.clientSecret)) {
            parameters.Add("client_secret", optional.clientSecret);
        }
        return parameters;
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public virtual UniWebViewAuthenticationStandardToken GenerateTokenFromExchangeResponse(string exchangeResponse) {
        return UniWebViewAuthenticationTokenFactory<UniWebViewAuthenticationStandardToken>.Parse(exchangeResponse);
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<UniWebViewAuthenticationStandardToken> OnAuthenticationFinished { get; set; }
    
    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<long, string> OnAuthenticationErrored { get; set; }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<UniWebViewAuthenticationStandardToken> OnRefreshTokenFinished { get; set; }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<long, string> OnRefreshTokenErrored { get; set; }
}

[Serializable]
public class UniWebViewAuthenticationFlowCustomizeConfig {
    public string authorizationEndpoint = "";
    public string tokenEndpoint = "";
    public string responseType = "code";
    public string grantType = "authorization_code";

    public string refreshTokenGrantType = "refresh_token";
}

[Serializable]
public class UniWebViewAuthenticationFlowCustomizeOptional {
    public UniWebViewAuthenticationPKCE PKCESupport = UniWebViewAuthenticationPKCE.None; 
    public bool enableState = false;
    public string clientSecret = "";
}