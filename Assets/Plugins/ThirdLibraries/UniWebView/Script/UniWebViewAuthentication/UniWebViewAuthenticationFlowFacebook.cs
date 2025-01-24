//
//  UniWebViewAuthenticationFlowFacebook.cs
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
/// A predefined authentication flow for Facebook Login.
///
/// It is not a standard OAuth2 flow, and using a plain web view. There once was a policy that Facebook did not allow
/// any third-party customize authentication flow other than using their official SDK. Recently Facebook started to provide
/// a so-called manual flow way to perform authentication. But it is originally only for Desktop apps, it is not stable
/// and not standard.
///
/// Facebook suggests "For mobile apps, use the Facebook SDKs for iOS and Android, and follow the separate guides for
/// these platforms." So on mobile, use this class with your own risk since it might be invalidated or forbidden by
/// Facebook in the future.
/// 
/// This implementation is based on the manual flow described in the following document:
/// https://developers.facebook.com/docs/facebook-login/guides/advanced/manual-flow 
///
/// See https://docs.uniwebview.com/guide/oauth2.html for a more detailed guide of authentication in UniWebView.
/// </summary>
public class UniWebViewAuthenticationFlowFacebook: UniWebViewAuthenticationCommonFlow {
    /// <summary>
    /// The App ID of your Facebook application
    /// </summary>
    public string appId = "";
    
    /// <summary>
    /// Optional to control this flow's behaviour.
    /// </summary>
    public UniWebViewAuthenticationFlowFacebookOptional optional;
    
    // The redirect URL should be exactly this one. Web view should inspect the loading of this URL to handle the result.
    private const string redirectUri = "https://www.facebook.com/connect/login_success.html";

    // Only `token` response type is supported to use Facebook Login as the manual flow.
    private const string responseType = "token";
    
    [field: SerializeField]
    public UnityEvent<UniWebViewAuthenticationFacebookToken> OnAuthenticationFinished { get; set; }
    [field: SerializeField]
    public UnityEvent<long, string> OnAuthenticationErrored { get; set; }
    
    private readonly UniWebViewAuthenticationConfiguration config = 
        new UniWebViewAuthenticationConfiguration(
            "https://www.facebook.com/v14.0/dialog/oauth", 
            // This `access_token` entry point is in fact not used in current auth model.
            "https://graph.facebook.com/v14.0/oauth/access_token" 
        );

    /// <summary>
    /// Starts the authentication flow.
    ///
    /// This flow is executed in a customized web view and it is not a standard OAuth2 flow. 
    /// </summary>
    public override void StartAuthenticationFlow() {
        var webView = gameObject.AddComponent<UniWebView>();
        
        // Facebook login deprecates the Web View login on Android. As a workaround, prevents to be a desktop browser to continue the manual flow.
        #if UNITY_ANDROID && !UNITY_EDITOR
        webView.SetUserAgent("Mozilla/5.0 (Macintosh; Intel Mac OS X 13_0_1) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.1 Safari/605.1.15");
        #endif
        
        webView.OnPageFinished += (view, status, url) => {
            if (status != 200) {
                if (OnAuthenticationErrored != null) {
                    OnAuthenticationErrored.Invoke(status, "Error while loading auth page.");
                }
                webView.Hide(false, UniWebViewTransitionEdge.Bottom, 0.25f, () => {
                    Destroy(webView);
                });
                return;
            }
            
            if (url.StartsWith(redirectUri)) {
                UniWebViewLogger.Instance.Info("Received redirect url: " + url);
                var uri = new Uri(url);
                var response = UniWebViewAuthenticationUtils.ParseFormUrlEncodedString(uri.Fragment);
                try {
                    VerifyState(response);
                    var token = new UniWebViewAuthenticationFacebookToken(url, response);
                    if (OnAuthenticationFinished != null) {
                        OnAuthenticationFinished.Invoke(token);
                    }
                }
                catch (Exception e) {
                    var message = e.Message;
                    var code = -1;
                    if (e is AuthenticationResponseException ex) {
                        code = ex.Code;
                    }

                    UniWebViewLogger.Instance.Critical("Exception on parsing response: " + e + ". Code: " + code +
                                                       ". Message: " + message);
                    if (OnAuthenticationErrored != null) {
                        OnAuthenticationErrored.Invoke(code, message);
                    }
                }
                finally {
                    webView.Hide(false, UniWebViewTransitionEdge.Bottom, 0.25f, () => {
                        Destroy(webView);
                    });
                }
            }
        };
        webView.OnLoadingErrorReceived += (view, code, message, payload) => {
            if (OnAuthenticationErrored != null) {
                OnAuthenticationErrored.Invoke(code, message);
            }
        };
        webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        webView.Load(GetAuthUrl());
        webView.EmbeddedToolbar.Show();
        webView.Show(false, UniWebViewTransitionEdge.Bottom, 0.25f);
    }

