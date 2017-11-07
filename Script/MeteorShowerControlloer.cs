using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorShowerControlloer : MonoBehaviour {

    private GameObject faceObj;

    [SerializeField]
    private float[] moveSpeed = { 0.1f, 0.15f, 0.2f };
    private int randomSpeed;
   // [HideInInspector]
    public int id;

	void Start () 
    {
        faceObj = GameObject.FindGameObjectWithTag("Face");
        randomSpeed = Random.Range(0, moveSpeed.Length);
        Invoke("Kill", 5);
	}
	
	void Update () 
    {
        if (GameController.Instance.gameStates == GameController.GameStates.Start)
        {
            Move();
        }
    }

    void Move()
    {
        transform.position += transform.TransformDirection(Vector3.forward) * moveSpeed[randomSpeed];
        transform.position = new Vector3(transform.position.x,
                                        transform.position.y,
                                        faceObj.transform.position.z);
    }

    void OnTriggerEnter(Collider collider)
    {
        //	メテオ同士か顔に当たったらメテオをリスポーン
        if (collider.gameObject.tag == "Meteo" )
        {
            EffectManager.Instance.PlayEffect("explosion", transform.position, Quaternion.identity); 
            SoundManeger.Instance.isPlayMeteoHitToMeteoSe = true;
            Kill();
        }
        else if (collider.gameObject.tag == "Face")
        {
            Kill();
        }
        else if (collider.gameObject.tag == "Missile")
        {
            ScoreInfo.HitMeteorNormal();
            Kill();
        }
    }

    void Kill()
    {
        var thisRenderer = this.GetComponent<Renderer>();
        if (thisRenderer != null && thisRenderer.materials != null)
        {
            foreach (var m in thisRenderer.materials)
            {
                DestroyImmediate(m);
            }
        }
        GameController.Instance.enemyList.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
}
