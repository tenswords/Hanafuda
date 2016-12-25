using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetCardField : MonoBehaviour {

    [SerializeField]
    private FieldManager fieldManager;

    public Dictionary<int, List<GameObject>> getCard_Dic = new Dictionary<int, List<GameObject>>();
    public Dictionary<int, Vector3> getCardPosition = new Dictionary<int, Vector3>();

    private const float adjustY = 1.25f;

    private Vector3 getCardFirstPosition = new Vector3(-8f, 2.5f, 0.0f);
    [HideInInspector]
    public Vector3 cardScale = new Vector3(0.5f, 0.5f, 1.0f);

    private const int FIELD_COUNT = 8;

    // Use this for initialization
    void Start () {
        //場のイメージ
        // 0 4
        // 1 5
        // 2 6
        // 3 7
        for (int i = 0; i < FIELD_COUNT; i++) {

            getCard_Dic.Add(i, null);

            var pos = getCardFirstPosition;

            if (i > 3) {
                pos.x = getCardFirstPosition.x * -1;
            }

            pos.y -= (i % 4) * adjustY;

            getCardPosition.Add(i, pos);
        }
    }
	
	// Update is called once per frame
	void Update () {
        switch (fieldManager.state) {
            case FieldManager.STATE.CHECK_ROLE:
                break;
        }
    }

    /// <summary>
    /// 指定されたカードの種類に対応した番号を取得
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public int GetCardTypeIndex(Card card) {
        var index = 0;

        switch (fieldManager.turnPlayer) {
        case FieldManager.TURNPLAYER.PLAYER://プレイヤーのターン

            //カードの種類に対応した場所に移動する
            switch (card.type) {
                case Card.TYPE.HIKARI:
                    index = 4;
                    break;

                case Card.TYPE.BAKE:
                case Card.TYPE.TANE:
                    index = 5;
                    break;

                case Card.TYPE.TANZAKU:
                    index = 6;
                    break;

                case Card.TYPE.KASU:
                    index = 7;
                    break;
            }

            break;

        case FieldManager.TURNPLAYER.COM://CPUのターン
            break;
        }
        return index;
    }

    /// <summary>
    /// 取得したカードで役ができているかチェック
    /// </summary>
    private void CheckRole() {

    }
}
