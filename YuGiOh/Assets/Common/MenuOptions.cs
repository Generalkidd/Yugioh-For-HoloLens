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
    }
    #endregion //MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public void ShowAboutPage()
    {
#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        Application.LoadLevel("Vuforia-1-About");
#else
        UnityEngine.SceneManagement.SceneManager.LoadScene("Vuforia-1-About");
#endif
    }

    public void ToggleAutofocus()
    {
        Toggle autofocusToggle = FindUISelectableWithText<Toggle>("Autofocus");
        if (autofocusToggle && mCamSettings)
            mCamSettings.SwitchAutofocus(autofocusToggle.isOn);
        
        CloseMenu();
    }

    public void ToggleTorch()
    {
        Toggle flashToggle = FindUISelectableWithText<Toggle>("Flash");
        if (flashToggle && mCamSettings) 
        {
            mCamSettings.SwitchFlashTorch (flashToggle.isOn);

            // Update UI toggle status (ON/OFF) in case the flash switch failed
            flashToggle.isOn = mCamSettings.IsFlashTorchEnabled();
        }
        CloseMenu();
    }

    public void SelectCamera(bool front)
    {
        if (mCamSettings)
            mCamSettings.SelectCamera(front ? CameraDevice.CameraDirection.CAMERA_FRONT : CameraDevice.CameraDirection.CAMERA_BACK);
        
        CloseMenu();
    }

    public void ToggleExtendedTracking()
    {
        Toggle extTrackingToggle = FindUISelectableWithText<Toggle>("Extended");
        if (extTrackingToggle && mTrackableSettings)
            mTrackableSettings.SwitchExtendedTracking(extTrackingToggle.isOn);
        
        CloseMenu();
    }

    public void ActivateDataset(string datasetName)
    {
        if (mTrackableSettings)
            mTrackableSettings.ActivateDataSet(datasetName);

        CloseMenu();
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
        if (stonesAndChipsToggle && mTrackableSettings)
            stonesAndChipsToggle.isOn = mTrackableSettings.GetActiveDatasetName().Contains("Stones");

        Toggle tarmacToggle = FindUISelectableWithText<Toggle>("Tarmac");
        if (tarmacToggle && mTrackableSettings)
            tarmacToggle.isOn = mTrackableSettings.GetActiveDatasetName().Contains("Tarmac");
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
        T[] uiElements = GetComponentsInChildren<T>();
        foreach (var uielem in uiElements)
        {
            string childText = uielem.GetComponentInChildren<Text>().text;
            if (childText.Contains(text))
                return uielem;
        }
        return null;
    }
    #endregion //PROTECTED_METHODS
}
