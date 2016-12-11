using UnityEngine;
using System.Collections;
using System;

public class Card : MonoBehaviour {

    public string fileName;//たぶん使わない
    //public int month;//たぶん使わない
    //public string flower;
    public MONTH month;
    public FLOWER flower;
    public DIVISION division;
    public TYPE type;

    private SpriteRenderer spriteRenderer;
    public Sprite[] image;//0:表用画像　1:裏用の画像

    //public HIKARI hikari;
    //public TANE tane;
    //public TANZAKU tanzaku;
    //public KASU kasu;
    //public enum DATA_NO{
    //    FILENAME    = 0,
    //    MONTH       = 1,
    //    FLOWER      = 2,
    //    HIKARI      = 3,
    //    TANE        = 4,
    //    TANZAKU     = 5,
    //    KASU        = 6
    //}
    public enum MONTH {
        NONE = 0,
        JAN = 1,
        FEB = 2,
        MAR = 3,
        APR = 4,
        MAY = 5,
        JUN = 6,
        JUL = 7,
        AUG = 8,
        SEP = 9,
        OCT = 10,
        NOV = 11,
        DEC = 12
    }
    public enum FLOWER {
        NONE = 0,
        MATSU = 1,
        UME = 2,
        SAKURA = 3,
        FUJI = 4,
        AYAME = 5,
        BOTAN = 6,
        HAGI = 7,
        SUSUKI = 8,
        KIKU = 9,
        MOMIJI = 10,
        YANAGI = 11,
        KIRI = 12
    }
    public enum DIVISION {
        NONE = 0,
        TSURU = 1,
        MAKU = 2,
        TSUKI = 3,
        ONO = 4,
        HOUOU = 5,
        UGUISU = 6,
        HOTOTOGISU = 7,
        YATSUHASHI = 8,
        KARI = 9,
        SAKAZUKI = 10,
        TSUBAME = 11,
        INOSISI = 12,
        SHIKA = 13,
        TYOU = 14,
        TANZAKU = 15,
        AKATANZAKU = 16,
        AOTANZAKU = 17,
        KASU1 = 18,
        KASU2 = 19,
        KASU3 = 20
    }
    public enum TYPE {
        NONE = 0,
        HIKARI = 1,
        TANE = 2,
        TANZAKU = 3,
        KASU = 4,
        BAKE = 5,
    }

    //public enum HIKARI {
    //    NONE    = 0,
    //    TSURU   = 1,
    //    MAKU    = 2,
    //    TSUKI   = 3,
    //    ONO     = 4,
    //    HOUOU   = 5
    //}
    //public enum TANE {
    //    NONE        = 0,
    //    UGUISU      = 1,
    //    HOTOTOGISU  = 2,
    //    YATSUHASHI  = 3,
    //    TYOU        = 4,
    //    INOSISI     = 5,
    //    KARI        = 6,
    //    SAKAZUKI    = 7,
    //    SHIKA       = 8,
    //    TSUBAME     = 9
    //}
    //public enum TANZAKU {
    //    NONE        = 0,
    //    TANZAKU     = 1,
    //    AOTANZAKU   = 2,
    //    AKATANZAKU  = 3
    //}
    //public enum KASU {
    //    NONE    = 0,
    //    KASU1   = 1,
    //    KASU2   = 2,
    //    KASU3   = 3
    //}

