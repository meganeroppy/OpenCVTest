using UnityEngine;
using System.Collections;


	public class ViewingDirection : MonoBehaviour
{
		//目のメッシュ設定
		public MeshRenderer eyeObjL;
		public MeshRenderer eyeObjR;

	private float dirValX = 0;
	private float dirValY = 0;
		private Vector2 eyeOffset;

		// Use this for initialization
		void Start () {
		//	eyeObjL = GameObject.Find("eye_L_old").GetComponent<MeshRenderer>();
		//	eyeObjR = GameObject.Find("eye_R_old").GetComponent<MeshRenderer>();
			eyeOffset = Vector2.zero;
		}

		void OnGUI() {
			//スライダーと値
			GUILayout.BeginArea (new Rect(Screen.width - 250, 10, 400, 50));
			dirValX = GUILayout.HorizontalSlider ( dirValX, -1, 1, GUILayout.Width (150));
			dirValY = GUILayout.HorizontalSlider ( dirValY, -1, 1, GUILayout.Width (150));
			GUILayout.Label ("Viewing direction  " + dirValX);
			GUILayout.Label ("Viewing direction  " + dirValY);
			GUILayout.EndArea ();

			//スライダの値で、瞳テクスチャのXオフセットを設定（Yは変えない）
			eyeOffset = new Vector2(dirValX * 0.2f , dirValY * 0.1f);
			eyeObjL.material.SetTextureOffset("_MainTex", eyeOffset);
			eyeObjR.material.SetTextureOffset("_MainTex", eyeOffset);
		}

	}
