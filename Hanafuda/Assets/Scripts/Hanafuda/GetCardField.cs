using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

    private Vector3 getCardFirstPosition = new Vector3(-8f, 2.4f, 0.0f);
    [HideInInspector]
    public Vector3 cardScale = new Vector3(0.5f, 0.5f, 1.0f);

    private const int FIELD_COUNT = 8;

    [SerializeField]
    private Color ROLE_REMAINING_COLOR;

    [SerializeField]
    private Color ROLE_FLUSH_COLOR;

    //成立したか、成立しそうかのリスト
    public List<string[]> establishRoleSList = new List<string[]>();

    //private int establishRoleIndex;
    //private List<int> establishRole_FlushList = new List<int>();

    //[SerializeField]
    //private Image establishRoleImage;

    //[SerializeField]
    //private Sprite[] roleNameImageList;
    //public Dictionary<string, Sprite> roleNameImage_Dic = new Dictionary<string, Sprite>();

    // Use this for initialization
    void Start () {

        //場のイメージ
        // 0 4
        // 1 5
        // 2 6
        // 3 7
        for (int i = 0; i < FIELD_COUNT; i++) {
            getCard_Dic.Add(i, new List<GameObject>());

            var stringSwitch_Dic = new Dictionary<int, string[]>();
            for (int j=0;j<4; j++) {
                stringSwitch_Dic.Add(j,new string[]{"",""});
            }
            getRoleString_Dic.Add(i, stringSwitch_Dic);

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
            //case FieldManager.STATE.ESTABLISHROLE: EstablishRole(); break;
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
                case Card.TYPE.HIKARI: index = 0; break;
                case Card.TYPE.BAKE:
                case Card.TYPE.TANE: index = 1; break;
                case Card.TYPE.TANZAKU: index = 2; break;
                case Card.TYPE.KASU: index = 3; break;
            }

            break;

        case FieldManager.TURNPLAYER.COM://CPUのターン
            //カードの種類に対応した場所に移動する
            switch (card.type) {
                case Card.TYPE.HIKARI: index = 4; break;
                case Card.TYPE.BAKE:
                case Card.TYPE.TANE: index = 5; break;
                case Card.TYPE.TANZAKU: index = 6; break;
                case Card.TYPE.KASU: index = 7; break;
            }

            break;
        }
        return index;
    }

    /// <summary>
    /// 取得したカードで役ができているかチェック
    /// </summary>
    private void CheckRole() {

        establishRoleSList.Clear();
        //fieldManager.establishRoleIndex = 0;
        fieldManager.establishRole_FlushList.Clear();

        //追加された種類の場所で役ができたかどうかのチェック！
        foreach (var getCard in fieldManager.getCardList) {
            var g_Card = getCard.GetComponent<Card>();
            var index = GetCardTypeIndex(g_Card);

            switch (index) {
                case 0:
                case 4:
                    CheckRoleHikari(index, g_Card);
                    break;
                case 1:
                case 5:
                    CheckRoleTane(index, g_Card);
                    break;
                case 2:
                case 6:
                    CheckRoleTanzaku(index, g_Card);
                    break;
                case 3:
                case 7:
                    CheckRoleKasu(index, g_Card);
                    break;

            }

            getCard_Dic[index].Add(getCard);

            var targetTypeCount = getCardFieldObject_Dic[index].transform.GetChild(4).GetComponent<Text>();
            var changedIndex = targetTypeCount.text.IndexOf("×")+1;
            var changeString = targetTypeCount.text.Substring(changedIndex, (targetTypeCount.text.Length - changedIndex));

            targetTypeCount.text = targetTypeCount.text.Replace(changeString, getCard_Dic[index].Count.ToString());
        }

        //役の成立状態が Remaining か Flush になった場合、被っている状態を上書きする処理
        if (establishRoleSList.Count > 0) {

            var removeIndexList = new List<int>();
            for (int i = 0; i < establishRoleSList.Count - 1; i++) {

                //空になっていたら検索対象外
                if (establishRoleSList[i][0] == "") continue;

                for (int j = i + 1; j < establishRoleSList.Count; j++) {
                    //空になっていたら検索対象外
                    if (establishRoleSList[j][0] == "") continue;

                    if (establishRoleSList[i][0] == establishRoleSList[j][0] &&
                        establishRoleSList[i][1] == establishRoleSList[j][1] &&
                        establishRoleSList[i][2] == establishRoleSList[j][2]) {

                        if(establishRoleSList[i][3] != establishRoleSList[j][3]) { 
                            //被っている役の成立状態を上書き
                            establishRoleSList[i][3] = establishRoleSList[j][3];
                        }

                        for (int k = 0; k < 4; k++) establishRoleSList[j][k] = "";
                        removeIndexList.Add(j);
                    }
                }
            }

            //役の成立リストから不要となったものを削除
            if (removeIndexList.Count > 0) {
                for (int i = removeIndexList.Count - 1; i >= 0; i--) {
                    establishRoleSList.RemoveAt(removeIndexList[i]);
                }
            }

            //成立した・成立しそうな役を表示
            for (int i = 0; i < establishRoleSList.Count; i++) {
                var typeIndex = int.Parse(establishRoleSList[i][0]);
                var typeSearchTextIndex = int.Parse(establishRoleSList[i][1]);
                var roleName = establishRoleSList[i][2];
                var state = establishRoleSList[i][3];

                Debug.Log("typeIndex " + typeIndex);
                Debug.Log("typeSearchTextIndex " + typeSearchTextIndex);
                Debug.Log("roleName " + roleName);
                Debug.Log("state " + state);

                var targetTypeObject = getCardFieldObject_Dic[typeIndex].transform.GetChild(typeSearchTextIndex).gameObject;
                var roleImage = targetTypeObject.GetComponent<Image>();
                var roleText = targetTypeObject.transform.GetChild(0).GetComponent<Text>();

                roleText.text = roleName;
                if (state == Role.RoleManager.REMAINING) {
                    roleImage.color = ROLE_REMAINING_COLOR;

                } else if (state == Role.RoleManager.FLUSH) {
                    roleImage.color = ROLE_FLUSH_COLOR;
                    fieldManager.establishRole_FlushList.Add(roleName);

                    fieldManager.TurnPlayerAddScore(roleName);
                }

                targetTypeObject.SetActive(true);
            }
        }


        //どの役もFlush状態になっていない場合、ターンをチェンジする
        if (fieldManager.establishRole_FlushList.Count == 0) {
            fieldManager.state = FieldManager.STATE.TURNCHANGE;

        } else if (fieldManager.establishRole_FlushList.Count > 0) {
            //何かの役がFlush状態になった場合、その役のエフェクトを表示
            fieldManager.state = FieldManager.STATE.ESTABLISHROLE;
        }
    }

    /// <summary>
    /// 役の成立情報の登録
    /// </summary>
    private void AddEstablishRoleSList(int index,int searchTextIndex,string roleName,string state) {
        var establishRole = new string[4];

        establishRole[0] = index.ToString();
        establishRole[1] = searchTextIndex.ToString();
        establishRole[2] = roleName;
        establishRole[3] = state;
        establishRoleSList.Add(establishRole);

        Debug.Log("establishRole " + establishRole[0]+establishRole[1]+establishRole[2]+establishRole[3]);

    }

    /// <summary>
    /// 光のカードが増えた時に、成立した役を取得
    /// </summary>
    /// <param name="getCardList"></param>
    /// <returns></returns>
    private void CheckRoleHikari(int index,Card card) {

        //三光 5文
        //雨四光 7文
        //四光 8文
        //五光 15文

        int searchTextIndex = 0;


        //今回追加されたカードが月であるかどうかを取得
        var isTuki = (card.division == Card.DIVISION.TSUKI) ? true : false;
        //今回追加されたカードが幕であるかどうかを取得
        var isMaku = (card.division == Card.DIVISION.MAKU) ? true : false;

        if (isTuki || isMaku) {

            //種のカードの中に盃があるかどうかを取得
            var isSakazuki = GetIsTargetDivision(getCard_Dic[index + 1], Card.DIVISION.SAKAZUKI);

            var roleName = "";
            if (isTuki) roleName = Role.RoleManager.TUKIMIZAKE;
            if (isMaku) roleName = Role.RoleManager.HANAMIZAKE;

            //月見酒、花見酒の表示設定
            if (isSakazuki) {
                //盃がある場合、月見酒・花見酒が登録されているはずなので表示状態をRemainingからFlushにする
                //盃が種のカードリストにある＝searchTextIndexが-1になることはない（見つからないことはない）

                //月見酒・花見酒が登録されているところまで探す
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index + 1], roleName);
                getRoleString_Dic[index + 1][searchTextIndex][1] = Role.RoleManager.FLUSH;

                //成立情報の登録
                AddEstablishRoleSList((index + 1), searchTextIndex, roleName, Role.RoleManager.FLUSH);

            } else {

                //何も役が表示されていないところに新しく登録する
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index+1], "");
                getRoleString_Dic[index + 1][searchTextIndex][0] = roleName;
                getRoleString_Dic[index + 1][searchTextIndex][1] = Role.RoleManager.REMAINING;

                //成立情報の登録
                AddEstablishRoleSList((index + 1), searchTextIndex, roleName, Role.RoleManager.REMAINING);
            }
        }

        //今回追加されたカードが小野道風であるかどうかを取得
        var isOno = (card.division == Card.DIVISION.ONO) ? true : false;

        //カードリストの中に小野道風が含まれているかどうかを取得
        var isOno_GetCardList = GetIsTargetDivision(getCard_Dic[index], Card.DIVISION.ONO);

        Debug.Log("getCard_Dic[index].Count " + getCard_Dic[index].Count);
        if (getCard_Dic[index].Count != 0) Debug.Log("getCard_Dic[index][0].name " + getCard_Dic[index][0].name);

        switch (getCard_Dic[index].Count) {
            case 1:
                if (!isOno && !isOno_GetCardList) {
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
                    getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.SANKOU;
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;

                    //成立情報の登録
                    AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.SANKOU, Role.RoleManager.REMAINING);

                }
                break;
            case 2:
                if (!isOno && !isOno_GetCardList) {
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.SANKOU);
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

                    //成立情報の登録
                    AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.SANKOU, Role.RoleManager.FLUSH);

                } else {
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
                    getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.AMEYONKOU;
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;

                    //成立情報の登録
                    AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.AMEYONKOU, Role.RoleManager.REMAINING);

                }
                break;
            case 3:
                var roleName = "";
                if (!isOno && !isOno_GetCardList) roleName = Role.RoleManager.YONKOU;
                else roleName = Role.RoleManager.AMEYONKOU;

                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], roleName);
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

                //成立情報の登録
                AddEstablishRoleSList(index, searchTextIndex, roleName, Role.RoleManager.FLUSH);
                break;
            case 4:
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.GOKOU);
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

                //成立情報の登録
                AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.GOKOU, Role.RoleManager.FLUSH);
                break;
        }
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
    private void CheckRoleTane(int index,Card card) {

        //猪鹿蝶 5文+a
        //花見酒 3文+a
        //月見酒 3文+a
        //タネ 1文+a

        int searchTextIndex;

        //今回追加されたカードが盃であるかどうかを取得
        var isSakazuki = (card.division == Card.DIVISION.SAKAZUKI) ? true : false;

        if (isSakazuki) {

            //月見酒の表示設定
            var isTuki = GetIsTargetDivision(getCard_Dic[index - 1], Card.DIVISION.TSUKI);
            if (isTuki) {
                //月見酒が登録されているところまで探す
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.TUKIMIZAKE);
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

                //成立情報の登録
                AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.TUKIMIZAKE, Role.RoleManager.FLUSH);

            } else {

                //何も役が表示されていないところに新しく登録する
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
                getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.TUKIMIZAKE;
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;

                //成立情報の登録
                AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.TUKIMIZAKE, Role.RoleManager.REMAINING);
            }

            //花見酒の表示設定
            var isMaku = GetIsTargetDivision(getCard_Dic[index - 1], Card.DIVISION.MAKU);
            if (isMaku) {
                //花見酒が登録されているところまで探す
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.HANAMIZAKE);
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

                //成立情報の登録
                AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.HANAMIZAKE, Role.RoleManager.FLUSH);

            } else {

                //何も役が表示されていないところに新しく登録する
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
                getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.HANAMIZAKE;
                getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;
                
                //成立情報の登録
                AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.HANAMIZAKE, Role.RoleManager.REMAINING);
            }

            //カスのカードの枚数によってRemaining Flushを設定
            if (getCard_Dic[index + 2].Count == 8) {
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index + 2], "");
                getRoleString_Dic[index + 2][searchTextIndex][0] = Role.RoleManager.KASU;
                getRoleString_Dic[index + 2][searchTextIndex][1] = Role.RoleManager.REMAINING;

                //成立情報の登録
                AddEstablishRoleSList((index + 2), searchTextIndex, Role.RoleManager.KASU, Role.RoleManager.REMAINING);

            } else if (getCard_Dic[index + 2].Count > 8) {
                searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index + 2], Role.RoleManager.KASU);
                getRoleString_Dic[index + 2][searchTextIndex][1] = Role.RoleManager.FLUSH;

                //成立情報の登録
                AddEstablishRoleSList((index + 2), searchTextIndex, Role.RoleManager.KASU, Role.RoleManager.FLUSH);
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

                    //成立情報の登録
                    AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.INOSHIKATYOU, Role.RoleManager.REMAINING);

                    break;
                case 2://猪鹿蝶の枚数2枚の場合Flushに設定
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.INOSHIKATYOU);
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

                    //成立情報の登録
                    AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.INOSHIKATYOU, Role.RoleManager.FLUSH);
                    break;
            }

        }

        //種のカードの枚数によってRemaining Flushの設定
        if (getCard_Dic[index].Count == 3) {
            searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
            getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.TANE;
            getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;

            //成立情報の登録
            AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.TANE, Role.RoleManager.REMAINING);

        } else if (getCard_Dic[index].Count > 3) {
            searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.TANE);
            getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

            //成立情報の登録
            AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.TANE, Role.RoleManager.FLUSH);
        }
    }  

    /// <summary>
    /// 短冊のカードが増えた時に、成立した役を取得
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private void CheckRoleTanzaku(int index,Card card) {

        //赤青短 10文 +a
        //赤短 5文+a
        //青短 5文+a

        //短冊 1文+a

        int searchTextIndex;

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

                    //成立情報の登録
                    AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.AKATANZAKU, Role.RoleManager.REMAINING);

                    break;
                case 2:
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.AKATANZAKU);
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

                    //成立情報の登録
                    AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.AKATANZAKU, Role.RoleManager.FLUSH);
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

                    //成立情報の登録
                    AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.AOTANZAKU, Role.RoleManager.REMAINING);

                    break;
                case 2:
                    searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.AOTANZAKU);
                    getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

                    //成立情報の登録
                    AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.AOTANZAKU, Role.RoleManager.FLUSH);

                    break;
            }
        }

        //短冊のカードの枚数によってRemaining Flushの設定
        if (getCard_Dic[index].Count == 4) {
            searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
            getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.TANZAKU;
            getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;

            //成立情報の登録
            AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.TANZAKU, Role.RoleManager.REMAINING);

        } else if (getCard_Dic[index].Count > 4) {
            searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.TANZAKU);
            getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

            //成立情報の登録
            AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.TANZAKU, Role.RoleManager.FLUSH);
        }
    }

    /// <summary>
    /// カスのカードが増えた時に、成立した役を取得
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private void CheckRoleKasu(int index, Card card) {

        //カス 1文+a

        int searchTextIndex;

        int isSakazukiCount = 0;
        var isSakazuki = GetIsTargetDivision(getCard_Dic[index - 2], Card.DIVISION.SAKAZUKI);
        if (isSakazuki) isSakazukiCount = 1;

        if (getCard_Dic[index].Count + isSakazukiCount == 8) {
            searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], "");
            getRoleString_Dic[index][searchTextIndex][0] = Role.RoleManager.KASU;
            getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.REMAINING;

            //成立情報の登録
            AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.KASU, Role.RoleManager.REMAINING);

        } else if (getCard_Dic[index].Count + isSakazukiCount > 8) {
            searchTextIndex = GetCardListIndex_TargetText(getRoleString_Dic[index], Role.RoleManager.KASU);
            getRoleString_Dic[index][searchTextIndex][1] = Role.RoleManager.FLUSH;

            //成立情報の登録
            AddEstablishRoleSList(index, searchTextIndex, Role.RoleManager.KASU, Role.RoleManager.FLUSH);

        }
    }
}
