//
//  UniWebViewInterface.cs
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
#if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using AOT;
using System.Reflection;

public class UniWebViewInterface {
    
    private const string StaticListenerName = "UniWebView-static";
    
    static UniWebViewInterface() {
        ConnectMessageSender();
        RegisterChannel();
    }

    delegate void UnitySendMessageDelegate(IntPtr objectName, IntPtr methodName, IntPtr parameter);

    private const string DllLib =
    #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        "UniWebView";
    #else
        "__Internal";
    #endif

    private static bool correctPlatform = 
    #if UNITY_EDITOR_OSX
        Application.platform == RuntimePlatform.OSXEditor || 
        Application.platform ==  RuntimePlatform.IPhonePlayer || // Support for Device Simulator package
        Application.platform ==  RuntimePlatform.Android;        // Support for Device Simulator package
    #elif UNITY_STANDALONE_OSX
        Application.platform == RuntimePlatform.OSXPlayer;
    #else
        Application.platform == RuntimePlatform.IPhonePlayer;
    #endif

    [DllImport(DllLib)]
    private static extern void uv_connectMessageSender(
        [MarshalAs(UnmanagedType.FunctionPtr)] UnitySendMessageDelegate sendMessageDelegate
    );
    static void ConnectMessageSender() {
        UniWebViewLogger.Instance.Info("Connecting to native side message sender.");
        CheckPlatform();
        uv_connectMessageSender(SendMessage);
    }

    [MonoPInvokeCallback(typeof(UnitySendMessageDelegate))]
    private static void SendMessage(IntPtr namePtr, IntPtr methodPtr, IntPtr parameterPtr) {
        string name = Marshal.PtrToStringAuto(namePtr);
        string method = Marshal.PtrToStringAuto(methodPtr);
        string parameters = Marshal.PtrToStringAuto(parameterPtr);

        UniWebViewLogger.Instance.Verbose(
            "Received message sent from native. Name: " + name + " Method: " + method + " Params: " + parameters
        );

        if (name == StaticListenerName) {
            MethodInfo methodInfo = typeof(UniWebViewStaticListener)
                .GetMethod(method, BindingFlags.Static | BindingFlags.Public);
            methodInfo.Invoke(null, new object[] { parameters });
            return;
        }
        
        var listener = UniWebViewNativeListener.GetListener(name);
        if (listener) {
            MethodInfo methodInfo = typeof(UniWebViewNativeListener).GetMethod(method);
            if (methodInfo != null) {
                methodInfo.Invoke(listener, new object[] { parameters });
            }
        }
    }
    
    delegate string ChannelMethodDelegate(IntPtr namePtr, IntPtr methodPtr, IntPtr parameterPtr);
    
    [DllImport(DllLib)]
    private static extern void uv_registerChannel([MarshalAs(UnmanagedType.FunctionPtr)] ChannelMethodDelegate channel);
    public static void RegisterChannel() {
        UniWebViewLogger.Instance.Info("Connecting to native side method channel.");
        CheckPlatform();
        uv_registerChannel(ChannelFunc);
    }

