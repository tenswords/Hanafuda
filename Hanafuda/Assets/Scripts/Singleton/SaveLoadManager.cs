using UnityEngine;
using System.Collections;

///セーブロード関連
public class SaveLoadManager : SingletonMonoBehaviour<SaveLoadManager> {

	public void Awake (){
		if (this != Instance) {
			Destroy (this.gameObject);
			return;
		}

		DontDestroyOnLoad (this.gameObject);
	}




//	//セーブ関連------------------------------------------
//
//	public void SaveFloat(string key,float f){
//		PlayerPrefs.SetFloat (key,f);
//	}
//
//	public void SaveInt(string key,float i){
//		PlayerPrefs.SetFloat (key,i);
//	}
//
//	public void SaveString(string key,float s){
//		PlayerPrefs.SetFloat (key,s);
//	}
//
//	//------------------------------------------セーブ関連
//
//	//ロード関連------------------------------------------
//
//	public float LoadFloat(string key){
//		return PlayerPrefs.GetFloat(key);
//	}
//
//	public int LoadInt(string key){
//		return PlayerPrefs.GetInt(key);
//	}
//
//	public string LoadString(string key){
//		return PlayerPrefs.GetString(key);
//	}
//
//	//------------------------------------------ロード関連


	///
	/// 不要なデータを削除
	///
	public void DeleteKey(){

		//保存されていた
		if(PlayerPrefs.HasKey ("gameend_scenename")) PlayerPrefs.DeleteKey ("gameend_scenename");

		//ステージセレクトシーンのデータを削除
		if(PlayerPrefs.HasKey ("select_stageno")) PlayerPrefs.DeleteKey ("select_stageno");
		if(PlayerPrefs.HasKey ("select_monsterno")) PlayerPrefs.DeleteKey ("select_monsterno");

		//ゲームシーンのデータを削除
		if(PlayerPrefs.HasKey ("game_state")) PlayerPrefs.DeleteKey ("game_state");
		if(PlayerPrefs.HasKey ("game_oldstate")) PlayerPrefs.DeleteKey ("game_oldstate");
		if(PlayerPrefs.HasKey ("game_time")) PlayerPrefs.DeleteKey ("game_time");
		if(PlayerPrefs.HasKey ("game_maxtime")) PlayerPrefs.DeleteKey ("game_maxtime");
		if(PlayerPrefs.HasKey ("game_puzzle")) PlayerPrefs.DeleteKey ("game_puzzle");
		if(PlayerPrefs.HasKey ("backgroundname")) PlayerPrefs.DeleteKey ("backgroundname");

		//モンスターゲットシーンのデータを削除
		if(PlayerPrefs.HasKey ("monsterget_state")) PlayerPrefs.DeleteKey ("monsterget_state");
		if(PlayerPrefs.HasKey ("monsterget_getflg")) PlayerPrefs.DeleteKey ("monsterget_getflg");
		if(PlayerPrefs.HasKey ("monsterget_useitem")) PlayerPrefs.DeleteKey ("monsterget_useitem");
		if(PlayerPrefs.HasKey ("sumprobabilty")) PlayerPrefs.DeleteKey ("sumprobabilty");
	}
}
