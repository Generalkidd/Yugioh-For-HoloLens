/*==============================================================================
Copyright (c) 2013-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
#if ENABLE_HOLOLENS_MODULE_API
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
#if ENABLE_HOLOLENS_MODULE_API
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
                    // unregister for callbacks first
                    wa.OnTrackingChanged -= OnWorldAnchorTrackingChanged;
                    GameObject.DestroyImmediate(wa);
                }
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
#endif

        private ScreenOrientation mScreenOrientation = ScreenOrientation.Unknown;
        private static string UNITY_HOLOLENS_IDENTIFIER = "HoloLens";

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
        /// Initializes Vuforia; called from Start
        /// </summary>
        public VuforiaUnity.InitError Start(string licenseKey)
        {
            int errorCode = initVuforiaWSA(licenseKey);
            if (errorCode >= 0)
            {
                InitializeSurface();

#if ENABLE_HOLOLENS_MODULE_API
                // This determines if we are starting on a holographic device
                if (UnityEngine.VR.VRSettings.loadedDeviceName.Equals(UNITY_HOLOLENS_IDENTIFIER)
                    && UnityEngine.VR.VRDevice.isPresent)
                {
                    // set the focus point setter implementation
                    VuforiaUnity.SetHoloLensApiAbstraction(new HoloLensApiImplementation());

                    Debug.Log("Detected Holographic Device");
                    if (!VuforiaUnity.SetHoloLensWorldCoordinateSystem(WorldManager.GetNativeISpatialCoordinateSystemPtr()))
                    {
                        return VuforiaUnity.InitError.INIT_ERROR;
                    }
                }
#endif
            }
            return (VuforiaUnity.InitError)errorCode;
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
