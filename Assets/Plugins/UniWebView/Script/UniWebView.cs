//
//  UniWebView.cs
//  Created by Wang Wei (@onevcat) on 2017-04-11.
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
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Main class of UniWebView. Any `GameObject` instance with this script can represent a webview object in the scene. 
/// Use this class to create, load, show and interact with a general-purpose web view.
/// </summary>
public class UniWebView: MonoBehaviour {
    /// <summary>
    /// Delegate for page started event.
    /// </summary>
    /// <param name="webView">The web view component which raises this event.</param>
    /// <param name="url">The url which the web view is about to load.</param>
    public delegate void PageStartedDelegate(UniWebView webView, string url);
    
    /// <summary>
    /// Raised when the web view starts loading a url.
    /// 
    /// This event will be invoked for both url loading with `Load` method or by a link navigating from page.
    /// </summary>
    public event PageStartedDelegate OnPageStarted;

    /// <summary>
    /// Delegate for page finished event.
    /// </summary>
    /// <param name="webView">The web view component which raises this event.</param>
    /// <param name="statusCode">HTTP status code received from response.</param>
    /// <param name="url">The url which the web view loaded.</param>
    public delegate void PageFinishedDelegate(UniWebView webView, int statusCode, string url);
    /// <summary>
    /// Raised when the web view finished to load a url successfully.
    /// 
    /// This method will be invoked when a valid response received from the url, regardless of the response status.
    /// If a url loading fails before reaching to the server and getting a response, `OnPageErrorReceived` will be 
    /// raised instead.
    /// </summary>
    public event PageFinishedDelegate OnPageFinished;

    /// <summary>
    /// Delegate for page error received event.
    /// </summary>
    /// <param name="webView">The web view component which raises this event.</param>
    /// <param name="errorCode">
    /// The error code which indicates the error type. 
    /// It can be different from systems and platforms.
    /// </param>
    /// <param name="errorMessage">The error message which indicates the error.</param>
    public delegate void PageErrorReceivedDelegate(UniWebView webView, int errorCode, string errorMessage);
    /// <summary>
    /// Raised when an error encountered during the loading process. 
    /// Such as the "host not found" error or "no Internet connection" error will raise this event.
    /// </summary>
    public event PageErrorReceivedDelegate OnPageErrorReceived;

    /// <summary>
    /// Delegate for page progress changed event.
    /// </summary>
    /// <param name="webView">The web view component which raises this event.</param>
    /// <param name="progress">A value indicates the loading progress of current page. It is a value between 0.0f and 1.0f.</param>
    public delegate void PageProgressChangedDelegate(UniWebView webView, float progress);
    /// <summary>
    /// Raised when the loading progress value changes in current web view.
    /// </summary>
    public event PageProgressChangedDelegate OnPageProgressChanged;

    /// <summary>
    /// Delegate for message received event.
    /// </summary>
    /// <param name="webView">The web view component which raises this event.</param>
    /// <param name="message">Message received from web view.</param>
    public delegate void MessageReceivedDelegate(UniWebView webView, UniWebViewMessage message);
    /// <summary>
    /// Raised when a message from web view is received. 
    /// 
    /// Generally, the message comes from a navigation to 
    /// a scheme which is observed by current web view. You could use `AddUrlScheme` and 
    /// `RemoveUrlScheme` to manipulate the scheme list.
    /// 
    /// "uniwebview://" scheme is default in the list, so a clicking on link starting with "uniwebview://"
    /// will raise this event, if it is not removed.
    /// </summary>
    public event MessageReceivedDelegate OnMessageReceived;

    /// <summary>
    /// Delegate for should close event.
    /// </summary>
    /// <param name="webView">The web view component which raises this event.</param>
    /// <returns>Whether the web view should be closed and destroyed.</returns>
    public delegate bool ShouldCloseDelegate(UniWebView webView);
    /// <summary>
    /// Raised when the web view is about to close itself.
    /// 
    /// This event is raised when the users close the web view by Back button on Android, Done button on iOS,
    /// or Close button on Unity Editor. It gives a chance to make final decision whether the web view should 
    /// be closed and destroyed. You can also clean all related resources you created (such as a reference to
    /// the web view) in this event.
    /// </summary>
    public event ShouldCloseDelegate OnShouldClose;

    /// <summary>
    /// Delegate for orientation changed event.
    /// </summary>
    /// <param name="webView">The web view component which raises this event.</param>
    /// <param name="orientation">The screen orientation for current state.</param>
    public delegate void OrientationChangedDelegate(UniWebView webView, ScreenOrientation orientation);
    /// <summary>
    /// Raised when the screen orientation is changed. It is a good time to set the web view frame if you 
    /// need to support multiple orientations in your game.
    /// </summary>
    public event OrientationChangedDelegate OnOrientationChanged;

    /// <summary>
    /// Delegate for content loading terminated event.
    /// </summary>
    /// <param name="webView">The web view component which raises this event.</param>
    public delegate void OnWebContentProcessTerminatedDelegate(UniWebView webView);
    /// <summary>
    /// Raised when on iOS, when system calls `webViewWebContentProcessDidTerminate` method. 
    /// It is usually due to a low memory when loading the web content and leave you a blank white screen. 
    /// You need to free as much as memory you could and then do a page reload.
    /// </summary>
    public event OnWebContentProcessTerminatedDelegate OnWebContentProcessTerminated;

    /// <summary>
    /// Delegate for file download task starting event.
    /// </summary>
    /// <param name="webView">The web view component which raises this event.</param>
    /// <param name="remoteUrl">The remote URL of this download task. This is also the download URL for the task.</param>
    /// <param name="fileName">The file name which user chooses to use.</param>
    public delegate void FileDownloadStarted(UniWebView webView, string remoteUrl, string fileName);
    /// <summary>
    /// Raised when a file download task starts.
    /// </summary>
    public event FileDownloadStarted OnFileDownloadStarted;

    /// <summary>
    /// Delegate for file download task finishing event.
    /// </summary>
    /// <param name="webView">The web view component which raises this event.</param>
    /// <param name="errorCode">
    /// The error code of the download task result. Value `0` means the download finishes without a problem. 
    /// Any other non-`0` value indicates an issue. The detail meaning of the error code depends on system. 
    /// On iOS, it is usually the `errorCode` of the received `NSURLError`. On Android, the value usually represents
    /// an `ERROR_*` value in `DownloadManager`.
    /// </param>
    /// <param name="remoteUrl">The remote URL of this download task.</param>
    /// <param name="diskPath">
    /// The file path of the downloaded file. On iOS, the downloader file is in a temporary folder of your app sandbox.
    /// On Android, it is in the "Download" folder of your app.
    /// </param>
    public delegate void FileDownloadFinished(UniWebView webView, int errorCode, string remoteUrl, string diskPath);
    /// <summary>
    /// Raised when a file download task finishes with either an error or success.
    /// </summary>
    public event FileDownloadFinished OnFileDownloadFinished;

    /// <summary>
    /// Delegate for capturing snapshot finished event.
    /// </summary>
    /// <param name="webView">The web view component which raises this event.</param>
    /// <param name="errorCode">
    /// The error code of the event. If the snapshot is captured and stored without a problem, the error code is 0.
    /// Any other number indicates an error happened. In most cases, the screenshot capturing only fails due to lack
    /// of disk storage.
    /// </param>
    /// <param name="diskPath">
    /// An accessible disk path to the captured snapshot image. If an error happens, it is an empty string.
    /// </param>
    public delegate void CaptureSnapshotFinished(UniWebView webView, int errorCode, string diskPath);
    /// <summary>
    /// Raised when an image captured and stored in a cache path on disk.
    /// </summary>
    public event CaptureSnapshotFinished OnCaptureSnapshotFinished;

    /// <summary>
    /// Delegate for multiple window opening event.
    /// </summary>
    /// <param name="webView">The web view component which opens the new multiple (pop-up) window.</param>
    /// <param name="multipleWindowId">The identifier of the opened new window.</param>
    public delegate void MultipleWindowOpenedDelegate(UniWebView webView, string multipleWindowId);
    /// <summary>
    /// Raised when a new window is opened. This happens when you enable the `SetSupportMultipleWindows` and open a
    /// new pop-up window.
    /// </summary>
    public event MultipleWindowOpenedDelegate OnMultipleWindowOpened;

