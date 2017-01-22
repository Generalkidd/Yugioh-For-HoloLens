/*===============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.
 
Copyright (c) 2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
===============================================================================*/
using UnityEngine;
using System.Collections;
using Vuforia;

public class InitErrorHandler : MonoBehaviour
{
    #region PUBLIC_MEMBER_VARIABLES
    public UnityEngine.UI.Text errorText;
    #endregion //PUBLIC_MEMBER_VARABLES


    #region PRIVATE_MEMBER_VARIABLES
    private Canvas errorCanvas;
    private string key;
    #endregion //PRIVATE_MEMBER_VARIABLES


    #region MONOBEHAVIOUR_METHODS
    void Awake () 
    {
        // Get the UI Canvas that contains (parent of) the error text box
        if (errorText)
        {
            errorCanvas = errorText.GetComponentsInParent<Canvas>(true)[0];
        }

        VuforiaRuntime.Instance.RegisterVuforiaInitErrorCallback(OnInitError);
    }
    #endregion //MONOBEHAVIOUR_METHODS


    #region PRIVATE_METHODS
    private void OnInitError(VuforiaUnity.InitError error)
    {
        if (error != VuforiaUnity.InitError.INIT_SUCCESS)
        {
            ShowErrorMessage(error);
        }
    }

    private void ShowErrorMessage(VuforiaUnity.InitError errorCode)
    {
        switch (errorCode)
        {
            case VuforiaUnity.InitError.INIT_EXTERNAL_DEVICE_NOT_DETECTED:
                errorText.text =
                    "Failed to initialize Vuforia because this " +
                    "device is not docked with required external hardware.";
                break;
            case VuforiaUnity.InitError.INIT_LICENSE_ERROR_MISSING_KEY:
                // Key length is 0
                errorText.text =
                    "Vuforia App Key is missing. \n" +
                    "Please get a valid key, by logging into your account at " +
                    "developer.vuforia.com and creating a new project.";
                break;
            case VuforiaUnity.InitError.INIT_LICENSE_ERROR_INVALID_KEY:
                // Key is not a valid Vuforia key
                key = VuforiaConfiguration.Instance.Vuforia.LicenseKey;
                if (key.Length > 10)
                {
                    errorText.text =
                    "Vuforia App key is invalid. \n" +
                    "Please get a valid key, by logging into your account at " +
                    "developer.vuforia.com and creating a new project. \n\n" +
                    "Your current key is <color=red>" + key.Length + "</color> characters in length. " +
                    "It begins with <color=red>" + key.Substring(0, 5) + "</color> " +
                    "and ends with <color=red>" + key.Substring(5, 5) + "</color>.";
                } else
                {
                    errorText.text =
                    "Vuforia App key is invalid. \n" +
                    "Please get a valid key, by logging into your account at " +
                    "developer.vuforia.com and creating a new project. \n\n" +
                    "Your current key is <color=red>" + key.Length + "</color> characters in length. \n" +
                    "The key is: <color=red>" + key + "</color>.";
                }
                break;
            case VuforiaUnity.InitError.INIT_LICENSE_ERROR_NO_NETWORK_TRANSIENT:
                errorText.text =
                    "Unable to contact server. Please try again later.";
                break;
            case VuforiaUnity.InitError.INIT_LICENSE_ERROR_NO_NETWORK_PERMANENT:
                errorText.text =
                    "No network available. Please make sure you are connected to the internet.";
                break;
            case VuforiaUnity.InitError.INIT_LICENSE_ERROR_CANCELED_KEY:
                key = VuforiaConfiguration.Instance.Vuforia.LicenseKey;
                errorText.text =
                    "This App license key has been cancelled and may no longer be used. " +
                    "Please get a new license key. \n\n" +
                    "Your current key is <color=red>" + key.Length + "</color> characters in length. " +
                    "It begins with <color=red>" + key.Substring(0, 5) + "</color> " +
                    "and ends with <color=red>" + key.Substring(5, 5) + "</color>.";
                break;
            case VuforiaUnity.InitError.INIT_LICENSE_ERROR_PRODUCT_TYPE_MISMATCH:
                key = VuforiaConfiguration.Instance.Vuforia.LicenseKey;
                errorText.text =
                    "Vuforia App key is not valid for this product. Please get a valid key, " +
                    "by logging into your account at developer.vuforia.com and choosing the " +
                    "right product type during project creation. \n\n" +
                    "Your current key is <color=red>" + key.Length + "</color> characters in length. " +
                    "It begins with <color=red>" + key.Substring(0, 5) + "</color> " +
                    "and ends with <color=red>" + key.Substring(5, 5) + "</color>. \n\n" +
                    "Note that Universal Windows Platform (UWP) apps require " +
                    "a license key created on or after August 9th, 2016.";
                break;
#if (UNITY_IPHONE || UNITY_IOS)
                case VuforiaUnity.InitError.INIT_NO_CAMERA_ACCESS:
                    errorText.text = 
                        "Camera Access was denied to this App. \n" + 
                        "When running on iOS8 devices, \n" + 
                        "users must explicitly allow the App to access the camera.\n" + 
                        "To restore camera access on your device, go to: \n" + 
                        "Settings > Privacy > Camera > [This App Name] and switch it ON.";
                    break;
#endif
            case VuforiaUnity.InitError.INIT_DEVICE_NOT_SUPPORTED:
                errorText.text =
                    "Failed to initialize Vuforia because this device is not supported.";
                break;
            case VuforiaUnity.InitError.INIT_ERROR:
                errorText.text = "Failed to initialize Vuforia.";
                break;
        }

        Debug.Log(errorCode);
        Debug.Log(errorText.text);

        if (errorCanvas)
        {
            // Show the error message UI canvas
            errorCanvas.transform.parent.position = Vector3.zero;
            errorCanvas.gameObject.SetActive(true);
            errorCanvas.enabled = true;
        }
    }
    #endregion //PRIVATE_METHODS


    #region PUBLIC_METHODS
    public void OnErrorDialogClose()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion //PUBLIC_METHODS
}