    [MonoPInvokeCallback(typeof(ChannelMethodDelegate))]
    private static string ChannelFunc(IntPtr namePtr, IntPtr methodPtr, IntPtr parameterPtr) {
        string name = Marshal.PtrToStringAuto(namePtr);
        string method = Marshal.PtrToStringAuto(methodPtr);
        string parameters = Marshal.PtrToStringAuto(parameterPtr);

        UniWebViewLogger.Instance.Verbose("ChannelFunc invoked by native side. Name: " + name + " Method: " 
                                          + method + " Params: " + parameters);
        return UniWebViewChannelMethodManager.Instance.InvokeMethod(name, method, parameters);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setLogLevel(int level);
    public static void SetLogLevel(int level) {
        CheckPlatform();
        uv_setLogLevel(level);
    }

    [DllImport(DllLib)]
    private static extern void uv_init(string name, int x, int y, int width, int height);
    public static void Init(string name, int x, int y, int width, int height) {
        CheckPlatform();
        if (String.IsNullOrEmpty(name)) {
            return;
        }
        uv_init(name, x, y, width, height);
    }

    [DllImport(DllLib)]
    private static extern void uv_destroy(string name);
    public static void Destroy(string name) {
        CheckPlatform();
        uv_destroy(name);
    }

    [DllImport(DllLib)]
    private static extern void uv_load(string name, string url, bool skipEncoding, string readAccessURL);
    public static void Load(string name, string url, bool skipEncoding, string readAccessURL) {
        CheckPlatform();
        uv_load(name, url, skipEncoding, readAccessURL);
    }

    [DllImport(DllLib)]
    private static extern void uv_loadHTMLString(string name, string html, string baseUrl, bool skipEncoding);
    public static void LoadHTMLString(string name, string html, string baseUrl, bool skipEncoding) {
        CheckPlatform();
        uv_loadHTMLString(name, html, baseUrl, skipEncoding);
    }

    [DllImport(DllLib)]
    private static extern void uv_reload(string name);
    public static void Reload(string name) {
        CheckPlatform();
        uv_reload(name);
    }

    [DllImport(DllLib)]
    private static extern void uv_stop(string name);
    public static void Stop(string name) {
        CheckPlatform();
        uv_stop(name);
    }

    [DllImport(DllLib)]
    private static extern string uv_getUrl(string name);
    public static string GetUrl(string name) {
        CheckPlatform();
        return uv_getUrl(name);
    }

    [DllImport(DllLib)]
    private static extern void uv_setFrame(string name, int x, int y, int width, int height);
    public static void SetFrame(string name, int x, int y, int width, int height) {
        CheckPlatform();
        uv_setFrame(name, x, y, width, height);
    }

    [DllImport(DllLib)]
    private static extern void uv_setPosition(string name, int x, int y);
    public static void SetPosition(string name, int x, int y) {
        CheckPlatform();
        uv_setPosition(name, x, y);
    }

    [DllImport(DllLib)]
    private static extern void uv_setSize(string name, int width, int height);
    public static void SetSize(string name, int width, int height) {
        CheckPlatform();
        uv_setSize(name, width, height);
    }

    [DllImport(DllLib)]
    private static extern bool uv_show(string name, bool fade, int edge, float duration, string identifier);
    public static bool Show(string name, bool fade, int edge, float duration, bool useAsync, string identifier) {
        CheckPlatform();
        return uv_show(name, fade, edge, duration, identifier);
    }

    [DllImport(DllLib)]
    private static extern bool uv_hide(string name, bool fade, int edge, float duration, string identifier);
    public static bool Hide(string name, bool fade, int edge, float duration, bool useAsync, string identifier) {
        CheckPlatform();
        return uv_hide(name, fade, edge, duration, identifier);
    }

    [DllImport(DllLib)]
    private static extern bool uv_animateTo(
        string name, int x, int y, int width, int height, float duration, float delay, string identifier
    );
    public static bool AnimateTo(
        string name, int x, int y, int width, int height, float duration, float delay, string identifier) 
    {
        CheckPlatform();
        return uv_animateTo(name, x, y, width, height, duration, delay, identifier);
    }

    [DllImport(DllLib)]
    private static extern void uv_addJavaScript(string name, string jsString, string identifier);
    public static void AddJavaScript(string name, string jsString, string identifier) {
        CheckPlatform();
        uv_addJavaScript(name, jsString, identifier);
    }

    [DllImport(DllLib)]
    private static extern void uv_evaluateJavaScript(string name, string jsString, string identifier);
    public static void EvaluateJavaScript(string name, string jsString, string identifier) {
        CheckPlatform();
        uv_evaluateJavaScript(name, jsString, identifier);
    }

    [DllImport(DllLib)]
    private static extern void uv_addUrlScheme(string name, string scheme);
    public static void AddUrlScheme(string name, string scheme) {
        CheckPlatform();
        uv_addUrlScheme(name, scheme);
    }

    [DllImport(DllLib)]
    private static extern void uv_removeUrlScheme(string name, string scheme);
    public static void RemoveUrlScheme(string name, string scheme) {
        CheckPlatform();
        uv_removeUrlScheme(name, scheme);
    }

    [DllImport(DllLib)]
    private static extern void uv_addSslExceptionDomain(string name, string domain);
    public static void AddSslExceptionDomain(string name, string domain) {
        CheckPlatform();
        uv_addSslExceptionDomain(name, domain);
    }

    [DllImport(DllLib)]
    private static extern void uv_removeSslExceptionDomain(string name, string domain);
    public static void RemoveSslExceptionDomain(string name, string domain) {
        CheckPlatform();
        uv_removeSslExceptionDomain(name, domain);
    }

    [DllImport(DllLib)]
    private static extern void uv_setHeaderField(string name, string key, string value);
    public static void SetHeaderField(string name, string key, string value) {
        CheckPlatform();
        uv_setHeaderField(name, key, value);
    }

    [DllImport(DllLib)]
    private static extern void uv_setUserAgent(string name, string userAgent);
    public static void SetUserAgent(string name, string userAgent) {
        CheckPlatform();
        uv_setUserAgent(name, userAgent);
    }

    [DllImport(DllLib)]
    private static extern string uv_getUserAgent(string name);
    public static string GetUserAgent(string name) {
        CheckPlatform();
        return uv_getUserAgent(name);
    }


    [DllImport(DllLib)]
    private static extern void uv_setContentInsetAdjustmentBehavior(string name, int behavior);
    public static void SetContentInsetAdjustmentBehavior(
        string name, UniWebViewContentInsetAdjustmentBehavior behavior
    ) 
    {
        CheckPlatform();
        uv_setContentInsetAdjustmentBehavior(name, (int)behavior);
    }

    [DllImport(DllLib)]
    private static extern void uv_setAllowAutoPlay(bool flag);
    public static void SetAllowAutoPlay(bool flag) {
        CheckPlatform();
        uv_setAllowAutoPlay(flag);
    }

    [DllImport(DllLib)]
    private static extern void uv_setAllowInlinePlay(bool flag);
    public static void SetAllowInlinePlay(bool flag) {
        CheckPlatform();
        uv_setAllowInlinePlay(flag);
    }

    [DllImport(DllLib)]
    private static extern void uv_setAllowFileAccess(string name, bool flag);
    public static void SetAllowFileAccess(string name, bool flag) {
        CheckPlatform();
        uv_setAllowFileAccess(name, flag);
    }
        [DllImport(DllLib)]
    private static extern void uv_setAllowFileAccessFromFileURLs(string name, bool flag);
    public static void SetAllowFileAccessFromFileURLs(string name, bool flag) {
        CheckPlatform();
        uv_setAllowFileAccessFromFileURLs(name, flag);
    }

    [DllImport(DllLib)]
    private static extern void uv_setAllowUniversalAccessFromFileURLs(bool flag);
    public static void SetAllowUniversalAccessFromFileURLs(bool flag) {
        CheckPlatform();
        uv_setAllowUniversalAccessFromFileURLs(flag);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setForwardWebConsoleToNativeOutput(bool flag);
    public static void SetForwardWebConsoleToNativeOutput(bool flag) {
        CheckPlatform();
        uv_setForwardWebConsoleToNativeOutput(flag);
    }

    [DllImport(DllLib)]
    private static extern void uv_setAllowJavaScriptOpenWindow(bool flag);
    public static void SetAllowJavaScriptOpenWindow(bool flag) {
        CheckPlatform();
        uv_setAllowJavaScriptOpenWindow(flag);
    }

    [DllImport(DllLib)]
    private static extern void uv_setJavaScriptEnabled(bool flag);
    public static void SetJavaScriptEnabled(bool flag) {
        CheckPlatform();
        uv_setJavaScriptEnabled(flag);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setLimitsNavigationsToAppBoundDomains(bool flag);
    public static void SetLimitsNavigationsToAppBoundDomains(bool flag) {
        CheckPlatform();
        uv_setLimitsNavigationsToAppBoundDomains(flag);
    }

    [DllImport(DllLib)]
    private static extern void uv_cleanCache(string name);
    public static void CleanCache(string name) {
        CheckPlatform();
        uv_cleanCache(name);
    }

    [DllImport(DllLib)]
    private static extern void uv_setCacheMode(string name, int mode);
    public static void SetCacheMode(string name, int mode) {
        CheckPlatform();
        uv_setCacheMode(name, mode);
    }

    [DllImport(DllLib)]
    private static extern void uv_clearCookies();
    public static void ClearCookies() {
        CheckPlatform();
        uv_clearCookies();
    }

    [DllImport(DllLib)]
    private static extern void uv_setCookie(string url, string cookie, bool skipEncoding);
    public static void SetCookie(string url, string cookie, bool skipEncoding) {
        CheckPlatform();
        uv_setCookie(url, cookie, skipEncoding);
    }

    [DllImport(DllLib)]
    private static extern void uv_removeCookies(string url, bool skipEncoding);
    public static void RemoveCookies(string url, bool skipEncoding) {
        CheckPlatform();
        uv_removeCookies(url, skipEncoding);
    }

    [DllImport(DllLib)]
    private static extern void uv_removeCookie(string url, string key, bool skipEncoding);
    public static void RemoveCookie(string url, string key, bool skipEncoding) {
        CheckPlatform();
        uv_removeCookie(url, key, skipEncoding);
    }

    [DllImport(DllLib)]
    private static extern string uv_getCookie(string url, string key, bool skipEncoding);
    public static string GetCookie(string url, string key, bool skipEncoding) {
        CheckPlatform();
        return uv_getCookie(url, key, skipEncoding);
    }

    [DllImport(DllLib)]
    private static extern void uv_clearHttpAuthUsernamePasswordHost(string host, string realm);
    public static void ClearHttpAuthUsernamePassword(string host, string realm) {
        CheckPlatform();
        uv_clearHttpAuthUsernamePasswordHost(host, realm);
    }

    [DllImport(DllLib)]
    private static extern void uv_setBackgroundColor(string name, float r, float g, float b, float a);
    public static void SetBackgroundColor(string name, float r, float g, float b, float a) {
        CheckPlatform();
        uv_setBackgroundColor(name, r, g, b, a);
    }

    [DllImport(DllLib)]
    private static extern void uv_setWebViewAlpha(string name, float alpha);
    public static void SetWebViewAlpha(string name, float alpha) {
        CheckPlatform();
        uv_setWebViewAlpha(name, alpha);
    }

    [DllImport(DllLib)]
    private static extern float uv_getWebViewAlpha(string name);
    public static float GetWebViewAlpha(string name) {
        CheckPlatform();
        return uv_getWebViewAlpha(name);
    }

    [DllImport(DllLib)]
    private static extern void uv_setShowSpinnerWhileLoading(string name, bool show);
    public static void SetShowSpinnerWhileLoading(string name, bool show) {
        CheckPlatform();
        uv_setShowSpinnerWhileLoading(name, show);
    }

    [DllImport(DllLib)]
    private static extern void uv_setSpinnerText(string name, string text);
    public static void SetSpinnerText(string name, string text) {
        CheckPlatform();
        uv_setSpinnerText(name, text);
    }

    [DllImport(DllLib)]
    private static extern void uv_setAllowUserDismissSpinnerByGesture(string name, bool flag);
    public static void SetAllowUserDismissSpinnerByGesture(string name, bool flag) {
        CheckPlatform();
        uv_setAllowUserDismissSpinnerByGesture(name, flag);
    }

    [DllImport(DllLib)]
    private static extern void uv_showSpinner(string name);
    public static void ShowSpinner(string name) {
        CheckPlatform();
        uv_showSpinner(name);
    }

    [DllImport(DllLib)]
    private static extern void uv_hideSpinner(string name);
    public static void HideSpinner(string name) {
        CheckPlatform();
        uv_hideSpinner(name);
    }

    [DllImport(DllLib)]
    private static extern bool uv_canGoBack(string name);
    public static bool CanGoBack(string name) {
        CheckPlatform();
        return uv_canGoBack(name);
    }

    [DllImport(DllLib)]
    private static extern bool uv_canGoForward(string name);
    public static bool CanGoForward(string name) {
        CheckPlatform();
        return uv_canGoForward(name);
    }

    [DllImport(DllLib)]
    private static extern void uv_goBack(string name);
    public static void GoBack(string name) {
        CheckPlatform();
        uv_goBack(name);
    }

    [DllImport(DllLib)]
    private static extern void uv_goForward(string name);
    public static void GoForward(string name) {
        CheckPlatform();
        uv_goForward(name);
    }

    [DllImport(DllLib)]
    private static extern void uv_setOpenLinksInExternalBrowser(string name, bool flag);
    public static void SetOpenLinksInExternalBrowser(string name, bool flag) {
        CheckPlatform();
        uv_setOpenLinksInExternalBrowser(name, flag);
    }

    [DllImport(DllLib)]
    private static extern void uv_setHorizontalScrollBarEnabled(string name, bool enabled);
    public static void SetHorizontalScrollBarEnabled(string name, bool enabled) {
        CheckPlatform();
        uv_setHorizontalScrollBarEnabled(name, enabled);
    }

    [DllImport(DllLib)]
    private static extern void uv_setVerticalScrollBarEnabled(string name, bool enabled);
    public static void SetVerticalScrollBarEnabled(string name, bool enabled) {
        CheckPlatform();
        uv_setVerticalScrollBarEnabled(name, enabled);
    }

    [DllImport(DllLib)]
    private static extern void uv_setBouncesEnabled(string name, bool enabled);
    public static void SetBouncesEnabled(string name, bool enabled) {
        CheckPlatform();
        uv_setBouncesEnabled(name, enabled);
    }

    [DllImport(DllLib)]
    private static extern void uv_setZoomEnabled(string name, bool enabled);
    public static void SetZoomEnabled(string name, bool enabled) {
        CheckPlatform();
        uv_setZoomEnabled(name, enabled);
    }

    [DllImport(DllLib)]
    private static extern void uv_setWindowUserResizeEnabled(string name, bool enabled);
    public static void SetWindowUserResizeEnabled(string name, bool enabled) {
        CheckPlatform();
        uv_setWindowUserResizeEnabled(name, enabled);
    }

    [DllImport(DllLib)]
    private static extern void uv_setUserInteractionEnabled(string name, bool enabled);
    public static void SetUserInteractionEnabled(string name, bool enabled) {
        CheckPlatform();
        uv_setUserInteractionEnabled(name, enabled);
    }

    [DllImport(DllLib)]
    private static extern void uv_setTransparencyClickingThroughEnabled(string name, bool enabled);
    public static void SetTransparencyClickingThroughEnabled(string name, bool enabled) {
        CheckPlatform();
        uv_setTransparencyClickingThroughEnabled(name, enabled);
    }

    [DllImport(DllLib)]
    private static extern void uv_setWebContentsDebuggingEnabled(bool enabled);
    public static void SetWebContentsDebuggingEnabled(bool enabled) {
        CheckPlatform();
        uv_setWebContentsDebuggingEnabled(enabled);
    }

    [DllImport(DllLib)]
    private static extern void uv_setAllowBackForwardNavigationGestures(string name, bool flag);
    public static void SetAllowBackForwardNavigationGestures(string name, bool flag) {
        CheckPlatform();
        uv_setAllowBackForwardNavigationGestures(name, flag);
    }

    [DllImport(DllLib)]
    private static extern void uv_setAllowHTTPAuthPopUpWindow(string name, bool flag);
    public static void SetAllowHTTPAuthPopUpWindow(string name, bool flag) {
        CheckPlatform();
        uv_setAllowHTTPAuthPopUpWindow(name, flag);
    }

    [DllImport(DllLib)]
    private static extern void uv_print(string name);
    public static void Print(string name) {
        CheckPlatform();
        uv_print(name);
    }

    [DllImport(DllLib)]
    private static extern void uv_captureSnapshot(string name, string fileName);
    public static void CaptureSnapshot(string name, string fileName) {
        CheckPlatform();
        uv_captureSnapshot(name, fileName);
    }

    [DllImport(DllLib)]
    private static extern void uv_scrollTo(string name, int x, int y, bool animated);
    public static void ScrollTo(string name, int x, int y, bool animated) {
        CheckPlatform();
        uv_scrollTo(name, x, y, animated);
    }

    [DllImport(DllLib)]
    private static extern void uv_setCalloutEnabled(string name, bool flag);
    public static void SetCalloutEnabled(string name, bool flag) {
        CheckPlatform();
        uv_setCalloutEnabled(name, flag);
    }

    [DllImport(DllLib)]
    private static extern void uv_setSupportMultipleWindows(string name, bool enabled, bool allowJavaScriptOpening);
    public static void SetSupportMultipleWindows(string name, bool enabled, bool allowJavaScriptOpening) {
        CheckPlatform();
        uv_setSupportMultipleWindows(name, enabled, allowJavaScriptOpening);
    }

    [DllImport(DllLib)]
    private static extern void uv_setDragInteractionEnabled(string name, bool flag);
    public static void SetDragInteractionEnabled(string name, bool flag) {
        CheckPlatform();
        uv_setDragInteractionEnabled(name, flag);
    }

    [DllImport(DllLib)]
    private static extern float uv_nativeScreenWidth();
    public static float NativeScreenWidth() {
        #if UNITY_EDITOR_OSX
        return Screen.width;
        #else
        return uv_nativeScreenWidth();
        #endif
    }

    [DllImport(DllLib)]
    private static extern float uv_nativeScreenHeight();
    public static float NativeScreenHeight() {
        #if UNITY_EDITOR_OSX
        return Screen.height;
        #else
        return uv_nativeScreenHeight();
        #endif
    }

    [DllImport(DllLib)]
    private static extern void uv_addDownloadURL(string name, string urlString, int type);
    public static void AddDownloadURL(string name, string urlString, int type) {
        CheckPlatform();
        uv_addDownloadURL(name, urlString, type);
    }

    [DllImport(DllLib)]
    private static extern void uv_removeDownloadURL(string name, string urlString, int type);
    public static void RemoveDownloadURL(string name, string urlString, int type) {
        CheckPlatform();
        uv_removeDownloadURL(name, urlString, type);
    }

    [DllImport(DllLib)]
    private static extern void uv_addDownloadMIMEType(string name, string MIMEType, int type);
    public static void AddDownloadMIMEType(string name, string MIMEType, int type) {
        CheckPlatform();
        uv_addDownloadMIMEType(name, MIMEType, type);
    }

    [DllImport(DllLib)]
    private static extern void uv_removeDownloadMIMETypes(string name, string MIMEType, int type);
    public static void RemoveDownloadMIMETypes(string name, string MIMEType, int type) {
        CheckPlatform();
        uv_removeDownloadMIMETypes(name, MIMEType, type);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setAllowUserEditFileNameBeforeDownloading(string name, bool allowed);
    public static void SetAllowUserEditFileNameBeforeDownloading(string name, bool allowed) {
        CheckPlatform();
        uv_setAllowUserEditFileNameBeforeDownloading(name, allowed);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setAllowUserChooseActionAfterDownloading(string name, bool allowed);
    public static void SetAllowUserChooseActionAfterDownloading(string name, bool allowed) {
        CheckPlatform();
        uv_setAllowUserChooseActionAfterDownloading(name, allowed);
    }

    [DllImport(DllLib)]
    private static extern void uv_safeBrowsingInit(string name, string url);
    public static void SafeBrowsingInit(string name, string url) {
        CheckPlatform();
        if (String.IsNullOrEmpty(name)) {
            return;
        }
        uv_safeBrowsingInit(name, url);
    }

    [DllImport(DllLib)]
    private static extern void uv_safeBrowsingShow(string name);
    public static void SafeBrowsingShow(string name) {
        CheckPlatform();
        uv_safeBrowsingShow(name);
    }

    [DllImport(DllLib)]
    private static extern void uv_safeBrowsingSetToolbarColor(string name, float r, float g, float b);
    public static void SafeBrowsingSetToolbarColor(string name, float r, float g, float b) {
        CheckPlatform();
        uv_safeBrowsingSetToolbarColor(name, r, g, b);
    }

    [DllImport(DllLib)]
    private static extern void uv_safeBrowsingSetToolbarItemColor(string name, float r, float g, float b);
    public static void SafeBrowsingSetToolbarItemColor(string name, float r, float g, float b) {
        CheckPlatform();
        uv_safeBrowsingSetToolbarItemColor(name, r, g, b);
    }

    [DllImport(DllLib)]
    private static extern void uv_safeBrowsingDismiss(string name);
    public static void SafeBrowsingDismiss(string name) {
        CheckPlatform();
        uv_safeBrowsingDismiss(name);
    }

    [DllImport(DllLib)]
    private static extern bool uv_authenticationIsSupported();
    public static bool IsAuthenticationIsSupported() {
        CheckPlatform();
        return uv_authenticationIsSupported();
    }

    [DllImport(DllLib)]
    private static extern void uv_authenticationInit(string name, string url, string scheme);
    public static void AuthenticationInit(string name, string url, string scheme) {
        CheckPlatform();
        uv_authenticationInit(name, url, scheme);
    }

    [DllImport(DllLib)]
    private static extern void uv_authenticationStart(string name);
    public static void AuthenticationStart(string name) {
        CheckPlatform();
        uv_authenticationStart(name);
    }

    [DllImport(DllLib)]
    private static extern void uv_authenticationSetPrivateMode(string name, bool flag);
    public static void AuthenticationSetPrivateMode(string name, bool flag) {
        CheckPlatform();
        uv_authenticationSetPrivateMode(name, flag);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setShowEmbeddedToolbar(string name, bool show);
    public static void SetShowEmbeddedToolbar(string name, bool show) {
        CheckPlatform();
        uv_setShowEmbeddedToolbar(name, show);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setEmbeddedToolbarOnTop(string name, bool top);
    public static void SetEmbeddedToolbarOnTop(string name, bool top) {
        CheckPlatform();
        uv_setEmbeddedToolbarOnTop(name, top);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setEmbeddedToolbarDoneButtonText(string name, string text);
    public static void SetEmbeddedToolbarDoneButtonText(string name, string text) {
        CheckPlatform();
        uv_setEmbeddedToolbarDoneButtonText(name, text);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setEmbeddedToolbarGoBackButtonText(string name, string text);
    public static void SetEmbeddedToolbarGoBackButtonText(string name, string text) {
        CheckPlatform();
        uv_setEmbeddedToolbarGoBackButtonText(name, text);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setEmbeddedToolbarGoForwardButtonText(string name, string text);
    public static void SetEmbeddedToolbarGoForwardButtonText(string name, string text) {
        CheckPlatform();
        uv_setEmbeddedToolbarGoForwardButtonText(name, text);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setEmbeddedToolbarTitleText(string name, string text);
    public static void SetEmbeddedToolbarTitleText(string name, string text) {
        CheckPlatform();
        uv_setEmbeddedToolbarTitleText(name, text);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setEmbeddedToolbarBackgroundColor(string name, float r, float g, float b, float a);
    public static void SetEmbeddedToolbarBackgroundColor(string name, Color color) {
        CheckPlatform();
        uv_setEmbeddedToolbarBackgroundColor(name, color.r, color.g, color.b, color.a);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setEmbeddedToolbarButtonTextColor(string name, float r, float g, float b, float a);
    public static void SetEmbeddedToolbarButtonTextColor(string name, Color color) {
        CheckPlatform();
        uv_setEmbeddedToolbarButtonTextColor(name, color.r, color.g, color.b, color.a);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setEmbeddedToolbarTitleTextColor(string name, float r, float g, float b, float a);
    public static void SetEmbeddedToolbarTitleTextColor(string name, Color color) {
        CheckPlatform();
        uv_setEmbeddedToolbarTitleTextColor(name, color.r, color.g, color.b, color.a);
    }
    
    [DllImport(DllLib)]
    private static extern void uv_setEmbeddedToolbarNavigationButtonsShow(string name, bool show);
    public static void SetEmeddedToolbarNavigationButtonsShow(string name, bool show) {
        CheckPlatform();
        uv_setEmbeddedToolbarNavigationButtonsShow(name, show);
    }

    [DllImport(DllLib)]
    private static extern void uv_startSnapshotForRendering(string name, string identifier);
    public static void StartSnapshotForRendering(string name, string identifier) {
        CheckPlatform();
        uv_startSnapshotForRendering(name, identifier);
    }

    [DllImport(DllLib)]
    private static extern void uv_stopSnapshotForRendering(string name);
    public static void StopSnapshotForRendering(string name) {
        CheckPlatform();
        uv_stopSnapshotForRendering(name);
    }
    
    [DllImport(DllLib)]
    private static extern IntPtr uv_getRenderedData(string name, int x, int y, int width, int height, out int length);
    public static byte[] GetRenderedData(string name, int x, int y, int width, int height) {
        CheckPlatform();

        IntPtr dataPtr = uv_getRenderedData(name, x, y, width, height, out var length);
        
        byte[] managedData = new byte[length];
        Marshal.Copy(dataPtr, managedData, 0, length);
        
        return managedData;
    }

    #region Deprecated
    
    [DllImport(DllLib)]
    private static extern void uv_setShowToolbar(string name, bool show, bool animated, bool onTop, bool adjustInset);
    public static void SetShowToolbar(string name, bool show, bool animated, bool onTop, bool adjustInset) {
        CheckPlatform();
        uv_setShowToolbar(name, show, animated, onTop, adjustInset);
    }

    [DllImport(DllLib)]
    private static extern void uv_setShowToolbarNavigationButtons(string name, bool show);
    public static void SetShowToolbarNavigationButtons(string name, bool show) {
        CheckPlatform();
        uv_setShowToolbarNavigationButtons(name, show);
    }

    [DllImport(DllLib)]
    private static extern void uv_setToolbarDoneButtonText(string name, string text);
    public static void SetToolbarDoneButtonText(string name, string text) {
        CheckPlatform();
        uv_setToolbarDoneButtonText(name, text);
    }

    [DllImport(DllLib)]
    private static extern void uv_setGoBackButtonText(string name, string text);
    public static void SetToolbarGoBackButtonText(string name, string text) { 
        CheckPlatform(); 
        uv_setGoBackButtonText(name, text);
    }

    [DllImport(DllLib)]
    private static extern void uv_setGoForwardButtonText(string name, string text);
    public static void SetToolbarGoForwardButtonText(string name, string text) { 
        CheckPlatform();
        uv_setGoForwardButtonText(name, text);
    }
    
    
    [DllImport(DllLib)]
    private static extern void uv_setToolbarTintColor(string name, float r, float g, float b);
    public static void SetToolbarTintColor(string name, float r, float g, float b) {
        CheckPlatform();
        uv_setToolbarTintColor(name, r, g, b);
    }

    [DllImport(DllLib)]
    private static extern void uv_setToolbarTextColor(string name, float r, float g, float b);
    public static void SetToolbarTextColor(string name, float r, float g, float b) {
        CheckPlatform();
        uv_setToolbarTextColor(name, r, g, b);
    }
    
    #endregion

    public static void CheckPlatform() {
        if (!correctPlatform) {
            throw new System.InvalidOperationException(
                "Method can only be performed on correct platform. Current: " + Application.platform
            );
        }
    }
}
#endif