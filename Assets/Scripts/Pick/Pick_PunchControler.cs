using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pick_PunchController : MonoBehaviour {

    public GameObject TrajectoryPointPrefeb;
    public int numOfTrajectoryPoints = 30;
    public float _Angle;//Góc sai khác của súng so vs goc ban đầu
    public int _NumTurn = 1;//Số turn của khẩu súng 
    private bool isPressed, isBallThrown;
    private float power = 5;
    private List<GameObject> trajectoryPoints;
    GameController _GameController;
    private float _scale = 0.1f;
    UIManager _UIManager;
    SoundController _SoundController;
    //---------------------------------------	
    void Start()
    {
        trajectoryPoints = new List<GameObject>();
        _GameController = FindObjectOfType<GameController>();
        _UIManager = FindObjectOfType<UIManager>();
        _SoundController = FindObjectOfType<SoundController>();
        isPressed = isBallThrown = false;
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            GameObject dot = (GameObject)Instantiate(TrajectoryPointPrefeb);
            dot.transform.localScale = new Vector3((numOfTrajectoryPoints - i * 0.6f) * _scale, (numOfTrajectoryPoints - i * 0.6f) * _scale, (numOfTrajectoryPoints - i * 0.6f) * _scale);
            dot.GetComponent<Renderer>().enabled = false;
            trajectoryPoints.Insert(i, dot);
        }
    }
    //---------------------------------------	
    void Update()
    {
        if (_GameController._CheckHit) return;
        if (_GameController._TypeGame == 0 && _GameController._GameState._IsEnemyStart) return;
        if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).y < _GameController.INood.transform.position.y || _UIManager._ListPopup[0].activeSelf) && !isPressed) return;
        if (isBallThrown)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            isPressed = true;

        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
            if (!isBallThrown)
            {
                throwBall();
            }
        }
        if (isPressed)
        {
            Vector3 vel = GetForceFrom(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));//Vector vận tốc ban đầu
            PickRotate(vel);
            setTrajectoryPoints(transform.position, vel);
        }
    }

    private void PickRotate(Vector3 vel)
    {
        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;//Góc tạo giữa vel và trục Ox
        Player player = _GameController._GameObj.GetComponent<Player>();
        angle += _Angle;
        if (player._Dir == Player.Dir.left)
        {
            if (angle % 360 > -90 && angle % 360 < 90)
            {
                transform.localScale = new Vector3(1, -1, 1);
            }
            else transform.localScale = new Vector3(1, 1, 1);
            angle = 360 - angle + 180;
        }
        else
        {
            if (angle % 360 > 90 || angle % 360 < -90)
            {
                transform.localScale = new Vector3(1, -1, 1);
            }
            else transform.localScale = new Vector3(1, 1, 1);
        }
        transform.eulerAngles = new Vector3(0, 0, angle);
    }
    //---------------------------------------	
    private void throwBall()
    {
        StartCoroutine(waitPlaySound());
        Animator ani = gameObject.GetComponentInChildren<Animator>();
        ani.SetBool("start", true);
      
        //	ball.GetComponent<Rigidbody2D>().useGravity = true;
        _NumTurn--;
        if (_NumTurn == 0)
        {
            isBallThrown = true;
            ///Xóa đường chỉ dẫn
            GameObject[] arrObj = GameObject.FindGameObjectsWithTag("Dot");
            for (int i = 0; i < arrObj.Length; i++) Destroy(arrObj[i]);
            //===================
            StartCoroutine(waitChangTurn());
        }
        _GameController._CheckHit = true;
        _UIManager.ResetImgPick();
    }
    public void throwBallEnemy(Vector3 target)
    {
        StartCoroutine(waitPlaySound());
        Vector3 vel = target - transform.position;
        PickRotate(vel);
        Animator ani = gameObject.GetComponentInChildren<Animator>();
        ani.SetBool("start", true);
        //	ball.GetComponent<Rigidbody2D>().useGravity = true;
        _NumTurn--;
        if (_NumTurn == 0)
        {
            isBallThrown = true;
            StartCoroutine(waitChangTurn());
        }
        _GameController._CheckHit = true;
        _UIManager.ResetImgPick();
    }
    IEnumerator waitPlaySound()
    {
        yield return new WaitForSeconds(0.7f);
        _SoundController.PlayPunchSound(true);
    }
    IEnumerator waitChangTurn()
    {
        yield return new WaitForSeconds(1.5f);
        GameController gameController = FindObjectOfType<GameController>();
        if (!gameController._GameState._IsChangding) gameController.ChangeTurn();
    }
    //---------------------------------------	
    private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
    {
        power = 5;
        while (((new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power).sqrMagnitude < 30f) power += 0.05f;
        while (((new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power).sqrMagnitude > 35f) power -= 0.05f;
        return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power;//*ball.rigidbody.mass;
    }
    //---------------------------------------	
    // It displays projectile trajectory path
    //---------------------------------------	
    void setTrajectoryPoints(Vector3 pStartPosition, Vector3 pVelocity)
    {
        float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));
        float fTime = 0;

        fTime += 0.1f;
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 2);
            trajectoryPoints[i].transform.position = pos;
            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.1f;
        }
    }
}
