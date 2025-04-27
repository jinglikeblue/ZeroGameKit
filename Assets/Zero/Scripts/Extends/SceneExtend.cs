using UnityEngine;
using UnityEngine.SceneManagement;
using Zero;

namespace Zero
{
    /// <summary>
    /// Scene类的扩展
    /// </summary>
    public static class SceneExtend
    {
        /// <summary>
        /// 查找GameObject对象
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="gameObjectName"></param>
        /// <returns></returns>
        public static GameObject FindGameObject(this Scene scene, string gameObjectName)
        {
            return SceneManagerUtility.FindGameObject(scene, gameObjectName);
        }
    }
}