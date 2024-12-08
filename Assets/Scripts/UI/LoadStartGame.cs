using UnityEngine;
using System.Collections;
using System;

public class LoadStartGame : MonoBehaviour {

    public GameObject PanelStartGame;
    public float _Speed = 2;//Tốc độ quay
    GameController _GameController;
    int _numTime=0;
    float _timeCount = 0;
    NumberText _lbNum;

	// Use this for initialization
    void Awake()
    {
        string sound = ReadWriteFileText.GetStringFromPrefab(Data._LinkSound);
        if (sound=="")
        {
            _GameController = FindObjectOfType<GameController>();
            _GameController._GameState._Music = true;
            _GameController._GameState._Sound = true;
        }
        else
        {
            string[] arrSound = sound.Split('#');
            _GameController._GameState._Music = bool.Parse(arrSound[0]);
            _GameController._GameState._Sound = bool.Parse(arrSound[1]);
        }

    }
    void Start()
    {
        _lbNum = gameObject.GetComponentInChildren<NumberText>();
       // _lbNum.SetNumberText(Convert.ToInt32(0));
	}
	
	// Update is called once per frame
	void Update () {
        _timeCount += Time.deltaTime;
        if (_timeCount>0.03f)
        {
            _timeCount = 0;
            if (_lbNum == null) return;
            _numTime++;
            _lbNum.SetNumberText(Convert.ToInt32(_numTime));
            if (_numTime >= 110)
            {
                PanelStartGame.SetActive(true);
                gameObject.SetActive(false);
            }
        }
	}
}
