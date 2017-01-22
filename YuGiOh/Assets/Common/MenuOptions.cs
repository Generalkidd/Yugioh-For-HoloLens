/*===============================================================================
Copyright (c) 2015-2016 PTC Inc. All Rights Reserved.
 
Copyright (c) 2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Vuforia;

public class MenuOptions : MonoBehaviour
{
    #region PRIVATE_MEMBERS
    private CameraSettings mCamSettings = null;
    private TrackableSettings mTrackableSettings = null;
    private MenuAnimator mMenuAnim = null;
    #endregion //PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    protected virtual void Start()
    {
        mCamSettings = FindObjectOfType<CameraSettings>();
        mTrackableSettings = FindObjectOfType<TrackableSettings>();
        mMenuAnim = FindObjectOfType<MenuAnimator>();

        var vuforia = VuforiaARController.Instance;
        vuforia.RegisterOnPauseCallback(OnPaused);
    }
    #endregion //MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public void ShowAboutPage()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Vuforia-1-About");
    }

    public void ToggleAutofocus()
    {
        Toggle autofocusToggle = FindUISelectableWithText<Toggle>("Autofocus");
        if (autofocusToggle && mCamSettings)
            mCamSettings.SwitchAutofocus(autofocusToggle.isOn);
    }

    public void ToggleTorch()
    {
        Toggle flashToggle = FindUISelectableWithText<Toggle>("Flash");
        if (flashToggle && mCamSettings)
        {
            mCamSettings.SwitchFlashTorch(flashToggle.isOn);

            // Update UI toggle status (ON/OFF) in case the flash switch failed
            flashToggle.isOn = mCamSettings.IsFlashTorchEnabled();
        }
    }

    public void SelectCamera(bool front)
    {
        if (mCamSettings)
		{
			mCamSettings.SelectCamera(front ? CameraDevice.CameraDirection.CAMERA_FRONT : CameraDevice.CameraDirection.CAMERA_BACK);

			// Toggle flash if it is on while switching to front camera
			Toggle flashToggle = FindUISelectableWithText<Toggle>("Flash");
			if (front && flashToggle && flashToggle.isOn)
				ToggleTorch();
		}
    }

    public void ToggleExtendedTracking()
    {
        Toggle extTrackingToggle = FindUISelectableWithText<Toggle>("Extended");
        if (extTrackingToggle && mTrackableSettings)
            mTrackableSettings.SwitchExtendedTracking(extTrackingToggle.isOn);
    }

    public void ActivateDataset(string datasetName)
    {
        if (mTrackableSettings)
            mTrackableSettings.ActivateDataSet(datasetName);
    }

    public void UpdateUI()
    {
        Toggle extTrackingToggle = FindUISelectableWithText<Toggle>("Extended");
        if (extTrackingToggle && mTrackableSettings) 
            extTrackingToggle.isOn = mTrackableSettings.IsExtendedTrackingEnabled();

        Toggle flashToggle = FindUISelectableWithText<Toggle>("Flash");
        if (flashToggle && mCamSettings)
            flashToggle.isOn = mCamSettings.IsFlashTorchEnabled();

        Toggle autofocusToggle = FindUISelectableWithText<Toggle>("Autofocus");
        if (autofocusToggle && mCamSettings) 
            autofocusToggle.isOn = mCamSettings.IsAutofocusEnabled();

        Toggle frontCamToggle = FindUISelectableWithText<Toggle>("Front");
        if (frontCamToggle && mCamSettings)
            frontCamToggle.isOn = mCamSettings.IsFrontCameraActive();

        Toggle rearCamToggle = FindUISelectableWithText<Toggle>("Rear");
        if (rearCamToggle && mCamSettings)
            rearCamToggle.isOn = !mCamSettings.IsFrontCameraActive();
        
        Toggle stonesAndChipsToggle = FindUISelectableWithText<Toggle>("Stones");
        Toggle tarmacToggle = FindUISelectableWithText<Toggle>("Tarmac");

        if (mTrackableSettings)
        {
            if (stonesAndChipsToggle && stonesAndChipsToggle.gameObject.activeInHierarchy)
                stonesAndChipsToggle.isOn = mTrackableSettings.GetActiveDatasetName().Contains("Stones");

            if (tarmacToggle && mTrackableSettings && tarmacToggle.gameObject.activeInHierarchy)
                tarmacToggle.isOn = mTrackableSettings.GetActiveDatasetName().Contains("Tarmac");
        }        
    }

    public void CloseMenu()
    {
        if (mMenuAnim)
            mMenuAnim.Hide();
    }
    #endregion //PUBLIC_METHODS


    #region PROTECTED_METHODS
    protected T FindUISelectableWithText<T>(string text) where T : UnityEngine.UI.Selectable
    {
        T[] uiElements = GetComponentsInChildren<T>(true);
        foreach (var uielem in uiElements)
        {
            string childText = uielem.GetComponentInChildren<Text>().text;
            if (childText.Contains(text))
                return uielem;
        }
        return null;
    }
    #endregion //PROTECTED_METHODS

    #region PRIVATE_METHODS
    private void OnPaused(bool paused)
    {
        bool appResumed = !paused;
        if (appResumed)
        {
            // The flash torch is switched off by the OS automatically when app is paused.
            // On resume, update torch UI toggle to match torch status.
            Toggle flashToggle = FindUISelectableWithText<Toggle>("Flash");

            if (flashToggle != null)
                flashToggle.isOn = mCamSettings.IsFlashTorchEnabled();
        }
    }
    #endregion //PRIVATE_METHODS

}
