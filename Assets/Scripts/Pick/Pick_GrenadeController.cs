using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pick_GrenadeController : MonoBehaviour
{

    public GameObject TrajectoryPointPrefeb;  // Prefab điểm hiển thị quỹ đạo ném
    public GameObject _NumberText;  // Text hiển thị thời gian đếm ngược
    public int numOfTrajectoryPoints = 30;  // Số lượng điểm quỹ đạo
    public float _Angle;  // Góc sai lệch của súng so với góc ban đầu
    private GameObject numText;  // Biến lưu trữ object số đếm ngược
    public GameObject _Explosion;  // Hiệu ứng nổ khi bom nổ
    private bool isPressed, isBallThrown;  // Cờ kiểm tra trạng thái nhấn chuột và ném bom
    private float power = 5;  // Lực ném
    private List<GameObject> trajectoryPoints;  // Danh sách điểm quỹ đạo
    GameController _GameController;  // Truy cập GameController
    private float _scale = 0.1f;  // Tỉ lệ thu phóng các điểm quỹ đạo
    private int _timeDestroy = 3;  // Thời gian đếm ngược trước khi bom nổ
    private float _timeCount = 0;  // Biến đếm thời gian
    private Vector3 _posDown, _posUp;  // Điểm bắt đầu và kết thúc khi kéo chuột
    UIManager _UIManager;  // Truy cập UIManager
    SoundController _SoundController;  // Truy cập SoundController

    //---------------------------------------	
    void Start()
    {
        trajectoryPoints = new List<GameObject>();  // Khởi tạo danh sách điểm quỹ đạo
        _GameController = FindObjectOfType<GameController>();  // Tìm GameController trong game
        _UIManager = FindObjectOfType<UIManager>();  // Tìm UIManager trong game
        _SoundController = FindObjectOfType<SoundController>();  // Tìm SoundController trong game
        isPressed = isBallThrown = false;  // Khởi tạo trạng thái chưa nhấn chuột và chưa ném bom

        // Tạo và ẩn các điểm quỹ đạo khi bắt đầu
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            GameObject dot = (GameObject)Instantiate(TrajectoryPointPrefeb);  // Tạo một điểm quỹ đạo
            dot.transform.localScale = new Vector3((numOfTrajectoryPoints - i * 0.6f) * _scale, (numOfTrajectoryPoints - i * 0.6f) * _scale, (numOfTrajectoryPoints - i * 0.6f) * _scale);  // Điều chỉnh kích thước điểm quỹ đạo
            dot.GetComponent<Renderer>().enabled = false;  // Ẩn điểm quỹ đạo khi chưa ném
            trajectoryPoints.Insert(i, dot);  // Thêm điểm vào danh sách
        }
    }

    //---------------------------------------	
    void Update()
    {
        //=======Di chuyển số theo gameObject
        if (isBallThrown && numText != null)
        {
            numText.transform.position = gameObject.transform.position;  // Cập nhật vị trí của text theo vị trí bom
            _timeCount += Time.deltaTime;  // Tăng thời gian đếm
            if (_timeCount > 1)
            {
                _timeDestroy--;  // Giảm số đếm ngược
                _timeCount = 0;  // Reset thời gian đếm

                // Cập nhật số hiển thị trên text
                NumberText lbNumText = numText.GetComponentInChildren<NumberText>();
                lbNumText.SetNumberText(_timeDestroy);  // Cập nhật số đếm ngược

                // Nếu thời gian đếm bằng 0, bom nổ
                if (_timeDestroy == 0)
                {
                    Instantiate(_Explosion, transform.position, Quaternion.identity);  // Tạo hiệu ứng nổ
                    _SoundController.PlayExplosionSound(true);  // Phát âm thanh nổ

                    // Chuyển lượt sau khi nổ
                    GameController gameController = FindObjectOfType<GameController>();
                    if (!gameController._GameState._IsChangding) gameController.ChangeTurn();

                    Destroy(numText);  // Xóa text số đếm ngược
                    Destroy(gameObject);  // Xóa bom sau khi nổ
                }
            }
            return;
        }

        // Kiểm tra trạng thái của game trước khi ném
        if (_GameController._CheckHit) return;
        if (_GameController._TypeGame == 0 && _GameController._GameState._IsEnemyStart) return;

        // Kiểm tra điều kiện không được nhấn chuột ở vùng phía dưới hoặc UI đang mở
        if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).y < _GameController.INood.transform.position.y || _UIManager._ListPopup[0].activeSelf) && !isPressed) return;

        // Khi nhấn chuột xuống
        if (Input.GetMouseButtonDown(0))
        {
            _posDown = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // Lưu vị trí chuột khi nhấn
            isPressed = true;  // Cập nhật trạng thái nhấn chuột

            // Tạo text nếu chưa có
            if (!numText)
            {
                createText();
            }
        }
        else if (Input.GetMouseButtonUp(0)) // Khi chuột buông
        {
            isPressed = false;  // Cập nhật trạng thái thả chuột
            _posUp = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // Lưu vị trí chuột khi thả

            // Nếu chưa ném bom, thực hiện ném
            if (!isBallThrown)
            {
                throwBall();
            }
        }

        // Khi chuột đang được nhấn, tính toán và hiển thị đường quỹ đạo ném
        if (isPressed)
        {
            Vector3 vel = GetForceFrom(Camera.main.ScreenToWorldPoint(Input.mousePosition), _posDown);  // Tính toán vận tốc dựa trên vị trí chuột
            float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;  // Tính góc giữa vận tốc và trục Ox

            // Điều chỉnh góc theo hướng của người chơi
            Player player = _GameController._GameObj.GetComponent<Player>();
            angle += _Angle;
            if (player._Dir == Player.Dir.left)
            {
                angle = 360 - angle;
            }
            transform.eulerAngles = new Vector3(0, 0, angle);  // Cập nhật góc của bom

            // Hiển thị đường quỹ đạo dựa trên vận tốc
            setTrajectoryPoints(transform.position, vel / gameObject.GetComponent<Rigidbody2D>().mass);
        }

    }

    // Tạo text số đếm ngược
    private void createText()
    {
        numText = (GameObject)Instantiate(_NumberText);  // Tạo object số đếm ngược
        Vector3 pos = transform.position;
        pos.z = 1;  // Đặt text hiển thị ở layer phía trước
        numText.transform.position = pos;  // Đặt vị trí của text trùng với vị trí bom
    }

    //---------------------------------------	
    // Ném bom với lực được tính toán từ vị trí thả chuột
    private void throwBall()
    {
        _NumberText.SetActive(true);  // Hiển thị text số đếm ngược
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;  // Kích hoạt trọng lực cho bom
        //	ball.GetComponent<Rigidbody2D>().useGravity = true;
        gameObject.GetComponent<Rigidbody2D>().AddForce(GetForceFrom(_posUp, _posDown), ForceMode2D.Impulse);  // Thêm lực để ném bom
        isBallThrown = true;  // Cập nhật trạng thái bom đã ném

        // Xóa các điểm quỹ đạo sau khi ném
        GameObject[] arrObj = GameObject.FindGameObjectsWithTag("Dot");
        for (int i = 0; i < arrObj.Length; i++) Destroy(arrObj[i]);

        _GameController._CheckHit = true;  // Cập nhật trạng thái trúng đích
        _GameController._StopTime = true;  // Dừng thời gian
        _UIManager.ResetImgPick();  // Reset UI
    }

    // Hàm ném bom của đối thủ AI
    public void throwBallAIEnemy(Vector3 vel)
    {
        if (!numText)
            createText();
        _NumberText.SetActive(true);
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        //	ball.GetComponent<Rigidbody2D>().useGravity = true;
        gameObject.GetComponent<Rigidbody2D>().AddForce(vel, ForceMode2D.Impulse);
        isBallThrown = true;
        _GameController._CheckHit = true;
        _GameController._StopTime = true;
        _UIManager.ResetImgPick();
    }
    //---------------------------------------	
    // Tính toán lực ném dựa trên vị trí bắt đầu và kết thúc
    private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
    {
        power = 5;  // Khởi tạo lực ném
                    // Giảm lực ném cho đến khi độ lớn của vector lực đủ nhỏ
        while (((new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power).sqrMagnitude > 450f)
        {
            power -= 0.05f;  // Giảm lực ném từng chút một
        }

        // Tính toán và trả về vector lực từ vị trí bắt đầu đến vị trí kết thúc
        return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power; // Lực ném nhân với một hằng số
    }

    //---------------------------------------	
    // It displays projectile trajectory path
    //---------------------------------------	
    void setTrajectoryPoints(Vector3 pStartPosition, Vector3 pVelocity)
    {
        // Tính toán vận tốc từ thành phần x và y của vector vận tốc
        float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
        // Tính toán góc ném từ vector vận tốc
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));
        float fTime = 0;  // Khởi tạo thời gian mô phỏng

        fTime += 0.1f;  // Bắt đầu mô phỏng từ 0.1 giây

        // Lặp qua từng điểm trong quỹ đạo để tính toán và hiển thị
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            // Tính toán thay đổi vị trí theo thời gian
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);  // Đổi vị trí theo trục x
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * fTime * fTime / 2.0f);  // Đổi vị trí theo trục y, tính toán ảnh hưởng của trọng lực

            // Tính toán vị trí mới cho điểm quỹ đạo
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 2);
            trajectoryPoints[i].transform.position = pos;  // Cập nhật vị trí cho điểm quỹ đạo
            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;  // Hiển thị điểm quỹ đạo
                                                                          // Cập nhật góc của điểm quỹ đạo theo hướng di chuyển
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);

            fTime += 0.1f;  // Tăng thời gian cho lần lặp tiếp theo
        }
    }
}
