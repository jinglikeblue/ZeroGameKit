using Jing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZeroGameKit;
using Zero;

namespace Example
{
    class CSVFileExample
    {
        static public void Start()
        {
            var sb = new StringBuilder();

            var ta = Resources.Load<TextAsset>("examples/test_csv");
            var csv = new CSVFile(ta.bytes, Encoding.UTF8);
            for(var rowIdx = 0; rowIdx < csv.RowCount; rowIdx++)
            {
                var rowStr = "";
                for(var colIdx = 0; colIdx < csv.ColCount; colIdx++)
                {
                    Debug.Log($"row = {rowIdx}, colIdx = {colIdx}");
                    var v = csv.GetValue(rowIdx, colIdx);
                    if(v == string.Empty)
                    {
                        v = "[EMPTY!!!]";
                    }
                    rowStr += v.PadRight(18);
                }
                sb.AppendLine(rowStr);
            }

            var win = MsgWin.Show("CSVFileExample", sb.ToString());
            win.Resize(1280, 600);
            win.SetFontSize(16);
            win.SetContentAlignment(TextAnchor.MiddleLeft);            
        }
    }
}
