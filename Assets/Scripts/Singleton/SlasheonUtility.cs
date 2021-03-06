﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public static bool IsAnyLayerNameMatch(GameObject obj, string[] layerNames)
    {
        int hitLayerCount = layerNames.Where(x => x == LayerMask.LayerToName(obj.layer)).Count();
        //Debug.Log("hitlayercount : " + hitLayerCount + " : " + LayerMask.LayerToName(obj.layer));
        if(hitLayerCount >= 1)
        {
            return true;
        }
        //if (obj != null && layerName1 != string.Empty && layerName1 != "" && layerName2 != string.Empty && layerName2 != "")
        //{
        //    string objLayer = LayerMask.LayerToName(obj.layer);
        //    return objLayer == layerName1 || objLayer == layerName2;
        //}
        return false;
    }

    public static readonly string[] UILayer = new string[]
    {
        "PlayerUI", "Button","UI", "CameraRotationUI"
    };
}
