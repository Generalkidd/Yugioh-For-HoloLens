/*========================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.

Copyright (c) 2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
=========================================================================*/
using UnityEngine;
using Vuforia;

public class FrameRateSettings : MonoBehaviour
{
    #region MONOBEHAVIOUR_METHODS
    void Start ()
    {
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
    }
    #endregion


    #region PRIVATE_METHODS
    private void OnVuforiaStarted ()
    {
        // Query Vuforia for recommended frame rate and set it in Unity
        int targetFps = VuforiaRenderer.Instance.GetRecommendedFps(VuforiaRenderer.FpsHint.NONE);

        // By default, we use Application.targetFrameRate to set the recommended frame rate.
        // Google Cardboard does not use vsync, and OVR explicitly disables it. If developers 
        // use vsync in their quality settings, they should also set their QualitySettings.vSyncCount
        // according to the value returned above.
        // e.g: If targetFPS > 50 --> vSyncCount = 1; else vSyncCount = 2;
        if (Application.targetFrameRate != targetFps)
        {
            Debug.Log("Setting frame rate to " + targetFps + "fps");
            Application.targetFrameRate = targetFps;
        }
    }
    #endregion
}
