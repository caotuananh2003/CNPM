using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShotGunController : MonoBehaviour
{

    public GameObject TrajectoryPointPrefeb; // Prefab để vẽ đường bay của đạn
    public GameObject BallPrefb; // Prefab của đạn
    public int numOfTrajectoryPoints = 30; // Số điểm hiển thị để mô phỏng đường bay của đạn
    public int _NumShot; // Số nòng của súng, có thể là 3 hoặc 5
    public GameObject[] _listShot; // Danh sách các vị trí nòng súng
    public float _Angle; // Góc lệch của súng so với hướng ban đầu
    public int _NumTurn = 1; // Số lượt bắn của súng
    private GameObject[] ball; // Mảng chứa các viên đạn
    private bool isPressed, isBallThrown; // Trạng thái khi người chơi nhấn chuột và khi đạn đã được bắn
    private float power = 5; // Lực bắn của súng
    private List<GameObject> trajectoryPoints; // Danh sách các điểm mô phỏng đường bay của đạn
    GameController _GameController; // Điều khiển trò chơi
    private float _scale = 0.1f; // Hệ số tỷ lệ của các điểm đường bay
    private Animator _anim; // Bộ điều khiển hoạt ảnh
    UIManager _UIManager; // Quản lý giao diện người dùng
    SoundController _SoundController; // Điều khiển âm thanh

    //---------------------------------------	
    void Start()
    {
        // Khởi tạo các biến và các đối tượng cần thiết khi bắt đầu game
        trajectoryPoints = new List<GameObject>();
        _GameController = FindObjectOfType<GameController>(); // Tìm và tham chiếu đến đối tượng GameController
        _anim = gameObject.GetComponentInChildren<Animator>(); // Lấy animator của đối tượng con
        _UIManager = FindObjectOfType<UIManager>(); // Tìm UIManager
        _SoundController = FindObjectOfType<SoundController>(); // Tìm SoundController
        isPressed = isBallThrown = false; // Đặt các trạng thái ban đầu cho isPressed và isBallThrown

        // Tạo các điểm để mô phỏng đường bay của đạn
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            GameObject dot = (GameObject)Instantiate(TrajectoryPointPrefeb);
            // Đặt tỷ lệ cho các điểm
            dot.transform.localScale = new Vector3((numOfTrajectoryPoints - i * 0.6f) * _scale, (numOfTrajectoryPoints - i * 0.6f) * _scale, (numOfTrajectoryPoints - i * 0.6f) * _scale);
            dot.GetComponent<Renderer>().enabled = false; // Tắt renderer để không hiển thị ngay lập tức
            trajectoryPoints.Insert(i, dot); // Thêm các điểm vào danh sách
        }
    }

    //---------------------------------------	
    void Update()
    {
        // Nếu game đang tạm dừng hoặc không phải lượt của người chơi thì return
        if (_GameController._StopTime) return;
        if (_GameController._TypeGame == 0 && _GameController._GameState._IsEnemyStart) return;

        // Nếu vị trí chuột ở dưới nòng súng hoặc giao diện đang bật, không thực hiện gì
        if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).y < _GameController.INood.transform.position.y || _UIManager._ListPopup[0].activeSelf) && !isPressed) return;
        if (isBallThrown) return; // Nếu đã bắn đạn thì không làm gì thêm

        // Xử lý nhấn chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            isPressed = true;
            if (ball == null)
                createBall(); // Nếu chưa có đạn, tạo đạn mới
        }
        // Xử lý khi nhả chuột trái
        else if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
            if (!isBallThrown)
            {
                throwBall(); // Nếu chưa bắn đạn, thực hiện bắn đạn
            }
        }

        // Nếu đang nhấn chuột, cập nhật hướng bắn và hiển thị đường bay của đạn
        if (isPressed)
        {
            Vector3 vel = GetForceFrom(ball[0].transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)); // Lấy vector vận tốc
            PickRotate(vel); // Xoay hướng súng dựa vào vận tốc
            setTrajectoryPoints(transform.position, vel); // Hiển thị đường bay của đạn
        }
    }

    // Hàm xoay hướng súng dựa trên vận tốc bắn
    private void PickRotate(Vector3 vel)
    {
        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg; // Tính góc giữa vận tốc và trục Ox
        Player player = _GameController._GameObj.GetComponent<Player>();
        angle += _Angle; // Thêm góc lệch của súng

        // Xử lý hướng của súng dựa trên hướng của người chơi
        if (player._Dir == Player.Dir.left)
        {
            if (angle % 360 > -90 && angle % 360 < 90)
            {
                transform.localScale = new Vector3(1, -1, 1); // Lật hướng nòng súng
            }
            else transform.localScale = new Vector3(1, 1, 1);
            angle = 360 - angle + 180; // Điều chỉnh góc cho bên trái
        }
        else
        {
            if (angle % 360 > 90 || angle % 360 < -90)
            {
                transform.localScale = new Vector3(1, -1, 1);
            }
            else transform.localScale = new Vector3(1, 1, 1);
        }
        transform.eulerAngles = new Vector3(0, 0, angle); // Cập nhật góc xoay của súng
    }


    //---------------------------------------	
    // When ball is thrown, it will create new ball
    //---------------------------------------	
    private void createBall()
    {
        if (_NumShot == 3)
        {
            ball = new GameObject[3];
        }
        else
        {
            ball = new GameObject[5];
        }

        // Đoạn này đang suy nghĩ dùng vòng for để xử lý.
        // Đang chưa hiểu rõ cách hoạy động của _Turn trong script BulletCollision2D
        ball[0] = (GameObject)Instantiate(BallPrefb);
        BulletCollision2D bulletCo = ball[0].GetComponentInChildren<BulletCollision2D>(); bulletCo._Turn = _NumTurn;

        ball[1] = (GameObject)Instantiate(BallPrefb);
        BulletCollision2D bulletC1 = ball[1].GetComponentInChildren<BulletCollision2D>(); bulletC1._Turn = _NumTurn;

        ball[2] = (GameObject)Instantiate(BallPrefb);
        BulletCollision2D bulletC2 = ball[2].GetComponentInChildren<BulletCollision2D>(); bulletC2._Turn = _NumTurn;


        Vector3 pos = transform.position;
        pos.z = 1;

        ball[0].transform.position = ball[1].transform.position = ball[2].transform.position = pos;

        ball[0].SetActive(false); ball[1].SetActive(false); ball[2].SetActive(false);

        if (_NumShot == 5)
        {
            ball[3] = (GameObject)Instantiate(BallPrefb);
            BulletCollision2D bulletC3 = ball[3].GetComponentInChildren<BulletCollision2D>(); bulletC3._Turn = _NumTurn;

            ball[4] = (GameObject)Instantiate(BallPrefb);
            BulletCollision2D bulletC4 = ball[4].GetComponentInChildren<BulletCollision2D>(); bulletC4._Turn = _NumTurn;

            ball[3].transform.position = ball[4].transform.position = pos;

            ball[3].SetActive(false); ball[4].SetActive(false);
        }
    }
    //---------------------------------------	
    private void throwBall()
    {
        _SoundController.PlayShotgun_shotSound(true); // Âm thanh bắn
        _anim.Play("Hotgun"); // Anim bắn

        // Kích hoạt 3 viên đạn
        ball[0].SetActive(true); ball[1].SetActive(true); ball[2].SetActive(true);

        /*
        Áp dụng lực cho từng viên đạn thông qua thành phần Rigidbody2D.
        Hàm AddForce tính toán lực từ vị trí hiện tại của viên đạn đến vị trímục tiêu (_listShot), sau đó áp dụng lực để viên đạn được bắn ra.
        ForceMode2D.Impulse đảm bảo rằng lực này được áp dụng như một cú đẩy ngắn hạn (impulse).
        */
        ball[0].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(ball[0].transform.position, _listShot[0].transform.position), ForceMode2D.Impulse);
        ball[1].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(ball[1].transform.position, _listShot[1].transform.position), ForceMode2D.Impulse);
        ball[2].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(ball[2].transform.position, _listShot[2].transform.position), ForceMode2D.Impulse);

        // Nếu số đạn là 5 thì tiến hành kích hoạt nốt 2 viên còn lại
        if (_NumShot == 5)
        {
            ball[3].SetActive(true); ball[4].SetActive(true);
            ball[3].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(ball[3].transform.position, _listShot[3].transform.position), ForceMode2D.Impulse);
            ball[4].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(ball[4].transform.position, _listShot[4].transform.position), ForceMode2D.Impulse);
        }

        // Đoạn này định áp dụng trọng lực để cho đạn rơi theo hình parabol, nhưng tạm bỏ qua để cho đạn bay thẳng.
        //ball.GetComponent<Rigidbody2D>().useGravity = true;

        // Đặt cờ _CheckHit trong GameController thành true, báo hiệu rằng viên đạn đã được bắn ra và trò chơi sẽ bắt đầu kiểm tra va chạm của đạn.
        _GameController._CheckHit = true;

        //Giảm số lượt của người chơi
        _NumTurn--;

        // Xóa tham chiếu tới mảng ball để chuẩn bị cho những viên đạn mới trong lượt tiếp theo
        ball = null;

        // Nếu vẫn còn lượt chơi, các điểm chỉ dẫn quỹ đạo (trajectory points) sẽ bị tắt bằng cách
        // vô hiệu hóa thành phần Renderer để chúng không hiển thị nữa.
        if (_NumTurn != 0)
        {
            for (int i = 0; i < numOfTrajectoryPoints; i++)
            {
                trajectoryPoints[i].GetComponent<Renderer>().enabled = false;
            }
        }

        /*
        Nếu số lượt chơi đã hết (_NumTurn == 0):
        isBallThrown được đặt thành true, báo hiệu rằng viên đạn đã được ném.
        Tất cả các đối tượng có tag "Dot" (chỉ dẫn quỹ đạo) sẽ bị xóa khỏi trò chơi.
        Trò chơi tạm dừng bằng cách đặt _StopTime thành true.
        Giao diện người dùng được thiết lập lại thông qua lệnh ResetImgPick().
        */
        if (_NumTurn == 0)
        {
            isBallThrown = true;
            ///Xóa đường chỉ dẫn
            GameObject[] arrObj = GameObject.FindGameObjectsWithTag("Dot");
            for (int i = 0; i < arrObj.Length; i++) Destroy(arrObj[i]);
            _GameController._StopTime = true;
            _UIManager.ResetImgPick();
        }

    }
    public void throwBallAIEnemy(Vector3 target)
    {
        // Phát âm thanh và anim khi AI bắn.
        _SoundController.PlayShotgun_shotSound(true);
        _anim.Play("Hotgun");

        // Tính toán vector vận tốc từ AI địch đến mục tiêu và thực hiện xoay hướng bắn bằng cách gọi PickRotate(vel).
        Vector3 vel = target - gameObject.transform.position;
        PickRotate(vel);

        // Nếu mảng ball chưa được khởi tạo, hàm createBall() sẽ tạo ra các viên đạn mới cho AI.
        if (ball == null)
        {
            createBall();
        }

        // Kích hoạt và bắn các viên đạn của AI về phía các vị trí mục tiêu trong _listShot, tương tự như hành vi của người chơi.
        ball[0].SetActive(true); ball[1].SetActive(true); ball[2].SetActive(true);
        ball[0].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(ball[0].transform.position, _listShot[0].transform.position), ForceMode2D.Impulse);
        ball[1].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(ball[1].transform.position, _listShot[1].transform.position), ForceMode2D.Impulse);
        ball[2].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(ball[2].transform.position, _listShot[2].transform.position), ForceMode2D.Impulse);

        // Tương tự trong hàm throwBall().
        if (_NumShot == 5)
        {
            ball[3].SetActive(true); ball[4].SetActive(true);
            ball[3].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(ball[3].transform.position, _listShot[3].transform.position), ForceMode2D.Impulse);
            ball[4].GetComponent<Rigidbody2D>().AddForce(GetForceFrom(ball[4].transform.position, _listShot[4].transform.position), ForceMode2D.Impulse);
        }

        //	ball.GetComponent<Rigidbody2D>().useGravity = true;
        _NumTurn--;
        ball = null;
        if (_NumTurn != 0)
        {
            for (int i = 0; i < numOfTrajectoryPoints; i++)
            {
                trajectoryPoints[i].GetComponent<Renderer>().enabled = false;
            }
        }
        if (_NumTurn == 0)
        {
            isBallThrown = true;
            _GameController._CheckHit = true;
            _GameController._StopTime = true;
            _UIManager.ResetImgPick();
        }
    }

    //---------------------------------------	
    // Tính toán lực được áp dụng cho viên đạn dựa trên vị trí hiện tại của nó (fromPos) và vị trí mục tiêu (toPos).
    // Độ lớn của lực được điều chỉnh để nằm trong khoảng từ 30 đến 35 đơn vị.
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
        // Tính độ lớn của vận tốc (velocity) từ vận tốc theo trục x (pVelocity.x) và vận tốc theo trục y (pVelocity.y).
        float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));

        // Tính góc bắn của vật thể dựa trên vận tốc theo hai trục x và y, sử dụng hàm Atan2 để tính góc (theo radian).
        // Sau đó chuyển đổi từ radian sang độ bằng cách nhân với hằng số Rad2Deg.
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));

        // Khởi tạo biến thời gian fTime để theo dõi thời gian trôi qua khi tính toán vị trí của các điểm trong quỹ đạo.
        float fTime = 0;

        // Tăng giá trị thời gian thêm 0.1 giây, sử dụng để mô phỏng khoảng thời gian giữa các điểm trên quỹ đạo.
        fTime += 0.1f;

        // Vòng lặp để tính toán vị trí của từng điểm trên quỹ đạo, numOfTrajectoryPoints xác định số lượng điểm trong quỹ đạo.
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {

            // Tính toán khoảng cách di chuyển theo trục x (dx) và trục y (dy) dựa trên vận tốc ban
            // đầu (velocity), góc bắn (angle), và thời gian trôi qua (fTime).
            // Sử dụng Mathf.Cos cho trục x và Mathf.Sin cho trục y. angle được chuyển đổi từ độ sang radian bằng Mathf.Deg2Rad.
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad);

            // Xác định vị trí của điểm trên quỹ đạo (trajectory point) bằng cách cộng thêm độ dịch chuyển (dx, dy) vào vị trí ban đầu (pStartPosition).
            // Trục z được đặt cố định tại giá trị 2. 
            // Đoạn này đang thắc mắc tạo sao giá trị trục z lại là 2? Có thể là để đường bay của đạn không bị che lấp.
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 2);

            // Đặt vị trí của từng điểm trên quỹ đạo (trajectory point) với vị trí đã tính toán (pos).
            trajectoryPoints[i].transform.position = pos;

            // Bật thành phần Renderer của điểm trên quỹ đạo, đảm bảo rằng nó được hiển thị trên màn hình.
            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;

            // Đặt góc xoay của điểm quỹ đạo, dựa trên góc mà viên đạn đang di chuyển tại thời điểm fTime. Góc được tính bằng cách sử
            // dụng hàm Atan2, lấy vào vận tốc theo trục y có tính thêm lực hấp dẫn (Physics.gravity.magnitude * fTime) và vận tốc
            // theo trục x. Góc này sau đó được chuyển đổi từ radian sang độ.
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.1f; //Tăng thời gian thêm 0.1 giây để tiếp tục tính toán vị trí của điểm tiếp theo trong quỹ đạo cho lần lặp kế tiếp của vòng lặp.
        }
    }
}
