using UnityEngine;

namespace VoidWars {
    public class UIAudioPlayer : MonoBehaviour {
        [SerializeField] private AudioClip _buttonClick;
        [SerializeField] private AudioClip _errorSound;
        private AudioSource _audioSource;

        // Use this for initialization
        private void Start() {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayButtonClick() {
            _audioSource.PlayOneShot(_buttonClick);
        }

        public void PlayErrorSound() {
            _audioSource.PlayOneShot(_errorSound);
        }
    }
}