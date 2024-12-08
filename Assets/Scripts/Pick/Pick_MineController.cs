using UnityEngine;
using System.Collections;

public class Pick_MineController : MonoBehaviour
{
    // GameObject đại diện cho hiệu ứng nổ khi mìn phát nổ.
    public GameObject Explose;

    // Cờ xác định trạng thái mìn đã được kích hoạt hay chưa.
    bool _IsReally = false;

    // Cờ xác định trạng thái khởi tạo mìn.
    public bool _IsInit = false;

    // Cờ xác định mìn đã sẵn sàng hoạt động hay chưa.
    bool _IsStart = false;

    // Biến đếm thời gian cho quá trình khởi tạo.
    float _timeCount = 0;

    // Cờ xác định mìn đã va chạm hay chưa.
    bool _checkIssHit = false;

    // Tham chiếu đến các đối tượng quản lý trò chơi, giao diện, và âm thanh.
    GameController _GameController;
    UIManager _UIManager;
    SoundController _SoundController;

    // Hàm khởi tạo, được gọi khi đối tượng bắt đầu hoạt động.
    void Start()
    {
        // Tìm và liên kết đối tượng GameController trong scene.
        _GameController = FindObjectOfType<GameController>();

        // Đặt vị trí của mìn trong không gian, đảm bảo nằm ở lớp Z = -1 để hiển thị chính xác.
        gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -1);

        // Tìm và liên kết đối tượng UIManager và SoundController trong scene.
        _UIManager = FindObjectOfType<UIManager>();
        _SoundController = FindObjectOfType<SoundController>();
    }

    // Hàm Update, được gọi mỗi khung hình.
    void Update()
    {
        // Nếu mìn đang trong trạng thái khởi tạo.
        if (_IsInit)
        {
            // Tăng bộ đếm thời gian.
            _timeCount += Time.deltaTime;

            // Nếu thời gian khởi tạo vượt quá 5 giây.
            if (_timeCount > 5)
            {
                // Kết thúc trạng thái khởi tạo, chuyển sang trạng thái hoạt động.
                _IsInit = false;
                _IsStart = true;

                // Kích hoạt collider để mìn có thể va chạm.
                gameObject.GetComponent<BoxCollider2D>().enabled = true;

                // Cập nhật trạng thái trong animator.
                Animator ani = gameObject.GetComponent<Animator>();
                ani.SetBool("Init", false);
                ani.SetBool("Start", true);

                // Giảm số lượng đạn của người chơi hoặc kẻ địch tùy thuộc vào ai đặt mìn.
                if (_GameController._GameObj.tag == "Player")
                    _GameController._ListStatePickPlayer[5].Ammo -= 1;
                else
                    _GameController._ListStatePickEnemy[5].Ammo -= 1;
            }
        }

        // Nếu mìn đã va chạm, không thực hiện xử lý thêm.
        if (_checkIssHit) return;

        // Nếu không phải lượt của người chơi, không thực hiện xử lý.
        if (_GameController._TypeGame == 0 && _GameController._GameState._IsEnemyStart) return;

        // Kiểm tra nếu vị trí con trỏ chuột nằm dưới một vùng xác định hoặc giao diện popup đang mở.
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < _GameController.INood.transform.position.y || _UIManager._ListPopup[0].activeSelf) return;

        // Nếu mìn chưa thực sự được kích hoạt.
        if (!_IsReally)
        {
            // Kiểm tra nếu người chơi nhấp chuột trái.
            if (Input.GetMouseButtonDown(0))
            {
                // Lấy đối tượng tại vị trí con trỏ chuột.
                GameObject obj = Helper.GetGameObjectAtPossition();
                if (obj == null) return;

                // Nếu đối tượng là mìn.
                if (obj.tag == "Pick")
                {
                    // Giảm số lượng đạn và kích hoạt mìn.
                    if (_GameController._GameObj.tag == "Player")
                        _GameController._ListStatePickPlayer[5].Ammo -= 1;
                    else
                        _GameController._ListStatePickEnemy[5].Ammo -= 1;

                    _GameController._CheckHit = true;
                    _UIManager.ResetImgPick();
                    ActivePick_Mine();
                }
            }
        }
    }

    // Hàm kích hoạt mìn, làm cho mìn bị ảnh hưởng bởi trọng lực.
    public void ActivePick_Mine()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        _IsReally = true;
    }

    // Hàm xử lý khi mìn va chạm với một đối tượng khác.
    void OnCollisionEnter2D(Collision2D other)
    {
        // Nếu mìn chưa được kích hoạt, không làm gì.
        if (!_IsReally) return;

        // Nếu mìn va chạm với bản đồ.
        if (other.gameObject.tag == "Map")
        {
            // Dừng mìn, vô hiệu hóa collider, và đặt vào trạng thái khởi tạo.
            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            _IsInit = true;

            // Cập nhật trạng thái trong animator.
            Animator ani = gameObject.GetComponent<Animator>();
            ani.SetBool("Init", true);
            ani.SetBool("Start", false);

            // Gắn mìn vào bản đồ.
            gameObject.transform.parent = other.gameObject.transform;

            // Đánh dấu rằng mìn đã va chạm.
            _checkIssHit = true;
        }
    }

    // Hàm xử lý khi một đối tượng khác đi vào vùng kích hoạt của mìn.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu mìn chưa sẵn sàng hoạt động, không làm gì.
        if (!_IsStart) return;

        // Nếu đối tượng là người chơi hoặc kẻ địch.
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            // Phát âm thanh nổ, tạo hiệu ứng nổ, và hủy mìn.
            _SoundController.PlayExplosionSound(true);
            Instantiate(Explose, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
