using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zero;

public class ScreenSwitch : MonoBehaviour
{
    public Text text;

    bool isPortrait = false;

    private void Awake()
    {
        if (ScreenUtility.IsLandscape)
        {
            text.text = "横屏模式";
        }
        else if (ScreenUtility.IsPortrait)
        {
            text.text = "竖屏模式";
        }
        else
        {
            text.text = "未知模式";
        }


        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }

    public void Switch()
    {
        isPortrait = !isPortrait;
        if (isPortrait)
        {
            ScreenUtility.SwitchToPortrait();            
        }
        else
        {
            ScreenUtility.SwitchToLandscape();
            
        }

        if (ScreenUtility.IsLandscape)
        {
            text.text = "横屏模式";            
        }
        else if (ScreenUtility.IsPortrait)
        {
            text.text = "竖屏模式";
        }
    }
}
