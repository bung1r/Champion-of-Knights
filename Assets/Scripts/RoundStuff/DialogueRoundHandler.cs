using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueRoundHandler : MonoBehaviour
{
    public static DialogueRoundHandler Instance;
    private DialogueManager dialogueManager;
    private RoundManager roundManager;
    public const int finalRound = 7;

    public Talkable guide;
    public List<DialogueRound> guideRoundsDialogues;
    // 0 is intro 
    // 1 and 3 are hard coded dialogue
    // 2 is recurring dialogue
    // 4 end of game dialogue
    // 5 -> future use
    public Talkable viewer;
    public DialogueRound allViewers;
    private List<FullDialogue> thisRoundViewerDialogues = new List<FullDialogue>();
    private int roundViewerIndex = 0;
    private bool consistentViewerDialogue = true; // turns false when too corrupted.
    public List<DialogueRound> persistentViewerRoundDialogue;
    // 0 is intro
    // 1 -> 2 are hoardcoded dialogue
    // 3 recurring dialogue
    // 4 end of game dialogue
    // 5 -> future use
    public FullDialogue corruptionDialogue;
    public Talkable shopkeeper;
    public DialogueRound shopkeeperDialogues;
    public List<FullDialogue> endingDialogues = new List<FullDialogue>();
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
    }
    void OnEnable()
    {
        viewer.onTalkedTo += OnViewerTalkedTo;
    }
    void OnViewerTalkedTo(Talkable talkable)
    {
        roundViewerIndex++;
        if (roundViewerIndex < thisRoundViewerDialogues.Count)
        {
            talkable.SwitchDialouge(thisRoundViewerDialogues[roundViewerIndex]);
        } else
        {
            talkable.SwitchDialouge(null);
        }
    }   

    public void HandleRoundDialogue(int roundIndex)
    {
        thisRoundViewerDialogues.Clear();
        roundViewerIndex = 0;
        if (RoundManager.Instance.GetPlayer.stats.corruption >= 98f)
        {
            // everything you talk to is your own corrupted dialogue.
            thisRoundViewerDialogues.Add(corruptionDialogue);
            guide.SwitchDialouge(corruptionDialogue);
            return;
        }
        // pick 3 random viewer dialogues.
        while (thisRoundViewerDialogues.Count < 3) {
            FullDialogue randDialogue = allViewers.roundDialogue[UnityEngine.Random.Range(0, allViewers.roundDialogue.Count)];

            if (!thisRoundViewerDialogues.Contains(randDialogue)) {
                Debug.Log(randDialogue.name);
                thisRoundViewerDialogues.Add(randDialogue);
            }
        }
        
        GuideDialogue(roundIndex);
        PersistentViewerDialogue(roundIndex);

        viewer.dialogue = thisRoundViewerDialogues[0];
    }

    void PersistentViewerDialogue(int roundIndex)
    {
        float corruption = RoundManager.Instance.GetPlayer.stats.corruption;
        float highestViewers = RoundManager.Instance.GetHighestViewersThisRound;
        switch (roundIndex)
        {
            case 1:
                thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[0].roundDialogue[0]);
                break;
            case 2: 
                if (corruption >= 20f)
                {
                    thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[1].roundDialogue[2]);
                } else if (highestViewers >= 1500f)
                {
                    thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[1].roundDialogue[0]);
                }
                else
                {
                    thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[1].roundDialogue[1]);
                }
                break;
            case 3:
                if (corruption >= 35f)
                {
                    thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[2].roundDialogue[2]);
                } else if (highestViewers >= 1600f)
                {
                    thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[2].roundDialogue[0]);
                }
                else
                {
                    thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[2].roundDialogue[1]);
                }
                break;
            default:
                if (consistentViewerDialogue == false) break;
                if (corruption >= 80f)
                {
                    consistentViewerDialogue = false;
                    thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[3].roundDialogue[3]);
                }
                else if (corruption >= 40f)
                {
                    thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[3].roundDialogue[2]);
                } else if (highestViewers >= 2400f)
                {
                    thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[3].roundDialogue[0]);
                }
                else
                {
                    thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[3].roundDialogue[1]);
                }
                break;
            case finalRound - 1:
                if (consistentViewerDialogue)
                {
                    thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[4].roundDialogue[1]);
                } else
                {
                    thisRoundViewerDialogues.Add(persistentViewerRoundDialogue[4].roundDialogue[0]);
                }
                break;

        }
    }
    void GuideDialogue(int roundIndex)
    {   
        switch(roundIndex) {
            case 1:
                guide.SwitchDialouge(guideRoundsDialogues[0].roundDialogue[0]);
                break;
            case 2:
                guide.SwitchDialouge(guideRoundsDialogues[1].roundDialogue[0]);
                break;
            case 4:
                guide.SwitchDialouge(guideRoundsDialogues[3].roundDialogue[0]);
                break;
            default:
                if (RoundManager.Instance.GetHighestViewersThisRound >= 1800)
                {
                    guide.SwitchDialouge(guideRoundsDialogues[2].roundDialogue[0]);
                }
                else
                {
                    guide.SwitchDialouge(guideRoundsDialogues[2].roundDialogue[1]);
                }
                break;
            case finalRound:
                guide.SwitchDialouge(guideRoundsDialogues[4].roundDialogue[0]);
                break;
        }
    }

    public FullDialogue GetEndingDialogue(int endingIndex)
    {
        float corruption = RoundManager.Instance.GetPlayer.stats.corruption;
        switch (endingIndex)
        {
            case 1:
                if (corruption < 20f)
                {
                    return endingDialogues[0]; // good end
                } else if (corruption < 50f)
                {
                    return endingDialogues[1]; // battleseeker end
                } else
                {
                    return endingDialogues[2]; // insanity end
                }
            case 2:
                return endingDialogues[3]; // stay end
            case 3:
                return endingDialogues[4]; // corrupt end 1
            case 4:
                return endingDialogues[5]; // corrupt end 2
            default:
                return endingDialogues[0];
        }   
    }
}