using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour {

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private CommandManager commandManager;

    public string CHAPTER_PATH = "Story/Chapter/";
    public string BACK_GROUND_PATH = "Story/BackGround/";
    public string CHARA_PATH = "Story/Character/";

    private string selectChapterName = "";

    private ReadTextFile readTextFile;

    public int storyNo;
    private string[] lineText;
    private string line = "";

    [HideInInspector]
    public int lineIndex;
    public bool isLineRead;

    private int wardIndex;

    private bool isLineEnd;

    //話しているキャラクター名
    public string talkCharaName;

    [SerializeField, Header("最初のテキストを読み込むまでの時間")]
    private float WAIT_STORY_STATED_TIME;

    [SerializeField,Header("1文字を表示する時間")]
    private float MAX_ONE_WARD_READ_TIME;
    [SerializeField]
    private float wardReadTime;

    [SerializeField]
    private Text textField_Text;

    [SerializeField]
    private GameObject textField_NextButton;

    //画像関係のディクショナリ
    public Dictionary<string, Sprite> spriteDic;
    //会話しているキャラクターのディクショナリ
    public Dictionary<string, Image> talkCharaDic;

    //背景イメージ変数
    public Image backGround;

    private STATE oldState;
    public STATE state;
    public enum STATE {
        READTEXT,
        WAIT_NEXT,
        PAUSE
    }

    public bool isFading;
    public bool isWaitStoryStarted;
    private bool isWardRead;

    void Awake() {
        GameManager.Instance.state = GameManager.STATE.STORY;
        //InterruptionDialogManager.Instance.SetCanvasCamera();
    }

    // Use this for initialization
    void Start() {

        spriteDic = new Dictionary<string, Sprite>();
        talkCharaDic = new Dictionary<string, Image>();

        isWaitStoryStarted = true;

        //セーブデータがある場合、保存されたデータを読み込む
        if (SaveLoadManager.Instance.CheckHasSaveData()) {
            Resumption();
        }else {
            lineIndex = 0;            
        }

        StoryStart();
    }

    /// <summary>
    /// 中断ボタンを押したときに現在の状態を保存
    /// </summary>
    public void Interruption() {
        InterruptionDialogManager.Instance.SwitchStatus(InterruptionDialogManager.STATE.Interruption);

        oldState = state;
        state = STATE.PAUSE;
    }

    /// <summary>
    /// 中断ボタンをキャンセルしたときに保存しておいた状態に戻す
    /// </summary>
    public void InterruptionCancel() {
        state = oldState;
    }

    /// <summary>
    /// ストーリー再開
    /// </summary>
    public void Resumption() {

        //ストーリーNoの読み込み
        GameManager.Instance.storyNo = PlayerPrefs.GetInt(SaveLoadManager.Instance.SAVE_DATA_STORY_NO);

        //行数の読み込み
        lineIndex = PlayerPrefs.GetInt(SaveLoadManager.Instance.SAVE_DATA_LINE_INDEX);

        //背景の読み込み
        var backGroundName = PlayerPrefs.GetString(SaveLoadManager.Instance.SAVE_DATA_BACK_GROUND);
        commandManager.SetImage(backGroundName, backGround, BACK_GROUND_PATH);
        backGround.color = Color.white;

        //キャラデータの読み込み
        if (PlayerPrefs.HasKey(SaveLoadManager.Instance.SAVE_DATA_TALK_CHARA_LIST)) {
            var talkCharaList = PlayerPrefs.GetString(SaveLoadManager.Instance.SAVE_DATA_TALK_CHARA_LIST);
            var charaDataSplit = talkCharaList.Split("、"[0]);
            for (int i = 0; i < charaDataSplit.Length - 1; i++) {
                var charaData = charaDataSplit[i];

                var data = charaData.Split("："[0]);
                var charaName = data[0];
                var spliteName = data[1];

                commandManager.AddTalkChara(charaName);
                commandManager.SetImage(spliteName, talkCharaDic[charaName], CHARA_PATH);
                talkCharaDic[charaName].color = Color.white;

            }
        }

        //話しているキャラの読み込み（話しているキャラクター以外を暗くするため）
        if (PlayerPrefs.HasKey(SaveLoadManager.Instance.SAVE_DATA_TALK_CHARA_NAME)) {
            var talkCharaName = PlayerPrefs.GetString(SaveLoadManager.Instance.SAVE_DATA_TALK_CHARA_NAME);
            commandManager.SetNonTalkCharaColor(talkCharaName);
        }
    }

    public void StoryStart() {
        storyNo = GameManager.Instance.storyNo;
        selectChapterName = CHAPTER_PATH + "Chapter_" + storyNo;
        var textAsset = (TextAsset)Resources.Load(selectChapterName);
        lineText = textAsset.ToString().Split("\n"[0]);

        isWardRead = true;

        StartCoroutine(WaitStoryStarted(WAIT_STORY_STATED_TIME));
    }

	// Update is called once per frame
	void Update () {

        //フェード処理中は以降に進まない
        if (isWaitStoryStarted || isFading) return;

        switch (state) {
            case STATE.READTEXT:
                ReadText();
                break;

            case STATE.WAIT_NEXT:
                break;
            case STATE.PAUSE:
                break;
        }
    }

    private IEnumerator WaitStoryStarted(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        isWaitStoryStarted = false;
    }

    /// <summary>
    /// テキストファイルの1行を読み込む処理
    /// </summary>
    private void ReadText() {

        //今読み込む行が改行のみだったら次の行へ進む
 //       if (!isLineRead) {

            //1行分を取得
            line = lineText[lineIndex];

            if (line.Length == 1) {
                lineIndex++;
            } else {
                isLineRead = false;
                //    }

                //} else {

                //読み込んだ1行の文字列にコマンド用の文字が含まれている場合、どのコマンドかを解析して対応した処理を行う
                if (commandManager.IsContainsCommandWard(line)) {

                    var command = commandManager.GetCommand(line, 0);
                    commandManager.CommandAnalize(command);

                } else {

                    if (isWardRead) {

                        var ward = line.Substring(wardIndex, 1);
                        //コマンド用の文字ではない場合、文字の表示を行う
                        textField_Text.text += ward;
                        wardIndex++;
                        //1行の最後まで読み込んだら、プレイヤーのTAP待ちにする（NextButton点滅）
                        if (wardIndex == (line.Length - 1)) {

                            //次の行を読み込み改行コマンドであった場合、さらに次の行へ進むための処理
                            var nextLine = lineText[lineIndex + 1];
                            var nextWard = nextLine.Substring(0, 1);
                            if (commandManager.IsContainsCommandWard(nextWard)) {
                                //読み込んだ1文字がコマンド用の文字の場合、どのコマンドかを解析して対応した処理を行う
                                var command = commandManager.GetCommand(nextLine);
                                commandManager.CommandAnalize(command);

                            } else {
                                isLineEnd = true;
                            }
                        }

                        if (isLineEnd) {
                            isLineEnd = false;
                            state = STATE.WAIT_NEXT;
                            textField_NextButton.SetActive(true);
                        }

                        StartCoroutine(WaitOneWardRead(MAX_ONE_WARD_READ_TIME));
                    }
                }
            }
        //}
    }

    /// <summary>
    /// 1行全てを表示する
    /// </summary>
    public void LineTextShowAll() {

        while (wardIndex < line.Length-1) {
            var ward = line.Substring(wardIndex, 1);
            textField_Text.text += ward;
            wardIndex++;
        }

        //改行コマンドの場合、表示される残りのテキストを表示する
        var nextLine = lineText[lineIndex + 1];
        var nextWard = nextLine.Substring(0, 1);
        if (commandManager.IsContainsCommandWard(nextWard)) {

            var command = commandManager.GetCommand(nextLine);
            if (commandManager.IsNewLineCommand(command)) {

                lineIndex += 2;
                wardIndex = 0;
                textField_Text.text += "\n";
                line = lineText[lineIndex];

                LineTextShowAll();
            }
        }else {
            state = STATE.WAIT_NEXT;
            textField_NextButton.SetActive(true);
        }
    }

    private IEnumerator WaitOneWardRead(float waitTime) {
        isWardRead = false;
        yield return new WaitForSeconds(waitTime);
        isWardRead = true;
    }

    /// <summary>
    /// 改行処理
    /// </summary>
    public void CommandNewLine(bool isNewLine = false) {

        if(isNewLine) {
            textField_Text.text += "\n";
            lineIndex++;
        }else {
            textField_Text.text = "";
        }

        isLineRead = true;
        lineIndex++;
        wardIndex = 0;
    }

    /// <summary>
    /// 指定した行の文字列を取得
    /// </summary>
    public string GetTargetLine(int targetIndex) {
        return lineText[targetIndex];
    }
}
