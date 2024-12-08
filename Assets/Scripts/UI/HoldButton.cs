using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class HoldButton : MonoBehaviour, IEventSystemHandler
{
    GameController _GameController;
    SoundController _SoundController;
    SoundPlayerController _SoundPlayer;
    void Start()
    {
        _GameController = FindObjectOfType<GameController>();
        _SoundController = FindObjectOfType<SoundController>();
        _SoundPlayer = FindObjectOfType<SoundPlayerController>();
    }
    public void MoveRight_On(BaseEventData eventData)
    {
        if (_GameController._StopTime) return;
        if (_GameController._GameObj.tag == "Enemy" && _GameController._TypeGame == 0) return;
        _SoundController.PlayClickSound(true);
        Player player = _GameController._GameObj.GetComponent<Player>();
        player._MovePlayer = Player.MovePlayer.MoveRigh;
    }
    public void MoveRight_Off(BaseEventData eventData)
    {
        Player player = _GameController._GameObj.GetComponent<Player>();
        player._MovePlayer = Player.MovePlayer.None;
    }
    //=================
    public void MoveLeft_On(BaseEventData eventData)
    {
        if (_GameController._StopTime) return;
        if (_GameController._GameObj.tag == "Enemy" && _GameController._TypeGame == 0) return;
        _SoundController.PlayClickSound(true);
        Player player = _GameController._GameObj.GetComponent<Player>();
        player._MovePlayer = Player.MovePlayer.MoveLeft;
    }
    public void MoveLeft_Off(BaseEventData eventData)
    {
        Player player = _GameController._GameObj.GetComponent<Player>();
        player._MovePlayer = Player.MovePlayer.None;
    }
    //=================
    public void JumpLeft_On(BaseEventData eventData)
    {
        if (_GameController._StopTime) return;
        if (_GameController._GameObj.tag == "Enemy" && _GameController._TypeGame == 0) return;
        _SoundController.PlayClickSound(true);
        _SoundPlayer.PlayJump(true);
        Player player = _GameController._GameObj.GetComponent<Player>();
        player._MovePlayer = Player.MovePlayer.JumpLeft;
    }
    //=================
    public void JumpRight_On(BaseEventData eventData)
    {
        if (_GameController._StopTime) return;
        if (_GameController._GameObj.tag == "Enemy" && _GameController._TypeGame == 0) return;
        _SoundController.PlayClickSound(true);
        _SoundPlayer.PlayJump(true);
        Player player = _GameController._GameObj.GetComponent<Player>();
        player._MovePlayer = Player.MovePlayer.JumpRight;
    }
}