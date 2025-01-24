//
//  UniWebViewAuthenticationStandardToken.cs
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
using UnityEngine;

/// <summary>
/// Represents the standard token used in the OAuth 2 process.
/// </summary>
[Serializable]
public class UniWebViewAuthenticationStandardToken {
    // Unity's JsonUtility.FromJson is quite stupid on this.
    // Switch to Newtonsoft.Json when we can support from Unity 2021.
    [SerializeField]
    private string access_token = default;
    /// <summary>
    /// The access token retrieved from the service provider.
    ///
    /// This usually comes from the `access_token` field in the response.
    /// Use this token to access the service provider's API.
    /// 
    /// If you do not need the token "offline", just use it and discard. UniWebView will not store this token, if you
    /// need to keep it for other purpose, please make sure you do not violate any policies and put it to a secure
    /// place yourself.
    /// </summary>
    public string AccessToken => access_token;

    [SerializeField]
    private string scope = default;
    /// <summary>
    /// The granted scopes of the token. This is usually comes from the `scope` field in the response.
    ///
    /// If there are optional scopes in the initial auth request, the user can choose to not give you some of the
    /// permissions. Check this field before you use the access token to perform certain actions to avoid failure
    /// before actual attempts.
    /// </summary>
    public string Scope => scope;

    [SerializeField]
    private string token_type = default;
    /// <summary>
    /// The token type. This usually comes from the `token_type` field in the response.
    ///
    /// For most OAuth 2.0 services, it is fixed to `Bearer`.
    /// </summary>
    public string TokenType => token_type;

    [SerializeField]
    private string refresh_token = default;
    /// <summary>
    /// The refresh token retrieved from the service provider. This usually comes from the `refresh_token` field in the
    /// response.
    ///
    /// If the access token is refreshable, you can use this
    /// refresh token to perform a refresh operation and get a new access token without the user's consent again.
    ///
    /// The refresh policy can be different from the service providers. Read the documentation of the service provider
    /// to determine the use of refresh token.
    ///
    /// If the response does not contain a refresh token, this field will be `null`.
    /// </summary>
    public string RefreshToken => refresh_token;
    
    [SerializeField]
    private long expires_in = default;
    /// <summary>
    /// How long does this token remain valid. This usually comes from the `expires_in` field in the response.
    /// </summary>
    public long ExpiresIn => expires_in;
    
    [SerializeField]
    private string id_token = default;
    /// <summary>
    /// The ID token retrieved from the service provider. This usually comes from the `id_token` field in the response.
    ///
    /// If the service provider does not support ID token or you did not apply for it, this field will be `null`.
    /// The ID token is usually a JWT token that contains information about the user.
    /// </summary>
    public string IdToken => id_token;
    
    /// <summary>
    /// The raw value of the response of the exchange token request.
    /// 
    /// If the predefined fields are not enough, you can parse the raw value to get the extra information.
    /// </summary>
    public string RawValue { get; set; }
}

/// <summary>
/// Util class to generate the standard token from a JSON based exchange token response.
/// </summary>
/// <typeparam name="TToken">The type of target token.</typeparam>
public abstract class UniWebViewAuthenticationTokenFactory<TToken> where TToken : UniWebViewAuthenticationStandardToken {
    public static TToken Parse(string result)  {
        var json = JsonUtility.FromJson<TToken>(result);
        json.RawValue = result;
        if (json.AccessToken == null) {
            throw AuthenticationResponseException.InvalidResponse(result);
        }
        return json;
    }
}