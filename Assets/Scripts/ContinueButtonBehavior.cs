using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContinueButtonBehavior : MonoBehaviour
{
    public UnityEngine.UI.Button continueButton;
    public TextMeshProUGUI continueButtonText;

    // Start is called before the first frame update
    private void Start()
    {
        var savedSceneName = CustomSceneManager.Instance.GetSavedSceneName();
        continueButton.interactable = savedSceneName != null;
        // grey out the button if there is no saved scene
        if (!continueButton.interactable)
        {
            continueButton.transition = Selectable.Transition.None;
            continueButtonText.text = "继续游戏<size=50%> (暂无存档)</size>";
        }
    }
}