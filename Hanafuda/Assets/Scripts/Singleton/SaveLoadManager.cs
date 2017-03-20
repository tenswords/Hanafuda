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

    public string SAVE_DATA_WIN_PLAYER = "winPlayer";
    public string SAVE_DATA_FLUSH_DATA = "flushData";
    public string SAVE_DATA_PLAYER_SCORE = "playerScore";
    public string SAVE_DATA_CPU_SCORE = "cpuScore";
    public string SAVE_DATA_IS_RESULT = "isResult";

    public int saveData_Interruption;

    public string savaData_SceneName;
    public int saveData_StoryNo;
    public int saveData_LineIndex;
    public string saveData_TalkCharaName;
    public string saveData_TalkCharaList;
    public string saveData_BackGround;

    public string saveData_WinPlayer;
    public string saveData_FlushData;
    public int saveData_PlayerScore;
    public int saveData_CpuScore;
    public string saveData_IsResult;

    [SerializeField]
    private GameManager gameManager;

    void Awake() {
        if (isDebug) {
            SaveData();
        }
    }

    /// <summary>
    /// ストーリー時のセーブデータを登録
    /// </summary>
    public void SetStorySaveData(string sceneName,int storyNo, int lineIndex,string talkCharaName, string talkCharaList,string backGround) {

        PlayerPrefs.DeleteAll();

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
    /// ストーリー時のセーブデータを登録
    /// </summary>
    public void SetStorySaveData(string sceneName, int storyNo, int lineIndex) {

        PlayerPrefs.DeleteAll();

        saveData_Interruption = 1;
        PlayerPrefs.SetInt(SAVE_DATA_INTERRUPTION, saveData_Interruption);

        savaData_SceneName = sceneName;
        saveData_StoryNo = storyNo;
        saveData_LineIndex = lineIndex;
    }

    /// <summary>
    /// 花札からストーリーへ移行するする際のセーブデータを登録
    /// </summary>
    public void SetStorySaveData(string sceneName) {

        saveData_Interruption = 1;
        PlayerPrefs.SetInt(SAVE_DATA_INTERRUPTION, saveData_Interruption);

        savaData_SceneName = sceneName;
    }

    /// <summary>
    /// 花札時のセーブデータを登録
    /// </summary>
    public void SetHanafudaData(string sceneName,string winPlayer,string flushData,int playerScore,int cpuScore, string isResult) {

        saveData_Interruption = 1;
        PlayerPrefs.SetInt(SAVE_DATA_INTERRUPTION, saveData_Interruption);

        savaData_SceneName = sceneName;
        saveData_WinPlayer = winPlayer;
        saveData_FlushData = flushData;
        saveData_PlayerScore = playerScore;
        saveData_CpuScore = cpuScore;
        saveData_IsResult = isResult;
    }

    /// <summary>
    /// 花札時のセーブデータを登録
    /// </summary>
    public void SetHanafudaData(string sceneName, string isResult) {
        saveData_Interruption = 1;
        PlayerPrefs.SetInt(SAVE_DATA_INTERRUPTION, saveData_Interruption);

        savaData_SceneName = sceneName;
        saveData_IsResult = isResult;
    }

    /// <summary>
    /// データを保存
    /// </summary>
    public void SaveData() {

        PlayerPrefs.SetString(SAVE_DATA_SCENE_NAME, savaData_SceneName);

        Debug.Log("PsaveScene " + PlayerPrefs.GetString(SAVE_DATA_SCENE_NAME));

        switch (gameManager.state) {
            case GameManager.STATE.STORY:

                PlayerPrefs.SetInt(SAVE_DATA_STORY_NO, saveData_StoryNo);
                PlayerPrefs.SetInt(SAVE_DATA_LINE_INDEX, saveData_LineIndex);
                
                if (saveData_TalkCharaName != "") PlayerPrefs.SetString(SAVE_DATA_TALK_CHARA_NAME, saveData_TalkCharaName);
                if (saveData_TalkCharaList != "") PlayerPrefs.SetString(SAVE_DATA_TALK_CHARA_LIST, saveData_TalkCharaList);
                if (saveData_BackGround != "") PlayerPrefs.SetString(SAVE_DATA_BACK_GROUND, saveData_BackGround);

                break;

            case GameManager.STATE.HANAFUDA:

                if (saveData_WinPlayer != "") PlayerPrefs.SetString(SAVE_DATA_WIN_PLAYER, saveData_WinPlayer);
                if (saveData_FlushData != "") PlayerPrefs.SetString(SAVE_DATA_FLUSH_DATA, saveData_FlushData);

                PlayerPrefs.SetInt(SAVE_DATA_PLAYER_SCORE, saveData_PlayerScore);
                PlayerPrefs.SetInt(SAVE_DATA_CPU_SCORE, saveData_CpuScore);
                PlayerPrefs.SetString(SAVE_DATA_IS_RESULT, saveData_IsResult);

                break;
        }
    }

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

    /// <summary>
    /// セーブしたシーン名を取得
    /// </summary>
    /// <returns></returns>
    public string GetSaveScene() {
        return PlayerPrefs.GetString(SAVE_DATA_SCENE_NAME,"");
    }
}