    /// <summary>
    /// Delegate for multiple window closing event.
    /// </summary>
    /// <param name="webView">The web view component which closes the multiple window.</param>
    /// <param name="multipleWindowId">The identifier of the closed window.</param>
    public delegate void MultipleWindowClosedDelegate(UniWebView webView, string multipleWindowId);
    /// <summary>
    /// Raised when the multiple window is closed. This happens when the pop-up window is closed by navigation operation
    /// or by a invocation of `close()` on the page.
    /// </summary>
    public event MultipleWindowClosedDelegate OnMultipleWindowClosed;

    private string id = Guid.NewGuid().ToString();
    private UniWebViewNativeListener listener;

    private bool isPortrait;
    [SerializeField]

    #pragma warning disable 0649 
    private string urlOnStart;
    [SerializeField]
    private bool showOnStart = false;

    [SerializeField]
    private bool fullScreen;

    [SerializeField]
    private bool useToolbar;

        
    [SerializeField]
    private UniWebViewToolbarPosition toolbarPosition;

    #pragma warning restore 0649

    // Action callback holders
    private Dictionary<String, Action> actions = new Dictionary<String, Action>();
    private Dictionary<String, Action<UniWebViewNativeResultPayload>> payloadActions = new Dictionary<String, Action<UniWebViewNativeResultPayload>>();

    [SerializeField]
    private Rect frame;
    /// <summary>
    /// Gets or sets the frame of current web view. The value is based on current `Screen.width` and `Screen.height`.
    /// The first two values of `Rect` is `x` and `y` position and the followed two `width` and `height`.
    /// </summary>
    public Rect Frame {
        get { return frame; }
        set {
            frame = value;
            UpdateFrame();
        }
    }

    [SerializeField]
    private RectTransform referenceRectTransform;
    /// <summary>
    /// A reference rect transform which the web view should change its position and size to.
    /// Set it to a Unity UI element (which contains a `RectTransform`) under a canvas to determine 
    /// the web view frame by a certain UI element. 
    /// 
    /// By using this, you could get benefit from [Multiple Resolutions UI](https://docs.unity3d.com/Manual/HOWTO-UIMultiResolution.html).
    /// 
    /// </summary>
    public RectTransform ReferenceRectTransform {
        get {
            return referenceRectTransform;
        }
        set {
            referenceRectTransform = value;
            UpdateFrame();
        }
    }

    private bool started;

    private bool backButtonEnabled = true;

    /// <summary>
    /// The url of current loaded web page.
    /// </summary>
    public string Url {
        get { return UniWebViewInterface.GetUrl(listener.Name); } 
    }

