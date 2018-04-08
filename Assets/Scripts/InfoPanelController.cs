﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    /// <summary>
    /// Controller for the top-level info UI panel..
    /// </summary>
    public class InfoPanelController : MonoBehaviour {
        public float NotificationTimeout = 1.0f;

        [SerializeField]
        private Text _titleText;

        [SerializeField]
        private RectTransform _contentRoot;

        /// <summary>
        /// Sets the title of the panel.
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title) {
            _titleText.text = title;
        }

        /// <summary>
        /// Passes a message to the internal content telling it to perform an operation.
        /// </summary>
        /// <param name="notification">The message function.</param>
        /// <param name="value">Message data.</param>
        public void NotifyContent(string notification, object value) {
            // It's possible a network message to enable the info panel hasn't got to its
            // destination yet, so we wait a while to see if it gets enabled to avoid race
            // conditions.
            StartCoroutine(notifyContentInner(notification, value));
        }

        private IEnumerator notifyContentInner(string notification, object value) {
            for (float t = 0f; t < NotificationTimeout; t += Time.deltaTime) {
                if (_content == null) {
                    yield return null;
                }
                else {
                    _content.SendMessage(notification, value, SendMessageOptions.RequireReceiver);
                    yield break;
                }
            }

            Debug.LogWarning("InfoPanelController: notification timeout");
        }

        /// <summary>
        /// Clears the content panel.
        /// </summary>
        public void ClearContent() {
            if (_content != null) {
                _content.SetParent(null);
                Destroy(_content);
                _content = null;
            }
        }

        /// <summary>
        /// Sets the content panel to contain the prefab panel.
        /// </summary>
        /// <param name="prefabName">The name of the prefab panel to build.</param>
        public void SetContent(string prefabName) {
            var contentPrefab = (GameObject)Resources.Load("Prefabs/UI/" + prefabName);
            ClearContent();
            var contentObj = Instantiate(contentPrefab);
            var content = contentObj.GetComponent<RectTransform>();
            if (content != null) {
                content.SetParent(_contentRoot, false);
                _content = content;
            }
        }

        private RectTransform _content;
    }
}