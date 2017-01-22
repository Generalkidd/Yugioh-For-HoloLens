/*===============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/


using System;
using UnityEngine;

namespace Vuforia
{
    static class VuforiaRuntimeInitialization
    {
        #region PRIVATE_METHODS

        /// <summary>
        /// Initialize platform before first scene is loaded
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitPlatform()
        {
            VuforiaUnity.SetStandardInitializationParameters();
            VuforiaRuntime.Instance.InitPlatform(CreateUnityPlayer());
        }

        /// <summary>
        /// Initialize Vuforia after first scene is loaded to make
        /// sure that the platform has already been initialized.
        /// This is only executed if delayed initialization is disabled
        /// in the Vuforia configuration.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitVuforia()
        {
            if(!VuforiaConfiguration.Instance.Vuforia.DelayedInitialization)
            {
                VuforiaRuntime.Instance.InitVuforia();
            }
        }

        /// <summary>
        /// Create platform-specific unity player
        /// </summary>
        private static IUnityPlayer CreateUnityPlayer()
        {
            IUnityPlayer unityPlayer = new NullUnityPlayer();

            // instantiate the correct UnityPlayer for the current platform
            if (Application.platform == RuntimePlatform.Android)
                unityPlayer = new AndroidUnityPlayer();
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                unityPlayer = new IOSUnityPlayer();
            else if (VuforiaRuntimeUtilities.IsPlayMode())
                unityPlayer = new PlayModeUnityPlayer();
            else if (VuforiaRuntimeUtilities.IsWSARuntime())
            {
                unityPlayer = new WSAUnityPlayer();
            }

            return unityPlayer;
        }

        #endregion // PRIVATE_METHODS

    }
}
