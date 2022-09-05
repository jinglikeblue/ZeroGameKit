using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Sokoban
{
    public class LevelModel
    {
        /**
        * 关卡ID 
        */
        public int id;

        /// <summary>
        /// 阻挡
        /// </summary>
        public UnitVO[] blocks;

        /// <summary>
        /// 角色初始位置
        /// </summary>
        public UnitVO[] roles;

        /// <summary>
        /// 箱子的初始位置
        /// </summary>
        public UnitVO[] boxes;

        /// <summary>
        /// 目标点
        /// </summary>
        public UnitVO[] targets;

        private XmlDocument _xml;

        Dictionary<string, UnitVO> _unitDic = new Dictionary<string, UnitVO>();

        public LevelModel(int levelId)
        {
            id = levelId;            
            
            TextAsset data = ResMgr.Ins.Load<TextAsset>(AB.EXAMPLES_SOKOBAN_CONFIGS_LEVELS.NAME, $"level{levelId}.xml");
            var xml = new XmlDocument();
            xml.LoadXml(data.text);
            _xml = xml;

            blocks = GetUnitVO("blocks", EUnitType.BLOCK);
            roles = GetUnitVO("roles", EUnitType.ROLE);
            boxes = GetUnitVO("boxs", EUnitType.BOX);
            targets = GetUnitVO("targets", EUnitType.TARGET);
        }

        UnitVO[] GetUnitVO(string type, EUnitType unitType)
        {
            var levelNode = _xml.SelectSingleNode("level");
            XmlNodeList unitNodes = levelNode.SelectSingleNode(type).SelectNodes("unit");
            UnitVO[] units = new UnitVO[unitNodes.Count];
            for (int i = 0; i < units.Length; i++)
            {
                var attr = unitNodes[i] as XmlElement;
                var x = attr.GetAttribute("x");
                var y = attr.GetAttribute("y");

                UnitVO unitVO = new UnitVO();
                unitVO.x = ushort.Parse(x);
                unitVO.y = ushort.Parse(y);
                unitVO.type = unitType;
                units[i] = unitVO;
                _unitDic[GetPosFlag(unitVO.x, unitVO.y)] = unitVO;
            }
            return units;
        }

        UnitVO GetUnitVO(ushort x, ushort y)
        {
            string flag = GetPosFlag(x, y);
            if(_unitDic.ContainsKey(flag))
            {
                return _unitDic[flag];
            }
            return null;
        }

        string GetPosFlag(ushort x, ushort y)
        {
            return string.Format("{0}_{1}", x, y);
        }

        public bool IsBlock(ushort x, ushort y)
        {
            return IsType(x, y, EUnitType.BLOCK);       
        }

        public bool IsTarget(ushort x, ushort y)
        {
            return IsType(x, y, EUnitType.TARGET);
        }

        public bool IsBox(ushort x, ushort y)
        {
            return IsType(x, y, EUnitType.BOX);
        }

        

        public bool IsType(ushort x, ushort y, EUnitType unitType)
        {
            var vo = GetUnitVO(x, y);
            if (null == vo || vo.type != unitType)
            {
                return false;
            }

            return true;
        }
    }
}
