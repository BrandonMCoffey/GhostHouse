#if UNITY_EDITOR
using System.Collections.Generic;
using Mechanics.Level_Mechanics;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InteractablesCheck))]
public class EndCheckEditor : Editor
{
    private List<Interactable> inGame;
    private List<StoryInteractable> storiesInScene;
    private List<Interactable> inScene;
    private int count;
    private int cost;
    private List<Interactable> costInt;
    private int trueEnd;
    private List<Interactable> trueInt;
    private int sisters;
    private List<Interactable> sisterInt;
    private int cousins;
    private List<Interactable> cousinInt;

    private bool f0;
    private bool f1;
    private bool f2;
    private bool f3;
    private bool f4;
    private bool f5;
    private bool f6;
    private bool f7;
    private bool f8;
    private bool f9;
    private bool f10;

    private void OnEnable() {
        var stories = (StoryInteractable[])Resources.FindObjectsOfTypeAll(typeof(StoryInteractable));
        inScene = new List<Interactable>();
        storiesInScene = new List<StoryInteractable>();
        foreach (var story in stories) {
            if (!EditorUtility.IsPersistent(story.transform.root.gameObject)) {
                if (story.Interaction != null) inScene.Add(story.Interaction);
                if (story.AltInteraction != null) inScene.Add(story.AltInteraction);
                storiesInScene.Add(story);
            }
        }
        var interactables = (Interactable[])Resources.FindObjectsOfTypeAll(typeof(Interactable));
        inGame = new List<Interactable>();
        count = 0;
        costInt = new List<Interactable>();
        cost = 0;
        inGame = new List<Interactable>();
        trueEnd = 0;
        trueInt = new List<Interactable>();
        sisters = 0;
        sisterInt = new List<Interactable>();
        cousins = 0;
        cousinInt = new List<Interactable>();

        foreach (var interactable in interactables)
       {
            inGame.Add(interactable);
            count++;
            if (interactable.Cost > 0) {
                cost++;
                costInt.Add(interactable);
            }
            if (interactable.TrueEndingPoints > 0) {
                trueEnd += interactable.TrueEndingPoints;
                trueInt.Add(interactable);
            }
            if (interactable.SisterEndPoints > 0) {
                sisters += interactable.SisterEndPoints;
                sisterInt.Add(interactable);
            }
            if (interactable.CousinEndingPoints > 0) {
                cousins += interactable.CousinEndingPoints;
                cousinInt.Add(interactable);
            }
        }
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUIStyle headerStyle = new GUIStyle();
        headerStyle.fontSize = 24;
        GUILayout.Label("In Scene", headerStyle);

        f0 = EditorGUILayout.BeginFoldoutHeaderGroup(f0, "Triggers: " + storiesInScene.Count);
        if (f0)
        {
            foreach (StoryInteractable i in storiesInScene)
            {
                EditorGUILayout.ObjectField(i, typeof(StoryInteractable), true);
            }
            GUILayout.Space(8);
        }
        GUILayout.Space(2);
        EditorGUILayout.EndFoldoutHeaderGroup();
        f1 = EditorGUILayout.BeginFoldoutHeaderGroup(f1, "Interactions: " + inScene.Count);
        if (f1) {
            foreach (Interactable i in inScene) {
                EditorGUILayout.ObjectField(i, typeof(Interactable), true);
            }
            GUILayout.Space(8);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        int cost = 0;
        int truePoints = 0;
        int sisterPoints = 0;
        int cousinPoints = 0;
        foreach (Interactable i in inScene) {
            if (i.Cost > 0) {
                cost++;
            }
            if (i.TrueEndingPoints > 0) {
                truePoints++;
            }
            if (i.SisterEndPoints > 0) {
                sisterPoints++;
            }
            if (i.CousinEndingPoints > 0) {
                cousinPoints++;
            }
        }
        GUILayout.Space(2);
        f2 = EditorGUILayout.BeginFoldoutHeaderGroup(f2, "With cost: " + cost);
        if (f2)
        {
            foreach (Interactable i in inScene)
            {
                if (i.Cost > 0)
                    EditorGUILayout.ObjectField(i, typeof(Interactable), true);
            }
            GUILayout.Space(8);
        }
        GUILayout.Space(2);
        EditorGUILayout.EndFoldoutHeaderGroup();
        f3 = EditorGUILayout.BeginFoldoutHeaderGroup(f3, "With True Points: " + truePoints);
        if (f3)
        {
            foreach (Interactable i in inScene)
            {
                if (i.TrueEndingPoints > 0)
                    EditorGUILayout.ObjectField(i, typeof(Interactable), true);
            }
            GUILayout.Space(8);
        }
        GUILayout.Space(2);
        EditorGUILayout.EndFoldoutHeaderGroup();
        f4 = EditorGUILayout.BeginFoldoutHeaderGroup(f4, "With Sister Points: " + sisterPoints);
        if (f4)
        {
            foreach (Interactable i in inScene)
            {
                if (i.SisterEndPoints > 0)
                    EditorGUILayout.ObjectField(i, typeof(Interactable), true);
            }
            GUILayout.Space(8);
        }
        GUILayout.Space(2);
        EditorGUILayout.EndFoldoutHeaderGroup();
        f5 = EditorGUILayout.BeginFoldoutHeaderGroup(f5, "With Cousin Points: " + cousinPoints);
        if (f5)
        {
            foreach (Interactable i in inScene)
            {
                if (i.CousinEndingPoints > 0)
                    EditorGUILayout.ObjectField(i, typeof(Interactable), true);
            }
            GUILayout.Space(8);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();


        GUILayout.Space(32);
        GUILayout.Label("Total In Game", headerStyle);
        GUILayout.Space(2);



        f6 = EditorGUILayout.BeginFoldoutHeaderGroup(f6, "Interactions: " + count);
        if (f6)
        {
            foreach (Interactable i in inGame)
            {
                EditorGUILayout.ObjectField(i, typeof(Interactable), true);
            }
            GUILayout.Space(8);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(2);
        f7 = EditorGUILayout.BeginFoldoutHeaderGroup(f7, "With Cost: " + costInt.Count);
        if (f7)
        {
            foreach (Interactable i in costInt)
            {
                EditorGUILayout.ObjectField(i, typeof(Interactable), true);
            }
            GUILayout.Space(8);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(2);
        f8 = EditorGUILayout.BeginFoldoutHeaderGroup(f8, "With True Points: " + trueEnd);
        if (f8)
        {
            foreach (Interactable i in trueInt)
            {
                EditorGUILayout.ObjectField(i, typeof(Interactable), true);
            }
            GUILayout.Space(8);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(2);
        f9 = EditorGUILayout.BeginFoldoutHeaderGroup(f9, "With Sister Points: " + sisters);
        if (f9)
        {
            foreach (Interactable i in sisterInt)
            {
                EditorGUILayout.ObjectField(i, typeof(Interactable), true);
            }
            GUILayout.Space(8);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(2);
        f10 = EditorGUILayout.BeginFoldoutHeaderGroup(f10, "With Cousins Points: " + cousins);
        if (f10)
        {
            foreach (Interactable i in cousinInt)
            {
                EditorGUILayout.ObjectField(i, typeof(Interactable), true);
            }
            GUILayout.Space(8);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();



        GUILayout.Space(32);
        GUILayout.Label("Points to Win", headerStyle);
        GUILayout.Space(2);
        GUILayout.Label("True: 5");
        GUILayout.Label("Cousin: 4");
        GUILayout.Label("Sister: 4");
    }
}
#endif