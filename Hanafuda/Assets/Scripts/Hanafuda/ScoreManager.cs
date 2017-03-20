using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    public int turnPlayerScore;

    [Header("初期スコア")]
    [SerializeField]
    private int DEFAULT_PLAYER_SCORE;
    [SerializeField]
    private int DEFAULT_CPU_SCORE;

    public int playerScore;
    public int cpuScore;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 再開用スコアの設定
    /// </summary>
    public void SetResumptionScore() {
        playerScore = PlayerPrefs.GetInt(SaveLoadManager.Instance.SAVE_DATA_PLAYER_SCORE);
        cpuScore = PlayerPrefs.GetInt(SaveLoadManager.Instance.SAVE_DATA_CPU_SCORE);
    }

    /// <summary>
    /// 初期スコアの設定
    /// </summary>
    public void SetDefaultScore() {
        playerScore = DEFAULT_PLAYER_SCORE;
        cpuScore = DEFAULT_CPU_SCORE;
    }
    public int GetDefaultPlayerScore() {
        return DEFAULT_PLAYER_SCORE;
    }
    public int GetDefaultCPUScore() {
        return DEFAULT_CPU_SCORE;
    }



}
