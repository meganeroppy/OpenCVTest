﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using OpenCVForUnity;
using DlibFaceLandmarkDetector;

namespace DlibFaceLandmarkDetectorExample
{
    /// <summary>
    /// Noise Filter WebCamTexture Example
    /// </summary>
    [RequireComponent (typeof(WebCamTextureToMatHelper))]
    public class NoiseFilterWebCamTextureExample : MonoBehaviour
    {
        public enum FilterMode : int
        {
            None,
            LowPassFilter,
            KalmanFilter,
            OpticalFlowFilter,
            OFAndLPFilter,
        }

        /// <summary>
        /// Determines if is debug mode.
        /// </summary>
        public bool isDebugMode = false;

        [Space(10)]

        /// <summary>
        /// The filter mode dropdown.
        /// </summary>
        public Dropdown filterModeDropdown;

        /// <summary>
        /// The filter Mmode.
        /// </summary>
        public FilterMode filterMode = FilterMode.OFAndLPFilter;

        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// The webcam texture to mat helper.
        /// </summary>
        WebCamTextureToMatHelper webCamTextureToMatHelper;

        /// <summary>
        /// The face landmark detector.
        /// </summary>
        FaceLandmarkDetector faceLandmarkDetector;

        /// <summary>
        /// The FPS monitor.
        /// </summary>
        FpsMonitor fpsMonitor;

        /// <summary>
        /// The dlib shape predictor file name.
        /// </summary>
        string dlibShapePredictorFileName = "sp_human_face_68.dat";

        /// <summary>
        /// The dlib shape predictor file path.
        /// </summary>
        string dlibShapePredictorFilePath;

        /// <summary>
        /// The mean points filter.
        /// </summary>
        LowPassPointsFilter lowPassFilter;

        /// <summary>
        /// The kanlam filter points filter.
        /// </summary>
        KFPointsFilter kalmanFilter;

        /// <summary>
        /// The optical flow points filter.
        /// </summary>
        OFPointsFilter opticalFlowFilter;

        List<Vector2> lowPassFilteredPoints = new List<Vector2> ();
        List<Vector2> kalmanFilteredPoints = new List<Vector2> ();
        List<Vector2> opticalFlowFilteredPoints = new List<Vector2> ();
        List<Vector2> ofAndLPFilteredPoints = new List<Vector2> ();

        /// <summary>
        /// The number of skipped frames.
        /// </summary>
        int skippedFrames;

        /// <summary>
        /// The number of maximum allowed skipped frames.
        /// </summary>
        const int maximumAllowedSkippedFrames = 4;

        #if UNITY_WEBGL && !UNITY_EDITOR
        Stack<IEnumerator> coroutines = new Stack<IEnumerator> ();
        #endif

        #if UNITY_ANDROID && !UNITY_EDITOR
        float rearCameraRequestedFPS;
        #endif

        // Use this for initialization
        void Start ()
        {
            fpsMonitor = GetComponent<FpsMonitor> ();

            filterModeDropdown.value = (int)filterMode;

            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper> ();

            dlibShapePredictorFileName = DlibFaceLandmarkDetectorExample.dlibShapePredictorFileName;
            #if UNITY_WEBGL && !UNITY_EDITOR
            var getFilePath_Coroutine = DlibFaceLandmarkDetector.Utils.getFilePathAsync (dlibShapePredictorFileName, (result) => {
                coroutines.Clear ();

                dlibShapePredictorFilePath = result;
                Run ();
            });
            coroutines.Push (getFilePath_Coroutine);
            StartCoroutine (getFilePath_Coroutine);
            #else
            dlibShapePredictorFilePath = DlibFaceLandmarkDetector.Utils.getFilePath (dlibShapePredictorFileName);
            Run ();
            #endif
        }

        private void Run ()
        {
            faceLandmarkDetector = new FaceLandmarkDetector (dlibShapePredictorFilePath);

            lowPassFilter = new LowPassPointsFilter ((int)faceLandmarkDetector.GetShapePredictorNumParts());
            kalmanFilter = new KFPointsFilter ((int)faceLandmarkDetector.GetShapePredictorNumParts());
            opticalFlowFilter = new OFPointsFilter ((int)faceLandmarkDetector.GetShapePredictorNumParts());

            #if UNITY_ANDROID && !UNITY_EDITOR
            // Set the requestedFPS parameter to avoid the problem of the WebCamTexture image becoming low light on some Android devices. (Pixel, pixel 2)
            // https://forum.unity.com/threads/android-webcamtexture-in-low-light-only-some-models.520656/
            // https://forum.unity.com/threads/released-opencv-for-unity.277080/page-33#post-3445178
            rearCameraRequestedFPS = webCamTextureToMatHelper.requestedFPS;
            if (webCamTextureToMatHelper.requestedIsFrontFacing) {                
                webCamTextureToMatHelper.requestedFPS = 15;
                webCamTextureToMatHelper.Initialize ();
            } else {
                webCamTextureToMatHelper.Initialize ();
            }
            #else
            webCamTextureToMatHelper.Initialize ();
            #endif
        }

