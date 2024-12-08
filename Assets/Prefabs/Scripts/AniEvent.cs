using UnityEngine;
using System.Collections;

public class AniEvent : MonoBehaviour {

    private Animator _anim;
	// Use this for initialization
	void Start () {
        _anim = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void Pick_ShotGun_Off()
    {
        _anim.Play("Hotgun_Idle");
    }
    public void Pick_Bazooka_Off()
    {
        _anim.Play("Bazooka_Idle");
    }
    public void Player_Idle()
    {
        _anim.Play("Player_Idle");
    }
    public void Enemy_Idle()
    {
        _anim.Play("Enemy_Idle");
    }
    public void Pick_Punch()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }
}
