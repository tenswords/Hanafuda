﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InterruptionDialogManager : SingletonMonoBehaviour<InterruptionDialogManager> {

    //ストーリー時のマネージャー
    private StoryManager storyManager;
    //花札時のマネージャー
    //private StoryManager storyManager;

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject interruptionDialog;

    [SerializeField]
    private Image image;

    [SerializeField]
    private Sprite[] switchImage;

    public STATE state;
    public enum STATE {
        Interruption = 0,
        Resumption = 1
    }

    void Awake() {
    }

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
	    
	}

    /// <summary>
    /// 中断時、再開時の切り替え
    /// </summary>
    public void SwitchStatus(STATE state) {
        this.state = state;
        image.sprite = switchImage[(int)state];

        if (canvas.worldCamera == null) {
            canvas.worldCamera = Camera.main;
            canvas.sortingLayerName = SortingLayer.SortingLayerManager.FRONT_DIALOG;
        }
        interruptionDialog.SetActive(true);
    }

    /// <summary>
    /// YESボタンを押したときの処理
    /// </summary>
    public void OnYesButton() {
        interruptionDialog.SetActive(false);

        switch (state) {
            case STATE.Interruption: //中断時

                switch (GameManager.Instance.state) {
                    case GameManager.STATE.STORY:

                        ReferStoryManager();

                        var talkCharaList = "";
                        foreach (var chara in storyManager.talkCharaDic) {
                            talkCharaList += chara.Key + "：" + chara.Value.sprite.name + "、";
                        }
                        SaveLoadManager.Instance.SetStorySaveData(
                            SceneName.SceneNameManager.SCENE_NAME_STORY,
                            storyManager.storyNo,
                            storyManager.lineIndex,
                            storyManager.talkCharaName,
                            talkCharaList,
                            storyManager.backGround.sprite.name
                            );
                        break;
                }

                SaveLoadManager.Instance.SaveData();

                //タイトルへ移行
                FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_TITLE,1.0f);
                break;

            case STATE.Resumption: //再開時

                //保存されたシーンに移行
                switch (PlayerPrefs.GetString(SaveLoadManager.Instance.SAVE_DATA_SCENE_NAME)) {
                    case SceneName.SceneNameManager.SCENE_NAME_STORY:
                        //ストーリーへ移行
                        FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_STORY, GameManager.Instance.sceneChangeInterval);
                        break;
                }
                break;
        }

    }

    /// <summary>
    /// NOボタンを押したときの処理
    /// </summary>
    public void OnNoButton() {
        interruptionDialog.SetActive(false);

        switch (state) {
            case STATE.Interruption: //中断時

                switch (GameManager.Instance.state) {
                    case GameManager.STATE.STORY:

                        ReferStoryManager();

                        storyManager.InterruptionCancel();
                        break;
                }

                break;
            case STATE.Resumption: //再開時

                switch (PlayerPrefs.GetString(SaveLoadManager.Instance.SAVE_DATA_SCENE_NAME)) {
                    case SceneName.SceneNameManager.SCENE_NAME_STORY:
                        //チャプター選択へ移行
                        FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_CHAPTER_SELECT, GameManager.Instance.sceneChangeInterval);
                        break;
                }

                //保存していたデータを全て削除
                SaveLoadManager.Instance.DeleteAllData();

                break;
        }
    }

    /// <summary>
    /// ストーリーマネージャーを参照していなければ、参照させる
    /// </summary>
    private void ReferStoryManager() {
        if (storyManager == null) {
            storyManager = GameObject.Find("StoryManager").GetComponent<StoryManager>();
        }
    }

}
