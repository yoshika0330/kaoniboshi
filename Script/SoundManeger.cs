using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManeger : MonoBehaviour {

    [HideInInspector]
    public bool isPlayMissileSe = false;
    [HideInInspector]
    public bool isPlayPlayerHitSe = false;
    [HideInInspector]
    public bool isPlayMissileHitSe = false;
    [HideInInspector]
    public bool isPlayPlayerOrMarsDeathSe = false;
    [HideInInspector]
    public bool isPlayMeteoHitToMeteoSe = false;

    private AudioSource audioSource;

    public AudioClip missileSe;
    public AudioClip playerHitSe;
    public AudioClip missileHitSe;
    public AudioClip playerOrMarsDeathSe;
    public AudioClip meteoHitToMeteoSe;

    private int deathSeCount = 0;

    #region Singlton
    public static SoundManeger Instance { get; private set; }
    void Awake()
    {
        if (Instance != null)
        {
            enabled = false;
            DestroyImmediate(this);
            return;
        }
        Instance = this;
    }
    #endregion

	void Start () 
    {
        audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	void Update ()
    {
        PlayMissileSe();
        PlayPlayerHitSe();
        PlayMissileHitSe();
        PlayPlayerOrMarsDeathSe();
        PlayMeteoHitToMeteoSe();
	}

    // Missileが出た時のSE
    void PlayMissileSe()
    {
        if(isPlayMissileSe)
        {
            isPlayMissileSe = false;
            audioSource.PlayOneShot(missileSe);
        }
    }

    //　地球に敵が当たった時のSE
    void PlayPlayerHitSe()
    {
        if (isPlayPlayerHitSe)
        {
            isPlayPlayerHitSe = false;
            audioSource.PlayOneShot(playerHitSe);
        }
    }

    // missileが何かに当たった時の音
    void PlayMissileHitSe()
    {
        if(isPlayMissileHitSe)
        {
            isPlayMissileHitSe = false;
            audioSource.PlayOneShot(missileHitSe);
        }
    }

    // プレイヤーがやられた時の音
    void PlayPlayerOrMarsDeathSe()
    {
        if(isPlayPlayerOrMarsDeathSe && deathSeCount < 1)
        {
            deathSeCount++;
            isPlayPlayerOrMarsDeathSe = false;
            audioSource.PlayOneShot(playerOrMarsDeathSe);
        }
    }

    // 隕石同士がぶつかった時の音
    void PlayMeteoHitToMeteoSe()
    {
        if(isPlayMeteoHitToMeteoSe)
        {
            isPlayMeteoHitToMeteoSe = false;
            audioSource.PlayOneShot(meteoHitToMeteoSe);
        }
    }

}
