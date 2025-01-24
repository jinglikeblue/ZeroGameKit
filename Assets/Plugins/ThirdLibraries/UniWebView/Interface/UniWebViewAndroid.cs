#if UNITY_ANDROID && !UNITY_EDITOR

using UnityEngine;

class UniWebViewMethodChannel: AndroidJavaProxy
    {
        public UniWebViewMethodChannel() : base("com.onevcat.uniwebview.UniWebViewNativeChannel") { }

        string invokeChannelMethod(string name, string method, string parameters) {
            UniWebViewLogger.Instance.Verbose("invokeChannelMethod invoked by native side. Name: " + name + " Method: " 
                                          + method + " Params: " + parameters);
            return UniWebViewChannelMethodManager.Instance.InvokeMethod(name, method, parameters);
        }
    }

public class UniWebViewInterface {
    private static readonly AndroidJavaClass plugin;
    private static bool correctPlatform = Application.platform == RuntimePlatform.Android;
    
    static UniWebViewInterface() {
        var go = new GameObject("UniWebViewAndroidStaticListener");
        go.AddComponent<UniWebViewAndroidStaticListener>();
        plugin = new AndroidJavaClass("com.onevcat.uniwebview.UniWebViewInterface");
        
        CheckPlatform();

        plugin.CallStatic("prepare");

        UniWebViewLogger.Instance.Info("Connecting to native side method channel.");
        plugin.CallStatic("registerChannel", new UniWebViewMethodChannel());
    }

    public static void SetLogLevel(int level) {
        CheckPlatform();
        plugin.CallStatic("setLogLevel", level); 
    }

    public static bool IsWebViewSupported() {
        CheckPlatform();
        return plugin.CallStatic<bool>("isWebViewSupported");
    }

    public static void Init(string name, int x, int y, int width, int height) {
        CheckPlatform();
        plugin.CallStatic("init", name, x, y, width, height);
    }

    public static void Destroy(string name) {
        CheckPlatform();
        plugin.CallStatic("destroy", name);
    }

    public static void Load(string name, string url, bool skipEncoding, string readAccessURL) {
        CheckPlatform();
        plugin.CallStatic("load", name, url);
    }

    public static void LoadHTMLString(string name, string html, string baseUrl, bool skipEncoding) {
        CheckPlatform();
        plugin.CallStatic("loadHTMLString", name, html, baseUrl);
    }

    public static void Reload(string name) {
        CheckPlatform();
        plugin.CallStatic("reload", name);
    }

    public static void Stop(string name) {
        CheckPlatform();
        plugin.CallStatic("stop", name);
    }

    public static string GetUrl(string name) {
        CheckPlatform();
        return plugin.CallStatic<string>("getUrl", name);
    }

    public static void SetFrame(string name, int x, int y, int width, int height) {
        CheckPlatform();
        plugin.CallStatic("setFrame", name, x, y, width, height);
    }

    public static void SetPosition(string name, int x, int y) {
        CheckPlatform();
        plugin.CallStatic("setPosition", name, x, y);
    }

    public static void SetSize(string name, int width, int height) {
        CheckPlatform();
        plugin.CallStatic("setSize", name, width, height);
    }

    public static bool Show(string name, bool fade, int edge, float duration, bool useAsync, string identifier) {
        CheckPlatform();
        if (useAsync) {
            plugin.CallStatic("showAsync", name, fade, edge, duration, identifier);
            return true;
        } else {
            return plugin.CallStatic<bool>("show", name, fade, edge, duration, identifier);
        }
    }

    public static bool Hide(string name, bool fade, int edge, float duration, bool useAsync, string identifier) {
        CheckPlatform();
        if (useAsync) {
            plugin.CallStatic("hideAsync", name, fade, edge, duration, identifier);
            return true;
        } else {
            return plugin.CallStatic<bool>("hide", name, fade, edge, duration, identifier);
        }
    }

