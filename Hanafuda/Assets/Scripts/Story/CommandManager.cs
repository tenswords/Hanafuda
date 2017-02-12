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
    private const string COMMAND_HANAFUDA_START = "HanafudaStart";

    //[SerializeField]
    //private Image backGround;

    [SerializeField]
    private Image[] talkCharaList;

    [SerializeField]
    private float FADE_TIME;
    [SerializeField]
    private float CHARAFACE_FADE_TIME;

    [SerializeField]
    private Color DEFAULT_COLOR;
    [SerializeField]
    private Color ALPHA_COLOR;
    [SerializeField]
    private Color NOT_TALK_CHARA_COLOR;

    ////キャラクターの表情ディクショナリ
    //private Dictionary<string, Sprite> storyManager.charaSpriteDic;
    ////トークキャラクターのディクショナリ
    //private Dictionary<string, Image> storyManager.talkCharaDic;

    //だんだん暗くする処理を行うかどうかのフラグ
    private bool isCharaFade;
    //だんだん明るくする処理を行うかどうかのフラグ
    private bool isCharaHide;

    // Use this for initialization
    void Start() {
        //storyManager.charaSpriteDic = new Dictionary<string, Sprite>();
        //storyManager.talkCharaDic = new Dictionary<string, Image>();
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
            case COMMAND_HANAFUDA_START: break;
        }
    }

    /// <summary>
    /// [BackGround]コマンドの処理
    /// </summary>
    private void CommnadBackGround(string nextSpriteName) {
        if (storyManager.lineIndex == 0) {
            SetImage(nextSpriteName, storyManager.backGround, storyManager.BACK_GROUND_PATH);
            storyManager.backGround.color = Color.white;
            storyManager.CommandNewLine();
        } else {
            StartCoroutine(ChangeSprite(FADE_TIME, nextSpriteName, storyManager.backGround, storyManager.BACK_GROUND_PATH));
        }
    }

    /// <summary>
    /// [TalkName]コマンドの処理
    /// </summary>
    public void CommandTalkName(string talkName) {

        if (!storyManager.talkCharaDic.ContainsKey(talkName)) {
            AddTalkChara(talkName);
        }

        SetNonTalkCharaColor(talkName);

        storyManager.talkCharaName = talkName;
        storyManager.CommandNewLine();
    }

    /// <summary>
    /// 話すキャラクターを新規に登録する
    /// </summary>
    /// <param name="talkName"></param>
    public void AddTalkChara(string talkName) {
        var activeIndex = storyManager.talkCharaDic.Count;
        storyManager.talkCharaDic.Add(talkName, talkCharaList[activeIndex]);

        //話しているキャラクターの数によって座標をずらす
        //Position;

        talkCharaList[activeIndex].gameObject.SetActive(true);

        //ここではいらない気がする
        //isCharaFade = true;
    }


    /// <summary>
    /// 話すキャラクター以外を暗くする
    /// </summary>
    /// <param name="talkName"></param>
    public void SetNonTalkCharaColor(string talkName) {
        if (storyManager.talkCharaDic.Count > 1) {
            foreach (var data in storyManager.talkCharaDic) {
                if (talkName != data.Key) data.Value.color = NOT_TALK_CHARA_COLOR;
            }
        }
    }

    /// <summary>
    /// [CharaFace]コマンドの処理
    /// </summary>
    private void CommandCharaFace(string targetCharaFace) {
        //StartCoroutine(ChangeSprite(FADE_TIME, analize[1], backGround, BACK_GROUND_PATH));

        var talkCharaString = targetCharaFace.Split("、"[0]);
        var talkCharaName = talkCharaString[0];
        var nextFaceName = talkCharaString[1];

        isCharaFade = true;

        StartCoroutine(ChangeSprite(CHARAFACE_FADE_TIME, nextFaceName, storyManager.talkCharaDic[talkCharaName], storyManager.CHARA_PATH));
    }

    /// <summary>
    /// [CharaHide]コマンドの処理
    /// </summary>
    private void CommandCharaHide(string hideCharaName) {

        isCharaHide = true;
        var hideCharaNameList = hideCharaName.Split("、"[0]);
        foreach(var charaName in hideCharaNameList) {
            StartCoroutine(ChangeSprite(CHARAFACE_FADE_TIME, "", storyManager.talkCharaDic[charaName], ""));
        }

        storyManager.CommandNewLine();
    }

    /// <summary>
    /// [ChapterEnd]コマンドの処理
    /// </summary>
    private void CommandChapterEnd() {
        FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_TITLE, GameManager.Instance.sceneChangeInterval);
    }

    /// <summary>
    /// 画像を変更する
    /// </summary>
    private IEnumerator ChangeSprite(float interval, string nextSpriteName, Image spriteImage, string PATH) {

        storyManager.isFading = true;
        float time = 0;

        if (!isCharaFade) {
            //だんだん透明に
            while (time <= interval) {
                spriteImage.color = Color.Lerp(spriteImage.color, ALPHA_COLOR, time / interval);
                time += Time.deltaTime;
                yield return 0;
            }

            if (spriteImage.color != ALPHA_COLOR) spriteImage.color = ALPHA_COLOR;
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
        }
        storyManager.isFading = false;
        isCharaFade = false;
        isCharaHide = false;
        storyManager.CommandNewLine();

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
