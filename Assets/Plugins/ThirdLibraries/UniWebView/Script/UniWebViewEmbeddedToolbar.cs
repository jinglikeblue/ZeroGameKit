using UnityEngine;

/// <summary>
/// Represents the embedded toolbar in a web view.
///
/// You do not create an instance of this class directly. Instead, use the `EmbeddedWebView` property in `UniWebView` to
/// get the current embedded toolbar in the web view and interact with it.
///
/// The embedded toolbar of a web view expands its width to match the web view's frame width. It is displayed at either
/// top or bottom of the web view, based on the setting received through `SetPosition`. By default, the toolbar contains
/// a main title, a back button, a forward button and a done button to close the web view. You can use methods in this
/// class to customize the toolbar to match your app's style. 
/// </summary>
public class UniWebViewEmbeddedToolbar {

    private readonly UniWebViewNativeListener listener;
    
    internal UniWebViewEmbeddedToolbar(UniWebViewNativeListener listener) {
        this.listener = listener;
    }
    
    /// <summary>
    /// Sets the position of the embedded toolbar. You can put the toolbar at either top or bottom of your web view.
    /// The default position is `Top`.
    /// </summary>
    /// <param name="position">The desired position of the toolbar.</param>
    public void SetPosition(UniWebViewToolbarPosition position) {
        UniWebViewInterface.SetEmbeddedToolbarOnTop(listener.Name, position == UniWebViewToolbarPosition.Top);
    }

    /// <summary>
    /// Shows the toolbar.
    /// </summary>
    public void Show() {
        UniWebViewInterface.SetShowEmbeddedToolbar(listener.Name, true);
    }

    /// <summary>
    /// Hides the toolbar.
    /// </summary>
    public void Hide() {
        UniWebViewInterface.SetShowEmbeddedToolbar(listener.Name, false);
    }

    /// <summary>
    /// Sets the text of the done button. The default text is "Done".
    /// </summary>
    /// <param name="text">The desired text to display as the done button.</param>
    public void SetDoneButtonText(string text) {
        UniWebViewInterface.SetEmbeddedToolbarDoneButtonText(listener.Name, text);
    }

    /// <summary>
    /// Sets the text of the back button. The default text is "❮" ("\u276E").
    /// </summary>
    /// <param name="text">The desired text to display as the back button.</param>
    public void SetGoBackButtonText(string text) {
        UniWebViewInterface.SetEmbeddedToolbarGoBackButtonText(listener.Name, text);
    }

    /// <summary>
    /// Sets the text of the forward button. The default text is "❯" ("\u276F").
    /// </summary>
    /// <param name="text">The desired text to display as the forward button.</param>
    public void SetGoForwardButtonText(string text) {
        UniWebViewInterface.SetEmbeddedToolbarGoForwardButtonText(listener.Name, text);
    }
    
    /// <summary>
    /// Sets the text of toolbar title. The default is empty. The space is limited, setting a long text as title might
    /// not fit in the space. 
    /// </summary>
    /// <param name="text">The desired text to display as the title in the toolbar.</param>
    public void SetTitleText(string text) {
        UniWebViewInterface.SetEmbeddedToolbarTitleText(listener.Name, text);
    }
    
    /// <summary>
    /// Sets the background color of the toolbar.
    /// </summary>
    /// <param name="color">The desired color of toolbar's background.</param>
    public void SetBackgroundColor(Color color) {
        UniWebViewInterface.SetEmbeddedToolbarBackgroundColor(listener.Name, color);
    }

    /// <summary>
    /// Sets the buttons color of the toolbar. This color affects the back, forward and done button.
    /// </summary>
    /// <param name="color">The desired color of toolbar's buttons.</param>
    public void SetButtonTextColor(Color color) {
        UniWebViewInterface.SetEmbeddedToolbarButtonTextColor(listener.Name, color);
    }
    
    /// <summary>
    /// Sets the text color of the toolbar title. 
    /// </summary>
    /// <param name="color">The desired color of the toolbar's title.</param>
    public void SetTitleTextColor(Color color) {
        UniWebViewInterface.SetEmbeddedToolbarTitleTextColor(listener.Name, color);
    }

    /// <summary>
    /// Hides the navigation buttons on the toolbar. 
    /// 
    /// When called, the back button and forward button will not be shown.
    /// By default, the navigation buttons are shown.
    /// </summary>
    public void HideNavigationButtons() {
        UniWebViewInterface.SetEmeddedToolbarNavigationButtonsShow(listener.Name, false);
    }

    /// <summary>
    /// Shows the navigation buttons on the toolbar. 
    /// 
    /// When called, the back button and forward button will be shown.
    /// By default, the navigation buttons are shown.
    /// </summary>
    public void ShowNavigationButtons() {
        UniWebViewInterface.SetEmeddedToolbarNavigationButtonsShow(listener.Name, true);
    }
}