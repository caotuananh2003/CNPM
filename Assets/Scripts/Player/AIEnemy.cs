using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIEnemy : MonoBehaviour {
   
    public LayerMask collisionMask;
    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    GameController _GameController;
    float _Distance = 100f;
    public float _MaxDestince = 2f;//Khỏng cách lớn nhất chấp nhận được để các loại vũ khí ở gần có thể bắn được mục tiêu
    public float _MinDestince = 2f;//Khoảng cách tối thiểu chấp nhận được sự sai số khi bắn tới mục tiêu
    public float _AngleTest = 0;
    BoxCollider2D collider;
    GameObject _Target;///Mục tiêu của lần bắn hiện tại
    UIManager _UIManager;
    // Use this for initialization
    void Start()
    {
        _GameController = FindObjectOfType<GameController>();
        _UIManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _targetReady = true;
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//_bullseye.position;
        }
    }

    // handles
    [SerializeField]
    private Transform _bullseye;    // target transform
    // Editor variables
    [Range(1f, 6f)]
    public float _targetRange; 
    private bool _targetReady;
    /// <summary>
    /// Trả về lực cần bắn với với góc bắn và vị trí đích biết trước
    /// </summary>
    public Vector3 Launch(float _angle, Vector3 tar)
    {
        // source and target positions
        Vector2 pos = transform.position;
        Vector2 target = tar;
        // distance between target and source
        float t1 = (((target.y - pos.y) * (1 - (0.1f * (target.y - pos.y)))) / Mathf.Tan(Mathf.Deg2Rad * _angle));
        float t2 = ((target.y - pos.y) * (1 - (0.1f * (target.y - pos.y))));
        float proportion = (_angle > 90 && _angle < 270) ? -(((target.y - pos.y) * (1 - (0.1f * (target.y - pos.y)))) / Mathf.Tan(Mathf.Deg2Rad * _angle)) : (((target.y - pos.y) * (1 - (0.1f * (target.y - pos.y)))) / Mathf.Tan(Mathf.Deg2Rad * _angle));
        float dist = Vector3.Distance(pos, target) + proportion;
        // calculate initival velocity required to land the cube on target using the formula (9)
        float a = dist * -Physics.gravity.y / (Mathf.Sin(Mathf.Deg2Rad * _angle * 2));
        float Vy, Vx;   // y,z components of the initial velocity
        float Vi = a > 0 ? Mathf.Sqrt(a) : Mathf.Sqrt(-a);
        Vy = Vi * Mathf.Sin(Mathf.Deg2Rad * _angle);
        Vx = Vi * Mathf.Cos(Mathf.Deg2Rad * _angle);
        // create the velocity vector in local space
        Vector3 localVelocity = new Vector3(Vx, Vy, 0);
        return localVelocity;
        //Bắn góc 120
    }
    #region=======THỰC HIỆN CÁC PHƯƠNG THỨC TRẢ VỀ THẰNG PLAYER CÓ GÓC BẮN TỚI NÓ LÀ NHỎ NHẤT=======
    /// <summary>
    /// Xác định mục tiêu cần bắn
    /// </summary>
    private int IdentifyTarget()
    {
        GameObject[] arrPlayer = GameObject.FindGameObjectsWithTag("Player");
       //Xác định con không có vật cản
        List<GameObject> list = new List<GameObject>();//Danh sách những con không có vật cản
        for (int i = 0; i < arrPlayer.Length; i++)
        {
            if (!CheckCollectMap(arrPlayer[i].transform.position))
            {
                list.Add(arrPlayer[i]);
            }
        }
        //1. Không có vật cản và nhỏ hơn khoảng cách nào đó phù hợp vũ khí bắn gần|| hoặc gần có vật cản
        if (list.Count!=0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (Vector2.Distance(transform.position, list[i].transform.position) < _MaxDestince)
                {
                    _Target = list[i];
                    Debug.Log("Mục tiêu trường hợp 1");
                    return 1;
                }
                else
                {
                    Player player = list[i].GetComponent<Player>();
                    if (Vector2.Distance(transform.position, list[i].transform.position) < _MaxDestince + 2 && player._Health < 60 && _GameController._ListStatePickEnemy[16].State)
                    {
                        if (_GameController._Level==3)
                        {
                            if (CheckPick_Flame(list[i].transform.position))
                            {
                                 _Target = list[i];
                                Debug.Log("Mục tiêu trường hợp 5");
                                return 5;
                            }
                        }
                        else
                        {
                            _Target = list[i];
                            Debug.Log("Mục tiêu trường hợp 5");
                            return 5;
                        }
                      
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < arrPlayer.Length; i++)
            {
                Player player = arrPlayer[i].GetComponent<Player>();
                if (Vector2.Distance(transform.position, arrPlayer[i].transform.position) < _MaxDestince + 2 && player._Health <60 && _GameController._ListStatePickEnemy[16].State)
                {
                    if (_GameController._Level == 3)
                    {
                        if (CheckPick_Flame(arrPlayer[i].transform.position))
                        {
                            _Target = arrPlayer[i];
                            Debug.Log("Mục tiêu trường hợp 5");
                            return 5;
                        }
                    }
                    else
                    {
                        _Target = arrPlayer[i];
                        Debug.Log("Mục tiêu trường hợp 5");
                        return 5;
                    }
                }
                if (Vector2.Distance(transform.position,arrPlayer[i].transform.position)<_MaxDestince)
                {
                    //Dùng cưa lốc
                    if (_GameController._ListStatePickEnemy[4].State)
                    {
                        _Target = arrPlayer[i];
                        Debug.Log("Mục tiêu trường hợp 2");
                        return 2;
                    }
                }
            }
        }
        //2. Mục tiêu không có vật cản ở giữa
        if (list.Count != 0) { _Target = list[0]; Debug.Log("Mục tiêu trường hợp 3"); return 3; }

        //3.Random 1 trong những mục tiêu bất kỳ
        int ran = Random.Range(0,arrPlayer.Length);
        _Target = arrPlayer[ran];
        Debug.Log("Mục tiêu trường hợp 4");
        return 4;
    }
    /// <summary>
    /// Chọn loại vũ khí dể sử dụng
    /// </summary>
    public int ChoosePick()
    {
        _UIManager.LoadDataPopupPick();
        //===========
        Player player = gameObject.GetComponent<Player>();
        if (player._Health<30 &&  _GameController._ListStatePickEnemy[14].State)
        {
            Debug.Log("Chọn vũ khí là tăng máu");
            return 14;
        }
        //=============
        int caseEnemy = IdentifyTarget();
        if (caseEnemy==1)//Gan mà không có vật cản
        {
            if (_GameController._ListStatePickEnemy[12].State)
            {
                Debug.Log("Chọn súng máy");
                return 12;
            }
            else if (Vector2.Distance(transform.position,_Target.transform.position)<1)
            {
                if (_GameController._ListStatePickEnemy[10].State)
                {
                    Debug.Log("Chon đấm");
                    return 10;
                }
                else
                {
                    Debug.Log("Chọn súng Slo");
                    return 1;
                }
            }
            else
            {
                Debug.Log("Chọn súng Slo");
                return 1;
            }
        }
        if (caseEnemy==2 && _GameController._ListStatePickEnemy[4].State)//Gần mà có vật cản
        {
            Debug.Log("Chọn cưa lốc");
            return 4;
        }
        if (caseEnemy==3)//Không có vật cản
        {
            GameObject[] arrPlayer = GameObject.FindGameObjectsWithTag("Player");
            if (_GameController._ListStatePickEnemy[3].State)//Rocket
            {
                for (int i = 0; i < arrPlayer.Length; i++)
                {
                    if (!CheckCollectMapAbove(arrPlayer[i].transform.position))
                    {
                        if (CheckPick_Bomp(arrPlayer[i].transform.position))
                        {
                            _Target = arrPlayer[i];
                            Debug.Log("Chọn Rocket");
                            return 3;
                        }
                    }
                }
            }
            //========Chọn random ngẫu nhiên các súng bắn theo dường vòng cung
            List<int> listPick = new List<int>();
            if (_GameController._ListStatePickEnemy[13].State) { listPick.Add(13); }
            if (_GameController._ListStatePickEnemy[11].State) { listPick.Add(11); }
            if (_GameController._ListStatePickEnemy[2].State) { listPick.Add(2); }
            if (_GameController._ListStatePickEnemy[8].State) { listPick.Add(8); }
          //  if (_GameController._ListStatePickEnemy[0].State) { listPick.Add(0); }
            if (_GameController._ListStatePickEnemy[6].State && _GameController._Level != 3) { listPick.Add(6); }
        //    if (_GameController._ListStatePickEnemy[9].State) { listPick.Add(9); }
            int ran = Random.Range(0, listPick.Count);
            Debug.Log("Chọn súng bắn vòng cung: " + listPick[ran]);
            return listPick[ran];
        }
        if (caseEnemy==4)
        {
            GameObject[] arrPlayer = GameObject.FindGameObjectsWithTag("Player");
            //===================
            if (_GameController._ListStatePickEnemy[7].State)
            {
                for (int i = 0; i < arrPlayer.Length; i++)
                {
                    Player playerP = arrPlayer[i].GetComponent<Player>();
                    if (playerP._Health < 30) {
                        _Target = arrPlayer[i];
                        Debug.Log("Chọn súng tilare"); return 7; }
                }
            }
            //========================
            if (_GameController._ListStatePickEnemy[3].State)//Rocket
            {
                for (int i = 0; i < arrPlayer.Length; i++)
                {
                    if (!CheckCollectMapAbove(arrPlayer[i].transform.position))
                    {
                        if (CheckPick_Bomp(arrPlayer[i].transform.position))
                        {
                            _Target = arrPlayer[i];
                            Debug.Log("Chọn Rocket");
                            return 3;
                        }
                    }
                }
            }
           
            //========Chọn random ngẫu nhiên các súng bắn theo dường vòng cung
            List<int> listPick = new List<int>();
            if (_GameController._ListStatePickEnemy[13].State) { listPick.Add(13);}
            if (_GameController._ListStatePickEnemy[11].State) { listPick.Add(11);}
            if (_GameController._ListStatePickEnemy[2].State) { listPick.Add(2); }
            if (_GameController._ListStatePickEnemy[8].State) { listPick.Add(8); }
           // if (_GameController._ListStatePickEnemy[0].State) { listPick.Add(0); }
            if (_GameController._ListStatePickEnemy[6].State && _GameController._Level!=3) { listPick.Add(6); }
           // if (_GameController._ListStatePickEnemy[9].State) { listPick.Add(9); }
            int ran = Random.Range(0,listPick.Count);
            Debug.Log("Chọn súng bắn vòng cung: " + listPick[ran]);
            return listPick[ran];
        }
        if (caseEnemy==5)
        {
            Debug.Log("Chọn súng phun lửa");
            return 16;
        }
        return -1;
    }
    /// <summary>
    /// Check xem có vật cản giữa mình và mục tiêu hay không
    /// </summary>
    private bool CheckCollectMap(Vector3 target)
    {
        Vector3 velocity = target - transform.position;
        float directionX = Mathf.Sign((target - transform.position).x);
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = transform.position;
            // rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            float rayLengthHor = Vector2.Distance(target, transform.position);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, velocity, rayLengthHor, collisionMask);

            Debug.DrawRay(rayOrigin, velocity * rayLengthHor, Color.red);
            if (hit)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Kiểm tra bên trên đầu của mục tiêu có vật cản hay ko
    /// </summary>
    private bool CheckCollectMapAbove(Vector3 target)
    {
        float directionX = Mathf.Sign((target - transform.position).x);
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = target;
            // rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            float rayLengthHor = 10;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,new Vector3(0,1,0), rayLengthHor, collisionMask);

            Debug.DrawRay(rayOrigin, new Vector3(0, 1, 0) * rayLengthHor, Color.red);
            if (hit)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Thực hiện bắn Player
    /// </summary>
    public void HitPlayer()
    {
        int pick = ChoosePick();
        while (pick == -1) pick = ChoosePick();
        UIManager ui = FindObjectOfType<UIManager>();
        ui.ChoosePick(pick);
        if (pick == 14) return;
        //============
        Vector3 dir = _Target.transform.position - transform.position;
        float angleWithTarget = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;//Góc tạo giữa vel và trục Ox
        angleWithTarget = ((angleWithTarget + 360) % 360);
        float angle=-1;
        if (_GameController._Level == 3)
        {
            if (angleWithTarget >= 85 && angleWithTarget <= 95)
            {
                angle = angleWithTarget;
            }
            else if (angleWithTarget>95 && angleWithTarget<270)
            {
                angle = Random.Range(95,115);
            }
            else if (angleWithTarget>270 || angleWithTarget<85)
            {
                angle = Random.Range(65,85);
            }
        }
        else
        {
            if (angleWithTarget > 135 && angleWithTarget < 225)
            {
                angle = Random.Range(95, angleWithTarget - 50);
            }
            else if (angleWithTarget >= 225 && angleWithTarget < 270)
            {
                angle = Random.Range(95, angleWithTarget - 100);
            }
            else if (angleWithTarget > 95 && angleWithTarget <= 135)
            {
                angle = Random.Range(95, (angleWithTarget - 10 < 95) ? 95 : angleWithTarget - 10);
            }
            else if ((angleWithTarget > 0 && angleWithTarget < 45) || angleWithTarget > 315)
            {
                angle = Random.Range((angleWithTarget + 50) % 360, 85);
            }
            else if (angleWithTarget > 270 && angleWithTarget <= 315)
            {
                angle = Random.Range((angleWithTarget + 100) % 360, 85);
            }
            else if (angleWithTarget >= 45 && angleWithTarget < 85)
            {
                angle = Random.Range((angleWithTarget + 10) > 85 ? 85 : angleWithTarget + 10, 85);
            }
            else if (angleWithTarget >= 85 && angleWithTarget <= 95)
            {
                angle = angleWithTarget;
            }
            else //Trường hợp mục tiêu ở bên dưới thì không bắn được
            {
                return;
            }
        }
        Vector3 vel = Launch(angle,_Target.transform.position);
        Debug.Log("Lực tính được là"+vel+" ViTri: "+_Target.transform.position);
        StartCoroutine(WaitForHitPlayer(pick, vel));
        
    }
    IEnumerator WaitForHitPlayer(int indexPick,Vector3 vel)
    {
        yield return new WaitForSeconds(1);
        if(_GameController._Level==1)
        {
            int ran = Random.Range(1,4);
            if (ran==1)
            {
                int ranDir = 1;
                vel*=(ranDir*1.2f);
            }
        }
        switch(indexPick)
        {
            case 0://Lưu đạn 
                Pick_GrenadeController pick_grenade = gameObject.GetComponentInChildren<Pick_GrenadeController>();
                pick_grenade.throwBallAIEnemy(vel);
                break;
            case 1://Slo
                ShotGunController pick_shotGun = gameObject.GetComponentInChildren<ShotGunController>();
                if (_Target==null) { IdentifyTarget(); }
                pick_shotGun.throwBallAIEnemy(_Target.transform.position);
                StartCoroutine(WaitForTurn2ShotGun());
                break;
            case 2://Cối
                CannonScript pick_Bazoo = gameObject.GetComponentInChildren<CannonScript>();
                pick_Bazoo.ThrowBallEnemy(vel);
                break;
            case 3: //Rocket
                AirCallController pick_Bomp = gameObject.GetComponentInChildren<AirCallController>();
                pick_Bomp.Pick_BombEnemy(_Target.transform.position);
                break;
            case 4:
                break;
            case 5: break;
            case 6: break;//Tự dùng
            case 7:
                break;
            case 8:
                break;
            case 9: break;
            case 10:
                break;
            case 11:
                break;
            case 12:
                break;
            case 13:
                break;
            case 14: break;
            case 15: break;
            case 16:
                break;
            case 17: break;
        }
    }
    IEnumerator WaitForTurn2ShotGun()
    {
        yield return new WaitForSeconds(1);
        ShotGunController pick_shotGun = gameObject.GetComponentInChildren<ShotGunController>();
        if (_Target == null) { IdentifyTarget(); }
        pick_shotGun.throwBallAIEnemy(_Target.transform.position);
    }
    public void Test()
    {
        Debug.Log(Mathf.Tan(Mathf.Deg2Rad * _AngleTest) + " Angle: " + Mathf.Deg2Rad * _AngleTest);
        Debug.Log("Sin: " + (Mathf.Sin(Mathf.Deg2Rad * _AngleTest * 2)));
        float a = 5 * -Physics.gravity.y / (Mathf.Sin(Mathf.Deg2Rad * _AngleTest * 2));
        float Vi = a > 0 ? Mathf.Sqrt(a) : Mathf.Sqrt(-a);
        float Vy, Vx;   // y,z components of the initial velocity
        Vy = Vi * Mathf.Sin(Mathf.Deg2Rad * _AngleTest);
        Vx = Vi * Mathf.Cos(Mathf.Deg2Rad * _AngleTest);
        Debug.Log("Test: " + 5 * -Physics.gravity.y / (Mathf.Sin(Mathf.Deg2Rad * _AngleTest * 2)));
        Debug.Log("Vi: "+Vi+" Vx: "+Vx+" Vy: "+Vy);
    }
    
    /// <summary>
    /// Check xem có nên sử dụng súng phun lửa hay không 
    /// Nếu có quân của mình ở xung quanh thì ko sử dụng
    /// </summary>
    private bool CheckPick_Flame(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        float angleWithTarget = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;//Góc tạo giữa vel và trục Ox
        angleWithTarget = ((angleWithTarget + 360) % 360);
        GameObject[] arrEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < arrEnemy.Length; i++)
        {
            Player player=arrEnemy[i].GetComponent<Player>();
            if (!player._IsCurrent && Vector2.Distance(gameObject.transform.position,arrEnemy[i].transform.position)<_MinDestince+2)
            {
                Vector3 dirE = arrEnemy[i].transform.position - transform.position;
                float angleWithTargetE = Mathf.Atan2(dirE.y, dirE.x) * Mathf.Rad2Deg;//Góc tạo giữa vel và trục Ox
                angleWithTargetE = ((angleWithTargetE + 360) % 360);
                if (angleWithTarget - angleWithTargetE < 20 || angleWithTarget - angleWithTargetE > -20 || angleWithTarget - angleWithTargetE > 340 || angleWithTarget - angleWithTargetE < -340)
                {
                    return false;
                }
            }
        }
        return true;
    }
    /// <summary>
    /// Kiểm tra khi quân địch sử dụng Bomp
    /// </summary>
    private bool CheckPick_Bomp(Vector3 target)
    {
        GameObject[] arrEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < arrEnemy.Length; i++)
        {
            if (target.x-arrEnemy[i].transform.position.x<1.5f && target.x-arrEnemy[i].transform.position.x>-1.5f)
            {
                return false;
            }
        }
        return true;
    }
    #endregion

}
