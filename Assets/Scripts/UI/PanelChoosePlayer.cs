using UnityEngine;
using System.Collections;

public class PanelChoosePlayer : MonoBehaviour {

    // Tham chiếu đến các panel khác trong giao diện
    public GameObject _PanelLevel;          // Panel chọn cấp độ chơi (Level)
    public GameObject _PanelChooseMap;      // Panel chọn bản đồ (Map)
    public GameObject _PanelArmory;         // Panel chọn vũ khí (Armory)

    // Các biến Animator để điều khiển hoạt ảnh của các panel
    private Animator _animPanelLevel;       // Animator cho _PanelLevel
    private Animator _animPanelChooseMap;   // Animator cho _PanelChooseMap

    // Các controller dùng để quản lý trạng thái và âm thanh
    GameController _GameController;           // Quản lý logic game
    SoundController _SoundController;         // Quản lý âm thanh


    // Animator của chính đối tượng hiện tại (PanelChoosePlayer)
    private Animator anim;//Anim của chính nó

	// Use this for initialization
	void Start () {
        // Kích hoạt _PanelLevel để đảm bảo panel này luôn hiển thị
        _PanelLevel.SetActive(true);

        // Lấy component Animator của PanelChoosePlayer
        anim = gameObject.GetComponent<Animator>();

        // Lấy Animator của các panel liên quan
        _animPanelLevel = _PanelLevel.GetComponent<Animator>();
        _animPanelChooseMap = _PanelChooseMap.GetComponent<Animator>();

        // Tắt Animator của các panel để ngăn chúng tự động chạy hoạt ảnh mặc định
        _animPanelLevel.enabled = false;
        _animPanelChooseMap.enabled = false;
        anim.enabled = false;

        // Tìm SoundController trong Scene để phát âm thanh
        _SoundController = FindObjectOfType<SoundController>();
    }

    // Hàm xử lý khi nhấn nút "Back" để quay lại giao diện trước
    public void btnBack()
    {
        _SoundController.PlayClickSound(true); // Phát âm thanh click
        anim.enabled = true;                  // Bật Animator của PanelChoosePlayer
        anim.Play("PanelChoosePlayerOut");    // Chạy hoạt ảnh để ẩn panel

        //set back the time scale to normal time scale
       // Time.timeScale = 1;
    }

    // Hàm xử lý khi chuyển sang giao diện chọn cấp độ chơi (Level)
    public void CallPanelLevel()
    {
        _SoundController.PlayClickSound(true); // Phát âm thanh click

        // Tìm GameController để quản lý trạng thái game
        _GameController = FindObjectOfType<GameController>();
        _GameController._TypeGame = 0;         // Đặt loại game là 0 (chế độ chọn cấp độ)

        // Bật Animator của _PanelLevel và chạy hoạt ảnh xuất hiện
        _animPanelLevel.enabled = true;
        _animPanelLevel.Play("PanelLevelIn");

        //freeze the timescale
        // Time.timeScale = 0;
    }

    // Hàm xử lý khi chuyển sang giao diện chọn bản đồ (Map)
    public void CallPanelChooseMap()
    {
        _SoundController.PlayClickSound(true); // Phát âm thanh click

        // Tìm GameController để quản lý trạng thái game
        _GameController = FindObjectOfType<GameController>();
        _GameController._TypeGame = 1;         // Đặt loại game là 1 (chế độ chọn bản đồ)

        // Lấy script PanelChooseMap và gọi hàm để tải thông tin bản đồ
        PanelChooseMap panel = _PanelChooseMap.GetComponent<PanelChooseMap>();
        panel.LoadMapInfo();                 // Tải thông tin bản đồ

        // Bật Animator của _PanelChooseMap và chạy hoạt ảnh xuất hiện
        _animPanelChooseMap.enabled = true;
        _animPanelChooseMap.Play("PanelChooseMapIn");
    }

    // Hàm xử lý khi chuyển sang giao diện chọn vũ khí (Armory)
    public void CallPanelArmory()
    {
        _SoundController.PlayClickSound(true); // Phát âm thanh click

        // Hiển thị _PanelArmory
        _PanelArmory.SetActive(true);

        // Lấy script PanelArmory và gọi hàm để tải menu chọn vũ khí
        PanelArmory panel = _PanelArmory.GetComponent<PanelArmory>();
        panel.LoadPickMenu();                // Tải menu chọn vũ khí
    }
}
