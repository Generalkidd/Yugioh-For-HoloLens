/*===============================================================================
Copyright (c) 2015-2016 PTC Inc. All Rights Reserved.
 
Copyright (c) 2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using System.Collections;

public class MenuAnimator : MonoBehaviour
{
    #region PRIVATE_MEMBERS
    private Vector3 mVisiblePos = Vector3.zero;
    private Vector3 mInvisiblePos = -Vector3.right * 2000;
    private float mVisibility = 0;
    private bool mVisible = false;
    private Canvas mCanvas = null;
    private MenuOptions mMenuOptions = null;
    #endregion //PRIVATE_MEMBERS


    #region PUBLIC_PROPERTIES
    [Range(0,1)]
    public float SlidingTime = 0.3f;// seconds
    #endregion //PUBLIC_PROPERTIES


    #region MONOBEHAVIOUR_METHODS
    void Start () 
    {
        mInvisiblePos = -Vector3.right * (2 * Screen.width);
        mVisibility = 0;
        mVisible = false;
        this.transform.position = mInvisiblePos;
        mCanvas = GetComponentsInChildren<Canvas>(true)[0];
        mMenuOptions = FindObjectOfType<MenuOptions>();
    }
    
    void Update () 
    {
        mInvisiblePos = -Vector3.right * Screen.width * 2;

        if (mVisible)
        {
            // Switch ON the UI Canvas.
            mCanvas.gameObject.SetActive(true);
            if (!mCanvas.enabled)
                mCanvas.enabled = true;

            if (mVisibility < 1)
            {
                mVisibility += Time.deltaTime / SlidingTime;
                mVisibility = Mathf.Clamp01(mVisibility);
                this.transform.position = Vector3.Slerp(mInvisiblePos, mVisiblePos, mVisibility);
            }
        }
        else
        {
            if (mVisibility > 0)
            {
                mVisibility -= Time.deltaTime / SlidingTime;
                mVisibility = Mathf.Clamp01(mVisibility);
                this.transform.position = Vector3.Slerp(mInvisiblePos, mVisiblePos, mVisibility);

                // Switch OFF the UI Canvas when the transition is done.
                mCanvas.gameObject.SetActive(false);
                if (mVisibility < 0.01f)
                {
                    if (mCanvas.enabled)
                        mCanvas.enabled = false;
                }
            }
            else
            {
                this.transform.position = mInvisiblePos;
            }
        }
    }
    #endregion //MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public void Show()
    {
        mVisible = true;
        if (mMenuOptions)
            mMenuOptions.UpdateUI();
    }

    public void Hide()
    {
        mVisible = false;
    }

    public bool IsVisible()
    {
        return mVisibility > 0.05f;
    }
    #endregion //PUBLIC_METHODS
}
