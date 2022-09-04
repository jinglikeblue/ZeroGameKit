//
//  UniWebViewContentInsetAdjustmentBehavior.cs
//  Created by Wang Wei(@onevcat) on 2019-09-20.
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
/// Constants indicating how safe area insets are added to the adjusted content inset.
/// This is only for iOS.
/// </summary>
public enum UniWebViewContentInsetAdjustmentBehavior
{
    /// <summary>
    /// Automatically adjust the scroll view insets.
    /// </summary>
    Automatic = 0,

    /// <summary>
    /// Adjust the insets only in the scrollable directions.
    /// </summary>
    ScrollableAxes = 1,

    /// <summary>
    /// Do not adjust the scroll view insets.
    /// </summary>
    Never = 2,

    /// <summary>
    /// Always include the safe area insets in the content adjustment.
    /// </summary>
    Always = 3
}