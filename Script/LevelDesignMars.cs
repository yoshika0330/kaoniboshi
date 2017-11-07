using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MovePositions
{
	public List<Vector3> posList;
}

[CreateAssetMenu (menuName = "LevelDesign/Mars/Move")]
public class LevelDesignMars : ScriptableObject {

	public List<MovePositions> moveList = new List<MovePositions>();
	public float endTime = 3.0f;
    public float bodyBlowSpeed = 0.025f;
    public int bodyBlowParcent = 60;
    public int moveParcent = 30;

	public Vector3 defaultPos;

}

public class StateBase
{
	// 最初に呼ばれる
	public virtual void Enter(MarsController owner)
	{
	}

	// Update内で呼ばれる
	public virtual void Excute(MarsController owner)
	{
	}

	// 最後に呼ばれる
	public virtual void Exit(MarsController owner)
	{
	}
}

public class StateAttack : StateBase
{
	public override void Enter (MarsController owner)
	{
		
	}

	public override void Excute(MarsController owner)
	{
		owner.BodyBlow ();
	}
		
}

public class StateMove : StateBase
{

	public override void Enter (MarsController owner)
	{
		owner.SpecifiedMove ();
	}

	public override void Excute(MarsController owner)
	{
		
	}
}

public class StateStop : StateBase
{
	float elapsedTime = 0.0f;

	public override void Excute (MarsController owner)
	{
		elapsedTime += Time.deltaTime;
		if (elapsedTime > 2.0f) {
			owner.MoveLottery ();
		}
	}

	public override void Exit (MarsController owner)
	{
		elapsedTime = 0.0f;
	}
}

public class StateStart : StateBase
{
	public override void Enter (MarsController owner)
	{
		// ここで初期化
		owner.returnPosA = owner.transform.position;
	}

	public override void Excute (MarsController owner)
	{
		//画面外から画面内に移動する処理
		owner.StartEvent();
	}

	public override void Exit (MarsController owner)
	{
		// 終了時になんかしたい場合
	}
}

public class StateRetrun : StateBase
{
	public override void Enter (MarsController owner)
	{
		owner.returnPosA = owner.transform.position;
		owner.returnTime = 0;
		owner.RetrunEvent ();
	}

	public override void Excute (MarsController owner)
	{
		//owner.RetrunEvent ();
	}

	public override void Exit (MarsController owner)
	{
		owner.returnTime = 0;
	}
}