//#define VERBOSE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    /// <summary>
    /// The state the game is in.
    /// </summary>
    public enum GameState {
        UNINITIALIZED,
        LOBBY,
        CONFIGURATION,
        WAIT_FOR_SPAWN,
        SETUP,
        IN_PLAY,
        FINISHED
    }

    /// <summary>
    /// The phase of play in the IN_PLAY state.
    /// </summary>
    public enum PlayPhase {
        IDLE,
        SELECTING_MOVES,
        MOVING_SHIPS,
        TAKING_ACTION,
        SELECTING_ATTACK,
        UPDATE_NPCS,
        // TODO: other stuff.
    }

    /// <summary>
    /// Class responsible for coordinating the game events.
    /// </summary>
    public partial class GameController : MonoBehaviour {
        [Header("Database")]
        public SpeciesInfo[] SpeciesInfo;
        public ShipClass[] ShipClasses;
        public WeaponClass[] WeaponClasses;
        public AuxiliaryClass[] ItemClasses;
        public GameObject[] StartPositions;
        public MoveTemplate[] MoveTemplates;

        [Header("Configuration")]
        public GameConfig Configuration;

        [Header("Controls")]
        public VoidNetworkManager NetworkManager;
        public GameObject Board;
        public GameObject ActiveShipIndicator;
        public InfoPanelController InfoPanel;
        public RectTransform ControlPanel;
        public ActionPanelController ActionPanel;
        public CameraRigController CameraRig;
        public BoardBorderController BorderController;
        public TitleTextController TitleController;
        public StatusPanelController StatusPanelController;
        public DamageIndicator DamageIndicator;
        public UIAudioPlayer UIAudioPlayer;
        public MessagePanelController MessagePanelController;
        public RectTransform CameoPanel;
        public PowerMeter PowerMeter;

        [Header("Prefabs")]
        public GameObject MapPinPrefab;
        public GameObject TargetIndicatorPrefab;
        public GameObject ScannerInfoPrefab;
        public ShipCameo ShipCameoPrefab;

        [Header("Parameters")]
        public float NPCUpdateTimeout = 10f;

        public void OnShieldsFailed(ShipController ship) {
            Debug.Log("Shields failed");
            _communicator.CmdSetShieldStatus(ship.ID, false);
        }

        /// <summary>
        /// Gets the current game state.
        /// </summary>
        public GameState State {
            get { return _state; }
        }

        /// <summary>
        /// Gets the current play phase.
        /// </summary>
        public PlayPhase PlayPhase {
            get { return _playPhase; }
        }

        /// <summary>
        /// Is the active ship the local one?
        /// </summary>
        public bool IsActiveShipLocal {
            get { return (_activeShip != null) && (_activeShip.OwnerID == _communicator.ID); }
        }

        #region UI
        /// <summary>
        /// Zooms the view in. If there's an active ship it will centre on that. Otherwise to
        /// the board centre.
        /// </summary>
        /// <param name="level">The level of zoom {0,1,2}</param>
        public void SetZoomLevel(int level) {
            #if VERBOSE
            Debug.LogFormat("GameController.SetZoomLevel({0})", level);
            #endif

            if (_activeShip != null) {
                CameraRig.ZoomTo(_activeShip.transform.position, level);
            }
            else {
                CameraRig.ZoomTo(Vector3.zero, level);

            }
        }

        /// <summary>
        /// Zooms all the way out an centers the camera.
        /// </summary>
        public void ResetZoom() {
            CameraRig.ZoomTo(Vector3.zero, 0);
        }


        /// <summary>
        /// Makes the info panel visible and sets its content up.
        /// </summary>
        /// <param name="caption">The caption for the info panel.</param>
        /// <param name="prefabName">The name of the content prefab.</param>
        public void EnableInfoPanel(string caption, string prefabName) {
            InfoPanel.gameObject.SetActive(true);
            InfoPanel.SetTitle(caption);
            InfoPanel.SetContent(prefabName);
        }

        /// <summary>
        /// Disables the info panel in the UI.
        /// </summary>
        public void DisableInfoPanel() {
            InfoPanel.ClearContent();
            InfoPanel.gameObject.SetActive(false);
        }

        /// <summary>
        /// Enables / disables the control panel.
        /// </summary>
        /// <param name="enable">Enable / disable flag.</param>
        public void EnableControlPanel(bool enable) {
            ControlPanel.gameObject.SetActive(enable);
        }

        /// <summary>
        /// Enables / disables the action selection panel.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableActionPanel(bool enable) {
            ActionPanel.gameObject.SetActive(enable);
        }

        /// <summary>
        /// Enables / disables the ship status panel.
        /// </summary>
        /// <param name="enable">Enable flag.</param>
        public void EnableStatusPanel(bool enable) {
            StatusPanelController.gameObject.SetActive(enable);
        }

        public void EnableDoneButton(bool enable) {
            InfoPanel.NotifyContent("EnableDoneButton", enable);
        }

        private void refreshInfoPanel(bool isActive, bool doneOK) {
            InfoPanel.NotifyActiveShipChange(isActive);
            EnableDoneButton(isActive && doneOK);
            EnableControlPanel(isActive);
            EnableStatusPanel(isActive);
        }
        #endregion UI

        #region Database
        /// <summary>
        /// Gets ta ship (controller) by ID.
        /// </summary>
        /// <param name="shipID">The ship ID.</param>
        /// <returns>The ship (controller) with that ID.</returns>
        public ShipController GetShip(int shipID) {
            return _ships.Find(s => s.ID == shipID);
        }

        /// <summary>
        /// Gets the ships owned by a player.
        /// </summary>
        /// <param name="ownerID">The owner to query.</param>
        /// <returns>The requesed ships.</returns>
        public List<ShipController> GetShipsOwnedBy(int ownerID) {
            return _ships.FindAll(s => s.OwnerID == ownerID);
        }

        /// <summary>
        /// Gets the ships NOT owned by a player.
        /// </summary>
        /// <param name="ownerID">The owner to exclude</param>
        /// <returns>A list of ships not owned by the player.</returns>
        public List<ShipController> GetShipsNotOwnedBy(int ownerID) {
            return _ships.FindAll(s => s.OwnerID != ownerID);
        }

        /// <summary>
        /// Gets the currently active ship controller.
        /// </summary>
        /// <returns>The active ship controller, or null.</returns>
        public ShipController GetActiveShip() {
            if (_activeShipID == -1) {
                return null;
            }
            else {
                return _ships.Find(s => s.ID == _activeShipID);
            }
        }

        /// <summary>
        /// Gets the template for a move.
        /// </summary>
        /// <param name="move">The move for a ship.</param>
        /// <returns></returns>
        public MoveTemplate GetMoveTemplate(ShipMove move) {
            return Array.Find(MoveTemplates, mt => mt.MoveType == move.MoveType && mt.Size == move.Size);
        }

        /// <summary>
        /// Gets the ship class data given its name.
        /// </summary>
        /// <param name="className">The ship class name.</param>
        /// <returns>The ship class</returns>
        public ShipClass GetShipClassByName(string className) {
            return Array.Find(ShipClasses, sc => sc.Name == className);
        }

        /// <summary>
        /// Gets the region where a player at the given start position can move their ship during setup.
        /// </summary>
        /// <param name="spawnPointIndex">The index of the ship's spawn point.</param>
        /// <returns></returns>
        public Rect GetStartPositionBoundary(int spawnPointIndex) {
            var spawnObj = StartPositions[spawnPointIndex];
            var spawnPoint = spawnObj.GetComponent<SpawnPoint>();
            var collider = spawnPoint.StartBoundary.GetComponent<BoxCollider>();
            var bounds = collider.bounds;
            var rx = bounds.center.x - bounds.size.x / 2;
            var ry = bounds.center.z - bounds.size.z / 2;
            return new Rect(rx, ry, bounds.size.x, bounds.size.z);
        }

        /// <summary>
        /// Gets the start point indices for a game configuration.
        /// </summary>
        /// <param name="faction">The faction to get the start points for.</param>
        /// <param name="numPlayers">How many human players there are.</param>
        /// <param name="numShipsPerPlayer">How many ships ach player controls.</param>
        /// <returns>An array of start point indices.</returns>
        public int [] GetStartPointIndices(Faction faction, int numPlayers, int numShipsPerPlayer) {
            if (faction == Faction.HUMANS) {
                if (numPlayers == 1) {
                    if (numShipsPerPlayer == 1) {
                        return s_p1StartPositions1;
                    }
                    else {
                        return s_p1StartPositions2;
                    }
                }
                else {
                    if (numShipsPerPlayer == 1) {
                        return s_startPositions2;
                    }
                    else {
                        return s_startPositions4;
                    }
                }
            }
            else {
                if (numPlayers == 1) {
                    if (numShipsPerPlayer == 1) {
                        return s_p2StartPositions1;
                    }
                    else {
                        return s_p2StartPositions2;
                    }
                }
                else {
                    if (numShipsPerPlayer == 1) {
                        return s_startPositions2;
                    }
                    else {
                        return s_startPositions4;
                    }
                }
            }
        }

        /// <summary>
        /// Gets weapon class data for the indicated type.
        /// </summary>
        /// <param name="type">The weapon type.</param>
        /// <returns>The weapon class.</returns>
        public WeaponClass GetWeaponClass(WeaponType type) {
            return WeaponClasses[(int)type-1];
        }

        /// <summary>
        /// Gets the item class of the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The assoicated item class.</returns>
        public ItemClass GetItemClass(AuxType type) {
            return ItemClasses.FirstOrDefault(c => c.ItemType == type);
        }

        /// <summary>
        /// Finds the object (ship or NPC) with the given ID.
        /// </summary>
        /// <param name="objID">Target ID.</param>
        /// <returns>The object, or null.</returns>
        public VoidWarsObject GetObjectWithID(int objID) {
            return objects.FirstOrDefault(o => o.ID == objID);
        }

        private IEnumerable<VoidWarsObject> objects {
            get {
                foreach(var ship in _ships) {
                    yield return ship;
                }

                foreach(var npc in _npcs) {
                    yield return npc;
                }
            }
        }

        #endregion Database

        /// <summary>
        /// Called by action panel to execute the currently selected action.
        /// </summary>
        public void ExecuteSelectedAction() {
            EnableActionPanel(false);
            _actionComplete = false;
            ActionPanel.SelectCurrentAction();
            StartCoroutine(waitForActionCompletion());
        }

        /// <summary>
        /// Called when an action is selected.
        /// </summary>
        /// <param name="action">The action as a string.</param>
        /// <param name="energy">The energy cost of the action.</param>
        public void OnActionSelected(string action, float energy, float drain) {
            _energyAfterSelection = Mathf.Max(_energyAfterSelection - energy, 0f);
            _drainAfterSelection = Mathf.Max(_drainBeforeSelection + drain, 0f);
            StatusPanelController.GoToPanel("Energy Use");
        }

        /// <summary>
        /// Gets the energy for the active ship after a selection (move / action) has been selected.
        /// </summary>
        public float ShipEnergyAfterSelection {
            get { return _energyAfterSelection; }
        }

        /// <summary>
        /// Gets the energy drain for the active ship after an action has been selected.
        /// </summary>
        public float ShipEnergyDrainAfterSelection {
            get { return _drainAfterSelection; }
        }

        /// <summary>
        /// Sets the action complete flag.
        /// </summary>
        public void OnActionComplete() {
            _actionComplete = true;
        }

        private IEnumerator waitForActionCompletion() {
            while (!_actionComplete) {
                yield return null;
            }

            NextAction();
        }

        /// <summary>
        /// Sets the local communicator instance so the game controller can perform network
        /// operations with the correct authority.
        /// </summary>
        /// <param name="communicator">The communicator.</param>
        public void SetCommunicator(Communicator communicator) {
            _communicator = communicator;
        }

        /// <summary>
        /// Is the supplied owner the one for the local machine?
        /// </summary>
        /// <param name="ownerID">An owner ID (usually of a ship).</param>
        /// <returns>True if the (ship's) owner is the communicator (ie player).</returns>
        public bool IsOwner(int ownerID) {
            return (ownerID == _communicator.ID);
        }

        /// <summary>
        /// Gets the ID of the local player.
        /// </summary>
        public int LocalOwnerID {
            get { return _communicator.ID; }
        }

        #region Client Code
        /// <summary>
        /// Creates a cameo for a ship in the UI.
        /// </summary>
        /// <param name="ship">The ship to cameo.</param>
        /// <returns></returns>
        public ShipCameo CreateShipCameo(ShipController ship) {
            // Create the UI element.
            var shipCameo = Instantiate(ShipCameoPrefab, CameoPanel);

            // Allocate a render target for the cameo camera to render into.
            var renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            shipCameo.Initialize(renderTexture, ship.ShipData.Name);

            // Mount the camera above the ship looking down.
            var cameraMount = new GameObject("CameoCam");
            var cameoCam = cameraMount.AddComponent<Camera>();
            cameoCam.fieldOfView = 45.0f;
            cameoCam.targetTexture = renderTexture;
            cameoCam.cullingMask = 1 << LayerMask.NameToLayer("Ships");
            cameraMount.transform.position = new Vector3(0, 5f, 0);
            cameraMount.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            cameraMount.transform.SetParent(ship.transform, false);

            return shipCameo;
        }

        /// <summary>
        /// Shows a message popup.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="icon">The icon to show.</param>
        public void ShowMsg(string message, Sprite icon) {
            MessagePanelController.ShowMessage(message, icon);
        }

        /// <summary>
        /// Shows everyone a popup message.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="role">Who issued the message.</param>
        public void BroadcastMsg(string message, Role role) {
            _communicator.CmdBroadcastMessage(message, role);
        }

        /// <summary>
        /// Ses the selected target for something.
        /// </summary>
        public void SelectTarget(GameObject target) {
            // Check power requirements.
            var weaponType = _activeShip.GetWeaponType(_activeWeapon);
            var weaponClass = GetWeaponClass(weaponType);
            if (_activeShip.GetEnergyBudgetFor(EnergyConsumer.Weapons) < weaponClass.PowerUsage) {
                return;
            }

            // It's an enemy. Change phase to ATTACKING.
            UIAudioPlayer.PlayButtonClick();
            _target = target;
            performAttack();
        }

        /// <summary>
        /// Client attack routine.
        /// </summary>
        /// <param name="sourceID">The ID of the source ship.</param>
        /// <param name="targetID">The ID of the target ship.</param>
        /// <param name="weaponSlot">The weapon slot to use.</param>
        /// <param name="applyDamage">If true, apply the damage.</param>
        public void PerformAttack(int sourceID, int targetID, int weaponSlot) {
            EnableDoneButton(false);
            EnableStatusPanel(false);
            StartCoroutine(attackCoroutine(sourceID, targetID, weaponSlot));
        }

        private IEnumerator attackCoroutine(int sourceID, int targetID, int weaponSlot) {
            const float margin = 10f;

            // Stash the current camera state.
            var oldCameraPos = CameraRig.transform.position;
            var oldCameraRot = CameraRig.transform.rotation;

            // Compute the new camera position to frame the ships.
            var sourceShip = GetShip(sourceID);
            var aspectRatio = Screen.width / Screen.height;
            var tanFOV = Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2.0f);
            var target = GetObjectWithID(targetID);
            var sourceObj = sourceShip.gameObject;
            var targetObj = target.gameObject;
            var sourcePos = sourceObj.transform.position;
            var targetPos = targetObj.transform.position;
            var delta = targetPos - sourcePos;
            var midPoint = sourcePos + 0.5f * delta;
            var separation = delta.magnitude;
            var cameraDistance = (separation / 2f / aspectRatio) / tanFOV;
            var direction = (CameraRig.transform.position - midPoint).normalized;
            var cameraDestination = midPoint + direction * (cameraDistance + margin);

            // Zip the camera over there.
            Vector3 velocity = Vector3.zero;
            while(Vector3.Distance(CameraRig.transform.position, cameraDestination) > 5.0e-1f) {
                CameraRig.transform.position = Vector3.SmoothDamp(CameraRig.transform.position, cameraDestination, ref velocity, 0.75f);
                yield return null;
            }

            var weapon = sourceShip.GetWeapon(weaponSlot);
            yield return sourceShip.Attack(target, weaponSlot, weapon);

            // TODO: if target is wiped, do death stuff.

            // Restore the camera state.
            velocity = Vector3.zero;
            while (Vector3.Distance(CameraRig.transform.position, oldCameraPos) > 5.0e-1f) {
                CameraRig.transform.position = Vector3.SmoothDamp(CameraRig.transform.position, oldCameraPos, ref velocity, 0.75f);
                yield return null;
            }

            CameraRig.transform.position = oldCameraPos;
            CameraRig.transform.rotation = oldCameraRot;

            // TODO: is this ok? The Done button should always be on for the attack panel.
            EnableDoneButton(true);

            if (_communicator.isServer) {
                NextShipServer();
            }
        }

        /// <summary>
        /// Sets the active weapon on the active ship.
        /// </summary>
        /// <param name="index">0=primary, 1 = secondary.</param>
        public void SetActiveWeapon(int index) {
            _activeWeapon = index;
            computeAttackTargets();
        }

        /// <summary>
        /// Gets the currently active weapon.
        /// </summary>
        /// <returns></returns>
        public int GetActiveWeaponIndex() {
            return _activeWeapon;
        }

        /// <summary>
        /// Gets the number of attack targets.
        /// </summary>
        public int AttackTargetCount {
            get { return _attackTargets.Count; }
        }

        private void computeAttackTargets() {
            clearAttackTargets();

            // Find max range of the weapons.
            var shipController = GetActiveShip();

            // Can't fire if you're cloaked!
            if (!shipController.IsCloaked) {
                var weapon = shipController.GetWeapon(_activeWeapon);
                if (weapon.IsAvailable) { 
                    var range = weapon.Range;
                    var position = shipController.gameObject.transform.position;
                    var objectsInRange = Physics.OverlapSphere(position, range);
                    foreach (var target in objectsInRange) {
                        if ((target.gameObject != shipController.gameObject) &&
                            (target.gameObject.CompareTag("Targetable"))) {
                            var targetShip = target.GetComponent<ShipController>();
                            if (targetShip != null) {
                                // Can't attack your own side or cloaked vessels
                                if (IsOwner(targetShip.ID) || targetShip.IsCloaked) {
                                    continue;
                                }
                            }

                            // Target in range, but we need to check the angles.
                            if (checkTargetGeometry(shipController, target.gameObject, weapon)) {
                                var indicatorGO = Instantiate(TargetIndicatorPrefab);
                                var indicator = indicatorGO.GetComponent<TargetIndicator>();
                                indicator.Initialize(shipController.transform.position, target.transform.position, Color.red);
                                _attackTargets.Add(indicator);
                            }
                        }
                    }
                }
            }
            InfoPanel.NotifyTargetsChanged();
        }

        /// <summary>
        /// Applies damage to an object.
        /// </summary>
        /// <param name="objID">The ID of the object to damage.</param>
        /// <param name="damage">The amount of damage to apply.</param>
        /// <param name="dT">Change in temerpature of the ship as a result of the attack.</param>
        public void ApplyDamage(int objID, float damage, float dT) {
            // Do damage on server.
            _communicator.CmdApplyDamage(objID, damage, dT);
        }

        /// <summary>
        /// Creates a damage indicator sprite for a ship.
        /// </summary>
        /// <param name="objID">The ID of the damaged object.</param>
        /// <param name="damage">The amount of damage.</param>
        public void ShowDamage(int objID, float damage) {
            var ship = GetShip(objID);
            if (ship != null) {
                DamageIndicator.SetValue(ship.gameObject.transform.position, (int)damage);
            }
        }

        /// <summary>
        /// Given that a target is within range, check that it lies within the targetable cone that the
        /// weapon can reach.
        /// </summary>
        /// <param name="shipController">The ship (controller)</param>
        /// <param name="target">The targeted object.</param>
        /// <param name="weapon">The weapon being used.</param>
        /// <returns>True if the target can be hit.</returns>
        private bool checkTargetGeometry(ShipController shipController, GameObject target, WeaponInstance weapon) {
            // Front or back?
            Transform node;
            float angle;
            if (_activeWeapon == 0) {
                // Front.
                node = shipController.FrontNode;
                angle = Mathf.Deg2Rad * weapon.PrimaryAngle / 2;
            }
            else {
                node = shipController.RearNode;
                angle = Mathf.Deg2Rad * weapon.SecondaryAngle / 2;
            }

            // Zero angle => it's not an issue.
            if (Mathf.Abs(angle) < 1.0e-4f) {
                return true;
            }

            var nodeToTargetDir = (target.transform.position - node.position).normalized;
            var cosAngle = Vector3.Dot(nodeToTargetDir, node.forward);
            if (cosAngle <= 0f) {
                // Look out, he's behind you!
                return false;
            }

            if (Mathf.Acos(cosAngle) > angle) {
                return false;
            }

            // Depending on weapon type, ray cast to see if there's anything in the way.
            if (weapon.RequiresLineOfSight) {
                RaycastHit hit;
                var direction = (target.transform.position - node.position).normalized;
                var ray = new Ray(node.position, direction);
                var layer = 1 << LayerMask.NameToLayer("ActiveObjects");
                if (Physics.Raycast(ray, out hit, layer)) {
                    if (!ReferenceEquals(hit.collider.transform.root.gameObject, target)) {
                        // Something is obstructing the ray.
                        return false;
                    }
                }
            }

            return true;
        }

        private void clearAttackTargets() {
            // TODO: object pool?
            foreach(var target in _attackTargets) {
                Destroy(target.gameObject);
            }
            _attackTargets.Clear();
        }

        /// <summary>
        /// Gets the bounding rectangle of the play area.
        /// </summary>
        /// <returns>The bounding rectangle.</returns>
        public Rect GetBoardBounds() {
            return _boardBounds;
        }

        /// <summary>
        /// Gets the marker colour for a ship.
        /// </summary>
        /// <param name="shipID">The ID of the ship.</param>
        /// <returns>The marker color.</returns>
        public Color GetShipMarkerColor(int shipID) {
            var shipController = _ships.Find(s => s.ID == shipID);
            var shipClass = GetShipClassByName(shipController.ClassID);
            var species = SpeciesInfo[(int)shipClass.Species];
            return species.MarkerColor;
        }

        /// <summary>
        /// Called client-side to notify a new ship has become active.
        /// </summary>
        /// <param name="ownerID">The ID of the ship's owner.</param>
        /// <param name="shipID">The ship's ID.</param>
        public void NotifyActiveShip(int ownerID, int shipID) {
            if (_communicator.ID == ownerID) {
                SetActiveShip(shipID);
            }
            else {
                SetActiveShip(-1);
            }

            if (shipID >= 0) {
                // Change the border colour to reflect the new ship's faction.
                var markerColor = GetShipMarkerColor(shipID);
                BorderController.SetColor(markerColor);
            }
        }

        /// <summary>
        /// Registers a ship with the controller.
        /// </summary>
        /// <param name="ship">The ship.</param>
        public void RegisterShip(ShipController ship) {
            // Preconditions.
            Debug.Assert(ship != null);
            Debug.Assert(!_ships.Contains(ship), "Duplicate controller?");

            // Implementation.
            #if VERBOSE
            Debug.LogFormat("GameController.RegisterShip()");
            #endif

            _ships.Add(ship);

        }

        /// <summary>
        /// Called client-side to handle activation of a ship.
        /// </summary>
        /// <param name="shipID">The ID of the ship to activate.</param>
        public void SetActiveShip(int shipID) {
            // Is there a currently active ship?
            if (_activeShip != null) {
                // Yes, deactivate it.
                _activeShip.Deactivate();
            }

            ActiveShipIndicator.transform.parent = null;
            if (shipID == -1) {
                ActiveShipIndicator.SetActive(false);
                _activeShip = null;
                _activeShipID = shipID;
                refreshInfoPanel(false, false);
                EnableActionPanel(false);
                clearAttackTargets();
            }
            else {
                _activeShip = _ships.Find(s => s.ID == shipID);
                _activeShipID = _activeShip.ID;
                _energyAfterSelection = _activeShip.Energy;
                _energyBeforeSelection = _activeShip.Energy;
                _drainBeforeSelection = _activeShip.EnergyDrainPerTurn;
                _drainAfterSelection = _activeShip.EnergyDrainPerTurn;
                _actionCount = 1;
                if (_activeShip.ControlType == ControlType.HUMAN) {
                    // Fire up the UI.
                    ActiveShipIndicator.SetActive(true);
                    var ship = _activeShip.gameObject;
                    ActiveShipIndicator.transform.parent = ship.transform;
                    ActiveShipIndicator.transform.localPosition = Vector3.zero;
                    var rotator = ActiveShipIndicator.GetComponent<Rotator>();
                    var shipClass = GetShipClassByName(_activeShip.ClassID);
                    var species = SpeciesInfo[(int)shipClass.Species];
                    var color = species.MarkerColor;
                    rotator.SetColor(color);
                    updateUI(true);
                }

                _activeShip.Activate();
            }
        }

        private void updateUI(bool infoPanelStatus) {
            bool doneOK = true;
            switch(_playPhase) {
                case PlayPhase.TAKING_ACTION:
                    if (IsActiveShipLocal) {
                        InfoPanel.NotifyActiveShipChange(true);
                        EnableActionPanel(true);
                        setActionPanelTitle();
                        InfoPanel.NotifyContent("SetDoneButtonCaption", "Select");
                    }
                    else {
                        InfoPanel.NotifyActiveShipChange(false);
                        EnableActionPanel(false);
                    }
                    doneOK = false;
                    break;
            }
            refreshInfoPanel(infoPanelStatus, doneOK);
        }

        private void setActionPanelTitle() {
            if (_activeShip.ActionsThisTurn == 1) {
                ActionPanel.SetTitle("Action");
            }
            else {
                ActionPanel.SetTitle(string.Format("Action {0}/{1}", _actionCount, _activeShip.ActionsThisTurn));
            }
        }

        /// <summary>
        /// Call to indicate a ship wants to take its next action. If it's run out of actions,
        /// move on to the next ship.
        /// </summary>
        public void NextAction() {
            #if VERBOSE
            Debug.Log("GameController.NextAction()");
            #endif

            ++_actionCount;
            if (_actionCount > _activeShip.ActionsThisTurn) {
                _actionCount = 1;
                NextShip();
            }
            else {
                EnableActionPanel(true);
                setActionPanelTitle();
            }
        }

        /// <summary>
        /// Whatever the game state / phase, moves on to the next ship in the round. If there is none,
        /// updates the phase / state as required.
        /// </summary>
        public void NextShip() {
            #if VERBOSE
            Debug.Log("GameController.NextShip()");
            #endif

            _communicator.CmdNextShip();
        }

        /// <summary>
        /// Get / set the selected move for the current ship.
        /// </summary>
        public ShipMoveInstance SelectedMove {
            get { return _selectedMove; }
            set {
                _selectedMove = value;
                var energyForMove = 0f;
                if (_selectedMove.Move.MoveType != MoveType.None) {
                    energyForMove = _activeShip.GetEnergyForMove(_selectedMove.Move);
                }
                _energyAfterSelection = Mathf.Max(_energyBeforeSelection - energyForMove, 0f);
                StatusPanelController.GoToPanel("Energy Use");
            }
        }

        /// <summary>
        /// Sets the move for a ship.
        /// </summary>
        public void StoreSelectedMove() {
            if (SelectedMove.Move.MoveType != MoveType.None) {
                // Drop a pin to show where the move will take the ship.
                var pin = Instantiate(MapPinPrefab, SelectedMove.Position, Quaternion.identity);
                _mapPins.Add(pin);
            }
            else {
                // If you elect not to move you can do an additional action.
                Debug.Log("GameController: additional action scheduled");
                _activeShip.IncreaseActionsThisTurn();
            }

            // Tell the server about the move.
            _communicator.CmdAddPlayerMove(SelectedMove);
        }

        /// <summary>
        /// Called to perform client work at the start of a round.
        /// </summary>
        public void BeginRoundClient() {
            #if VERBOSE
            Debug.Log("GameController.BeginRoundClient()");
            #endif

            // Update ships.
            foreach(var ship in _ships) {
                ship.BeginRound();
            }

            EnableInfoPanel("Move", "MoveInfoPanel");
        }

        /// <summary>
        /// Called to perform client work at the end of a round.
        /// </summary>
        public void EndRoundClient() {
            #if VERBOSE
            Debug.Log("GameController.EndRoundClient()");
            #endif
        }

        /// <summary>
        /// Moves the indicated ship to the target point IF it's the controlled ship.
        /// </summary>
        /// <param name="move">The move to execute.</param>
        public void MoveShip(ShipMoveInstance move) {
            var shipController = _ships.Find(s => s.ID == move.ShipID);
            if (shipController.OwnerID == _communicator.ID) {
                // Ship is controlled by me.
                shipController.EnactMove(move, onMoveFinished);
            }
        }

        private void onMoveFinished(int shipID) {
            _communicator.CmdMoveFinished(shipID);
        }
        #endregion Client Code

        #region Server code
        /// <summary>
        /// Computes a destination position for teleporting to.
        /// </summary>
        /// <param name="ship">The teleporting ship.</param>
        /// <returns>The desination position.</returns>
        public Vector3 GetTeleportDestination(ShipController ship) {
            var layerMask = 1 << LayerMask.NameToLayer("Ships");
            layerMask |= 1 << LayerMask.NameToLayer("ActiveObjects");

            var enemyShips = GetShipsNotOwnedBy(ship.OwnerID);
            var bestPoint = -1;
            var bestDistance = 0f;
            for (int i = 0; i < _teleportPoints.Length; ++i) {
                var point = _teleportPoints[i];
                var distance = 0f;
                foreach (var enemy in enemyShips) {
                    distance += Vector3.Distance(enemy.transform.position, point);
                }

                if (distance > bestDistance) {
                    if (!Physics.CheckSphere(point, 1f, layerMask)) {
                        bestDistance = distance;
                        bestPoint = i;
                    }
                }
            }

            Vector3 result;
            if (bestPoint >= 0) {
                result =_teleportPoints[bestPoint];
                result.y = ship.transform.position.y;
            }
            else {
                result = ship.transform.position;
            }
            return result;
        }

        /// <summary>
        /// Adds an NPC to the controlled set.
        /// </summary>
        /// <param name="npc">The NPC to add.</param>
        public void AddNPC(NPCObject npc) {
            _npcs.Add(npc);
        }

        /// <summary>
        /// Removes an NPC from the controlled set.
        /// </summary>
        /// <param name="npc">The NPC to remove.</param>
        public void RemoveNPC(NPCObject npc) {
            _npcs.Remove(npc);
        }

        /// <summary>
        /// Updates the NPCs.
        /// </summary>
        public void UpdateNPCs() {
            #if VERBOSE
            Debug.Log("GameController.UpdateNPCs()");
            #endif
            var numNPCs = _npcs.Count;
            if (numNPCs > 0) {
                var syncToken = new NPCSyncToken(numNPCs);
                StartCoroutine(npcUpdateCoro(syncToken));
            }
            else {
                AdvanceGame();
            }
        }

        private IEnumerator npcUpdateCoro(NPCSyncToken syncToken) {
            // Start NPC updates.
            foreach(var npc in _npcs) {
                StartCoroutine(npc.PerTurnUpdate(syncToken));
            }

            // Wait for them all to complete.
            var startTime = Time.time;
            while(!syncToken.IsSynced) {
                yield return null;
                var elapsed = Time.time - startTime;
                if (elapsed > NPCUpdateTimeout) {
                    Debug.LogError("Timeout waiting for NPC update sync");
                    break;
                }
            }

            // Clean up any expired ones.
            for (int i = _npcs.Count-1; i >= 0; --i) {
                var npc = _npcs[i];
                if (npc.HasExpired) {
                    NetworkServer.Destroy(npc.gameObject);
                }
            }

            // Move on.
            AdvanceGame();
        }

        /// <summary>
        /// Adds a player move to the set. There should be one for each player.
        /// </summary>
        /// <param name="move"></param>
        public void AddPlayerMove(ShipMoveInstance move) {
            _selectedMoves.Add(move);
        }

        /// <summary>
        /// Called when a round begins.
        /// </summary>
        public void BeginRoundServer() {
            #if VERBOSE
            Debug.LogFormat("GameController.BeginRoundServer({0})", _round);
            #endif

            foreach (var shipController in _ships) {
                shipController.BeginRound(_round);
            }
            ++_round;
        }

        /// <summary>
        /// Called when a round ends.
        /// </summary>
        public void EndRoundServer() {
            #if VERBOSE
            Debug.Log("GameController.EndRoundServer()");
            #endif
            _selectedMoves.Clear();
        }

        /// <summary>
        /// Server-side call for moving to the next ship in the turn order.
        /// </summary>
        public void NextShipServer() {
            var turnList = getTurnOrder();
            var nextIndex = _activeShipIndex + 1;
            if (nextIndex == turnList.Count) {
                // End of state / phase.
                AdvanceGame();
            }
            else {
                SetActiveShipByIndex(nextIndex, false);
            }
        }

        /// <summary>
        /// Updates the game when a turn ends.
        /// </summary>
        public void AdvanceGame() {
            #if VERBOSE
            Debug.Log("GameController.AdvanceGame()");
            #endif
            switch(_state) {
                case GameState.SETUP:
                    // Setup is done. Time to play!
                    _round = 0;
                    SetState(GameState.IN_PLAY, true);
                    _communicator.BeginNextRound();
                    SetActiveShipByIndex(0, true);
                    break;

                case GameState.IN_PLAY:
                    advancePlay();
                    // TODO: game over? Otherwise, around we go again.
                    SetActiveShipByIndex(0, true);
                    break;

                default:
                    Debug.LogError("Broken game state machine");
                    break;
            }
        }

        private void advancePlay() {
            switch(_playPhase) {
                case PlayPhase.SELECTING_MOVES:
                    // TODO: lockout UI.
                    _communicator.NotifyActiveShip(-1, -1);
                    _movesToMake = _selectedMoves.Count(m => m.Move.MoveType != MoveType.None);
                    _communicator.EnactMoves(_selectedMoves);
                    SetPlayPhase(PlayPhase.MOVING_SHIPS, true);
                    break;

                case PlayPhase.TAKING_ACTION:
                    // Ensure all action panels are closed.
                    _communicator.CmdDisableActionPanel();
                    _communicator.CmdEnableInfoPanel("Attacking", "AttackInfoPanel");
                    SetPlayPhase(PlayPhase.SELECTING_ATTACK, true);
                    break;

                case PlayPhase.SELECTING_ATTACK:
                    _communicator.CmdEnableInfoPanel("Please wait", "NpcMoveInfoPanel");
                    SetPlayPhase(PlayPhase.UPDATE_NPCS, true);
                    _communicator.CmdUpdateNPCs();
                    break;

                case PlayPhase.UPDATE_NPCS:
                    _communicator.CmdEnableInfoPanel("Move", "MoveInfoPanel");
                    SetPlayPhase(PlayPhase.SELECTING_MOVES, true);
                    _communicator.EndThisRound();
                    _communicator.BeginNextRound();
                    break;
            }
        }
        
        /// <summary>
        /// Adds a player to the active list.
        /// </summary>
        /// <param name="player">The player to add.</param>
        public void AddPlayer(PlayerServerRep player) {
            #if VERBOSE
            Debug.LogFormat("GameController.AddPlayer({0})", player.PlayerID);
            #endif
            _players.Add(player);
        }

        /// <summary>
        /// Removes a playr from the active list.
        /// </summary>
        /// <param name="playerID">The player's ID.</param>
        public void RemovePlayer(int playerID) {
            #if VERBOSE
            Debug.LogFormat("GameController.RemovePlayer({0})", playerID);
            #endif
            var index = _players.FindIndex(p => p.PlayerID == playerID);
            if (index >= 0) {
                _players.RemoveAt(index);
            }
            else {
                Debug.LogWarning("GameController: unable to remove player with ID " + playerID);
            }
        }

        /// <summary>
        /// Gets a player by ID (server only).
        /// </summary>
        /// <param name="playerID">The player ID.</param>
        /// <returns>The player instance.</returns>
        public PlayerServerRep GetPlayer(int playerID) {
            return _players.Find(p => p.PlayerID == playerID);
        }

        /// <summary>
        /// Updates the server every frame.
        /// </summary>
        public void UpdateServer() {
            switch(_state) {
                case GameState.UNINITIALIZED:
                    // Obvious hack for now.
                    SetState(GameState.LOBBY, true);
                    break;

                case GameState.LOBBY:
                    if (_players.Count == Configuration.NumberOfHumanPlayers) {
                        // Everyone has joined.
                        // TODO remove the fudges. For now, spawn all the players.
                        _communicator.SpawnShips(Configuration);
                        SetState(GameState.WAIT_FOR_SPAWN, true);
                    }
                    break;

                case GameState.WAIT_FOR_SPAWN:
                    if (_ships.Count == Configuration.NumberOfShips) {
                        // Enable the info panel UI.
                        _communicator.CmdEnableInfoPanel("Setup", "SetupInfoPanel");

                        // All the ships have spawned. Do some book-keeping, and move on to setup.
                        // TODO: simultaneous setup. For now, it's one player, one ship at a time.
                        buildTurnLists();
                        SetState(GameState.SETUP, true);
                        SetActiveShipByIndex(0, true);
                    }
                    break;

                case GameState.IN_PLAY:
                    updateServerInPlay();
                    break;

                default:
                    break;
            }
        }

        private void updateServerInPlay() {
            switch(_playPhase) {
                case PlayPhase.MOVING_SHIPS:
                    if (_movesToMake == 0) {
                        SetPlayPhase(PlayPhase.TAKING_ACTION, true);
                        _communicator.CmdEnableInfoPanel("Act", "ActionInfoPanel");
                        SetActiveShipByIndex(0, true);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the active ship to be the index in the current turn order.
        /// </summary>
        /// <param name="index">Ship index.</param>
        /// <param name="force">If true, always set the state regardless of whether it's currently active.</param>
        public void SetActiveShipByIndex(int index, bool force) {
            if (index != _activeShipIndex || force) {
                _activeShipIndex = index;
                var shipOrder = getTurnOrder();
                _activeShipID = shipOrder[index].ID;
                var ownerID = shipOrder[index].OwnerID;
                _communicator.NotifyActiveShip(ownerID, _activeShipID);
            }
        }

        private List<ShipController> getTurnOrder() {
            if ((_state == GameState.IN_PLAY) && (_playPhase == PlayPhase.SELECTING_ATTACK)) {
                return _setupOrderShips;
//                return _attackOrderShips;
            }
            else if (_state == GameState.SETUP) {
                return _setupOrderShips;
            }
            else {
                return _moveOrderShips;
            }
        }
        
        /// <summary>
        /// Called when a ship is above to move.
        /// </summary>
        /// <param name="move">The move.</param>
        public void BeginMove(ShipMoveInstance move) {
            var shipController = _ships.Find(s => s.ID == move.ShipID);
            shipController.BeginMove(move.Move);
        }

        /// <summary>
        ///  Called when a ship has finished its motion.
        /// </summary>
        /// <param name="shipID">The ship ID.</param>
        public void MoveFinished(int shipID) {
            #if VERBOSE
            Debug.LogFormat("GameController.MoveFinished({0})", shipID);
            #endif
            --_movesToMake;
            Debug.Assert(_movesToMake >= 0);
        }
        #endregion

        /// <summary>
        /// Sets the game state.
        /// </summary>
        /// <param name="newState">The new state.</param>
        /// <param name="notify">If true, all clients are notified of the change.</param>
        public void SetState(GameState newState, bool notify) {
            if (_state != newState) {
                #if VERBOSE
                Debug.LogFormat("GameController.SetState({0})", newState);
                #endif
                _state = newState;
                _activeShipIndex = 0;
                if (notify) {
                    _communicator.NotifyGameStateChange(_state);
                }
                onEnterState(newState, notify);
            }
        }

        private void onEnterState(GameState newState, bool notify) {
            switch(newState) {
                case GameState.LOBBY:
                    break;

                case GameState.SETUP: {
                        NetworkManager.GetComponent<NetworkManagerHUD>().showGUI = false;
                        TitleController.Stop();
                        var zoomControl = ControlPanel.GetComponentInChildren<ZoomButtonController>();
                        zoomControl.ResetZoom();
                    }
                    break;

                case GameState.IN_PLAY: {
                        _selectedMoves.Clear();
                        SetPlayPhase(PlayPhase.SELECTING_MOVES, notify);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the play phase.
        /// </summary>
        /// <param name="newPhase">The new play phase.</param>
        /// <param name="notify">If true, all clients are notified of the change.</param>
        public void SetPlayPhase(PlayPhase newPhase, bool notify) {
            if (_playPhase != newPhase) {
                #if VERBOSE
                Debug.LogFormat("GameController.SetPlayPhase({0})", newPhase);
                #endif
                onExitPhase(_playPhase);
                _playPhase = newPhase;
                _activeShipIndex = 0;
                if (notify) {
                    _communicator.NotifyPlayPhaseChange(_playPhase);
                }
                onEnterPhase(newPhase);
            }
        }

        private void onEnterPhase(PlayPhase newPhase) {
            #if VERBOSE
            Debug.LogFormat("GameController.onEnterPhase({0})", newPhase);
            #endif

            // TODO: Always reset zoom?
            var zoomControl = ControlPanel.GetComponentInChildren<ZoomButtonController>();
            zoomControl.ResetZoom();

            switch (newPhase) {
                case PlayPhase.SELECTING_MOVES:
                    // Reset the *local* action counter to 1.
                    foreach (var ship in _ships) {
                        ship.ResetActionsThisTurn();
                    }
                    break;

                case PlayPhase.TAKING_ACTION:
                    break;

                case PlayPhase.SELECTING_ATTACK:
                    _activeWeapon = 0;
                    _target = null;
                    break;
            }
        }

        private void onExitPhase(PlayPhase oldPhase) {
            #if VERBOSE
            Debug.LogFormat("GameController.onExitPhase({0})", oldPhase);
            #endif

            switch(oldPhase) {
                case PlayPhase.SELECTING_MOVES:
                    // Purge any map pins.
                    foreach (var pin in _mapPins) {
                        Destroy(pin);
                    }
                    _mapPins.Clear();
                    break;

                case PlayPhase.SELECTING_ATTACK:
                    clearAttackTargets();
                    break;
            }
        }

        private void buildTurnLists() {
            _moveOrderShips.Clear();
            _attackOrderShips.Clear();
            _moveOrderShips.AddRange(_ships);
            _moveOrderShips.Sort((s1, s2) => {
                // TODO: factor in captain's rank when all that stuff is in place.
                if (s1.Faction > s2.Faction) {
                    return 1;
                }

                if (s1.Faction < s2.Faction) {
                    return -1;
                }

                return 0;
            });
            _attackOrderShips.AddRange(_moveOrderShips);
            _attackOrderShips.Reverse();

            // For setup, interleave players.
            var p1Ships = _ships.FindAll(s => s.Faction == Faction.HUMANS);
            var p2Ships = _ships.FindAll(s => s.Faction == Faction.ALIENS);
            _setupOrderShips.Clear();
            for(int i = 0; i < p1Ships.Count; ++i) {
                _setupOrderShips.Add(p1Ships[i]);
                _setupOrderShips.Add(p2Ships[i]);
            }
        }

        private void performAttack() {
            clearAttackTargets();
            var targetObj = _target.GetComponent<VoidWarsObject>();
            _communicator.CmdPerformAttack(_activeShipID, targetObj.ID, _activeWeapon);
        }

        private void Start() {
            TitleController.SetText("VOID WARS()");
            var boardMesh = Board.GetComponent<MeshRenderer>();
            var bounds = boardMesh.bounds;
            _boardBounds = new Rect(bounds.min.x, bounds.min.z, bounds.size.x, bounds.size.z);
            var teleportGOs = GameObject.FindGameObjectsWithTag("Teleport");
            _teleportPoints = new Vector3[teleportGOs.Length];
            for(int i = 0; i < _teleportPoints.Length; ++i) {
                _teleportPoints[i] = teleportGOs[i].transform.position;
            }
        }

        #region Server Data
        private GameState _state = GameState.UNINITIALIZED;
        private PlayPhase _playPhase = PlayPhase.IDLE;
        private int _activeShipIndex = -1;
        private int _activeShipID = -1;
        private readonly List<ShipMoveInstance> _selectedMoves = new List<ShipMoveInstance>();
        private readonly List<GameObject> _mapPins = new List<GameObject>();
        private int _round;
        private int _movesToMake;
        private readonly List<NPCObject> _npcs = new List<NPCObject>();
        private Vector3[] _teleportPoints;
        #endregion Server Data

        private Communicator _communicator;
        private readonly List<PlayerServerRep> _players = new List<PlayerServerRep>();
        private readonly List<ShipController> _ships = new List<ShipController>();
        private readonly List<ShipController> _moveOrderShips = new List<ShipController>();
        private readonly List<ShipController> _attackOrderShips = new List<ShipController>();
        private readonly List<ShipController> _setupOrderShips = new List<ShipController>();
        private readonly List<TargetIndicator> _attackTargets = new List<TargetIndicator>();
        private Rect _boardBounds;
        private ShipController _activeShip;
        private int _actionCount;
        private int _activeWeapon;
        private GameObject _target;
        private bool _actionComplete;
        private ShipMoveInstance _selectedMove;
        private float _energyBeforeSelection;
        private float _energyAfterSelection;
        private float _drainBeforeSelection;
        private float _drainAfterSelection;

        private static readonly int[] s_p1StartPositions1 = new[] { 3 };
        private static readonly int[] s_p1StartPositions2 = new[] { 0, 1 };
        private static readonly int[] s_p2StartPositions1 = new[] { 2 };
        private static readonly int[] s_p2StartPositions2 = new[] { 4, 5 };
        private static readonly int[] s_startPositions2 = new[] { 3, 2 };
        private static readonly int[] s_startPositions4 = new[] { 0, 1, 4, 5 };
    }
}