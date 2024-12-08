using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    // Biến boolean cho phép camera di chuyển, có thể bật/tắt di chuyển camera
    public bool _IsMove = true;
    
    // GameObject đại diện cho bản đồ trong game
    GameObject _Map;
    
    // Vị trí khi click chuột lần đầu
    Vector3 hit_position = Vector3.zero;
    
    // Vị trí hiện tại của chuột khi đang kéo
    Vector3 current_position = Vector3.zero;
    
    // Vị trí ban đầu của camera
    Vector3 camera_position = Vector3.zero;
    
    // Tốc độ di chuyển của camera khi kéo chuột
    public float _Speed = 2f;
    
    // Biến điều khiển game
    GameController _GameController;

    // Hàm khởi tạo được gọi khi bắt đầu chạy script
    void Start () {
        // Tìm đối tượng GameController trong game để quản lý trạng thái game
        _GameController = FindObjectOfType<GameController>();
    }
    
    // Hàm cập nhật được gọi mỗi frame
    void Update () {
        // Nếu _Map chưa được gán, tìm GameObject có tag "Map_Front"
        if (_Map == null) {
            _Map = GameObject.FindGameObjectWithTag("Map_Front");
            if (_Map == null) return; // Nếu không tìm thấy, thoát khỏi hàm
        }

        // Nếu game không đang trong trạng thái chơi thì thoát
        if (!_GameController._GameState._IsGamePlay) return;

        // Tìm đối tượng đạn (Bullet) hoặc mũi tên (Dart) nếu có
        GameObject bullet = GameObject.FindGameObjectWithTag("Bullet");
        GameObject dart = GameObject.FindGameObjectWithTag("Dart");
        GameObject Tar = null;
        
        // Ưu tiên di chuyển theo viên đạn nếu tồn tại
        if (bullet != null) Tar = bullet;
        if (dart != null) Tar = dart;
        
        // Nếu có viên đạn/mũi tên, di chuyển camera theo vị trí của nó
        if (Tar != null) MoveCameraToBullet(Tar.transform.position);

        // Nếu không cho phép di chuyển (mouse drag), thoát
        if (!_IsMove) return;

        // Kiểm tra xem người dùng có nhấn chuột trái
        if (Input.GetMouseButtonDown(0)) {
            hit_position = Input.mousePosition; // Lưu lại vị trí chuột khi nhấn
            camera_position = Helper.OrthographicBounds(Camera.main).center; // Lấy vị trí trung tâm của camera
        }

        // Kiểm tra nếu chuột trái vẫn đang được giữ
        if (Input.GetMouseButton(0)) {
            current_position = Input.mousePosition; // Cập nhật vị trí chuột hiện tại
            LeftMouseDrag(); // Gọi hàm kéo camera
        }
    }

    // Hàm xử lý khi người dùng kéo camera bằng chuột trái
    void LeftMouseDrag() {
        // Đặt z của current_position và hit_position bằng với y của camera (chiều sâu không ảnh hưởng do camera 2D)
        current_position.z = hit_position.z = camera_position.y;

        // Tính hướng di chuyển bằng cách lấy vị trí chuột hiện tại trừ vị trí lúc nhấn chuột
        Vector3 direction = Camera.main.ScreenToWorldPoint(current_position) - Camera.main.ScreenToWorldPoint(hit_position);

        // Đảo ngược hướng để di chuyển bản đồ cùng hướng với di chuyển của chuột
        direction = (direction * -1) * _Speed;

        // Cập nhật vị trí camera mới
        Vector3 position = camera_position + direction;
        position.z = -10; // Đặt z về giá trị -10 (giữ camera cố định theo trục z)

        // Kiểm tra giới hạn di chuyển của camera trong phạm vi bản đồ (theo trục x)
        Vector2 size = _Map.GetComponent<BoxCollider2D>().size * gameObject.transform.localScale.x;
        if (position.x + Helper.OrthographicBounds(Camera.main).size.x / 2 >= (_Map.transform.position.x + size.x / 2)) {
            position.x = (_Map.transform.position.x + size.x / 2) - Helper.OrthographicBounds(Camera.main).size.x / 2;
        }
        if (position.x - Helper.OrthographicBounds(Camera.main).size.x / 2 <= (_Map.transform.position.x - size.x / 2)) {
            position.x = (_Map.transform.position.x - size.x / 2) + Helper.OrthographicBounds(Camera.main).size.x / 2;
        }

        // Kiểm tra giới hạn di chuyển của camera trong phạm vi bản đồ (theo trục y)
        if (position.y + Helper.OrthographicBounds(Camera.main).size.y / 2 >= (_Map.transform.position.x + size.y / 2)) {
            position.y = (_Map.transform.position.y + size.y / 2) - Helper.OrthographicBounds(Camera.main).size.y / 2;
        }
        if (position.y - Helper.OrthographicBounds(Camera.main).size.y / 2 <= (_Map.transform.position.y - size.y / 2)) {
            position.y = (_Map.transform.position.y - size.y / 2) + Helper.OrthographicBounds(Camera.main).size.y / 2;
        }

        // Cập nhật lại vị trí camera
        transform.position = position;
    }

    // Hàm di chuyển camera theo viên đạn
    public void MoveCameraToBullet(Vector3 _target) {
        Vector3 position = _target; // Lấy vị trí mục tiêu là viên đạn
        position.z = Camera.main.transform.position.z; // Giữ nguyên giá trị z của camera
        
        // Kiểm tra giới hạn di chuyển (theo trục x)
        Vector2 size = _Map.GetComponent<BoxCollider2D>().size * gameObject.transform.localScale.x;
        if (position.x + Helper.OrthographicBounds(Camera.main).size.x / 2 >= (_Map.transform.position.x + size.x / 2)) {
            position.x = (_Map.transform.position.x + size.x / 2) - Helper.OrthographicBounds(Camera.main).size.x / 2;
        }
        if (position.x - Helper.OrthographicBounds(Camera.main).size.x / 2 <= (_Map.transform.position.x - size.x / 2)) {
            position.x = (_Map.transform.position.x - size.x / 2) + Helper.OrthographicBounds(Camera.main).size.x / 2;
        }

        // Kiểm tra giới hạn di chuyển (theo trục y)
        if (position.y + Helper.OrthographicBounds(Camera.main).size.y / 2 >= (_Map.transform.position.x + size.y / 2)) {
            position.y = (_Map.transform.position.y + size.y / 2) - Helper.OrthographicBounds(Camera.main).size.y / 2;
        }
        if (position.y - Helper.OrthographicBounds(Camera.main).size.y / 2 <= (_Map.transform.position.y - size.y / 2)) {
            position.y = (_Map.transform.position.y - size.y / 2) + Helper.OrthographicBounds(Camera.main).size.y / 2;
        }

        // Cập nhật vị trí camera theo viên đạn
        transform.position = position;
    }
}
