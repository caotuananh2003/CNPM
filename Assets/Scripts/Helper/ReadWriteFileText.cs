using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization;

#if NETFX_CORE
// Sử dụng các API cụ thể cho .NET Framework trên nền tảng Universal Windows Platform (UWP)

 using System.Threading.Tasks;
 using Windows.Storage;
 using Windows.Storage.Streams;
#endif

//Chi tiết giải thích các phần chính:
//1. Đọc / Ghi file văn bản:

//- WriteFile: Ghi nội dung vào file tại đường dẫn chỉ định, ghi đè nếu file đã tồn tại.
//- ReadFile: Đọc toàn bộ nội dung từ file tại đường dẫn chỉ định.

//2. Quản lý file mặc định với IsolatedStorage:
//- WriteDefaultValues và DisplayValues: Ý tưởng là lưu và đọc file từ không gian lưu trữ cô lập (IsolatedStorage), nhưng phần cài đặt đã bị comment.

//3. Sử dụng PlayerPrefs để lưu trữ chuỗi:
//- SaveStringToPrefab: Lưu chuỗi vào PlayerPrefs bằng khóa cụ thể.
//- GetStringFromPrefab: Lấy giá trị chuỗi từ PlayerPrefs nếu khóa tồn tại.

//Mục đích của lớp:
//1. Lớp ReadWriteFileText cung cấp các phương pháp tiện lợi để:

//- Ghi / đọc dữ liệu từ file văn bản thông thường.
//- Lưu và lấy dữ liệu bằng PlayerPrefs.
//- Chuẩn bị xử lý file với IsolatedStorage (mặc dù chưa hoàn thiện).


// Lớp hỗ trợ đọc/ghi file văn bản và quản lý chuỗi dữ liệu
public class ReadWriteFileText
{
    // Hàm ghi nội dung vào file tại đường dẫn được cung cấp
    public static void WriteFile(string content, string path)
    {
        // Tạo đối tượng StreamWriter để ghi file
        StreamWriter swriter = new StreamWriter(path, false); // Tham số 'false' đảm bảo file cũ sẽ bị ghi đè
        swriter.Write(content); // Ghi nội dung vào file
        swriter.Flush(); // Đảm bảo tất cả dữ liệu được ghi ra file ngay lập tức
        swriter.Close(); // Đóng StreamWriter để giải phóng tài nguyên
    }

    // Hàm đọc nội dung từ file tại đường dẫn được cung cấp
    public static string ReadFile(string path)
    {
        string str = ""; // Chuỗi lưu nội dung file
        // Tạo đối tượng StreamReader để đọc file
        StreamReader sreader = new StreamReader(path);
        str = sreader.ReadToEnd(); // Đọc toàn bộ nội dung file vào chuỗi
        sreader.Close(); // Đóng StreamReader để giải phóng tài nguyên
        return str; // Trả về nội dung file
    }


    //=================
    // Hàm ghi nội dung mặc định vào file sử dụng IsolatedStorage
    public static void WriteDefaultValues(string fileName, string Content)
    {
        // Phần này bị comment nhưng mô tả ý tưởng:
        // - Kiểm tra xem file đã tồn tại trong IsolatedStorage chưa
        // - Nếu file tồn tại, ghi đè nội dung mới vào file
        // - Nếu file không tồn tại, tạo file mới và ghi nội dung mặc định vào

        // Gợi ý cách sử dụng IsolatedStorageFile để quản lý file:
        IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
        if (isoStore.GetFileNames(fileName).Length > 0)
        {
            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.Create, isoStore))
            {
                using (StreamWriter writer = new StreamWriter(isoStream))
                {
                    writer.WriteLine(Content);
                }
            }
        }
        else
        {
            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.CreateNew, isoStore))
            {
                using (StreamWriter writer = new StreamWriter(isoStream))
                {
                    writer.WriteLine(Content);
                }
            }
        }
    }

    // Hàm đọc nội dung từ file trong IsolatedStorage (phần cài đặt bị comment)
    public static string DisplayValues(string fileName)
    {
        string str = ""; // Chuỗi để lưu nội dung file

        // Ý tưởng (comment bên dưới):
        // - Mở file trong IsolatedStorage và đọc nội dung
        // - Trả về chuỗi chứa nội dung file

        IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
        if (isoStore.GetFileNames(fileName).Length > 0)
        {
            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.Open, isoStore))
            {
                using (StreamReader reader = new StreamReader(isoStream))
                {
                    str = reader.ReadToEnd();
                }
            }
        }
        return str; // Trả về chuỗi rỗng (do phần cài đặt bị bỏ qua)
    }

    #region============READ AND SAVE STRING TO PLAYERPREFS==========
    // Hàm lưu chuỗi vào PlayerPrefs với khóa và nội dung được chỉ định
    public static void SaveStringToPrefab(string key, string content)
    {
        PlayerPrefs.SetString(key, content); // Lưu chuỗi vào PlayerPrefs với khóa 'key'
        PlayerPrefs.Save(); // Lưu lại toàn bộ dữ liệu PlayerPrefs
    }

    // Hàm đọc chuỗi từ PlayerPrefs với khóa được chỉ định
    public static string GetStringFromPrefab(string key)
    {
        string result = ""; // Chuỗi lưu kết quả
        if (PlayerPrefs.HasKey(key)) // Kiểm tra xem khóa có tồn tại trong PlayerPrefs không
        {
            result = PlayerPrefs.GetString(key); // Nếu tồn tại, đọc giá trị từ PlayerPrefs
        }
        return result; // Trả về chuỗi đọc được (hoặc rỗng nếu không có giá trị)
    }
    #endregion
}

