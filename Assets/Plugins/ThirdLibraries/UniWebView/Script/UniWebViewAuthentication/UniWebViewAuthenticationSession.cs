//
//  UniWebViewAuthenticationSession.cs
//  Created by Wang Wei(@onevcat) on 2022-06-21.
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
using System;

/// <summary>
/// Represents a session that can be used to authenticate a user through a web service.
/// </summary>
/// <remarks>
/// Initialize the session with a URL that points to the authentication webpage. A browser or a secure web view loads
/// and displays the page. On completion, the service sends a callback URL to the session with an authentication token,
/// and this triggers the `OnAuthenticationFinished` with the received URL. To make your app be invoked by the system,
/// you need to also add the correct callback URL starting with the value of `CallbackScheme` to UniWebView's preferences.
///
/// Usually this session processes an OAuth 2 flow. It will be used along with a following "exchange token" request, to
/// finally get the user's access token to allow you use the service APIs on behalf of the user. This token exchange can
/// happen in the client app, or you can pass the code to your server and let your server do the left work.
///
/// UniWebView also provides built-in integrated authentication flows for several popular service. The the
/// `UniWebViewAuthenticationFlow` cluster classes to use them and simplify your work. If the built-in models do not
/// fit your work, you can use this class as a starting point of your own authentication integration. 
/// 
/// See https://docs.uniwebview.com/guide/oauth2.html for a more detailed guide of authentication in UniWebView.
/// </remarks>
public class UniWebViewAuthenticationSession: UnityEngine.Object {
  /// <summary>
  /// Delegate for authentication session finished event.
  /// </summary>
  /// <param name="session">The session which raised this event.</param>
  /// <param name="url">
  /// The received URL from service. It might contain a valid `code` from the service, or an error.
  /// </param>
  public delegate void AuthenticationFinishedDelegate(UniWebViewAuthenticationSession session, string url);
  
  /// <summary>
  /// Raised when the session finishes authentication.
  ///
  /// This event will be invoked when the service provider calls the callback URL. regardless of the authentication code
  /// is retrieved or an error is returned in the callback URL.
  /// </summary>
  public event AuthenticationFinishedDelegate OnAuthenticationFinished;

  /// <summary>
  /// Delegate for authentication session error encounter event.
  /// </summary>
  /// <param name="session">The session which raised this event.</param>
  /// <param name="errorCode">The error code represents the error type.</param>
  /// <param name="errorMessage">The error message describes the error in detail.</param>
  public delegate void AuthErrorReceivedDelegate(UniWebViewAuthenticationSession session, int errorCode, string errorMessage);
  
  /// <summary>
  /// Raised when the session encounters an error.
  ///
  /// This event will be invoked when the authentication session cannot finishes with a URL callback. This usually
  /// happens when a network error or the user dismisses the authentication page from native UI.
  /// </summary>
  public event AuthErrorReceivedDelegate OnAuthenticationErrorReceived;

  private readonly string id = Guid.NewGuid().ToString();
  private UniWebViewNativeListener listener;
  
  /// <summary>
  /// The URL of the authentication webpage. This is the value you used to create this session.
  /// </summary>
  public string Url { get; private set; }
  
  /// <summary>
  /// The callback scheme of the authentication webpage. This is the value you used to create this session. The service
  /// is expected to use a URL with this scheme to return to your app. 
  /// </summary>
  public string CallbackScheme { get; private set; }

  internal void InternalAuthenticationFinished(string url) {
    if (OnAuthenticationFinished != null) {
      OnAuthenticationFinished(this, url);
    }
    UniWebViewNativeListener.RemoveListener(listener.Name);
    Destroy(listener.gameObject);
  }

  internal void InternalAuthenticationErrorReceived(UniWebViewNativeResultPayload payload) {
    if (OnAuthenticationErrorReceived != null) {
      int errorCode = int.TryParse(payload.resultCode, out errorCode) ? errorCode : -1;
      OnAuthenticationErrorReceived(this, errorCode, payload.data);
    }
    
    UniWebViewNativeListener.RemoveListener(listener.Name);
    Destroy(listener.gameObject);
  }

  private UniWebViewAuthenticationSession() {  
    var listenerObject = new GameObject("UniWebViewAuthSession-" + id);
    listener = listenerObject.AddComponent<UniWebViewNativeListener>();
    UniWebViewNativeListener.AddListener(listener);
  }

  /// <summary>
  /// Check whether the current device and system supports the authentication session.
  /// </summary>
  /// <remarks>
  /// This property always returns `true` on iOS 11, macOS 10.15 and later. On Android, it depends on whether there
  /// is an Intent can handle the safe browsing request,  which is use to display the authentication page. Usually
  /// it is provided by Chrome. If there is no Intent can open the URL in safe browsing mode, this property will
  /// return `false`.
  /// 
  /// To use this API on Android when you set your Target SDK to Android 11 or later, you need to declare the correct 
  /// intent query explicitly in your AndroidManifest.xml, to follow the Package Visibility 
  /// (https://developer.android.com/about/versions/11/privacy/package-visibility):
  /// 
  /// ```xml
  /// <queries>
  ///   <intent>
  ///     <action android:name="android.support.customtabs.action.CustomTabsService" />
  ///   </intent>
  /// </queries>
  /// ```
  /// </remarks>
  /// <returns>
  /// Returns `true` if the safe browsing mode is supported and the page will be opened in safe browsing 
  /// mode. Otherwise, `false`.
  /// </returns>
  public static bool IsAuthenticationSupported => UniWebViewInterface.IsAuthenticationIsSupported();

  /// <summary>
  /// Creates a new authentication session with a given authentication page URL and a callback scheme.
  /// </summary>
  /// <param name="url">
  /// The authentication page which is provided by the service. It should be a URL with some information like your app's
  /// client id and required scopes, etc.
  /// </param>
  /// <param name="callbackScheme">The URL scheme which the service will use to navigate back to your client app.</param>
  /// <returns></returns>
  public static UniWebViewAuthenticationSession Create(string url, string callbackScheme) {
    var session = new UniWebViewAuthenticationSession();
    session.listener.session = session;
    session.Url = url;
    session.CallbackScheme = callbackScheme;

    UniWebViewInterface.AuthenticationInit(session.listener.Name, url, callbackScheme);

    return session;
  }

  /// <summary>
  /// Start the authentication session process. It will show up a secured web page and navigate users to the `Url`.
  /// </summary>
  public void Start() {
    UniWebViewInterface.AuthenticationStart(listener.Name);
  }

  /// <summary>
  /// Sets to use the private mode for the authentication. If running under private mode, the previous stored
  /// authentication information will not be used.
  /// <remarks>
  /// On Apple's platform, this works from iOS 13 and macOS 10.15. On Android, this depends on the Chrome setting on the
  /// device. The users should enable the "incognito" and "third-party incognito" to allow to use this feature.
  ///
  /// Check them in Chrome app:
  ///   chrome://flags/#cct-incognito
  ///   chrome://flags/#cct-incognito-available-to-third-party
  /// </remarks>
  /// </summary>
  /// <param name="flag">Whether the session should run in private mode or not.</param>
  public void SetPrivateMode(bool flag) {
    UniWebViewInterface.AuthenticationSetPrivateMode(listener.Name, flag);
  }
}