using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour {

    [SerializeField]
    private CommandManager commandManager;

    private const string CHAPTER_PATH = "Story/Chapter/";
    private string selectChapterName = "";

    private ReadTextFile readTextFile;

    private string[] lineText;
    private string line = "";

    [HideInInspector]
    public int lineIndex;
    private bool isLineRead;

    private int wardIndex;

    private bool isLineEnd;

    [SerializeField,Header("1行を読み込む時間")]
    private float lineReadTime;
    [SerializeField]
    private float MAX_LINE_READ_TIME;

    [SerializeField,Header("1文字を表示する時間")]
    private float MAX_ONE_WARD_READ_TIME;

    [SerializeField]
    private Text textField_Text;

    [SerializeField]
    private GameObject textField_NextButton;

    public STATE state;
    public enum STATE {
        READTEXT,
        WAIT_NEXT
    }

    public bool isFading;
    public bool isStarted;

    // Use this for initialization
    void Start () {
        selectChapterName = CHAPTER_PATH + "Chapter_1";
        var textAsset = (TextAsset)Resources.Load(selectChapterName);
        lineText = textAsset.ToString().Split("\n"[0]);
        lineIndex = 0;

        StartCoroutine(StoryStarted());
    }
	
	// Update is called once per frame
	void Update () {

        //フェード処理中は以降に進まない
        if (isFading) return;

        switch (state) {
            case STATE.READTEXT:
                ReadText();
                break;

            case STATE.WAIT_NEXT:
                break;
        }
    }

    private IEnumerator StoryStarted() {
        isStarted = true;
        yield return new WaitForSeconds(0.25f);
        isStarted = false;
    }

    /// <summary>
    /// テキストファイルの1行を読み込む処理
    /// </summary>
    private void ReadText() {

        //今読み込む行が改行のみだったら次の行へ進む
        if (!isLineRead) {

            //1行分を取得
            line = lineText[lineIndex];

            if (line.Length == 1) {
                lineIndex++;
            } else {
                isLineRead = true;
            }

        } else {

            //読み込んだ1行の文字列にコマンド用の文字が含まれている場合、どのコマンドかを解析して対応した処理を行う
            if (commandManager.IsContainsCommandWard(line)) {

                var command = commandManager.GetCommand(line,0);
                commandManager.CommandAnalize(command);

            } else {

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
            }
        }
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

        isLineRead = false;
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
