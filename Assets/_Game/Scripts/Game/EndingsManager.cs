﻿using Mechanics.Level_Mechanics;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility.Audio.Clips;
using Utility.Audio.Managers;

public class EndingsManager : MonoBehaviour
{
    [SerializeField] private Ending _trueEnding = null;

    [SerializeField] private Ending _cousinEnding = null;

    [SerializeField] private Ending _sisterEnding = null;

    [SerializeField]
    [Tooltip("Threshold for this ending is ignored.")]
    private Ending badEnding = null;

    [Space]
    [SerializeField]
    private Game.TransitionManager _transitionManager = null;

    public event Action<string> OnEnd = delegate { };

    private void Start()
    {
        DataManager data = DataManager.Instance;
        List<EndingPair> possibleChoices = new List<EndingPair>();

        // get prioritized list of endings not yet unlocked
        data.TestJournalUnlockExists(_trueEnding.Dialog.name);
        data.TestJournalUnlockExists(_sisterEnding.Dialog.name);
        data.TestJournalUnlockExists(_cousinEnding.Dialog.name);
        if (!data.journalUnlocks[_trueEnding.Dialog.name] && data.trueEndingPoints >= _trueEnding.Threshold)
        {
            possibleChoices.Add(new EndingPair(_trueEnding, 0));
        }
        else if (!data.journalUnlocks[_sisterEnding.Dialog.name] && data.sistersEndingPoints >= _sisterEnding.Threshold)
        {
            possibleChoices.Add(new EndingPair(_sisterEnding, 3));
        }
        else if (!data.journalUnlocks[_cousinEnding.Dialog.name] && data.cousinsEndingPoints >= _cousinEnding.Threshold)
        {
            possibleChoices.Add(new EndingPair(_cousinEnding, 2));
        }

        EndingPair selectedEnding;
        // choose from list of endings not done yet, if possible
        if (possibleChoices.Count > 0)
        {
            selectedEnding = possibleChoices[0];
        }
        // follow default priorities
        else
        {
            if (data.trueEndingPoints >= _trueEnding.Threshold)
            {
                selectedEnding = new EndingPair(_trueEnding, 0);
            }
            else if (data.sistersEndingPoints >= _sisterEnding.Threshold)
            {
                selectedEnding = new EndingPair(_sisterEnding, 3);
            }
            else if (data.cousinsEndingPoints >= _cousinEnding.Threshold)
            {
                selectedEnding = new EndingPair(_cousinEnding, 2);
            }
            else
            {
                selectedEnding = new EndingPair(badEnding, 1);
            }
        }

        foreach (Ending end in new List<Ending>() { _trueEnding, _cousinEnding, _sisterEnding, badEnding })
        {
            if (end == selectedEnding.ending)
            {
                end.Visuals?.SetActive(true);
                _transitionManager._interactionOnStart = end.Dialog;
                SoundManager.MusicManager.PlayMusic(end.MusicTrack);
                data.SetInteraction(selectedEnding.ending.Dialog.name, true);
                data.WriteFile();
                OnEnd?.Invoke(end.Visuals.name);
            }
            else
            {
                end.Visuals?.SetActive(false);
            }
        }
    }

    public void GoToScene(string nextScene)
    {
        DataManager.SceneLoader.LoadScene(nextScene);
    }

    [Serializable]
    private class Ending
    {
        [Min(0)]
        public int Threshold = 0;

        public Interactable Dialog = null;
        public GameObject Visuals = null;
        public MusicTrack MusicTrack = null;
    }

    private class EndingPair
    {
        public Ending ending;
        public int index;

        public EndingPair(Ending ending, int index)
        {
            this.ending = ending;
            this.index = index;
        }
    }
}