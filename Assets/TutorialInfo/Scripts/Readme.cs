using System;
using UnityEngine;

// Lớp Readme kế thừa từ ScriptableObject, là một dạng tài sản đặc biệt trong Unity.
// Được sử dụng để lưu trữ dữ liệu không liên kết trực tiếp với bất kỳ đối tượng nào trong cảnh (scene).
public class Readme : ScriptableObject
{
    // Biến lưu trữ một hình ảnh (icon) để hiển thị trong giao diện Readme.
    public Texture2D icon;

    // Chuỗi lưu tiêu đề của Readme, được hiển thị ở phần đầu trong giao diện.
    public string title;

    // Mảng các phần (sections) chứa nội dung của Readme, được hiển thị tuần tự trong Inspector.
    public Section[] sections;

    // Cờ (flag) để kiểm tra xem layout của Readme đã được tải trong Unity Editor hay chưa.
    public bool loadedLayout;

    // Lớp con (nested class) Section được sử dụng để mô tả từng phần nội dung của Readme.
    [Serializable] // Thuộc tính này giúp lớp Section có thể được lưu trữ và hiển thị trong Unity Editor.
    public class Section
    {
        // Tiêu đề của phần (section).
        public string heading;

        // Nội dung văn bản của phần.
        public string text;

        // Văn bản liên kết được hiển thị trong giao diện.
        public string linkText;

        // URL của liên kết, khi nhấp vào sẽ mở trong trình duyệt.
        public string url;
    }
}
