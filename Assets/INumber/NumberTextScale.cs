using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumberTextScale : MonoBehaviour {

    public Sprite[] _ListSpriteNumber;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void SetNumText1(int num,Image[] _listImage)
    {
        _listImage[0].sprite = _ListSpriteNumber[num];
    }
    public void SetNumText2(int num, Image[] _listImage)
    {
        int a = num / 10;
        int b = num % 10;
        _listImage[0].sprite = _ListSpriteNumber[a];
        _listImage[1].sprite = _ListSpriteNumber[b];
    }
    public void SetNumText3(int num, Image[] _listImage)
    {
        int a = num / 100;
        int ma = num % 100;
        int b = ma / 10;
        int c = ma % 10;
        _listImage[0].sprite = _ListSpriteNumber[a];
        _listImage[1].sprite = _ListSpriteNumber[b];
        _listImage[2].sprite = _ListSpriteNumber[c];
    }
}
