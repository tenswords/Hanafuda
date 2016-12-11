using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private CardManager cardManager;

    [SerializeField]
    private FieldManager fieldManager;

    //ゲーム全体の状態
    public STATE state;
    public enum STATE {
        READY,
        HANAFUDA,
        PAUSE,
    }

    [SerializeField]
    private Button roleListButton;
    [SerializeField]
    private GameObject roleListCanvas;

    //private float readyTime;
    private bool onceWaitRaady;

    // Use this for initialization
    void Start () {
        state = STATE.READY;
        onceWaitRaady = false;
    }
	
	// Update is called once per frame
	void Update () {
        switch (state) {
            case STATE.READY:
                if (!onceWaitRaady) {
                    Debug.Log("GameManager STATE_READY");
                    onceWaitRaady = true;
                    StartCoroutine("WaitNextState", STATE.HANAFUDA);
                }
                break;

            case STATE.HANAFUDA:
                break;
            case STATE.PAUSE:
                break;

        }
	}

    private IEnumerator WaitNextState(STATE nextState) {
        yield return new WaitForSeconds(1.0f);
        state = nextState;
    }

    /// <summary>
    /// 役一覧ボタンを押したときの処理
    /// </summary>
    public void OnRoleListButton() {
        roleListCanvas.SetActive(true);
    }


}
