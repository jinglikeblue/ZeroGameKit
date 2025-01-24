//
//  UniWebViewNativeListener.cs
//  Created by Wang Wei(@onevcat) on 2017-04-11.
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
using System.Collections.Generic;
using System;
using UniWebViewExternal;

/// <summary>
/// A listener script for message sent from native side of UniWebView.
/// Normally this component will be attached to a sub `GameObject` under the `UniWebView` one.
/// It will be added automatically and destroyed as needed. So there is rarely a need for you 
/// to manipulate on this class.
/// </summary>
public class UniWebViewNativeListener: MonoBehaviour {
    
    private static Dictionary<String, UniWebViewNativeListener> listeners = new Dictionary<String, UniWebViewNativeListener>();
    public static void AddListener(UniWebViewNativeListener target) {
        listeners.Add(target.Name, target);
    }

    public static void RemoveListener(String name) {
        listeners.Remove(name);
    }

    public static UniWebViewNativeListener GetListener(String name) {
        UniWebViewNativeListener result = null;
        if (listeners.TryGetValue(name, out result)) {
            return result;
        } else {
            return null;
        }
    }

    /// <summary>
    /// The web view holder of this listener.
    /// It will be linked to original web view in web view context, so you should never set it yourself.
    /// Either `webView` or `safeBrowsing` will be valid in this listener.
    /// </summary>
    [HideInInspector]
    public UniWebView webView;

    // The safe browsing of this listener.
    /// It will be linked to original safe browsing in browsing context, so you should never set it yourself.
    /// Either `webView` or `safeBrowsing` will be valid in this listener.
    [HideInInspector]
    public UniWebViewSafeBrowsing safeBrowsing;

    [HideInInspector]
    public UniWebViewAuthenticationSession session;

    /// <summary>
    /// Name of current listener. This is a UUID string by which native side could use to find 
    /// the message destination.
    /// </summary>
    public string Name => gameObject.name;

    public void PageStarted(string url) {
        UniWebViewLogger.Instance.Info("Page Started Event. Url: " + url);
        webView.InternalOnPageStarted(url);
    }

    public void PageFinished(string result) {
        UniWebViewLogger.Instance.Info("Page Finished Event. Url: " + result);
        var payload = JsonUtility.FromJson<UniWebViewNativeResultPayload>(result);
        webView.InternalOnPageFinished(payload);
    }

    public void PageErrorReceived(string result) {
        UniWebViewLogger.Instance.Info("Page Error Received Event. Result: " + result);
        var payload = JsonUtility.FromJson<UniWebViewNativeResultPayload>(result);
        webView.InternalOnPageErrorReceived(payload);
    }

    public void PageProgressChanged(string result) {
        float progress;
        if (float.TryParse(result, out progress)) {
            webView.InternalOnPageProgressChanged(progress);
        }
    }

    public void ShowTransitionFinished(string identifer) {
        UniWebViewLogger.Instance.Info("Show Transition Finished Event. Identifier: " + identifer);
        webView.InternalOnShowTransitionFinished(identifer);
    }

    public void HideTransitionFinished(string identifer) {
        UniWebViewLogger.Instance.Info("Hide Transition Finished Event. Identifier: " + identifer);
        webView.InternalOnHideTransitionFinished(identifer);
    }

    public void AnimateToFinished(string identifer) {
        UniWebViewLogger.Instance.Info("Animate To Finished Event. Identifier: " + identifer);
        webView.InternalOnAnimateToFinished(identifer);
    }

    public void AddJavaScriptFinished(string result) {
        UniWebViewLogger.Instance.Info("Add JavaScript Finished Event. Result: " + result);
        var payload = JsonUtility.FromJson<UniWebViewNativeResultPayload>(result);
        webView.InternalOnAddJavaScriptFinished(payload);
    }

    public void EvalJavaScriptFinished(string result) {
        UniWebViewLogger.Instance.Info("Eval JavaScript Finished Event. Result: " + result);
        
        var payload = JsonUtility.FromJson<UniWebViewNativeResultPayload>(result);
        webView.InternalOnEvalJavaScriptFinished(payload);
    }

    public void MessageReceived(string result) {
        UniWebViewLogger.Instance.Info("Message Received Event. Result: " + result);
        webView.InternalOnMessageReceived(result);
    }

