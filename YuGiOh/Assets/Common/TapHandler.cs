/*===============================================================================
Copyright (c) 2015-2016 PTC Inc. All Rights Reserved.
 
Copyright (c) 2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using System.Collections;

public class TapHandler : MonoBehaviour
{
    #region PRIVATE_MEMBERS
    private const float DOUBLE_TAP_MAX_DELAY = 0.5f;//seconds
    private float mTimeSinceLastTap = 0;
    private MenuAnimator mMenuAnim = null;
    #endregion //PRIVATE_MEMBERS


    #region PROTECTED_MEMBERS
    protected int mTapCount = 0;
    #endregion //PROTECTED_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start() 
    {
        mTapCount = 0;
        mTimeSinceLastTap = 0;
        mMenuAnim = FindObjectOfType<MenuAnimator>();
    }

    void Update() 
    {
        if (mMenuAnim && mMenuAnim.IsVisible())
        {
            mTapCount = 0;
            mTimeSinceLastTap = 0;
        }
        else
        {
            HandleTap();
        }

#if UNITY_ANDROID
        // On Android, the Back button is mapped to the Esc key
        if (Input.GetKeyUp(KeyCode.Escape))
        {
#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
            Application.LoadLevel("Vuforia-1-About");   
#else // UNITY_5_3 or above
            UnityEngine.SceneManagement.SceneManager.LoadScene("Vuforia-1-About");
#endif
        }
#endif
    }
    #endregion //MONOBEHAVIOUR_METHODS


    #region PRIVATE_METHODS
    private void HandleTap()
    {
        if (mTapCount == 1)
        {
            mTimeSinceLastTap += Time.deltaTime;
            if (mTimeSinceLastTap > DOUBLE_TAP_MAX_DELAY)
            {
                // too late for double tap, 
                // we confirm it was a single tap
                OnSingleTapConfirmed();

                // reset touch count and timer
                mTapCount = 0;
                mTimeSinceLastTap = 0;
            }
        }
        else if (mTapCount == 2)
        {
            // we got a double tap
            OnDoubleTap();

            // reset touch count and timer
            mTimeSinceLastTap = 0;
            mTapCount = 0;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mTapCount++;
            if (mTapCount == 1)
            {
                OnSingleTap();
            }
        }
    }
    #endregion // PRIVATE_METHODS


    #region PROTECTED_METHODS
    /// <summary>
    /// This method can be overridden by custom (derived) TapHandler implementations,
    /// to perform special actions upon single tap.
    /// </summary>
    protected virtual void OnSingleTap() { }

    protected virtual void OnSingleTapConfirmed()
    {
        CameraSettings camSettings = GetComponentInChildren<CameraSettings>();
        if (camSettings)
        {
            camSettings.TriggerAutofocusEvent();
        }
    }

    protected virtual void OnDoubleTap()
    {
        if (mMenuAnim && !mMenuAnim.IsVisible())
        {
            mMenuAnim.Show();
        }
    }
    #endregion // PROTECTED_METHODS
}
