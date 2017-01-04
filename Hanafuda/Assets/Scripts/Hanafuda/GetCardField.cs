using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetCardField : MonoBehaviour {

    [SerializeField]
    private FieldManager fieldManager;

    [SerializeField]
    private GameObject[] getCardFieldObjectList;
    private Dictionary<int, GameObject> getCardFieldObject_Dic = new Dictionary<int, GameObject>();

    public Dictionary<int, List<GameObject>> getCard_Dic = new Dictionary<int, List<GameObject>>();
    private Dictionary<int, Dictionary<int, string[]>> getRoleString_Dic = new Dictionary<int, Dictionary<int, string[]>>();
    public Dictionary<int, Vector3> getCardPosition = new Dictionary<int, Vector3>();

    private const float adjustY = 1.25f;

    private Vector3 getCardFirstPosition = new Vector3(-8f, 2.5f, 0.0f);
    [HideInInspector]
    public Vector3 cardScale = new Vector3(0.5f, 0.5f, 1.0f);

    private const int FIELD_COUNT = 8;

    [HideInInspector]
    public List<int> checkRoleIndexList = new List<int>();

    public List<GameObject> addGetCard_Dic = new List<GameObject>();

    // Use this for initialization
    void Start () {

        //場のイメージ
        // 0 4
        // 1 5
        // 2 6
        // 3 7
        for (int i = 0; i < FIELD_COUNT; i++) {
            getCard_Dic.Add(i, new List<GameObject>());

            for (int j=0;j<4; j++) {
                var stringSwitch_Dic = new Dictionary<int, string[]>();
                stringSwitch_Dic.Add(j,new string[]{"",""});
                getRoleString_Dic.Add(i, stringSwitch_Dic);
            }

            var typeObject = getCardFieldObjectList[i / 4].transform.GetChild(i % 4).gameObject;
            getCardFieldObject_Dic.Add(i, typeObject);

            var pos = getCardFirstPosition;
            if (i > 3) pos.x = getCardFirstPosition.x * -1;

            pos.y -= (i % 4) * adjustY;
            getCardPosition.Add(i, pos);
        }
    }
	
	// Update is called once per frame
	void Update () {
        switch (fieldManager.state) {
            case FieldManager.STATE.CHECK_ROLE: CheckRole(); break;
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
                case Card.TYPE.HIKARI: index = 4; break;
                case Card.TYPE.BAKE:
                case Card.TYPE.TANE: index = 5; break;
                case Card.TYPE.TANZAKU: index = 6; break;
                case Card.TYPE.KASU: index = 7; break;
            }

            break;

        case FieldManager.TURNPLAYER.COM://CPUのターン
            //カードの種類に対応した場所に移動する
            switch (card.type) {
                case Card.TYPE.HIKARI: index = 0; break;
                case Card.TYPE.BAKE:
                case Card.TYPE.TANE: index = 1; break;
                case Card.TYPE.TANZAKU: index = 2; break;
                case Card.TYPE.KASU: index = 3; break;
            }

            break;
        }
        return index;
    }

    /// <summary>
    /// 取得したカードで役ができているかチェック
    /// </summary>
    private void CheckRole() {
        
        //追加された種類の場所で役ができたかどうかのチェック！
        foreach (var getCard in fieldManager.getCardList) {
            var role_Dic = new Dictionary<int, string[]>();
            var g_Card = getCard.GetComponent<Card>();
            var index = GetCardTypeIndex(g_Card);

            switch (index) {
                case 0:
                case 4:
                    role_Dic = GetRoleHikari(index, g_Card);
                    break;
                case 1:
                case 5:
                    role_Dic = GetRoleTane(index, g_Card);
                    break;
                case 2:
                case 6:
                    role_Dic = GetRoleTanzaku(index, g_Card);
                    break;
                case 3:
                case 7:
                    role_Dic = GetRoleKasu(index, g_Card);
                    break;

            }

            getCard_Dic[index].Add(getCard);

            //取得したカードの種類で役が揃った場合、isRoleをONにする            
            if (role_Dic.Count > 0) {

            }

        }

    }

    /// <summary>
    /// 光のカードが増えた時に、成立した役を取得
    /// </summary>
    /// <param name="getCardList"></param>
    /// <returns></returns>
    private Dictionary<int, string[]> GetRoleHikari(int index,Card card) {

        //三光 5文
        //雨四光 7文
        //四光 8文
        //五光 15文

        int searchTextIndex = 0;
        var stringSwitch_Dic = new Dictionary<int, string[]>();

        //今回追加されたカードが月であるかどうかを取得
        var isTuki = (card.division == Card.DIVISION.TSUKI) ? true : false;
        //今回追加されたカードが幕であるかどうかを取得
        var isMaku = (card.division == Card.DIVISION.MAKU) ? true : false;

        if (isTuki || isMaku) {

            //種のカードの中に盃があるかどうかを取得
            var isSakazuki = GetIsTargetDivision(getCard_Dic[index + 1], Card.DIVISION.SAKAZUKI);

            //月見酒、花見酒の表示設定
            if (isSakazuki) {
                //盃がある場合、月見酒・花見酒が登録されているはずなので表示状態をRemainingからFlushにする
                //盃が種のカードリストにある＝searchTextIndexが-1になることはない（見つからないことはない）

                //月見酒・花見酒が登録されているところまで探す
                if (isTuki) searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index + 1], Role.RoleManager.TUKIMIZAKE);
                if (isMaku) searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index + 1], Role.RoleManager.HANAMIZAKE);
                getRoleString_Dic[index + 1][searchTextIndex][1] = Role.RoleManager.FLUSH;

            } else {

                //何も役が表示されていないところに新しく登録する
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index+1], "");
                if (isTuki) getRoleString_Dic[index + 1][searchTextIndex][0] = Role.RoleManager.TUKIMIZAKE;
                if (isMaku) getRoleString_Dic[index + 1][searchTextIndex][0] = Role.RoleManager.HANAMIZAKE;
                getRoleString_Dic[index + 1][searchTextIndex][1] = Role.RoleManager.REMAINING;

            }
        }

        //今回追加されたカードが小野道風であるかどうかを取得
        var isOno = (card.division == Card.DIVISION.ONO) ? true : false;

        //カードリストの中に小野道風が含まれているかどうかを取得
        var isOno_GetCardList = GetIsTargetDivision(getCard_Dic[index], Card.DIVISION.ONO);

        switch (getCard_Dic[index].Count) {
            case 1:
                if (!isOno && !isOno_GetCardList) {
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
                    getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.SANKOU;
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;
                }
                break;
            case 2:
                if (!isOno && !isOno_GetCardList) {
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.SANKOU);
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

                }else {
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
                    getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.AMEYONKOU;
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;
                }
                break;
            case 3:
                if (!isOno && !isOno_GetCardList) {
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.YONKOU);
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;
                }else {
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.AMEYONKOU);
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;
                }
                break;
            case 4:
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.GOKOU);
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;
                break;
        }
            return stringSwitch_Dic;
    }

    /// <summary>
    /// 指定したカードリストの中に指定したカードがあるかどうかを取得
    /// </summary>
    /// <param name="targetDivision"></param>
    /// <returns></returns>
    private bool GetIsTargetDivision(List<GameObject> getCardList,Card.DIVISION targetDivision) {

        foreach (var card in getCardList) {
            var g_Card = card.GetComponent<Card>();
            if (g_Card.division == targetDivision) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 指定したカードリストから指定したテキストを探し、そのインデックスを取得
    /// </summary>
    /// <returns></returns>
    private int GetCardListIndex_TargetText(Dictionary<int,string[]> getRoleString, string text) {

        for (int i = 0; i < getRoleString.Count; i++) {
            if (getRoleString[i][0] == text) {
                return i;
            }
        }
        
        return -1;
    }

    /// <summary>
    /// 種のカードが増えた時に、成立した役を取得
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private Dictionary<int, string[]> GetRoleTane(int index,Card card) {

        //猪鹿蝶 5文+a
        //花見酒 3文+a
        //月見酒 3文+a
        //タネ 1文+a

        int searchTextIndex;
        var stringSwitch_Dic = new Dictionary<int, string[]>();

        //今回追加されたカードが盃であるかどうかを取得
        var isSakazuki = (card.division == Card.DIVISION.SAKAZUKI) ? true : false;

        if (isSakazuki) {

            //月見酒の表示設定
            var isTuki = GetIsTargetDivision(getCard_Dic[index - 1], Card.DIVISION.TSUKI);
            if (isTuki) {
                //月見酒が登録されているところまで探す
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.TUKIMIZAKE);
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

            } else {

                //何も役が表示されていないところに新しく登録する
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
                getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.TUKIMIZAKE;
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;
            }

            //花見酒の表示設定
            var isMaku = GetIsTargetDivision(getCard_Dic[index - 1], Card.DIVISION.MAKU);
            if (isMaku) {
                //花見酒が登録されているところまで探す
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.HANAMIZAKE);
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

            } else {

                //何も役が表示されていないところに新しく登録する
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
                getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.HANAMIZAKE;
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;
            }

            //カスのカードの枚数によってRemaining Flushを設定
            if (getCard_Dic[index + 2].Count == 8) {
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index + 2], "");
                getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.KASU;
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;

            } else if (getCard_Dic[index + 2].Count > 8) {
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index + 2], Role.RoleManager.KASU);
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;
            }
        }

        //今回追加されたカードが猪・鹿・蝶のいずれかのカードかどうかを取得
        var isInosisi = (card.division == Card.DIVISION.INOSISI) ? true : false;
        var isShika = (card.division == Card.DIVISION.SHIKA) ? true : false;
        var isTyou = (card.division == Card.DIVISION.TYOU) ? true : false;

        //今回のカードが猪・鹿・蝶のいずれかのカードであった場合、
        //取得している種のカードの中から猪鹿蝶の枚数によってRemaining Flushの設定
        if (isInosisi || isShika || isTyou) {

            //var isInoShikaTyouCount = GetIsInoShikaTyou(getCard_Dic[index]);
            var isInoShikaTyouCount = 0;
            foreach (var getCard in getCard_Dic[index]) {
                var g_Card = getCard.GetComponent<Card>();
                if (g_Card.division == Card.DIVISION.INOSISI ||
                    g_Card.division == Card.DIVISION.SHIKA ||
                    g_Card.division == Card.DIVISION.TYOU) {
                    isInoShikaTyouCount++;
                }
            }

            switch (isInoShikaTyouCount) {
                case 1://猪鹿蝶の枚数が1枚の場合Remainingに設定
                    //何も役が表示されていないところに新しく登録する
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
                    getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.INOSHIKATYOU;
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;
                    break;
                case 2://猪鹿蝶の枚数2枚の場合Flushに設定
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.INOSHIKATYOU);
                    getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.INOSHIKATYOU;
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;
                    break;
            }

        }

        //種のカードの枚数によってRemaining Flushの設定
        if (getCard_Dic[index].Count == 3) {
            searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
            getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.TANE;
            getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;

        } else if (getCard_Dic[index].Count > 3) {
            searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.TANE);
            getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;
        }

            return stringSwitch_Dic;
    }  

    /// <summary>
    /// 短冊のカードが増えた時に、成立した役を取得
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private Dictionary<int, string[]> GetRoleTanzaku(int index,Card card) {

        //赤青短 10文 +a
        //赤短 5文+a
        //青短 5文+a

        //短冊 1文+a

        int searchTextIndex;
        var stringSwitch_Dic = new Dictionary<int, string[]>();

        //今回追加されたカードが赤短冊であるかどうかを取得
        var isAkatanzaku = (card.division == Card.DIVISION.AKATANZAKU) ? true : false;

        //赤短冊のカードの場合、取得している赤短冊の枚数によってRemaining Flushを設定
        if (isAkatanzaku) {
            var isAkatanzakuCount = 0;
            foreach (var getCard in getCard_Dic[index]) {
                var g_Card = getCard.GetComponent<Card>();
                if (g_Card.division == Card.DIVISION.AKATANZAKU) isAkatanzakuCount++;
            }

            switch (isAkatanzakuCount) {
                case 1:
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
                    getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.AKATANZAKU;
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;
                    break;
                case 2:
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.AKATANZAKU);
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;
                    break;
            }
        }

        //今回追加されたカードが青短冊であるかどうかを取得
        var isAotanzaku = (card.division == Card.DIVISION.AOTANZAKU) ? true : false;

        //青短冊のカードの場合、取得している青短冊の枚数によってRemaining Flushを設定
        if (isAotanzaku) {
            var isAotanzakuCount = 0;
            foreach (var getCard in getCard_Dic[index]) {
                var g_Card = getCard.GetComponent<Card>();
                if (g_Card.division == Card.DIVISION.AOTANZAKU) isAotanzakuCount++;
            }

            switch (isAotanzakuCount) {
                case 1:
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
                    getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.AOTANZAKU;
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;
                    break;
                case 2:
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.AOTANZAKU);
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;
                    break;
            }
        }

        //短冊のカードの枚数によってRemaining Flushの設定
        if (getCard_Dic[index].Count == 4) {
            searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
            getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.TANZAKU;
            getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;

        } else if (getCard_Dic[index].Count > 4) {
            searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.TANZAKU);
            getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;
        }

        return stringSwitch_Dic;
    }

    /// <summary>
    /// カスのカードが増えた時に、成立した役を取得
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private Dictionary<int, string[]> GetRoleKasu(int index, Card card) {

        //カス 1文+a

        int searchTextIndex;
        var stringSwitch_Dic = new Dictionary<int, string[]>();

        int isSakazukiCount = 0;
        var isSakazuki = GetIsTargetDivision(getCard_Dic[index - 2], Card.DIVISION.SAKAZUKI);
        if (isSakazuki) isSakazukiCount = 1;

        if (getCard_Dic[index].Count + isSakazukiCount == 8) {
            searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
            getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.KASU;
            getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;
        } else if (getCard_Dic[index].Count + isSakazukiCount > 8) {
            searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.KASU);
            getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;
        }

        return stringSwitch_Dic;
    }



}
