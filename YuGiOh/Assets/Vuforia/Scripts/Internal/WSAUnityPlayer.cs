/*==============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
==============================================================================*/

#if ENABLE_HOLOLENS_MODULE_API || UNITY_5_5_OR_NEWER
#if UNITY_WSA_10_0
#define HOLOLENS_API_AVAILABLE
#endif
#endif

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
#if HOLOLENS_API_AVAILABLE
using UnityEngine.VR.WSA;
#endif


namespace Vuforia
{
    using TrackableIdPair = VuforiaManager.TrackableIdPair;

    /// <summary>
    /// This class encapsulates functionality to detect various surface events
    /// (size, orientation changed) and delegate this to native.
    /// These are used by Unity Extension code and should usually not be called by app code.
    /// </summary>
    class WSAUnityPlayer : IUnityPlayer
    {
#if HOLOLENS_API_AVAILABLE
        #region NESTED

        private class HoloLensApiImplementation : IHoloLensApiAbstraction
        {
            private Dictionary<TrackableIdPair, WorldAnchor> mWorldAnchors = 
                new Dictionary<TrackableIdPair, WorldAnchor>();
            private Action<TrackableIdPair, bool> mHoloLensTrackingCallback = null;
 
            public void SetFocusPoint(Vector3 point, Vector3 normal)
            {
                // use HL specific API to set the focus point
                HolographicSettings.SetFocusPointForFrame(point, normal);
            }

            public void SetWorldAnchor(TrackableBehaviour trackableBehaviour, TrackableIdPair trackableID)
            {
                // add a world anchor to the given trackablebehaviour
                WorldAnchor wa = trackableBehaviour.gameObject.AddComponent<WorldAnchor>();
                mWorldAnchors[trackableID] = wa;
                // register for callbacks
                wa.OnTrackingChanged += OnWorldAnchorTrackingChanged;
            }

            public void DeleteWorldAnchor(TrackableIdPair trackableID)
            {
                // delete an existing world anchor
                if (mWorldAnchors.ContainsKey(trackableID))
                {
                    WorldAnchor wa = mWorldAnchors[trackableID];
                    mWorldAnchors.Remove(trackableID);

                    InternalDeleteWA(wa);
                }
            }

            public void DeleteWorldAnchor(TrackableBehaviour trackableBehaviour)
            {
                WorldAnchor wa = trackableBehaviour.GetComponent<WorldAnchor>();
                if (wa != null)
                {
                    if (mWorldAnchors.ContainsValue(wa))
                    {
                        // find all occurrences of that world anchor and remove them from the dict:
                        List<TrackableIdPair> idsToRemove = new List<TrackableIdPair>();
                        foreach (KeyValuePair<TrackableIdPair, WorldAnchor> kvp in mWorldAnchors)
                            if (kvp.Value == wa)
                                idsToRemove.Add(kvp.Key);

                        foreach (TrackableIdPair trackableIdPair in idsToRemove)
                            mWorldAnchors.Remove(trackableIdPair);
                    }

                    InternalDeleteWA(wa);
                }
            }

            private void InternalDeleteWA(WorldAnchor wa)
            {
                    // unregister for callbacks first
                    wa.OnTrackingChanged -= OnWorldAnchorTrackingChanged;
                    GameObject.DestroyImmediate(wa);
            }

            public void SetSpatialAnchorTrackingCallback(Action<TrackableIdPair, bool> trackingCallback)
            {
                mHoloLensTrackingCallback = trackingCallback;
            }

            private void OnWorldAnchorTrackingChanged(WorldAnchor wa, bool tracked)
            {
                if (mHoloLensTrackingCallback != null)
                {
                    // translate from world anchor to trackable behaviour
                    foreach (KeyValuePair<TrackableIdPair, WorldAnchor> worldAnchor in mWorldAnchors)
                    {
                        if (worldAnchor.Value == wa)
                        {
                            mHoloLensTrackingCallback(worldAnchor.Key, tracked);
                        }
                    }
                }
            }
        }

        #endregion // NESTED

        private static string UNITY_HOLOLENS_IDENTIFIER = "HoloLens";
#endif

        private ScreenOrientation mScreenOrientation = ScreenOrientation.Unknown;

        /// <summary>
        /// Loads native plugin libraries on platforms where this is explicitly required.
        /// </summary>
        public void LoadNativeLibraries()
        {
        }

        /// <summary>
        /// Initialized platform specific settings
        /// </summary>
        public void InitializePlatform()
        {
            setPlatFormNative();
        }

