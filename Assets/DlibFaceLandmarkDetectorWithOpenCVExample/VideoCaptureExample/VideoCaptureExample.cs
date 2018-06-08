﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using OpenCVForUnity;
using DlibFaceLandmarkDetector;

namespace DlibFaceLandmarkDetectorExample
{
    /// <summary>
    /// VideoCapture Example
    /// An example of detecting face landmarks in VideoCapture images.
    /// </summary>
    public class VideoCaptureExample : MonoBehaviour
    {
        /// <summary>
        /// The video capture.
        /// </summary>
        VideoCapture capture;

        /// <summary>
        /// The rgb mat.
        /// </summary>
        Mat rgbMat;

        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;

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
        /// The couple_avi_filepath.
        /// </summary>
        string couple_avi_filepath;

        #if UNITY_WEBGL && !UNITY_EDITOR
        Stack<IEnumerator> coroutines = new Stack<IEnumerator> ();
        #endif

        // Use this for initialization
        void Start ()
        {
            fpsMonitor = GetComponent<FpsMonitor> ();

            dlibShapePredictorFileName = DlibFaceLandmarkDetectorExample.dlibShapePredictorFileName;
            #if UNITY_WEBGL && !UNITY_EDITOR
            var getFilePath_Coroutine = GetFilePath ();
            coroutines.Push (getFilePath_Coroutine);
            StartCoroutine (getFilePath_Coroutine);
            #else
            dlibShapePredictorFilePath = DlibFaceLandmarkDetector.Utils.getFilePath (dlibShapePredictorFileName);
            couple_avi_filepath = OpenCVForUnity.Utils.getFilePath ("couple.avi");
            Run ();
            #endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        private IEnumerator GetFilePath ()
        {
            var getFilePathAsync_dlibShapePredictorFilePath_Coroutine = DlibFaceLandmarkDetector.Utils.getFilePathAsync (dlibShapePredictorFileName, (result) => {
                    dlibShapePredictorFilePath = result;
            });
            coroutines.Push (getFilePathAsync_dlibShapePredictorFilePath_Coroutine);
            yield return StartCoroutine (getFilePathAsync_dlibShapePredictorFilePath_Coroutine);

            var getFilePathAsync_couple_avi_filepath_Coroutine = OpenCVForUnity.Utils.getFilePathAsync ("couple.avi", (result) => {
                couple_avi_filepath = result;
            });
            coroutines.Push (getFilePathAsync_couple_avi_filepath_Coroutine);
            yield return StartCoroutine (getFilePathAsync_couple_avi_filepath_Coroutine);

            coroutines.Clear ();

            Run ();
        }
        #endif
        
        private void Run ()
        {
            faceLandmarkDetector = new FaceLandmarkDetector (dlibShapePredictorFilePath);
            
            rgbMat = new Mat ();
            
            capture = new VideoCapture ();
            capture.open (couple_avi_filepath);
            
            if (capture.isOpened ()) {
                Debug.Log ("capture.isOpened() true");
            } else {
                Debug.Log ("capture.isOpened() false");
            }


            Debug.Log ("CAP_PROP_FORMAT: " + capture.get (Videoio.CAP_PROP_FORMAT));
            Debug.Log ("CV_CAP_PROP_PREVIEW_FORMAT: " + capture.get (Videoio.CV_CAP_PROP_PREVIEW_FORMAT));
            Debug.Log ("CAP_PROP_POS_MSEC: " + capture.get (Videoio.CAP_PROP_POS_MSEC));
            Debug.Log ("CAP_PROP_POS_FRAMES: " + capture.get (Videoio.CAP_PROP_POS_FRAMES));
            Debug.Log ("CAP_PROP_POS_AVI_RATIO: " + capture.get (Videoio.CAP_PROP_POS_AVI_RATIO));
            Debug.Log ("CAP_PROP_FRAME_COUNT: " + capture.get (Videoio.CAP_PROP_FRAME_COUNT));
            Debug.Log ("CAP_PROP_FPS: " + capture.get (Videoio.CAP_PROP_FPS));
            Debug.Log ("CAP_PROP_FRAME_WIDTH: " + capture.get (Videoio.CAP_PROP_FRAME_WIDTH));
            Debug.Log ("CAP_PROP_FRAME_HEIGHT: " + capture.get (Videoio.CAP_PROP_FRAME_HEIGHT));

            capture.grab ();
            capture.retrieve (rgbMat, 0);
            int frameWidth = rgbMat.cols ();
            int frameHeight = rgbMat.rows ();
            texture = new Texture2D (frameWidth, frameHeight, TextureFormat.RGB24, false);
            gameObject.transform.localScale = new Vector3 ((float)frameWidth, (float)frameHeight, 1);
            float widthScale = (float)Screen.width / (float)frameWidth;
            float heightScale = (float)Screen.height / (float)frameHeight;
            if (widthScale < heightScale) {
                Camera.main.orthographicSize = ((float)frameWidth * (float)Screen.height / (float)Screen.width) / 2;
            } else {
                Camera.main.orthographicSize = (float)frameHeight / 2;
            }
            capture.set (Videoio.CAP_PROP_POS_FRAMES, 0);

            gameObject.GetComponent<Renderer> ().material.mainTexture = texture;

            if (fpsMonitor != null) {
                fpsMonitor.Add ("dlib shape predictor", dlibShapePredictorFileName);
                fpsMonitor.Add ("width", frameWidth.ToString ());
                fpsMonitor.Add ("height", frameHeight.ToString ());
                fpsMonitor.Add ("orientation", Screen.orientation.ToString ());
            }
        }
        
        // Update is called once per frame
        void Update ()
        {
            if (capture == null)
                return;

            //Loop play
            if (capture.get (Videoio.CAP_PROP_POS_FRAMES) >= capture.get (Videoio.CAP_PROP_FRAME_COUNT))
                capture.set (Videoio.CAP_PROP_POS_FRAMES, 0);

            //error PlayerLoop called recursively! on iOS.reccomend WebCamTexture.
            if (capture.grab ()) {

                capture.retrieve (rgbMat, 0);

                Imgproc.cvtColor (rgbMat, rgbMat, Imgproc.COLOR_BGR2RGB);
                //Debug.Log ("Mat toString " + rgbMat.ToString ());


                OpenCVForUnityUtils.SetImage (faceLandmarkDetector, rgbMat);
                
                //detect face rects
                List<UnityEngine.Rect> detectResult = faceLandmarkDetector.Detect ();
                
                foreach (var rect in detectResult) {

                    //detect landmark points
                    List<Vector2> points = faceLandmarkDetector.DetectLandmark (rect);

                    //draw landmark points
                    OpenCVForUnityUtils.DrawFaceLandmark (rgbMat, points, new Scalar (0, 255, 0), 2);

                    //draw face rect
                    OpenCVForUnityUtils.DrawFaceRect (rgbMat, rect, new Scalar (255, 0, 0), 2);
                }
                
                //Imgproc.putText (rgbMat, "W:" + rgbMat.width () + " H:" + rgbMat.height () + " SO:" + Screen.orientation, new Point (5, rgbMat.rows () - 10), Core.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255), 1, Imgproc.LINE_AA, false);

                OpenCVForUnity.Utils.fastMatToTexture2D (rgbMat, texture);
            }
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy ()
        {
            if (capture != null)
                capture.release ();

            if (rgbMat != null)
                rgbMat.Dispose ();

            if (texture != null) {
                Texture2D.Destroy (texture);
                texture = null;
            }

            if (faceLandmarkDetector != null)
                faceLandmarkDetector.Dispose ();

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
    }
}