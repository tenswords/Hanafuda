using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FieldManager : MonoBehaviour {

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
    private GameObject dialogCanvas;

    [SerializeField]
    private GameObject hideBlackImage;

    [SerializeField]
    private GameObject koikoiDialog;

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
        READY,
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
        ESTABLISHROLE,
        WAIT_ESTABLISHROLE_ANIMATION,
        TURNCHANGE,
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

    [HideInInspector]
    public int establishRoleIndex;
    [HideInInspector]
    public List<string> establishRole_FlushList = new List<string>();

    [SerializeField]
    private EstablishRoleImage establishRoleImage;

    [SerializeField]
    private Sprite[] roleNameImageList;
    public Dictionary<string, Sprite> roleNameImage_Dic = new Dictionary<string, Sprite>();
    public Dictionary<string, int> roleScore_Dic = new Dictionary<string, int>();

    private Quaternion TURN_CARD_ROTATION = Quaternion.Euler(new Vector3(0.0f, 359.9f, 0.0f));


    /// <summary>
    /// 指定した時間後に次の状態へ遷移する
    /// </summary>
    private IEnumerator WaitNextState(float waitTimer, STATE nextState) {
        yield return new WaitForSeconds(waitTimer);
        state = nextState;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize() {
        playerSelectCard = null;
        getCardList.Clear();

        foreach (var fieldCard in field.fieldCard_Dic) {
            if(fieldCard.Value.Count > 0) {
                for (int i=0;i< fieldCard.Value.Count; i++) {
                    cardManager.SetCardOrderInLayer(fieldCard.Value[i],i);
                }
            }
        }

    }

    // Use this for initialization
    void Start () {
        handoutCounter = 0;
        handCardTimer = HANDCARD_MAXTIMER;
        turnPlayer = TURNPLAYER.PLAYER;

        roleNameImage_Dic.Add(Role.RoleManager.SANKOU, roleNameImageList[0]);
        roleNameImage_Dic.Add(Role.RoleManager.YONKOU, roleNameImageList[1]);
        roleNameImage_Dic.Add(Role.RoleManager.AMEYONKOU, roleNameImageList[2]);
        roleNameImage_Dic.Add(Role.RoleManager.GOKOU, roleNameImageList[3]);
        roleNameImage_Dic.Add(Role.RoleManager.INOSHIKATYOU, roleNameImageList[4]);
        roleNameImage_Dic.Add(Role.RoleManager.TUKIMIZAKE, roleNameImageList[5]);
        roleNameImage_Dic.Add(Role.RoleManager.HANAMIZAKE, roleNameImageList[6]);
        roleNameImage_Dic.Add(Role.RoleManager.TANE, roleNameImageList[7]);
        roleNameImage_Dic.Add(Role.RoleManager.AKATANZAKU, roleNameImageList[8]);
        roleNameImage_Dic.Add(Role.RoleManager.AOTANZAKU, roleNameImageList[9]);
        roleNameImage_Dic.Add(Role.RoleManager.TANZAKU, roleNameImageList[10]);
        roleNameImage_Dic.Add(Role.RoleManager.KASU, roleNameImageList[11]);

        roleScore_Dic.Add(Role.RoleManager.SANKOU, Role.RoleManager.SCORE_SANKOU);
        roleScore_Dic.Add(Role.RoleManager.YONKOU, Role.RoleManager.SCORE_YONKOU);
        roleScore_Dic.Add(Role.RoleManager.AMEYONKOU, Role.RoleManager.SCORE_AMEYONKOU);
        roleScore_Dic.Add(Role.RoleManager.GOKOU, Role.RoleManager.SCORE_GOKOU);
        roleScore_Dic.Add(Role.RoleManager.INOSHIKATYOU, Role.RoleManager.SCORE_INOSHIKATYOU);
        roleScore_Dic.Add(Role.RoleManager.TUKIMIZAKE, Role.RoleManager.SCORE_TUKIMIZAKE);
        roleScore_Dic.Add(Role.RoleManager.HANAMIZAKE, Role.RoleManager.SCORE_HANAMIZAKE);
        roleScore_Dic.Add(Role.RoleManager.TANE, Role.RoleManager.SCORE_TANE);
        roleScore_Dic.Add(Role.RoleManager.AKATANZAKU, Role.RoleManager.SCORE_AKATANZAKU);
        roleScore_Dic.Add(Role.RoleManager.AOTANZAKU, Role.RoleManager.SCORE_AOTANZAKU);
        roleScore_Dic.Add(Role.RoleManager.TANZAKU, Role.RoleManager.SCORE_TANZAKU);
        roleScore_Dic.Add(Role.RoleManager.KASU, Role.RoleManager.SCORE_KASU);

        StartCoroutine(WaitNextState(1.0f, STATE.DECK_SHUFLE));
    }

    // Update is called once per frame
    void Update () {

        switch (state) {
            case STATE.READY: break;
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
            case STATE.ESTABLISHROLE: EstablishRole(); break;
            case STATE.TURNCHANGE: TurnChange(); break;
            case STATE.RESULT:  break;
            case STATE.PAUSE: break;
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

        Debug.Log("ターンチェンジ");
        if(turnPlayer == TURNPLAYER.PLAYER) turnPlayer = TURNPLAYER.COM;
        else if (turnPlayer == TURNPLAYER.COM) turnPlayer = TURNPLAYER.PLAYER;

        switch (turnPlayer) {
            case TURNPLAYER.PLAYER:
                player.Initialize();
                break;
            case TURNPLAYER.COM:
                break;
        }

        state = STATE.WAIT_SELECT_CARD;

        Initialize();
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

        if(turnPlayer == TURNPLAYER.PLAYER) playerSelectCard.transform.rotation = Quaternion.identity;

        if (getCardList.Count == 0) {
            state = STATE.TURNCHANGE;

        } else if (getCardList.Count > 0) {
            foreach (var getCard in getCardList) {
                //今回ゲットしたカードを場のカードリストから削除する
                field.RemoveCard(getCard);

                getCard.transform.parent = getCardField.transform;
                cardManager.SetCardTag(getCard, TAG.TagManager.GET_CARD_FIELD);
                cardManager.SetCardSortingLayer(getCard, SortingLayer.SortingLayerManager.GET_CARD_FIELD);
                //cardManager.SetCardOrderInLayer(getCard, 0);
            }

            state = STATE.GET_CARD_MOVE;
        }

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

        //場からカードかCOMの手札から出たときはカードを回転させる（山札から出るときに場のカードを選択した場合は回転させない）
        //if ((state == STATE.TURN_PLAYER_SELECT_CARD && turnPlayer == TURNPLAYER.COM) || state == STATE.DECK_CARD_MOVE) {
        if (turnPlayer == TURNPLAYER.COM || state == STATE.DECK_CARD_MOVE) {

            //一定距離まで移動したら、回転させる
            //if (distanceLeap >= handCardCenterDistanePer) {
            var spriteRenderer = targetCard.GetComponent<SpriteRenderer>();
            var card = targetCard.GetComponent<Card>();

            targetCard.transform.rotation = Quaternion.Slerp(targetCard.transform.rotation, TURN_CARD_ROTATION, distanceLeap);

            //本当は中間まで回転したらの割合でやりたい
            if (targetCard.transform.rotation.eulerAngles.y >= 270.0f && spriteRenderer.sprite != card.image[0]) {
                spriteRenderer.sprite = card.image[0];
            }
            //}
        }
    }

    /// <summary>
    /// 指定したカードの情報を変更する（移動、大きさ、角度）
    /// </summary>
    private void TargetCardChangeTransform(GameObject targetCard,int index,Vector3 targetCard_ChangeScale) {

        var targetCard_ChangePosition = getCardField.getCardPosition[index];
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

        //8割り以上移動したら、カードの情報を変更する
        if (distanceLeap >= 0.8f) {
            var currentOrderInLayerNo = targetCard.GetComponent<SpriteRenderer>().sortingOrder;
            var orderInLayerNo = getCardField.getCard_Dic[index].Count;

            if (currentOrderInLayerNo != orderInLayerNo) {
                cardManager.SetCardOrderInLayer(targetCard, orderInLayerNo);
            }
        }
    }

    /// <summary>
    /// デッキからカードを場に出す処理
    /// </summary>
    private void DeckCardPutField() {
        var deckCard = deck.transform.GetChild(0).gameObject;
        int putIndex = -1;

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
            cardManager.SetCardSortingLayer(deckCard, SortingLayer.SortingLayerManager.FIELD_CARD);
            cardManager.SetCardOrderInLayer(deckCard, 4);

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
                cardManager.SetCardSortingLayer(deckCard, SortingLayer.SortingLayerManager.FIELD_CARD);
                cardManager.SetCardOrderInLayer(deckCard, 4);

            } else {
                //複数取れるカードがある場合

                switch (turnPlayer) {
                    case TURNPLAYER.PLAYER://プレイヤーのターンだったら、選択待ちにする
                        foreach (var index in field.nonGetCardPutIndexList) {
                            SetFieldCardColor(field.fieldCard_Dic[index], false);
                        }
                        
                        //state = STATE.WAIT_FIELD_SELECT_CARD;

                        SetFieldCardColor(targetCard,false);

                        var spriteRenderer = deckCard.GetComponent<SpriteRenderer>();
                        var card = deckCard.GetComponent<Card>();

                        spriteRenderer.sprite = card.image[0];
                        break;

                    case TURNPLAYER.COM://CPUのターンだったら、役が作れそうなら役を作る、作れないならランダムで選ぶ
                        Debug.Log("CPUデッキから出たときに複数ある");
                        break;
                }

                state = STATE.WAIT_FIELD_SELECT_CARD;
                cardManager.SetCardTag(deckCard, TAG.TagManager.FIELD_CARD);
                cardManager.SetCardSortingLayer(deckCard, SortingLayer.SortingLayerManager.FIELD_CARD);
                cardManager.SetCardOrderInLayer(deckCard, 4);
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
        }
    }

    /// <summary>
    /// デッキから場に出るときに取得できるカードを選択した後の処理
    /// </summary>
    public void FieldSelectCard(int selectIndex) {
        var deckCard = deck.transform.GetChild(0).gameObject;

        field.SetSelectCardIndex(selectIndex);
        var putIndex = field.selectCardIndex;

        getCardList.Add(deckCard);

        for (int i = 0; i < field.fieldCard_Dic[putIndex].Count; i++) {
            getCardList.Add(field.fieldCard_Dic[putIndex][i]);
        }
        SetMoveCardStatus(deckCard, field.fieldPosition[putIndex], Vector3.zero, field.cardScale, STATE.FIELD_SELECT_DECK_CARD_MOVE);
        deckCard.transform.parent = field.transform;
    }


    /// <summary>
    /// カードを取得カードの座標へ移動させる処理
    /// </summary>
    private void GetCardMove() {

        //ターンプレイヤーの取得カードの座標へ移動
        var isMovement = true;
        foreach (var getCard in getCardList) {
            var g_Card = getCard.GetComponent<Card>();
            var index = getCardField.GetCardTypeIndex(g_Card);

            if (getCard.transform.position != getCardField.getCardPosition[index]) {
                TargetCardChangeTransform(getCard, index, getCardField.cardScale);
                isMovement = false;
            }
        }

        //全てのカードが移動しきっていたら、役ができたかどうかのチェックに移行
        if (isMovement) {
            state = STATE.CHECK_ROLE;
        }
    }

    public void TurnPlayerAddScore(string roleName) {
        //スコア加算処理
        switch (turnPlayer) {
            case FieldManager.TURNPLAYER.PLAYER:
                player.score += roleScore_Dic[roleName];
                break;
            case FieldManager.TURNPLAYER.COM:
                cpu.score += roleScore_Dic[roleName];
                break;
        }
    }



    /// <summary>
    /// 役成立時の処理
    /// </summary>
    private void EstablishRole() {
        dialogCanvas.SetActive(true);
        hideBlackImage.SetActive(true);

        establishRoleImage.SetEstablishRoleFlushCount(establishRole_FlushList.Count);
        PlayEstablishRoleAnimation();
        //establishRoleImage.StartAnimation(getCardField.establishRoleSList[establishRoleIndex][2]);
        state = STATE.WAIT_ESTABLISHROLE_ANIMATION;
    }

    /// <summary>
    /// 役成立時のエフェクトアニメーションが全て終了したときの処理
    /// </summary>
    public void EstablishRole_EndAnimation() {

        establishRoleIndex = 0;

        //成立した役のアニメーション処理が全て終了した場合、こいこいするかどうかの処理
        switch (turnPlayer) {
            case TURNPLAYER.PLAYER:
                //プレイヤーのターンだったら、こいこいするかどうかのダイアログを表示
                hideBlackImage.SetActive(false);
                koikoiDialog.SetActive(true);
                break;

            case TURNPLAYER.COM:
                //CPUのターンだったら
                //手札が3枚よりも多い場合、必ずこいこいする
                //手札が3枚以下の場合、今の手札と場のカードを調べてあがれる役があるならこいこいする、ないならあがる

                //state = FieldManager.STATE.TURNCHANGE;
                break;
        }
    }

    public void PlayEstablishRoleAnimation() {
        establishRoleImage.SetRoleName(establishRole_FlushList[establishRoleIndex]);
        establishRoleImage.gameObject.SetActive(true);
    }

}
