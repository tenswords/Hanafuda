using UnityEngine;
using System.Collections;

///セーブロード関連
public class SaveLoadManager : SingletonMonoBehaviour<SaveLoadManager> {

    [SerializeField]
    private bool isDebug;

    public string SAVE_DATA_INTERRUPTION = "interruption";

    public string SAVE_DATA_SCENE_NAME = "sceneName";
    public string SAVE_DATA_STORY_NO = "storyNo";
    public string SAVE_DATA_LINE_INDEX = "lineIndex";
    public string SAVE_DATA_TALK_CHARA_NAME = "talkCharaName";
    public string SAVE_DATA_TALK_CHARA_LIST = "talkCharaList";
    public string SAVE_DATA_BACK_GROUND = "backGround";

    [Header("saveData_Interruption == 1 デバッグ用（saveDataの入力必要）")]
    public int saveData_Interruption;

    public string savaData_SceneName;
    public int saveData_StoryNo;
    public int saveData_LineIndex;
    public string saveData_TalkCharaName;
    public string saveData_TalkCharaList;
    public string saveData_BackGround;

    [SerializeField]
    private GameManager gameManager;

    void Awake() {
        //if (isDebug) {
        //    PlayerPrefs.DeleteAll();

        //    if (saveData_Interruption == 1) {
        //        //デバッグ
        //        PlayerPrefs.SetInt(SAVE_DATA_INTERRUPTION, saveData_Interruption);
        //        PlayerPrefs.SetString(SAVE_DATA_SCENE_NAME, savaData_SceneName);
        //        PlayerPrefs.SetInt(SAVE_DATA_STORY_NO, saveData_StoryNo);
        //        PlayerPrefs.SetInt(SAVE_DATA_LINE_INDEX, saveData_LineIndex);

        //        if (saveData_TalkCharaName != "") {
        //            PlayerPrefs.SetString(SAVE_DATA_TALK_CHARA_NAME, saveData_TalkCharaName);
        //        }
        //        if (saveData_TalkCharaList != "") {
        //            PlayerPrefs.SetString(SAVE_DATA_TALK_CHARA_LIST, saveData_TalkCharaList);
        //        }
        //        PlayerPrefs.SetString(SAVE_DATA_BACK_GROUND, saveData_BackGround);

        //        Debug.Log("デバッグON");
        //    }
        //    isDebug = false;
        //}
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

    /// <summary>
    /// セーブデータを登録
    /// </summary>
    public void SetStorySaveData(string sceneName,int storyNo, int lineIndex,string talkCharaName, string talkCharaList,string backGround) {

        saveData_Interruption = 1;
        PlayerPrefs.SetInt(SAVE_DATA_INTERRUPTION, saveData_Interruption);

        savaData_SceneName = sceneName;
        saveData_StoryNo = storyNo;
        saveData_LineIndex = lineIndex;
        saveData_TalkCharaName = talkCharaName;
        saveData_TalkCharaList = talkCharaList;
        saveData_BackGround = backGround;
    }

    /// <summary>
    /// データを保存
    /// </summary>
    public void SaveData() {

        switch (gameManager.state) {
            case GameManager.STATE.STORY:

                PlayerPrefs.SetString(SAVE_DATA_SCENE_NAME, savaData_SceneName);                
                PlayerPrefs.SetInt(SAVE_DATA_STORY_NO, saveData_StoryNo);
                PlayerPrefs.SetInt(SAVE_DATA_LINE_INDEX, saveData_LineIndex);

                if (saveData_TalkCharaName != "") {
                     PlayerPrefs.SetString(SAVE_DATA_TALK_CHARA_NAME, saveData_TalkCharaName);
                }
                if (saveData_TalkCharaList != "") {
                    PlayerPrefs.SetString(SAVE_DATA_TALK_CHARA_LIST, saveData_TalkCharaList);
                }
                PlayerPrefs.SetString(SAVE_DATA_BACK_GROUND, saveData_BackGround);

                break;
            case GameManager.STATE.HANAFUDA:
                break;
        }
    }

    ///// <summary>
    ///// データの読み込み
    ///// </summary>
    //public void LoadData() {



    //    switch (gameManager.state) {
    //        case GameManager.STATE.STORY:
    //            PlayerPrefs.GetInt(SAVE_DATA_STORY_NO);
    //            PlayerPrefs.GetInt(SAVE_DATA_LINE_INDEX);
    //            PlayerPrefs.GetString(SAVE_DATA_TALK_CHARA_LIST);

    //            break;
    //        case GameManager.STATE.HANAFUDA:
    //            break;
    //    }
    //}

    /// <summary>
    /// データを削除
    /// </summary>
    public void DeleteKey() {

    }

    /// <summary>
    /// データを全削除
    /// </summary>
    public void DeleteAllData() {
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// セーブデータがあるかどうかを取得
    /// </summary>
    /// <returns></returns>
    public bool CheckHasSaveData() {
        return PlayerPrefs.HasKey(SAVE_DATA_INTERRUPTION);
    }
}