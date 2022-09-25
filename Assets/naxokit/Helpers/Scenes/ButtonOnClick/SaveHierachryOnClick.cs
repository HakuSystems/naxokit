using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using naxokit;
using UnityEngine.SceneManagement;
using naxokit.Helpers.Logger;

public class SaveHierachryOnClick : MonoBehaviour
{
    public void SaveHierachry()
    {
        const string path = "Assets/naxokit/SavedHierachry/";
        var sceneToIgnore = SceneManager.GetSceneByName("naxokitPlayModeTools");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var infoText = GameObject.Find("InfoText");
        var infoTextText = infoText.GetComponent<Text>();
        infoTextText.text = "Saving Hierachry";
        naxoLog.Log("SaveHierachryButton", "Saving Hierachry");

        var folder = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        Directory.CreateDirectory(path + folder);

        var savedHierachry = new GameObject("SavedHierachry");
        foreach (var go in (GameObject[])FindObjectsOfType(typeof(GameObject)))
        {
            if (go.transform.parent == null && go.scene != sceneToIgnore)
            {
                go.transform.SetParent(savedHierachry.transform);
            }
        }

        PrefabUtility.SaveAsPrefabAsset(savedHierachry, path + folder + "/SavedHierachry.prefab");
        infoTextText.text = "Saved Hierachry";
        naxoLog.Log("SaveHierachryButton", "Saved Hierachry to " + path + folder + "/SavedHierachry.prefab");
        infoTextText.color = Color.green;
    }
}
