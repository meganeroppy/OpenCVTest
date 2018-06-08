using UnityEngine;
using System.Collections;


	public class ViewingDirection : MonoBehaviour {

		//目のメッシュ設定
		public MeshRenderer eyeObjL;
		public MeshRenderer eyeObjR;

		private float dirVal = 0;
		private Vector2 eyeOffset;

		// Use this for initialization
		void Start () {
			eyeObjL = GameObject.Find("eye_L_old").GetComponent<MeshRenderer>();
			eyeObjR = GameObject.Find("eye_R_old").GetComponent<MeshRenderer>();
			eyeOffset = Vector2.zero;
		}

		void OnGUI() {
			//スライダーと値
			GUILayout.BeginArea (new Rect(Screen.width - 250, 10, 400, 50));
			dirVal = GUILayout.HorizontalSlider ( dirVal, -1, 1, GUILayout.Width (150));
			GUILayout.Label ("Viewing direction  " + dirVal);
			GUILayout.EndArea ();
			//スライダの値で、瞳テクスチャのXオフセットを設定（Yは変えない）
			eyeOffset = new Vector2(dirVal * 0.2f , 0);
			eyeObjL.material.SetTextureOffset("_MainTex", eyeOffset);
			eyeObjR.material.SetTextureOffset("_MainTex", eyeOffset);
		}

	}