    public void WebViewDone(string param) {
        UniWebViewLogger.Instance.Info("Web View Done Event.");
        webView.InternalOnShouldClose();
    }

    public void WebContentProcessDidTerminate(string param) {
        UniWebViewLogger.Instance.Info("Web Content Process Terminate Event.");
        webView.InternalOnWebContentProcessDidTerminate();
    }

    public void SafeBrowsingFinished(string param) {
        UniWebViewLogger.Instance.Info("Safe Browsing Finished.");
        safeBrowsing.InternalSafeBrowsingFinished();
    }

    public void MultipleWindowOpened(string param) {
        UniWebViewLogger.Instance.Info("MultipleWindowOpened Event. Multi Window: " + param);
        webView.InternalOnMultipleWindowOpened(param);
    }

    public void MultipleWindowClosed(string param) {
        UniWebViewLogger.Instance.Info("MultipleWindowClose Event. Multi Window: " + param);
        webView.InternalOnMultipleWindowClosed(param);
    }

    public void FileDownloadStarted(string result) {
        UniWebViewLogger.Instance.Info("FileDownloadStarted Event. Result: " + result);

        var payload = JsonUtility.FromJson<UniWebViewNativeResultPayload>(result);
        webView.InternalOnFileDownloadStarted(payload);
    }

    public void FileDownloadFinished(string result) {
        UniWebViewLogger.Instance.Info("FileDownloadFinished Event. Result: " + result);

        var payload = JsonUtility.FromJson<UniWebViewNativeResultPayload>(result);
        webView.InternalOnFileDownloadFinished(payload);
    }

    public void CaptureSnapshotFinished(string result) {
        UniWebViewLogger.Instance.Info("CaptureSnapshotFinished Event. Result: " + result);
        
        var payload = JsonUtility.FromJson<UniWebViewNativeResultPayload>(result);
        webView.InternalOnCaptureSnapshotFinished(payload);
    }

    public void AuthFinished(string result) {
        UniWebViewLogger.Instance.Info("Auth Session Finished. Url: " + result);
        session.InternalAuthenticationFinished(result);
    }

    public void AuthErrorReceived(string result) {
        UniWebViewLogger.Instance.Info("Auth Session Error Received. Result: " + result);
        var payload = JsonUtility.FromJson<UniWebViewNativeResultPayload>(result);
        session.InternalAuthenticationErrorReceived(payload);
    }

    public void SnapshotRenderingStarted(string identifier) {
        UniWebViewLogger.Instance.Info("Snapshot Rendering Started Event. Identifier: " + identifier);
        webView.InternalOnSnapshotRenderingStarted(identifier);
    }
}

/// <summary>
/// A payload received from native side. It contains information to identify the message sender,
/// as well as some necessary field to bring data from native side to Unity.
/// </summary>
[System.Serializable]
public class UniWebViewNativeResultPayload {

    /// <summary>
    /// The key in `Extra` dictionary which contains the failing URL, if available.
    /// </summary>
    public const string ExtraFailingURLKey = "failingURL";
    
    /// <summary>
    /// The identifier bound to this payload. It would be used internally to identify the callback.
    /// </summary>
    public string identifier;
    /// <summary>
    /// The result code contained in this payload. Generally, "0" means the operation finished without
    /// problem, while a non-zero value means somethings goes wrong.
    /// </summary>
    public string resultCode;
    /// <summary>
    /// Return value or data from native. You should look at 
    /// corresponding APIs to know what exactly contained in this.
    ///
    /// Usually it is a string value represents the reason or a small piece of data related to the result. 
    /// </summary>
    public string data;

    /// <summary>
    /// The extra data from native side. It is a JSON string and could be parsed to a dictionary.
    ///
    /// Usually you do not access to this value directly. Instead, use `Extra` property to get the parsed dictionary.
    /// </summary>
    public string extra;
    
    /// <summary>
    /// The extra data from native side, in dictionary format. If there is no extra data provided, this will be null.
    /// Otherwise, it contains values passed from native side.
    /// </summary>
    public Dictionary<string, object> Extra {
        get {
            if (String.IsNullOrEmpty(extra)) {
                return null;
            }
            return Json.Deserialize(extra) as Dictionary<string, object>;
        }
    }
}