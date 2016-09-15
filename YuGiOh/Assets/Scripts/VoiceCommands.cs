/*===============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/

using UnityEngine;

#if ENABLE_HOLOLENS_MODULE_API
using UnityEngine.Windows.Speech;
#endif

using System.Collections.Generic;
using System.Linq;
using Vuforia;

public class VoiceCommands : MonoBehaviour
{

// So that this builds against older versions of the Unity DLLs we need to 
// #if the code that uses HoloLens specific features out.
// Unity have suggested that UNITY_HOLOGRAPHIC should be defined but we
// have not seen this work
#if ENABLE_HOLOLENS_MODULE_API
    
    #region PRIVATE_MEMBERS
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    #endregion //PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS
    // Use this for initialization
    void Start()
    {
        keywords.Add("Start Extended Tracking", () =>
        {
            StateManager stateManager = TrackerManager.Instance.GetStateManager();
            foreach (var tb in stateManager.GetTrackableBehaviours())
            {
                if (tb is ImageTargetBehaviour)
                {
                    ImageTargetBehaviour itb = tb as ImageTargetBehaviour;
                    if (!itb.ImageTarget.StartExtendedTracking())
                    {
                        Debug.LogError("Failed to start Extended Tracking on Target " + itb.TrackableName);
                    }
                }
            }
            Debug.Log("Start Extended Tracking");
        });

        keywords.Add("Stop Extended Tracking", () =>
        {
            StateManager stateManager = TrackerManager.Instance.GetStateManager();
            foreach (var tb in stateManager.GetTrackableBehaviours())
            {
                if (tb is ImageTargetBehaviour)
                {
                    ImageTargetBehaviour itb = tb as ImageTargetBehaviour;
                    if (!itb.ImageTarget.StopExtendedTracking())
                    {
                        Debug.LogError("Failed to stop Extended Tracking on Target " + itb.TrackableName);
                    }
                }
            }
            Debug.Log("Stop Extended Tracking");
        });

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

    }

    void Update()
    {
    }

    #endregion //MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
    #endregion //PRIVATE_METHODS

#endif // ENABLE_HOLOLENS_MODULE_API

}
