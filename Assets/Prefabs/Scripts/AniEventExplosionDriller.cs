using UnityEngine;
using System.Collections;

public class AniEventExplosionDriller : MonoBehaviour {
    public GameObject[] _ListExplosion;
    public void ShowExplosion()
    {
        for (int i = 0; i < _ListExplosion.Length; i++)
        {
            _ListExplosion[i].SetActive(true);
        }
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
