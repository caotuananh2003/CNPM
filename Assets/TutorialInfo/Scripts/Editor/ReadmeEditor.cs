using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;

// Đánh dấu lớp này là một trình chỉnh sửa tùy chỉnh (CustomEditor) dành cho lớp Readme.
// Lớp này được sử dụng để hiển thị nội dung tệp Readme trong Unity Editor.
[CustomEditor(typeof(Readme))]
[InitializeOnLoad]
public class ReadmeEditor : Editor
{
    // Biến lưu trạng thái phiên làm việc (session) để kiểm tra xem Readme đã được hiển thị chưa.
    static string s_ShowedReadmeSessionStateName = "ReadmeEditor.showedReadme";

    // Đường dẫn mặc định đến thư mục chứa Readme.
    static string s_ReadmeSourceDirectory = "Assets/TutorialInfo";

    // Khoảng cách (padding) giữa các thành phần trong giao diện.
    const float k_Space = 16f;

    // Constructor tĩnh: Gọi khi lớp được tải.
    // Gắn một hành động để tự động chọn Readme khi Unity Editor khởi động.
    static ReadmeEditor()
    {
        EditorApplication.delayCall += SelectReadmeAutomatically;
    }

    // Phương thức xóa thư mục chứa nội dung hướng dẫn (Tutorial).
    static void RemoveTutorial()
    {
        // Hiển thị hộp thoại xác nhận trước khi xóa.
        if (EditorUtility.DisplayDialog("Remove Readme Assets",
            $"All contents under {s_ReadmeSourceDirectory} will be removed, are you sure you want to proceed?",
            "Proceed",
            "Cancel"))
        {
            // Kiểm tra nếu thư mục tồn tại, xóa nó cùng với tệp meta.
            if (Directory.Exists(s_ReadmeSourceDirectory))
            {
                FileUtil.DeleteFileOrDirectory(s_ReadmeSourceDirectory);
                FileUtil.DeleteFileOrDirectory(s_ReadmeSourceDirectory + ".meta");
            }
            else
            {
                Debug.Log($"Could not find the Readme folder at {s_ReadmeSourceDirectory}");
            }

            // Tìm và xóa tệp Readme nếu nó tồn tại.
            var readmeAsset = SelectReadme();
            if (readmeAsset != null)
            {
                var path = AssetDatabase.GetAssetPath(readmeAsset);
                FileUtil.DeleteFileOrDirectory(path + ".meta");
                FileUtil.DeleteFileOrDirectory(path);
            }

            // Làm mới cơ sở dữ liệu tài sản của Unity.
            AssetDatabase.Refresh();
        }
    }

    // Tự động chọn Readme khi Unity Editor khởi động lần đầu.
    static void SelectReadmeAutomatically()
    {
        // Kiểm tra xem Readme đã được hiển thị trong phiên làm việc này chưa.
        if (!SessionState.GetBool(s_ShowedReadmeSessionStateName, false))
        {
            var readme = SelectReadme();
            SessionState.SetBool(s_ShowedReadmeSessionStateName, true);

            // Nếu Readme chưa được tải layout, tải layout mặc định.
            if (readme && !readme.loadedLayout)
            {
                LoadLayout();
                readme.loadedLayout = true;
            }
        }
    }

    // Tải layout giao diện từ tệp Layout.wlt trong thư mục TutorialInfo.
    static void LoadLayout()
    {
        var assembly = typeof(EditorApplication).Assembly;
        var windowLayoutType = assembly.GetType("UnityEditor.WindowLayout", true);
        var method = windowLayoutType.GetMethod("LoadWindowLayout", BindingFlags.Public | BindingFlags.Static);
        method.Invoke(null, new object[] { Path.Combine(Application.dataPath, "TutorialInfo/Layout.wlt"), false });
    }

    // Tìm và chọn đối tượng Readme trong dự án.
    static Readme SelectReadme()
    {
        // Tìm tất cả tài sản có tên "Readme" và kiểu dữ liệu là Readme.
        var ids = AssetDatabase.FindAssets("Readme t:Readme");
        if (ids.Length == 1)
        {
            var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));

            // Chọn đối tượng Readme trong Unity Editor.
            Selection.objects = new UnityEngine.Object[] { readmeObject };

