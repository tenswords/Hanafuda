using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CardManager : MonoBehaviour {

    //private Dictionary<GameObject,Card> cardDic;
    //[SerializeField]
    ////private Card[] cardList = new Card[48];
    //private GameObject[] cardList = new GameObject[48];
    //private const string DATA_FILE_PATH = "CSV/Hanafuda_Data";

    //// Use this for initialization
    //void Start() {
    //    //あとでCSVからの読み込みは無くすかも！
    //    //ReadCsv();
    //}

    //// Update is called once per frame
    //void Update() {

    //}

    //private void ReadCsv() {

    //    try {
    //        ReadCSVFile();

    //    } catch (System.Exception e) {
    //        // ファイルを開くのに失敗したとき
    //        System.Console.WriteLine(e.Message);
    //        Debug.Log("read failed");
    //        ReadCSVFile();
    //    }
    //}

    //private void ReadCSVFile() {
    //    int index = 0;
    //    TextAsset data = Resources.Load(DATA_FILE_PATH) as TextAsset;
    //    StringReader reader = new StringReader(data.text);

    //    // ストリームの末尾まで繰り返す
    //    while (reader.Peek() > -1) {

    //        // ファイルから一行読み込む
    //        string lineValue = reader.ReadLine();

    //        //1行目は見出しのため、読み込まない
    //        if (index > 0) {
    //            Debug.Log("lineValue " + lineValue);
    //            string[] values = lineValue.Split(',');

    //           // cardList[index-1] = new Card(values);

    //        }
    //        index++;
    //    }

    //    Debug.Log("success");
    //    reader.Close();
    //}

    /// <summary>
    /// カードリストを取得
    /// </summary>
    /// <returns></returns>
    //public Card[] GetCardList() {
    //    return cardList;
    //}



    /// <summary>
    /// カードのタグを設定する
    /// </summary>
    public void SetCardTag(GameObject target,string tag) {
        target.tag = tag;
    }

    /// <summary>
    /// カードの描画順レイヤーを設定する
    /// </summary>
    public void SetCardSortingLayer(GameObject target, string sortingLayerName) {
        target.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
    }

    /// <summary>
    /// カードの描画順レイヤーを設定する
    /// </summary>
    public void SetCardOrderInLayer(GameObject target,int sortingOrderNo) {
        target.GetComponent<SpriteRenderer>().sortingOrder = sortingOrderNo;
    }

    /// <summary>
    /// カードのエフェクトの表示・非表示を設定
    /// </summary>
    /// <param name="cardEffect"></param>
    /// <param name="isActive"></param>
    public void SetCardEffectIsActive(GameObject cardEffect, bool isActive) {
        cardEffect.SetActive(isActive);
    }
}

