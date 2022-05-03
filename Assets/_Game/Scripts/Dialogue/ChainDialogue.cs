﻿using UnityEngine;
using Mechanics.Level_Mechanics;

public class ChainDialogue : MonoBehaviour
{
    public void SetNextDialogue(StoryInteractable interaction)
    {
        AfterDialogueInteraction.Singleton.SetNextInteraction(interaction);
    }
}
