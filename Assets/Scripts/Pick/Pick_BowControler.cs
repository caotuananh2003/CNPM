using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pick_BowController : MonoBehaviour {

    public GameObject TrajectoryPointPrefeb;
    public GameObject arrow;
    public GameObject[] BallPrefb;
    public int numOfTrajectoryPoints = 30;
    public float _Angle;//Góc sai khác của súng so vs goc ban đầu
    private GameObject[] ball;
    private bool isPressed, isBallThrown;
    private float power = 5;
    private List<GameObject> trajectoryPoints;
    GameController _GameController;
    private float _scale = 0.1f;
    private Vector3 _posDown, _posUp;//Điểm bắt đầu và kết thúc kéo tay lấy lực
    UIManager _UIManager;
    //---------------------------------------	
    void Start()
    {
        trajectoryPoints = new List<GameObject>();
        _GameController = FindObjectOfType<GameController>();
        _UIManager = FindObjectOfType<UIManager>();
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
            _posDown = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isPressed = true;
            if (ball==null)
                createBall();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
            _posUp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!isBallThrown)
            {
                throwBall();
            }
        }
        if (isPressed)
        {

            Vector3 vel = GetForceFrom(Camera.main.ScreenToWorldPoint(Input.mousePosition), _posDown);//Vector vận tốc ban đầu
            float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;//Góc tạo giữa vel và trục Ox
            Player player = _GameController._GameObj.GetComponent<Player>();
            angle += _Angle;
            if (player._Dir == Player.Dir.left)
            {
                angle = 360 - angle+180;
            }
            transform.eulerAngles = new Vector3(0, 0, angle);
            setTrajectoryPoints(transform.position, vel / BallPrefb[0].GetComponent<Rigidbody2D>().mass);
        }
    }
    //---------------------------------------	
    // When ball is thrown, it will create new ball
    //---------------------------------------	
    private void createBall()
    {
        ball = new GameObject[3];
        ball[0] = (GameObject)Instantiate(arrow);
        ball[1] = (GameObject)Instantiate(arrow);
        ball[2] = (GameObject)Instantiate(arrow);
        ball[0].transform.position = BallPrefb[0].transform.position;
        ball[1].transform.position = BallPrefb[1].transform.position;
        ball[2].transform.position = BallPrefb[2].transform.position;
        ball[0].SetActive(false);
        ball[1].SetActive(false);
        ball[2].SetActive(false);
    }
    //---------------------------------------	
    private void throwBall()
    {
        if (_GameController._GameObj.tag == "Player") _GameController._ListStatePickPlayer[11].Ammo -= 1; else _GameController._ListStatePickEnemy[11].Ammo -= 1;
        ball[0].SetActive(true); ball[1].SetActive(true); ball[2].SetActive(true);
        BallPrefb[0].SetActive(false); BallPrefb[1].SetActive(false); BallPrefb[2].SetActive(false);
        //	ball.GetComponent<Rigidbody2D>().useGravity = true;
        ball[0].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(_posUp,_posDown),ForceMode2D.Impulse);
        ball[1].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(_posUp*1.01f, _posDown), ForceMode2D.Impulse);
        ball[2].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(_posUp*1.02f, _posDown), ForceMode2D.Impulse);
        isBallThrown = true;
        ball[0].GetComponent<BulletController>().enabled = true;
        ball[1].GetComponent<BulletController>().enabled = true;
        ball[2].GetComponent<BulletController>().enabled = true;
        ball[0].GetComponent<Rigidbody2D>().gravityScale = 1;
        ball[1].GetComponent<Rigidbody2D>().gravityScale = 1;
        ball[2].GetComponent<Rigidbody2D>().gravityScale = 1;
        ///Xóa đường chỉ dẫn
        GameObject[] arrObj = GameObject.FindGameObjectsWithTag("Dot");
        for (int i = 0; i < arrObj.Length; i++) Destroy(arrObj[i]);
        _GameController._CheckHit = true;
        _GameController._StopTime = true;
        _UIManager.ResetImgPick();
    }
    /// <summary>
    /// Phương thức bắn súng cho địch
    /// </summary>
    public void throwBallEnemy(Vector3 vel)
    {
        if (ball==null)
        {
            createBall();
        }
        if (_GameController._GameObj.tag == "Player") _GameController._ListStatePickPlayer[11].Ammo -= 1; else _GameController._ListStatePickEnemy[11].Ammo -= 1;
        ball[0].SetActive(true); ball[1].SetActive(true); ball[2].SetActive(true);
        BallPrefb[0].SetActive(false); BallPrefb[1].SetActive(false); BallPrefb[2].SetActive(false);
        //	ball.GetComponent<Rigidbody2D>().useGravity = true;
        ball[0].GetComponent<Rigidbody2D>().AddForce(vel, ForceMode2D.Impulse);
        ball[1].GetComponent<Rigidbody2D>().AddForce(vel*1.02f, ForceMode2D.Impulse);
        ball[2].GetComponent<Rigidbody2D>().AddForce(vel*1.04f, ForceMode2D.Impulse);
        isBallThrown = true;
        ball[0].GetComponent<BulletController>().enabled = true;
        ball[1].GetComponent<BulletController>().enabled = true;
        ball[2].GetComponent<BulletController>().enabled = true;
        ball[0].GetComponent<Rigidbody2D>().gravityScale = 1;
        ball[1].GetComponent<Rigidbody2D>().gravityScale = 1;
        ball[2].GetComponent<Rigidbody2D>().gravityScale = 1;
        ///Xóa đường chỉ dẫn
        GameObject[] arrObj = GameObject.FindGameObjectsWithTag("Dot");
        for (int i = 0; i < arrObj.Length; i++) Destroy(arrObj[i]);
        _GameController._CheckHit = true;
        _GameController._StopTime = true;
        _UIManager.ResetImgPick();
    }
    //---------------------------------------	
    private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
    {
        power = 5;
        while (((new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power).sqrMagnitude > 450f) power -= 0.05f;
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
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * fTime * fTime / 2.0f);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 2);
            trajectoryPoints[i].transform.position = pos;
            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.1f;
        }
    }
}