    /// <summary>
    /// Updates and sets current frame of web view to match the setting.
    /// 
    /// This is useful if the `referenceRectTransform` is changed and you need to sync the frame change
    /// to the web view. This method follows the frame determining rules.
    /// </summary>
    public void UpdateFrame() {
        Rect rect = NextFrameRect();
        UniWebViewInterface.SetFrame(listener.Name, (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
    }

    Rect NextFrameRect() {
        if (referenceRectTransform == null) {
            UniWebViewLogger.Instance.Info("Using Frame setting to determine web view frame.");
            return frame;
        } else {
            UniWebViewLogger.Instance.Info("Using reference RectTransform to determine web view frame.");
            var worldCorners = new Vector3[4];
            
            referenceRectTransform.GetWorldCorners(worldCorners);
            
            var bottomLeft = worldCorners[0];
            var topLeft = worldCorners[1];
            var topRight = worldCorners[2];
            var bottomRight = worldCorners[3];

            var canvas = referenceRectTransform.GetComponentInParent<Canvas>();
            if (canvas == null) {
                return frame;
            }

            switch (canvas.renderMode) {
                case RenderMode.ScreenSpaceOverlay:
                    break;
                case RenderMode.ScreenSpaceCamera:
                case RenderMode.WorldSpace:
                    var camera = canvas.worldCamera;
                    if (camera == null) {
                        UniWebViewLogger.Instance.Critical(@"You need a render camera 
                        or event camera to use RectTransform to determine correct 
                        frame for UniWebView.");
                        UniWebViewLogger.Instance.Info("No camera found. Fall back to ScreenSpaceOverlay mode.");
                    } else {
                        bottomLeft = camera.WorldToScreenPoint(bottomLeft);
                        topLeft = camera.WorldToScreenPoint(topLeft);
                        topRight = camera.WorldToScreenPoint(topRight);
                        bottomRight = camera.WorldToScreenPoint(bottomRight);
                    }
                    break;
            }

            float widthFactor = (float)UniWebViewInterface.NativeScreenWidth() / (float)Screen.width;
            float heightFactor = (float)UniWebViewInterface.NativeScreenHeight() / (float)Screen.height;

            float x = topLeft.x * widthFactor;
            float y = (Screen.height - topLeft.y) * heightFactor;
            float width = (bottomRight.x - topLeft.x) * widthFactor;
            float height = (topLeft.y - bottomRight.y) * heightFactor;
            return new Rect(x, y, width, height);
        }
    }

    void Awake() {
        var listenerObject = new GameObject(id);
        listener = listenerObject.AddComponent<UniWebViewNativeListener>();
        listenerObject.transform.parent = transform;
        listener.webView = this;
        UniWebViewNativeListener.AddListener(listener);

        Rect rect;
        if (fullScreen) {
            rect = new Rect(0, 0, Screen.width, Screen.height);
        } else {
            rect = NextFrameRect();
        }

        UniWebViewInterface.Init(listener.Name, (int)rect.x, (int)rect. y, (int)rect.width, (int)rect.height);
        isPortrait = Screen.height >= Screen.width;
    }

    void Start() {
        if (showOnStart) {
            Show();
        }
        if (!string.IsNullOrEmpty(urlOnStart)) {
            Load(urlOnStart);
        }
        started = true;
        if (referenceRectTransform != null) {
            UpdateFrame();
        }
    }

    void Update() {
        var newIsPortrait = Screen.height >= Screen.width;
        if (isPortrait != newIsPortrait) {
            isPortrait = newIsPortrait;
            if (OnOrientationChanged != null) {
                OnOrientationChanged(this, isPortrait ? ScreenOrientation.Portrait : ScreenOrientation.LandscapeLeft);
            }
            if (referenceRectTransform != null) {
                UpdateFrame();
            }
        }

        if (backButtonEnabled && Input.GetKeyUp(KeyCode.Escape)) {
            UniWebViewLogger.Instance.Info("Received Back button, handling GoBack or close web view.");
            if (CanGoBack) {
                GoBack();
            } else {
                InternalOnShouldClose();
            }
        }
    }

    void OnEnable() {
        if (started) {
            Show();
        }
    }

    void OnDisable() {
        if (started) {
            Hide();
        }
    }

    /// <summary>
    /// Loads a url in current web view.
    /// </summary>
    /// <param name="url">The url to be loaded. This url should start with `http://` or `https://` scheme. You could even load a non-ascii url text if it is valid.</param>
    /// <param name="skipEncoding">
    /// Whether UniWebView should skip encoding the url or not. If set to `false`, UniWebView will try to encode the url parameter before
    /// loading it. Otherwise, your original url string will be used as the url if it is valid. Default is `false`.
    /// </param>
    /// <param name="readAccessURL">
    /// The URL to allow read access to. This parameter is only used when loading from the filesystem in iOS, and passed
    /// to `loadFileURL:allowingReadAccessToURL:` method of WebKit. By default, the parent folder of the `url` parameter will be read accessible.
    /// </param>
    public void Load(string url, bool skipEncoding = false, string readAccessURL = null) {
        UniWebViewInterface.Load(listener.Name, url, skipEncoding, readAccessURL);
    }

    /// <summary>
    /// Loads an HTML string in current web view.
    /// </summary>
    /// <param name="htmlString">The HTML string to use as the contents of the webpage.</param>
    /// <param name="baseUrl">The url to use as the page's base url.</param>
    /// <param name="skipEncoding">
    /// Whether UniWebView should skip encoding the baseUrl or not. If set to `false`, UniWebView will try to encode the baseUrl parameter before
    /// using it. Otherwise, your original url string will be used as the baseUrl if it is valid. Default is `false`.
    /// </param>
    public void LoadHTMLString(string htmlString, string baseUrl, bool skipEncoding = false) {
        UniWebViewInterface.LoadHTMLString(listener.Name, htmlString, baseUrl, skipEncoding);
    }

    /// <summary>
    /// Reloads the current page.
    /// </summary>
    public void Reload() {
        UniWebViewInterface.Reload(listener.Name);
    }

    /// <summary>
    /// Stops loading all resources on the current page.
    /// </summary>
    public void Stop() {
        UniWebViewInterface.Stop(listener.Name);
    }

    /// <summary>
    /// Gets whether there is a back page in the back-forward list that can be navigated to.
    /// </summary>
    public bool CanGoBack {
        get {
            return UniWebViewInterface.CanGoBack(listener.Name);
        }
    }

    /// <summary>
    /// Gets whether there is a forward page in the back-forward list that can be navigated to.
    /// </summary>
    public bool CanGoForward {
        get {
            return UniWebViewInterface.CanGoForward(listener.Name);
        }
    }

    /// <summary>
    /// Navigates to the back item in the back-forward list.
    /// </summary>
    public void GoBack() {
        UniWebViewInterface.GoBack(listener.Name);
    }

    /// <summary>
    /// Navigates to the forward item in the back-forward list.
    /// </summary>
    public void GoForward() {
        UniWebViewInterface.GoForward(listener.Name);
    }

    /// <summary>
    /// Sets whether the link clicking in the web view should open the page in an external browser.
    /// </summary>
    /// <param name="flag">The flag indicates whether a link should be opened externally.</param>
    public void SetOpenLinksInExternalBrowser(bool flag) {
        UniWebViewInterface.SetOpenLinksInExternalBrowser(listener.Name, flag);
    }

    /// <summary>
    /// Sets the web view visible on screen.
    /// 
    /// If you pass `false` and `UniWebViewTransitionEdge.None` to the first two parameters, it means no animation will
    /// be applied when showing. So the `duration` parameter will not be taken into account. Otherwise, when 
    /// either or both `fade` and `edge` set, the showing operation will be animated.
    /// 
    /// Regardless of there is an animation or not, the `completionHandler` will be called if not `null` when the web 
    /// view showing finishes.
    /// </summary>
    /// <param name="fade">Whether show with a fade in animation. Default is `false`.</param>
    /// <param name="edge">The edge from which the web view showing. It simulates a modal effect when showing a web view. Default is `UniWebViewTransitionEdge.None`.</param>
    /// <param name="duration">Duration of the showing animation. Default is `0.4f`.</param>
    /// <param name="completionHandler">Completion handler which will be called when showing finishes. Default is `null`.</param>
    /// <returns>A bool value indicates whether the showing operation started.</returns>
    public bool Show(bool fade = false, UniWebViewTransitionEdge edge = UniWebViewTransitionEdge.None, 
                float duration = 0.4f, Action completionHandler = null) 
    {
        var identifier = Guid.NewGuid().ToString();
        var showStarted = UniWebViewInterface.Show(listener.Name, fade, (int)edge, duration, identifier);
        if (showStarted && completionHandler != null) {
            var hasAnimation = (fade || edge != UniWebViewTransitionEdge.None);
            if (hasAnimation) {
                actions.Add(identifier, completionHandler);
            } else {
                completionHandler();
            }
        }
        if (showStarted && useToolbar) {
            var top = (toolbarPosition == UniWebViewToolbarPosition.Top);
            SetShowToolbar(true, false, top, fullScreen);
        }
        return showStarted;
    }

    /// <summary>
    /// Sets the web view invisible from screen.
    /// 
    /// If you pass `false` and `UniWebViewTransitionEdge.None` to the first two parameters, it means no animation will 
    /// be applied when hiding. So the `duration` parameter will not be taken into account. Otherwise, when either or 
    /// both `fade` and `edge` set, the hiding operation will be animated.
    /// 
    /// Regardless there is an animation or not, the `completionHandler` will be called if not `null` when the web view
    /// hiding finishes.
    /// </summary>
    /// <param name="fade">Whether hide with a fade in animation. Default is `false`.</param>
    /// <param name="edge">The edge from which the web view hiding. It simulates a modal effect when hiding a web view. Default is `UniWebViewTransitionEdge.None`.</param>
    /// <param name="duration">Duration of hiding animation. Default is `0.4f`.</param>
    /// <param name="completionHandler">Completion handler which will be called when hiding finishes. Default is `null`.</param>
    /// <returns>A bool value indicates whether the hiding operation started.</returns>
    public bool Hide(bool fade = false, UniWebViewTransitionEdge edge = UniWebViewTransitionEdge.None,
                float duration = 0.4f, Action completionHandler = null)
    {
        var identifier = Guid.NewGuid().ToString();
        var hideStarted = UniWebViewInterface.Hide(listener.Name, fade, (int)edge, duration, identifier);
        if (hideStarted && completionHandler != null) {
            var hasAnimation = (fade || edge != UniWebViewTransitionEdge.None);
            if (hasAnimation) {
                actions.Add(identifier, completionHandler);
            } else {
                completionHandler();
            }
        }
        if (hideStarted && useToolbar) {
            var top = (toolbarPosition == UniWebViewToolbarPosition.Top);
            SetShowToolbar(false, false, top, fullScreen);
        }
        return hideStarted;
    }

    /// <summary>
    /// Animates the web view from current position and size to another position and size.
    /// </summary>
    /// <param name="frame">The new `Frame` which the web view should be.</param>
    /// <param name="duration">Duration of the animation.</param>
    /// <param name="delay">Delay before the animation begins. Default is `0.0f`, which means the animation will start immediately.</param>
    /// <param name="completionHandler">Completion handler which will be called when animation finishes. Default is `null`.</param>
    /// <returns></returns>
    public bool AnimateTo(Rect frame, float duration, float delay = 0.0f, Action completionHandler = null) {
        var identifier = Guid.NewGuid().ToString();
        var animationStarted = UniWebViewInterface.AnimateTo(listener.Name, 
                    (int)frame.x, (int)frame.y, (int)frame.width, (int)frame.height, duration, delay, identifier);
        if (animationStarted) {
            this.frame = frame;
            if (completionHandler != null) {
                actions.Add(identifier, completionHandler);
            }
        }
        return animationStarted;
    }

    /// <summary>
    /// Adds a JavaScript to current page.
    /// </summary>
    /// <param name="jsString">The JavaScript code to add. It should be a valid JavaScript statement string.</param>
    /// <param name="completionHandler">Called when adding JavaScript operation finishes. Default is `null`.</param>
    public void AddJavaScript(string jsString, Action<UniWebViewNativeResultPayload> completionHandler = null) {
        var identifier = Guid.NewGuid().ToString();
        UniWebViewInterface.AddJavaScript(listener.Name, jsString, identifier);
        if (completionHandler != null) {
            payloadActions.Add(identifier, completionHandler);
        }
    }

    /// <summary>
    /// Evaluates a JavaScript string on current page.
    /// </summary>
    /// <param name="jsString">The JavaScript string to evaluate.</param>
    /// <param name="completionHandler">Called when evaluating JavaScript operation finishes. Default is `null`.</param>
    public void EvaluateJavaScript(string jsString, Action<UniWebViewNativeResultPayload> completionHandler = null) {
        var identifier = Guid.NewGuid().ToString();
        UniWebViewInterface.EvaluateJavaScript(listener.Name, jsString, identifier);
        if (completionHandler != null) {
            payloadActions.Add(identifier, completionHandler);
        }
    }

    /// <summary>
    /// Adds a url scheme to UniWebView message system interpreter.
    /// All following url navigation to this scheme will be sent as a message to UniWebView instead.
    /// </summary>
    /// <param name="scheme">The url scheme to add. It should not contain "://" part. You could even add "http" and/or 
    /// "https" to prevent all resource loading on the page. "uniwebview" is added by default. Nothing will happen if 
    /// you try to add a duplicated scheme.</param>
    public void AddUrlScheme(string scheme) {
        if (scheme == null) {
            UniWebViewLogger.Instance.Critical("The scheme should not be null.");
            return;
        }

        if (scheme.Contains("://")) {
            UniWebViewLogger.Instance.Critical("The scheme should not include invalid characters '://'");
            return;
        }
        UniWebViewInterface.AddUrlScheme(listener.Name, scheme);
    }

    /// <summary>
    /// Removes a url scheme from UniWebView message system interpreter.
    /// </summary>
    /// <param name="scheme">The url scheme to remove. Nothing will happen if the scheme is not in the message system.</param>
    public void RemoveUrlScheme(string scheme) {
        if (scheme == null) {
            UniWebViewLogger.Instance.Critical("The scheme should not be null.");
            return;
        }
        if (scheme.Contains("://")) {
            UniWebViewLogger.Instance.Critical("The scheme should not include invalid characters '://'");
            return;
        }
        UniWebViewInterface.RemoveUrlScheme(listener.Name, scheme);
    }

    /// <summary>
    /// Adds a domain to the SSL checking white list.
    /// If you are trying to access a web site with untrusted or expired certification, 
    /// the web view will prevent its loading. If you could confirm that this site is trusted,
    /// you can add the domain as an SSL exception, so you could visit it.
    /// </summary>
    /// <param name="domain">The domain to add. It should not contain any scheme or path part in url.</param>
    public void AddSslExceptionDomain(string domain) {
        if (domain == null) {
            UniWebViewLogger.Instance.Critical("The domain should not be null.");
            return;
        }
        if (domain.Contains("://")) {
            UniWebViewLogger.Instance.Critical("The domain should not include invalid characters '://'");
            return;
        }
        UniWebViewInterface.AddSslExceptionDomain(listener.Name, domain);
    }

    /// <summary>
    /// Removes a domain from the SSL checking white list.
    /// </summary>
    /// <param name="domain">The domain to remove. It should not contain any scheme or path part in url.</param>
    public void RemoveSslExceptionDomain(string domain) {
        if (domain == null) {
            UniWebViewLogger.Instance.Critical("The domain should not be null.");
            return;
        }
        if (domain.Contains("://")) {
            UniWebViewLogger.Instance.Critical("The domain should not include invalid characters '://'");
            return;
        }
        UniWebViewInterface.RemoveSslExceptionDomain(listener.Name, domain);
    }

    /// <summary>
    /// Sets a customized header field for web view requests.
    /// 
    /// The header field will be used for all subsequence request. 
    /// Pass `null` as value to unset a header field.
    /// 
    /// Some reserved headers like user agent are not be able to override by setting here, 
    /// use the `SetUserAgent` method for them instead.
    /// </summary>
    /// <param name="key">The key of customized header field.</param>
    /// <param name="value">The value of customized header field. `null` if you want to unset the field.</param>
    public void SetHeaderField(string key, string value) {
        if (key == null) {
            UniWebViewLogger.Instance.Critical("Header key should not be null.");
            return;
        }
        UniWebViewInterface.SetHeaderField(listener.Name, key, value);
    }

    /// <summary>
    /// Sets the user agent used in the web view. 
    /// If the string is null or empty, the system default value will be used. 
    /// </summary>
    /// <param name="agent">The new user agent string to use.</param>
    public void SetUserAgent(string agent) {
        UniWebViewInterface.SetUserAgent(listener.Name, agent);
    }

    /// <summary>
    /// Gets the user agent string currently used in web view.
    /// If a customized user agent is not set, the default user agent in current platform will be returned.
    /// </summary>
    /// <returns>The user agent string in use.</returns>
    public string GetUserAgent() {
        return UniWebViewInterface.GetUserAgent(listener.Name);
    }

    /// <summary>
    /// Sets the adjustment behavior which indicates how safe area insets 
    /// are added to the adjusted content inset. It is a wrapper of `contentInsetAdjustmentBehavior` on iOS.
    /// 
    /// It only works on iOS 11 and above. You need to call this method as soon as you create a web view,
    /// before you call any other methods related to web view layout (like `Show` or `SetShowToolbar`).
    /// </summary>
    /// <param name="behavior">The behavior for determining the adjusted content offsets.</param>
    public void SetContentInsetAdjustmentBehavior(
        UniWebViewContentInsetAdjustmentBehavior behavior
    )
    {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.SetContentInsetAdjustmentBehavior(listener.Name, behavior);
        #endif
    }

    /// <summary>
    /// Sets allow auto play for current web view. By default, 
    /// users need to touch the play button to start playing a media resource.
    /// 
    /// By setting this to `true`, you can start the playing automatically through
    /// corresponding media tag attributes.
    /// </summary>
    /// <param name="flag">A flag indicates whether autoplaying of media is allowed or not.</param>
    public static void SetAllowAutoPlay(bool flag) {
        UniWebViewInterface.SetAllowAutoPlay(flag);
    }

    /// <summary>
    /// Sets allow inline play for current web view. By default, on iOS, the video 
    /// can only be played in a new full screen view.
    /// By setting this to `true`, you could play a video inline the page, instead of opening 
    /// a new full screen window.
    /// 
    /// This only works for iOS and macOS Editor. 
    /// On Android, you could play videos inline by default and calling this method does nothing.
    /// </summary>
    /// <param name="flag">A flag indicates whether inline playing of media is allowed or not.</param>
    public static void SetAllowInlinePlay(bool flag) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.SetAllowInlinePlay(flag);
        #endif
    }

    /// <summary>
    /// Sets whether loading a local file is allowed.
    /// 
    /// If set to `false`, any load from a file URL `file://` for `Load` method will be rejected and trigger an 
    /// `OnPageErrorReceived` event. That means you cannot load a web page from any local file. If you are not going to 
    /// load any local files, setting it to `false` helps to reduce the interface of web view and improve the security.
    /// 
    /// By default, it is `true` and the local file URL loading is allowed.
    /// </summary>
    /// <param name="flag">Whether the local file access by web view loading is allowed or not.</param>
    public void SetAllowFileAccess(bool flag) {
        UniWebViewInterface.SetAllowFileAccess(listener.Name, flag);
    }

    /// <summary>
    /// Sets whether file access from file URLs is allowed.
    /// 
    /// By setting with `true`, access to file URLs inside the web view will be enabled and you could access 
    /// sub-resources or make cross origin requests from local HTML files.
    /// 
    /// On iOS, it uses some "hidden" way by setting `allowFileAccessFromFileURLs` in config preferences for WebKit.
    /// So it is possible that it stops working in a future version.
    /// 
    /// On Android, it sets the `WebSettings.setAllowFileAccessFromFileURLs` for the current web view.
    /// </summary>
    /// <param name="flag">Whether the file access inside web view from file URLs is allowed or not.</param>
    public void SetAllowFileAccessFromFileURLs(bool flag) {
        UniWebViewInterface.SetAllowFileAccessFromFileURLs(listener.Name, flag);
    }

    /// <summary>
    /// Sets allow universal access from file URLs. By default, on iOS, the `WKWebView` forbids any load of local files
    /// through AJAX even when opening a local HTML file. It checks the CORS rules and fails at web view level. 
    /// This is useful when you want access these files by setting the `allowUniversalAccessFromFileURLs` key of web view
    /// configuration.
    /// 
    /// On iOS and macOS Editor. It uses some "hidden" way by setting `allowUniversalAccessFromFileURLs` in config 
    /// for WebKit. So it is possible that it stops working in a future version.
    /// 
    /// On Android, it sets the `WebSettings.setAllowUniversalAccessFromFileURLs` and any later-created web views uses
    /// that value.
    /// </summary>
    /// <param name="flag">A flag indicates whether the universal access for files are allowed or not.</param>
    public static void SetAllowUniversalAccessFromFileURLs(bool flag) {
        UniWebViewInterface.SetAllowUniversalAccessFromFileURLs(flag);
    }

    /// <summary>
    /// Sets whether JavaScript should be enabled in current web view. Default is enabled.
    /// </summary>
    /// <param name="enabled">Whether JavaScript should be enabled.</param>
    public static void SetJavaScriptEnabled(bool enabled) {
        UniWebViewInterface.SetJavaScriptEnabled(enabled);
    }

    /// <summary>
    /// Sets whether JavaScript can open windows without user interaction.
    /// 
    /// By setting this to `true`, an automatically JavaScript navigation will be allowed in the web view.
    /// </summary>
    /// <param name="flag">Whether JavaScript could open window automatically.</param>
    public static void SetAllowJavaScriptOpenWindow(bool flag) {
        UniWebViewInterface.SetAllowJavaScriptOpenWindow(flag);
    }

    /// <summary>
    /// Cleans web view cache. This removes cached local data of web view. 
    /// 
    /// If you need to clear all cookies, use `ClearCookies` instead.
    /// </summary>
    public void CleanCache() {
        UniWebViewInterface.CleanCache(listener.Name);
    }

    /// <summary>
    /// Clears all cookies from web view.
    /// 
    /// This will clear cookies from all domains in the web view and previous.
    /// If you only need to remove cookies from a certain domain, use `SetCookie` instead.
    /// </summary>
    public static void ClearCookies() {
        UniWebViewInterface.ClearCookies();
    }

    /// <summary>
    /// Sets a cookie for a certain url.
    /// </summary>
    /// <param name="url">The url to which cookie will be set.</param>
    /// <param name="cookie">The cookie string to set.</param>
    /// <param name="skipEncoding">
    /// Whether UniWebView should skip encoding the url or not. If set to `false`, UniWebView will try to encode the url parameter before
    /// using it. Otherwise, your original url string will be used to set the cookie if it is valid. Default is `false`.
    /// </param>
    public static void SetCookie(string url, string cookie, bool skipEncoding = false) {
        UniWebViewInterface.SetCookie(url, cookie, skipEncoding);
    }

    /// <summary>
    /// Gets the cookie value under a url and key.
    /// </summary>
    /// <param name="url">The url (domain) where the target cookie is.</param>
    /// <param name="key">The key for target cookie value.</param>
    /// <param name="skipEncoding">
    /// Whether UniWebView should skip encoding the url or not. If set to `false`, UniWebView will try to encode the url parameter before
    /// using it. Otherwise, your original url string will be used to get the cookie if it is valid. Default is `false`.
    /// </param>
    /// <returns>Value of the target cookie under url.</returns>
    public static string GetCookie(string url, string key, bool skipEncoding = false) {
        return UniWebViewInterface.GetCookie(url, key, skipEncoding);
    }

    /// <summary>
    /// Removes all the cookies under a url.
    /// </summary>
    /// <param name="url">The url (domain) where the cookies is under.</param>
    /// <param name="skipEncoding">
    /// Whether UniWebView should skip encoding the url or not. If set to `false`, UniWebView will try to encode the url parameter before
    /// using it. Otherwise, your original url string will be used to get the cookie if it is valid. Default is `false`.
    /// </param>
    public static void RemoveCookies(string url, bool skipEncoding = false) {
        UniWebViewInterface.RemoveCookies(url, skipEncoding);
    }
    
    /// <summary>
    /// Removes the certain cookie under a url for the specified key.
    /// </summary>
    /// <param name="url">The url (domain) where the cookies is under.</param>
    /// <param name="key">The key for target cookie.</param>
    /// <param name="skipEncoding">
    /// Whether UniWebView should skip encoding the url or not. If set to `false`, UniWebView will try to encode the url parameter before
    /// using it. Otherwise, your original url string will be used to get the cookie if it is valid. Default is `false`.
    /// </param>
    public static void RemoveCooke(string url, string key, bool skipEncoding = false) {
        UniWebViewInterface.RemoveCookie(url, key, skipEncoding);
    }

    /// <summary>
    /// Clears any saved credentials for HTTP authentication for both Basic and Digest.
    /// 
    /// On both iOS and Android, the user input credentials will be stored permanently across session.
    /// It could prevent your users to input username and password again until they changed. If you need the 
    /// credentials only living in a shorter lifetime, call this method at proper timing.
    /// 
    /// On iOS, it will clear the credentials immediately and completely from both disk and network cache. 
    /// On Android, it only clears from disk database, the authentication might be still cached in the network stack
    /// and will not be removed until next session (app restarting). 
    /// 
    /// The client logout mechanism should be implemented by the Web site designer (such as server sending a HTTP 
    /// 401 for invalidating credentials).
    /// 
    /// </summary>
    /// <param name="host">The host to which the credentials apply. It should not contain any thing like scheme or path part.</param>
    /// <param name="realm">The realm to which the credentials apply.</param>
    public static void ClearHttpAuthUsernamePassword(string host, string realm) {
        UniWebViewInterface.ClearHttpAuthUsernamePassword(host, realm);
    }

    private Color backgroundColor = Color.white;
    /// <summary>
    /// Gets or sets the background color of web view. The default value is `Color.white`.
    /// </summary>
    public Color BackgroundColor {
        get {
            return backgroundColor;
        }
        set {
            backgroundColor = value;
            UniWebViewInterface.SetBackgroundColor(listener.Name, value.r, value.g, value.b, value.a);
        }
    }

    /// <summary>
    /// Gets or sets the alpha value of the whole web view.
    /// 
    /// You can make the game scene behind web view visible to make the web view transparent.
    /// 
    /// Default is `1.0f`, which means totally opaque. Set it to `0.0f` will make the web view totally transparent.
    /// </summary>
    public float Alpha {
        get {
            return UniWebViewInterface.GetWebViewAlpha(listener.Name);
        }
        set {
            UniWebViewInterface.SetWebViewAlpha(listener.Name, value);
         }
    }

    /// <summary>
    /// Sets whether to show a loading indicator while the loading is in progress.
    /// </summary>
    /// <param name="flag">Whether an indicator should show.</param>
    public void SetShowSpinnerWhileLoading(bool flag) {
        UniWebViewInterface.SetShowSpinnerWhileLoading(listener.Name, flag);
    }

    /// <summary>
    /// Sets the text displayed in the loading indicator, if `SetShowSpinnerWhileLoading` is set to `true`.
    /// </summary>
    /// <param name="text">The text to display while loading indicator visible. Default is "Loading..."</param>
    public void SetSpinnerText(string text) {
        UniWebViewInterface.SetSpinnerText(listener.Name, text);
    }

    /// <summary>
    /// Sets whether the horizontal scroll bar should show when the web content beyonds web view bounds.
    /// 
    /// This only works on mobile platforms. It will do nothing on macOS Editor.
    /// </summary>
    /// <param name="enabled">Whether enable the scroll bar or not.</param>
    public void SetHorizontalScrollBarEnabled(bool enabled) {
        UniWebViewInterface.SetHorizontalScrollBarEnabled(listener.Name, enabled);
    }

    /// <summary>
    /// Sets whether the vertical scroll bar should show when the web content beyonds web view bounds.
    /// 
    /// This only works on mobile platforms. It will do nothing on macOS Editor.
    /// </summary>
    /// <param name="enabled">Whether enable the scroll bar or not.</param>
    public void SetVerticalScrollBarEnabled(bool enabled) {
        UniWebViewInterface.SetVerticalScrollBarEnabled(listener.Name, enabled);
    }

    /// <summary>
    /// Sets whether the web view should show with a bounces effect when scrolling to page edge.
    /// 
    /// This only works on mobile platforms. It will do nothing on macOS Editor.
    /// </summary>
    /// <param name="enabled">Whether the bounces effect should be applied or not.</param>
    public void SetBouncesEnabled(bool enabled) {
        UniWebViewInterface.SetBouncesEnabled(listener.Name, enabled);
    }

    /// <summary>
    /// Sets whether the web view supports zoom gesture to change content size. 
    /// Default is `false`, which means the zoom gesture is not supported.
    /// </summary>
    /// <param name="enabled">Whether the zoom gesture is allowed or not.</param>
    public void SetZoomEnabled(bool enabled) {
        UniWebViewInterface.SetZoomEnabled(listener.Name, enabled);
    }

    /// <summary>
    /// Adds a trusted domain to white list and allow permission requests from the domain.
    /// 
    /// You only need this on Android devices with system before 6.0 when a site needs the location or camera 
    /// permission. It will allow the permission gets approved so you could access the corresponding devices.
    /// From Android 6.0, the permission requests method is changed and this is not needed anymore.
    /// </summary>
    /// <param name="domain">The domain to add to the white list.</param>
    public void AddPermissionTrustDomain(string domain) {
        #if UNITY_ANDROID && !UNITY_EDITOR
        UniWebViewInterface.AddPermissionTrustDomain(listener.Name, domain);
        #endif
    }

    /// <summary>
    /// Removes a trusted domain from white list.
    /// </summary>
    /// <param name="domain">The domain to remove from white list.</param>
    public void RemovePermissionTrustDomain(string domain) {
        #if UNITY_ANDROID && !UNITY_EDITOR
        UniWebViewInterface.RemovePermissionTrustDomain(listener.Name, domain);
        #endif
    }

    /// <summary>
    /// Sets whether the device back button should be enabled to execute "go back" or "closing" operation.
    /// 
    /// On Android, the device back button in navigation bar will navigate users to a back page. If there is 
    /// no any back page avaliable, the back button clicking will try to raise a `OnShouldClose` event and try 
    /// to close the web view if `true` is return from the event. If the `OnShouldClose` is not listened, 
    /// the web view will be closed and the UniWebView component will be destroyed to release using resource.
    /// 
    /// Listen to `OnKeyCodeReceived` if you need to disable the back button, but still want to get the back 
    /// button key pressing event.
    /// 
    /// Default is enabled.
    /// </summary>
    /// <param name="enabled">Whether the back button should perform go back or closing operation to web view.</param>
    public void SetBackButtonEnabled(bool enabled) {
        this.backButtonEnabled = enabled;
    }

    /// <summary>
    /// Sets whether the web view should enable support for the "viewport" HTML meta tag or should use a wide viewport.
    /// </summary>
    /// <param name="flag">Whether to enable support for the viewport meta tag.</param>
    public void SetUseWideViewPort(bool flag) {
        #if UNITY_ANDROID && !UNITY_EDITOR
        UniWebViewInterface.SetUseWideViewPort(listener.Name, flag);
        #endif
    } 

    /// <summary>
    /// Sets whether the web view loads pages in overview mode, that is, zooms out the content to fit on screen by width. 
    /// 
    /// This method is only for Android. Default is disabled.
    /// </summary>
    /// <param name="flag"></param>
    public void SetLoadWithOverviewMode(bool flag) {
        #if UNITY_ANDROID && !UNITY_EDITOR
        UniWebViewInterface.SetLoadWithOverviewMode(listener.Name, flag);
        #endif
    }

    /// <summary>
    /// Sets whether to show a toolbar which contains navigation buttons and Done button.
    /// 
    /// You could choose to show or hide the tool bar. By configuring the `animated` and `onTop` 
    /// parameters, you can control the animating and position of the toolbar. If the toolbar is 
    /// overlapping with some part of your web view, pass `adjustInset` with `true` to have the 
    /// web view relocating itself to avoid the overlap.
    /// 
    /// This method is only for iOS. The toolbar is hidden by default.
    /// </summary>
    /// <param name="show">Whether the toolbar should show or hide.</param>
    /// <param name="animated">Whether the toolbar state changing should be with animation. Default is `false`.</param>
    /// <param name="onTop">Whether the toolbar should snap to top of screen or to bottom of screen. 
    /// Default is `true`</param>
    /// <param name="adjustInset">Whether the toolbar transition should also adjust web view position and size
    ///  if overlapped. Default is `false`</param>
    public void SetShowToolbar(bool show, bool animated = false, bool onTop = true, bool adjustInset = false) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.SetShowToolbar(listener.Name, show, animated, onTop, adjustInset);
        #endif
    }

    /// <summary>
    /// Sets the done button text in toolbar.
    /// 
    /// By default, UniWebView will show a "Done" button at right size in the 
    /// toolbar. You could change its title by passing a text.
    /// 
    /// This method is only for iOS, since there is no toolbar on Android.
    /// </summary>
    /// <param name="text">The text needed to be set as done button title.</param>
    public void SetToolbarDoneButtonText(string text) {
        #if UNITY_IOS && !UNITY_EDITOR
        UniWebViewInterface.SetToolbarDoneButtonText(listener.Name, text);
        #endif
    }

    /// <summary>
    /// Sets the go back button text in toolbar.
    /// 
    /// By default, UniWebView will show a back arrow at the left side in the 
    /// toolbar. You could change its text.
    /// 
    /// This method is only for iOS and macOS Editor, since there is no toolbar on Android.
    /// </summary>
    /// <param name="text">The text needed to be set as go back button.</param>
    public void SetToolbarGoBackButtonText(string text) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.SetToolbarGoBackButtonText(listener.Name, text);
        #endif
    }