    public static bool AnimateTo(string name, int x, int y, int width, int height, float duration, float delay, string identifier) {
        CheckPlatform();
        return plugin.CallStatic<bool>("animateTo", name, x, y, width, height, duration, delay, identifier);
    }

    public static void AddJavaScript(string name, string jsString, string identifier) {
        CheckPlatform();
        plugin.CallStatic("addJavaScript", name, jsString, identifier);
    }

    public static void EvaluateJavaScript(string name, string jsString, string identifier) {
        CheckPlatform();
        plugin.CallStatic("evaluateJavaScript", name, jsString, identifier);
    }

    public static void AddUrlScheme(string name, string scheme) {
        CheckPlatform();
        plugin.CallStatic("addUrlScheme", name, scheme);
    }

    public static void RemoveUrlScheme(string name, string scheme) {
        CheckPlatform();
        plugin.CallStatic("removeUrlScheme", name, scheme);
    }

    public static void AddSslExceptionDomain(string name, string domain) {
        CheckPlatform();
        plugin.CallStatic("addSslExceptionDomain", name, domain);
    }

    public static void RemoveSslExceptionDomain(string name, string domain) {
        CheckPlatform();
        plugin.CallStatic("removeSslExceptionDomain", name, domain);
    }

    public static void AddPermissionTrustDomain(string name, string domain) {
        CheckPlatform();
        plugin.CallStatic("addPermissionTrustDomain", name, domain);
    }

    public static void RemovePermissionTrustDomain(string name, string domain) {
        CheckPlatform();
        plugin.CallStatic("removePermissionTrustDomain", name, domain);
    }

    public static void SetHeaderField(string name, string key, string value) {
        CheckPlatform();
        plugin.CallStatic("setHeaderField", name, key, value);
    }

    public static void SetUserAgent(string name, string userAgent) {
        CheckPlatform();
        plugin.CallStatic("setUserAgent", name, userAgent);
    }

    public static string GetUserAgent(string name) {
        CheckPlatform();
        return plugin.CallStatic<string>("getUserAgent", name);
    }

    public static void SetAllowAutoPlay(bool flag) {
        CheckPlatform();
        plugin.CallStatic("setAllowAutoPlay", flag);
    }

    public static void SetAllowJavaScriptOpenWindow(bool flag) {
        CheckPlatform();
        plugin.CallStatic("setAllowJavaScriptOpenWindow", flag);
    }

    public static void SetAllowFileAccess(string name, bool flag) { 
        CheckPlatform();
        plugin.CallStatic("setAllowFileAccess", name, flag);
    }

    public static void SetAcceptThirdPartyCookies(string name, bool flag) {
        CheckPlatform();
        plugin.CallStatic("setAcceptThirdPartyCookies", name, flag);
    }

    public static void SetAllowFileAccessFromFileURLs(string name, bool flag) { 
        CheckPlatform();
        plugin.CallStatic("setAllowFileAccessFromFileURLs", name, flag);
    }

    public static void SetAllowUniversalAccessFromFileURLs(bool flag) {
        CheckPlatform();
        plugin.CallStatic("setAllowUniversalAccessFromFileURLs", flag);
    }
    public static void BringContentToFront(string name) {
        CheckPlatform();
        plugin.CallStatic("bringContentToFront", name);
    }

    public static void SetForwardWebConsoleToNativeOutput(bool flag) {
        CheckPlatform();
        plugin.CallStatic("setForwardWebConsoleToNativeOutput", flag);
    }

    public static void SetEnableKeyboardAvoidance(bool flag) {
        CheckPlatform();
        plugin.CallStatic("setEnableKeyboardAvoidance", flag);
    }

    public static void SetJavaScriptEnabled(bool enabled) {
        CheckPlatform();
        plugin.CallStatic("setJavaScriptEnabled", enabled);
    }

    public static void CleanCache(string name) {
        CheckPlatform();
        plugin.CallStatic("cleanCache", name);
    }

