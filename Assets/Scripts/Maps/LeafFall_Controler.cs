using UnityEngine;
using System.Collections;

/*
Giải thích: Script này điều khiển việc lá cây rơi xuống với tốc độ ngẫu nhiên, đồng
thời xoay lá theo hướng xác định. Khi lá rơi ra khỏi màn hình, nó sẽ tự động bị hủy.
*/
public class LeafFall_Controler : MonoBehaviour
{
    private float _speed = 0; // Tốc độ rơi của lá
    private int _dir; // Hướng quay của lá
    private Bounds _bound; // Giới hạn của camera

    // Khởi tạo ban đầu
    void Start()
    {
        // Xác định phạm vi camera
        _bound = Helper.OrthographicBounds(Camera.main);
        // Tốc độ rơi được chọn ngẫu nhiên trong khoảng từ 1 đến 3
        _speed = Random.Range(1f, 3f);
        // Hướng quay của lá (ngẫu nhiên chọn 1 hoặc -1)
        _dir = Random.Range(0, 2);
        if (_dir == 1) _dir = 1;
        else _dir = -1;
    }

    // Cập nhật mỗi khung hình
    void Update()
    {
        // Di chuyển lá rơi theo phương Y với tốc độ được chỉ định
        transform.position += Vector3.down * _speed * Time.deltaTime;
        // Quay lá theo hướng được chỉ định
        transform.Rotate(Vector3.forward * _dir * _speed);

        // Nếu lá ra ngoài giới hạn của màn hình thì hủy đối tượng
        if (transform.position.y < Camera.main.transform.position.y - _bound.extents.y)
        {
            Destroy(gameObject);
        }
    }
}