        /// <summary>
        /// Raises the web cam texture to mat helper initialized event.
        /// </summary>
        public void OnWebCamTextureToMatHelperInitialized ()
        {
            Debug.Log ("OnWebCamTextureToMatHelperInitialized");

            Mat webCamTextureMat = webCamTextureToMatHelper.GetMat ();

            texture = new Texture2D (webCamTextureMat.cols (), webCamTextureMat.rows (), TextureFormat.RGBA32, false);
            gameObject.GetComponent<Renderer> ().material.mainTexture = texture;

            gameObject.transform.localScale = new Vector3 (webCamTextureMat.cols (), webCamTextureMat.rows (), 1);
            Debug.Log ("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            if (fpsMonitor != null){
                fpsMonitor.Add ("dlib shape predictor", dlibShapePredictorFileName);
                fpsMonitor.Add ("width", webCamTextureToMatHelper.GetWidth().ToString());
                fpsMonitor.Add ("height", webCamTextureToMatHelper.GetHeight().ToString());
                fpsMonitor.Add ("orientation", Screen.orientation.ToString());
            }

                                    
            float width = webCamTextureMat.width ();
            float height = webCamTextureMat.height ();
                                    
            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale) {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            } else {
                Camera.main.orthographicSize = height / 2;
            }

            if (lowPassFilter != null)
                lowPassFilter.Reset ();
            if (kalmanFilter != null)
                kalmanFilter.Reset ();
            if (opticalFlowFilter != null)
                opticalFlowFilter.Reset ();
            skippedFrames = 0;
        }

        /// <summary>
        /// Raises the web cam texture to mat helper disposed event.
        /// </summary>
        public void OnWebCamTextureToMatHelperDisposed ()
        {
            Debug.Log ("OnWebCamTextureToMatHelperDisposed");

            if (texture != null) {
                Texture2D.Destroy(texture);
                texture = null;
            }
        }

