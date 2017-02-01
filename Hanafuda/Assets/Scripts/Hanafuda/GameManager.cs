using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : SingletonMonoBehaviour<GameManager> {

    [SerializeField]
    private float sceneChangeInterval;

    //ゲーム全体の状態
    public STATE state;
    public enum STATE {
        TITLE,
        STORY,
        HANAFUDA,
    }

    void Awake() {
        Debug.Log("フレームレート 変更前" + Application.targetFrameRate);
        Application.targetFrameRate = 60;
        Debug.Log("フレームレート 変更後" + Application.targetFrameRate);
    }

    // Use this for initialization
    void Start () {
        state = STATE.TITLE;
    }
	
	// Update is called once per frame
	void Update () {
	}

    /// <summary>
    /// シーン切り替え
    /// </summary>
    public void SceneChange(string sceneName) {
        FadeManager.Instance.LoadLevel(sceneName, sceneChangeInterval);
    }
}
