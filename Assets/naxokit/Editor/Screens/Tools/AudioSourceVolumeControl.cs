using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using naxokit.Styles;
using naxokit.Helpers.Logger;
using System.Linq;

public class AudioSourceVolumeControl : EditorWindow
{
    private static readonly List<AudioSource> AudioSources = new List<AudioSource>();
    private Vector2 _scrollPosition;
    private int _showindex = 10;
    private string _searchBar = "";

    public static void ShowWindow()
    {
        var window = GetWindow<AudioSourceVolumeControl>();
        window.titleContent = new GUIContent("AudioSourceVolumeControl");
        window.Show();
    }

    private void OnEnable()
    {
        minSize = new Vector2(1000, 300);
        maxSize = new Vector2(1000, 300);
        AudioSources.Clear();
        foreach (var audioSource in GameObject.FindObjectsOfType<AudioSource>())
        {
            AudioSources.Add(audioSource);
        }
    }
    private void OnDestroy()
    {
        foreach (var audioSource in AudioSources)
        {
            audioSource.Pause();
        }
    }
    private void Update()
    {
        Repaint();
        if (AudioSources.Count != GameObject.FindObjectsOfType<AudioSource>().Length)
        {
            AudioSources.Clear();
            foreach (var audioSource in GameObject.FindObjectsOfType<AudioSource>())
            {
                AudioSources.Add(audioSource);
            }
        }
        var currentlyPlaying = AudioSources.Count(audioSource => audioSource.isPlaying);
        if (currentlyPlaying <= 1) return;
        {
            foreach (var audioSource in AudioSources.Where(audioSource => audioSource.isPlaying))
            {
                audioSource.Pause();
            }
        }
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (AudioSources.Count == 0) EditorGUILayout.LabelField("No AudioSources found", EditorStyles.toolbarButton);
                else EditorGUILayout.LabelField("AudioSources found: " + AudioSources.Count, EditorStyles.toolbarButton);

                EditorGUILayout.LabelField("AudioSources in Hierarchy", EditorStyles.toolbarButton);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                _showindex = AudioSources.Count == 1 ? 1 : EditorGUILayout.IntSlider("Show Amount: ", _showindex, 1, AudioSources.Count);
                _searchBar = EditorGUILayout.TextField(_searchBar, EditorStyles.toolbarSearchField);
            }
            EditorGUILayout.EndHorizontal();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false, GUILayout.Width(EditorGUIUtility.currentViewWidth));
            {
                EditorGUILayout.BeginVertical();
                {
                    var resultCount = 0;
                    foreach (var audioSource in AudioSources)
                    {
                        resultCount++;
                        if (audioSource.name.ToLower().Contains(_searchBar.ToLower()) && _showindex > resultCount - 1)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                if (audioSource.volume > 0.5f)
                                {
                                    GUI.color = Color.green;
                                }
                                else if (audioSource.volume > 0.25f)
                                {
                                    GUI.color = Color.yellow;
                                }
                                else
                                {
                                    GUI.color = Color.red;
                                }
                                if (audioSource != null)
                                {
                                    if (GUILayout.Button(audioSource.name, EditorStyles.toolbarButton))
                                    {
                                        Selection.activeObject = audioSource.gameObject;
                                    }
                                    AudioClip clip = audioSource.clip;
                                    var clipFileNameExtension = Path.GetFileName(AssetDatabase.GetAssetPath(clip));

                                    EditorGUILayout.LabelField(clipFileNameExtension, EditorStyles.boldLabel);

                                    if (audioSource.isPlaying)
                                        EditorGUI.ProgressBar(GUILayoutUtility.GetRect(100, 20), audioSource.time / audioSource.clip.length, $"{(int)(audioSource.time / 60)}:{(int)(audioSource.time % 60)} / {(int)(audioSource.clip.length / 60)}:{(int)(audioSource.clip.length % 60)}");

                                    if (clip != null)
                                    {
                                        if (GUILayout.Button("Play", EditorStyles.miniButton))
                                        {
                                            audioSource.Play();
                                        }
                                        if (GUILayout.Button("Pause", EditorStyles.miniButton))
                                        {
                                            audioSource.Pause();
                                        }
                                    }


                                    if (GUI.color == Color.green)
                                    {
                                        EditorGUILayout.LabelField("GOOD TO HEAR", EditorStyles.toolbarButton);
                                    }
                                    else if (GUI.color == Color.yellow)
                                    {
                                        EditorGUILayout.LabelField("LISTEN CLOSELY", EditorStyles.toolbarButton);
                                    }
                                    else if (GUI.color == Color.red)
                                    {
                                        EditorGUILayout.LabelField("MUTE/ALMOST MUTE", EditorStyles.toolbarButton);
                                    }
                                    audioSource.volume = EditorGUILayout.Slider(audioSource.volume, 0, 1);

                                }

                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        else { resultCount--; }
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }
}