using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Field : MonoBehaviour {

    [SerializeField]
    private FieldManager fieldManager;
    [SerializeField]
    private CardManager cardManager;

    private Dictionary<GameObject, float> handCardTargetMaxDistance_Dic = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Vector3> handCardTargePosition_Dic = new Dictionary<GameObject, Vector3>();
    public List<GameObject> fieldCard = new List<GameObject>();

    private Dictionary<int, bool> fieldCard_isPut = new Dictionary<int, bool>();
    private Dictionary<int, Vector3> fieldPosition = new Dictionary<int, Vector3>();

    private const float adjustX = 1.55f;
    private const float adjustY = 2.05f;
    private Vector3 fieldFirstPosition = new Vector3(-2.325f, 1.55f, 0.0f);
    [HideInInspector]
    public Vector3 cardScale = new Vector3(1.0f, 1.0f, 1.0f);

    private const int FIELD_COUNT = 12;

    // Use this for initialization
    void Start () {
        //場のイメージ
        //  8  0  1  2  3  9
        // 10  4  5  6  7 11
        for (int i = 0; i < FIELD_COUNT; i++) {
            fieldCard_isPut.Add(i, true);

            var pos = fieldFirstPosition;
            if (i < 8) {
                pos.x += (i % 4) * adjustX;
                pos.y -= (i / 4) * adjustY;

            } else {

                switch (i) {
                    case 8:
                        pos.x -= adjustX;
                        break;
                    case 9:
                        pos.x += 4 * adjustX;
                        break;
                    case 10:
                        pos.x -= adjustX;
                        pos.y -= adjustY;
                        break;
                    case 11:
                        pos.x += 4 * adjustX;
                        pos.y -= adjustY;
                        break;
                }
            }
            fieldPosition.Add(i, pos);
        }
    }
	
	// Update is called once per frame
	void Update () {
        switch (fieldManager.state) {
            case FieldManager.STATE.CARD_HAND_OUT: CardHandOut(); break;
        }
    }

    /// <summary>
    /// 配られているときの処理
    /// </summary>
    private void CardHandOut() {

        for (int i = 0; i < fieldCard.Count; i++) {

            fieldCard[i].transform.position = Vector3.MoveTowards(fieldCard[i].transform.position,
                                                                  handCardTargePosition_Dic[fieldCard[i]],
                                                                  Time.deltaTime * fieldManager.handCardSpeed);

            //目標までの距離に近づくにつれてだんだん大きく
            var distance = Vector3.Distance(fieldCard[i].transform.position, handCardTargePosition_Dic[fieldCard[i]]);
            var distanceLeap = Mathf.Lerp(1f, 0f, distance / handCardTargetMaxDistance_Dic[fieldCard[i]]);
            fieldCard[i].transform.localScale = cardScale * distanceLeap;

            //一定距離まで移動したら、回転させる
            if (distanceLeap >= fieldManager.handCardCenterDistanePer) {
                //if (posLeap > fieldManager.handCardMoveDistaneRotPer) {
                var spriteRenderer = fieldCard[i].GetComponent<SpriteRenderer>();
                var card = fieldCard[i].GetComponent<Card>();

                fieldCard[i].transform.rotation = Quaternion.RotateTowards(fieldCard[i].transform.rotation,
                                                                           Quaternion.identity,
                                                                           Time.deltaTime * 250);

                //本当は中間まで回転したらの割合でやりたい
                if (fieldCard[i].transform.rotation.eulerAngles.y >= 270.0f) {
                    spriteRenderer.sprite = card.image[0];
                }
            }
        }

    }

    /// <summary>
    /// ゲーム開始時の場に追加される処理
    /// </summary>
    public void AddCard(GameObject card) {

        var pos = fieldPosition[fieldCard.Count];
        var maxDistance = Vector3.Distance(card.transform.position, pos);

        fieldCard.Add(card);
        handCardTargetMaxDistance_Dic.Add(card, maxDistance);
        handCardTargePosition_Dic.Add(card, pos);
        card.transform.parent = transform;

        //タグと描画順を変更
        cardManager.SetCardTag(card, TAG.TagManager.FIELD_CARD);
    }

    /// <summary>
    /// ゲーム開始時の手札が指定されている座標まで全て移動しきっているかを取得
    /// </summary>
    public bool GetIsHandOutMovement() {

        foreach (var card in fieldCard) {
            if (card.transform.position != handCardTargePosition_Dic[card]) {
                return false;
            }
        }
        return true;

    }
}
