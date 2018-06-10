using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using UnityEngine.UI;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using OpenCVForUnity;
using OpenCVFaceTracker;
using OpenCVForUnitySample;

namespace FaceTrackerExample
{
	/// <summary>
	/// WebCamTexture face tracker example.
	/// </summary>
	[RequireComponent(typeof(WebCamTextureToMatHelper))]
	public class WebCamTextureFaceTrackerExample : MonoBehaviour
	{
        public static WebCamTextureFaceTrackerExample instance;

		/// <summary>
		/// The auto reset mode. if ture, Only if face is detected in each frame, face is tracked.
		/// </summary>
		public bool isAutoResetMode;

		/// <summary>
		/// The auto reset mode toggle.
		/// </summary>
		public Toggle isAutoResetModeToggle;

		/// <summary>
		/// The gray mat.
		/// </summary>
		Mat grayMat;

		/// <summary>
		/// The texture.
		/// </summary>
		Texture2D texture;

		/// <summary>
		/// The cascade.
		/// </summary>
		CascadeClassifier cascade;

		/// <summary>
		/// The face tracker.
		/// </summary>
		FaceTracker faceTracker;

		/// <summary>
		/// The face tracker parameters.
		/// </summary>
		FaceTrackerParams faceTrackerParams;

		/// <summary>
		/// The web cam texture to mat helper.
		/// </summary>
		WebCamTextureToMatHelper webCamTextureToMatHelper;

		/// <summary>
		/// The tracker_model_json_filepath.
		/// </summary>
		private string tracker_model_json_filepath;

		/// <summary>
		/// The haarcascade_frontalface_alt_xml_filepath.
		/// </summary>
		private string haarcascade_frontalface_alt_xml_filepath;

        public List<OpenCVForUnity.Rect> rectsList = new List<OpenCVForUnity.Rect>();

	//	[SerializeField]
	//	MeshRenderer targetMeshPrefab;

	//	public List<MeshRenderer> targetMeshList = new List<MeshRenderer>();

		[SerializeField]
		float posZ = 10f;

        public float Height { get { return height; } }
        float height = 640;
        public float Width { get { return width; } }
        float width = 480;

        [SerializeField]
		Camera mainCamera;

		[SerializeField]
		Camera objectCamera;

		// Use this for initialization
		void Start()
		{
            instance = this;

			webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();

			isAutoResetModeToggle.isOn = isAutoResetMode;

			#if UNITY_WEBGL && !UNITY_EDITOR
			StartCoroutine(getFilePathCoroutine());
			#else
			tracker_model_json_filepath = Utils.getFilePath("tracker_model.json");
			haarcascade_frontalface_alt_xml_filepath = Utils.getFilePath("haarcascade_frontalface_alt.xml");
			Run();
			#endif
		}

		#if UNITY_WEBGL && !UNITY_EDITOR
		private IEnumerator getFilePathCoroutine()
		{
		var getFilePathAsync_0_Coroutine = StartCoroutine(Utils.getFilePathAsync("tracker_model.json", (result) => {
		tracker_model_json_filepath = result;
		}));
		var getFilePathAsync_1_Coroutine = StartCoroutine(Utils.getFilePathAsync("haarcascade_frontalface_alt.xml", (result) => {
		haarcascade_frontalface_alt_xml_filepath = result;
		}));


		yield return getFilePathAsync_0_Coroutine;
		yield return getFilePathAsync_1_Coroutine;

		Run();
		}
		#endif

		private void Run()
		{
			//initialize FaceTracker
			faceTracker = new FaceTracker(tracker_model_json_filepath);
			//initialize FaceTrackerParams
			faceTrackerParams = new FaceTrackerParams();

			cascade = new CascadeClassifier();
			cascade.load(haarcascade_frontalface_alt_xml_filepath);
			//            if (cascade.empty())
			//            {
			//                Debug.LogError("cascade file is not loaded.Please copy from “FaceTrackerExample/StreamingAssets/” to “Assets/StreamingAssets/” folder. ");
			//            }


			webCamTextureToMatHelper.Initialize();

		}

		/// <summary>
		/// Raises the webcam texture to mat helper initialized event.
		/// </summary>
		public void OnWebCamTextureToMatHelperInitialized()
		{
            if (mainCamera == null)
                mainCamera = Camera.main;

            Debug.Log("OnWebCamTextureToMatHelperInitialized");

			Mat webCamTextureMat = webCamTextureToMatHelper.GetMat();

			texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);

			gameObject.transform.localScale = new Vector3(webCamTextureMat.cols(), webCamTextureMat.rows(), 1);
			Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

			float width = 0;
			float height = 0;

			width = gameObject.transform.localScale.x;
			height = gameObject.transform.localScale.y;

			float widthScale = (float)Screen.width / width;
			float heightScale = (float)Screen.height / height;
			if (widthScale < heightScale)
			{
				mainCamera.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
			} 
			else
			{
				mainCamera.orthographicSize = height / 2;
			}