    public static void SetCacheMode(string name, int mode) {
        CheckPlatform();
        plugin.CallStatic("setCacheMode", name, mode);
    }

    public static void ClearCookies() {
        CheckPlatform();
        plugin.CallStatic("clearCookies");
    }

    public static void SetCookie(string url, string cookie, bool skipEncoding) {
        CheckPlatform();
        plugin.CallStatic("setCookie", url, cookie);
    }

    public static string GetCookie(string url, string key, bool skipEncoding) {
        CheckPlatform();
        return plugin.CallStatic<string>("getCookie", url, key);
    }

    public static void RemoveCookies(string url, bool skipEncoding) {
        CheckPlatform();
        plugin.CallStatic("removeCookies", url);
    }

    public static void RemoveCookie(string url, string key, bool skipEncoding) {
        CheckPlatform();
        plugin.CallStatic("removeCookie", url, key);
    }

    public static void ClearHttpAuthUsernamePassword(string host, string realm) {
        CheckPlatform();
        plugin.CallStatic("clearHttpAuthUsernamePassword", host, realm);
    }

    public static void SetBackgroundColor(string name, float r, float g, float b, float a) {
        CheckPlatform();
        plugin.CallStatic("setBackgroundColor", name, r, g, b, a);
    }

    public static void SetWebViewAlpha(string name, float alpha) {
        CheckPlatform();
        plugin.CallStatic("setWebViewAlpha", name, alpha);
    }

    public static float GetWebViewAlpha(string name) {
        CheckPlatform();
        return plugin.CallStatic<float>("getWebViewAlpha", name);
    }

    public static void SetShowSpinnerWhileLoading(string name, bool show) {
        CheckPlatform();
        plugin.CallStatic("setShowSpinnerWhileLoading", name, show);
    }

    public static void SetSpinnerText(string name, string text) {
        CheckPlatform();
        plugin.CallStatic("setSpinnerText", name, text);
    }

    public static void SetAllowUserDismissSpinnerByGesture(string name, bool flag) {
        CheckPlatform();
        plugin.CallStatic("setAllowUserDismissSpinnerByGesture", name, flag);
    }

    public static void ShowSpinner(string name) {
        CheckPlatform();
        plugin.CallStatic("showSpinner", name);
    }

    public static void HideSpinner(string name) {
        CheckPlatform();
        plugin.CallStatic("hideSpinner", name);
    }

    public static bool CanGoBack(string name) {
        CheckPlatform();
        return plugin.CallStatic<bool>("canGoBack", name);
    }

    public static bool CanGoForward(string name) {
        CheckPlatform();
        return plugin.CallStatic<bool>("canGoForward", name);
    }

    public static void GoBack(string name) {
        CheckPlatform();
        plugin.CallStatic("goBack", name);
    }
    public static void GoForward(string name) {
        CheckPlatform();
        plugin.CallStatic("goForward", name);
    }

    public static void SetOpenLinksInExternalBrowser(string name, bool flag) {
        CheckPlatform();
        plugin.CallStatic("setOpenLinksInExternalBrowser", name, flag);
    }

    public static void SetHorizontalScrollBarEnabled(string name, bool enabled) {
        CheckPlatform();
        plugin.CallStatic("setHorizontalScrollBarEnabled", name, enabled);
    }

    public static void SetVerticalScrollBarEnabled(string name, bool enabled) {
        CheckPlatform();
        plugin.CallStatic("setVerticalScrollBarEnabled", name, enabled);
    }

    public static void SetBouncesEnabled(string name, bool enabled) {
        CheckPlatform();
        plugin.CallStatic("setBouncesEnabled", name, enabled);
    }

    public static void SetZoomEnabled(string name, bool enabled) {
        CheckPlatform();
        plugin.CallStatic("setZoomEnabled", name, enabled);
    }