    void Start() {
        fileName = gameObject.name;

        spriteRenderer = GetComponent<SpriteRenderer>();

        //画像の設定
        image = new Sprite[2];
        image[0] = spriteRenderer.sprite;
        image[1] = Resources.Load<Sprite>("Card/Hanafuda_Uramen");
        spriteRenderer.sprite = image[1];

        //画像の名前から、カードの詳細を設定
        var replaceFileName = fileName;
        string[] split_Name = new string[4];

        for (int i=0;i< split_Name.Length;i++) {
            if (i < split_Name.Length - 1) {
                var start = replaceFileName.LastIndexOf("_");
                var end = replaceFileName.Length-1 - start;

                split_Name[i] = replaceFileName.Substring(start + 1, end);
                replaceFileName = replaceFileName.Substring(0, start);

            } else {
                split_Name[i] = replaceFileName;
            }
        }

        for (int i=0;i< split_Name.Length; i++) {
            Debug.Log(split_Name[i]);
            switch(i){
                case 0:
                     
                switch (split_Name[i]) {
                    case "Hikari":  type = TYPE.HIKARI; break;
                    case "Tane": type = TYPE.TANE; break;
                    case "Tanzaku": type = TYPE.TANZAKU; break;
                    case "Kasu": type = TYPE.KASU; break;
                    case "Bake": type = TYPE.BAKE; break;
                }
                    break;
                case 1:

                    switch (split_Name[i]) {
                        case "Tsuru": division = DIVISION.TSURU; break;
                        case "Maku": division = DIVISION.MAKU; break;
                        case "Tsuki": division = DIVISION.TSUKI; break;
                        case "Ono": division = DIVISION.ONO; break;
                        case "Houou": division = DIVISION.HOUOU; break;
                        case "Uguisu": division = DIVISION.UGUISU; break;
                        case "Hototogisu": division = DIVISION.HOTOTOGISU; break;
                        case "Yatsuhashi": division = DIVISION.YATSUHASHI; break;
                        case "Kari": division = DIVISION.KARI; break;
                        case "Sakazuki": division = DIVISION.SAKAZUKI; break;
                        case "Tsubame": division = DIVISION.TSUBAME; break;
                        case "Inosisi": division = DIVISION.INOSISI; break;
                        case "Shika": division = DIVISION.SHIKA; break;
                        case "Tyou": division = DIVISION.TYOU; break;
                        case "Tanzaku": division = DIVISION.TANZAKU; break;
                        case "Akatanzaku": division = DIVISION.AKATANZAKU; break;
                        case "Aotanzaku": division = DIVISION.AOTANZAKU; break;
                        case "Kasu1": division = DIVISION.KASU1; break;
                        case "Kasu2": division = DIVISION.KASU2; break;
                        case "Kasu3": division = DIVISION.KASU3; break;
                    }

                    break;
                case 2:

                    switch (split_Name[i]) {
                        case "Matsu": flower = FLOWER.MATSU; break;
                        case "Ume": flower = FLOWER.UME; break;
                        case "Sakura": flower = FLOWER.SAKURA; break;
                        case "Fuji": flower = FLOWER.FUJI; break;
                        case "Ayame": flower = FLOWER.AYAME; break;
                        case "Botan": flower = FLOWER.BOTAN; break;
                        case "Hagi": flower = FLOWER.HAGI; break;
                        case "Susuki": flower = FLOWER.SUSUKI; break;
                        case "Kiku": flower = FLOWER.KIKU; break;
                        case "Momiji": flower = FLOWER.MOMIJI; break;
                        case "Yanagi": flower = FLOWER.YANAGI; break;
                        case "Kiri": flower = FLOWER.KIRI; break;
                    }
                    break;
                case 3:

                    switch (split_Name[i]) {
                        case "Jan": month = MONTH.JAN; break;
                        case "Feb": month = MONTH.FEB; break;
                        case "Mar": month = MONTH.MAR; break;
                        case "Apr": month = MONTH.APR; break;
                        case "May": month = MONTH.MAY; break;
                        case "Jun": month = MONTH.JUN; break;
                        case "Jul": month = MONTH.JUL; break;
                        case "Aug": month = MONTH.AUG; break;
                        case "Sep": month = MONTH.SEP; break;
                        case "Oct": month = MONTH.OCT; break;
                        case "Nov": month = MONTH.NOV; break;
                        case "Dec": month = MONTH.DEC; break;
                    }
                    break;
            }
        }


    }

    ///// <summary>
    ///// コンストラクタ
    ///// </summary>
    ///// <param name="values"></param>
    //public Card(string[] values) {

    //    for (int index=0;index<values.Length ;index++) {
    //        var value = values[index];


    //        //各データの読み込み処理
    //        switch (index) {
    //            case (int)DATA_NO.FILENAME:
    //                fileName = value;
    //                Debug.Log("fileName " + fileName);
    //                break;

    //            case (int)DATA_NO.MONTH:
    //                month = int.Parse(value);
    //                Debug.Log("month "  + month);
    //                break;

    //            case (int)DATA_NO.FLOWER:
    //                flower = value;
    //                Debug.Log("flower " + flower);
    //                break;

    //            case (int)DATA_NO.HIKARI:
    //                hikari = (HIKARI)Enum.GetValues(typeof(HIKARI)).GetValue(int.Parse(value));
    //                Debug.Log("hikari " + hikari);
    //                break;

    //            case (int)DATA_NO.TANE:
    //                tane = (TANE)Enum.GetValues(typeof(TANE)).GetValue(int.Parse(value));
    //                Debug.Log("tane " + tane);
    //                break;

    //            case (int)DATA_NO.TANZAKU:
    //                tanzaku = (TANZAKU)Enum.GetValues(typeof(TANZAKU)).GetValue(int.Parse(value));
    //                Debug.Log("tanzaku " + tanzaku);
    //                break;

    //            case (int)DATA_NO.KASU:
    //                kasu = (KASU)Enum.GetValues(typeof(KASU)).GetValue(int.Parse(value));
    //                Debug.Log("kasu " + kasu);
    //                break;
    //        }
    //    }
    //}
}
