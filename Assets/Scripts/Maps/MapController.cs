using UnityEngine;
using System.Collections;

/*
Giải thích chi tiết:
_IsMove: Kiểm soát việc có cho phép kéo map bằng chuột không.
hit_position, current_position, map_position: Lưu các vị trí chuột và vị trí ban đầu của map khi kéo.
_Speed: Điều chỉnh tốc độ di chuyển của map khi kéo.
MoveMapToBullet: Di chuyển map tới vị trí của một đối tượng (đạn, phi tiêu) và đảm bảo map không di chuyển ra khỏi phạm vi màn hình.
LeftMouseDrag: Tính toán vị trí di chuyển của map khi người dùng kéo chuột, đồng thời đảm bảo map không vượt ra khỏi giới hạn màn hình.
Script này kiểm soát việc di chuyển của bản đồ trong trò chơi, dựa trên việc kéo chuột và theo dõi vị trí của các đối tượng đạn hoặc phi tiêu.
*/

public class MapController : MonoBehaviour
{
    public bool _IsMove = true; // Biến kiểm soát việc cho phép map di chuyển bằng chuột (true: cho phép, false: không cho phép)
    Vector3 hit_position = Vector3.zero; // Lưu vị trí khi nhấp chuột lần đầu tiên (khi bắt đầu kéo map)
    Vector3 current_position = Vector3.zero; // Lưu vị trí hiện tại của chuột trong quá trình kéo
    Vector3 map_position = Vector3.zero; // Lưu vị trí của map tại thời điểm bắt đầu kéo
    public float _Speed = 2f; // Tốc độ di chuyển của map khi kéo bằng chuột
    GameController _GameController; // Biến tham chiếu đến GameController để kiểm tra trạng thái của trò chơi

    // Use this for initialization
    void Start()
    {
        // Tìm đối tượng GameController trong scene và gán vào biến _GameController
        _GameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Nếu trò chơi không ở trạng thái đang chơi (IsGamePlay = false), dừng việc cập nhật
        if (!_GameController._GameState._IsGamePlay) return;

        // Tìm đối tượng có tag là "Bullet" (đạn) và "Dart" (phi tiêu)
        GameObject bullet = GameObject.FindGameObjectWithTag("Bullet");
        GameObject dart = GameObject.FindGameObjectWithTag("Dart");
        GameObject Tar = null; // Biến lưu trữ đối tượng mục tiêu (đạn hoặc phi tiêu)

        // Nếu tìm thấy đạn, đặt Tar là bullet
        if (bullet != null) Tar = bullet;
        // Nếu không có đạn nhưng có phi tiêu, đặt Tar là dart
        if (dart != null) Tar = dart;

        // Nếu có đối tượng mục tiêu, gọi hàm MoveMapToBullet để di chuyển map theo vị trí của đối tượng đó
        if (Tar != null) MoveMapToBullet(Tar.transform.position);

        // Nếu biến _IsMove = false, không cho phép kéo map bằng chuột
        if (!_IsMove) return;

        // Khi nhấn chuột trái (bắt đầu kéo map)
        if (Input.GetMouseButtonDown(0))
        {
            // Lưu vị trí chuột vào hit_position
            hit_position = Input.mousePosition;
            // Lưu vị trí hiện tại của map vào map_position
            map_position = transform.position;
        }

        // Khi giữ chuột trái (đang kéo map)
        if (Input.GetMouseButton(0))
        {
            // Lưu vị trí hiện tại của chuột vào current_position
            current_position = Input.mousePosition;
            // Gọi hàm LeftMouseDrag để xử lý kéo map
            LeftMouseDrag();
        }
    }

