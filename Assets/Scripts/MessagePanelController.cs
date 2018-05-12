using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VoidWars {
    public class Message {
        public readonly string Text;
        public readonly Sprite Icon;

        public Message(string text, Sprite icon) {
            Text = text;
            Icon = icon;
        }
    }

    public class MessagePanelController : MonoBehaviour {
        public float DisplayDuration = 2.0f;
        public float InterMessageDuration = 1.0f;

        [SerializeField] private AnimationClip _inClip;
        [SerializeField] private AnimationClip _outClip;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;

        private readonly Queue<Message> _messages = new Queue<Message>();
        private Animator _animator;

        private void Start() {
            _animator = GetComponent<Animator>();
            StartCoroutine(handleMessages());
        }

        public void ShowMessage(string text, Sprite icon) {
            var message = new Message(text, icon);
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
                _text.text = msg.Text;
                _image.sprite = msg.Icon;
                _animator.SetTrigger("Summon");
                yield return new WaitForSeconds(_inClip.length + DisplayDuration);
                _animator.SetTrigger("Dismiss");
                yield return new WaitForSeconds(_outClip.length + InterMessageDuration);
            }
        }
    }
}