using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using naxokit.Styles;

public class Settings : EditorWindow
{
    bool state = false;
    Texture2D texture2d;
    public Settings(string image)
    {
        texture2d = Resources.Load(image) as Texture2D;
        state = FoldoutTexture.MakeTextureFoldout();
    }

}
