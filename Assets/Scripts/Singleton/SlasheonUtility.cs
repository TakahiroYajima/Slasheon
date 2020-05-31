using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlasheonUtility {

    /// <summary>
    /// レイヤー名の一致判定を返す
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layerName"></param>
    /// <returns></returns>
    public static bool IsLayerNameMatch(GameObject obj, string layerName)
    {
        if (obj != null && layerName != string.Empty && layerName != "")
        {
            return LayerMask.LayerToName(obj.layer) == layerName;
        }
        return false;
    }

    public static bool IsAnyLayerNameMatch(GameObject obj, string layerName1, string layerName2)
    {
        if (obj != null && layerName1 != string.Empty && layerName1 != "" && layerName2 != string.Empty && layerName2 != "")
        {
            string objLayer = LayerMask.LayerToName(obj.layer);
            return objLayer == layerName1 || objLayer == layerName2;
        }
        return false;
    }
}