    /// <summary>
    /// Sets the go forward button text in toolbar.
    /// 
    /// By default, UniWebView will show a forward arrow at the left side in the 
    /// toolbar. You could change its text.
    /// 
    /// This method is only for iOS and macOS Editor, since there is no toolbar on Android.
    /// </summary>
    /// <param name="text">The text needed to be set as go forward button.</param>
    public void SetToolbarGoForwardButtonText(string text) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.SetToolbarGoForwardButtonText(listener.Name, text);
        #endif
    }

    /// <summary>
    /// Sets the background tint color for the toolbar.
    /// 
    /// By default, UniWebView uses a default half-transparent iOS standard background for toolbar.
    /// You can change it by setting a new opaque color.
    /// 
    /// This method is only for iOS, since there is no toolbar on Android.
    /// </summary>
    /// <param name="color">The color should be used for the background tint of the toolbar.</param>
    public void SetToolbarTintColor(Color color) {
        #if UNITY_IOS && !UNITY_EDITOR
        UniWebViewInterface.SetToolbarTintColor(listener.Name, color.r, color.g, color.b);
        #endif
    }

    /// <summary>
    /// Sets the button text color for the toolbar.
    /// 
    /// By default, UniWebView uses the default text color on iOS, which is blue for most cases.
    /// You can change it by setting a new opaque color.
    /// 
    /// This method is only for iOS, since there is no toolbar on Android.
    /// </summary>
    /// <param name="color">The color should be used for the button text of the toolbar.</param>
    public void SetToolbarTextColor(Color color) {
        #if UNITY_IOS && !UNITY_EDITOR
        UniWebViewInterface.SetToolbarTextColor(listener.Name, color.r, color.g, color.b);
        #endif
    }

    /// <summary>
    /// Sets the visibility of navigation buttons, such as "Go Back" and "Go Forward", on toolbar.
    /// 
    /// By default, UniWebView will show the "Go Back" and "Go Forward" navigation buttons on the toolbar.
    /// Users can use these buttons to perform go back or go forward action just like in a browser. If the navigation
    /// model is not for your case, call this method with `false` as `show` parameter to hide them.
    /// 
    /// This method is only for iOS, since there is no toolbar on Android.
    /// </summary>
    /// <param name="show">Whether the navigation buttons on the toolbar should show or hide.</param>
    public void SetShowToolbarNavigationButtons(bool show) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.SetShowToolbarNavigationButtons(listener.Name, show);
        #endif
    }

    /// <summary>
    /// Sets whether the web view can receive user interaction or not.
    /// 
    /// By setting this to `false`, the web view will not accept any user touch event so your users cannot tap links or
    /// scroll the page.
    /// 
    /// </summary>
    /// <param name="enabled">Whether the user interaction should be enabled or not.</param>
    public void SetUserInteractionEnabled(bool enabled) {
        UniWebViewInterface.SetUserInteractionEnabled(listener.Name, enabled);
    }

    /// <summary>
    /// Enables debugging of web contents. You could inspect of the content of a 
    /// web view by using a browser development tool of Chrome for Android or Safari for macOS.
    /// 
    /// This method is only for Android and macOS Editor. On iOS, you do not need additional step. 
    /// You could open Safari's developer tools to debug a web view on iOS.
    /// </summary>
    /// <param name="enabled">Whether the content debugging should be enabled.</param>
    public static void SetWebContentsDebuggingEnabled(bool enabled) {
        UniWebViewInterface.SetWebContentsDebuggingEnabled(enabled);
    }

    /// <summary>
    /// Enables user resizing for web view window. By default, you can only set the window size
    /// by setting its frame on mac Editor. By enabling user resizing, you would be able to resize 
    /// the window by dragging its border as a normal macOS window.
    /// 
    /// This method only works for macOS for debugging purpose. It does nothing on iOS and Android.
    /// </summary>
    /// <param name="enabled">Whether the window could be able to be resized by cursor.</param>
    public void SetWindowUserResizeEnabled(bool enabled) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.SetWindowUserResizeEnabled(listener.Name, enabled);
        #endif
    }

    /// <summary>
    /// Gets the HTML content from current page by accessing its outerHTML with JavaScript.
    /// </summary>
    /// <param name="handler">Called after the JavaScript executed. The parameter string is the content read 
    /// from page.</param>
    public void GetHTMLContent(Action<string> handler) {
        EvaluateJavaScript("document.documentElement.outerHTML", payload => {
            if (handler != null) {
                handler(payload.data);
            }
        });
    }

    /// <summary>
    /// Sets whether horizontal swipe gestures should trigger back-forward list navigation.
    /// 
    /// By setting with `true`, users can swipe from screen edge to perform a back or forward navigation.
    /// This method only works on iOS and macOS Editor. Default is `false`. 
    /// 
    /// On Android, the screen navigation gestures are simulating the traditional back button and it is enabled by 
    /// default. To disable gesture navigation on Android, you have to also disable the device back button. See 
    /// `SetBackButtonEnabled` for that purpose.
    /// </summary>
    /// <param name="flag">
    /// The value indicates whether a swipe gestures driven navigation should be allowed. Default is `false`.
    /// </param>
    public void SetAllowBackForwardNavigationGestures(bool flag) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.SetAllowBackForwardNavigationGestures(listener.Name, flag);
        #endif
    }

    /// <summary>
    /// Sets whether a prompt alert should be displayed for collection username and password when the web view receives an
    /// HTTP authentication challenge (HTTP Basic or HTTP Digest) from server.
    /// 
    /// By setting with `false`, no prompt will be shown and the user cannot login with input credentials. In this case,
    /// you can only access this page by providing username and password through the URL like: "http://username:password@example.com".
    /// If the username and password does not match, normally an error with 401 as status code would be returned (this behavior depends
    /// on the server implementation). If set with `true`, a prompt will be shown when there is no credentials provided or it is not
    /// correct in the URL.
    /// 
    /// Default is `true`.
    /// </summary>
    /// <param name="flag">Whether a prompt alert should be shown for HTTP authentication challenge or not.</param>
    public void SetAllowHTTPAuthPopUpWindow(bool flag) {
        UniWebViewInterface.SetAllowHTTPAuthPopUpWindow(listener.Name, flag);
    }

    /// <summary>
    /// Sets whether a callout (context) menu should be displayed when user long tapping on certain web view content.
    /// 
    /// When enabled, when user long presses an image or link in the web page, a context menu would be show up to ask 
    /// user's action. On iOS, it is a action sheet to ask whether opening the target link or saving the image. On 
    /// Android it is a pop up dialog to ask whether saving the image to local disk. On iOS, the preview page triggered 
    /// by force touch on iOS is also considered as a callout menu.
    /// 
    /// Default is `true`, means that the callout menu will be displayed. Call this method with `false` to disable 
    /// it on the web view.
    /// </summary>
    /// <param name="enabled">
    /// Whether a callout menu should be displayed when user long pressing or force touching a certain web page element.
    /// </param>
    public void SetCalloutEnabled(bool enabled) {
        UniWebViewInterface.SetCalloutEnabled(listener.Name, enabled);
    }

    /// <summary>
    /// Sets whether the web view should support a pop up web view triggered by user in a new tab.
    /// 
    /// In a general web browser (such as Google Chrome or Safari), a URL with `target="_blank"` attribute is intended 
    /// to be opened in a new tab. However, in the context of web view, there is no way to handle new tabs without 
    /// proper configurations. Due to that, by default UniWebView will ignore the `target="_blank"` and try to open 
    /// the page in the same web view if that kind of link is pressed.
    /// 
    /// It works for most cases, but if this is a problem to your app logic, you can change this behavior by calling 
    /// this method with `true`. It enables the "opening in new tab" behavior in a limited way, by adding the new tab 
    /// web view above to the current web view, with the same size and position. When the opened new tab is closed, 
    /// it will be removed from the view hierarchy automatically.
    /// 
    /// </summary>
    /// <param name="enabled">
    /// Whether to support multiple windows. If `true`, the `target="_blank"` link will be opened in a new web view.
    /// Default is `false`.
    /// </param>
    public void SetSupportMultipleWindows(bool enabled) {
        UniWebViewInterface.SetSupportMultipleWindows(listener.Name, enabled);
    }

    /// <summary>
    /// Sets the default font size used in the web view.
    /// 
    /// On Android, the web view font size can be affected by the system font scale setting. Use this method to set the 
    /// font size in a more reasonable way, by giving the web view another default font size with the system font scale 
    /// considered. It can removes or reduces the effect of system font scale when displaying the web content.
    /// 
    /// This method only works on Android. On iOS, this method does nothing since the web view will respect the font 
    /// size setting in your CSS styles.
    /// </summary>
    /// <param name="size">The target default font size set to the web view.</param>
    public void SetDefaultFontSize(int size) {
        #if UNITY_ANDROID && !UNITY_EDITOR
        UniWebViewInterface.SetDefaultFontSize(listener.Name, size);
        #endif
    }

    /// <summary>
    /// Sets the text zoom used in the web view.
    /// 
    /// On Android, this method call `WebSettings.setTextZoom` to the the text zoom used in the web view.
    /// 
    /// This method only works on Android.
    /// </summary>
    /// <param name="textZoom">The text zoom in percent.</param>
    public void SetTextZoom(int textZoom) {
        #if UNITY_ANDROID && !UNITY_EDITOR
        UniWebViewInterface.SetTextZoom(listener.Name, textZoom);
        #endif
    }

    /// <summary>
    /// Sets whether the drag interaction should be enabled on iOS.
    /// 
    /// From iOS 11, the iPad web view supports the drag interaction when user long presses an image, link or text.
    /// Setting this to `false` would disable the drag feather on the web view.
    /// 
    /// This method only works on iOS. It does nothing on Android or macOS editor. Default is `true`, which means
    /// drag interaction on iPad is enabled.
    /// </summary>
    /// <param name="enabled">
    /// Whether the drag interaction should be enabled.
    /// </param>
    public void SetDragInteractionEnabled(bool enabled) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.SetDragInteractionEnabled(listener.Name, enabled);
        #endif
    }

    /// <summary>
    /// Prints current page.
    /// 
    /// By calling this method, a native print preview panel will be brought up on iOS and Android. 
    /// This method does nothing on macOS editor.
    /// On iOS and Android, the web view does not support JavaScript (window.print()), 
    /// you can only initialize a print job from Unity by this method.
    /// </summary>
    public void Print() {
        UniWebViewInterface.Print(listener.Name);
    }

    /// <summary>
    /// Capture the content of web view and store it to the cache path on disk with the given file name.
    /// 
    /// When the capturing finishes, `OnCaptureSnapshotFinished` event will be raised, with an error code to indicate
    /// whether the operation succeeded and an accessible disk path of the image. 
    /// 
    /// The captured image will be stored as a PNG file under the `fileName` in app's cache folder. If a file with the 
    /// same file name already exists, it will be overridden by the new captured image.
    /// </summary>
    /// <param name="fileName">
    /// The file name to which the captured image is stored to, for example "screenshot.png". If empty, UniWebView will
    /// pick a random UUID with "png" file extension as the file name.
    /// </param>
    public void CaptureSnapshot(string fileName) {
        UniWebViewInterface.CaptureSnapshot(listener.Name, fileName);
    }

    /// <summary>
    /// Scrolls the web view to a certain point.
    /// 
    /// Use 0 for both `x` and `y` value to scroll the web view to its origin.
    /// In a normal vertical web page, it is equivalent as scrolling to top.
    /// 
    /// You can use the `animated` parameter to control whether scrolling the page with or without animation.
    /// This parameter only works on iOS and Android. On macOS editor, the scrolling always happens without animation.
    /// </summary>
    /// <param name="x">X value of the target scrolling point.</param>
    /// <param name="y">Y value of the target scrolling point.</param>
    /// <param name="animated">If `true`, the scrolling happens with animation. Otherwise, it happens without
    ///  animation and the content is set directly.
    /// </param>
    public void ScrollTo(int x, int y, bool animated) {
        UniWebViewInterface.ScrollTo(listener.Name, x, y, animated);
    }

    /// <summary>
    /// Adds the URL to download inspecting list.
    /// 
    /// If a response is received in main frame and its URL is already in the inspecting list, a download task will be 
    /// triggered. Check "Download Files" guide for more.
    /// 
    /// This method only works on iOS and macOS Editor.
    /// </summary>
    /// <param name="urlString">The inspected URL.</param>
    public void AddDownloadURL(string urlString) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.AddDownloadURL(listener.Name, urlString);
        #endif
    }

    /// <summary>
    /// Removes the URL from download inspecting list.
    /// 
    /// If a response is received in main frame and its URL is already in the inspecting list, a download task will be 
    /// triggered. Check "Download Files" guide for more.
    /// 
    /// This method only works on iOS and macOS Editor.
    /// </summary>
    /// <param name="urlString">The inspected URL.</param>
    public void RemoveDownloadURL(string urlString) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.RemoveDownloadURL(listener.Name, urlString);
        #endif
    }

    /// <summary>
    /// Adds the MIME type to download inspecting list.
    /// 
    /// If a response is received in main frame and its MIME type is already in the inspecting list, a 
    /// download task will be triggered. Check "Download Files" guide for more.
    /// 
    /// This method only works on iOS and macOS Editor.
    /// </summary>
    /// <param name="MIMEType">The inspected MIME type of the response.</param>
    public void AddDownloadMIMEType(string MIMEType) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.AddDownloadMIMEType(listener.Name, MIMEType);
        #endif
    }

    /// <summary>
    /// Removes the MIME type from download inspecting list.
    /// 
    /// If a response is received in main frame and its MIME type is already in the inspecting list, a 
    /// download task will be triggered. Check "Download Files" guide for more.
    /// 
    /// This method only works on iOS and macOS Editor.
    /// </summary>
    /// <param name="MIMEType">The inspected MIME type of the response.</param>
    public void RemoveDownloadMIMETypes(string MIMEType) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.RemoveDownloadMIMETypes(listener.Name, MIMEType);
        #endif
    }

    /// <summary>
    /// Sets whether allowing users to choose the way to handle the downloaded file. Default is `true`.
    /// 
    /// On iOS, the downloaded file will be stored in a temporary folder. Setting this to `true` will show a system 
    /// default share sheet and give the user a chance to send and store the file to another location (such as the 
    /// File app or iCloud).
    /// 
    /// On macOS Editor, setting this to `true` will allow UniWebView to open the file in Finder.
    /// 
    /// This method does not have any effect on Android. On Android, the file is downloaded to the Download folder.
    /// 
    /// </summary>
    /// <param name="allowed"></param>
    public void SetAllowUserChooseActionAfterDownloading(bool allowed) {
        #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        UniWebViewInterface.SetAllowUserChooseActionAfterDownloading(listener.Name, allowed);
        #endif
    }

    /// <summary>
    /// Sets whether the `OnFileDownloadStarted` and `OnFileDownloadFinished` events should be raised even for an image
    /// saving action triggered by the callout (context) menu on Android.
    /// 
    /// By default, the image saving goes through a different route and it does not trigger the `OnFileDownloadStarted` 
    /// and `OnFileDownloadFinished` events like other normal download tasks. Setting this with enabled with `true` if
    /// you also need to get notified when user long-presses on the image and taps "Save Image" button. By default, the
    /// image will be saved to the Downloads directory and you can get the path from the parameter 
    /// of `OnFileDownloadFinished` event.
    /// 
    /// This only works on Android. On iOS, there is no way to get a callback or any event from the "Add to Photos"
    /// button in the callout menu.
    /// </summary>
    /// <param name="enabled">Whether the context menu image saving action triggers the download related events.</param>
    public void SetDownloadEventForContextMenuEnabled(bool enabled) {
        #if UNITY_ANDROID && !UNITY_EDITOR
        UniWebViewInterface.SetDownloadEventForContextMenuEnabled(listener.Name, enabled);
        #endif
    }

    void OnDestroy() {
        UniWebViewNativeListener.RemoveListener(listener.Name);
        UniWebViewInterface.Destroy(listener.Name);
        Destroy(listener.gameObject);
    }

    /* //////////////////////////////////////////////////////
    // Internal Listener Interface
    ////////////////////////////////////////////////////// */
    internal void InternalOnShowTransitionFinished(string identifier) {
        Action action;
        if (actions.TryGetValue(identifier, out action)) {
            action();
            actions.Remove(identifier);
        }
    }

    internal void InternalOnHideTransitionFinished(string identifier) {
        Action action;
        if (actions.TryGetValue(identifier, out action)) {
            action();
            actions.Remove(identifier);
        }
    }

    internal void InternalOnAnimateToFinished(string identifier) {
        Action action;
        if (actions.TryGetValue(identifier, out action)) {
            action();
            actions.Remove(identifier);
        }
    }

    internal void InternalOnAddJavaScriptFinished(UniWebViewNativeResultPayload payload) {
        Action<UniWebViewNativeResultPayload> action;
        var identifier = payload.identifier;
        if (payloadActions.TryGetValue(identifier, out action)) {
            action(payload);
            payloadActions.Remove(identifier);
        }
    }

    internal void InternalOnEvalJavaScriptFinished(UniWebViewNativeResultPayload payload) {
        Action<UniWebViewNativeResultPayload> action;
        var identifier = payload.identifier;
        if (payloadActions.TryGetValue(identifier, out action)) {
            action(payload);
            payloadActions.Remove(identifier);
        }
    }

    internal void InternalOnPageFinished(UniWebViewNativeResultPayload payload) {
        if (OnPageFinished != null) {
            int code = -1;
            if (int.TryParse(payload.resultCode, out code)) {
                OnPageFinished(this, code, payload.data);
            } else {
                UniWebViewLogger.Instance.Critical("Invalid status code received: " + payload.resultCode);
            }
        }
    }

    internal void InternalOnPageStarted(string url) {
        if (OnPageStarted != null) {
            OnPageStarted(this, url);
        }
    }

    internal void InternalOnPageErrorReceived(UniWebViewNativeResultPayload payload) {
        if (OnPageErrorReceived != null) {
            int code = -1;
            if (int.TryParse(payload.resultCode, out code)) {
                OnPageErrorReceived(this, code, payload.data);
            } else {
                UniWebViewLogger.Instance.Critical("Invalid error code received: " + payload.resultCode);
            }
        }
    }

    internal void InternalOnPageProgressChanged(float progress) {
        if (OnPageProgressChanged != null) {
            OnPageProgressChanged(this, progress);
        }
    }

    internal void InternalOnMessageReceived(string result) {
         if (OnMessageReceived != null) {
             var message = new UniWebViewMessage(result);
             OnMessageReceived(this, message);
         }
    }

    internal void InternalOnShouldClose() {
        if (OnShouldClose != null) {
            var shouldClose = OnShouldClose(this);
            if (shouldClose) {
                Destroy(this);
            }
        } else {
            Destroy(this);
        }
    }

    internal void InternalOnWebContentProcessDidTerminate() {
        if (OnWebContentProcessTerminated != null) {
            OnWebContentProcessTerminated(this);
        }
    }

    internal void InternalOnMultipleWindowOpened(string multiWindowId) {
        if (OnMultipleWindowOpened != null) {
            OnMultipleWindowOpened(this, multiWindowId);
        }
    }

    internal void InternalOnMultipleWindowClosed(string multiWindowId) {
        if (OnMultipleWindowClosed != null) {
            OnMultipleWindowClosed(this, multiWindowId);
        }
    }

    internal void InternalOnFileDownloadStarted(UniWebViewNativeResultPayload payload) {
        if (OnFileDownloadStarted != null) {
            OnFileDownloadStarted(this, payload.identifier, payload.data);
        }
    }

    internal void InternalOnFileDownloadFinished(UniWebViewNativeResultPayload payload) {
        if (OnFileDownloadFinished != null) {
            int errorCode = int.TryParse(payload.resultCode, out errorCode) ? errorCode : -1;
            OnFileDownloadFinished(this, errorCode, payload.identifier, payload.data);
        }
    }

    internal void InternalOnCaptureSnapshotFinished(UniWebViewNativeResultPayload payload) {
        if (OnCaptureSnapshotFinished != null) {
            int errorCode = int.TryParse(payload.resultCode, out errorCode) ? errorCode : -1;
            OnCaptureSnapshotFinished(this, errorCode,  payload.data);
        }
    }

    [Obsolete("SetImmersiveModeEnabled is deprecated. Now UniWebView always respect navigation bar/status bar settings from Unity.", false)]
    /// <summary>
    /// Sets whether the web view should behave in immersive mode, that is, 
    /// hides the status bar and navigation bar with a sticky style.
    /// 
    /// This method is only for Android. Default is enabled.
    /// </summary>
    /// <param name="enabled"></param>
    public void SetImmersiveModeEnabled(bool enabled) {
        Debug.LogError(
            "SetImmersiveModeEnabled is removed in UniWebView 4." + 
            "Now UniWebView always respect navigation bar/status bar settings from Unity."
        );
    } 

    [Obsolete("KeyCodeReceivedDelegate is deprecated. Now UniWebView never intercepts device key code events. Check `Input.GetKeyUp` instead.", false)]
    /// <summary>
    /// Delegate for code keycode received event.
    /// </summary>
    /// <param name="webView">The web view component which raises this event.</param>
    /// <param name="keyCode">The key code of pressed key. See [Android API for keycode](https://developer.android.com/reference/android/view/KeyEvent.html#KEYCODE_0) to know the possible values.</param>
    public delegate void KeyCodeReceivedDelegate(UniWebView webView, int keyCode);

    [Obsolete("OnKeyCodeReceived is deprecated and never called. Now UniWebView never intercepts device key code events. Check `Input.GetKeyUp` instead.", false)]
    /// <summary>
    /// Raised when a key (like back button or volume up) on the device is pressed.
    /// 
    /// This event only raised on Android. It is useful when you disabled the back button but still need to 
    /// get the back button event. On iOS, user's key action is not avaliable and this event will never be 
    /// raised.
    /// </summary>
    #pragma warning disable CS0067
    public event KeyCodeReceivedDelegate OnKeyCodeReceived;

}