    // Hàm xử lý kéo map bằng chuột trái
    void LeftMouseDrag()
    {
        // From the Unity3D docs: "The z position is in world units from the camera."  In my case I'm using the y-axis as height
        // with my camera facing back down the y-axis.  You can ignore this when the camera is orthograhic.
        // Tính toán vị trí z, ở đây sử dụng chiều cao y của map để làm mốc
        current_position.z = hit_position.z = map_position.y;

        // Get direction of movement. (Note: Don't normalize, the magnitude of change is going to be Vector3.Distance(current_position-hit_position) anyways).
        // Tính hướng di chuyển bằng cách lấy sự chênh lệch giữa vị trí hiện tại của chuột và vị trí ban đầu
        // (Không chuẩn hóa hướng di chuyển để giữ độ lớn của khoảng cách di chuyển)
        Vector3 direction = Camera.main.ScreenToWorldPoint(current_position) - Camera.main.ScreenToWorldPoint(hit_position);

        // Đảo ngược hướng di chuyển để map di chuyển theo cùng chiều với chuột
        direction = (direction * -1) * _Speed;

        // Tính toán vị trí mới của map sau khi di chuyển
        Vector3 position = map_position + direction;

        // Lấy kích thước của map từ collider của nó
        Vector2 size = gameObject.GetComponent<BoxCollider2D>().size * gameObject.transform.localScale.x;

        // Đảm bảo rằng map không di chuyển ra khỏi giới hạn bên phải của màn hình
        if (position.x + size.x / 2 <= (Helper.OrthographicBounds(Camera.main).size.x / 2 + Camera.main.transform.position.x))
        {
            position.x = (Helper.OrthographicBounds(Camera.main).size.x / 2 + Camera.main.transform.position.x) - size.x / 2;
        }

        // Đảm bảo rằng map không di chuyển ra khỏi giới hạn bên trái của màn hình
        if (position.x - size.x / 2 >= (Camera.main.transform.position.x - Helper.OrthographicBounds(Camera.main).size.x / 2))
        {
            position.x = (Camera.main.transform.position.x - Helper.OrthographicBounds(Camera.main).size.x / 2) + size.x / 2;
        }

        // Đảm bảo rằng map không di chuyển ra khỏi giới hạn phía trên của màn hình
        if (position.y + size.y / 2 <= (Helper.OrthographicBounds(Camera.main).size.y / 2 + Camera.main.transform.position.y))
        {
            position.y = (Helper.OrthographicBounds(Camera.main).size.y / 2 + Camera.main.transform.position.y) - size.y / 2;
        }

        // Đảm bảo rằng map không di chuyển ra khỏi giới hạn phía dưới của màn hình
        if (position.y - size.y / 2 >= (Camera.main.transform.position.y - Helper.OrthographicBounds(Camera.main).size.y / 2))
        {
            position.y = (Camera.main.transform.position.y - Helper.OrthographicBounds(Camera.main).size.y / 2) + size.y / 2;
        }

        // Cập nhật vị trí mới của map
        transform.position = position;
    }
    /// <summary>
    /// Di chuyển map theo đạn
    /// </summary>
    public void MoveMapToBullet(Vector3 _target)
    {
        // Lấy vị trí của mục tiêu
        Vector3 position = _target;
        position.z = gameObject.transform.position.z; // Giữ nguyên tọa độ Z của map

        // Lấy kích thước của map từ collider của nó
        Vector2 size = gameObject.GetComponent<BoxCollider2D>().size * gameObject.transform.localScale.x;

        // Đảm bảo rằng map không di chuyển ra khỏi giới hạn bên phải của màn hình
        if (position.x + size.x / 2 <= (Helper.OrthographicBounds(Camera.main).size.x / 2 + Camera.main.transform.position.x))
        {
            position.x = (Helper.OrthographicBounds(Camera.main).size.x / 2 + Camera.main.transform.position.x) - size.x / 2;
        }

        // Đảm bảo rằng map không di chuyển ra khỏi giới hạn bên trái của màn hình
        if (position.x - size.x / 2 >= (Camera.main.transform.position.x - Helper.OrthographicBounds(Camera.main).size.x / 2))
        {
            position.x = (Camera.main.transform.position.x - Helper.OrthographicBounds(Camera.main).size.x / 2) + size.x / 2;
        }

        // Đảm bảo rằng map không di chuyển ra khỏi giới hạn phía trên của màn hình
        if (position.y + size.y / 2 <= (Helper.OrthographicBounds(Camera.main).size.y / 2 + Camera.main.transform.position.y))
        {
            position.y = (Helper.OrthographicBounds(Camera.main).size.y / 2 + Camera.main.transform.position.y) - size.y / 2;
        }

        // Đảm bảo rằng map không di chuyển ra khỏi giới hạn phía dưới của màn hình
        if (position.y - size.y / 2 >= (Camera.main.transform.position.y - Helper.OrthographicBounds(Camera.main).size.y / 2))
        {
            position.y = (Camera.main.transform.position.y - Helper.OrthographicBounds(Camera.main).size.y / 2) + size.y / 2;
        }

        // Cập nhật vị trí của map theo đối tượng mục tiêu
        transform.position = position;
    }
}
