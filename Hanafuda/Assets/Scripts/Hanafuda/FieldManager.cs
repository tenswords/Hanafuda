using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FieldManager : MonoBehaviour {

    [SerializeField]
    private CardManager cardManager;
    [SerializeField]
    private ScoreManager scoreManager;
    
    [SerializeField]
    private Player player;
    [SerializeField]
    private Cpu cpu;
    [SerializeField]
    private Field field;
    [SerializeField]
    private GetCardField getCardField;

    [SerializeField, Header("対局開始イメージ")]
    private Image gameStartImage;

    [SerializeField]
    private GameObject dialogCanvas;

    [SerializeField]
    private GameObject hideBlackImage;

    [SerializeField]
    private KoiKoiDialog koikoiDialog;

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
        NONE = 0,
        PLAYER = 1,
        COM = 2
    }

    //ゲーム中の状態
    public STATE state;
    public enum STATE {
        NONE,
        READY,
        DECK_SHUFLE,
        CARD_HAND_OUT,
        SAME_MONTH_MOVE,
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
    private Image establishRoleImage;

    [SerializeField]
    private Sprite[] roleNameImageList;
    public string[][] arrayRoleType;
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

        GameManager.Instance.state = GameManager.STATE.HANAFUDA;

        if (state != STATE.RESULT) {

            AudioManager.Instance.PlayBGM(AudioName.AudioNameManager.BGM_BGM_CHAPTER2_3, true, 1.0f);

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

            arrayRoleType = new string[][] { new string[]{ Role.RoleManager.GOKOU, Role.RoleManager.YONKOU, Role.RoleManager.AMEYONKOU, Role.RoleManager.SANKOU },
                                            new string[]{ Role.RoleManager.HANAMIZAKE, Role.RoleManager.TUKIMIZAKE, Role.RoleManager.INOSHIKATYOU, Role.RoleManager.TANE },
                                            new string[]{ Role.RoleManager.AKATANZAKU, Role.RoleManager.AOTANZAKU, Role.RoleManager.TANZAKU },
                                            new string[]{ Role.RoleManager.KASU }
                                            };

            //StartCoroutine(WaitNextState(1.0f, STATE.DECK_SHUFLE));
            StartCoroutine(StartGame());
        }
    }

    // Update is called once per frame
    void Update () {

        switch (state) {
            case STATE.READY: break;
            case STATE.DECK_SHUFLE: DeckShufle(); break;
            case STATE.CARD_HAND_OUT: CardHandOut(); break;
            case STATE.SAME_MONTH_MOVE: SameMonthMove(); break;
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
    /// 対局開始のアニメーション再生
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartGame() {
        yield return new WaitForSeconds(1.0f);

        var time = 0.0f;
        var interval = 1.0f;
        var targetScale = new Vector3(1.0f,1.0f,1.0f);
        var rect = gameStartImage.GetComponent<RectTransform>();

        gameStartImage.gameObject.SetActive(true);

        while (time<interval) {
            time += Time.deltaTime;
            var lerp = Mathf.Lerp(0f, 1f, time / interval);
            rect.localScale = Vector3.Lerp(rect.localScale, targetScale,lerp);
            gameStartImage.color = Color.Lerp(gameStartImage.color, Color.white, lerp);
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
            gameStartImage.color = Color.Lerp(gameStartImage.color, Color.clear, lerp);
            yield return 0;
        }

        gameStartImage.gameObject.SetActive(false);

        state = STATE.DECK_SHUFLE;
    }

    /// <summary>
    /// 山札をランダムに並び替える
    /// </summary>
    private void DeckShufle() {
        //Debug.Log("FieldManager DeckShufle");
        //（とりあえず100回シャッフル）

        for (int i = 0; i < 100; i++) {
            var change1 = Random.Range(0, deck.transform.childCount);
            var change2 = Random.Range(0, deck.transform.childCount);
            deck.transform.GetChild(change1).SetSiblingIndex(change2);
        }

        //フィールドに４枚、同じ月のカードが配られる場合は再びシャッフル
        if (CheckSameMonthCard()) {
            DeckShufle();
            Debug.Log("フィールドに４枚、同じ月のカードが配られるため　再びシャッフル");
        } else {
            state = STATE.CARD_HAND_OUT;
        }
    }

    /// <summary>
    /// シャッフル時にフィールドに同じ月のカードが４枚配られるかどうかを取得
    /// </summary>
    /// <returns></returns>
    private bool CheckSameMonthCard() {

        var sameMonthIndexList = new List<int>();
        var checkMonthList = new List<Card.MONTH>();
        var checkCount = 3;
        var startIndex = 0;
        var startIndex_tmp = 0;

        for (int i = 0; i < 8 - checkCount; i++) {

            if (i / 4 == 0) startIndex = 4;
            else if (i / 4 == 1) startIndex = 16;

            var d_card = deck.transform.GetChild(startIndex + (i % 4)).GetComponent<Card>();

            //現在のカードの月を一度もチェックしていない場合
            if (!checkMonthList.Contains(d_card.month)) {
                checkMonthList.Add(d_card.month);

                sameMonthIndexList.Clear();
                sameMonthIndexList.Add(i);

                for (int j = i + 1; j < 8 + (-checkCount + sameMonthIndexList.Count); j++) {
                    if (j / 4 == 0) startIndex_tmp = 4;
                    else if (j / 4 == 1) startIndex_tmp = 16;

                    var d_card_tmp = deck.transform.GetChild(startIndex_tmp + (j % 4)).GetComponent<Card>();

                    if (d_card.month == d_card_tmp.month) {
                        sameMonthIndexList.Add(j);
                        if (sameMonthIndexList.Count == 4) return true;
                    }
                }
            }
        }
        return false;
    }


    private List<int> sameMonthIndexList = new List<int>();
    /// <summary>
    /// カードを配る処理
    /// </summary>
    private void CardHandOut() {
        //Debug.Log("FieldManager CardHandOut");
        if (handoutCounter == 24) {
            //全てのカードが指定の座標まで移動しきっていたら、ゲーム状態に変更
            if (player.GetIsHandOutMovement() && cpu.GetIsHandOutMovement() && field.GetIsHandOutMovement()) {

                //フィールドに同じ月のカードが３枚ある場合は、それらのカードを合わせてから次へ進む処理
                sameMonthIndexList = GetSameMonthIndexList();
                if (sameMonthIndexList != null) {
                    state = STATE.SAME_MONTH_MOVE;
                } else {
                    state = STATE.WAIT_SELECT_CARD;
                }
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
    /// 山札から配られた際に同じ月のカードが、フィールドに３枚あるかどうかを取得
    /// </summary>
    /// <returns></returns>
    private List<int> GetSameMonthIndexList() {

        var sameMonthIndexList = new List<int>();
        var checkMonthList = new List<Card.MONTH>();
        var checkCount = 2;

        for (int i=0; i<field.fieldCard_Dic.Count-4- checkCount; i++) {

            var f_card = field.fieldCard_Dic[i][0].GetComponent<Card>();

            //現在のカードの月を一度もチェックしていない場合
            if (!checkMonthList.Contains(f_card.month)) {
                checkMonthList.Add(f_card.month);

                sameMonthIndexList.Clear();
                sameMonthIndexList.Add(i);

                for (int j = i + 1; j < field.fieldCard_Dic.Count - 4 + (-checkCount + sameMonthIndexList.Count); j++) {
                    var f_card_tmp = field.fieldCard_Dic[j][0].GetComponent<Card>();

                    if (f_card.month == f_card_tmp.month) {
                        sameMonthIndexList.Add(j);
                        if (sameMonthIndexList.Count == 3) return sameMonthIndexList;
                    }
                }
            }
        }
        return null;
    }


    private bool onceIsSame;
    private bool isSameMove;
    private bool isSameTarget;
    private Vector3 targetMovePosition;
    /// <summary>
    /// 同じ月のカードが３枚ある場合、カードを動かす処理
    /// </summary>
    private void SameMonthMove() {

        if (!onceIsSame) {
            onceIsSame = true;
            StartCoroutine(WaitSameMonth());
        }

        if (isSameMove) {
            if (!isSameTarget) {
                isSameTarget = true;
                targetMovePosition = field.fieldCard_Dic[sameMonthIndexList[0]][0].transform.position;

                //描画順の変更
                for (int i = 1; i < sameMonthIndexList.Count; i++) {
                    //描画順の変更
                    var targetCard = field.fieldCard_Dic[sameMonthIndexList[i]][0];
                    cardManager.SetCardOrderInLayer(targetCard, i);
                }
            }

            var isEndMoveCount = 0;
            for (int i = 1; i < sameMonthIndexList.Count; i++) {
                var targetCard = field.fieldCard_Dic[sameMonthIndexList[i]][0];
                //目標の座標まで動かす
                targetCard.transform.position = Vector3.MoveTowards(targetCard.transform.position,
                                                                    targetMovePosition,
                                                                    Time.deltaTime * cardMoveSpeed);

                if (targetCard.transform.position == targetMovePosition) {
                    isEndMoveCount++;
                }
            }

            //全て移動し終えていたら、プレイヤーのタッチ待ちにする
            if (isEndMoveCount == sameMonthIndexList.Count - 1) {
                state = STATE.WAIT_SELECT_CARD;

                //合わせる場所にカードを追加
                for (int i = 1; i < sameMonthIndexList.Count; i++) {
                    var targetCard = field.fieldCard_Dic[sameMonthIndexList[i]][0];
                    //合わせる場所にカードを追加
                    field.fieldCard_Dic[sameMonthIndexList[0]].Add(targetCard);
                    field.fieldCard_Dic[sameMonthIndexList[i]].Clear();
                }
            }
        }
    }

    /// <summary>
    /// 同じ月のカードを移動させるた
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitSameMonth() {
        yield return new WaitForSeconds(0.25f);
        isSameMove = true;
    }


    private bool isTurnChange;
    /// <summary>
    /// ターンが切り替わる時の初期化処理
    /// </summary>
    private void TurnChange() {

        //プレイヤーの手札とCPUの手札が０枚になった場合、対局終了となる
        if (player.GetHandCardCount() == 0 && cpu.GetHandCardCount() == 0) {

            //koikoiDialog.gameObject.SetActive(true);
            scoreManager.turnPlayerScore = 0;
            koikoiDialog.OnAgariButton();

        } else {

            if (!isTurnChange) {
                isTurnChange = true;
                StartCoroutine(WaitTurnChange());
            }
            //Debug.Log("ターンチェンジ");
            //if (turnPlayer == TURNPLAYER.PLAYER) turnPlayer = TURNPLAYER.COM;
            //else if (turnPlayer == TURNPLAYER.COM) turnPlayer = TURNPLAYER.PLAYER;

            //switch (turnPlayer) {
            //    case TURNPLAYER.PLAYER:
            //        player.Initialize();
            //        break;
            //    case TURNPLAYER.COM:
            //        break;
            //}
            //state = STATE.WAIT_SELECT_CARD;
            //Initialize();
        }
    }

    private IEnumerator WaitTurnChange() {
        
        yield return new WaitForSeconds(0.2f);

        Debug.Log("ターンチェンジ");
        if (turnPlayer == TURNPLAYER.PLAYER) turnPlayer = TURNPLAYER.COM;
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

        isTurnChange = false;
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

            //foreach (var getCard in getCardList) {
            //    //今回ゲットしたカードを場のカードリストから削除する
            //    field.RemoveCard(getCard);
            //}

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

            //置ける場所がない場合、プレイヤーが選択した場所に置く
            if (isPutList.Count == 0) {
                putIndex = field.selectCardIndex;
            } else {
                putIndex = isPutList[Random.Range(0, isPutList.Count)];
            }

            SetMoveCardStatus(deckCard, field.fieldPosition[putIndex], Vector3.zero, field.cardScale, STATE.DECK_CARD_MOVE);

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
                        //とりあえずランダム
                        Debug.Log("CPUデッキから出たときに複数ある");


                        //putIndex = field.getCardPutIndexList[0];
                        //getCardList.Add(deckCard);

                        ////場の対応した番号にあるカードを全て取得リストに追加
                        //for (int i = 0; i < field.fieldCard_Dic[putIndex].Count; i++) {
                        //    getCardList.Add(field.fieldCard_Dic[putIndex][i]);
                        //}

                        //SetMoveCardStatus(deckCard, field.fieldPosition[putIndex], Vector3.zero, field.cardScale, STATE.DECK_CARD_MOVE);

                        //deckCard.transform.parent = field.transform;
                        //cardManager.SetCardTag(deckCard, TAG.TagManager.FIELD_CARD);
                        //cardManager.SetCardSortingLayer(deckCard, SortingLayer.SortingLayerManager.FIELD_CARD);
                        //cardManager.SetCardOrderInLayer(deckCard, 4);



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
    /// デッキから出るときに置ける場所がなかった場合、プレイヤーのカードの移動が終わってから移動
    /// </summary>
    private void WaitDeckPutMove() {

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

                //今回成立した役が光系であり、自分の成立済み役リストの中に他の光系の役がある場合、
                //その役のスコアを取り消し、リストからも削除する
                if (roleName.Contains("光")) {
                    var hikariName = player.GetFlushListHikari();
                    //光系の役があった場合
                    if(hikariName != "") {
                        //player.score -= roleScore_Dic[roleName];
                        player.flushList.Remove(hikariName);
                    }
                }
                //新規の役の場合、役とスコアを登録
                //同じ役がある場合、登録されている役のスコアを＋１する
                if (!player.flushList.ContainsKey(roleName)) {
                    //player.score += roleScore_Dic[roleName];
                    player.flushList.Add(roleName, roleScore_Dic[roleName]);
                    //player.establishRole_FlushScoreList.Add(roleScore_Dic[roleName]);
                } else {
                    player.flushList[roleName] += 1;
                }

                break;
            case FieldManager.TURNPLAYER.COM:

                //今回成立した役が光系であり、自分の成立済み役リストの中に他の光系の役がある場合、
                //その役のスコアを取り消し、リストからも削除する
                if (roleName.Contains("光")) {
                    var hikariName = cpu.GetFlushListHikari();
                    //光系の役があった場合
                    if (hikariName != "") {
                        //cpu.score -= roleScore_Dic[roleName];
                        cpu.flushList.Remove(hikariName);
                    }
                }
                //新規の役の場合、役とスコアを登録
                //同じ役がある場合、登録されている役のスコアを＋１する
                if (!cpu.flushList.ContainsKey(roleName)) {
                    //cpu.score += roleScore_Dic[roleName];
                    cpu.flushList.Add(roleName, roleScore_Dic[roleName]);
                    //cpu.establishRole_FlushScoreList.Add(roleScore_Dic[roleName]);
                } else {
                    cpu.flushList[roleName] += 1;
                }
                break;
        }
    }

    /// <summary>
    /// 役成立時の処理
    /// </summary>
    private void EstablishRole() {
        dialogCanvas.SetActive(true);
        hideBlackImage.SetActive(true);

        //establishRoleImage.SetEstablishRoleFlushCount(establishRole_FlushList.Count);
        //PlayEstablishRoleAnimation();

        state = STATE.WAIT_ESTABLISHROLE_ANIMATION;
        StartCoroutine(PlayEstablishRoleAnimation());
        //state = STATE.WAIT_ESTABLISHROLE_ANIMATION;
    }

    //public void PlayEstablishRoleAnimation() {
    //    establishRoleImage.SetRoleName(establishRole_FlushList[establishRoleIndex]);
    //    establishRoleImage.gameObject.SetActive(true);
    //}

    /// <summary>
    /// あがり役のアニメーション再生
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayEstablishRoleAnimation() {
        var time = 0.0f;
        var interval = 2.0f;
        var targetScale = new Vector3(1.5f, 1.5f, 1.5f);
        var rect = establishRoleImage.GetComponent<RectTransform>();

        var roleName = establishRole_FlushList[establishRoleIndex];
        establishRoleImage.sprite = roleNameImage_Dic[roleName];

        establishRoleImage.gameObject.SetActive(true);

        while (time < interval) {
            time += Time.deltaTime;
            var lerp = Mathf.Lerp(0f, 1f, time / interval);
            rect.localScale = Vector3.Lerp(rect.localScale, targetScale, lerp);
            establishRoleImage.color = Color.Lerp(establishRoleImage.color, Color.white, lerp);
            yield return 0;
        }

        time = 0.0f;
        interval = 0.5f;
        while (time < interval) {
            time += Time.deltaTime;
            var lerp = Mathf.Lerp(0f, 1f, time / interval);
            establishRoleImage.color = Color.Lerp(establishRoleImage.color, Color.clear, lerp);
            yield return 0;
        }

        establishRoleIndex++;
        gameStartImage.gameObject.SetActive(false);
        rect.localScale = Vector3.zero;

        yield return new WaitForSeconds(0.2f);

        if (establishRoleIndex == establishRole_FlushList.Count) {
            EstablishRole_EndAnimation();
        }else {
            StartCoroutine(PlayEstablishRoleAnimation());
        }
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

                scoreManager.turnPlayerScore = player.GetTotalScore();

                koikoiDialog.SetScoreText();
                //koikoiDialog.gameObject.SetActive(true);
                StartCoroutine(KoiKoiDialogView(0.25f));
                break;

            case TURNPLAYER.COM:
                //CPUのターンだったら
                //手札が1枚よりも多い場合、必ずこいこいする
                scoreManager.turnPlayerScore = cpu.GetTotalScore();

                if (cpu.GetHand().Count > 1) {
                    koikoiDialog.OnKoiKoiButton();
                }else {
                    koikoiDialog.OnAgariButton();
                }
                //state = FieldManager.STATE.TURNCHANGE;
                break;
        }
    }

    private IEnumerator KoiKoiDialogView(float interval) {
        var time = 0.0f;
        var rect = koikoiDialog.GetComponent<RectTransform>();

        while (time < interval) {
            time += Time.deltaTime;
            var lerp = Mathf.Lerp(0f, 1f,time/interval);

            rect.localScale = Vector3.Lerp(rect.localScale, Vector3.one, lerp);
            yield return 0;
        }

        rect.localScale = Vector3.one;
    }

    public Player GetPlayer() {
        return player;
    }
    public Cpu GetCPU() {
        return cpu;
    }    
}
