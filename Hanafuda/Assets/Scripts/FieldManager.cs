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

    public float handCardSpeed;
    //プレイヤーに配る時にどこまで移動したら表向きになるかの割合
    public float handCardCenterDistanePer;

    //private const float adjustX= 1.55f;
    //private const float adjustY = 1.6f;
    //private Vector3 fieldFirstPosition = new Vector3(-2.325f, 1.55f, 0.0f);
    //private Vector3 cardScale = new Vector3(1.0f, 1.0f, 1.0f);


    public bool isCardHandOut;
    private int handoutCounter;
    [SerializeField,Header("配る速度")]
    private float HANDCARD_MAXTIMER;
    private float handCardTimer;
    [SerializeField, Header("お互いの手札やフィールドにカードを配る間隔")]
    private float HANDCARD_MAXINTERVAL;
    private float handCardInterval;


    //private const int FIELD_COUNT = 12;

    //private List<GameObject> fieldCard = new List<GameObject>();
    //private Dictionary<int, Vector3> fieldPosition;
    //private Dictionary<int,bool> fieldCard_isPut;

    // Use this for initialization
    void Start () {
        handoutCounter = 0;
        isCardHandOut = true;

        //fieldCard_isPut = new Dictionary<int, bool>();
        //fieldPosition = new Dictionary<int, Vector3>();

        handCardTimer = HANDCARD_MAXTIMER;

        ////場のイメージ
        ////  8  0  1  2  3  9
        //// 10  4  5  6  7 11
        //for (int i=0;i< FIELD_COUNT; i++) {
        //    fieldCard_isPut.Add(i, true);

        //    var pos = fieldFirstPosition;
        //    if (i<8) {
        //        pos.x += (i % 4) * adjustX;
        //        pos.y += (i / 4) * adjustY;
        //        fieldPosition.Add(i, pos);

        //    }else {
        //        switch (i) {
        //            case 8:
        //                pos.x -= adjustX;
        //                break;
        //            case 9:
        //                pos.x += 4 * adjustX;
        //                break;
        //            case 10:
        //                pos.x -= adjustX;
        //                pos.y += adjustY;
        //                break;
        //            case 11:
        //                pos.x += 4 * adjustX;
        //                pos.y += adjustY;
        //                break;
        //        }
        //    }
        //}
    }

    // Update is called once per frame
    void Update () {
        switch (gameManager.state) {

            case GameManager.STATE.CARD_HAND_OUT:

                if (isCardHandOut) { 
                    handCardInterval -= Time.deltaTime;
                    if (handCardInterval < 0.0f) {

                        handCardTimer -= Time.deltaTime;
                        if(handCardTimer < 0.0f) {
                            
                            CardHandOut();
                            handoutCounter++;

                            handCardTimer = HANDCARD_MAXTIMER;

                            if (handoutCounter % 3 == 0) {
                                handCardInterval = HANDCARD_MAXINTERVAL;
                            }

                            if (handoutCounter == 24) {
                                isCardHandOut = false;
                            }
                        }
                    }
                }
                break;
            case GameManager.STATE.GAME:
                break;

        }
    }
    public void DeckShufle() {
        //山札をランダムに並び替える（100回シャッフル）
        for (int i = 0; i < 100; i++) {
            var change1 = Random.Range(0, deck.transform.childCount);
            var change2 = Random.Range(0, deck.transform.childCount);
            deck.transform.GetChild(change1).SetSiblingIndex(change2);
        }
    }

    private IEnumerator WaitCardHandOut(float waitTimer) {
        yield return new WaitForSeconds(waitTimer);
        CardHandOut();


        //if (handoutCounter % 4 == 0) {
        //    yield return new WaitForSeconds(0.01f);
        //}
        if (handoutCounter == 24) {
            gameManager.StartCoroutine("WaitNextState", GameManager.STATE.GAME);
        }
    }

    /// <summary>
    /// カードを配る処理
    /// </summary>
    public void CardHandOut() {

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

}
