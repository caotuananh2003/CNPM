using UnityEngine; // Sử dụng không gian tên UnityEngine cho các lớp và chức năng liên quan đến Unity
using System.Collections; // Sử dụng không gian tên System.Collections cho các kiểu tập hợp

public class BulletControler : MonoBehaviour
{

    // Biến kiểm tra trạng thái xử lý va chạm
    bool _Check = false;

    // Biến lưu vị trí cũ của viên đạn
    Vector3 _posOld = new Vector3();

    // Phương thức khởi tạo
    void Start()
    {
        // Lưu vị trí ban đầu của viên đạn vào biến _posOld
        _posOld = transform.position;
    }

    // Phương thức Update được gọi mỗi khung hình
    void Update()
    {

        // Ghi chú: Hai dòng này đã bị chú thích và không được sử dụng
        // Destroy(gameObject.GetComponent<PolygonCollider2D>()); // Xóa collider polygon hiện tại (nếu có)
        // PolygonCollider2D poly = gameObject.AddComponent<PolygonCollider2D>(); // Thêm một collider polygon mới

        // Tính toán hướng di chuyển của viên đạn bằng cách lấy hiệu giữa vị trí hiện tại và vị trí cũ
        Vector3 dir = transform.position - _posOld;

        // Tính góc giữa hướng di chuyển và trục Ox
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // Chuyển đổi góc từ radian sang độ

        // Kiểm tra xem góc có khác không (điều này có thể kiểm tra nếu viên đạn đã di chuyển)
        if (angle != 0)
        {
            // Quay viên đạn theo góc đã tính toán
            transform.eulerAngles = new Vector3(0, 0, angle);

            // Cập nhật vị trí cũ với vị trí hiện tại
            _posOld = transform.position;
        }

        // Kích hoạt collider BoxCollider2D của viên đạn (nếu nó được đặt là không hoạt động)
        gameObject.GetComponentInChildren<BoxCollider2D>().enabled = true;
    }
}
