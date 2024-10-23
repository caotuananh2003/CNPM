using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Helper
{
    /// <summary>
    /// Lấy biên giới của camera so với bản đồ thế giới
    /// </summary>
    public static Bounds OrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }
    /// <summary>
    /// Lay đối tượng tại vị trí con trỏ chuột
    /// </summary>
    /// <returns></returns>
    public static GameObject GetGameObjectAtPossition()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }
    /// <summary>
    /// Get game obj tại một điểm trên bản đồ world
    /// </summary>
    public static GameObject GetGameObjectAtPossition(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }
    /// <summary>
    /// Tìm đối tượng con trong đối tượng cha
    /// </summary>
    /// <param name="objParent"></param>
    /// <param name="nameChild"></param>
    /// <returns></returns>
    public static GameObject FinObjectChildByName(GameObject objParent, string nameChild)
    {
        Transform[] tran = objParent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < tran.Length; i++)
        {
            if (tran[i].name == nameChild)
            {
                return tran[i].gameObject;
            }
        }
        return null;
    }
}

