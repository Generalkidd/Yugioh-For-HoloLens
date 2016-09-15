/*===============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AsyncSceneLoader : MonoBehaviour
{
    #region PUBLIC_MEMBERS
	public float loadingDelay = 5.0F;
    #endregion //PUBLIC_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {	
        StartCoroutine(LoadNextSceneAfter(loadingDelay));
    }
    #endregion //MONOBEHAVIOUR_METHODS
    
    
    #region PRIVATE_METHODS
    private IEnumerator LoadNextSceneAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        Application.LoadLevel(Application.loadedLevel+1);
#else
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex+1);
#endif
    }
    #endregion //PRIVATE_METHODS
}
