using UnityEngine;
using System.Collections;

/*
Giải thích: Script này tạo ra các đối tượng (ví dụ như lá rơi, đá rơi) tại vị trí ngẫu
nhiên phía trên camera và để chúng rơi xuống. Khoảng thời gian giữa các lần tạo đối tượng
được xác định bởi biến _TimeInit.
*/
public class FallingController : MonoBehaviour
{

    public GameObject[] _Falling; // Mảng chứa các đối tượng rơi (ví dụ: lá cây, đá)
    public float _TimeInit = 2; // Khoảng thời gian giữa các lần rơi
    private float _timeCount = 0; // Đếm thời gian từ lần rơi trước đó
    Bounds _boundCamera; // Giới hạn của camera để biết phạm vi hiển thị

    // Khởi tạo ban đầu
    void Start()
    {
        // Xác định phạm vi giới hạn của camera
        _boundCamera = Helper.OrthographicBounds(Camera.main);
    }

    // Cập nhật mỗi khung hình
    void Update()
    {
        // Tăng giá trị bộ đếm thời gian
        _timeCount += Time.deltaTime;
        // Kiểm tra nếu đã đủ thời gian để tạo một đối tượng mới rơi
        if (_timeCount > _TimeInit)
        {
            _timeCount = 0; // Đặt lại bộ đếm thời gian
            // Chọn ngẫu nhiên một đối tượng rơi từ mảng
            int ranFalling = Random.Range(0, _Falling.Length);
            // Xác định vị trí rơi ngẫu nhiên trong phạm vi màn hình camera
            Vector2 pos = new Vector2(
                Random.Range(Camera.main.transform.position.x - _boundCamera.min.x,
                             Camera.main.transform.position.x + _boundCamera.min.x),
                Camera.main.transform.position.y + _boundCamera.max.y
            );
            // Tạo mới đối tượng rơi tại vị trí xác định
            Instantiate(_Falling[ranFalling], pos, Quaternion.identity);
        }
    }
}