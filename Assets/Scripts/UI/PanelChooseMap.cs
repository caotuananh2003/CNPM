using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelChooseMap : MonoBehaviour {

    // Biến lưu trữ animator của chính đối tượng này
    private Animator anim; 

    // Mảng hình ảnh đại diện số xu
    public Image[] _ListNumCoin;

    // Mảng hình ảnh đại diện số người chơi
    public Image[] _ListNumPlayer;

    // Mảng sprite đại diện cho các bản đồ
    public Sprite[] _ListSpriteMap;

    // Hình ảnh hiện tại của bản đồ được chọn
    public Image _ImgMap;

    // Số người chơi hiện tại
    int _NumPlayer = 3; 

    // Chỉ số bản đồ hiện tại
    int _IndexMap = 0;

    // Chỉ số giới hạn bản đồ cao nhất có thể chọn
    int _IndexMapMax = 2;

    // Script xử lý các con số được hiển thị
    NumberTextScale _NumberText;

    // Controller của game
    GameController _GameController;

    // Quản lý giao diện người dùng
    UIManager _UIManager;

    // Controller âm thanh
    SoundController _SoundController;

    // Khởi tạo đối tượng
    void Start () {
        // Tìm và lưu tham chiếu tới các script cần thiết
        _NumberText = FindObjectOfType<NumberTextScale>();
        _GameController = FindObjectOfType<GameController>();
        _UIManager = FindObjectOfType<UIManager>();
        anim = gameObject.GetComponent<Animator>();
        _SoundController = FindObjectOfType<SoundController>();
        anim.enabled = false; // Vô hiệu hóa animator khi bắt đầu
    }

    // Hàm xử lý nút "Quay lại" trong giao diện
    public void btnBack()
    {
        _SoundController.PlayClickSound(true); // Phát âm thanh click
        anim.enabled = true; // Bật animator
        anim.Play("PanelChooseMapOut"); // Chạy animation ẩn giao diện
    }

    /// <summary>
    /// Chọn người chơi tiếp theo
    /// </summary>
    public void NextPlayer()
    {
        _SoundController.PlayClickSound(true); // Phát âm thanh click
        _NumPlayer++; // Tăng số người chơi

        // Nếu vượt quá giới hạn, quay lại từ 1
        if (_NumPlayer > 5)
        {
            _NumPlayer = 1;
        }

        // Cập nhật hiển thị số người chơi
        _NumberText.SetNumText1(_NumPlayer, _ListNumPlayer);
    }

    /// <summary>
    /// Chọn người chơi trước đó
    /// </summary>
    public void PrePlayer()
    {
        _SoundController.PlayClickSound(true); // Phát âm thanh click
        _NumPlayer--; // Giảm số người chơi

        // Nếu nhỏ hơn 1, quay lại số tối đa
        if (_NumPlayer < 1)
        {
            _NumPlayer = 5;
        }

        // Cập nhật hiển thị số người chơi
        _NumberText.SetNumText1(_NumPlayer, _ListNumPlayer);
    }

    // Chọn bản đồ tiếp theo
    public void NextMap()
    {
        _SoundController.PlayClickSound(true); // Phát âm thanh click
        _IndexMap++; // Tăng chỉ số bản đồ

        // Nếu vượt quá số lượng bản đồ, quay lại từ đầu
        if (_IndexMap > 6)
        {
            _IndexMap = 0;
        }

        // Hiển thị sprite tương ứng với bản đồ được chọn
        if (_IndexMap <= _IndexMapMax)
        {
            _ImgMap.sprite = _ListSpriteMap[_IndexMap];
        }
        else
        {
            _ImgMap.sprite = _ListSpriteMap[_IndexMap + 4]; // Sử dụng sprite khác nếu vượt giới hạn
        }
    }

    // Chọn bản đồ trước đó
    public void PreMap()
    {
        _SoundController.PlayClickSound(true); // Phát âm thanh click
        _IndexMap--; // Giảm chỉ số bản đồ

        // Nếu nhỏ hơn 0, quay lại bản đồ cuối cùng
        if (_IndexMap < 0)
        {
            _IndexMap = 6;
        }

        // Hiển thị sprite tương ứng với bản đồ được chọn
        if (_IndexMap <= _IndexMapMax)
        {
            _ImgMap.sprite = _ListSpriteMap[_IndexMap];
        }
        else
        {
            _ImgMap.sprite = _ListSpriteMap[_IndexMap + 4]; // Sử dụng sprite khác nếu vượt giới hạn
        }
    }

    // Tải thông tin bản đồ dựa trên số vàng của người chơi
    public void LoadMapInfo()
    {
        // Cập nhật số vàng hiển thị
        _NumberText.SetNumText3(_GameController._Gold, _ListNumCoin);

        // Xác định giới hạn bản đồ có thể chọn dựa trên số vàng
        if (_GameController._Gold >= 100)
        {
            _IndexMapMax = 6;
        }
        else if (_GameController._Gold >= 50)
        {
            _IndexMapMax = 5;
        }
        else if (_GameController._Gold >= 25)
        {
            _IndexMapMax = 4;
        }
        else if (_GameController._Gold >= 10)
        {
            _IndexMapMax = 3;
        }
        else
        {
            _IndexMapMax = 2;
        }
    }

    /// <summary>
    /// Bắt đầu chơi game khi người chơi nhấn nút Start
    /// </summary>
    public void StartGame()
    {
        // Log thử ra console để kiểm tra hàm.
        Debug.Log("StartGame");

        // Kiểm tra nếu bản đồ được chọn vượt quá giới hạn thì không làm gì
        if (_IndexMap > _IndexMapMax)
        {
            Debug.Log("_IndexMap > _IndexMapMax");
            return;
        }

        _SoundController.PlayClickSound(true); // Phát âm thanh click
        Time.timeScale = 1; // Đặt thời gian về bình thường (nếu đã tạm dừng trước đó)

        // Gán số người chơi và chỉ số bản đồ vào controller
        _GameController._NumberPlayer = _NumPlayer;
        _GameController._IndexMap = _IndexMap;

        // Bắt đầu game
        _GameController.StartGame();

        // Hiển thị và ẩn các giao diện cần thiết
        _UIManager.ShowPopopTurnGreen(); // Hiển thị popup lượt chơi
        _UIManager.HidePanelArmory();
        _UIManager.HidePanelChooseMapAndNumPlayer();
        _UIManager.HidePanelLevel();
        _UIManager.HidePanelLoadGame();
        _UIManager.HidePanelChoosePlayer();
        _UIManager.HidePanelStartGame();
    }
}
