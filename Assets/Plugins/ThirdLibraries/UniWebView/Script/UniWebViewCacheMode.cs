//
//  UniWebViewCacheMode.cs
//  Created by Wang Wei(@onevcat) on 2024-01-17.
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

/// <summary>
/// Defines the cache mode for UniWebView.
/// </summary>
public enum UniWebViewCacheMode
{
    /// <summary>
    /// Default mode. The web view will check the validity of the cache copy when there is one. If the copy is invalid,
    /// the web view will load from the network. This is the default setting.
    /// </summary>
    Default = 0,

    /// <summary>
    /// No cache is used. All pages are loaded directly from the network. This is useful for applications that do not
    /// want to have a cache.
    /// </summary>
    NoCache = 1,

    /// <summary>
    /// Prioritize the cache. If there is a copy of the page in the cache, the web view will use it even if the copy
    /// has expired. The web view will only load from the network when the page does not exist in the cache.
    /// </summary>
    CacheElseLoad = 2,

    /// <summary>
    /// Only use the cache. In this mode, the web view will not load pages from the network, only use the content in
    /// the cache. If the requested URL is not in the cache, an error is returned.
    /// </summary>
    CacheOnly = 3,
}