			if( objectCamera != null ) objectCamera.orthographicSize = mainCamera.orthographicSize;

			gameObject.GetComponent<Renderer>().material.mainTexture = texture;

			grayMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC1);
		}
			
		/// <summary>
		/// Raises the webcam texture to mat helper disposed event.
		/// </summary>
		public void OnWebCamTextureToMatHelperDisposed()
		{
			Debug.Log("OnWebCamTextureToMatHelperDisposed");

			faceTracker.reset();
			grayMat.Dispose();
		}

		/// <summary>
		/// Raises the webcam texture to mat helper error occurred event.
		/// </summary>
		/// <param name="errorCode">Error code.</param>
		//        public void OnWebCamTextureToMatHelperErrorOccurred(WebCamTextureToMatHelper.ErrorCode errorCode)
		//        {
		//            Debug.Log("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
		//        }

		// Update is called once per frame
		void Update()
		{
			if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
			{

				Mat rgbaMat = webCamTextureToMatHelper.GetMat();

				//convert image to greyscale
				Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);


				if (isAutoResetMode || faceTracker.getPoints().Count <= 0)
				{
					//                                      Debug.Log ("detectFace");

					//convert image to greyscale
					using (var equalizeHistMat = new Mat())
					using (var faces = new MatOfRect())
					{
						Imgproc.equalizeHist(grayMat, equalizeHistMat);

						cascade.detectMultiScale(equalizeHistMat,
							faces,
							1.1f, 
							2,
							0 | Objdetect.CASCADE_SCALE_IMAGE,
							new OpenCVForUnity.Size(equalizeHistMat.cols() * 0.15, 
								equalizeHistMat.cols() * 0.15), 
							new Size()
						);

						if (faces.rows() > 0)
						{
							//                          Debug.Log ("faces " + faces.dump ());

							rectsList = faces.toList();
							List<Point[]> pointsList = faceTracker.getPoints();

							if (isAutoResetMode)
							{
								//add initial face points from MatOfRect
								if (pointsList.Count <= 0)
								{
									faceTracker.addPoints(faces);
								//	Debug.Log ("reset faces ");
								} 
								else
								{
									for (int i = 0; i < rectsList.Count; i++)
									{
										var trackRect = new OpenCVForUnity.Rect(rectsList [i].x + rectsList [i].width / 3, rectsList [i].y + rectsList [i].height / 2, rectsList [i].width / 3, rectsList [i].height / 3);
										//It determines whether nose point has been included in trackRect.                                      
										if (i < pointsList.Count && !trackRect.contains(pointsList [i] [67]))
										{
											rectsList.RemoveAt(i);
											pointsList.RemoveAt(i);
											//                                          Debug.Log ("remove " + i);
										}
										Imgproc.rectangle(rgbaMat, new Point(trackRect.x, trackRect.y), new Point(trackRect.x + trackRect.width, trackRect.y + trackRect.height), new Scalar(0, 0, 255, 255), 2);
									}
								}
							} 
							else
							{
								faceTracker.addPoints(faces);
							}
                            /*
                            // ターゲットメッシュのリストを更新
                            if (targetMeshPrefab != null)
                            {
                                while ( targetMeshList.Count < rectsList.Count )
								{
									var obj = Instantiate( targetMeshPrefab ).GetComponent<MeshRenderer>();
									obj.transform.rotation = Quaternion.Euler( -90, 0, 0);
									targetMeshList.Add( obj );
								}

								for( int i=targetMeshList.Count-1 ; i >= 0 ; --i )
								{
									if( i >= rectsList.Count  )
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
							//draw face rect
							for (int i = 0; i < rectsList.Count; i++)
							{
								#if OPENCV_2
								Core.rectangle (rgbaMat, new Point (rectsList [i].x, rectsList [i].y), new Point (rectsList [i].x + rectsLIst [i].width, rectsList [i].y + rectsList [i].height), new Scalar (255, 0, 0, 255), 2);
								#else
								Imgproc.rectangle(rgbaMat, new Point(rectsList [i].x, rectsList [i].y), new Point(rectsList [i].x + rectsList [i].width, rectsList [i].y + rectsList [i].height), new Scalar(255, 0, 0, 255), 2);
#endif
                                /*
                                // 顔の中心にオブジェクトを移動
                                if (targetMeshList != null && targetMeshList.Count > 0)
                                {
                                    var rect = rectsList[i];

//									Debug.Log( string.Format("Rect位置( {0}, {1})  Rectサイズ( {2}, {3})", rect.x, rect.y, rect.width, rect.height) );
									var pos = new Vector2( 
										rect.x - ( rect.width / 2 )
										,
//										rect.y + ( rect.height / 2 )
										rect.y
									);
									// オブジェクトを移動する
									targetMeshList[i].transform.localPosition = Vector2ToVector3( pos );
								}
                                */
							}
                            /*
							// 顔の中心位置にオブジェクトを移動
							if( false )
							{
								for( int i=0 ; i<pointsList.Count ; ++i )
								{
									Vector2 pos;

									// 中心位置を求める
									{
										double sumX = 0, sumY = 0;
										for( int j=0 ; j<pointsList[i].Length ; ++j ){

											var point = pointsList[i][j];
											sumX += point.x;
											sumY += point.y;
										}

										var averageX = sumX / pointsList[i].Length;
										var averageY = sumY / pointsList[i].Length;

										pos = new Vector2( (float)averageX, (float)averageY );
									}

									{
										double leftEnd = 0, topEnd = 0;

									}


									// オブジェクトを移動する
									targetMeshList[i].transform.localPosition = Vector2ToVector3( pos );
								}
							}*/

						} 
						else
						{
							if (isAutoResetMode)
							{
								faceTracker.reset();
							}
						}
					}
				}

				//track face points.if face points <= 0, always return false.
				if (faceTracker.track(grayMat, faceTrackerParams))
					faceTracker.draw(rgbaMat, new Scalar(255, 0, 0, 255), new Scalar(0, 255, 0, 255));

				#if OPENCV_2
				Core.putText (rgbaMat, "'Tap' or 'Space Key' to Reset", new Point (5, rgbaMat.rows () - 5), Core.FONT_HERSHEY_SIMPLEX, 0.8, new Scalar (255, 255, 255, 255), 2, Core.LINE_AA, false);
				#else
				Imgproc.putText(rgbaMat, "'Tap' or 'Space Key' to Reset", new Point(5, rgbaMat.rows() - 5), Core.FONT_HERSHEY_SIMPLEX, 0.8, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
				#endif

				//              Core.putText (rgbaMat, "W:" + rgbaMat.width () + " H:" + rgbaMat.height () + " SO:" + Screen.orientation, new Point (5, rgbaMat.rows () - 10), Core.FONT_HERSHEY_SIMPLEX, 1.0, new Scalar (255, 255, 255, 255), 2, Core.LINE_AA, false);

				Utils.matToTexture2D(rgbaMat, texture, webCamTextureToMatHelper.GetBufferColors());
			}

			if (Input.GetKeyUp(KeyCode.Space) || Input.touchCount > 0)
			{
				faceTracker.reset();
			}
		}


		/// <summary>
		/// Raises the disable event.
		/// </summary>
		void OnDisable()
		{
			webCamTextureToMatHelper.Dispose();

			if (cascade != null)
				cascade.Dispose();
		}

		/// <summary>
		/// Raises the back button event.
		/// </summary>
		public void OnBackButton()
		{
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			SceneManager.LoadScene("FaceTrackerExample");
			#else
			Application.LoadLevel("FaceTrackerExample");
			#endif
		}

		/// <summary>
		/// Raises the play button event.
		/// </summary>
		public void OnPlayButton()
		{
			webCamTextureToMatHelper.Play();
		}

		/// <summary>
		/// Raises the pause button event.
		/// </summary>
		public void OnPauseButton()
		{
			webCamTextureToMatHelper.Pause();
		}

		/// <summary>
		/// Raises the stop button event.
		/// </summary>
		public void OnStopButton()
		{
			webCamTextureToMatHelper.Stop();
		}

		/// <summary>
		/// Raises the change camera button event.
		/// </summary>
		public void OnChangeCameraButton()
		{
			webCamTextureToMatHelper.Initialize(null, webCamTextureToMatHelper.requestedWidth, webCamTextureToMatHelper.requestedHeight, !webCamTextureToMatHelper.requestedIsFrontFacing);
		}

		/// <summary>
		/// Raises the change auto reset mode toggle event.
		/// </summary>
		public void OnIsAutoResetModeToggle()
		{
			if (isAutoResetModeToggle.isOn)
			{
				isAutoResetMode = true;
			} 
			else
			{
				isAutoResetMode = false;
			}
		}

		/// <summary>
		/// OpenCVの2次元座標をUnityの3次元座標に変換する
		/// </summary>
		/// <param name="vector2"></param>
		/// <returns></returns>
		private Vector3 Vector2ToVector3( Vector2 vector2 )
		{
			if ( mainCamera == null ) {
				throw new System.Exception("");
			}

			// スクリーンサイズで調整(WebCamera->Unity)
			vector2.x = vector2.x * Screen.width / Width;
			vector2.y = vector2.y * Screen.height / Height;

			// Unityのワールド座標系(3次元)に変換
//			var vector3 = _Camera.ScreenToWorldPoint( vector2 );
			var vector3 = mainCamera.ScreenToWorldPoint( vector2 );

			// 座標の調整
			// Y座標は逆、Z座標は0にする(Xもミラー状態によって逆にする必要あり)
			vector3.y *= -1;
			vector3.z = posZ;

			return vector3;
		}
	}
}