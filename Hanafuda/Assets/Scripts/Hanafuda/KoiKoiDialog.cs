using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KoiKoiDialog : MonoBehaviour {

    [SerializeField]
    private FieldManager fieldManager;
    [SerializeField]
    private ScoreManager scoreManager;
    [SerializeField]
    private Image koikoiSelectImage;

    [SerializeField]
    private Result result;

    [SerializeField]
    private GameObject hideBlackImage;

    [SerializeField]
    private Sprite[] buttonImage;

    private bool isColor;
    private Image image;
    private Color defaultColor;
    [SerializeField]
    private Color ALFA_COLOR;
    private Color targetColor;

    private float alfaTimer;
    [SerializeField,Header("何秒間かけて透過させるか")]
    private float MAX_ALFA_TIMER;

    [SerializeField]
    private Text currentScoreText;
    [SerializeField]
    private Text playerScoreText;
    [SerializeField]
    private Text cpuScoreText;

    // Use this for initialization
    void Start () {
        image = GetComponent<Image>();
        defaultColor = image.color;
        isColor = true;

        SetScoreText();
    }

    // Update is called once per frame
    void Update() {

        if (!isColor) {

            alfaTimer += Time.deltaTime;
            image.color = Color.Lerp(image.color, targetColor, alfaTimer / MAX_ALFA_TIMER);

            if(image.color == targetColor) {
                isColor = true;
            }
        }
	}

    public void OnPointerDown() {
        isColor = false;
        alfaTimer = 0.0f;
        targetColor = ALFA_COLOR;
    }
    public void OnPointerUp() {
        isColor = false;
        alfaTimer = 0.0f;
        targetColor = defaultColor;
    }

    public void OnKoiKoiButton() {
        koikoiSelectImage.sprite = buttonImage[0];
        StartCoroutine(KoiKoiDialogView(0.25f));
    }
    public void OnAgariButton() {
        SaveScoreData();

        koikoiSelectImage.sprite = buttonImage[1];
        StartCoroutine(KoiKoiDialogView(0.25f));

        fieldManager.state = FieldManager.STATE.RESULT;
    }

    /// <summary>
    /// 対戦終了となった瞬間に結果を保存
    /// </summary>
    private void SaveScoreData() {

        var winPlayer = "";
        var flushData = "";
        var scoreTmp_1 = 0;
        var scoreTmp_2 = 0;

        switch (fieldManager.turnPlayer) {
            case FieldManager.TURNPLAYER.PLAYER:
                //プレイヤーがあがった場合
                var player = fieldManager.GetPlayer();

                scoreTmp_1 = scoreManager.playerScore;
                scoreTmp_2 = scoreManager.cpuScore - scoreManager.turnPlayerScore;
                scoreTmp_2 = Mathf.Clamp(scoreTmp_2, 0, scoreManager.cpuScore);

                winPlayer = "Player";
                foreach (var data in player.GetFlushList()) {
                    flushData += data.Key + "、" + data.Value + ":";
                }

                break;
            case FieldManager.TURNPLAYER.COM:
                //CPUがあがった場合
                var cpu = fieldManager.GetCPU();

                scoreTmp_2 = scoreManager.cpuScore;
                scoreTmp_1 = scoreManager.playerScore - scoreManager.turnPlayerScore;
                scoreTmp_1 = Mathf.Clamp(scoreTmp_1, 0, scoreManager.playerScore);
                break;
        }

        SaveLoadManager.Instance.SetHanafudaData(SceneName.SceneNameManager.SCENE_NAME_HANAFUDA,
                                                winPlayer,
                                                flushData,
                                                scoreTmp_1,
                                                scoreTmp_2,
                                                "true");
    }

    private IEnumerator KoiKoiDialogView(float interval) {
        var time = 0.0f;
        var rect = GetComponent<RectTransform>();

        while (time < interval) {
            time += Time.deltaTime;
            var lerp = Mathf.Lerp(1f, 0f, time / interval);

            rect.localScale = Vector3.Lerp(rect.localScale, Vector3.zero, lerp);
            yield return 0;
        }

        rect.localScale = Vector3.zero;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(StartSelectButtonAnimation());
    }


    /// <summary>
    /// こいこい確認で選択したボタンのアニメーションを再生
    /// </summary>
    private IEnumerator StartSelectButtonAnimation() {

        //gameObject.SetActive(false);
        hideBlackImage.SetActive(true);

        var time = 0.0f;
        var interval = 0.5f;
        var targetScale = new Vector3(1.0f, 1.0f, 1.0f);
        var rect = koikoiSelectImage.GetComponent<RectTransform>();

        koikoiSelectImage.gameObject.SetActive(true);

        while (time < interval) {
            time += Time.deltaTime;
            var lerp = Mathf.Lerp(0f, 1f, time / interval);
            rect.localScale = Vector3.Lerp(rect.localScale, targetScale, lerp);
            koikoiSelectImage.color = Color.Lerp(koikoiSelectImage.color, Color.white, lerp);
            yield return 0;
        }

        yield return new WaitForSeconds(1.0f);

        time = 0.0f;
        interval = 1.0f;
        targetScale = new Vector3(2.0f, 2.0f, 2.0f);

        while (time < interval) {
            time += Time.deltaTime;
            var lerp = Mathf.Lerp(0f, 1f, time / interval);
            rect.localScale = Vector3.Lerp(rect.localScale, targetScale, lerp);
            koikoiSelectImage.color = Color.Lerp(koikoiSelectImage.color, Color.clear, lerp);
            yield return 0;
        }

        koikoiSelectImage.gameObject.SetActive(false);
        rect.localScale = Vector3.zero;
        koikoiSelectImage.color = Color.clear;

        EndSelectButtonAnimation();
    }

    /// <summary>
    /// こいこい確認で選択したボタンのアニメーションが終了したときの処理
    /// </summary>
    public void EndSelectButtonAnimation() {

        //koikoiSelectImage.gameObject.SetActive(false);

        if (koikoiSelectImage.sprite == buttonImage[0]) {
            hideBlackImage.SetActive(false);

            fieldManager.state = FieldManager.STATE.TURNCHANGE;

        } else if (koikoiSelectImage.sprite == buttonImage[1]) {
            //相手の文から獲得文数を引く
            result.SetScoreText();
            result.SetFlushListText();
            result.gameObject.SetActive(true);
        }
    }

    public void SetScoreText() {
        currentScoreText.text = scoreManager.turnPlayerScore + "文";
        playerScoreText.text = scoreManager.playerScore + "文";
        cpuScoreText.text = scoreManager.cpuScore + "文";
    }
}
