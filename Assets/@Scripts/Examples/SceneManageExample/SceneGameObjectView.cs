using behaviac;
using Zero;
using ZeroHot;

namespace Example
{
    public class SceneGameObjectView : AView
    {
        protected override void OnInit(object data)
        {
            base.OnInit(data);
            Debug.Log(LogColor.Zero2($"[SceneGameObjectView] {gameObject.name}"));
        }
    }
}