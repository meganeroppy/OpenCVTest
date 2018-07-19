using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWeapon : MonoBehaviour {

	[SerializeField]
	Transform muzzle;

	[SerializeField]
	float se_interval;

	float se_timer = 0;

	[SerializeField]
	AudioSource se;

    [SerializeField]
    GameObject gunVisual;

	/// <summary>
	/// 銃撃ビジュアルのマテリアル
	/// </summary>
	[SerializeField]
	GameObject laserVisual;

	[SerializeField]
	SpriteRenderer muzzleFlash;

	[SerializeField]
	float visualInterval;

	float visualTimer = 0;

	[SerializeField]
	float visualShowDur = 0.1f;

    [SerializeField]
    SteamVR_TrackedObject controller;

    public bool Firing {get;set;}

    /// <summary>
    /// 対応する手のアニメ制御
    /// </summary>
    [SerializeField]
    FingerAnim handAnim;

	// Use this for initialization
	void Start () 
	{
		PresetHoles();
		laserVisual.SetActive(false);
        muzzleFlash.color = Color.clear;
	}
	
	// Update is called once per frame
	void Update () 
	{
        var device = SteamVR_Controller.Input((int)controller.index);
        if (device == null) return;

        var gripped = device.GetPressDown(SteamVR_Controller.ButtonMask.Grip);
        if (gripped)
        {
            gunVisual.SetActive(!gunVisual.activeInHierarchy);

            // 手の形をデフォルトに
            handAnim.SetHandDefault();
            // 銃を持っているときは手のアニメなし
            handAnim.enabled = !gunVisual.activeInHierarchy;

            var efs = gunVisual.activeInHierarchy ? weaponAppearEffect : weaponDisappearEffect;
            Instantiate(efs, transform.position, Quaternion.identity);
        }

        if (!gunVisual.activeInHierarchy) return;

        var triggered = device.GetPress(SteamVR_Controller.ButtonMask.Trigger);

        Firing = Input.GetKey( KeyCode.Space ) || triggered;

		UpdateFire();
    }

    void UpdateFire()
	{
		if( Firing )
		{
			visualTimer += Time.deltaTime;

			if( visualTimer > visualInterval )
			{
				visualTimer = 0;
				StartCoroutine( ShowLaser() );

				UpdateHoles();

				se.Play();
			}

			/*
			se_timer += Time.deltaTime;
			if( se_timer >= se_interval )
			{
				se_timer = 0;

				// 効果音
				se.Play();
			}
			*/
		}
	}

	IEnumerator ShowLaser()
	{
		laserVisual.SetActive( true );
		muzzleFlash.color = Color.white;

		yield return new WaitForSeconds( visualShowDur );

		laserVisual.SetActive( false );
		muzzleFlash.color = Color.clear;
	}

	[SerializeField]
	GameObject prefab;

	[SerializeField]
	float holeScaleMax = 0.1f;

	[SerializeField]
	float holeScaleMin = 0.05f;

	int presetHolesCount = 8;

	static List<GameObject> holePool;

    [SerializeField]
    GameObject weaponAppearEffect;

    [SerializeField]
    GameObject weaponDisappearEffect;

    void UpdateHoles()
	{
		RaycastHit hit;
		if( !Physics.Raycast( muzzle.position, muzzle.transform.forward, out hit, 30f) )
		{
			return;
		}

		GameObject obj = null;

        if( holePool == null )
        {
            holePool = new List<GameObject>();
        }

		for( int i=0 ; i<holePool.Count ; ++i )
		{
			if( !holePool[i].gameObject.activeSelf )
			{
				obj = holePool[i].gameObject;
			}
		}

		if( !obj )
		{
			obj = Instantiate( prefab );
			holePool.Add( obj );
		}
		else
		{
			obj.SetActive(true);
		}

		obj.transform.SetPositionAndRotation(hit.point, Quaternion.Euler( hit.normal) );
		obj.transform.localScale = Vector3.one * Random.Range( holeScaleMin, holeScaleMax ); 
		obj.transform.localRotation = Quaternion.AngleAxis( Random.Range(0, 360), obj.transform.forward) ;

		obj.transform.SetParent( hit.transform );
	}

	void PresetHoles()
	{
        if (holePool == null)
        {
            holePool = new List<GameObject>();
        }

        for ( int i=0 ; i<presetHolesCount ; ++i )
		{
			var obj = Instantiate( prefab );
			obj.SetActive(false);
			holePool.Add( obj );
		}
	}
}
