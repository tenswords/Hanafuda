using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Result : MonoBehaviour {

    [SerializeField]
    private FieldManager fieldManager;
    [SerializeField]
    private ScoreManager scoreManager;

    private bool isButton;

    [SerializeField]
    private GameObject[] establishRoleText;

    [SerializeField]
    private Text turnPlayerScoreText;
    [SerializeField]
    private Text playerScoreText;
    [SerializeField]
    private Text cpuScoreText;

    private FieldManager.TURNPLAYER winPlayer;

    [SerializeField]
    private Button titleButton;
    [SerializeField]
    private Button nextGameButton;
    [SerializeField]
    private Button storyButton;
    [SerializeField]
    private Button reMatchButton;



    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 再開時に保存されていた成立済みリストの設定
    /// </summary>
    public void SetResumptionText() {

        var winPlayer = PlayerPrefs.GetString(SaveLoadManager.Instance.SAVE_DATA_WIN_PLAYER);
        var flusuData = PlayerPrefs.GetString(SaveLoadManager.Instance.SAVE_DATA_FLUSH_DATA);

        var flushSplit = flusuData.Split(":"[0]);

        switch (winPlayer) {
            case "Player": fieldManager.GetPlayer().SetFlushList(flushSplit); break;
            case "CPU": fieldManager.GetCPU().SetFlushList(flushSplit); break;
        }
    }

    public void SetScoreText() {
        turnPlayerScoreText.text = scoreManager.turnPlayerScore + "文";
        playerScoreText.text = scoreManager.playerScore + "文";
        cpuScoreText.text = scoreManager.cpuScore + "文";
    }

    public void SetFlushListText() {

        var establishRoleName = establishRoleText[0].transform.FindChild("EstablishRoleName").GetComponent<Text>();
        var establishRoleScore = establishRoleText[0].transform.FindChild("EstablishRoleScore").GetComponent<Text>();

        establishRoleName.text = "";
        establishRoleScore.text = "";

        establishRoleText[0].SetActive(true);

        string[] sortList;

        switch (fieldManager.turnPlayer) {
            case FieldManager.TURNPLAYER.PLAYER:
                var player = fieldManager.GetPlayer();

                //並び替えを行った配列を先頭から順番に表示していく
                sortList = GetSortList(player.GetFlushList());

                for (int i=0;i< sortList.Length; i++) {
                    if (i == 5) {
                        establishRoleName = establishRoleText[1].transform.FindChild("EstablishRoleName").GetComponent<Text>();
                        establishRoleScore = establishRoleText[1].transform.FindChild("EstablishRoleScore").GetComponent<Text>();

                        establishRoleName.text = "";
                        establishRoleScore.text = "";

                        establishRoleText[1].SetActive(true);
                    }

                    establishRoleName.text += sortList[i];
                    establishRoleScore.text += player.GetFlushList()[sortList[i]] + "文";

                    //表示可能領域の最終行でなければ改行を追加
                    if(i%5 < 4) {
                        establishRoleName.text += "\n";
                        establishRoleScore.text += "\n";
                    }

                }
                break;

            case FieldManager.TURNPLAYER.COM:

                var cpu = fieldManager.GetCPU();

                //並び替えを行った配列を先頭から順番に表示していく
                sortList = GetSortList(cpu.GetFlushList());


                for (int i = 0; i < sortList.Length; i++) {
                    if (i == 5) {
                        establishRoleName = establishRoleText[1].transform.FindChild("EstablishRoleName").GetComponent<Text>();
                        establishRoleScore = establishRoleText[1].transform.FindChild("EstablishRoleScore").GetComponent<Text>();

                        establishRoleName.text = "";
                        establishRoleScore.text = "";

                        establishRoleText[1].SetActive(true);
                    }

                    establishRoleName.text += sortList[i];
                    establishRoleScore.text += cpu.GetFlushList()[sortList[i]] + "文";

                    //表示可能領域の最終行でなければ改行を追加
                    if (i % 5 < 4) {
                        establishRoleName.text += "\n";
                        establishRoleScore.text += "\n";
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 指定された成立済みリストを、光、種、短冊、カスの順に並び替えたリストを取得
    /// </summary>
    private string[] GetSortList(Dictionary<string, int> list) {
        var sortIndex = 0;
        var sort = new string[list.Count];

        //i==0 光 i==1 種 i==2 短冊 i==3 カス の順に探す
        for (int i=0;i<fieldManager.arrayRoleType.Length; i++) {
            for (int j = 0; j < fieldManager.arrayRoleType[i].Length; j++) {

                foreach (var data in list) {

                    if (fieldManager.arrayRoleType[i][j] == data.Key) {
                        sort[sortIndex] = data.Key;
                        sortIndex++;
                    }
                }
            }      
        }

        return sort;
    }

    void OnEnable() {
        StartCoroutine(ScoreCalculation(1.0f,0.5f));
    }

    private IEnumerator ScoreCalculation(float waitTime,float interval) {
        yield return new WaitForSeconds(waitTime);

        isButton = false;
        //スコアの計算処理
        //相手のスコアを獲得したスコアで引く

        while (scoreManager.turnPlayerScore > 0) {
            scoreManager.turnPlayerScore--;
            turnPlayerScoreText.text = scoreManager.turnPlayerScore + "文";

            switch (fieldManager.turnPlayer) {
                case FieldManager.TURNPLAYER.PLAYER:
                    //プレイヤーがあがった場合
                    if (scoreManager.cpuScore > 0) {
                        scoreManager.cpuScore--;
                        cpuScoreText.text = scoreManager.cpuScore + "文";

                        if (scoreManager.cpuScore == 0 && winPlayer == FieldManager.TURNPLAYER.NONE) {
                            winPlayer = FieldManager.TURNPLAYER.PLAYER;
                        }
                    }

                    break;
                case FieldManager.TURNPLAYER.COM:
                    //CPUがあがった場合
                    if (scoreManager.playerScore > 0) {
                        scoreManager.playerScore--;
                        playerScoreText.text = scoreManager.playerScore + "文";

                        if (scoreManager.playerScore == 0 && winPlayer == FieldManager.TURNPLAYER.NONE) {
                            winPlayer = FieldManager.TURNPLAYER.COM;
                        }
                    }

                    break;
            }

            yield return new WaitForSeconds(0.001f);
            yield return 0;
        }

        //各ボタンの表示もここで行う
        switch(winPlayer) {
            //勝利者がいない場合
            case FieldManager.TURNPLAYER.NONE: nextGameButton.gameObject.SetActive(true); break;            
            //勝利者がプレイヤーの場合
            case FieldManager.TURNPLAYER.PLAYER:storyButton.gameObject.SetActive(true); break;
            //勝利者がCPUの場合
            case FieldManager.TURNPLAYER.COM: reMatchButton.gameObject.SetActive(true); break;
        }
        titleButton.gameObject.SetActive(true);

        isButton = true;
    }

    public void OnTitleButton() {
        //現在のプレイヤー、CPUの残文数をセーブする必要がある（対戦終了となった時点でデータは設定済み）
        SaveLoadManager.Instance.SetHanafudaData(SceneName.SceneNameManager.SCENE_NAME_HANAFUDA,"true");
        SaveLoadManager.Instance.SaveData();

        AudioManager.Instance.StopBGM(0.5f);

        FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_TITLE);
    }

    public void OnNextGameButton() {
        //お互いの文数が０以上の場合、次の局へ進む
        SaveLoadManager.Instance.SetHanafudaData(SceneName.SceneNameManager.SCENE_NAME_HANAFUDA, "false");
        SaveLoadManager.Instance.SaveData();
        FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_HANAFUDA);
    }
    public void OnStoryButton() {
        //プレイヤーが勝利した場合（CPUの残文数が０になった場合）、ストーリーへ戻る
        SaveLoadManager.Instance.SetStorySaveData(SceneName.SceneNameManager.SCENE_NAME_STORY);
        SaveLoadManager.Instance.SaveData();

        AudioManager.Instance.StopBGM(0.5f);

        FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_STORY);
    }
    public void OnReMatchButton() {
        //CPUが勝利した場合（プレイヤーの残文数が０になった場合）、花札の最初から
        SaveLoadManager.Instance.SetHanafudaData(SceneName.SceneNameManager.SCENE_NAME_HANAFUDA,
                                                "",
                                                "",
                                                scoreManager.GetDefaultPlayerScore(),
                                                scoreManager.GetDefaultCPUScore(),
                                                "false");
        SaveLoadManager.Instance.SaveData();
        FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_HANAFUDA);
    }

}
