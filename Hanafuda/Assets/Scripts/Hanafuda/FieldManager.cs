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
    private GetCardField getCardField;

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
    public TURNPLAYER turnPlayer;
    public enum TURNPLAYER {
        PLAYER = 0,
        COM = 1
    }

    //ゲーム中の状態
    public STATE state;
    public enum STATE {
        NONE,
        DECK_SHUFLE,
        CARD_HAND_OUT,
        WAIT_SELECT_CARD,
        TURN_PLAYER_SELECT_CARD,
        DECK_CARD_PUT_FIELD,
        DECK_CARD_MOVE,
        WAIT_FIELD_SELECT_CARD,
        FIELD_SELECT_DECK_CARD_MOVE,
        GET_CARD_MOVE,
        CHECK_ROLE,
        RESULT,
        PAUSE
    }

    [SerializeField]
    private Color CARD_DARK_COLOR;
    
    //選択されたカード関連
    private GameObject targetCard;
    private Vector3 targetCard_ChangePosition;
    private Vector3 targetCard_ChangeRotation;
    private Vector3 targetCard_ChangeScale;
    private float targetCard_ChangePosition_MaxDistance;

    [SerializeField,Header("ターンプレイヤーが選択したカードの移動速度")]
    private float cardMoveSpeed;

    public List<GameObject> getCardList = new List<GameObject>();
    public GameObject playerSelectCard;

    // Use this for initialization
    void Start () {
        handoutCounter = 0;
        handCardTimer = HANDCARD_MAXTIMER;

        state = STATE.DECK_SHUFLE;
        turnPlayer = TURNPLAYER.PLAYER;
    }

    // Update is called once per frame
    void Update () {

        switch (gameManager.state) {
            case GameManager.STATE.HANAFUDA:

                switch (state) {
                    case STATE.DECK_SHUFLE: DeckShufle(); break;
                    case STATE.CARD_HAND_OUT: CardHandOut(); break;
                    case STATE.WAIT_SELECT_CARD: break;
                    case STATE.TURN_PLAYER_SELECT_CARD: SelectCard_Move(); break;
                    case STATE.DECK_CARD_PUT_FIELD: DeckCardPutField(); break;
                    case STATE.DECK_CARD_MOVE: SelectCard_Move(); break;
                    case STATE.WAIT_FIELD_SELECT_CARD: break;
                    case STATE.FIELD_SELECT_DECK_CARD_MOVE: SelectCard_Move(); break;
                    case STATE.GET_CARD_MOVE: GetCardMove(); break;
                    case STATE.CHECK_ROLE: break;
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
            if (player.GetIsHandOutMovement() && cpu.GetIsHandOutMovement() && field.GetIsHandOutMovement()) {
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
    /// 場のカードの色を変更する処理（複数のカードを変更する場合）
    /// </summary>
    public void SetFieldCardColor(List<GameObject> cardList,bool isColor) {
        foreach (var card in cardList) {

            if (isColor) card.GetComponent<SpriteRenderer>().color = Color.white;
            else card.GetComponent<SpriteRenderer>().color = CARD_DARK_COLOR;
        }
    }

    /// <summary>
    /// 場のカードの色を変更する処理（1枚のカードを変更する場合）
    /// </summary>
    public void SetFieldCardColor(GameObject card, bool isColor) {
        if (isColor) card.GetComponent<SpriteRenderer>().color = Color.white;
        else card.GetComponent<SpriteRenderer>().color = CARD_DARK_COLOR;
    }


    /// <summary>
    /// 何も取れるカードがないときにタッチできる用のコライダーのアクティブを設定
    /// </summary>
    public void SetFieldColliderActive(bool isActive) {
        fieldSpaceCollider.SetActive(isActive);
    }

    /// <summary>
    /// 選択されたカードを目的の座標、角度、大きさになるように動かす
    /// </summary>
    private void SelectCard_Move() {

        if (targetCard.transform.position != targetCard_ChangePosition) {
            TargetCardChangeTransform();

        }else {

            switch (state) {
                case STATE.TURN_PLAYER_SELECT_CARD:
                    //デッキからのカード追加処理に移行
                    //StartCoroutine(WaitNextState(0.1f, STATE.DECK_CARD_PUT_FIELD));
                    state = STATE.DECK_CARD_PUT_FIELD;
                    break;

                case STATE.DECK_CARD_MOVE:
                    //デッキから場に出た後の処理に移行
                    //StartCoroutine(WaitNextState(0.1f, STATE.GET_CARD_MOVE));
                    ChangeState_GetCardMove();
                    break;

                case STATE.FIELD_SELECT_DECK_CARD_MOVE:
                    //デッキから場に出た後の処理に移行
                    //StartCoroutine(WaitNextState(0.1f, STATE.GET_CARD_MOVE));
                    ChangeState_GetCardMove();
                    break;

                //case STATE.GET_CARD_MOVE:
                //    break;
            }
        }
    }
    //private IEnumerator WaitNextState(float waitTimer, STATE nextState) {
    //    yield return new WaitForSeconds(waitTimer);
    //    state = nextState;
    //}

    /// <summary>
    /// 取得したカードの移動処理になる時の設定
    /// </summary>
    private void ChangeState_GetCardMove() {

        playerSelectCard.transform.rotation = Quaternion.identity;
        getCardField.addGetCard_Dic.Clear();
        getCardField.checkRoleIndexList.Clear();

        foreach (var getCard in getCardList) {
            //今回ゲットしたカードを場のカードリストから削除する
            field.RemoveCard(getCard);
                        
            getCard.transform.parent = getCardField.transform;
            cardManager.SetCardTag(getCard, TAG.TagManager.GET_CARD_FIELD);
            cardManager.SetCardSortingLayer(getCard, SortingLayer.SortingLayerManager.GET_CARD_FIELD);
        }

        state = STATE.GET_CARD_MOVE;
    }

    /// <summary>
    /// 指定したカードの移動させる座標や大きさ、角度を設定
    /// </summary>
    public void SetMoveCardStatus(GameObject card, Vector3 position, Vector3 rotation, Vector3 scale, STATE state) {
        targetCard = card;
        targetCard_ChangePosition = position;
        targetCard_ChangeRotation = rotation;
        targetCard_ChangeScale = scale;
        targetCard_ChangePosition_MaxDistance = Vector3.Distance(card.transform.position, position);
        this.state = state;
    }

    /// <summary>
    /// 指定したカードの情報を変更する（移動、大きさ、角度）
    /// </summary>
    private void TargetCardChangeTransform() {
        //目標の座標まで動かす
        targetCard.transform.position = Vector3.MoveTowards(targetCard.transform.position,
                                                            targetCard_ChangePosition,
                                                            Time.deltaTime * cardMoveSpeed);

        //目標までの距離に近づくにつれて、大きさを変える
        var distance = Vector3.Distance(targetCard.transform.position, targetCard_ChangePosition);
        var distanceLeap = Mathf.Lerp(1f, 0f, distance / targetCard_ChangePosition_MaxDistance);
        targetCard.transform.localScale = Vector3.Lerp(targetCard.transform.localScale,
                                                       targetCard_ChangeScale,
                                                       distanceLeap);

        //場からカードが出たときはカードを回転させる（山札から出るときに場のカードを選択した場合は回転させない）
        if (state == STATE.DECK_CARD_MOVE) {

            //一定距離まで移動したら、回転させる
            if (distanceLeap >= handCardCenterDistanePer) {
                var spriteRenderer = targetCard.GetComponent<SpriteRenderer>();
                var card = targetCard.GetComponent<Card>();

                targetCard.transform.rotation = Quaternion.Slerp(targetCard.transform.rotation, Quaternion.identity, distanceLeap);

                //本当は中間まで回転したらの割合でやりたい
                if (targetCard.transform.rotation.eulerAngles.y >= 270.0f) {
                    spriteRenderer.sprite = card.image[0];
                }
            }
        }
    }

    /// <summary>
    /// 指定したカードの情報を変更する（移動、大きさ、角度）
    /// </summary>
    private void TargetCardChangeTransform(GameObject targetCard,Vector3 targetCard_ChangePosition,Vector3 targetCard_ChangeScale) {

        var targetCard_ChangePosition_MaxDistance = Vector3.Distance(targetCard.transform.position, targetCard_ChangePosition);


        //目標の座標まで動かす
        targetCard.transform.position = Vector3.MoveTowards(targetCard.transform.position,
                                                            targetCard_ChangePosition,
                                                            Time.deltaTime * cardMoveSpeed);

        //目標までの距離に近づくにつれて、大きさを変える
        var distance = Vector3.Distance(targetCard.transform.position, targetCard_ChangePosition);
        var distanceLeap = Mathf.Lerp(1f, 0f, distance / targetCard_ChangePosition_MaxDistance);
        targetCard.transform.localScale = Vector3.Lerp(targetCard.transform.localScale,
                                                        targetCard_ChangeScale,
                                                        distanceLeap);
    }

    /// <summary>
    /// デッキからカードを場に出す処理
    /// </summary>
    private void DeckCardPutField() {
        var deckCard = deck.transform.GetChild(0).gameObject;
        int putIndex = 0;

        //場から取れるカードのリストを取得
        field.SetGetCardPutIndexList(deckCard);

        if (field.getCardPutIndexList.Count == 0) {
            //getFieldCardList.Count == 0　・・・　デッキから場に出るカードで場のカードを取れない場合
            //fieldのisputがfalseになっているところから、ランダムで選んで場に出す
            var isPutList = field.GetPutField();
            putIndex = isPutList[Random.Range(0,isPutList.Count)];

            SetMoveCardStatus(deckCard, field.fieldPosition[putIndex], Vector3.zero, field.cardScale, STATE.DECK_CARD_MOVE);

            //var _deckCard = new List<GameObject>();
            //_deckCard.Add(deckCard);
            //field.fieldCard_Dic[putIndex] = _deckCard;
            field.fieldCard_Dic[putIndex].Add(deckCard);
            deckCard.transform.parent = field.transform;
            cardManager.SetCardTag(deckCard, TAG.TagManager.FIELD_CARD);
            cardManager.SetCardSortingLayer(deckCard, SortingLayer.SortingLayerManager.FIELD_CARD_FORE);

        } else {
            //1枚だけの場合
            if (field.getCardPutIndexList.Count == 1) {
                //同じ月の場所にデッキのカードを出す
                putIndex = field.getCardPutIndexList[0];
                getCardList.Add(deckCard);

                //場の対応した番号にあるカードを全て取得リストに追加
                for (int i = 0; i < field.fieldCard_Dic[putIndex].Count; i++) {
                    getCardList.Add(field.fieldCard_Dic[putIndex][i]);
                }

                SetMoveCardStatus(deckCard, field.fieldPosition[putIndex], Vector3.zero, field.cardScale, STATE.DECK_CARD_MOVE);

                deckCard.transform.parent = field.transform;
                cardManager.SetCardTag(deckCard, TAG.TagManager.FIELD_CARD);
                cardManager.SetCardSortingLayer(deckCard, SortingLayer.SortingLayerManager.FIELD_CARD_FORE);

            } else {
                //複数取れるカードがある場合
                switch (turnPlayer) {
                    case TURNPLAYER.PLAYER://プレイヤーのターンだったら、選択待ちにする
                        foreach (var index in field.nonGetCardPutIndexList) {
                            SetFieldCardColor(field.fieldCard_Dic[index], false);
                        }
                        
                        state = STATE.WAIT_FIELD_SELECT_CARD;

                        //deckCard.transform.parent = field.transform;
                        SetFieldCardColor(targetCard,false);

                        var spriteRenderer = deckCard.GetComponent<SpriteRenderer>();
                        var card = deckCard.GetComponent<Card>();

                        spriteRenderer.sprite = card.image[0];

                        cardManager.SetCardTag(deckCard, TAG.TagManager.FIELD_CARD);
                        cardManager.SetCardSortingLayer(deckCard, SortingLayer.SortingLayerManager.FIELD_CARD_FORE);
                        break;

                    case TURNPLAYER.COM://CPUのターンだったら、役が作れそうなら役を作る、作れないならランダム？
                        break;
                }
            }

        }

    }

    /// <summary>
    /// デッキから場に出るときに取得できるカードを選択した後の処理
    /// </summary>
    /// <param name="fieldCard"></param>
    public void FieldSelectCard(GameObject fieldCard) {
        var deckCard = deck.transform.GetChild(0).gameObject;
        var d_Card = deckCard.GetComponent<Card>();
        var f_Card = fieldCard.GetComponent<Card>();

        if (d_Card.month == f_Card.month) {
            field.SetSelectCardIndex(fieldCard);
            var putIndex = field.selectCardIndex;

            getCardList.Add(deckCard);

            for (int i = 0; i < field.fieldCard_Dic[putIndex].Count; i++) {
                getCardList.Add(field.fieldCard_Dic[putIndex][i]);
            }

            //カードの色を明るくする
            foreach (var index in field.nonGetCardPutIndexList) {
                SetFieldCardColor(field.fieldCard_Dic[index], true);
            }
            SetFieldCardColor(targetCard, true);

            SetMoveCardStatus(deckCard, field.fieldPosition[putIndex], Vector3.zero, field.cardScale, STATE.FIELD_SELECT_DECK_CARD_MOVE);

            deckCard.transform.parent = field.transform;
            cardManager.SetCardTag(deckCard, TAG.TagManager.FIELD_CARD);
            cardManager.SetCardSortingLayer(deckCard, SortingLayer.SortingLayerManager.FIELD_CARD_FORE);
        }
    }

    /// <summary>
    /// カードを取得カードの座標へ移動させる処理
    /// </summary>
    private void GetCardMove() {

        ////ターンプレイヤーの取得カードの座標へ移動
        var isMovement = true;
        foreach (var getCard in getCardList) {
            var g_Card = getCard.GetComponent<Card>();
            var index = getCardField.GetCardTypeIndex(g_Card);

            if (getCard.transform.position != getCardField.getCardPosition[index]) {
                TargetCardChangeTransform(getCard, getCardField.getCardPosition[index], getCardField.cardScale);
                isMovement = false;
            }
        }

        //全てのカードが移動しきっていたら、役ができたかどうかのチェックに移行
        if (isMovement) {
            state = STATE.CHECK_ROLE;
        }

        //foreach (var getCard in getCardList) {

        //    var g_Card = getCard.GetComponent<Card>();
        //    var index = getCardField.GetCardTypeIndex(g_Card);

        //    //今回追加されたカードがまだ追加されていなければ追加
        //    //if (!getCardField.getCard_Dic[index].Contains(getCard)) {
        //    if (!getCardField.addGetCard_Dic.Contains(getCard)) {
        //        field.fieldCard_Dic[field.selectCardIndex].Remove(getCard);
        //        //getCardField.getCard_Dic[index].Add(getCard);
        //        getCardField.addGetCard_Dic.Add(getCard);

        //        getCard.transform.parent = getCardField.transform;
        //        cardManager.SetCardTag(getCard, TAG.TagManager.GET_CARD_FIELD);
        //        cardManager.SetCardSortingLayer(getCard, SortingLayer.SortingLayerManager.GET_CARD_FIELD);
        //    }
        //    //今回追加されたカードで、役ができたかどうかをチェックするために対応した番号を設定
        //    if (!getCardField.checkRoleIndexList.Contains(index)) {
        //        getCardField.checkRoleIndexList.Add(index);
        //    }

        //    if (getCard.transform.position != getCardField.getCardPosition[index]) {
        //        TargetCardChangeTransform(getCard, getCardField.getCardPosition[index], getCardField.cardScale);
        //    }

        //}
    }
}
