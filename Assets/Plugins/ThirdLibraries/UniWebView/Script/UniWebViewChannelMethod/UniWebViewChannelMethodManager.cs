//
//  UniWebViewChannelMethodManager.cs
//  Created by Wang Wei(@onevcat) on 2023-04-21.
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
using UnityEngine;

enum UniWebViewChannelMethod {
    ShouldUniWebViewHandleRequest,
    RequestMediaCapturePermission
}

class UniWebViewChannelMethodManager {
    
    private static UniWebViewChannelMethodManager instance;
    private Dictionary<string, Dictionary<string, Func<object, object>>> channels = 
        new Dictionary<string, Dictionary<string, Func<object, object>>>();

    internal static UniWebViewChannelMethodManager Instance {
        get {
            if (instance == null) {
                instance = new UniWebViewChannelMethodManager();
            }
            return instance;
        }
    }
    
    internal void RegisterChannelMethod(
        string webViewName, 
        UniWebViewChannelMethod method, 
        Func<object, object> handler)
    {
        if (!HasRegisteredChannel(webViewName)) {
            channels[webViewName] = new Dictionary<string, Func<object, object>>();
        }
        var methodName = method.ToString();
        channels[webViewName][methodName] = handler;
        UniWebViewLogger.Instance.Info("Channel method is registered for web view: " + webViewName + ", method: " +
                                       methodName);
    }
    
    internal void UnregisterChannel(string webViewName) {
        if (!HasRegisteredChannel(webViewName)) {
            return;
        }
        channels.Remove(webViewName);
        UniWebViewLogger.Instance.Debug("All channel methods are unregistered for web view: " + webViewName);
    }
    
    internal void UnregisterChannelMethod(string webViewName, UniWebViewChannelMethod method) {
        var methodName = method.ToString();
        if (!HasRegisteredChannel(webViewName)) {
            return;
        }
        channels[webViewName].Remove(methodName);
        UniWebViewLogger.Instance.Debug("Channel method is unregistered for web view: " + webViewName + ", method: " +
                                       methodName);
    }

    bool HasRegisteredChannel(string webViewName) {
        return channels.Keys.Contains(webViewName);
    }

    bool HasRegisteredMethod(string webViewName, string methodName) {
        if (!HasRegisteredChannel(webViewName)) {
            return false;
        }

        return channels[webViewName].Keys.Contains(methodName);
    }

    internal string InvokeMethod(string webViewName, string methodName, string parameters) {
        if (!HasRegisteredMethod(webViewName, methodName)) {
            UniWebViewLogger.Instance.Info("There is no handler for the channel method. Ignoring.");
            return null;
        }
        var func = channels[webViewName][methodName];

        if (!Enum.TryParse<UniWebViewChannelMethod>(methodName, out var method)) {
            UniWebViewLogger.Instance.Info("Unknown method name: " + methodName + ". Please check, ignoring.");
            return null;
        }
        
        UniWebViewLogger.Instance.Verbose("Channel method invoking received for web view: " + webViewName + ", method: " +
                                          methodName + ", parameters: " + parameters);
        string result;
        switch (method) {
            case UniWebViewChannelMethod.ShouldUniWebViewHandleRequest: {
                // (UniWebViewChannelMethodHandleRequest) -> bool
                var input = JsonUtility.FromJson<UniWebViewChannelMethodHandleRequest>(parameters);
                bool Func(UniWebViewChannelMethodHandleRequest i) => (bool)func(i);
                result = ResultJsonWith(Func(input));
                break;
            }
            case UniWebViewChannelMethod.RequestMediaCapturePermission: {
                // (UniWebViewChannelMethodMediaCapturePermission) -> UniWebViewMediaCapturePermissionDecision
                var input = JsonUtility.FromJson<UniWebViewChannelMethodMediaCapturePermission>(parameters);
                UniWebViewMediaCapturePermissionDecision Func(UniWebViewChannelMethodMediaCapturePermission i) => (UniWebViewMediaCapturePermissionDecision)func(i);
                result = ResultJsonWith(Func(input));
                break;
            }
            default:
                result = null;
                break;
        }
        UniWebViewLogger.Instance.Debug("Channel method handler responded. Result: " + result);
        return result;
    }

    string ResultJsonWith(bool value) {
        return value ? "{\"result\":true}" : "{\"result\":false}";
    }
    
    string ResultJsonWith(UniWebViewMediaCapturePermissionDecision decision) {
        switch (decision) {
            case UniWebViewMediaCapturePermissionDecision.Prompt:
                return "{\"result\":\"prompt\"}";
            case UniWebViewMediaCapturePermissionDecision.Grant:
                return "{\"result\":\"grant\"}";
            case UniWebViewMediaCapturePermissionDecision.Deny:
                return "{\"result\":\"deny\"}";
            default:
                UniWebViewLogger.Instance.Critical("Unknown decision: " + decision);
                break;
        }
        Debug.LogAssertion("Unrecognized UniWebViewMediaCapturePermissionDecision. Fallback to prompt.");
        return "{\"result\":\"prompt\"}";
    }
}