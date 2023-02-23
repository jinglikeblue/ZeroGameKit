using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroEditor.IOS
{
    public enum EIOSInfoPListItemDataType 
    {
        /// <summary>
        /// PlistElementArray
        /// </summary>
        ARRAY,
        /// <summary>
        /// PlistElementDict
        /// </summary>
        DICTIONARY,
        BOOLEAN,
        /// <summary>
        /// DateTime
        /// </summary>
        DATE,
        INTEGER,
        FLOAT,
        STRING           
    }

    public class IOSInfoPListDictionaryItem: IOSInfoPListItem
    {        
        [LabelText("Key")]  
        [PropertyOrder(-1)]
        public string key = "new_key";
    }

    [HideReferenceObjectPicker]        
    public class IOSInfoPListItem
    {
        void AddItemToArray()
        {
            arrayData.Add(new IOSInfoPListItem());
        }

        void AddItemToDict()
        {
            dictData.Add(new IOSInfoPListDictionaryItem());
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        [HideLabel]
        [HorizontalGroup("Item", 100)]        
        public EIOSInfoPListItemDataType type = EIOSInfoPListItemDataType.STRING;

        [ShowIf("type", EIOSInfoPListItemDataType.ARRAY)]
        [HideReferenceObjectPicker]
        [HorizontalGroup("Item")]
        [LabelText("Array")]
        [ListDrawerSettings(CustomAddFunction = "AddItemToArray", DraggableItems = false)]
        public List<IOSInfoPListItem> arrayData = new List<IOSInfoPListItem>();

        [ShowIf("type", EIOSInfoPListItemDataType.DICTIONARY)]
        [HideReferenceObjectPicker]
        [HorizontalGroup("Item")]
        [LabelText("Dictionary")]
        [ListDrawerSettings(CustomAddFunction = "AddItemToDict", DraggableItems = false)]
        public List<IOSInfoPListDictionaryItem> dictData = new List<IOSInfoPListDictionaryItem>();

        [ShowIf("type", EIOSInfoPListItemDataType.BOOLEAN)]
        [HorizontalGroup("Item")]
        [HideLabel]
        public bool booleanData = false;

        [ShowIf("type", EIOSInfoPListItemDataType.DATE)]
        [HorizontalGroup("Item")]
        [HideLabel]
        public DateTime dateData = DateTime.Now;

        [ShowIf("type", EIOSInfoPListItemDataType.INTEGER)]
        [HorizontalGroup("Item")]
        [HideLabel]
        public int integerData = 0;

        [ShowIf("type", EIOSInfoPListItemDataType.FLOAT)]
        [HorizontalGroup("Item")]
        [HideLabel]
        public float floatData = 0;

        [ShowIf("type", EIOSInfoPListItemDataType.STRING)]
        [HorizontalGroup("Item")]
        [HideLabel]
        public string stringData = "";
    }
}
