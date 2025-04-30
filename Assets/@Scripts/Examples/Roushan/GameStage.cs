using UnityEngine;
using Zero;
using ZeroGameKit;
using Zero;

namespace Roushan
{
    class GameStage : AView
    {
        Transform _blocks;
        GameObject _blockPrefab;

        Boss _boss;              

        protected override void OnInit(object data)
        {
            _blocks = GetChild("Blocks");
            _blockPrefab = GameObjectBindingData.Find(gameObject, "blockPrefab")[0] as GameObject;            
            _boss = CreateChildView<Boss>("Boss");

            UIPanelMgr.Ins.SwitchAsync<GamePanel>(this);
        }

        public void CreateBlock()
        {
            var ac = ResMgr.Load<AudioClip>(AB.EXAMPLES_AUDIOS.NAME, "click");
            AudioDevice.Get("effect", true).Play(gameObject,ac);
            //以异步方式创建Block
            ViewFactory.CreateAsync<Block>(AB.EXAMPLES_ROUSHAN.NAME, AB.EXAMPLES_ROUSHAN.Block, _blocks, null, OnCreatedBlock, OnProgressBlock, OnLoadedBlock);
            //ViewFactory.Create<Block>(_blocks);
        }

        private void OnProgressBlock(float progress)
        {
            Debug.LogFormat("方块资源加载进度:{0}", progress);
        }

        private void OnLoadedBlock(UnityEngine.Object obj)
        {
            Debug.LogFormat("方块资源加载完成:{0}", obj.ToString());
        }

        private void OnCreatedBlock(Block obj)
        {
            Debug.LogFormat("方块视图创建完成:{0}", obj.ToString());
        }
    }
}
