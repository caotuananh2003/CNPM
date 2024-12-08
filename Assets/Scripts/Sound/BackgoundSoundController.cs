using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgoundSoundController : MonoBehaviour
{
    //// EffectUrl tạm thời chưa dùng đến, khi update game và thêm các nhạc nền thì sẽ nâng cấp thêm.
    //public List<AudioClip> EffectUrl; // Danh sách chứa các clip âm thanh có thể phát
    
    private AudioSource audio; // Biến để lưu trữ thành phần AudioSource, sẽ được sử dụng để phát âm thanh
    GameController _GameController; // Biến để lưu trữ tham chiếu đến GameController

    // Use this for initialization
    void Awake()
    {
        audio = GetComponent<AudioSource>(); // Lấy thành phần AudioSource gắn liền với đối tượng này
        _GameController = FindObjectOfType<GameController>(); // Tìm và lưu trữ GameController đang hoạt động trong scene
    }

    public void PlayBackground(bool isShow)
    {
        _GameController = FindObjectOfType<GameController>(); // Tìm lại GameController trong trường hợp đã bị thay đổi

        // Nếu isShow là true và nhạc nền đang được bật trong trạng thái game
        if (isShow && _GameController._GameState._Music)
        {
            audio.Play(); // Phát âm thanh
            audio.loop = true; // Thiết lập âm thanh lặp lại
        }
        else
        {
            audio.Stop(); // Dừng phát âm thanh nếu isShow là false
        }
    }
}
