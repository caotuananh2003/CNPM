using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundController : MonoBehaviour
{
    public List<AudioClip> EffectUrl; // Danh sách chứa các clip âm thanh hiệu ứng có thể phát
    private AudioSource audio; // Biến để lưu trữ thành phần AudioSource, sẽ được sử dụng để phát âm thanh
    GameController _GameController; // Biến để lưu trữ tham chiếu đến GameController

    // Use this for initialization
    void Start()
    {
        audio = GetComponent<AudioSource>(); // Lấy thành phần AudioSource gắn liền với đối tượng này
        _GameController = FindObjectOfType<GameController>(); // Tìm và lưu trữ GameController đang hoạt động trong scene
    }

    // Phương thức phát âm thanh click, nhận vào tham số boolean isShow
    public void PlayClickSound(bool isShow)
    {
        // Nếu isShow là true và âm thanh đang được bật trong trạng thái game
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[0]); // Phát âm thanh click
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh nổ, nhận vào tham số boolean isShow
    public void PlayExplosionSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[1]); // Phát âm thanh nổ
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh máy bay, nhận vào tham số boolean isShow
    public void PlayAircraftSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[2]); // Phát âm thanh máy bay
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh bazooka, nhận vào tham số boolean isShow
    public void PlayBazookaSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[3]); // Phát âm thanh bazooka
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh va chạm, nhận vào tham số boolean isShow
    public void PlayBounceSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[4]); // Phát âm thanh va chạm
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh khoan, nhận vào tham số boolean isShow
    public void PlayDrillSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[5]); // Phát âm thanh khoan
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh bắn, nhận vào tham số boolean isShow
    public void PlayFireSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[6]); // Phát âm thanh bắn
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh kính vỡ, nhận vào tham số boolean isShow
    public void PlayGlassSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[7]); // Phát âm thanh kính vỡ
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh laser, nhận vào tham số boolean isShow
    public void PlayLaserSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[8]); // Phát âm thanh laser
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh bắn minigun, nhận vào tham số boolean isShow
    public void PlayMinigun_shotSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
        {
            audio.clip = EffectUrl[9]; // Gán clip âm thanh minigun
            audio.Play(); // Phát âm thanh
            audio.loop = true; // Thiết lập âm thanh lặp lại
        }
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh đấm, nhận vào tham số boolean isShow
    public void PlayPunchSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[10]); // Phát âm thanh đấm
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh lăn, nhận vào tham số boolean isShow
    public void PlayRollSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[11]); // Phát âm thanh lăn
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh khi có khiên, nhận vào tham số boolean isShow
    public void PlayShieldSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[12]); // Phát âm thanh khiên
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh bắn shotgun, nhận vào tham số boolean isShow
    public void PlayShotgun_shotSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[13]); // Phát âm thanh bắn shotgun
        else
            audio.Stop(); // Dừng âm thanh
    }

    // Phương thức phát âm thanh dịch chuyển, nhận vào tham số boolean isShow
    public void PlayTeleportSound(bool isShow)
    {
        if (isShow && _GameController._GameState._Sound)
            audio.PlayOneShot(EffectUrl[14]); // Phát âm thanh dịch chuyển
        else
            audio.Stop(); // Dừng âm thanh
    }
}
