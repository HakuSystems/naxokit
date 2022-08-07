using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using naxokit.Styles;
using naxokit.Helpers.Logger;
using System.Linq;

public class AudioSourceVolumeControl : EditorWindow {

    public static List<AudioSource> audioSources = new List<AudioSource>();
    private Vector2 scrollPosition;
    private int showindex = 10;
    private string searchBar ="";

    public static void ShowWindow() {
        var window = GetWindow<AudioSourceVolumeControl>();
        window.titleContent = new GUIContent("AudioSourceVolumeControl");
        window.Show();
    }

    private void OnEnable(){
        minSize = new Vector2(1000, 300);
        maxSize = new Vector2(1000, 300);
        audioSources.Clear();
        foreach(var audioSource in GameObject.FindObjectsOfType<AudioSource>()) {
            audioSources.Add(audioSource);
        }
    }
    private void OnDestroy(){
        foreach(var audioSource in audioSources){
            audioSource.Pause();
        }
    }
    private void Update() {
        Repaint();
        if(audioSources.Count != GameObject.FindObjectsOfType<AudioSource>().Length) {
            audioSources.Clear();
            foreach(var audioSource in GameObject.FindObjectsOfType<AudioSource>()) {
                audioSources.Add(audioSource);
            }
        }
        int currentlyPlaying = 0;
        foreach(var audioSource in audioSources){
            if(audioSource.isPlaying) {
                currentlyPlaying++;
            }
        }
        if(currentlyPlaying > 1) {
            foreach(var audioSource in audioSources){
                if(audioSource.isPlaying) {
                    audioSource.Pause();
                }
            }
        }
    }
    private void OnGUI() {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField($"Total: {audioSources.Count}", EditorStyles.toolbarButton);
                EditorGUILayout.LabelField("AudioSources in Hierarchy", EditorStyles.toolbarButton);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                showindex = EditorGUILayout.IntSlider("Show Amount: ",showindex, 0, audioSources.Count - 1);
                searchBar = EditorGUILayout.TextField(searchBar, EditorStyles.toolbarSearchField);
            }
            EditorGUILayout.EndHorizontal();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                EditorGUILayout.BeginVertical();
                {
                    var resultCount = 0;
                    foreach(var audioSource in audioSources) {
                        resultCount++;
                        if(audioSource.name.ToLower().Contains(searchBar.ToLower())&& showindex > resultCount - 1) {
                            EditorGUILayout.BeginHorizontal();
                            {
                                if(audioSource.volume > 0.5f){
                                    GUI.color = Color.green;
                                } else if(audioSource.volume > 0.25f){
                                    GUI.color = Color.yellow;
                                } else {
                                    GUI.color = Color.red;
                                }                        
                                if(audioSource != null){
                                    if(GUILayout.Button(audioSource.name, EditorStyles.toolbarButton)) {
                                        Selection.activeObject = audioSource.gameObject;
                                    }
                                    AudioClip clip = audioSource.clip;
                                    var clipFileNameExtension = Path.GetFileName(AssetDatabase.GetAssetPath(clip));
                                    
                                    EditorGUILayout.LabelField(clipFileNameExtension, EditorStyles.boldLabel);
                                    
                                    if(audioSource.isPlaying)
                                    EditorGUI.ProgressBar(GUILayoutUtility.GetRect(100, 20), audioSource.time / audioSource.clip.length, $"{(int)(audioSource.time / 60)}:{(int)(audioSource.time % 60)} / {(int)(audioSource.clip.length / 60)}:{(int)(audioSource.clip.length % 60)}");
                                    
                                    if(clip != null){
                                        if(GUILayout.Button("Play", EditorStyles.miniButton)) {
                                            audioSource.Play();
                                        }
                                        if(GUILayout.Button("Pause", EditorStyles.miniButton)) {
                                            audioSource.Pause();
                                        }
                                    }
                                    
                                    
                                    if(GUI.color == Color.green){
                                        EditorGUILayout.LabelField("GOOD TO HEAR", EditorStyles.toolbarButton);
                                    } else if(GUI.color == Color.yellow){
                                        EditorGUILayout.LabelField("LISTEN CLOSELY", EditorStyles.toolbarButton);
                                    } else if(GUI.color == Color.red){
                                        EditorGUILayout.LabelField("MUTE/ALMOST MUTE", EditorStyles.toolbarButton);
                                    }
                                    audioSource.volume = EditorGUILayout.Slider(audioSource.volume, 0, 1);
                                
                                }
                                
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        else{resultCount--;}
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }
}