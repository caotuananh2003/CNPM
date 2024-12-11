using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pick_Grenade_PoisonController : MonoBehaviour {

    public GameObject TrajectoryPointPrefeb;
    public int numOfTrajectoryPoints = 30;
    public float _Angle;//Góc sai khác của súng so vs goc ban đầu
  //  public GameObject _Explosion;
    private bool isPressed, isBallThrown;
    private float power = 5;
    private List<GameObject> trajectoryPoints;
    GameController _GameController;
    private float _scale = 0.1f;
    private int _timeDestroy = 3;
    private float _timeCount = 0;
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
        if (Input.GetMouseButtonDown(0))
        {
            _posDown = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isPressed = true;
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
                angle = 360 - angle;
            }
            transform.eulerAngles = new Vector3(0, 0, angle);
            setTrajectoryPoints(transform.position, vel / gameObject.GetComponent<Rigidbody2D>().mass);
        }

    }

    //---------------------------------------	
    private void throwBall()
    {
        if (_GameController._GameObj.tag == "Player") _GameController._ListStatePickPlayer[13].Ammo -= 1; else _GameController._ListStatePickEnemy[13].Ammo -= 1;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        //	ball.GetComponent<Rigidbody2D>().useGravity = true;
        gameObject.GetComponentInChildren<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<Rigidbody2D>().AddForce(GetForceFrom(_posUp, _posDown), ForceMode2D.Impulse);
        isBallThrown = true;
        ///Xóa đường chỉ dẫn
        GameObject[] arrObj = GameObject.FindGameObjectsWithTag("Dot");
        for (int i = 0; i < arrObj.Length; i++) Destroy(arrObj[i]);
        _GameController._CheckHit = true;
        _GameController._StopTime = true;
        _UIManager.ResetImgPick();
    }
    public void throwBallAIEnemy(Vector3 vel)
    {
        if (_GameController._GameObj.tag == "Player") _GameController._ListStatePickPlayer[13].Ammo -= 1; else _GameController._ListStatePickEnemy[13].Ammo -= 1;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        gameObject.GetComponentInChildren<BoxCollider2D>().enabled = true;
        //	ball.GetComponent<Rigidbody2D>().useGravity = true;
        gameObject.GetComponent<Rigidbody2D>().AddForce(vel, ForceMode2D.Impulse);
        isBallThrown = true;
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
