using UnityEngine;
using System.Collections;

public class Pick_ShieldController : MonoBehaviour
{
    // Đếm số lượt (turn) đã bảo vệ.
    int _numTurn = 0;

    // Cờ để kiểm soát việc đếm số lượt.
    bool _isReally = false;

    // Số lượt bảo vệ tối đa.
    public int _NumTurn = 4;

    // Tham chiếu đến các đối tượng quản lý trò chơi và giao diện.
    GameController _GameController;
    UIManager _UIManager;

    // Cờ kiểm tra xem hiệu ứng đã được kích hoạt chưa.
    bool _checkIssHit = false;

    // Tham chiếu đến đối tượng điều khiển âm thanh.
    SoundController _SoundController;

    // Khởi tạo, đặt trạng thái ban đầu cho lớp.
    void Start()
    {
        // Tìm và liên kết đối tượng GameController.
        _GameController = FindObjectOfType<GameController>();

        // Lấy thông tin về đối tượng người chơi đang sử dụng khiên.
        Player player = gameObject.GetComponentInParent<Player>();

        // Tìm và liên kết các đối tượng quản lý giao diện và âm thanh.
        _UIManager = FindObjectOfType<UIManager>();
        _SoundController = FindObjectOfType<SoundController>();

        // Phát âm thanh kích hoạt khiên bảo vệ.
        _SoundController.PlayShieldSound(true);

        // Kích hoạt trạng thái bảo vệ cho người chơi.
        player._ISShield = true;
    }

    // Hàm Update, được gọi mỗi khung hình.
    void Update()
    {
        // Nếu không trong trạng thái chuyển đổi lượt và hiệu ứng chưa được kích hoạt.
        if (_GameController._GameState._IsChangding == false && _isReally == false)
        {
            // Kích hoạt cờ cho phép đếm lượt.
            _isReally = true;

            // Nếu hiệu ứng chưa được kích hoạt.
            if (!_checkIssHit)
            {
                // Giảm số lượng khiên của người chơi hoặc kẻ địch.
                if (_GameController._GameObj.tag == "Player")
                    _GameController._ListStatePickPlayer[6].Ammo -= 1;
                else
                    _GameController._ListStatePickEnemy[6].Ammo -= 1;

                // Đánh dấu rằng đã có hành động kích hoạt khiên.
                _GameController._CheckHit = true;

                // Đặt lại hình ảnh chọn trong giao diện.
                _UIManager.ResetImgPick();

                // Nếu lượt hiện tại là của kẻ địch, chuyển lượt sang người chơi.
                GameController gameController = FindObjectOfType<GameController>();
                if (gameController._GameObj.tag == "Enemy")
                {
                    gameController.ChangeTurn();
                }
            }

            // Đánh dấu hiệu ứng đã được kích hoạt.
            _checkIssHit = true;
        }

        // Nếu hiệu ứng đã kích hoạt, đang trong trạng thái chuyển đổi lượt, và hiệu ứng đã được xử lý.
        if (_isReally && _GameController._GameState._IsChangding && _checkIssHit)
        {
            // Đặt lại cờ để chờ lượt tiếp theo.
            _isReally = false;

            // Tăng số lượt đã bảo vệ.
            _numTurn++;

            // Nếu đã hết số lượt bảo vệ tối đa.
            if (_numTurn == _NumTurn)
            {
                // Tắt trạng thái bảo vệ của người chơi.
                Player player = gameObject.GetComponentInParent<Player>();
                player._ISShield = false;

                // Hủy đối tượng khiên.
                Destroy(gameObject);
            }
        }
    }
}
