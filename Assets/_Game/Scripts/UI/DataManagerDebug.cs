﻿using System;
using System.Linq;
using TMPro;
using UnityEngine;
using System.Collections;
using Game;
using UnityEngine.SceneManagement;

public class DataManagerDebug : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text = null;
    [SerializeField] private GameObject _parent = null;
    [SerializeField] private Season _season = Season.None;
    [SerializeField] private TextMeshProUGUI _timerMain = null;
    [SerializeField] private TextMeshProUGUI _timerMil = null;

    private static bool _debugActive;
    static string myLog = "";
    private string output;
    private string stack;
    private float fps;
    private float updateInterval = 0.5f;
    private float accum = 0.0f;
    private int frames = 0;
    private float timeleft;
    private float currentTime;
    private float holdTime = 0;

    private void Awake()
    {
        if (_season == Season.End)
        {
            var endManager = FindObjectOfType<EndingsManager>();
            if (endManager != null)
            {
                endManager.OnEnd += SetEndTime;
            }
        }
    }

    private void Start() {
        currentTime = 0;
        SetDebugActive(_debugActive);
    }

    private void OnEnable() {
        Application.logMessageReceived += Log;
        TransitionManager.OnLevelComplete += SaveSplit;
    }

    private void OnDisable() {
        Application.logMessageReceived -= Log;
        TransitionManager.OnLevelComplete -= SaveSplit;
    }

    private void OnGUI() {
        if (_debugActive) {
            myLog = GUI.TextArea(new Rect(10, 420, 320, Screen.height - 430), myLog);
            GUI.Label(new Rect(10, 10, 120, 32), "FPS: " + fps);
        }
    }

    private void Update() {
        currentTime += Time.deltaTime;
        if (holdTime == 0)
        {
            _timerMain.text = TimeMain(currentTime);
            _timerMil.text = TimeMil(currentTime);
        }
        else
        {
            _timerMain.text = TimeMain(holdTime);
            _timerMil.text = TimeMil(holdTime);
        }

        if (Input.GetKeyDown(KeyCode.End)) {
            SetDebugActive(!_debugActive);
        }
        if (_debugActive) {
            string debug = "Season: " + DataManager.Instance.level + "\n";
            debug += "\n<b><u>Spirit Points</u></b>\n";
            debug += "Remaining: " + DataManager.Instance.remainingSpiritPoints + "\n";
            debug += "Total Used: " + DataManager.Instance.totalUsedSpiritPoints + "\n";
            debug += "\n<b><u>Endings</u></b>\n";
            debug += "True: " + DataManager.Instance.trueEndingPoints + "\n";
            debug += "Sisters: " + DataManager.Instance.sistersEndingPoints + "\n";
            debug += "Cousins: " + DataManager.Instance.cousinsEndingPoints + "\n";
            debug += "\n<b><u>Interactions</u></b>\n";
            debug += DataManager.Instance.interactions.Aggregate("", (current, interaction) => current + (interaction.Key + " - " + interaction.Value + "\n"));
            debug += "\n<b><u>Journal Unlocks</u></b>\n";
            debug += DataManager.Instance.journalUnlocks.Aggregate("", (current, interaction) => current + (interaction.Key + " - " + interaction.Value + "\n"));
            debug += "\n<b><u>Speedrunning</u></b>\n";
            debug += "Spring: " + TimeTotal(DataManager.Instance.SpringSplit) + "\n";
            debug += "Summer: " + TimeTotal(DataManager.Instance.SummerSplit) + "\n";
            debug += "Fall: " + TimeTotal(DataManager.Instance.FallSplit) + "\n";
            debug += "Winter: " + TimeTotal(DataManager.Instance.WinterSplit) + "\n";
            debug += "Total: " + TimeTotal(DataManager.Instance.SplitTotal) + "\n";
            debug += "\n<b><u>Best Splits</u></b>\n";
            debug += "Best Spring: " + TimeTotal(DataManager.Instance.SpringSplitBest) + "\n";
            debug += "Best Summer: " + TimeTotal(DataManager.Instance.SummerSplitBest) + "\n";
            debug += "Best Fall: " + TimeTotal(DataManager.Instance.FallSplitBest) + "\n";
            debug += "Best Winter: " + TimeTotal(DataManager.Instance.WinterSplitBest) + "\n";
            debug += "\n<b><u>Best Endings</u></b>\n";
            debug += "True: " + TimeTotal(DataManager.Instance.TrueEndBest) + "\n";
            debug += "Sister: " + TimeTotal(DataManager.Instance.SisterEndBest) + "\n";
            debug += "Cousin: " + TimeTotal(DataManager.Instance.CousinEndBest) + "\n";
            debug += "Bad: " + TimeTotal(DataManager.Instance.BadEndBest) + "\n";
            _text.text = debug;
        }
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0) {
            fps = (accum / frames);
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    private void SetEndTime(string end)
    {
        holdTime = DataManager.Instance.SplitTotal;
        end = end.ToLower();
        if (end.Contains("true"))
        {
            DataManager.Instance.SetTrueEnd(holdTime);
        } else if (end.Contains("sister"))
        {
            DataManager.Instance.SetSisterEnd(holdTime);
        }
        else if(end.Contains("cousin"))
        {
            DataManager.Instance.SetCousinEnd(holdTime);
        }
        else if(end.Contains("bad"))
        {
            DataManager.Instance.SetBadEnd(holdTime);
        }
    }

    private static string TimeTotal(float time) {
        return TimeMain(time) + TimeMil(time);
    }

    private static string TimeMain(float time) {
        float hour = Mathf.FloorToInt(time / 3600);
        float min = Mathf.FloorToInt(time / 60);
        float sec = Mathf.FloorToInt(time % 60);
        return $"{hour:0}:{min:00}:{sec:00}";
    }

    private static string TimeMil(float time) {
        float ms = (time % 1) * 1000;
        return $".{ms:000}";
    }

    private void SaveSplit() {
        DataManager.Instance.SetSplit(_season, currentTime);
    }

    public void GiveSpiritPoint() {
        if (DataManager.Instance.remainingSpiritPoints >= 10) return;
        DataManager.Instance.remainingSpiritPoints += 1;
        ModalWindowController.Singleton.ForceUpdateHudSpiritPoints();
    }

    private void SetDebugActive(bool active) {
        _debugActive = active;
        _parent.SetActive(active);
    }

    public void Log(string logString, string stackTrace, LogType type) {
        output = logString;
        stack = stackTrace;
        myLog = output + "\n" + myLog;
        if (myLog.Length > 5000) {
            myLog = myLog.Substring(0, 4000);
        }
    }
}