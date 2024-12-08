using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundPlayerController : MonoBehaviour
{

    public List<AudioClip> EffectUrl; // Danh sách chứa các clip âm thanh hiệu ứng có thể phát
    private AudioSource audio; // Biến để lưu trữ thành phần AudioSource, sẽ được sử dụng để phát âm thanh

    // Use this for initialization
    void Start()
    {
        audio = GetComponent<AudioSource>(); // Lấy thành phần AudioSource gắn liền với đối tượng này
    }

    // Phương thức phát âm thanh nhảy, nhận vào tham số boolean isShow
    public void PlayJump(bool isShow)
    {
        // Nếu isShow là true, phát âm thanh nhảy
        if (isShow)
            audio.PlayOneShot(EffectUrl[0]); // Phát âm thanh nhảy từ danh sách EffectUrl
        else
            audio.Stop(); // Nếu isShow là false, dừng âm thanh
    }

    // Phương thức phát âm thanh bị thương, nhận vào tham số boolean isShow
    public void PlayHurt(bool isShow)
    {
        // Nếu isShow là true, phát âm thanh bị thương
        if (isShow)
            audio.PlayOneShot(EffectUrl[1]); // Phát âm thanh bị thương từ danh sách EffectUrl
        else
            audio.Stop(); // Nếu isShow là false, dừng âm thanh
    }
}
