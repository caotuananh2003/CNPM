using UnityEngine;
using System.Collections;

public class ParticleSkullSmallControler : MonoBehaviour {

    public float _Speed;
    public float _a;
    Vector3 _posStart;
	// Use this for initialization
	void Start () {
        _posStart = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.up * Time.deltaTime * _Speed,Space.World);
        float a=GetComponent<SpriteRenderer>().color.a;
        float b=GetComponent<SpriteRenderer>().color.b;
        float r=GetComponent<SpriteRenderer>().color.r;
        float g=GetComponent<SpriteRenderer>().color.g;
        GetComponent<SpriteRenderer>().color = new Color(r, g, b, a - _a);
        if (gameObject.transform.position.y-_posStart.y>2)
        {
            Destroy(gameObject);
        }
	}
}
