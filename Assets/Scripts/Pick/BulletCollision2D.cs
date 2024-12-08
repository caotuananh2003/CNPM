using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class BulletCollision2D : MonoBehaviour
{

    public float RelativeVelocityRequired; // Tốc độ va chạm yêu cầu để phát hiện va chạm.
    public GameObject dot; // Đối tượng hiệu ứng va chạm (ví dụ: vụ nổ).
    public int _Turn = 1; // Định nghĩa lượt chơi, giá trị này có thể dùng để phân biệt đội.
    public bool _DestroyParent = true; // Quyết định có phá hủy đối tượng cha sau khi va chạm hay không.
    BoxCollider2D collider; // Thành phần BoxCollider2D để kiểm soát va chạm.
    const float skinWidth = 0.005f; // Độ rộng của ranh giới va chạm.
    float xMax, xMin, yMin, yMax; // Biên giới màn chơi, dùng để xác định khi viên đạn đi ra khỏi màn.
    public bool _BulletFinish = false; // Kiểm tra xem đạn đã kết thúc nhiệm vụ chưa.
    public bool _CheckYMax = false; // Cờ kiểm tra khi đạn vượt quá yMax.
    SoundController _SoundController; // Đối tượng để phát âm thanh.
    Vector3 _PosStart; // Vị trí bắt đầu của viên đạn.

    // Use this for initialization
    void Start()
    {
        // Tìm đối tượng có tag "Map_Front" để xác định giới hạn màn chơi.
        GameObject other = GameObject.FindGameObjectWithTag("Map_Front");

        // Tính toán các biên giới của màn chơi dựa trên vị trí và kích thước của đối tượng.
        xMax = other.transform.position.x + (other.GetComponent<BoxCollider2D>().size.x * other.transform.localScale.x) / 2;
        xMin = other.transform.position.x - (other.GetComponent<BoxCollider2D>().size.x * other.transform.localScale.x) / 2;
        yMin = other.transform.position.y - (other.GetComponent<BoxCollider2D>().size.y * other.transform.localScale.y) / 2;
        yMax = other.transform.position.y + (other.GetComponent<BoxCollider2D>().size.y * other.transform.localScale.y) / 2;

        collider = gameObject.GetComponent<BoxCollider2D>(); // Lấy thành phần BoxCollider2D của viên đạn.
        _SoundController = FindObjectOfType<SoundController>(); // Tìm đối tượng điều khiển âm thanh.
        _PosStart = gameObject.transform.position; // Lưu vị trí bắt đầu của viên đạn.
    }

    // Update is called once per frame
    void Update()
    {
        // Nếu cờ _CheckYMax bật và đạn đã vượt quá giới hạn yMax, thực hiện va chạm.
        if (_CheckYMax && gameObject.transform.position.y > yMax)
        {
            CollionActive();
        }

        // Kiểm tra xem đạn có vượt ra khỏi các giới hạn của màn chơi không.
        if (gameObject.transform.position.x > xMax || gameObject.transform.position.x < xMin || gameObject.transform.position.y < yMin)
        {
            CollionActive();
        }
        UpdateSkin();

        // 2 dòng mã sau chưa biết là để làm gì.
        //Destroy(gameObject.GetComponent<PolygonCollider2D>());
        //PolygonCollider2D poly = gameObject.AddComponent<PolygonCollider2D>();
    }
    /// <summary>
    /// Khi đạn va chạm player
    /// </summary>
    /// <param name="other"></param>

    // Hàm được gọi khi đạn va chạm với một đối tượng khác (dùng cho va chạm Trigger)
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đạn có va chạm với "Player" hoặc "Enemy".
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            // Đối với đạn loại "CarrotSmall", bỏ qua nếu khoảng cách quá nhỏ (dưới 0.5).
            if (gameObject.name == "CarrotSmall(Clone)")
            {
                if (Vector2.Distance(other.gameObject.transform.position, _PosStart) < 0.5f) return;
            }

            Player player = other.GetComponent<Player>(); // Lấy đối tượng Player từ đối tượng va chạm.

            // Nếu người chơi hiện tại đang là lượt của họ, bỏ qua va chạm.
            if (player._IsCurrent) return;

            // Kiểm tra loại đạn và phát âm thanh tương ứng.
            if (dot.tag == "Explosion" || dot.tag == "Explosion_Bomp")
            {
                _SoundController.PlayExplosionSound(true); // Âm thanh vụ nổ.
            }
            if (dot.tag == "Explosion_Poison")
            {
                _SoundController.PlayGlassSound(true); // Âm thanh chất độc.
            }

            // Tạo hiệu ứng va chạm tại vị trí đạn.
            Instantiate(dot, transform.position, transform.rotation);
            CollionActive(); // Gọi hàm xử lý va chạm.
        }

        // Nếu đạn va chạm với bản đồ.
        if (other.gameObject.tag == "Map")
        {
            // Phát âm thanh tương ứng dựa trên loại đạn.
            if (dot.tag == "Explosion" || dot.tag == "Explosion_Bomp")
            {
                _SoundController.PlayExplosionSound(true);
            }
            if (dot.tag == "Explosion_Poison")
            {
                _SoundController.PlayGlassSound(true);
            }

            // Tạo hiệu ứng tại vị trí va chạm.
            Instantiate(dot, transform.position, transform.rotation);
            CollionActive(); // Xử lý va chạm.
        }
    }

    /// <summary>
    /// Va chạm với map
    /// </summary>
    /// <param name="other"></param>
    // Hàm này xử lý va chạm vật lý với các đối tượng khác (không phải trigger).
    void OnCollisionEnter2D(Collision2D other)
    {
        // Kiểm tra nếu vận tốc va chạm đủ mạnh.
        if (other.relativeVelocity.magnitude >= RelativeVelocityRequired)
        {
            // Nếu va chạm với bản đồ.
            if (other.gameObject.tag == "Map")
            {
                var contact0 = other.contacts[0]; // Lấy điểm tiếp xúc.

                // Phát âm thanh dựa trên loại đạn.
                if (dot.tag == "Explosion" || dot.tag == "Explosion_Bomp")
                {
                    _SoundController.PlayExplosionSound(true);
                }
                if (dot.tag == "Explosion_Poison")
                {
                    _SoundController.PlayGlassSound(true);
                }

                // Tạo hiệu ứng vụ nổ tại điểm va chạm.
                Instantiate(dot, contact0.point, transform.rotation);
                CollionActive(); // Xử lý sau va chạm.
            }
        }
    }

    // Hàm xử lý hành động sau khi va chạm được kích hoạt.
    private void CollionActive()
    {
        GameController gameController = FindObjectOfType<GameController>(); // Lấy đối tượng GameController.

        // Nếu _Turn không phải là lượt đầu tiên, di chuyển camera tới đối tượng.
        if (_Turn != 1)
        {
            gameController.MoveCameraToObj();
        }

        // Nếu đạn đã hoàn thành và là lượt của nó, chuyển lượt.
        if (!gameController._GameState._IsChangding && _Turn == 1 && _BulletFinish)
            gameController.ChangeTurn();

        // Nếu _DestroyParent là true, phá hủy đối tượng cha, nếu không, phá hủy chính nó.
        if (_DestroyParent)
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
        else
            Destroy(gameObject);
    }

    // Hàm cập nhật lại kích thước vùng va chạm (hitbox) của đạn.
    private void UpdateSkin()
    {
        Bounds bounds = collider.bounds; // Lấy ranh giới hiện tại của đạn.
        bounds.Expand(skinWidth * -2); // Thu nhỏ ranh giới một chút để tránh va chạm không mong muốn.
    }
}