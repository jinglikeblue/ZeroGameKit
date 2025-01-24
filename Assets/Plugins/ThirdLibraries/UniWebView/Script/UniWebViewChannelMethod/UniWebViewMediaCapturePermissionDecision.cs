//
//  UniWebViewMediaCapturePermissionDecision.cs
//  Created by Wang Wei(@onevcat) on 2024-02-20.
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
/// Represents the decision of media capture permission request.
///
/// Use value in this enum to guide how UniWebView should handle the media capture permission request.
/// </summary>
public enum UniWebViewMediaCapturePermissionDecision {
    /// <summary>
    /// Display a prompt to ask user for the permission.
    ///
    /// The prompt alert shows the origin of the request and the resources requested. It asks user to grant or deny the
    /// permission.
    /// </summary>
    Prompt,
    
    /// <summary>
    /// Grant the permission request without asking user.
    /// </summary>
    Grant,
    
    /// <summary>
    /// Deny the permission request. The web page will receive an error and it knows the request resources are not
    /// allowed to use.
    /// </summary>
    Deny
}