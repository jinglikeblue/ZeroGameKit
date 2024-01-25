using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Button功能呢扩充类
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonExtend : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    /// <summary>
    /// 关联的Image
    /// </summary>
    public Image[] tingAssociatedImages;

    Button _btn;

    bool _isSelected = false;
    bool _isPointerEnter = false;
    bool _isPointerDown = false;

    void Start()
    {
        _btn = GetComponent<Button>();            
    }

    /// <summary>
    /// 切换图片的颜色
    /// </summary>
    /// <param name="color"></param>
    void SwitchImagesColor(Color color)
    {
        if(null == tingAssociatedImages)
        {
            return;
        }

        foreach(var image in tingAssociatedImages)
        {
            if (null != image)
            {
                image.color = color;
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        _isSelected = true;
        SwitchImagesColor(_btn.colors.selectedColor);
        //Debug.Log($"OnSelect");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;
        SwitchImagesColor(_btn.colors.normalColor);
        //Debug.Log($"OnDeselect");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
        SwitchImagesColor(_btn.colors.pressedColor);
        //Debug.Log($"OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;
        if (_isPointerEnter)
        {
            SwitchImagesColor(_btn.colors.highlightedColor);
        }
        else
        {
            SwitchImagesColor(_btn.colors.normalColor);
        }        
        //Debug.Log($"OnPointerUp");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerEnter = true;
        SwitchImagesColor(_btn.colors.highlightedColor);
        //Debug.Log($"OnPointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerEnter = false;
        if (false == _isPointerDown)
        {
            SwitchImagesColor(_btn.colors.normalColor);
        }
        //Debug.Log($"OnPointerExit");
    }
}
