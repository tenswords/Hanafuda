using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {

    [SerializeField]
    private float LIGHT_TIMER;

    [SerializeField]
    private Vector3 scalingSize;
    private Vector3 targetScale;

    private int lightDirection;
    private bool isLight;

    // Use this for initialization
    void Start () {
        lightDirection = 1;
        isLight = true;

        var scale = Random.Range(0.1f,0.5f);
        transform.localScale = new Vector3(scale, scale, scale);
    }
	
	// Update is called once per frame
	void Update () {
        if (isLight) {
            StartCoroutine(LIGHT(LIGHT_TIMER));
        }

    }

    private IEnumerator LIGHT(float interval) {

        isLight = false;
        var time = 0.0f;
        targetScale = transform.localScale + scalingSize * lightDirection;
        while (time<interval) {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, time/interval);
            yield return 0;
        }

        lightDirection *= -1;
        isLight = true;
    }
}
