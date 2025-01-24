//
//  UniWebViewDownloadMatchingType.cs
//  Created by Wang Wei(@onevcat) on 2021-09-02.
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

/// <summary>
/// The matching type used when UniWebView determines whether to download from a URL or MIME type.
/// </summary>
public enum UniWebViewDownloadMatchingType
{
    /// <summary>
    /// Matches exact the whole value.
    /// </summary>
    ExactValue = 1,
    /// <summary>
    /// Uses the value as a regular expression.
    /// </summary>
    RegularExpression = 2
}

