using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pick_CarotController : MonoBehaviour {

    public GameObject TrajectoryPointPrefeb;
    public GameObject[] _ListCarrot;//Luu trữ list carrot để thực hiện nổ
    public int numOfTrajectoryPoints = 30;
    public float _Angle;//Góc sai khác của súng so vs goc ban đầu
    public GameObject _Explosion;
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
        if (isBallThrown) ActiveOutMap();
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

    //private void createText()
    //{
    //    numText = (GameObject)Instantiate(_NumberText);

    //    Vector3 pos = transform.position;
    //    pos.z = 1;
    //    numText.transform.position = pos;
    //}

    //---------------------------------------	
    private void throwBall()
    {
        if (_GameController._GameObj.tag == "Player") _GameController._ListStatePickPlayer[8].Ammo -= 1; else _GameController._ListStatePickEnemy[8].Ammo -= 1;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        _ListCarrot[0].GetComponent<CircleCollider2D>().enabled = true;
        //	ball.GetComponent<Rigidbody2D>().useGravity = true;
        gameObject.GetComponent<Rigidbody2D>().AddForce(GetForceFrom(_posUp, _posDown), ForceMode2D.Impulse);
        isBallThrown = true;
        ///Xóa đường chỉ dẫn
        GameObject[] arrObj = GameObject.FindGameObjectsWithTag("Dot");
        for (int i = 0; i < arrObj.Length; i++) Destroy(arrObj[i]);
        _GameController._CheckHit = true;
        _GameController._StopTime = true;
        _UIManager.ResetImgPick();
    }
    public void throwBallEnemy(Vector3 vel)
    {
        if (_GameController._GameObj.tag == "Player") _GameController._ListStatePickPlayer[8].Ammo -= 1; else _GameController._ListStatePickEnemy[8].Ammo -= 1;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        _ListCarrot[0].GetComponent<CircleCollider2D>().enabled = true;
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
    private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos,float power)
    {
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
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Map")
        {
            CollisionActive();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            Player player = other.GetComponent<Player>();
            if (player._IsCurrent) return;
            CollisionActive();
        }
    }
    /// <summary>
    /// Thực hiện xử lý khi va chạm
    /// </summary>
    private void CollisionActive()
    {
        Instantiate(_Explosion, transform.position, Quaternion.identity);
        _ListCarrot[0].SetActive(false);
        ((GameObject)Instantiate(_ListCarrot[1], transform.position, Quaternion.identity)).GetComponent<Rigidbody2D>().AddForce(GetForceFrom(transform.position, new Vector3(transform.position.x+1, transform.position.y+1, transform.position.z), 3), ForceMode2D.Impulse);
        ((GameObject)Instantiate(_ListCarrot[2], transform.position, Quaternion.identity)).GetComponent<Rigidbody2D>().AddForce(GetForceFrom(transform.position, new Vector3(transform.position.x+1, transform.position.y+2, transform.position.z), 1.5f), ForceMode2D.Impulse);
        ((GameObject)Instantiate(_ListCarrot[3], transform.position, Quaternion.identity)).GetComponent<Rigidbody2D>().AddForce(GetForceFrom(transform.position, new Vector3(transform.position.x-1, transform.position.y+1, transform.position.z), 3), ForceMode2D.Impulse);
        ((GameObject)Instantiate(_ListCarrot[4], transform.position, Quaternion.identity)).GetComponent<Rigidbody2D>().AddForce(GetForceFrom(transform.position, new Vector3(transform.position.x-1, transform.position.y+2, transform.position.z), 1.5f), ForceMode2D.Impulse);
    }
    private void ActiveOutMap()
    {
        GameObject other = GameObject.FindGameObjectWithTag("Map_Front");
        float xMax = other.transform.position.x + other.GetComponent<BoxCollider2D>().size.x * other.transform.localScale.x;
        float xMin = other.transform.position.x - other.GetComponent<BoxCollider2D>().size.x * other.transform.localScale.x;
        float yMin = other.transform.position.y - other.GetComponent<BoxCollider2D>().size.y * other.transform.localScale.y;
        if (gameObject.transform.position.x > xMax || gameObject.transform.position.x < xMin || gameObject.transform.position.y < yMin)
        {
            GameController gameController = FindObjectOfType<GameController>();

            if (!gameController._GameState._IsChangding) gameController.ChangeTurn();
           
            Destroy(gameObject);
        }  
    }
}
