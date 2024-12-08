using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PanelStartGame : MonoBehaviour {
    //refrence for the pause menu panel in the hierarchy
    // Tham chiếu đến panel chọn người chơi (PanelChoosePlayer) trong hierarchy
    public GameObject PanelChoosePlayer;

    // Danh sách các nút trên giao diện (như nút nhạc, âm thanh)
    public Image[] _ListButton;

    // Tham chiếu đến component Animator để điều khiển hoạt ảnh
    private Animator anim;

    // Các biến tham chiếu đến các controller khác trong game
    SoundController _SoundController;
    GameController _GameController;
    BackgoundSoundController _BgSound;

    // Use this for initialization
    // Use this for initialization
    void Awake()
    {
        // Đọc trạng thái âm thanh từ file lưu trữ (nếu có)
        string sound = ReadWriteFileText.GetStringFromPrefab(Data._LinkSound);

        // Nếu file không có dữ liệu, thiết lập giá trị mặc định cho nhạc và âm thanh
        if (sound == "")
        {
            _GameController = FindObjectOfType<GameController>();
            _GameController._GameState._Music = true;
            _GameController._GameState._Sound = true;
        }
        else
        {
            // Nếu có dữ liệu, đọc và gán giá trị nhạc và âm thanh
            _GameController = FindObjectOfType<GameController>();
            string[] arrSound = sound.Split('#');
            _GameController._GameState._Music = bool.Parse(arrSound[0]);
            _GameController._GameState._Sound = bool.Parse(arrSound[1]);
        }

        // Tải số vàng hiện tại từ dữ liệu lưu trữ
        _GameController = FindObjectOfType<GameController>();
        //===========
        _GameController._Gold= _GameController.LoadGold();
    }

    // Hàm khởi tạo logic game, chạy ngay sau Awake()
    void Start () {
        // Kích hoạt panel chọn người chơi
        PanelChoosePlayer.SetActive(true);

        // Lấy component Animator từ panel chọn người chơi
        anim = PanelChoosePlayer.GetComponent<Animator>();

        // Vô hiệu hóa animator để không chạy hoạt ảnh mặc định
        anim.enabled = false;

        // Tạm dừng thời gian trong game
        Time.timeScale = 0;

        // Tìm các controller khác trong Scene
        _SoundController = FindObjectOfType<SoundController>();
        _BgSound = FindObjectOfType<BackgoundSoundController>();

        // Thiết lập màu sắc nút nhạc và âm thanh dựa trên trạng thái hiện tại
        if (_GameController._GameState._Music) _ListButton[0].color = Color.white;
        else _ListButton[0].color = Color.grey;

        if (_GameController._GameState._Sound) _ListButton[1].color = Color.white;
        else _ListButton[1].color = Color.grey;
	}


    //function to pause the game ???
    // Hàm để hiển thị panel chọn người chơi
    public void CallPanelChoosePlayer()
    {
        _SoundController.PlayClickSound(true); // Phát âm thanh khi click
        anim.enabled = true;                 // Bật animator
        anim.Play("PanelChoosePlayerIn");    // Chạy hoạt ảnh xuất hiện panel
    }

    // Hàm để đóng panel chọn người chơi
    public void UnpauseGame()
    {
        _SoundController.PlayClickSound(true); // Phát âm thanh khi click
        anim.Play("SlideOut");               // Chạy hoạt ảnh ẩn panel
    }

    // Hàm xử lý khi nhấn nút nhạc (Music)
    public void btnMusic_Click()
    {
        // Đảo trạng thái nhạc (Bật -> Tắt, Tắt -> Bật)
        if (_GameController._GameState._Music)
        {
            _GameController._GameState._Music = false;
            _BgSound.PlayBackground(false); // Tắt nhạc nền
        }
        else
        {
            _GameController._GameState._Music = true;
            _BgSound.PlayBackground(true); // Bật nhạc nền
        }

        // Lưu trạng thái nhạc và âm thanh vào file
        ReadWriteFileText.SaveStringToPrefab(Data._LinkSound,_GameController._GameState._Music + "#" + _GameController._GameState._Sound);

        // Cập nhật màu sắc nút nhạc dựa trên trạng thái
        if (_GameController._GameState._Music) _ListButton[0].color = Color.white;
        else _ListButton[0].color = Color.grey;
    }

    // Hàm xử lý khi nhấn nút âm thanh (Sound)
    public void btnSound_Click()
    {
        // Đảo trạng thái âm thanh (Bật -> Tắt, Tắt -> Bật)
        if (_GameController._GameState._Sound)
        {
            _GameController._GameState._Sound = false;
        }
        else
        {
            _GameController._GameState._Sound = true;
        }

        // Lưu trạng thái nhạc và âm thanh vào file
        ReadWriteFileText.SaveStringToPrefab(Data._LinkSound, _GameController._GameState._Music + "#" + _GameController._GameState._Sound);

        // Cập nhật màu sắc nút âm thanh dựa trên trạng thái
        if (_GameController._GameState._Sound) _ListButton[1].color = Color.white;
        else _ListButton[1].color = Color.grey;
    }

    // Hàm để hiển thị panel đánh giá trò chơi
    public void CallPanelRate()
    {
        Debug.Log("Rating");
        //_SoundController.PlayClickSound(true); // Phát âm thanh khi click
        //anim.enabled = true;                 // Bật animator
        //anim.Play("Tên anim");    // Chạy hoạt ảnh xuất hiện panel
    }
}
