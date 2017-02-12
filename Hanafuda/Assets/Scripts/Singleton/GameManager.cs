using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : SingletonMonoBehaviour<GameManager> {

    [Header("シーン変更時のフェード時間")]
    public float sceneChangeInterval;

    //[SerializeField]
    //private InterruptionDialog interruptionDialog;

    //ゲーム全体の状態
    public STATE state;

    public enum STATE {
        TITLE,
        CHAPTER_SELECT,
        STORY,
        HANAFUDA
    }

    public int storyNo;

    void Awake() {

    }

    // Use this for initialization
    void Start () {
        ////セーブデータがある場合、保存されたデータを読み込む
        //if (SaveLoadManager.Instance.CheckHasSaveData()) {
        //    interruptionDialog.SwitchStatus(InterruptionDialog.STATE.Resumption);
        //    interruptionDialog.gameObject.SetActive(true);
        //    Debug.Log("ダイアログ表示");
        //}
    }
	
	// Update is called once per frame
	void Update () {
	}
}
