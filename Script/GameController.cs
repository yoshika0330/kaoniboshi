using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	Camera mainCamera;
    [SerializeField]
    private ScoreGraph SGraph;

    public GameObject gameOverText;

    public GameObject earth_HP;
    public GameObject kasei_HP;

	public GameObject addMeteoObj;
    public GameObject addUFOObj;
    public GameObject bosMars;
    public GameObject earthBakuhatu;
    public GameObject marsBakuhatu;
    public GameObject titleImage;
    public GameObject allUi;
    public GameObject scoreUi;
    public GameObject[] textScore;

    public Text promptReportText;

    [SerializeField]
    private Animator promptReport_Anim;

    [SerializeField]
    private string[] news;

	[HideInInspector]
	public float timeCount;
	[HideInInspector]
	public int scoreCount = 0;
	[HideInInspector]
	public int adMeteoNum;

	[HideInInspector]
	public float cameraX, cameraY;
	[HideInInspector]
	public float randomX,randomY;

	[HideInInspector]
	public bool isScoreCount = false;
	[HideInInspector]
	public bool isAddMeteo = false;
	[HideInInspector]
	public bool isPlay = true;
    [HideInInspector]
    public bool isAddUfo = false;
    [HideInInspector]
    public bool boosSteage = false;
    
    private bool isAddMeteoCount = false;
    private bool isAddUFOCount = false;

    private bool isReport = false;

    public List<GameObject> enemyList = new List<GameObject>();

    [HideInInspector]
    public int addUFONum = 0;

	private float time;

    private int firstMeteo = 0;

    private int addMeteos = 0;
    private int addUFOs = 0;
    private int ufoPopCount = 0;
    private int marsPop = 0;
    private int onceDispScore = 0;
    private int nowPhase = 0;
    private int newsNum = 0;
    [SerializeField]
    private int changeFeseTime = 30;

    private int i = 0;
    private int j = 0;

    private int hitPoint;
    public int HitPoint
    {
        get { return hitPoint; }
        set { hitPoint = value; }
    }

    public enum GameStates
    {
        Title,
        Start,
        End,
        Result
    }
    public GameStates gameStates = GameStates.Title;

    private bool isSoundStop = false;
    
    public enum GamePhase :int
    {
        PromptReport,
        Phase1,
        Phase2,
        Phase3,
        Phase4,
        Phase5
    }
    public GamePhase gamePhase;

    private bool isStart;
    public bool IsStart
    {
        get { return isStart; }
        set { isStart = value; }
    }

    private bool isWin = false;
    public bool IsWin
    {
        get { return isWin; }
        set { isWin = value; }
    }

	#region Singlton
	public static GameController Instance{ get; private set; }
	void Awake()
	{
		if (Instance != null) 
		{
			enabled = false;
			DestroyImmediate (this);
			return;
		}
		Instance = this;
        ScoreInfo.Read();
	}
	#endregion

	void Start () {
		mainCamera = Camera.main.GetComponent<Camera> ();

		//	カメラの写ってる中心の位置からX軸に見切れるまでの距離
		cameraX = mainCamera.orthographicSize * 1.8f;
		//	カメラの写ってる中心の位置からY軸に見切れるまでの距離
		cameraY = mainCamera.orthographicSize;

	}

	void Update () {

        if (Input.GetKeyDown(KeyCode.R))
        {
            ScoreInfo.ResetHighScore();
        }

		//	カメラが写してる左端から右端まで距離のランダム
		randomX = Random.Range (-cameraX, cameraX);
		//	カメラが写してる上から下までの距離のランダム
		randomY = Random.Range (-cameraY, cameraY);

		//	スコア加算
		if (isScoreCount) {
			scoreCount++;
			isScoreCount = false;
		}

        // フェーズ1か2の時の処理
        if (gamePhase == GamePhase.Phase1 ||
            gamePhase == GamePhase.Phase2)
        {
            //　一回だけ呼ぶようにするif文
            if (isAddMeteo)
            {
                isAddMeteoCount = true;
                isAddMeteo = false;
                //　隕石を一回の生成で複数個生成するときの処理
                for (int i = 0; i < addMeteos; i++)
                {
                    //　隕石を生成
                    GameObject meteoClorn = Instantiate(addMeteoObj);
                    //　生成された隕石をエネミーリストに追加する
                    enemyList.Add(meteoClorn);
                }
                adMeteoNum = 0;
            }


            if (isAddMeteoCount)
            {
                //　一回だけ呼ぶようにするif文
                if (i == 0)
                {
                    i++;
                    StartCoroutine(addMeteoCount());
                }
            }
        }

        //　フェーズが4の時の処理
        if (gamePhase == GamePhase.Phase4)
        {
            //　一回だけ呼ぶようにするif文
            if (isAddUfo)
            {
                isAddUFOCount = true;
                isAddUfo = false;
                //　UFOを一回の生成で複数個生成する処理
                for (int i = 0; i < addUFOs; i++)
                {
                    //　UFOを生成
                    GameObject ufoClorn = Instantiate(addUFOObj, new Vector3(0,-10,0),Quaternion.identity);
                    //　生成されたUFOをエネミーリストに追加
                    enemyList.Add(ufoClorn);
                }
                addUFONum = 0;
            }

            if (isAddUFOCount)
            {
                //　一回だけ呼ぶようにするif文
                if (j == 0)
                {
                    j++;
                    StartCoroutine(addUFOCount());
                }
            }
        }

        //　ゲームステートごとの処理
        switch (gameStates)
        {
            //　ステートがタイトルの場合
            case GameStates.Title:
                //　地球のHPのUIを非表示にする
                earth_HP.SetActive(false);
                //　タイトルのBGMを流す
                BgmManeger.Instance.Changed(0);
                //　スタートのフラグがたったら
                if (IsStart)
                {
                    //　タイトルのUIをフェードアウトさせる
                    titleImage.GetComponent<FadeScript>().FadeOut();
                    //　ゲームステートをスタートに変える
                    gameStates = GameStates.Start;
                }
                break;
            //　ステートがスタートの場合
            case GameStates.Start:
                //　タイマーを起動する
                Timer();
                //　地球のHPのUIを表示させる
                earth_HP.SetActive(true);
                
                //　経過時間がフェーズを変える時間以上になったら
                if (time >= changeFeseTime)
                {
                    //　その時にフェーズが５だった場合は抜ける
                    if (gamePhase == GamePhase.Phase5) break;
                    //　ゲームフェーズを速報を出すフェーズに変える
                    gamePhase = GamePhase.PromptReport;
                    isReport = false;
                    //　経過時間の初期化
                    time = 0;
                }
                
                //　ゲームステートがスタート中のゲームフェーズごとの処理
                switch (gamePhase)
                {
                   //　ゲームフェーズが速報を出すフェーズだった場合
                    case GamePhase.PromptReport:
                        //　isReportがtrueじゃなかったら
                        if(!isReport)
                        {
                            //　EndPromptReportのコルーチンをスタート
                            StartCoroutine(EndPromptReport());
                            isReport = true;
                        }
                        break;
                    //　ゲームフェーズが1の場合
                    case GamePhase.Phase1: 
                        //  初期隕石を３つ生成
                        if (firstMeteo < 3)
                        {
                            firstMeteo++;
                            //　隕石を生成
                            GameObject meteoClorn = Instantiate(addMeteoObj);
                            //　生成した隕石をエネミーリストに追加
                            enemyList.Add(meteoClorn);
                        }
                        break;
                    case GamePhase.Phase2:
                        break;
                    case GamePhase.Phase3:
                        break;
                    //　ゲームフェーズが4の場合
                    case GamePhase.Phase4:
                        if (timeCount >= 5 && ufoPopCount < 3)
                        {
                            ufoPopCount++;
                            //　UFOを生成する処理
                            GameObject ufoClorn = Instantiate(addUFOObj, new Vector3(0, -10, 0), Quaternion.identity);
                            //　生成したUFOをエネミーリストに追加
                            enemyList.Add(ufoClorn);
                            isAddUfo = true;
                        }
                        break;
                    //　ゲームフェーズが5の場合
                    case GamePhase.Phase5:
                        //　一回だけ呼ぶif文
                       if(marsPop == 0)
                       {
                           marsPop++;
                           //　火星の出現
                           bosMars.gameObject.SetActive(true);
                           //　火星のHPのUIを表示
                           kasei_HP.SetActive(true);
                           //　エネミーリストに火星を追加
                           enemyList.Add(bosMars);
                       }
                        boosSteage = true;
                        break;
                }
                break;
            
            //　ゲームステートがエンドの場合
            case GameStates.End:
                //　ゲームクリアかゲームオーバの文字が入るのテキストを表示
                gameOverText.SetActive(true);
                //　chengeResultのコルーチンをスタートする
                StartCoroutine(chengeResult());
                break;
            //  ゲームステートがリザルトの場合
            case GameStates.Result:
                //　何かしらの入力があるとタイトルに戻る
                if (Input.anyKeyDown)
                {
                    ScoreInfo.Score = new Score();
                    SceneManager.LoadScene("MainScene");
                }
                //　一回だけ呼ぶif文
                if (onceDispScore < 1)
                {
                    SGraph.StartGraph();
                    onceDispScore++;
                    //　評価を表示させる
                    ResultController.Instance.Param();
                    //　スコアを表示させる
                    dispScore();
                }
                break;
        }
	}
    
    void Timer()
    {   
        //　フェーズごとの経過時間を測る
        time += Time.deltaTime;
        //　ゲームのトータルの経過時間を測る
        timeCount +=Time.deltaTime;
    }

    //　ｎ秒ごとに隕石が一度に増える数を増やす
    private IEnumerator addMeteoCount()
    {
        yield return new WaitForSeconds(10);
        addMeteos++;
        i = 0;
    }

    //　ｎ秒ごとにUFOが一度に増える数を増やす
    private IEnumerator addUFOCount()
    {
        yield return new WaitForSeconds(10);
        addUFOs++;
        j = 0;
    }

    //　リザルトに変える
    private IEnumerator chengeResult()
    {
        //----------------------------------------
        //ゲームオーバ時とゲームクリア時で曲を変える
        BgmManeger.Instance.audioSource.Stop();
        if (isWin)
        {
            BgmManeger.Instance.Changed(3);
        }
        else
        {
            BgmManeger.Instance.Changed(4);
        }
        //----------------------------------------

        //　4秒後にリザルトに入る
        yield return new WaitForSeconds(4);
        //　ゲームステートをリザルトに変える
        gameStates = GameStates.Result;
    }

    //　スコアを表示させる時の処理
    private void dispScore()
    {
        //　他のUIをすべて非表示にする
        allUi.SetActive(false);
        //　スコアのUIを表示させる
        scoreUi.SetActive(true);
        //　スコアのアニメーションを再生させる
        scoreUi.GetComponent<Animator>().SetBool("isAnimStart", true);
        ScoreInfo.Score.hp = hitPoint;
        ScoreInfo.AddToHighScores();

        for (int i=0;i<ScoreInfo.Scores.Count;i++)
        {
            textScore[i].GetComponent<Text>().text = ScoreInfo.GetTotalScore(ScoreInfo.Scores[i]).ToString();
            if (ScoreInfo.Scores[i] == ScoreInfo.Score)
            {
                //　自分のスコアがランクインしたら色を変える
                textScore[i].GetComponent<Text>().color = new Color(255, 0, 0);
            }
        }
    }

    //　速報を流す処理
    private IEnumerator EndPromptReport()
    {
        //　速報のアニメーションを再生させる
        promptReport_Anim.SetBool("isSokuhou", true);

        //---------------------------------------------
        //　1.5秒後にそれぞれのテキストを表示させる
        yield return new WaitForSeconds(1.5f);
        if (newsNum < news.Length)
        {
            //　速報の内容をテキストに入れる
            promptReportText.text = news[newsNum];
        }
        //　速報の内容を表示させる
        promptReportText.enabled = true;
        //---------------------------------------------

        //　-------------------------------------------------
        //　３秒後にフェーズを変える
        yield return new WaitForSeconds(3);
        switch(nowPhase)
        {
            case 0:
                gamePhase = GamePhase.Phase1;
                nowPhase = 1;
                if (!isSoundStop)
                {
                    //　BGMを道中のBGMに変える
                    BgmManeger.Instance.audioSource.Stop();
                    BgmManeger.Instance.Changed(1);
                    isSoundStop = true;
                }
                break;
            case 1:
                gamePhase = GamePhase.Phase2;
                nowPhase = 2;
                break;
            case 2:
                gamePhase = GamePhase.Phase3;
                nowPhase = 3;
                break;
            case 3:
                gamePhase = GamePhase.Phase4;
                nowPhase = 4;
                isSoundStop = false;
                break;
            case 4:
                gamePhase = GamePhase.Phase5;
                if (!isSoundStop)
                {
                    //　BGMをボスのBGMに変える
                    BgmManeger.Instance.audioSource.Stop();
                    BgmManeger.Instance.Changed(2);
                    isSoundStop = true;
                }
                isSoundStop = false;
                break;
        }
        //　-------------------------------------------------

        //速報の内容を非表示にする
        promptReportText.enabled = false;
        //速報の内容を次の内容にする
        newsNum++;
        //速報のアニメーションを逆再生させる
        promptReport_Anim.SetBool("isSokuhou", false);
    }
}
