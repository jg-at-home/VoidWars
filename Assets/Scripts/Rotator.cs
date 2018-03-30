using UnityEngine;
using UnityEngine.UI;

public class Rotator : MonoBehaviour {
    [Header("Rotation speeds")]
    [Tooltip("Inner speed in deg/s")]
    public float Inner = -37.0f;

    [Tooltip("Outer speed in deg/s")]
    public float Outer = 43.0f;

    [Header("Images")]
    [SerializeField]
    private Image _innerRing;

    [SerializeField]
    private Image _outerRing;

    public void SetColor(Color color) {
        _innerRing.color = color;
        _outerRing.color = color;
    }

	private void Update () {
        _rotation.z = Time.deltaTime * Inner;
        _innerRing.transform.Rotate(_rotation);
        _rotation.z = Time.deltaTime * Outer;
        _outerRing.transform.Rotate(_rotation);
	}

    private Vector3 _rotation = new Vector3(0, 0, 0);
}
