using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelDesign/Meteo")]
public class LevelDesignMeteo : ScriptableObject {
	//	メテオの動きの速さ
	public float[] moveSpeed;
	//	何秒置きにメテオを追加するか
	public float popTime;
	//	ゲーム開始からメテオを何秒後から増やすか
	public float popStartTime;

}
