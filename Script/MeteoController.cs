using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoController : MonoBehaviour {

	Camera mainCamera;

    public LevelDesignMeteo levelObj;

	private GameObject faceObj;

	public float distance = 10.0f;

	private int randomSpawnNum;
	private int randomSpeed;

	private float meteoX,meteoY;
	private float faceX,faceY;
	private float rad,angle;
	private float[] moveSpeed = {0.1f,0.15f,0.2f};

	private Vector3 leftTop, leftDown, rightTop, rightDown;

	private bool isSpawn = true;
	private bool isMove = false;
	private bool isOnCamera = false;

	void Start () {
		
		faceObj = GameObject.FindGameObjectWithTag ("Face");

	}

	// -------------------------------
	//	カメラに写っているか判定
	void OnBecameVisible ()
	{
		isOnCamera = true;
	}

	void OnBecameInvisible ()
	{
		// カメラに写ってからカメラから消えたら
		if (isOnCamera) {
			ReSpawn ();	//	リスポーン
		}

		isOnCamera = false;
	}
	// -------------------------------


	void Update () {
        if (GameController.Instance.gameStates == GameController.GameStates.Start)
        {
            if (isSpawn)
            {
                //	スポーンさせるための数字をランダムで生成
                randomSpawnNum = Random.Range(0, 4);
                //	速度を配列の中からランダムで取得
                randomSpeed = Random.Range(0, moveSpeed.Length );
                RandomSpawn();		//	決められた位置にランダムでスポーン
                isSpawn = false;
                GetAngle();
                //　多少の誤差をつけ角度を地球の方に向ける
                transform.rotation = Quaternion.Euler(-angle + Random.Range(-10f, 10f), 90, 0);
                isMove = true;
            }

            if (isMove)
            {
                MoveMeteo();
            }

            //　if (GameController.Instance.timeCount >= levelObj.popStartTime)
            if(GameController.Instance.gamePhase == GameController.GamePhase.Phase2)
            {
                StartCoroutine(AddMeteo());
            }
        }	
	}

	// ---------------------------------------------------------------
	//	上下左右それぞれの位置に隕石を移動
	void SpawnTop()
	{
		transform.position = new Vector3 (GameController.Instance.randomX, GameController.Instance.cameraY + 2, 0);
	}

	void SpawnDown()
	{
		transform.position = new Vector3 (GameController.Instance.randomX, GameController.Instance.cameraY * -1 - 2, 0);
	}

	void SpawnRight()
	{
		transform.position = new Vector3 (GameController.Instance.cameraX + 2, GameController.Instance.randomY, 0);
	}

	void SpawnLeft()
	{
		transform.position = new Vector3 (GameController.Instance.cameraX * -1 - 2, GameController.Instance.randomY, 0);
	}
	// ---------------------------------------------------------------

	void RandomSpawn(){
		switch (randomSpawnNum) 
		{
		case 0:
			SpawnTop ();
			break;
		case 1:
			SpawnDown ();
			break;
		case 2:
			SpawnLeft ();
			break;
		case 3:
			SpawnRight ();
			break;
		default:
			break;
		}
	}


	void ReSpawn(){
        if (GameController.Instance.gamePhase == GameController.GamePhase.Phase1 ||
            GameController.Instance.gamePhase == GameController.GamePhase.Phase2 ||
            GameController.Instance.gamePhase == GameController.GamePhase.PromptReport)
        {
            isMove = false;									//	動きを止める
            GetComponent<MeshRenderer>().enabled = false;	//	非表示にする
            isSpawn = true;									//	スポーンさせる
            GetComponent<MeshRenderer>().enabled = true;	//	表示させる
        }

        else
        {
            //　エネミーリストから自分を削除
            GameController.Instance.enemyList.Remove(this.gameObject);
            //　自分を削除
            Destroy(this.gameObject);
        }
	}

	//	顔からメテオの角度を計算する
	void GetAngle(){
		//	メテオの座標を取得
		meteoX = transform.position.x;
		meteoY = transform.position.y;
		//	顔の座標を取得
		faceX = faceObj.transform.position.x;
		faceY = faceObj.transform.position.y;
		//	角度を求める
		rad = Mathf.Atan2 ( faceY - meteoY ,faceX - meteoX);
		angle = rad * Mathf.Rad2Deg;
	}

	void MoveMeteo(){
		//	メテオの移動
		transform.position += transform.TransformDirection(Vector3.forward) * moveSpeed[randomSpeed];
        transform.position = new Vector3(transform.position.x,
                                         transform.position.y,
                                         faceObj.transform.position.z);
	}

	void OnTriggerEnter(Collider collider) { 
		//	メテオ同士か顔に当たったらメテオをリスポーン
        if (collider.gameObject.tag == "Meteo" && isOnCamera )
        {
            EffectManager.Instance.PlayEffect("explosion", this.transform.position, Quaternion.identity);
            SoundManeger.Instance.isPlayMeteoHitToMeteoSe = true;
            ReSpawn();
        }
        else if(collider.gameObject.tag == "Face")
        {
            ReSpawn();
        }
        else if (collider.gameObject.tag == "Missile")
        {
            TargetReset.Instance.Reset();
            ScoreInfo.HitMeteorNormal();
            ReSpawn();
        }
	}

	private IEnumerator AddMeteo(){
		if (GameController.Instance.adMeteoNum < 1) {
			GameController.Instance.adMeteoNum++;
			yield return new WaitForSeconds (levelObj.popTime);
			GameController.Instance.isAddMeteo = true;
		}
		
	}
		
}
