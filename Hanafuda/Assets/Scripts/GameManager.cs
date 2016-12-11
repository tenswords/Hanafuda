using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private CardManager cardManager;

    [SerializeField]
    private FieldManager fieldManager;

    //ゲーム中のステータス
    public STATE state;
    public enum STATE {
        READY,
        DECK_SHUFLE,
        CARD_HAND_OUT,
        GAME,
        PAUSE,
        RESULT
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
                    Debug.Log("STATE_READY");
                    onceWaitRaady = true;
                    StartCoroutine("WaitNextState", STATE.DECK_SHUFLE);
                }
                //readyTime += Time.deltaTime;
                //if (readyTime > 1.0f) {
                //    state = STATE.DECK_SHUFLE;
                //}
                break;

            case STATE.DECK_SHUFLE:
                Debug.Log("STATE_DECK_SHUFLE");
                //fieldManager.DeckShufle(cardManager.GetCardList());
                fieldManager.DeckShufle();
                state = STATE.CARD_HAND_OUT;
                break;

            case STATE.CARD_HAND_OUT:
                Debug.Log("CARD_HAND_OUT");
                if (!fieldManager.isCardHandOut) {
                    StartCoroutine("WaitNextState", GameManager.STATE.GAME);
                }
                break;

            case STATE.GAME:
                break;
            case STATE.PAUSE:
                break;
            case STATE.RESULT:
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
