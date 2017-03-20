using UnityEngine;
using System.Collections;

public class SlidingDoor : MonoBehaviour {

    [SerializeField]
    private ChapterSelectManager chapterSelectManager;

    [SerializeField]
    private GameObject leftDoor;
    [SerializeField]
    private GameObject rightDoor;

    [SerializeField]
    private Vector3 LEFT_DOOR_TARGET_POSITION;
    [SerializeField]
    private Vector3 RIGHT_DOOR_TARGET_POSITION;

    private Vector3 defaultLeftDoorPosition;
    private Vector3 defaultRightDoorPosition;

    private RectTransform leftDoorRect;
    private RectTransform rightDoorRect;

    // Use this for initialization
    void Start () {
        leftDoorRect = leftDoor.transform.GetComponent<RectTransform>();
        rightDoorRect = rightDoor.transform.GetComponent<RectTransform>();

        defaultLeftDoorPosition = leftDoorRect.localPosition;
        defaultRightDoorPosition = rightDoorRect.localPosition;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public void SlidingDoorClose() {
        StartCoroutine(DoorOpenClose(0.5f,"Close"));
    }

    private IEnumerator DoorOpenClose(float interval, string callName) {
        var time = 0.0f;
        var lerp = 0.0f;

        while (time < interval) {
            time += Time.deltaTime;
            lerp = Mathf.Lerp(0f, 1f, time / interval);
            leftDoorRect.localPosition = Vector3.Lerp(leftDoorRect.localPosition, LEFT_DOOR_TARGET_POSITION, lerp);
            rightDoorRect.localPosition = Vector3.Lerp(rightDoorRect.localPosition, RIGHT_DOOR_TARGET_POSITION, lerp);
            yield return 0;
        }

        if (callName == "Close") {
            chapterSelectManager.SlidingDoorClose();

            LEFT_DOOR_TARGET_POSITION = defaultLeftDoorPosition;
            RIGHT_DOOR_TARGET_POSITION = defaultRightDoorPosition;

            StartCoroutine(WaitSlidingDoorOpenAnimation());
            AudioManager.Instance.PlaySE(AudioName.AudioNameManager.SE_SE_CHAPTERSELECT2);

        } else if (callName == "Open") {
            chapterSelectManager.SlidingDoorOpen();
        }
    }

    private IEnumerator WaitSlidingDoorOpenAnimation() {
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(DoorOpenClose(0.5f, "Open"));
    }
}
