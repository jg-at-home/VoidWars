using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the Setup info panel.
/// </summary>
public class SetupPanelController : MonoBehaviour {
    [SerializeField]
    private Text _infoText;

    [SerializeField]
    private Button _doneButton;

    /// <summary>
    /// Sets the info text.
    /// </summary>
    /// <param name="text">The text.</param>
    public void SetInfoText(string text) {
        _infoText.text = text;
    }

    /// <summary>
    /// Enable or disable the 'done' button.
    /// </summary>
    /// <param name="enable">If true, enable the button.</param>
    public void EnableDoneButton(bool enable) {
        _doneButton.interactable = enable;
    }

    /// <summary>
    /// Called when the Done button is clicked.
    /// </summary>
    public void OnDoneButtonClicked() {
        Debug.Log("You clicked 'Done'");
        // TODO: stuff.
    }
}