    public static void SetUseWideViewPort(string name, bool use) {
        CheckPlatform();
        plugin.CallStatic("setUseWideViewPort", name, use);
    }

    public static void SetLoadWithOverviewMode(string name, bool overview) {
        CheckPlatform();
        plugin.CallStatic("setLoadWithOverviewMode", name, overview);
    }

    public static void SetImmersiveModeEnabled(string name, bool enabled) {
        CheckPlatform();
        plugin.CallStatic("setImmersiveModeEnabled", name, enabled);
    }

    public static void SetUserInteractionEnabled(string name, bool enabled) {
        CheckPlatform();
        plugin.CallStatic("setUserInteractionEnabled", name, enabled);
    }

    public static void SetTransparencyClickingThroughEnabled(string name, bool enabled) {
        CheckPlatform();
        plugin.CallStatic("setTransparencyClickingThroughEnabled", name, enabled);
    }

    public static void SetWebContentsDebuggingEnabled(bool enabled) {
        CheckPlatform();
        plugin.CallStatic("setWebContentsDebuggingEnabled", enabled);
    }

    public static void SetAllowHTTPAuthPopUpWindow(string name, bool flag) {
        CheckPlatform();
        plugin.CallStatic("setAllowHTTPAuthPopUpWindow", name, flag);
    }

    public static void Print(string name) {
        CheckPlatform();
        plugin.CallStatic("print", name);
    }

    public static void CaptureSnapshot(string name, string filename) { 
        CheckPlatform();
        plugin.CallStatic("captureSnapshot", name, filename);
    }

    public static void ScrollTo(string name, int x, int y, bool animated) {
        CheckPlatform();
        plugin.CallStatic("scrollTo", name, x, y, animated);
    }

    public static void SetCalloutEnabled(string name, bool flag) {
        CheckPlatform();
        plugin.CallStatic("setCalloutEnabled", name, flag);
    }

    public static void SetSupportMultipleWindows(string name, bool enabled, bool allowJavaScriptOpening) {
        CheckPlatform();
        plugin.CallStatic("setSupportMultipleWindows", name, enabled, allowJavaScriptOpening);
    }

    public static void SetDragInteractionEnabled(string name, bool flag) {
        CheckPlatform();
        plugin.CallStatic("setDragInteractionEnabled", name, flag);
    }

    public static void SetDefaultFontSize(string name, int size) {
        CheckPlatform();
        plugin.CallStatic("setDefaultFontSize", name, size);
    }

    public static void SetTextZoom(string name, int textZoom) { 
        CheckPlatform();
        plugin.CallStatic("setTextZoom", name, textZoom);
    }

    public static float NativeScreenWidth() {
        CheckPlatform();
        return plugin.CallStatic<float>("screenWidth");
    }

    public static float NativeScreenHeight() {
        CheckPlatform();
        return plugin.CallStatic<float>("screenHeight");
    }

    public static void SetDownloadEventForContextMenuEnabled(string name, bool enabled) {
        CheckPlatform();
        plugin.CallStatic("setDownloadEventForContextMenuEnabled", name, enabled);
    }

    public static void SetAllowUserEditFileNameBeforeDownloading(string name, bool allowed) {
        CheckPlatform();
        plugin.CallStatic("setAllowUserEditFileNameBeforeDownloading", name, allowed);
    }

    // Safe Browsing

    public static bool IsSafeBrowsingSupported() {
        CheckPlatform();
        return plugin.CallStatic<bool>("isSafeBrowsingSupported");
    }

    public static void SafeBrowsingInit(string name, string url) { 
        CheckPlatform();
        plugin.CallStatic("safeBrowsingInit", name, url);
    }

    public static void SafeBrowsingSetToolbarColor(string name, float r, float g, float b) {
        CheckPlatform(); 
        plugin.CallStatic("safeBrowsingSetToolbarColor", name, r, g, b);
    }

