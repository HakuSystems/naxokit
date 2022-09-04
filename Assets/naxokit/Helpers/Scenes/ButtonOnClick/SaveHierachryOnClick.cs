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
        string path = "Assets/naxokit/SavedHierachry/";
        Scene sceneToIgnore = SceneManager.GetSceneByName("naxokitPlayModeTools");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        GameObject InfoText = GameObject.Find("InfoText");
        Text InfoTextText = InfoText.GetComponent<Text>();
        InfoTextText.text = "Saving Hierachry";
        naxoLog.Log("SaveHierachryButton", "Saving Hierachry");

        string folder = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        Directory.CreateDirectory(path + folder);

        GameObject SavedHierachry = new GameObject("SavedHierachry");
        foreach (GameObject go in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if (go.transform.parent == null && go.scene != sceneToIgnore)
            {
                go.transform.SetParent(SavedHierachry.transform);
            }
        }

        PrefabUtility.SaveAsPrefabAsset(SavedHierachry, path + folder + "/SavedHierachry.prefab");
        InfoTextText.text = "Saved Hierachry";
        naxoLog.Log("SaveHierachryButton", "Saved Hierachry to " + path + folder + "/SavedHierachry.prefab");
        InfoTextText.color = Color.green;
    }
}
