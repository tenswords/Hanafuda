using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : SingletonMonoBehaviour<GameManager> {

    [Header("シーン変更時のフェード時間")]
    public float sceneChangeInterval;

    //ゲーム全体の状態
    public STATE state;

    public enum STATE {
        TITLE,
        CHAPTER_SELECT,
        STORY,
        HANAFUDA
    }

    public int storyNo;
    
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	}
}
