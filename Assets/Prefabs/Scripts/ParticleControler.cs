using UnityEngine;
using System.Collections;

public class ParticleControler : MonoBehaviour {
    public string _SortingName = "Middleground";
    public int _SortingOrder=4;
	// Use this for initialization
	void Start () {
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = _SortingName;
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = _SortingOrder;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
