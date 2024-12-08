using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Lớp Data: Lưu trữ thông tin tĩnh (cấu hình) cần thiết cho trò chơi.
// Các trường dữ liệu trong lớp này dùng để quản lý đường dẫn file hoặc các giá trị cấu hình liên quan.
public class Data
{
    // Đường dẫn đến file chứa thông tin về các vật phẩm (Pick items)
    public static string _LinkPic = "Pick.txt";

    // Chuỗi cấu hình số lượt sử dụng tối đa cho từng loại súng trong trò chơi.
    // Các số được phân cách bởi dấu '*' và đại diện cho lượt sử dụng của các loại súng.
    // Ví dụ: 
    // - Giá trị "1*1*1" nghĩa là 3 loại súng đầu tiên chỉ có 1 lượt sử dụng.
    // - Giá trị "3" ở vị trí thứ 5 nghĩa là loại súng thứ 5 có tối đa 3 lượt.
    public static string _LinkAmmo = "1*1*1*1*3*1*1*1*1*2*1*1*1*1*1*1*1*1";

    // Đường dẫn đến file chứa thông tin về âm thanh (Sound settings)
    public static string _LinkSound = "Sound.txt";

    // Đường dẫn đến file chứa thông tin về vàng (Gold)
    // File này có thể lưu số vàng hiện tại của người chơi, được dùng để mua vật phẩm hoặc nâng cấp.
    public static string _Gold = "Gold.txt";
}
