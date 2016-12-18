using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldManager : MonoBehaviour {

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private CardManager cardManager;

    [SerializeField]
    private Player player;
    [SerializeField]
    private Cpu cpu;
    [SerializeField]
    private Field field;
    
    [SerializeField]
    private GameObject deck;

    [SerializeField]
    private GameObject fieldSpaceCollider;

    public float handCardSpeed;
    //プレイヤーに配る時にどこまで移動したら表向きになるかの割合
    public float handCardCenterDistanePer;

    private int handoutCounter;
    [SerializeField,Header("配る速度")]
    private float HANDCARD_MAXTIMER;
    private float handCardTimer;
    [SerializeField, Header("お互いの手札やフィールドにカードを配る間隔")]
    private float HANDCARD_MAXINTERVAL;
    private float handCardInterval;

    [Header("現在のターンプレイヤー")]
    public TURNPLAYER tunePlayer;
    public enum TURNPLAYER {
        PLAYER = 0,
        COM = 1
    }

    //ゲーム中の状態
    public STATE state;
    public enum STATE {
        DECK_SHUFLE,
        CARD_HAND_OUT,
        WAIT_SELECT_CARD,
        TURN_PLAYER_SELECT_CARD,
        RESULT,
        PAUSE
    }

    [SerializeField]
    private Color CARD_DARK_COLOR;
    
    //ターンプレイヤーが選択したカード関連
    private GameObject selectCard;
    private Vector3 selectCard_MovePosition;
    private Vector3 selectCard_OriginScale;
    private float selectCard_MovePosition_MaxDistance;

    [SerializeField,Header("ターンプレイヤーが選択したカードの移動速度")]
    private float selectCard_MoveSpeed;

    // Use this for initialization
    void Start () {
        handoutCounter = 0;
        handCardTimer = HANDCARD_MAXTIMER;

        state = STATE.DECK_SHUFLE;
        tunePlayer = TURNPLAYER.PLAYER;
    }

    // Update is called once per frame
    void Update () {

        switch (gameManager.state) {
            case GameManager.STATE.HANAFUDA:

                switch (state) {
                    case STATE.DECK_SHUFLE: DeckShufle(); break;
                    case STATE.CARD_HAND_OUT: CardHandOut(); break;
                    case STATE.WAIT_SELECT_CARD:  break;
                    case STATE.TURN_PLAYER_SELECT_CARD: TurnPlayerSelectCard_Move(); break;
                    case STATE.RESULT:  break;
                    case STATE.PAUSE: break;
                }
                break;
        }
    }
    /// <summary>
    /// 山札をランダムに並び替える
    /// </summary>
    private void DeckShufle() {
        Debug.Log("FieldManager DeckShufle");
        //（とりあえず100回シャッフル）
        for (int i = 0; i < 100; i++) {
            var change1 = Random.Range(0, deck.transform.childCount);
            var change2 = Random.Range(0, deck.transform.childCount);
            deck.transform.GetChild(change1).SetSiblingIndex(change2);
        }
        state = STATE.CARD_HAND_OUT;
    }

    /// <summary>
    /// カードを配る処理
    /// </summary>
    private void CardHandOut() {
        Debug.Log("FieldManager CardHandOut");
        if (handoutCounter == 24) {
            //全てのカードが指定の座標まで移動しきっていたら、ゲーム状態に変更
            if (player.GetIsHandOutMovement()&& cpu.GetIsHandOutMovement() && field.GetIsHandOutMovement()) {
                state = STATE.WAIT_SELECT_CARD;
            }
        } else {

            handCardInterval -= Time.deltaTime;
            if (handCardInterval < 0.0f) {

                handCardTimer -= Time.deltaTime;
                if (handCardTimer < 0.0f) {

                    CardHandOutProcess();
                    handoutCounter++;

                    handCardTimer = HANDCARD_MAXTIMER;

                    if (handoutCounter % 3 == 0) {
                        handCardInterval = HANDCARD_MAXINTERVAL;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 各場所にカードを配る処理
    /// </summary>
    public void CardHandOutProcess() {

        var where = (handoutCounter / 4) % 3;

        var card = deck.transform.GetChild(0).gameObject;
        switch (where) {
            case 0://プレイヤーの手札にカードを配る
                player.AddCard(card);
                break;
            case 1://場にカードを配る
                field.AddCard(card);
                break;
            case 2://CPUの手札にカードを配る
                cpu.AddCard(card);
                break;
        }
    }

    /// <summary>
    /// ターンが切り替わる時の初期化処理
    /// </summary>
    private void TurnChange() {

    }

    /// <summary>
    /// 場のカードの色を変更する処理
    /// </summary>
    public void SetFieldCardColor(List<GameObject> cardList,bool isColor) {
        foreach (var card in cardList) {

            if (isColor) card.GetComponent<SpriteRenderer>().color = Color.white;
            else card.GetComponent<SpriteRenderer>().color = CARD_DARK_COLOR;
            Debug.Log("card name " + card.name);
        }
    }

    /// <summary>
    /// 何も取れるカードがないときにタッチできる用のコライダーのアクティブを設定
    /// </summary>
    public void SetFieldColliderActive(bool isActive) {
        fieldSpaceCollider.SetActive(isActive);
    }

    /// <summary>
    /// ターンプレイヤーが選択したカードを目的の座標、角度、大きさになるように動かす
    /// </summary>
    private void TurnPlayerSelectCard_Move() {
        selectCard.transform.position = Vector3.MoveTowards(selectCard.transform.position,
                                                            selectCard_MovePosition,
                                                            Time.deltaTime * selectCard_MoveSpeed);

        //目標までの距離に近づくにつれて、大きさを変える
        var distance = Vector3.Distance(selectCard.transform.position, selectCard_MovePosition);
        var distanceLeap = Mathf.Lerp(1f, 0f, distance/selectCard_MovePosition_MaxDistance);
        Debug.Log("distanceLeap "+ distanceLeap);
        selectCard.transform.localScale = selectCard_OriginScale * distanceLeap;

        if(selectCard.transform.position == selectCard_MovePosition) {
        }

    }

    /// <summary>
    /// 指定したカードの移動させる座標を設定
    /// </summary>
    public void SetMoveCardStatus(GameObject card,Vector3 movePosition,STATE state) {
        selectCard = card;
        selectCard_MovePosition = movePosition;
        selectCard_OriginScale = card.transform.localScale;
        selectCard_MovePosition_MaxDistance = Vector3.Distance(card.transform.position, movePosition);
        this.state = state;
    }
}
