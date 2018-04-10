using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Pilot subclass for humans.
    /// </summary>
    public class HumanPilot : Pilot {
        public override void OnShipActivation(GameController gameController, ShipController shipController) {
            Debug.Log("HumanPilot.OnShipActivation()");

            // Ship has become active. Pick a manipulator depending on what state the game is in.
            var ship = shipController.gameObject;
            switch (gameController.State) {
                case GameState.SETUP: {
                        _current = ship.AddComponent<HumanSetupManipulator>();
                        var spawnPointObj = gameController.StartPositions[shipController.StartPointIndex];
                        var spawnPoint = spawnPointObj.GetComponent<SpawnPoint>();
                        _startZone = spawnPoint.StartBoundary;
                        _startZone.SetActive(true);
                        var szc = _startZone.GetComponent<StartZoneController>();
                        var shipClass = gameController.GetShipClassByName(shipController.ClassID);
                        var species = gameController.SpeciesInfo[(int)shipClass.Species];
                        szc.SetColor(species.MarkerColor);
                        break;
                    }

                case GameState.IN_PLAY:
                    setupInPlay(ship, gameController, shipController);
                    break;

                default:
                    break;
            }

            if (_current != null) {
                _current.OnActivation();
            }
        }

        public override void OnShipDeactivation(GameController gameController, ShipController shipController) {
            Debug.Log("HumanPilot.OnShipDeactivation()");

            if (_current != null) {
                _current.OnDeactivation();
                Object.Destroy(_current);
                _current = null;
            }

            if (_startZone != null) {
                _startZone.SetActive(false);
                _startZone = null;
            }
        }

        private void setupInPlay(GameObject ship, GameController controller, ShipController shipController) {
            switch(controller.PlayPhase) {
                case PlayPhase.SELECTING_MOVES:
                    _current = ship.AddComponent<HumanMoveShipManipulator>();
                    break;
            }
        }

        private Manipulator _current;
        private GameObject _startZone;
    }
}