        /// <summary>
        /// Initializes Vuforia
        /// </summary>
        public VuforiaUnity.InitError InitializeVuforia(string licenseKey)
        {
            int errorCode = initVuforiaWSA(licenseKey);
            if (errorCode >= 0)
            {
                InitializeSurface();

#if HOLOLENS_API_AVAILABLE
                // This determines if we are starting on a holographic device
                if (UnityEngine.VR.VRSettings.loadedDeviceName.Equals(UNITY_HOLOLENS_IDENTIFIER)
                    && UnityEngine.VR.VRDevice.isPresent)
                {
                    // set the focus point setter implementation
                    VuforiaUnity.SetHoloLensApiAbstraction(new HoloLensApiImplementation());

                    Debug.Log("Detected Holographic Device");
                }
#endif
            }
            return (VuforiaUnity.InitError)errorCode;
        }

        /// <summary>
        /// Called on start each time a new scene is loaded
        /// </summary>
        public void StartScene()
        {
#if HOLOLENS_API_AVAILABLE
                // This determines if we are starting on a holographic device
                if (UnityEngine.VR.VRSettings.loadedDeviceName.Equals(UNITY_HOLOLENS_IDENTIFIER)
                    && UnityEngine.VR.VRDevice.isPresent)
                {
                    if (!VuforiaUnity.SetHolographicAppCoordinateSystem(WorldManager.GetNativeISpatialCoordinateSystemPtr()))
                    {
                        Debug.LogError("Failed to set holographic coordinate system pointer!");
                    }
                }
#endif
        }

        /// <summary>
        /// Called from Update, checks for various life cycle events that need to be forwarded
        /// to Vuforia, e.g. orientation changes
        /// </summary>
        public void Update()
        {
            if (SurfaceUtilities.HasSurfaceBeenRecreated())
            {
                InitializeSurface();
            }
            else
            {
                // if Unity reports that the orientation has changed, set it correctly in native
                ScreenOrientation currentOrientation = GetActualScreenOrientation();

                if (currentOrientation != mScreenOrientation)
                    SetUnityScreenOrientation();
            }

        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Pauses Vuforia
        /// </summary>
        public void OnPause()
        {
            VuforiaUnity.OnPause();
        }

        /// <summary>
        /// Resumes Vuforia
        /// </summary>
        public void OnResume()
        {
            VuforiaUnity.OnResume();
        }

        /// <summary>
        /// Deinitializes Vuforia
        /// </summary>
        public void OnDestroy()
        {
            VuforiaUnity.Deinit();
        }


        private void InitializeSurface()
        {
            SurfaceUtilities.OnSurfaceCreated();

            SetUnityScreenOrientation();
        }

        private void SetUnityScreenOrientation()
        {
            mScreenOrientation = GetActualScreenOrientation();

            SurfaceUtilities.SetSurfaceOrientation(mScreenOrientation);

            // set the native orientation (only required on iOS and WSA)
            setSurfaceOrientationWSA((int) mScreenOrientation);
        }        
 
        /// <summary>
        /// There is a known Unity issue for Windows 10 UWP apps where the initial orientation is wrongly
        /// reported as AutoRotation instead of the actual orientation.
        /// This method tries to infer the screen orientation from the device orientation if this is the case.
        /// </summary>
        /// <returns></returns>
        private ScreenOrientation GetActualScreenOrientation()
        {
            ScreenOrientation orientation = Screen.orientation;

            if (orientation == ScreenOrientation.AutoRotation)
            {
                DeviceOrientation devOrientation = Input.deviceOrientation;

                switch (devOrientation)
                {
                    case DeviceOrientation.LandscapeLeft:
                        orientation = ScreenOrientation.LandscapeLeft;
                        break;

                    case DeviceOrientation.LandscapeRight:
                        orientation = ScreenOrientation.LandscapeRight;
                        break;

                    case DeviceOrientation.Portrait:
                        orientation = ScreenOrientation.Portrait;
                        break;

                    case DeviceOrientation.PortraitUpsideDown:
                        orientation = ScreenOrientation.PortraitUpsideDown;
                        break;

                    default:
                        // fallback: Landscape Left
                        orientation = ScreenOrientation.LandscapeLeft;
                        break;
                }
            }

            return orientation;
        }

        [DllImport("VuforiaWrapper")]
        private static extern void setPlatFormNative();

        [DllImport("VuforiaWrapper")]
        private static extern int initVuforiaWSA(string licenseKey);

        [DllImport("VuforiaWrapper")]
        private static extern void setSurfaceOrientationWSA(int screenOrientation);
    }
}