            return (Readme)readmeObject;
        }
        else
        {
            Debug.Log("Couldn't find a readme");
            return null;
        }
    }

    // Hiển thị tiêu đề của Readme trong Inspector.
    protected override void OnHeaderGUI()
    {
        var readme = (Readme)target;
        Init();

        // Tính toán kích thước biểu tượng.
        var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);

        // Giao diện phần tiêu đề.
        GUILayout.BeginHorizontal("In BigTitle");
        {
            if (readme.icon != null)
            {
                GUILayout.Space(k_Space);
                GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
            }
            GUILayout.Space(k_Space);
            GUILayout.BeginVertical();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(readme.title, TitleStyle);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
    }

    // Hiển thị nội dung của Readme trong Inspector.
    public override void OnInspectorGUI()
    {
        var readme = (Readme)target;
        Init();

        // Lặp qua từng phần trong Readme và hiển thị nội dung.
        foreach (var section in readme.sections)
        {
            if (!string.IsNullOrEmpty(section.heading))
            {
                GUILayout.Label(section.heading, HeadingStyle);
            }

            if (!string.IsNullOrEmpty(section.text))
            {
                GUILayout.Label(section.text, BodyStyle);
            }

            if (!string.IsNullOrEmpty(section.linkText))
            {
                // Hiển thị liên kết. Khi nhấp vào, mở URL.
                if (LinkLabel(new GUIContent(section.linkText)))
                {
                    Application.OpenURL(section.url);
                }
            }

            GUILayout.Space(k_Space);
        }

        // Nút xóa các tài sản liên quan đến Readme.
        if (GUILayout.Button("Remove Readme Assets", ButtonStyle))
        {
            RemoveTutorial();
        }
    }

    // Biến kiểm tra xem các kiểu giao diện đã được khởi tạo chưa.
    bool m_Initialized;

    // Các kiểu giao diện (GUI Styles) để hiển thị Readme trong Inspector.
    GUIStyle LinkStyle
    {
        get { return m_LinkStyle; }
    }
    [SerializeField]
    GUIStyle m_LinkStyle;

    GUIStyle TitleStyle
    {
        get { return m_TitleStyle; }
    }
    [SerializeField]
    GUIStyle m_TitleStyle;

    GUIStyle HeadingStyle
    {
        get { return m_HeadingStyle; }
    }
    [SerializeField]
    GUIStyle m_HeadingStyle;

    GUIStyle BodyStyle
    {
        get { return m_BodyStyle; }
    }
    [SerializeField]
    GUIStyle m_BodyStyle;

    GUIStyle ButtonStyle
    {
        get { return m_ButtonStyle; }
    }
    [SerializeField]
    GUIStyle m_ButtonStyle;

    // Khởi tạo các kiểu giao diện (GUI Styles).
    void Init()
    {
        if (m_Initialized)
            return;

        m_BodyStyle = new GUIStyle(EditorStyles.label);
        m_BodyStyle.wordWrap = true;
        m_BodyStyle.fontSize = 14;
        m_BodyStyle.richText = true;

        m_TitleStyle = new GUIStyle(m_BodyStyle);
        m_TitleStyle.fontSize = 26;

        m_HeadingStyle = new GUIStyle(m_BodyStyle);
        m_HeadingStyle.fontStyle = FontStyle.Bold;
        m_HeadingStyle.fontSize = 18;

        m_LinkStyle = new GUIStyle(m_BodyStyle);
        m_LinkStyle.wordWrap = false;
        m_LinkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
        m_LinkStyle.stretchWidth = false;

        m_ButtonStyle = new GUIStyle(EditorStyles.miniButton);
        m_ButtonStyle.fontStyle = FontStyle.Bold;

        m_Initialized = true;
    }

    // Tạo một liên kết có thể nhấp vào trong giao diện Inspector.
    bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
    {
        var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

        // Vẽ đường gạch chân cho liên kết.
        Handles.BeginGUI();
        Handles.color = LinkStyle.normal.textColor;
        Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
        Handles.color = Color.white;
        Handles.EndGUI();

        // Thêm hiệu ứng chuột khi di chuyển qua liên kết.
        EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

        // Kích hoạt hành động khi nhấp chuột.
        return GUI.Button(position, label, LinkStyle);
    }
}
