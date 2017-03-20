using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CommandManager : MonoBehaviour {

    [SerializeField]
    private StoryManager storyManager;

    private const string COMMAND_START_WARD = "[";
    private const string COMMAND_END_WARD = "]";

    private const string COMMAND_BACK_GROUND = "BackGround";
    private const string COMMAND_TALK_NAME = "TalkName";
    private const string COMMAND_CHARA_FACE = "CharaFace";
    private const string COMMAND_CHARA_HIDE = "CharaHide";
    private const string COMMAND_NEW_LINE = "NewLine";
    private const string COMMAND_CHAPTER_END = "ChapterEnd";
    private const string COMMAND_HANAFUDA_START = "Hanafuda_Start";
    private const string COMMAND_HANAFUDA_END = "Hanafuda_End";
    private const string COMMAND_PLAY_BGM = "PlayBGM";
    private const string COMMAND_STOP_BGM = "StopBGM";
    private const string COMMAND_PLAY_SE = "PlaySE";
    private const string COMMAND_STOP_SE = "StopSE";

    [SerializeField]
    private Image[] talkCharaImageList;

    [SerializeField]
    private float BACKGROUND_FADE_TIME;
    [SerializeField]
    private float CHARAFACE_FADE_TIME;

    [SerializeField]
    private Color DEFAULT_COLOR;
    [SerializeField]
    private Color ALPHA_COLOR;
    [SerializeField]
    private Color NOT_TALK_CHARA_COLOR;

    //だんだん暗くする処理を行うかどうかのフラグ
    private bool isCharaFade;
    //だんだん明るくする処理を行うかどうかのフラグ
    private bool isCharaHide;

    private const string ERASE_INTERRUPTION_BUTTON = "Talk_Hanafuda1";
    [SerializeField]
    private Button interruptionButton;

    // Use this for initialization
    void Start() {
    }

    /// <summary>
    /// 指定した行の文字列にコマンドの開始文字が含まれているかどうかを取得
    /// </summary>
    public bool IsContainsCommandWard(string line) {

        if (line.Contains(COMMAND_START_WARD)) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// コマンドを解析し、そのコマンドの開始から終了までの文字列を取得
    /// </summary>
    public string GetCommand(string line, int commnadStartWardIndex = 0) {

        var commandEndWardIndex = line.LastIndexOf(COMMAND_END_WARD);
        var command = line.Substring(commnadStartWardIndex + 1, commandEndWardIndex - (commnadStartWardIndex + 1));

        return command;
    }

    /// <summary>
    /// 指定したコマンドが改行のコマンドかどうかを取得
    /// </summary>
    public bool IsNewLineCommand(string commnad) {
        if (commnad == COMMAND_NEW_LINE) return true;
        return false;
    }

    /// <summary>
    /// コマンド解析＆処理
    /// </summary>
    public void CommandAnalize(string command) {

        var analize = command.Split("="[0]);

        switch (analize[0]) {
            case COMMAND_BACK_GROUND: CommnadBackGround(analize[1]); break;
            case COMMAND_TALK_NAME: CommandTalkName(analize[1]); break;
            case COMMAND_CHARA_FACE: CommandCharaFace(analize[1]); break;
            case COMMAND_CHARA_HIDE: CommandCharaHide(analize[1]); break;
            case COMMAND_NEW_LINE: storyManager.CommandNewLine(true); break;
            case COMMAND_CHAPTER_END: CommandChapterEnd(); break;
            case COMMAND_HANAFUDA_START: CommandHanafudaStart(); break;
            case COMMAND_PLAY_BGM: CommandPlayBGM(analize[1]); break;
            case COMMAND_STOP_BGM: CommandStopBGM(); break;
            case COMMAND_PLAY_SE: CommandPlaySE(analize[1]); break;
            case COMMAND_STOP_SE: CommandStopSE(); break;
        }
    }

    /// <summary>
    /// [BackGround]コマンドの処理
    /// </summary>
    private void CommnadBackGround(string nextSpriteName) {

        //if(nextSpriteName == "Color_Black")

        var isBackGroundFade = true;
        if (storyManager.lineIndex == 0) {
            isBackGroundFade = false;

        } else {

            var lineTmp = storyManager.GetTargetLine(storyManager.lineIndex - 1);

            //一つ前の行が、[HanafudaEnd]コマンドだったら、フェードを行わないように設定
            if (IsContainsCommandWard(lineTmp)) {
                var command = GetCommand(lineTmp);
                if (command == COMMAND_HANAFUDA_END) {
                    isBackGroundFade = false;
                }
            }
        }

        if (!isBackGroundFade) {
            SetImage(nextSpriteName, storyManager.backGround, storyManager.BACK_GROUND_PATH);
            storyManager.backGround.color = Color.white;
            storyManager.CommandNewLine();

        } else {
            storyManager.isFading = true;

            var fadeInterval = BACKGROUND_FADE_TIME;
            if (nextSpriteName.Contains("、")) {
                var transitionList = nextSpriteName.Split("、"[0]);
                nextSpriteName = transitionList[0];
                fadeInterval = float.Parse(transitionList[1]);
            }

            if (nextSpriteName == ERASE_INTERRUPTION_BUTTON) {
                interruptionButton.gameObject.SetActive(false);
            }

            StartCoroutine(ChangeSprite(fadeInterval, nextSpriteName, storyManager.backGround, storyManager.BACK_GROUND_PATH, true));
        }
    }

    ///// <summary>
    ///// [TalkName]コマンドの処理
    ///// </summary>
    //public void CommandTalkName(string talkName) {

    //    if (!storyManager.talkCharaDic.ContainsKey(talkName)) {
    //        AddTalkChara(talkName);
    //    }

    //    SetTalkCaharaPosition();
    //    SetNonTalkCharaColor(talkName);

    //    storyManager.talkCharaName = talkName;
    //    storyManager.CommandNewLine();
    //}

    /// <summary>
    /// [TalkName]コマンドの処理
    /// </summary>
    private void CommandTalkName(string talkName) {

        var talkCharaNameList = talkName.Split("、"[0]);
        foreach (var charaName in talkCharaNameList) {
            //話しているキャラクターリストを登録
            if (!storyManager.talkCharaDic.ContainsKey(charaName)) {
                AddTalkChara(charaName);
            }
        }
        //人数に対応した座標に設定
        SetTalkCaharaPosition();
        SetCharaImageColor(talkCharaNameList);

        storyManager.talkCharaName = "";
        for (int i=0;i< talkCharaNameList.Length;i++) {
            storyManager.talkCharaName += talkCharaNameList[i] + "、";
        }

        //storyManager.talkCharaName = talkCharaNameList;
        storyManager.CommandNewLine();
    }


    /// <summary>
    /// 話すキャラクターを新規に登録する
    /// </summary>
    /// <param name="talkName"></param>
    public void AddTalkChara(string talkName) {
        var activeIndex = storyManager.talkCharaDic.Count;
        storyManager.talkCharaDic.Add(talkName, talkCharaImageList[activeIndex]);

        //登録順を管理
        storyManager.talkCharaAlignment.Add(talkName);

        //talkCharaList[activeIndex].gameObject.SetActive(true);

        //ここではいらない気がする
        //isCharaFade = true;
    }

    /// <summary>
    /// 話しているキャラクターの数によって座標をずらす
    /// </summary>
    public void SetTalkCaharaPosition() {

        if (storyManager.talkCharaAlignment.Count == 1) {
            //現在のトークキャラクターが１人の場合は必ず中央に表示する
            var charaName = storyManager.talkCharaAlignment[0];
            var pos = storyManager.talkCharaDic[charaName].gameObject.transform.localPosition;
            pos.x = 0.0f;
            storyManager.talkCharaDic[charaName].gameObject.transform.localPosition = pos;

        } else {
            //２人以上いる場合

            for (int i = 0; i < storyManager.talkCharaAlignment.Count; i++) {
                var charaName = storyManager.talkCharaAlignment[i];
                var pos = storyManager.talkCharaDic[charaName].gameObject.transform.localPosition;

                //今調べているキャラクターが花緒の場合、必ず左に表示する
                if (charaName == storyManager.PLAYER_NAME) {                    
                    pos.x = -800.0f;

                    //花緒より前に話していたキャラクターの座標をずらす
                    var j = i - 1;
                    while (j>=0) {
                        var charaNameTmp = storyManager.talkCharaAlignment[j];
                        var posTmp = storyManager.talkCharaDic[charaNameTmp].gameObject.transform.localPosition;

                        switch (j) {
                            case 0: posTmp.x = 800.0f; break;
                            case 1: posTmp.x = 0.0f; break;
                        }
                        storyManager.talkCharaDic[charaNameTmp].gameObject.transform.localPosition = posTmp;
                        j--;
                    }

                } else {
                    //花緒ではない場合、リストの順番に従って表示する

                    switch (i) {
                        case 0: pos.x = -800.0f; break;
                        case 1: pos.x = 800.0f; break;
                        case 2: pos.x = 0.0f; break;
                    }
                }

                storyManager.talkCharaDic[charaName].gameObject.transform.localPosition = pos;
            }
        }
    }

    ///// <summary>
    ///// 話すキャラクター以外を暗くする
    ///// </summary>
    ///// <param name="talkName"></param>
    //public void SetNonTalkCharaColor(string talkName) {
    //    if (storyManager.talkCharaDic.Count > 1) {
    //        foreach (var data in storyManager.talkCharaDic) {
    //            if (talkName != data.Key) data.Value.color = NOT_TALK_CHARA_COLOR;
    //        }
    //    }
    //}

    /// <summary>
    /// キャラクターの色を設定する（話すキャラクターは明るく、話さないキャラクターは暗く）
    /// </summary>
    /// <param name="talkNameList"></param>
    public void SetCharaImageColor(string[] talkNameList) {
        foreach (var charaName in storyManager.talkCharaDic) {
            if (CheckTalkChara(charaName.Key, talkNameList)) {
                if (storyManager.talkCharaDic[charaName.Key].color != Color.white) {
                    storyManager.talkCharaDic[charaName.Key].color = Color.white;
                }
            }else {
                if (storyManager.talkCharaDic[charaName.Key].color != NOT_TALK_CHARA_COLOR) {
                    storyManager.talkCharaDic[charaName.Key].color = NOT_TALK_CHARA_COLOR;
                }
            }
        }
    }

    /// <summary>
    /// 指定された登録済みキャラクター名と今回話すキャラクターが一致しているかどうかをチェック
    /// </summary>
    /// <param name="registCharaName"></param>
    /// <param name="talkNameList"></param>
    /// <returns></returns>
    private bool CheckTalkChara(string registCharaName,string[] talkNameList) {
        foreach (var talkCharaName in talkNameList) {
            if (registCharaName == talkCharaName) {
                return true;
            }
        }
        return false;
    }


    ///// <summary>
    ///// [CharaFace]コマンドの処理
    ///// </summary>
    //private void CommandCharaFace(string targetCharaFace) {
    //    //StartCoroutine(ChangeSprite(FADE_TIME, analize[1], backGround, BACK_GROUND_PATH));

    //    var talkCharaString = targetCharaFace.Split("、"[0]);
    //    var talkCharaName = talkCharaString[0];
    //    var nextFaceName = talkCharaString[1];

    //    isCharaFade = true;

    //    StartCoroutine(ChangeSprite(CHARAFACE_FADE_TIME, nextFaceName, storyManager.talkCharaDic[talkCharaName], storyManager.CHARA_PATH));
    //}

    /// <summary>
    /// [CharaFace]コマンドの処理
    /// </summary>
    private void CommandCharaFace(string talkCharaList) {

        var charaDataSplit = talkCharaList.Split(":"[0]);

        isCharaFade = true;

        for (int i = 0; i < charaDataSplit.Length; i++) {
            var charaData = charaDataSplit[i];

            var talkCharaString = charaData.Split("、"[0]);
            var talkCharaName = talkCharaString[0];
            var nextFaceName = talkCharaString[1];

            storyManager.isFading = true;
            if (i != charaDataSplit.Length-1) {
                StartCoroutine(ChangeSprite(CHARAFACE_FADE_TIME, nextFaceName, storyManager.talkCharaDic[talkCharaName], storyManager.CHARA_PATH,false));
            } else {
                StartCoroutine(ChangeSprite(CHARAFACE_FADE_TIME, nextFaceName, storyManager.talkCharaDic[talkCharaName], storyManager.CHARA_PATH,true));
            }
            
        }
    }

    /// <summary>
    /// [CharaHide]コマンドの処理
    /// </summary>
    private void CommandCharaHide(string hideCharaName) {

        isCharaHide = true;
        var hideCharaNameList = hideCharaName.Split("、"[0]);
        for(int i=0;i<hideCharaNameList.Length;i++) {
            var charaName = hideCharaNameList[i];

            storyManager.isFading = true;
            if (i != hideCharaNameList.Length - 1) {
                StartCoroutine(ChangeSprite(CHARAFACE_FADE_TIME, "", storyManager.talkCharaDic[charaName], "",false));
            }else {
                StartCoroutine(ChangeSprite(CHARAFACE_FADE_TIME, "", storyManager.talkCharaDic[charaName], "",true));
            }

            //イメージ配列の並び替え（削除したキャラクターのイメージを配列の末尾に移動）
            SortTalkCharaImageList(storyManager.talkCharaDic[charaName]);

            //話しているキャラクターリストから削除
            storyManager.talkCharaDic.Remove(charaName);
            storyManager.talkCharaAlignment.Remove(charaName);
        }
        //人数に対応した座標に設定
        if(storyManager.talkCharaDic.Count > 0 ) SetTalkCaharaPosition();
    }

    /// <summary>
    /// イメージ配列の並び替え
    /// </summary>
    private void SortTalkCharaImageList(Image image) {
        //イメージ配列から削除対象のイメージの添え字を取得
        var i = 0;
        while (i<talkCharaImageList.Length && talkCharaImageList[i] != image) i++;
        if (i == talkCharaImageList.Length-1 || i == talkCharaImageList.Length) return;

        //取得した添え字のイメージを配列の最後尾に移動させる
        var imageTmp = talkCharaImageList[i];
        for (int j=i+1;j<talkCharaImageList.Length; j++) talkCharaImageList[j - 1] = talkCharaImageList[j];
        talkCharaImageList[talkCharaImageList.Length-1] = imageTmp;
    }

    /// <summary>
    /// [ChapterEnd]コマンドの処理
    /// </summary>
    private void CommandChapterEnd() {
        SaveLoadManager.Instance.DeleteAllData();
        FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_CHAPTER_SELECT, GameManager.Instance.sceneChangeInterval);
    }

    /// <summary>
    /// [HanafudaStart]コマンドの処理
    /// </summary>
    private void CommandHanafudaStart() {
        var hanahudaEndCommandLineIndex = GetHanahudaEndCommandIndex()+1;
        SaveLoadManager.Instance.SetStorySaveData(SceneName.SceneNameManager.SCENE_NAME_STORY,
                                                  storyManager.storyNo,
                                                  hanahudaEndCommandLineIndex);

        SaveLoadManager.Instance.SaveData();

        FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_HANAFUDA, GameManager.Instance.sceneChangeInterval);
    }

    /// <summary>
    /// [Hanafuda_End]のコマンドの行数を取得
    /// </summary>
    private int GetHanahudaEndCommandIndex() {
        var lineIndexTmp = storyManager.lineIndex;
        var command = "";

        while (command != COMMAND_HANAFUDA_END) {
            lineIndexTmp++;
            var line = storyManager.GetTargetLine(lineIndexTmp);
            if (IsContainsCommandWard(line)) {
                command = GetCommand(line, 0);
            }
        }
        return lineIndexTmp;
    }

    /// <summary>
    /// [PlayBGM]コマンドの処理
    /// </summary>
    /// <param name="bgmName"></param>
    private void CommandPlayBGM(string bgmName) {
        AudioManager.Instance.PlayBGM(bgmName,true,1.0f);
        storyManager.CommandNewLine();
    }
    /// <summary>
    /// [StopBGM]コマンドの処理
    /// </summary>
    private void CommandStopBGM() {
        AudioManager.Instance.StopBGM(0.5f);
        storyManager.CommandNewLine();
    }
    /// <summary>
    /// [PlaySE]コマンドの処理
    /// </summary>
    /// <param name="seName"></param>
    private void CommandPlaySE(string seName){
        AudioManager.Instance.PlaySE(seName);
        storyManager.CommandNewLine();
    }
    /// <summary>
    /// [StopSE]コマンドの処理
    /// </summary>
    private void CommandStopSE() {
        AudioManager.Instance.StopSE();
        storyManager.CommandNewLine();
    }

    /// <summary>
    /// 画像を変更する
    /// </summary>
    private IEnumerator ChangeSprite(float interval, string nextSpriteName, Image spriteImage, string PATH,bool isEndChara) {

        //storyManager.isFading = true;
        float time = 0;

        if (!isCharaFade) {
            //だんだん透明に
            while (time <= interval) {
                spriteImage.color = Color.Lerp(spriteImage.color, ALPHA_COLOR, time / interval);
                time += Time.deltaTime;
                yield return 0;
            }

            if (spriteImage.color != ALPHA_COLOR) spriteImage.color = ALPHA_COLOR;
        }else {
            //キャラクターを表示する場合、その画像を表示状態にする
            spriteImage.gameObject.SetActive(true);
        }

        if (!isCharaHide) {
            SetImage(nextSpriteName, spriteImage, PATH);

            //だんだん明るく
            time = 0;
            while (time <= interval) {
                spriteImage.color = Color.Lerp(spriteImage.color, DEFAULT_COLOR, time / interval);
                time += Time.deltaTime;
                yield return 0;
            }

            if (spriteImage.color != DEFAULT_COLOR) spriteImage.color = DEFAULT_COLOR;

        }else {
            //キャラクターを隠す場合、その画像を非表示状態にする
            spriteImage.gameObject.SetActive(false);
        }

        if (isEndChara) {
            storyManager.isFading = false;
            isCharaFade = false;
            isCharaHide = false;
            storyManager.CommandNewLine();
        }
    }

    /// <summary>
    /// 画像を登録・変更する
    /// </summary>
    public void SetImage(string nextSpriteName, Image spriteImage, string PATH) {

        //変更したい画像がディクショナリ配列に既に登録されている場合、登録されている画像を設定
        if (storyManager.spriteDic.ContainsKey(nextSpriteName)) {
            spriteImage.sprite = storyManager.spriteDic[nextSpriteName];

        } else {
            //登録されていない場合、指定した名前の画像をResoucesフォルダから読み込んで新規に登録
            var loadSprite = Resources.Load<Sprite>(PATH + nextSpriteName);
            storyManager.spriteDic.Add(nextSpriteName, loadSprite);

            spriteImage.sprite = loadSprite;
        }
    }
}
