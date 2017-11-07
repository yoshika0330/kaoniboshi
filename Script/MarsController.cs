using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MarsController : MonoBehaviour {

	public LevelDesignMars levelObjMove;

    public Image hp_Slider;

    [SerializeField]
    private Text gameEndText;

	private GameObject faceObj;

	private Vector3 marsPos;
	private Vector3 facePos;
	private Vector3 startPos;
	private Vector3 firstPos;
	private Vector3 nextPos;
	public Vector3 returnPosA;
	public Vector3 returnPosB;

    public int hp = 25;
	public int randomMoveNum = 0;
    private int maxHp;
	private int findPosCount = 0;
	private int listIndex = 0;
	private int moveCount = 0;
	private int i = 0;
	private int j = 0;
    private int scoreCount = 0;

	private float time = 0;
	private float elapsedTime = 0.0f;
	private float calc = 0;
	private float clamp = 0.0f;
	public float returnTime  = 0.0f;
	private float firstMoveTime = 0.0f;

	private bool isBodyBlow = true;
	private bool isMove = false;
    private bool retrunOnce = true;
	public bool isFirstMove = false;

	public List<StateBase> stateList = new List<StateBase>();
	int currentStateIndex = 0;

	bool isInit = false;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Missile")
        {
            Quaternion lookAt = Quaternion.LookRotation(collider.transform.position - transform.position);
            EffectManager.Instance.PlayEffect("explosion", collider.transform.position, lookAt);
            hp--;
            hp_Slider.fillAmount = Mathf.Lerp(0, 1, ((float)hp/maxHp));
            if(hp <= 0)
            {
                if(scoreCount < 1)
                {
                    GameController.Instance.enemyList.Clear();
                    ScoreInfo.HitMars();
                    scoreCount++;
                    SoundManeger.Instance.isPlayPlayerOrMarsDeathSe = true;
                    K_Score.Instance.hitMars = true;
                    K_Score.Instance.marsDefeatCount++;
                }
                GameController.Instance.gameStates = GameController.GameStates.End;
                gameEndText.color = new Color(1, 1, 0);
                gameEndText.text = "GameClear";
                GameController.Instance.IsWin = true;
                GameController.Instance.marsBakuhatu.transform.position = transform.position;
                GameController.Instance.marsBakuhatu.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }
    }

	void Start () {
		faceObj = GameObject.FindGameObjectWithTag ("Face");
        maxHp = hp;

		stateList.Add(new StateAttack());	// 0
		stateList.Add(new StateMove());		// 1
		stateList.Add(new StateStop());		// 2
		stateList.Add(new StateRetrun());	// 3
		stateList.Add (new StateStart ());	// 4

		//stateList.FindLast ().Enter ();
		//stateList[3].Enter ();
	}

	void Update () 
	{
        if (GameController.Instance.gameStates == GameController.GameStates.Start &&
            GameController.Instance.boosSteage)
        {
            returnPosB = new Vector3(returnPosB.x, returnPosB.y, faceObj.transform.position.z);
            if (j < 1)
            {
                stateList[4].Enter(this);
                j++;
            }

            if(j == 1 && !isMove)
            {
                stateList[4].Excute(this);
            }

            //stateList[4].Exit(this);
            if (isMove)
            {
                stateList[currentStateIndex].Excute(this);
            }
        }
	}
		
	//	体当たりの処理
	public void BodyBlow(){
		//	火星と地球の位置を取得
		if (findPosCount < 1)
		{
            Debug.Log("位置確認");
			findPosCount++;
			marsPos = transform.position;
			facePos = faceObj.transform.position;
		}
		//	火星が地球に向かって体当たり
		if (isBodyBlow) {
			transform.position = Vector3.Lerp (marsPos, facePos, time);
			time += levelObjMove.bodyBlowSpeed;
			if (time >= 1) 
            {
				isBodyBlow = false;
                time = 0;
			}
		}
		else if (!isBodyBlow && retrunOnce) 
		{
            retrunOnce = false;
            stateList[3].Enter(this);
		}
	}

	public void SpecifiedMove(){
		if (_specifinedMovement != null) 
		{
			StopCoroutine (_specifinedMovement);
		}
		_specifinedMovement = SpecifinedMovement ();
		StartCoroutine (_specifinedMovement);
	}

	IEnumerator _specifinedMovement;
	IEnumerator SpecifinedMovement()
	{
		//	複数の移動パターンから一つをランダムで取得
		listIndex = Random.Range (0, levelObjMove.moveList.Count);
		calc = 1.0f / levelObjMove.endTime;
		//	最初のポジションを取得
		startPos = transform.position;
		firstPos = startPos;
		nextPos = startPos + levelObjMove.moveList [listIndex].posList[0];

		//	移動の回数文ループさせる
		for (moveCount = 0; moveCount < levelObjMove.moveList[listIndex].posList.Count; ++moveCount) 
		{
			//	移動パターンの中の１回の移動の処理
			while (elapsedTime < levelObjMove.endTime) 
			{
				elapsedTime += Time.deltaTime;
				clamp = Mathf.Clamp01 (calc * elapsedTime);
				transform.position = Vector3.Lerp (firstPos, nextPos, clamp);
				yield return null;
			}
			//初期化
			elapsedTime = 0;

			if (moveCount + 1 != levelObjMove.moveList [listIndex].posList.Count) 
			{
				firstPos = startPos +  levelObjMove.moveList [listIndex].posList [moveCount];
				nextPos = startPos + levelObjMove.moveList [listIndex].posList [moveCount + 1];
			}
		}

		stateList [3].Enter (this);
		//MoveLottery ();
		_specifinedMovement = null;
	}

	public void MoveLottery(){
		// ステートの終了処理（初回呼び出し時は呼ばない）
		if (isInit) {
			stateList [currentStateIndex].Exit (this);
		} else {
			isInit = true;
		}
        //　体当たりするときの位置確認の一回だけ呼ぶやつの初期化
        findPosCount = 0;
        //  体当たりの初期化
        isBodyBlow = true;
        // 
        retrunOnce = true;
		// 0-99の値をランダムで決める（100文率
		randomMoveNum = Random.Range (0, 100);
		//int attackPercent = 40, moveParcent = 40;
		// 決まった値から呼び出すステートを決める
		if (randomMoveNum < levelObjMove.bodyBlowParcent) {
			currentStateIndex = 0;
		} else if (randomMoveNum < levelObjMove.bodyBlowParcent + levelObjMove.moveParcent) {
			currentStateIndex = 1;
		} else {
			currentStateIndex = 2;
		}

		//stateList [3].Exit (this);
		// ステートの初回呼び出し処理（初期化
		stateList [currentStateIndex].Enter (this);
	}

	public void StartEvent()
	{

		if (returnTime < 1) 
		{
			transform.position = Vector3.Lerp (returnPosA, returnPosB, returnTime);
			returnTime += 0.01f;
		}

		if (returnTime >= 1) 
		{
			isMove = true;
		}
	}

	public void RetrunEvent()
	{
		StartCoroutine (RetrunMove());
	}

	IEnumerator RetrunMove()
	{
		time = 0;
		for(returnTime = 0; returnTime < 1.0f; returnTime += 0.02f)
		{
			transform.position = Vector3.Lerp (returnPosA, returnPosB, returnTime);
			yield return null;
		}
        returnTime = 1;
		MoveLottery ();
	}
}
