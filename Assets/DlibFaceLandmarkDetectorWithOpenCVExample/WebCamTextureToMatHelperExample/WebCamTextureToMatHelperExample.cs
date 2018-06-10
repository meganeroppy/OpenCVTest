using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using OpenCVForUnity;
using DlibFaceLandmarkDetector;

namespace DlibFaceLandmarkDetectorExample
{
    /// <summary>
    /// WebCamTextureToMatHelper Example
    /// </summary>
    [RequireComponent (typeof(WebCamTextureToMatHelper))]
    public class WebCamTextureToMatHelperExample : MonoBehaviour
    {
        public static WebCamTextureToMatHelperExample instance;
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

        public float Height { get { return height; }  }
        float height;
        public float Width { get { return width; } }
        float width;

//        [SerializeField]
//        MeshRenderer targetMeshPrefab;

//        List<MeshRenderer> targetMeshList = new List<MeshRenderer>();

        [SerializeField]
        float posZ = 10f;

        [HideInInspector]
        public List<UnityEngine.Rect> detectResult = new List<UnityEngine.Rect>();

#if UNITY_WEBGL && !UNITY_EDITOR
        Stack<IEnumerator> coroutines = new Stack<IEnumerator> ();
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        float rearCameraRequestedFPS;
#endif

        // Use this for initialization
        void Start ()
        {
            instance = this;

            fpsMonitor = GetComponent<FpsMonitor> ();

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
            if (myCamera == null)
                myCamera = Camera.main;

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

                                    
            width = webCamTextureMat.width ();
            height = webCamTextureMat.height ();
                                    
            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale) {
                //               Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
                myCamera.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            }
            else {
                //               Camera.main.orthographicSize = height / 2;
                myCamera.orthographicSize = height / 2;
            }
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
//                List<UnityEngine.Rect> detectResult = faceLandmarkDetector.Detect();
                detectResult = faceLandmarkDetector.Detect();
                /*
                // ターゲットメッシュのリストを更新
                if ( targetMeshPrefab != null )
                {
                    while (targetMeshList.Count < detectResult.Count)
                    {
                        var obj = Instantiate(targetMeshPrefab).GetComponent<MeshRenderer>();
                        obj.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        targetMeshList.Add(obj);
                    }

                    for (int i = targetMeshList.Count - 1; i >= 0; --i)
                    {
                        if (i >= detectResult.Count)
                        {
                            targetMeshList[i].material.color = Color.clear;
                        }
                        else
                        {
                            targetMeshList[i].material.color = Color.red;
                        }
                    }
                }
                */

                foreach (var rect in detectResult) {

                    //detect landmark points
                    List<Vector2> points = faceLandmarkDetector.DetectLandmark (rect);

                    //draw landmark points
                    OpenCVForUnityUtils.DrawFaceLandmark (rgbaMat, points, new Scalar (0, 255, 0, 255), 2);

                    //draw face rect
                    OpenCVForUnityUtils.DrawFaceRect (rgbaMat, rect, new Scalar (255, 0, 0, 255), 2);
                    /*
                    // 顔の中心にオブジェクトを移動
                    if( targetMeshList != null && targetMeshList.Count> 0 )
                    {
                     //   Debug.Log( string.Format("Rect位置( {0}, {1})  Rectサイズ( {2}, {3})", rect.x, rect.y, rect.width, rect.height) );
                        var pos = new Vector2(
                        //    rect.x - (rect.width / 2)
                            rect.center.x
                            ,
                            //										rect.y + ( rect.height / 2 )
                            rect.center.y
                        );
                        // オブジェクトを移動する
                        int i = detectResult.IndexOf(rect);
                        targetMeshList[i].transform.localPosition = Vector2ToVector3(pos);
                    }
                    */
                }

                //Imgproc.putText (rgbaMat, "W:" + rgbaMat.width () + " H:" + rgbaMat.height () + " SO:" + Screen.orientation, new Point (5, rgbaMat.rows () - 10), Core.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255, 255), 1, Imgproc.LINE_AA, false);

                OpenCVForUnity.Utils.fastMatToTexture2D (rgbaMat, texture);
            }
        }

        /// <summary>
        /// OpenCVの2次元座標をUnityの3次元座標に変換する
        /// </summary>
        /// <param name="vector2"></param>
        /// <returns></returns>
        private Vector3 Vector2ToVector3(Vector2 vector2)
        {
            if (myCamera == null)
            {
                throw new System.Exception("");
            }

            // スクリーンサイズで調整(WebCamera->Unity)
            vector2.x = vector2.x * Screen.width / Width;
            vector2.y = vector2.y * Screen.height / Height;

            // Unityのワールド座標系(3次元)に変換
            //			var vector3 = _Camera.ScreenToWorldPoint( vector2 );
            var vector3 = myCamera.ScreenToWorldPoint(vector2);

            // 座標の調整
            // Y座標は逆、Z座標は0にする(Xもミラー状態によって逆にする必要あり)
            vector3.y *= -1;
            vector3.z = posZ;

            return vector3;
        }

        [SerializeField]
        Camera myCamera = null;
        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy ()
        {
            if (webCamTextureToMatHelper != null)
                webCamTextureToMatHelper.Dispose ();

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
    }
}