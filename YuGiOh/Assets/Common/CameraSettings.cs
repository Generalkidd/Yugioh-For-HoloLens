/*===============================================================================
Copyright (c) 2015 PTC Inc. All Rights Reserved.
 
Copyright (c) 2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using System.Collections;
using Vuforia;

public class CameraSettings : MonoBehaviour
{
    #region PRIVATE_MEMBERS
    private bool mVuforiaStarted = false;
    private bool mAutofocusEnabled = true;
    private bool mFlashTorchEnabled = false;
    private CameraDevice.CameraDirection mActiveDirection = CameraDevice.CameraDirection.CAMERA_DEFAULT;
    #endregion //PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start () 
    {
        VuforiaAbstractBehaviour vuforia = FindObjectOfType<VuforiaAbstractBehaviour>();
        vuforia.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        vuforia.RegisterOnPauseCallback(OnPaused);
    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public bool IsFlashTorchEnabled()
    {
        return mFlashTorchEnabled;
    }

    public void SwitchFlashTorch(bool ON)
    {
        if (CameraDevice.Instance.SetFlashTorchMode(ON))
        {
            Debug.Log("Successfully turned flash " + ON);
            mFlashTorchEnabled = ON;
        }
        else
        {
            Debug.Log("Failed to set the flash torch " + ON);
            mFlashTorchEnabled = false;
        }
    }

    public bool IsAutofocusEnabled()
    {
        return mAutofocusEnabled;
    }

    public void SwitchAutofocus(bool ON)
    {
        if (ON)
        {
            if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO))
            {
                Debug.Log("Successfully enabled continuous autofocus.");
                mAutofocusEnabled = true;
            }
            else
            {
                // Fallback to normal focus mode
                Debug.Log("Failed to enable continuous autofocus, switching to normal focus mode");
                mAutofocusEnabled = false;
                CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
            }
        }
        else
        {
            Debug.Log("Disabling continuous autofocus (enabling normal focus mode).");
            mAutofocusEnabled = false;
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
        }
    }

    public void TriggerAutofocusEvent()
    {
        // Trigger an autofocus event
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);

        // Then restore original focus mode
        StartCoroutine(RestoreOriginalFocusMode());
    }

    public void SelectCamera(CameraDevice.CameraDirection camDir)
    {
        if (RestartCamera (camDir)) 
        {
            mActiveDirection = camDir;

            // Upon camera restart, flash is turned off
            mFlashTorchEnabled = false;
        }
    }

    public bool IsFrontCameraActive()
    {
        return (mActiveDirection == CameraDevice.CameraDirection.CAMERA_FRONT);
    }
    #endregion // PUBLIC_METHODS


    #region PRIVATE_METHODS
    private void OnVuforiaStarted()
    {
        mVuforiaStarted = true;
        // Try enabling continuous autofocus
        SwitchAutofocus(true);
    }

    private void OnPaused(bool paused)
    {
        bool appResumed = !paused;
        if (appResumed && mVuforiaStarted)
        {
            // Restore original focus mode when app is resumed
            if (mAutofocusEnabled)
                CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
            else
                CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);

            // Set the torch flag to false on resume (cause the flash torch is switched off by the OS automatically)
            mFlashTorchEnabled = false;
        }
    }

    private IEnumerator RestoreOriginalFocusMode()
    {
        // Wait 1.5 seconds
        yield return new WaitForSeconds(1.5f);

        // Restore original focus mode
        if (mAutofocusEnabled)
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        else
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
    }

    private bool RestartCamera(CameraDevice.CameraDirection direction)
    {
        ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        if (tracker != null)
            tracker.Stop();

        CameraDevice.Instance.Stop();
        CameraDevice.Instance.Deinit();

        if (!CameraDevice.Instance.Init(direction))
        {
            Debug.Log("Failed to init camera for direction: " + direction.ToString());
            return false;
        }
        if (!CameraDevice.Instance.Start())
        {
            Debug.Log("Failed to start camera for direction: " + direction.ToString());
            return false;
        }

        if (tracker != null)
        {
            if (!tracker.Start())
            {
                Debug.Log("Failed to restart the Tracker.");
                return false;
            }
        }
            
        return true;
    }
    #endregion // PRIVATE_METHODS
}
