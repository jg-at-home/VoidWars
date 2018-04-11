using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoidWars {
    public class SubpanelController : MonoBehaviour {
        public string Name;

        public virtual void OnActivation() {
            gameObject.SetActive(true);
        }

        public virtual void OnDeactivation() {
            gameObject.SetActive(false);
        }
    }
}
