using System.Collections;
using UnityEngine;
using VolumetricLines;

public class BoardBorderController : MonoBehaviour {
    [SerializeField] private VolumetricLineBehavior [] Lines;
    [SerializeField] private float _tweenTime = 3.0f;
    [SerializeField] private Color _color;
    [SerializeField] private float _pulseTime = 2.0f;
    [SerializeField] private float _lineWidth = 1.3f;
    [SerializeField] private float _pulseAmplitude = 1.25f;

	private void Update () {
        var zeroOne = Mathf.PingPong(Time.time, _pulseTime) / _pulseTime;
        var scale = 1.0f + _pulseAmplitude*zeroOne;
        foreach(var line in Lines) {
            line.LineWidth = _lineWidth * scale;
        }
	}

    public void SetColor(Color target) {
        StartCoroutine(tweenColor(_color, target));
    }

    private IEnumerator tweenColor(Color start, Color end) {
        for(float t = 0.0f; t < _tweenTime; t += Time.deltaTime) {
            var colorNow = Color.Lerp(start, end, t / _tweenTime);
            foreach(var line in Lines) {
                line.LineColor = colorNow;
            }
            yield return null;
        }
    }
}
