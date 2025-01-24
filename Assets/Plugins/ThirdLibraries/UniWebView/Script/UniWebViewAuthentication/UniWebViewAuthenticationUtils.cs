//
//  UniWebViewAuthenticationUtils.cs
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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;

/// <summary>
/// This class provides some helper utils for performing the authentication flow.
/// 
/// They are used inside the built-in flows, but you can also use them to implement your own flow.
/// </summary>
public class UniWebViewAuthenticationUtils {
    internal static Dictionary<string, string> ParseFormUrlEncodedString(string input) {
        var result = new Dictionary<string, string>();
        if (input.StartsWith("?") || input.StartsWith("#")) {
            input = input.Substring(1);
        }

        var pairs = input.Split('&');
        foreach (var pair in pairs) {
            var kv = pair.Split('=');
            result.Add(UnityWebRequest.UnEscapeURL(kv[0]), UnityWebRequest.UnEscapeURL(kv[1]));
        }

        return result;
    }

    /// <summary>
    /// Generates a random Base64 encoded string.
    /// </summary>
    /// <returns>A random Base64 encoded string.</returns>
    public static string GenerateRandomBase64String() {
        var randomNumber = new byte[32];
        string value = "";
        using (var rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(randomNumber);
            value = Convert.ToBase64String(randomNumber);
        }

        return value;
    }

    /// <summary>
    /// Generates a random Base64URL encoded string.
    /// </summary>
    /// <returns>A random Base64URL encoded string.</returns>
    public static string GenerateRandomBase64URLString() {
        var value = GenerateRandomBase64String();
        return ConvertToBase64URLString(value);
    }

    static readonly char[] padding = { '=' };

    /// <summary>
    /// Converts a Base64 encoded string to a Base64URL encoded string.
    /// </summary>
    /// <param name="input">The Base64 encoded string.</param>
    /// <returns>A string with Base64URL encoded for the input.</returns>
    public static string ConvertToBase64URLString(string input) {
        return input.TrimEnd(padding).Replace('+', '-').Replace('/', '_');
    }

    /// <summary>
    /// Converts a Base64URL encoded string to a Base64 encoded string.
    /// </summary>
    /// <param name="input">The Base64URL encoded string.</param>
    /// <returns>A string with Base64 encoded for the input.</returns>
    public static string ConvertToBase64String(string input) {
        var result = input.Replace('_', '/').Replace('-', '+');
        switch (input.Length % 4) {
            case 2:
                result += "==";
                break;
            case 3:
                result += "=";
                break;
        }

        return result;
    }

    /// <summary>
    /// Generates a code verifier for PKCE usage.
    /// </summary>
    /// <param name="length">The length of the target code verifier. Default is 64.</param>
    /// <returns>A generated code verifier for PKCE usage.</returns>
    public static string GenerateCodeVerifier(int length = 64) {
        var randomNumber = new byte[32];
        string value;

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        var random = new Random(System.BitConverter.ToInt32(randomNumber, 0));
        value = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());

        return value;
    }

    /// <summary>
    /// Calculates the code challenge for PKCE usage, with a given code verifier and hash method.
    /// </summary>
    /// <param name="codeVerifier">The code verifier you generated.</param>
    /// <param name="method">The hash method you want to use.</param>
    /// <returns>The result of the code challenge.</returns>
    public static string CalculateCodeChallenge(string codeVerifier, UniWebViewAuthenticationPKCE method) {
        switch (method) {
            case UniWebViewAuthenticationPKCE.None:
                throw new ArgumentOutOfRangeException(nameof(method), method, null);
            case UniWebViewAuthenticationPKCE.S256:
                var sha256 = SHA256.Create();
                var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(codeVerifier));
                return ConvertToBase64URLString(System.Convert.ToBase64String(hash));
            case UniWebViewAuthenticationPKCE.Plain:
                return codeVerifier;
            default:
                throw new ArgumentOutOfRangeException(nameof(method), method, null);
        }
    }

    public static string ConvertPKCEToString(UniWebViewAuthenticationPKCE method) {
        switch (method) {
            case UniWebViewAuthenticationPKCE.None:
                return null;
            case UniWebViewAuthenticationPKCE.S256:
                return "S256";
            case UniWebViewAuthenticationPKCE.Plain:
                return "plain";
        }

        return null;
    }

    public static string ConvertIntentUri(string input) {
        var uri = new Uri(input);
        if (uri.Scheme != "intent") {
            return input;
        }

        var host = uri.Host;
        string scheme = null;
        var fragments = uri.Fragment;
        fragments.Split(';').ToList().ForEach(fragment => {
            var kv = fragment.Split('=');
            if (kv.Length == 2 && kv[0] == "scheme") {
                scheme = kv[1];
            }
        });
        
        if (!String.IsNullOrEmpty(scheme)) {
            return scheme + "://" + host;
        }

        return input;
    }

    public static string CreateQueryString(Dictionary<string, string> collection) {
        int count = collection.Count;
        if (count == 0) {
            return "";            
        }
        StringBuilder sb = new StringBuilder();
        string [] keys = collection.Keys.ToArray();
        for (int i = 0; i < count; i++) {
            sb.AppendFormat ("{0}={1}&", keys[i], UnityWebRequest.EscapeURL(collection[keys[i]]));
        }

        if (sb.Length > 0) {
            sb.Length--;            
        }
        return sb.ToString();
    }
}

public enum UniWebViewAuthenticationPKCE
{
    None,
    S256,
    Plain
}