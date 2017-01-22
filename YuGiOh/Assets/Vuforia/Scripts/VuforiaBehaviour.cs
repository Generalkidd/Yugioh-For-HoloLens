/*==============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// The VuforiaBehaviour class handles tracking and triggers native video
    /// background rendering. The class updates all Trackables in the scene.
    /// </summary>
    public class VuforiaBehaviour : VuforiaAbstractBehaviour
    {
        protected override void Awake()
        {
            AddOSSpecificExternalDatasetSearchDirs();

            gameObject.AddComponent<ComponentFactoryStarterBehaviour>();

            base.Awake();
        }

        private static VuforiaBehaviour mVuforiaBehaviour= null;

        /// <summary>
        /// A simple static singleton getter to the VuforiaBehaviour (if present in the scene)
        /// Will return null if no VuforiaBehaviour has been instantiated in the scene.
        /// </summary>
        public static VuforiaBehaviour Instance
        {
            get
            {
                if (mVuforiaBehaviour == null)
                    mVuforiaBehaviour = FindObjectOfType<VuforiaBehaviour>();

                return mVuforiaBehaviour;
            }
        }



        /// <summary>
        /// This method inserts new dataset search roots for datasets defined in StreamingAssets/QCAR.  This may
        /// be used to streamline the "Split Application Binary" Unity feature under the Android plugin.  This method is 
        /// called before the datasets are loaded in the Start()-method.
        /// </summary>
        private void AddOSSpecificExternalDatasetSearchDirs()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                var databaseLoader = DatabaseLoadARController.Instance;

                // Get the external storage directory
                AndroidJavaClass jclassEnvironment = new AndroidJavaClass("android.os.Environment");
                AndroidJavaObject jobjFile = jclassEnvironment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory");
                string externalStorageDirectory = jobjFile.Call<string>("getAbsolutePath");

                // Get the package name
                AndroidJavaObject jobjActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                string packageName = jobjActivity.Call<string>("getPackageName");

                // Add some best practice search directories
                //
                // Assumes just Vufroria datasets extracted to the files directory
                databaseLoader.AddExternalDatasetSearchDir(externalStorageDirectory + "/Android/data/" + packageName + "/files/");

                // Assume entire StreamingAssets dir is extracted here and our datasets are in the "Vuforia" directory
                databaseLoader.AddExternalDatasetSearchDir(externalStorageDirectory + "/Android/data/" + packageName + "/files/Vuforia/");

                // Assume entire StreamingAssets dir is extracted here and our datasets are in the "QCAR" directory
                databaseLoader.AddExternalDatasetSearchDir(externalStorageDirectory + "/Android/data/" + packageName + "/files/QCAR/");
            }
#endif //UNITY_ANDROID
        }
    }
}
