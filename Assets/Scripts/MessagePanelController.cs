using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VoidWars {
    public class MessagePanelController : MonoBehaviour {
        public float DisplayDuration = 2.0f;
        public float InterMessageDuration = 1.0f;

        [SerializeField] private AnimationClip _inClip;
        [SerializeField] private AnimationClip _outClip;
        [SerializeField] private TextMeshProUGUI _text;
        private readonly Queue<string> _messages = new Queue<string>();
        private Animator _animator;

        private void Start() {
            _animator = GetComponent<Animator>();
            StartCoroutine(handleMessages());
        }

        public void ShowMessage(string message) {
            _messages.Enqueue(message);
        }

        public void RemoveAllMessages() {
            _messages.Clear();
        }

        private IEnumerator handleMessages() {
            while(true) {
                while(_messages.Count == 0) {
                    yield return null;
                }

                var msg = _messages.Dequeue();
                _text.text = msg;
                _animator.SetTrigger("Summon");
                yield return new WaitForSeconds(_inClip.length + DisplayDuration);
                _animator.SetTrigger("Dismiss");
                yield return new WaitForSeconds(_outClip.length + InterMessageDuration);
            }
        }
    }
}