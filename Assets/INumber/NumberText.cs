using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;


public class NumberText : MonoBehaviour
{

    public Alignments Alignment = Alignments.CENTER;
    public PivotAlignments PivotHorizontalAlignment = PivotAlignments.CENTER;

    public GameObject NumberItem;
    public int _NumStart = 3;

    [Range(1, 10)]
    public int LengthNumber = 1;// Số chữ số
    public float Distance = 5;// Khoảng cách giữa 2 số
    public List<Sprite> _ListSpriteNumber = new List<Sprite>();

    private string _NUMBER;

    public int NUMBER
    {
        set
        {
            _NUMBER = GetNumberDec(value, LengthNumber);
            SetNumber();
        }
    }

    /// <summary>
    /// Chiều cao của panel số
    /// </summary>
    private float HEIGHT
    {
        get { return transform.GetComponent<RectTransform>().sizeDelta.y; }
    }

    private void SetNumber()
    {
        ClearAllNumber();

        Vector2 positionPanel = transform.GetComponent<RectTransform>().position;
        Vector2 sizePanel = transform.GetComponent<RectTransform>().sizeDelta;
        if (PivotHorizontalAlignment == PivotAlignments.LEFT)
            positionPanel = new Vector2(positionPanel.x + sizePanel.x / 2, positionPanel.y);
        else if (PivotHorizontalAlignment == PivotAlignments.RIGHT)
            positionPanel = new Vector2(positionPanel.x - sizePanel.x / 2, positionPanel.y);

        float xStart = 0;

        if (Alignment == Alignments.CENTER)
            xStart = positionPanel.x - GetSizeNumbers(_NUMBER, Distance) / 2;
        else if (Alignment == Alignments.LEFT)
            xStart = positionPanel.x - sizePanel.x / 2;
        else if (Alignment == Alignments.RIGHT)
            xStart = positionPanel.x + sizePanel.x / 2 - GetSizeNumbers(_NUMBER, Distance);

        foreach (var ch in _NUMBER)
        {
            var pos = new Vector3(xStart, positionPanel.y, 0);
            InitNumber(ch, pos);
            xStart += GetSizeNumber(ch).x + Distance;
        }
    }

    /// <summary>
    /// Khởi tạo ra 1 số tại 1 vị trí
    /// </summary>
    private void InitNumber(char number, Vector3 postion)
    {
        NumberItem.GetComponent<Image>().sprite = _ListSpriteNumber[int.Parse(number.ToString())];
        NumberItem.GetComponent<RectTransform>().sizeDelta = GetSizeNumber(number);

        var obj = Instantiate(NumberItem, postion, new Quaternion()) as GameObject;
        obj.transform.SetParent(transform);
    }

    /// <summary>
    /// Lấy size của 1 số theo chiều cao HEIGHT định sẵn
    /// </summary>
    private Vector2 GetSizeNumber(char number)
    {
        var sprite = _ListSpriteNumber[int.Parse(number + "")];
        return new Vector2(HEIGHT * (sprite.textureRect.size.x / sprite.textureRect.size.y), HEIGHT);
    }

    /// <summary>
    /// Lấy chiều dài của dãy số
    /// </summary>
    private float GetSizeNumbers(string numbers, float distance)
    {
        float width = 0;
        foreach (var ch in numbers)
        {
            var sizeNumber = GetSizeNumber(ch);
            width += sizeNumber.x;
            width += distance;
        }
        width -= distance;// Trừ đi phát cuối nó cộng
        return width;
    }

    private string GetNumberDec(int number, int lengthNumber)
    {
        string num = number.ToString();
        while (num.Length < lengthNumber)
            num = "0" + num;
        return num;
    }

    private void ClearAllNumber()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    #region TEST AREA
    public float _TimeDelay = 0.2f;
    public float _StepNumberTest = 2;
    
    private NumberText numberText;

    void Start()
    {
        numberText = gameObject.GetComponent<NumberText>();
        numberText.NUMBER = Convert.ToInt32(_NumStart);
    }
    /// <summary>
    /// Xét text cho 1 player
    /// </summary>
    public void SetNumberText(int num)
    {
        numberText.NUMBER = Convert.ToInt32(num);
    }
    #endregion

    public enum Alignments
    {
        LEFT,
        CENTER,
        RIGHT,
    }

    public enum PivotAlignments
    {
        LEFT,
        CENTER,
        RIGHT,
    }
}
