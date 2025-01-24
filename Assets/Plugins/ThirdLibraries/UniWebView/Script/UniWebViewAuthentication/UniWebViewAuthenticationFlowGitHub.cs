//
//  UniWebViewAuthenticationFlowGitHub.cs
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

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

/// <summary>
/// A predefined authentication flow for GitHub.
///
/// This implementation follows the flow described here:
/// https://docs.github.com/en/developers/apps/building-oauth-apps/authorizing-oauth-apps
///
/// See https://docs.uniwebview.com/guide/oauth2.html for a more detailed guide of authentication in UniWebView.
/// </summary>
public class UniWebViewAuthenticationFlowGitHub: UniWebViewAuthenticationCommonFlow, IUniWebViewAuthenticationFlow<UniWebViewAuthenticationGitHubToken> {
    
    /// <summary>
    /// The client ID of your GitHub application.
    /// </summary>
    public string clientId = "";
    
    /// <summary>
    /// The client secret of your GitHub application.
    /// </summary>
    public string clientSecret = "";
    
    /// <summary>
    /// The callback URL of your GitHub application.
    /// </summary>
    public string callbackUrl = "";
    
    /// <summary>
    /// Optional to control this flow's behaviour.
    /// </summary>
    public UniWebViewAuthenticationFlowGitHubOptional optional;
    
    private readonly UniWebViewAuthenticationConfiguration config = 
        new UniWebViewAuthenticationConfiguration(
            "https://github.com/login/oauth/authorize", 
            "https://github.com/login/oauth/access_token"
        );

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<UniWebViewAuthenticationGitHubToken> OnAuthenticationFinished { get; set; }
    
    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<long, string> OnAuthenticationErrored { get; set; }
    
    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<UniWebViewAuthenticationGitHubToken> OnRefreshTokenFinished { get; set;  }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<long, string> OnRefreshTokenErrored { get; set; }

    /// <summary>
    /// Starts the authentication flow with the standard OAuth 2.0.
    /// This implements the abstract method in `UniWebViewAuthenticationCommonFlow`.
    /// </summary>
    public override void StartAuthenticationFlow() {
        var flow = new UniWebViewAuthenticationFlow<UniWebViewAuthenticationGitHubToken>(this);
        flow.StartAuth();
    }

    /// <summary>
    /// Starts the refresh flow with the standard OAuth 2.0.
    /// This implements the abstract method in `UniWebViewAuthenticationCommonFlow`.
    /// </summary>
    /// <param name="refreshToken">The refresh token received with a previous access token response.</param>
    public override void StartRefreshTokenFlow(string refreshToken) {
        var flow = new UniWebViewAuthenticationFlow<UniWebViewAuthenticationGitHubToken>(this);
        flow.RefreshToken(refreshToken);
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public Dictionary<string, string> GetAuthenticationUriArguments() {
        var authorizeArgs = new Dictionary<string, string> { { "client_id", clientId } };
        if (optional != null) {
            if (!String.IsNullOrEmpty(optional.redirectUri)) {
                authorizeArgs.Add("redirect_uri", optional.redirectUri);
            }
            if (!String.IsNullOrEmpty(optional.login)) {
                authorizeArgs.Add("login", optional.login);
            }
            if (!String.IsNullOrEmpty(optional.scope)) {
                authorizeArgs.Add("scope", optional.scope);
            }
            if (optional.enableState) {
                var state = GenerateAndStoreState();
                authorizeArgs.Add("state", state);
            }
            if (!optional.allowSignup) { // The default value is true.
                authorizeArgs.Add("allow_signup", "false");
            }
        }

        return authorizeArgs;
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public string GetCallbackUrl() {
        return callbackUrl;
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
    public Dictionary<string, string> GetAccessTokenRequestParameters(string authResponse) {
        if (!authResponse.StartsWith(callbackUrl, StringComparison.InvariantCultureIgnoreCase)) {
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
        var result = new Dictionary<string, string> {
            { "client_id", clientId }, 
            { "client_secret", clientSecret }, 
            { "code", code }
        };
        
        if (optional != null && String.IsNullOrEmpty(optional.redirectUri)) {
            result.Add("redirect_uri", optional.redirectUri);
        }
        return result;
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public Dictionary<string, string> GetRefreshTokenRequestParameters(string refreshToken) {
        return new Dictionary<string, string> {
            { "client_id", clientId }, 
            { "client_secret", clientSecret }, 
            { "refresh_token", refreshToken },
            { "grant_type", "refresh_token" }
        };
    }

    /// <summary>
    /// Implements required method in `IUniWebViewAuthenticationFlow`.
    /// </summary>
    public UniWebViewAuthenticationGitHubToken GenerateTokenFromExchangeResponse(string exchangeResponse) {
        return new UniWebViewAuthenticationGitHubToken(exchangeResponse);
    }
}

/// <summary>
/// The authentication flow's optional settings for GitHub.
/// </summary>
[Serializable]
public class UniWebViewAuthenticationFlowGitHubOptional {
    /// <summary>
    /// The redirect URI should be used in exchange token request.
    /// </summary>
    public string redirectUri = "";
    /// <summary>
    /// Suggests a specific account to use for signing in and authorizing the app.
    /// </summary>
    public string login = "";
    /// <summary>
    /// The scope string of all your required scopes.
    /// </summary>
    public string scope = "";
    /// <summary>
    /// Whether to enable the state verification. If enabled, the state will be generated and verified in the
    /// authentication callback.
    /// </summary>
    public bool enableState = false;
    /// <summary>
    /// Whether unauthenticated users will be offered an option to sign up for GitHub during the OAuth flow.
    /// </summary>
    public bool allowSignup = true;
}

/// <summary>
/// The token object from GitHub.
/// </summary>
public class UniWebViewAuthenticationGitHubToken {
    /// <summary>The access token retrieved from the service provider.</summary>
    public string AccessToken { get; }
    
    /// <summary>The granted scopes of the token.</summary>
    public string Scope { get; }
    
    /// <summary>The token type. Usually `bearer`.</summary>
    public string TokenType { get; }
    
    /// <summary>The refresh token retrieved from the service provider.</summary>
    public string RefreshToken { get; }
    
    /// <summary>Expiration duration for the refresh token.</summary>
    public long RefreshTokenExpiresIn { get; }
    
    /// <summary>
    /// The raw value of the response of the exchange token request.
    /// If the predefined fields are not enough, you can parse the raw value to get the extra information.
    /// </summary>
    public string RawValue { get; }
    
    public UniWebViewAuthenticationGitHubToken(string result) {
        RawValue = result;
        var values = UniWebViewAuthenticationUtils.ParseFormUrlEncodedString(result);
        AccessToken = values.ContainsKey("access_token") ? values["access_token"] : null ;
        if (AccessToken == null) {
            throw AuthenticationResponseException.InvalidResponse(result);
        }
        Scope = values.ContainsKey("scope") ? values["scope"] : null;
        TokenType = values.ContainsKey("token_type") ? values["token_type"] : null;
        RefreshToken = values.ContainsKey("refresh_token") ? values["refresh_token"] : null;
        RefreshTokenExpiresIn = values.ContainsKey("refresh_token_expires_in") ? long.Parse(values["refresh_token_expires_in"]) : 0;
    }
}