using System.Collections;
using UnityEngine;

namespace VoidWars {
    public class LaserController : MonoBehaviour {
        public float StartSpeed = 20f;
        public float Duration = 3.0f;

        [SerializeField] private LineRenderer _line;
        [SerializeField] private ParticleSystem _beamParticles;
        [SerializeField] private ParticleSystem _targetParticles;

        private void Awake() {
            _line.enabled = false;
        }

        public void Run(Color color, Vector3 start, Vector3 end) {
            gameObject.transform.position = start;
            var delta = end - start;
            gameObject.transform.rotation = Quaternion.LookRotation(delta.normalized);
            _line.enabled = true;
            _line.material.color = color;
            _line.SetPosition(0, start);
            _line.SetPosition(1, end);
            float length = delta.magnitude;
            var psMain = _beamParticles.main;
            _targetParticles.transform.position = end;
            psMain.startSpeed = StartSpeed;
            psMain.startLifetime = length / StartSpeed;
            _beamParticles.Play();
            _targetParticles.Play();
        }

        public void Stop() {
            _beamParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _targetParticles.Stop();
            _line.enabled = false;
        }
    }
}