    /// <summary>
    /// Starts the refresh flow with the standard OAuth 2.0.
    /// This implements the abstract method in `UniWebViewAuthenticationCommonFlow`.
    /// </summary>
    /// <param name="refreshToken">The refresh token received with a previous access token response.</param>
    public override void StartRefreshTokenFlow(string refreshToken) {
        Debug.LogError("Facebook does not provide a refresh token flow when building with the manual flow.");
        throw new NotImplementedException();
    }

    private string GetAuthUrl() {
        var builder = new UriBuilder(config.authorizationEndpoint);
        var query = new Dictionary<string, string>();
        foreach (var kv in GetAuthenticationUriArguments()) {
            query.Add(kv.Key, kv.Value);
        }

        builder.Query = UniWebViewAuthenticationUtils.CreateQueryString(query);
        return builder.ToString();
    }

    private Dictionary<string, string> GetAuthenticationUriArguments() {

        var state = GenerateAndStoreState();
        var authorizeArgs = new Dictionary<string, string> {
            { "client_id", appId },
            { "redirect_uri", redirectUri },
            { "state", state},
            { "response_type", responseType }
        };
        if (optional != null) {
            if (!String.IsNullOrEmpty(optional.scope)) {
                authorizeArgs.Add("scope", optional.scope);
            }
        }

        return authorizeArgs;
    }
}

/// <summary>
/// The authentication flow's optional settings for Facebook.
/// </summary>
[Serializable]
public class UniWebViewAuthenticationFlowFacebookOptional {
    /// <summary>
    /// The scope string of all your required scopes.
    /// </summary>
    public string scope = "";
}

/// The token object from Facebook. 
public class UniWebViewAuthenticationFacebookToken {
    /// <summary>
    /// The access token received from Facebook Login.
    /// </summary>
    public string AccessToken { get; }
    /// <summary>
    /// The expiration duration that your app can access requested items of user data.
    /// </summary>
    public long DataAccessExpirationTime { get; }
    /// <summary>
    /// The expiration duration that the access token is valid for authentication purpose.
    /// </summary>
    public long ExpiresIn { get; }
    /// <summary>
    /// The raw value of the response of the exchange token request.
    /// If the predefined fields are not enough, you can parse the raw value to get the extra information.
    /// </summary>
    public string RawValue { get; }
    
    public UniWebViewAuthenticationFacebookToken(string response, Dictionary<string, string> values) {
        RawValue = response;
        AccessToken = values.ContainsKey("access_token") ? values["access_token"] : null ;
        if (AccessToken == null) {
            throw AuthenticationResponseException.InvalidResponse(response);
        }
        DataAccessExpirationTime = values.ContainsKey("data_access_expiration_time") ? long.Parse(values["data_access_expiration_time"]) : 0;
        ExpiresIn = values.ContainsKey("expires_in") ? long.Parse(values["expires_in"]) : 0;
    }
}

