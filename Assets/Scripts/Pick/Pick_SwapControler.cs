using UnityEngine;
using System.Collections;

public class Pick_SwapController : MonoBehaviour {

    public float _Speed = 3;
    GameController _GameController;
	// Use this for initialization
	void Start () {
        _GameController = FindObjectOfType<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(transform.position, Vector3.forward, Time.deltaTime * _Speed);
	}
}
