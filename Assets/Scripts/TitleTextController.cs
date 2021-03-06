﻿using System.Collections;
using UnityEngine;
using TMPro;

namespace VoidWars {
    public class TitleTextController : MonoBehaviour {
        private TextMeshProUGUI _text;
        [SerializeField] private float _fadeInTime = 3.0f;
        [SerializeField] private float _holdTime = 5.0f;
        [SerializeField] private float _fadeOutTime = 4.0f;
        [SerializeField] private float _initialSpacing = 40.0f;

        private void Start() {
            _text = GetComponent<TextMeshProUGUI>();
            SetText(string.Empty);
        }

        public void SetText(string text) {
            _text.color = Color.clear;
            _text.text = text;
            if (!string.IsNullOrEmpty(text)) {
                StartCoroutine(fadeInAndOut());
            }
        }

        public void Stop() {
            SetText(string.Empty);
        }

        private IEnumerator fadeInAndOut() {
            var color = new Color(1f, 1f, 1f, 0f);
            var spacing = _initialSpacing;
            var spaceDelta = _initialSpacing / _fadeInTime;
            for(float t = 0f; t < _fadeInTime; t += Time.deltaTime) {
                _text.characterSpacing = spacing;
                spacing -= spaceDelta * Time.deltaTime;
                color.a = Mathf.Clamp(t / _fadeInTime, 0f, 1f);
                _text.color = color;
                yield return null;
            }

            yield return new WaitForSeconds(_holdTime);

            spaceDelta = _initialSpacing / _fadeOutTime;
            for (float t = 0f; t < _fadeOutTime; t += Time.deltaTime) {
                _text.characterSpacing = spacing;
                spacing += spaceDelta * Time.deltaTime;
                color.a = 1f - Mathf.Clamp(t / _fadeOutTime, 0f, 1f);
                _text.color = color;
                yield return null;
            }
        }
    }
}