    public static void SafeBrowsingShow(string name) {
        CheckPlatform();
        plugin.CallStatic("safeBrowsingShow", name);
    }

    // Authentication

    public static bool IsAuthenticationIsSupported() {
        CheckPlatform();
        return plugin.CallStatic<bool>("isAuthenticationIsSupported");
    }

    public static void AuthenticationInit(string name, string url, string scheme) {
        CheckPlatform();
        plugin.CallStatic("authenticationInit", name, url, scheme);
    }

    public static void AuthenticationStart(string name) {
        CheckPlatform();
        plugin.CallStatic("authenticationStart", name);
    }

    public static void AuthenticationSetPrivateMode(string name, bool enabled) {
        CheckPlatform();
        plugin.CallStatic("authenticationSetPrivateMode", name, enabled);
    }

    public static void SetShowEmbeddedToolbar(string name, bool show) {
        CheckPlatform();
        plugin.CallStatic("setShowEmbeddedToolbar", name, show);
    }

    public static void SetEmbeddedToolbarOnTop(string name, bool top) {
        CheckPlatform();
        plugin.CallStatic("setEmbeddedToolbarOnTop", name, top);
    }

    public static void SetEmbeddedToolbarDoneButtonText(string name, string text) {
        CheckPlatform();
        plugin.CallStatic("setEmbeddedToolbarDoneButtonText", name, text);
    }

    public static void SetEmbeddedToolbarGoBackButtonText(string name, string text) {
        CheckPlatform();
        plugin.CallStatic("setEmbeddedToolbarGoBackButtonText", name, text);
    }

    public static void SetEmbeddedToolbarGoForwardButtonText(string name, string text) {
        CheckPlatform();
        plugin.CallStatic("setEmbeddedToolbarGoForwardButtonText", name, text);
    }
    
    public static void SetEmbeddedToolbarTitleText(string name, string text) {
        CheckPlatform();
        plugin.CallStatic("setEmbeddedToolbarTitleText", name, text);
    }

    public static void SetEmbeddedToolbarBackgroundColor(string name, Color color) {
        CheckPlatform();
        plugin.CallStatic("setEmbeddedToolbarBackgroundColor", name, color.r, color.g, color.b, color.a);
    }
    
    public static void SetEmbeddedToolbarButtonTextColor(string name, Color color) {
        CheckPlatform();
        plugin.CallStatic("setEmbeddedToolbarButtonTextColor", name, color.r, color.g, color.b, color.a);
    }

    public static void SetEmbeddedToolbarTitleTextColor(string name, Color color) {
        CheckPlatform();
        plugin.CallStatic("setEmbeddedToolbarTitleTextColor", name, color.r, color.g, color.b, color.a);
    }

    public static void SetEmeddedToolbarNavigationButtonsShow(string name, bool show) {
        CheckPlatform();
        plugin.CallStatic("setEmbeddedToolbarNavigationButtonsShow", name, show);
    }

    public static void StartSnapshotForRendering(string name, string identifier) {
        CheckPlatform();
        plugin.CallStatic("startSnapshotForRendering", name, identifier);
    }

    public static void StopSnapshotForRendering(string name) {
        CheckPlatform();
        plugin.CallStatic("stopSnapshotForRendering", name);
    }

    public static byte[] GetRenderedData(string name, int x, int y, int width, int height) {
        CheckPlatform();
        var sbyteArray = plugin.CallStatic<sbyte[]>("getRenderedData", name, x, y, width, height);
        if (sbyteArray == null) {
            return null;
        }
        int length = sbyteArray.Length;
        byte[] byteArray = new byte[length];
        
        for (int i = 0; i < length; i++) {
            byteArray[i] = (byte)sbyteArray[i];
        }   
        return byteArray;
    }

    // Platform

    public static void CheckPlatform() {
        if (!correctPlatform) {
            throw new System.InvalidOperationException("Method can only be performed on Android.");
        }
    }
}
#endif