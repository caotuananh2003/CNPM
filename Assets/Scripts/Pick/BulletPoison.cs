using UnityEngine;
using System.Collections;

public class BulletPoison : MonoBehaviour
{

    public GameObject _Poison; // Biến công khai để tham chiếu đến đối tượng chất độc (Poison)

    // Use this for initialization
    void Start()
    {

    }

    // Phương thức này được gọi khi đối tượng va chạm với một Collider2D khác
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu đối tượng va chạm có tag là "Player" hoặc "Enemy"
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            // Tạo một đối tượng chất độc mới tại vị trí của đối tượng va chạm
            GameObject poison = (GameObject)Instantiate(_Poison, other.gameObject.transform.position, Quaternion.identity);

            // Đặt chất độc là con của đối tượng va chạm (Player hoặc Enemy)
            poison.transform.parent = other.gameObject.transform;

            // Lấy thành phần Player từ đối tượng va chạm
            Player player = other.gameObject.GetComponent<Player>();

            // Đánh dấu đối tượng Player là bị nhiễm độc
            player._IsPoison = true;
        }
    }
}