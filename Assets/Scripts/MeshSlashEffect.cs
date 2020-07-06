using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSlashEffect : MonoBehaviour {

    [SerializeField] List<Vector3> points = new List<Vector3>();
    [SerializeField] float appendDistance = 0.5f;
    private float appendSqrDistance = 1f;
    [SerializeField] int maxPointCount = 20;//画面に存在できるポイントの最大数
    [SerializeField] bool keepPointLength = true;
    private float laserMinusTime = 0.3f;//終了時、メッシュが細くなって消えるまでの時間

    //斬撃の方向反転時、pointsを削除するので退避用にとっておく
    private List<Vector3> reversPoints = new List<Vector3>();
    private const int reverceJudgeCount = 4;//反転の判定に必要なpointsの要素数
    private const int slashBeginJudgePointsCount = 3;//スラッシュした方向を判定するのに必要なpointsの要素数
    private bool isPrevActionTouchMoving = false;//直前にドラッグしていたかを保持

    private const float initSlashBeginAngle = 999f;
    private float slashBeginAngle = initSlashBeginAngle;//-360～360を想定しているため、初期値は999
    private Vector2 slashBeginVector = Vector2.zero;

    private struct section
    {
        public Vector3 direction;   // 方向ベクトル.

        public Vector3 left;        // セクションの左端.
        public Vector3 right;       // セクションの右側.
    }

    private section[] sections;

    private float laserWidth = 0.3f;
    private float initLaserWidth = 0f;

    [SerializeField] Material laserMat = null;
    [SerializeField] MeshFilter mf;
    Mesh mesh;
    //[SerializeField] MeshCollider mc = null;
    private bool isLaserEndAction = false;

    [SerializeField] private AudioSource slashAudio = null;
    [SerializeField] private AudioClip slashClip = null;
    [SerializeField] private Camera cameraObj = null;

    public delegate void SlashEndCallback();
    private SlashEndCallback slashEndCallback;
    public delegate void SlashBeginCallback();
    private SlashBeginCallback slashBeginCallback;

    private float currentSlashAngle = 0f;
    //public float CurrentSlashAngle { get { return currentSlashAngle; } }

    void Awake()
    {
        appendSqrDistance = Mathf.Pow(appendDistance, 2);
        initLaserWidth = laserWidth;
    }
    // Use this for initialization
    void Start () {
        mesh = mf.mesh = new Mesh();
    }

    public void SetSlashBeginCallback(SlashBeginCallback callback)
    {
        slashBeginCallback = callback;
    }
    public void SetSlashEndCallback(SlashEndCallback callback)
    {
        slashEndCallback = callback;
    }
	
	// Update is called once per frame
	public void UpdateAction () {
        Debug.Log("slash");
        if (InputManager.Instance.IsTouchMove(0))
        {
            isPrevActionTouchMoving = true;
        }
        if (InputManager.Instance.IsTouchMove(0) || InputManager.Instance.IsTouchDown(0))
        {
            setPoints();
            setVectors();
            createMesh();
        }
        else if (InputManager.Instance.IsTouchEnd(0) && points.Count >= 1)
        {
            isLaserEndAction = true;
        }
        //ドラッグした後に止まった時はTouchEndと同じ処理をする
        else if (!InputManager.Instance.IsTouchMove(0))
        {
            if (isPrevActionTouchMoving && sections != null && points.Count >= reverceJudgeCount)
            {
                isLaserEndAction = true;
                isPrevActionTouchMoving = false;
            }
        }
        
    }

    private void Update()
    {
        UpdateAngle();
        //タッチを離した時、メッシュがだんだん細くなって消えるアニメーション
        if (isLaserEndAction)
        {
            float minusWidth = ((Time.deltaTime / laserMinusTime) * initLaserWidth);
            laserWidth -= minusWidth;
            if(sections == null) { return; }
            for (int i = 0; i < sections.Length; i++)
            {
                Vector3 leftLength = sections[i].left - points[i];
                Vector3 rightLength = sections[i].right - points[i];

                leftLength = leftLength.normalized;
                rightLength = rightLength.normalized;
                leftLength = new Vector3(leftLength.x * minusWidth, leftLength.y * minusWidth);
                rightLength = new Vector3(rightLength.x * minusWidth, rightLength.y * minusWidth);
                sections[i].left -= leftLength;
                sections[i].right -= rightLength;
            }
            createMesh();
            if (laserWidth <= 0.07f)
            {
                isLaserEndAction = false;
                laserWidth = initLaserWidth;
                //mesh.vertices = null;
                mesh.uv = null;
                mesh.triangles = null;
                points.Clear();
                InitSlashBeginAngle();
                //slashEndCallback();
            }
        }
    }

    private void UpdateAngle()
    {
        //現在のスラッシュの角度を保持
        if (points.Count >= slashBeginJudgePointsCount)
        {
            float retDX = points[points.Count - 1].x - points[points.Count - (slashBeginJudgePointsCount - 1)].x;
            float retDY = points[points.Count - 1].y - points[points.Count - (slashBeginJudgePointsCount - 1)].y;
            float retAngle = Mathf.Atan2(retDY, retDX) * Mathf.Rad2Deg;
            currentSlashAngle = retAngle;
        }
    }
    public float GetCurrentSlashAngle()
    {
        UpdateAngle();
        return currentSlashAngle;
    }

    public void EndSlashEffect()
    {
        isLaserEndAction = true;
    }

    /// <summary>
    /// スクリプトロード時かインスペクターの値変更時に呼び出される
    /// </summary>
    void OnValidate()
    {
        appendSqrDistance = Mathf.Pow(appendDistance, 2);
    }
    /// <summary>
    /// マウス入力によってpointsを設定する.
    /// </summary>
    void setPoints()
    {
        // マウス押下中のみ処理を行う.
        if (!InputManager.Instance.IsTouchMove(0) && points.Count >= reverceJudgeCount)
        {
            points.Clear();
            InitSlashBeginAngle();
            isLaserEndAction = true;
            return;
        }

        //斬撃の方向反転時、ポイントとメッシュを強制削除し、反転開始時から現在の位置までのポイントを追加してメッシュ生成
        if (IsSlashRevercing())
        {
            reversPoints.Clear();
            for(int i = points.Count - (reverceJudgeCount - 1); i < points.Count; i++)
            {
                reversPoints.Add(points[i]);
            }
            isLaserEndAction = false;
            laserWidth = initLaserWidth;
            mesh.uv = null;
            mesh.triangles = null;
            points.Clear();
            InitSlashBeginAngle();
            //反転した位置から現在の位置までを追加
            points.AddRange(reversPoints);
            isLaserEndAction = false;
            //コールバック
            //slashEndCallback();
            return;
        }

        // マウスの位置をスクリーン座標からワールド座標に変換.
        //var screenMousePos = Input.mousePosition;
        var screenMousePos = InputManager.Instance.GetTouchPosition(0);
        //screenMousePos.z = -Camera.main.transform.position.z;
        screenMousePos.z = 3f;
        var curPoint = cameraObj.ScreenToWorldPoint(screenMousePos);

        if (points == null)
        {
            points = new List<Vector3>();
            points.Add(curPoint);
        }
        Vector3 curPointVec2 = new Vector3(curPoint.x, curPoint.y, curPoint.z);
        // 前回のポイントとの比較を行う.
        if (points.Count >= 2)
        {
            var distance = (curPointVec2 - points[points.Count - 1]);
            if (distance.sqrMagnitude >= appendSqrDistance)
            {
                points.Add(curPoint);
            }
        }
        // ポイントの追加.
        addPoint(curPoint);
        // 最大数を超えた場合ポイントを削除.
        while (points.Count > maxPointCount)
        {
            points.RemoveAt(0);
        }

        //斬撃開始時、効果音を鳴らす
        if (IsSlashBegin())
        {
            slashEndCallback();
            slashAudio.PlayOneShot(slashClip);
            if (slashBeginCallback != null)
            {
                slashBeginCallback();
            }
        }
        SetSlashBeginAngle();
    }
    void addPoint(Vector3 curPoint)
    {
        //生成したメッシュの長さを一定にする
        if (keepPointLength)
        {
            while (true)
            {

                if (points.Count >= 2)
                {
                    var distance = (curPoint - points[points.Count - 1]);
                    if (distance.sqrMagnitude < appendSqrDistance) break;
                    distance *= appendDistance / distance.magnitude;
                    points.Add(points[points.Count - 1] + distance);
                }
                else
                {
                    points.Add(curPoint);
                    break;
                }
            }
        }
        //一定にしない
        else
        {
            var distance = (curPoint - points[points.Count - 1]);
            if (distance.sqrMagnitude >= appendSqrDistance)
            {
                points.Add(curPoint);
            }
        }
    }
    /// <summary>
    /// メッシュを描く方向を設定
    /// </summary>
    void setVectors()
    {
        // 2つ以上セクションを用意できない状態の場合処理を抜ける.
        if (points == null || points.Count <= 1) return;

        sections = new section[points.Count];

        for (int i = 0; i < points.Count; i++)
        {
            // ----- 方向ベクトルの計算 -----
            if (i == 0)
            {
                // 始点の場合.
                sections[i].direction = points[i + 1] - points[i];
            }
            else if (i == points.Count - 1)
            {
                // 終点の場合.
                sections[i].direction = points[i] - points[i - 1];
            }
            else
            {
                // 途中の場合.
                sections[i].direction = points[i + 1] - points[i - 1];
            }

            sections[i].direction.Normalize();

            // ----- 方向ベクトルに直交するベクトルの計算 -----
            Vector3 side = Quaternion.AngleAxis(90f, -Vector3.forward) * sections[i].direction;
            side.Normalize();

            sections[i].left = points[i] - side * laserWidth / 2f;
            sections[i].right = points[i] + side * laserWidth / 2f;
        }
    }

    /// <summary>
    /// メッシュを作成
    /// </summary>
    void createMesh()
    {
        if (points == null || points.Count <= 1 || sections == null) return;

        //MeshFilter mf = GetComponent<MeshFilter>();
        //Mesh mesh = mf.mesh = new Mesh();
        //mc = GetComponent<MeshCollider>();

        mesh.name = "CurveLaserMesh";

        int meshCount = points.Count - 1;                   // 四角メッシュ生成数はセクション - 1.

        Vector3[] vertices = new Vector3[(meshCount) * 4];  // 四角なので頂点数は1つのメッシュに付き4つ.
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(meshCount) * 2 * 3];     // 1つの四角メッシュには2つ三角メッシュが必要. 三角メッシュには3つの頂点インデックスが必要.

        // ----- 頂点座標の割り当て -----
        for (int i = 0; i < meshCount; i++)
        {
            vertices[i * 4 + 0] = sections[i].left;
            vertices[i * 4 + 1] = sections[i].right;
            vertices[i * 4 + 2] = sections[i + 1].left;
            vertices[i * 4 + 3] = sections[i + 1].right;

            var step = (float)1 / meshCount;

            uvs[i * 4 + 0] = new Vector2(0f, i * step);
            uvs[i * 4 + 1] = new Vector2(1f, i * step);
            uvs[i * 4 + 2] = new Vector2(0f, (i + 1) * step);
            uvs[i * 4 + 3] = new Vector2(1f, (i + 1) * step);
        }

        // ----- 頂点インデックスの割り当て -----
        int positionIndex = 0;

        for (int i = 0; i < meshCount; i++)
        {
            triangles[positionIndex++] = (i * 4) + 1;
            triangles[positionIndex++] = (i * 4) + 0;
            triangles[positionIndex++] = (i * 4) + 2;

            triangles[positionIndex++] = (i * 4) + 2;
            triangles[positionIndex++] = (i * 4) + 3;
            triangles[positionIndex++] = (i * 4) + 1;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        //mc.sharedMesh = mesh;
        //mr.material = laserMat;
    }

    /// <summary>
    /// スラッシュし始めた方向をセット
    /// </summary>
    private void SetSlashBeginAngle()
    {
        if(IsSlashBegin())
        {
            //0と1の要素番号で行きたかったが、0と1が同じ値になってしまうので2にしている
            float dx = points[2].x - points[0].x;
            float dy = points[2].y - points[0].y;
            slashBeginAngle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
            currentSlashAngle = slashBeginAngle;
            //Debug.Log("slashBeginAngle :: " + slashBeginAngle);

            //内積での判定用
            float retDX = points[2].x - points[0].x;
            float retDY = points[2].y - points[0].y;
            slashBeginVector = new Vector2(retDX, retDY);
        }
    }
    /// <summary>
    /// スラッシュし始めの方向を初期化
    /// </summary>
    private void InitSlashBeginAngle()
    {
        slashBeginAngle = initSlashBeginAngle;
        slashBeginVector = Vector2.zero;
        currentSlashAngle = 0f;
    }
    /// <summary>
    /// 斬撃開始時（なぞってから方向を決定するタイミング）であるかを返す
    /// </summary>
    /// <returns></returns>
    private bool IsSlashBegin()
    {
        return points.Count >= slashBeginJudgePointsCount && slashBeginAngle == initSlashBeginAngle;
    }
    /// <summary>
    /// スラッシュし始めたアングルから折り返しているかの判定を返す(現在は90度で折り返しとみなす)
    /// </summary>
    /// <returns></returns>
    private bool IsSlashRevercing()
    {
        //if (points.Count < reverceJudgeCount) return false;
        //float retDX = points[points.Count - 1].x - points[points.Count - (reverceJudgeCount - 1)].x;
        //float retDY = points[points.Count - 1].y - points[points.Count - (reverceJudgeCount - 1)].y;
        //float retAngle = Mathf.Atan2(retDY, retDX) * Mathf.Rad2Deg;

        //float angleDir = retAngle - slashBeginAngle;

        //float absRetAngle = Mathf.Abs(retAngle);
        //float absBeginAngle = Mathf.Abs(slashBeginAngle);
        //if(absRetAngle > absBeginAngle)
        //{
        //    if(absRetAngle - absBeginAngle >= 90)
        //    {
        //        angleDir = 100;
        //    }
        //    else
        //    {
        //        angleDir = 0f;
        //    }
        //}else if (absBeginAngle > absRetAngle)
        //{
        //    if (absBeginAngle - absRetAngle >= 90)
        //    {
        //        angleDir = 100;
        //    }
        //    else
        //    {
        //        angleDir = 0f;
        //    }
        //}
        
        //return Mathf.Abs(angleDir) >= 90;

        //ベクトルの内積での判定
        if (points.Count < reverceJudgeCount) return false;
        float retDX = points[points.Count - 1].x - points[points.Count - (reverceJudgeCount - 1)].x;
        float retDY = points[points.Count - 1].y - points[points.Count - (reverceJudgeCount - 1)].y;
        Vector2 retDir = new Vector2(retDX, retDY);

        //開始時の単位ベクトル、現在の単位ベクトルの内積を求める
        float dot = Vector2.Dot(slashBeginVector.normalized, retDir.normalized);
        //内積が0に近ければ直行しているので角度90度折り返しとみなす
        return dot < 0.1f;
    }
}
