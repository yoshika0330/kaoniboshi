using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoPopContoler : MonoBehaviour {

    [SerializeField]
    private GameObject meteo;

    [SerializeField]
    private Vector2 leftPopPos;
    [SerializeField]
    private Vector2 rightPopPos;
    private Vector2 randomPop;

    [SerializeField]
    private int maxPopNum = 4;
    [SerializeField]
    private int minPopNum = 1;
    [SerializeField]
    private int popTime = 1;

    private bool once = true;


	void Start () 
    {
		
	}
	
	void Update () 
    {
		if(GameController.Instance.gamePhase == GameController.GamePhase.Phase3 && GameController.Instance.gameStates == GameController.GameStates.Start)
        {
            StartCoroutine(PopUp());
        }
	}

    IEnumerator PopUp()
    {
        if (once)
        {
            once = false;
            yield return new WaitForSeconds(popTime);
            for (int i = 0; i < Random.Range(minPopNum, (maxPopNum + 1)); i++)
            {
                randomPop.x = Random.Range(leftPopPos.x, rightPopPos.x);
                randomPop.y = Random.Range(leftPopPos.y, rightPopPos.y);
                GameObject Ms = Instantiate(meteo, new Vector2(randomPop.x, randomPop.y), Quaternion.Euler(60,-90,0));
                //GameController.Instance.enemyList.Find("meteor");
                //Ms.GetComponent<MeteorShowerControlloer>().id = 
                GameController.Instance.enemyList.Add(Ms);
                yield return null;
            }
            once = true;
        }
    }
}
