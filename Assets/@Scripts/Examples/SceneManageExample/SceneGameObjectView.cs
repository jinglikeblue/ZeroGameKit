using behaviac;
using ZeroHot;

namespace Example
{
    public class SceneGameObjectView : AView
    {
        protected override void OnInit(object data)
        {
            base.OnInit(data);
            Debug.Log($"[SceneGameObjectView] {gameObject.name}");
        }
    }
}