        /// <summary>
        /// Raises the web cam texture to mat helper error occurred event.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public void OnWebCamTextureToMatHelperErrorOccurred (WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log ("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
        }

        // Update is called once per frame
        void Update ()
        {
            if (webCamTextureToMatHelper.IsPlaying () && webCamTextureToMatHelper.DidUpdateThisFrame ()) {

                Mat rgbaMat = webCamTextureToMatHelper.GetMat ();

                OpenCVForUnityUtils.SetImage (faceLandmarkDetector, rgbaMat);

                //detect face rects
                List<UnityEngine.Rect> detectResult = faceLandmarkDetector.Detect ();

                UnityEngine.Rect rect = new UnityEngine.Rect();
                List<Vector2> points = null;
                bool shouldResetfilter = false;
                if (detectResult.Count > 0) {

                    rect = detectResult [0];

                    //detect landmark points
                    points = faceLandmarkDetector.DetectLandmark (rect);

                    skippedFrames = 0;
                } else {
                    skippedFrames++;
                    if (skippedFrames == maximumAllowedSkippedFrames) {
                        shouldResetfilter = true;
                    }
                }

                switch(filterMode)
                {
                default:
                case FilterMode.None:
                    break;
                case FilterMode.LowPassFilter:
                    if (shouldResetfilter)
                        lowPassFilter.Reset ();
                    lowPassFilter.Process (rgbaMat, points, lowPassFilteredPoints, isDebugMode);
                    break;
                case FilterMode.KalmanFilter:
                    if (shouldResetfilter)
                        kalmanFilter.Reset ();
                    kalmanFilter.Process (rgbaMat, points, kalmanFilteredPoints, isDebugMode);
                    break;
                case FilterMode.OpticalFlowFilter:
                    if (shouldResetfilter)
                        opticalFlowFilter.Reset ();
                    opticalFlowFilter.Process (rgbaMat, points, opticalFlowFilteredPoints, isDebugMode);
                    break;
                case FilterMode.OFAndLPFilter:
                    if (shouldResetfilter) {
                        opticalFlowFilter.Reset ();
                        lowPassFilter.Reset ();
                    }

                    opticalFlowFilter.Process (rgbaMat, points, points, false);
                    lowPassFilter.Process (rgbaMat, points, ofAndLPFilteredPoints, isDebugMode);
                    break;
                }


                if (points != null && !isDebugMode) {
                    // draw raw landmark points.
                    OpenCVForUnityUtils.DrawFaceLandmark (rgbaMat, points, new Scalar (0, 255, 0, 255), 2);
                }

                // draw face rect.
//                OpenCVForUnityUtils.DrawFaceRect (rgbaMat, rect, new Scalar (255, 0, 0, 255), 2);

                // draw filtered lam points. 
                if (points != null && !isDebugMode) {
                    switch (filterMode) {
                    default:
                    case FilterMode.None:
                        break;
                    case FilterMode.LowPassFilter:
                        OpenCVForUnityUtils.DrawFaceLandmark (rgbaMat, lowPassFilteredPoints, new Scalar (0, 255, 255, 255), 2);
                        break;
                    case FilterMode.KalmanFilter:
                        OpenCVForUnityUtils.DrawFaceLandmark (rgbaMat, kalmanFilteredPoints, new Scalar (0, 0, 255, 255), 2);
                        break;
                    case FilterMode.OpticalFlowFilter:
                        OpenCVForUnityUtils.DrawFaceLandmark (rgbaMat, opticalFlowFilteredPoints, new Scalar (255, 0, 0, 255), 2);
                        break;
                    case FilterMode.OFAndLPFilter:
                        OpenCVForUnityUtils.DrawFaceLandmark (rgbaMat, ofAndLPFilteredPoints, new Scalar (255, 0, 255, 255), 2);
                        break;
                    }
                }


                //Imgproc.putText (rgbaMat, "W:" + rgbaMat.width () + " H:" + rgbaMat.height () + " SO:" + Screen.orientation, new Point (5, rgbaMat.rows () - 10), Core.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255, 255), 1, Imgproc.LINE_AA, false);

                OpenCVForUnity.Utils.fastMatToTexture2D (rgbaMat, texture);
            }
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy ()
        {
            if (webCamTextureToMatHelper != null)
                webCamTextureToMatHelper.Dispose ();

            if (faceLandmarkDetector != null)
                faceLandmarkDetector.Dispose ();

            if (lowPassFilter != null)
                lowPassFilter.Dispose ();
            if (kalmanFilter != null)
                kalmanFilter.Dispose ();
            if (opticalFlowFilter != null)
                opticalFlowFilter.Dispose ();

            #if UNITY_WEBGL && !UNITY_EDITOR
            foreach (var coroutine in coroutines) {
                StopCoroutine (coroutine);
                ((IDisposable)coroutine).Dispose ();
            }
            #endif
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick ()
        {
            #if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene ("DlibFaceLandmarkDetectorExample");
            #else
            Application.LoadLevel ("DlibFaceLandmarkDetectorExample");
            #endif
        }

        /// <summary>
        /// Raises the play button click event.
        /// </summary>
        public void OnPlayButtonClick ()
        {
            webCamTextureToMatHelper.Play ();
        }

        /// <summary>
        /// Raises the pause button click event.
        /// </summary>
        public void OnPauseButtonCkick ()
        {
            webCamTextureToMatHelper.Pause ();
        }

        /// <summary>
        /// Raises the stop button click event.
        /// </summary>
        public void OnStopButtonClick ()
        {
            webCamTextureToMatHelper.Stop ();
        }

        /// <summary>
        /// Raises the change camera button click event.
        /// </summary>
        public void OnChangeCameraButtonClick ()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            if (!webCamTextureToMatHelper.IsFrontFacing ()) {
                rearCameraRequestedFPS = webCamTextureToMatHelper.requestedFPS;
                webCamTextureToMatHelper.Initialize (!webCamTextureToMatHelper.IsFrontFacing (), 15, webCamTextureToMatHelper.rotate90Degree);
            } else {                
                webCamTextureToMatHelper.Initialize (!webCamTextureToMatHelper.IsFrontFacing (), rearCameraRequestedFPS, webCamTextureToMatHelper.rotate90Degree);
            }
            #else
            webCamTextureToMatHelper.requestedIsFrontFacing = !webCamTextureToMatHelper.IsFrontFacing ();
            #endif
        }

        /// <summary>
        /// Raises the filter mode dropdown value changed event.
        /// </summary>
        public void OnFilterModeDropdownValueChanged (int result)
        {
            if ((int)filterMode != result) {
                filterMode = (FilterMode)result;

                if (lowPassFilter != null)
                    lowPassFilter.Reset ();
                if (kalmanFilter != null)
                    kalmanFilter.Reset ();
                if (opticalFlowFilter != null)
                    opticalFlowFilter.Reset ();
                skippedFrames = 0;
            }
        }
    }
}