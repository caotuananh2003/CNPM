using UnityEngine;
using System.Collections;

public class PanelExitGame : MonoBehaviour {

    UIManager _UIManager;
	// Use this for initialization
	void Start () {
        _UIManager = FindObjectOfType<UIManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void btnOke()
    {
        _UIManager.ShowPanelStartGame();
        _UIManager.ShowPanelChoosePlayer();
        _UIManager.ShowPanelChooseMapAndNumPlayer();
        _UIManager.ShowPanelLevel();
        _UIManager.HidePanelExitGame();
    }
    public void btnNo()
    {
        _UIManager.HidePanelExitGame();
    }
}