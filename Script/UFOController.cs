using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOController : MonoBehaviour {


	private GameObject faceObj;

	private bool isMove = false;
	private bool isBack = true;
	private bool isSpawn = false;
	private bool isStop = false;
	private bool isTimeCount = false;

	private int randomMoveNum = 0;
	private int randomSpawnNum = 0;
	private int findPosCount = 0;
    private int hp;

    public int maxHp = 5;

	private float moveTime = 0;
	private float randomX, randomY;
	private float ufoX,ufoY;
	private float faceX,faceY;
	private float rad,angle;
	private float time;

	private Vector3 ufoPos,facePos;

    public float ufoPopTime = 3;

	void OnBecameVisible ()
	{
		//	画面外に消えて戻ってきたら
		if (isBack) {
			StartCoroutine (ComeBack());
		}
	}

	void OnBecameInvisible ()
	{
		//	画面外に消えたらBackフラグを立てる。
		isBack = true;
		moveTime = 0;
	}

	void OnTriggerEnter(Collider collider) { 
		if (collider.gameObject.tag == "Face")
        {
			ReSpawn ();
            if(GameController.Instance.gamePhase != GameController.GamePhase.Phase4)
            {
                GameController.Instance.enemyList.Remove(this.gameObject);
                Destroy(this.gameObject);
            }
		}

        else if(collider.gameObject.tag == "Missile")
        {
            TargetReset.Instance.Reset();
            ScoreInfo.HitUfo();
            K_Score.Instance.hitUfo = true;
            K_Score.Instance.ufoDefeatCount++;
            hp--;
            if(hp <= 0)
            {
                ReSpawn();
                if (GameController.Instance.gamePhase != GameController.GamePhase.Phase4)
                {
                    GameController.Instance.enemyList.Remove(this.gameObject);
                    Destroy(this.gameObject);
                }
            }
        }
	}

	void Start () {
		faceObj = GameObject.FindGameObjectWithTag ("Face");
        hp = maxHp;
	}

	void Update () {
        if (GameController.Instance.gameStates == GameController.GameStates.Start)
        {
            //	動ける時に２種類のうち１種類の動きをする
            StartCoroutine(MoveUFOType());

            if (isMove)
            {
                switch (randomMoveNum)
                {
                    case 0:
                        transform.position += new Vector3(randomX, randomY, 0);
                        transform.position = new Vector3(transform.position.x,
                                         transform.position.y,
                                         faceObj.transform.position.z);
                        break;
                    case 1:
                        GoFace();
                        break;
                }
                
            }

            //	画面外に行ったら一回だけ自分と顔の位置を把握する
            if (isBack && findPosCount == 0)
            {
                findPosCount++;
                FindPos();
            }

            //	画面外に出たら顔に戻ろうとする
            if (isBack)
            {
                isMove = false;
                GoFace();
            }


            if (isSpawn)
            {
                //	スポーンさせるための数字をランダムで生成
                randomSpawnNum = Random.Range(0, 4);
                RandomSpawn();		//	決められた位置にランダムでスポーン
                FindPos();
                moveTime = 0;
                isSpawn = false;
            }

            if (GameController.Instance.isAddUfo)
            {
                ReSpawn();
                isBack = true;
                GameController.Instance.isAddUfo = false;
            }

            //if (GameController.Instance.timeCount >= 10)
            if (GameController.Instance.gamePhase == GameController.GamePhase.Phase4)
            {
                StartCoroutine(AddUFO());
            }
        }
	}

	//	0.5秒に2種類の動きから一つをランダムで選択
	private IEnumerator MoveUFOType(){
		if (isStop == false && isBack == false) {
			isStop = true;
			yield return new WaitForSeconds (2f);
			randomMoveNum = Random.Range (0, 2);
			switch (randomMoveNum) {
			case 0:
				RandomPos ();
				break;
			case 1:
				moveTime = 0;
				FindPos ();
				break;
			}
			isStop = false;
		}
	}
		
	private IEnumerator ComeBack(){
		yield return new WaitForSeconds (0.1f);
		isBack = false;
		isMove = true;
		findPosCount = 0;
	}
		

	//	ランダムな値を入れる
	void RandomPos(){
		randomX = Random.Range (-0.1f, 0.1f);
		randomY = Random.Range (-0.1f, 0.1f);
	}

	//	UFOと顔の位置を取得
	void FindPos(){
		ufoPos = transform.position;
		facePos = faceObj.transform.position;
	}

	//	顔に向かって移動
	void GoFace(){
		transform.position = Vector3.Lerp (ufoPos, facePos, moveTime);
        transform.position = new Vector3(transform.position.x,
                                         transform.position.y,
                                         faceObj.transform.position.z);
		moveTime += 0.01f;
	}

	// -------------------------------------------------------------------------------------------------------------------------
	//	上下左右それぞれの位置にUFOを移動
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
	// ------------------------------------------------------------------------------------------------------------------------

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
        if (GameController.Instance.gamePhase == GameController.GamePhase.Phase4)
        {
            hp = maxHp;
            isMove = false;									//	動きを止める
            GetComponent<MeshRenderer>().enabled = false;	//	非表示にする
            isSpawn = true;									//	スポーンさせる
            GetComponent<MeshRenderer>().enabled = true;	//	表示させる
        }
	}

	private IEnumerator AddUFO()
	{
		if (GameController.Instance.isPlay && GameController.Instance.addUFONum == 0) 
		{
			GameController.Instance.addUFONum++;
			yield return new WaitForSeconds (ufoPopTime);
			GameController.Instance.isAddUfo = true;
		}
	}
}
