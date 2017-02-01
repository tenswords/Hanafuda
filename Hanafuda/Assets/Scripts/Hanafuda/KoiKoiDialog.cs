using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KoiKoiDialog : MonoBehaviour {

    private bool isColor;

    private Image image;

    private Color defaultColor;
    [SerializeField]
    private Color ALFA_COLOR;

    private Color targetColor;

    private float alfaTimer;
    [SerializeField,Header("何秒間かけて透過させるか")]
    private float MAX_ALFA_TIMER;

    // Use this for initialization
    void Start () {
        image = GetComponent<Image>();
        defaultColor = image.color;

        isColor = true;
    }

    // Update is called once per frame
    void Update() {

        if (!isColor) {

            alfaTimer += Time.deltaTime;
            image.color = Color.Lerp(image.color, targetColor, alfaTimer / MAX_ALFA_TIMER);

            if(image.color == targetColor) {
                isColor = true;
            }
        }
	}

    public void OnPointerDown() {
        isColor = false;
        alfaTimer = 0.0f;
        targetColor = ALFA_COLOR;
    }
    public void OnPointerUp() {
        isColor = false;
        alfaTimer = 0.0f;
        targetColor = defaultColor;
    }

    public void OnYesButton() {

    }

    public void OnNoButton() {
        
    }

}
