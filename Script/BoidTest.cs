using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidTest : MonoBehaviour
{
    [SerializeField]
    private int N;

    Vector3[] PrePosV;
    Vector3[] PreDirV;

    Vector3[] NextPosV;
    Vector3[] NextDirV;

    Vector3[] RepDir;
    Vector3[] OriDir;
    Vector3[] AttDir;

    Vector3[] DesiredDir;

    [SerializeField]
    private int[] BoidID;

    [SerializeField]
    private int[] flag;

    [SerializeField]
    private GameObject[] Boid;

    int[] State = new int[10000];

    [SerializeField]
    private float repulsion;
    [SerializeField]
    private float orientation;
    [SerializeField]
    private float attraction;

    [SerializeField]
    private float perception;

    [SerializeField]
    private float theta;

    [SerializeField]
    public float tankSize;

    public GameObject BoidObject;

    [SerializeField]
    private float speed;

    [SerializeField]
    private bool toggle = false;

    [SerializeField]
    private float Pgroup = 0.0f;

    [SerializeField]
    private float Mgroup = 0.0f;

    [SerializeField]
    private Vector3 Cgroup = Vector3.zero;

    [SerializeField]
    private Vector3 Dgroup = Vector3.zero;

    [SerializeField]
    private float Gauss;

    [SerializeField]
    private float Tau;

    [SerializeField]
    private float[] AverageP;

    [SerializeField]
    private float[] AverageM;

    void GenarateBoids()
    {
        // 配列要素数設定
        System.Array.Resize(ref PrePosV, N);
        System.Array.Resize(ref PreDirV, N);
        System.Array.Resize(ref NextPosV, N);
        System.Array.Resize(ref NextDirV, N);

        System.Array.Resize(ref RepDir, N);
        System.Array.Resize(ref OriDir, N);
        System.Array.Resize(ref AttDir, N);

        System.Array.Resize(ref DesiredDir, N);

        System.Array.Resize(ref Boid, N);

        System.Array.Resize(ref BoidID, N);

        System.Array.Resize(ref flag, N);
        // 初期位置配置、初期位置保管
        for (int i = 0; i < BoidID.Length; i++)
        {
            float x = Random.Range(-tankSize, tankSize);
            float y = Random.Range(-tankSize, tankSize);
            float z = Random.Range(-tankSize, tankSize);

            var pos = new Vector3(x, y, z);

            var dir = Random.insideUnitSphere.normalized;

            Boid[i] = Instantiate(BoidObject, pos, Quaternion.identity);

            Boid[i].transform.LookAt(pos + dir);

            BoidID[i] = i;

            PrePosV[i] = pos;
            PreDirV[i] = dir;

            NextPosV[i] = pos;
            NextDirV[i] = Vector3.zero;

            //Debug.Log("PreDirV1 :" + PreDirV[i]);
            //Debug.Log("NextDirV1 :" + NextDirV[i]);

            RepDir[i] = Vector3.zero;
            OriDir[i] = Vector3.zero;
            AttDir[i] = Vector3.zero;

            DesiredDir[i] = Vector3.zero;

            State[i] = 99;

            flag[i] = 0;
        }
    }

    void CheckDistance()
    {
        for (int i = 0; i < BoidID.Length; i++)
        {
            int RepNum = 0;
            int OriNum = 0;
            int AttNum = 0;

            var Rep = Vector3.zero;
            var Ori = Vector3.zero;
            var Att = Vector3.zero;

            flag[i] = 0;
            for (int j = 0; j < BoidID.Length; j++)
            {
                if (i != j)
                {
                    float Per = Vector3.Angle(PreDirV[i], PrePosV[j] - PrePosV[i]);


                    if (perception / 2.0f >= Per)
                    {
                        //Debug.Log("Perception angle:" + Per);

                        var r = PrePosV[j] - PrePosV[i];

                        var dist = 0.0f;

                        if (!toggle)
                        {
                            if (r.x > tankSize)
                            {
                                r.x -= tankSize * 2;
                            }
                            else if (r.x < -tankSize)
                            {
                                r.x += tankSize * 2;
                            }


                            if (r.y > tankSize)
                            {
                                r.y -= tankSize * 2;
                            }
                            else if (r.y < -tankSize)
                            {
                                r.y += tankSize * 2;
                            }


                            if (r.z > tankSize)
                            {
                                r.z -= tankSize * 2;
                            }
                            else if (r.z < -tankSize)
                            {
                                r.z += tankSize * 2;
                            }

                            dist = r.magnitude;
                        }
                        else if (toggle)
                        {
                            dist = Vector3.Distance(PrePosV[i], PrePosV[j]);
                        }

                        var UnitV = r / r.magnitude;

                        if (dist < repulsion)
                        {
                            Rep -= UnitV;

                            RepNum++;

                        }
                        if (dist < orientation)
                        {
                            Ori += PreDirV[j] / PreDirV[j].magnitude;

                            OriNum++;

                        }
                        if (dist < attraction)
                        {
                            Att += UnitV;

                            AttNum++;

                        }
                    }
                }
            }

            if (RepNum > 0)
            {
                Rep = Rep.normalized;

                NextDirV[i] = Rep;

                flag[i] = 4;
            }
            else
            {

                Ori += PreDirV[i] / PreDirV[i].magnitude;

                if (OriNum > 0)
                {
                    Ori = Ori.normalized;
                }
                if (AttNum > 0)
                {
                    Att = Att.normalized;
                }

                if (OriNum > 0 && AttNum > OriNum)
                {
                    NextDirV[i] = (Ori + Att) / 2.0f;

                    flag[i] = 3;
                }
                else if (OriNum > 0 && AttNum == OriNum)
                {
                    NextDirV[i] = Ori;
                    //Debug.Log("Ori :" + Ori);

                    flag[i] = 2;
                }
                else if (AttNum > 0 && OriNum == 0)
                {
                    NextDirV[i] = Att;
                    //Debug.Log("Att :" + Att);

                    flag[i] = 1;
                }
                else
                {
                    NextDirV[i] = PreDirV[i];

                    flag[i] = 0;
                }
            }

            if (NextDirV[i] == Vector3.zero)
            {
                NextDirV[i] = PreDirV[i];
            }
            //Debug.Log("PreDirV :" + PreDirV[i]);
            //Debug.Log("NextDirV :" + NextDirV[i]);
            //Debug.Log("RepNum :" + RepNum + "OriNum :" + OriNum + "AttNum :" + AttNum);

            float angle = Vector3.Angle(PreDirV[i], NextDirV[i]);
            //Debug.Log("angle :" + angle);

            if (angle > theta * Tau)
            {
                float deg = theta * Tau / angle;
                //Debug.Log("deg :" + deg);
                if (deg >= 1)
                {
                    Debug.LogError("deg over 1");
                    Debug.Break();
                }
                NextDirV[i] = Vector3.Slerp(PreDirV[i], NextDirV[i], deg);
            }

            var theta2 = rand_normal(0, Gauss);

            var phi = rand_normal(0, Gauss);

            //Debug.Log("error angle :" + Vector3.Angle(NextDirV[i], Quaternion.Euler((float)theta2 * Mathf.Rad2Deg, 0, (float)phi * Mathf.Rad2Deg) * NextDirV[i]));

            //Debug.Log("before error :" + NextDirV[i].ToString("F4"));

            if(NextDirV[i] == Vector3.zero)
            {
                Debug.Break();
            }
            NextDirV[i] = Quaternion.Euler(theta2, 0.0f, phi) * NextDirV[i];

            //if (theta2 == 0 && phi == 0)
            //{
            //    NextDirV[i] = NextDirV[i];
            //}
            //else
            //{
                
            //}

            //Debug.Log("After error :" + NextDirV[i].ToString("F4"));

            NextDirV[i] = NextDirV[i].normalized;
        }
    }

    // 正規分布
    float rand_normal(float mu, float sigma)
    {
        float z = Mathf.Sqrt(-2.0f * Mathf.Log(Random.Range(0.0f, 1.0f))) * Mathf.Sin(2.0f * Mathf.PI * Random.Range(0.0f, 1.0f));
        return mu + sigma * z;
    }

    void Repulsion()
    {
        for (int i = 0; i < BoidID.Length; i++)
        {
            RepDir[i] = Vector3.zero;

            for (int j = 0; j < BoidID.Length; j++)
            {
                if (i != j)
                {
                    if (State[i] == 0)
                    {
                        var mag = PrePosV[j] - PrePosV[i];
                        var r = (PrePosV[j] - PrePosV[i]) / mag.magnitude;

                        RepDir[i] -= r / r.magnitude;

                    }
                }
            }
        }

    }

    void Orientation()
    {
        for (int i = 0; i < BoidID.Length; i++)
        {
            OriDir[i] = Vector3.zero;

            for (int j = 0; j < BoidID.Length; j++)
            {
                if (State[i] == 1 || State[i] == 3)
                {
                    var v = PreDirV[i] / PreDirV[i].magnitude;

                    OriDir[i] += v;

                }
            }
        }
    }

    void Attraction()
    {
        for (int i = 0; i < BoidID.Length; i++)
        {
            AttDir[i] = Vector3.zero;

            for (int j = 0; j < BoidID.Length; j++)
            {
                if (i != j)
                {
                    if (State[i] == 2 || State[i] == 3)
                    {
                        var mag = PrePosV[j] - PrePosV[i];
                        var r = (PrePosV[j] - PrePosV[i]) / mag.magnitude;

                        AttDir[i] += r / r.magnitude;

                    }
                }
            }
        }
    }

    void UpdatePosition()
    {
        for (int i = 0; i < BoidID.Length; i++) {

            //Debug.Log("NextDirV1 Normalized :" + NextDirV[i].normalized.ToString("F4"));

            //Debug.Log("PrePosV1 :" + PrePosV[i].ToString("F4"));

            NextPosV[i] += speed * Tau * NextDirV[i];

            //Debug.Log("PrePosV2 :" + PrePosV[i].ToString("F4"));
        }

    }

    void Draw()
    {
        for (int i = 0; i < BoidID.Length; i++)
        {
            Boid[i].transform.position = NextPosV[i];

            Boid[i].transform.LookAt(NextPosV[i] + NextDirV[i].normalized);

            switch (flag[i]) {
                case 0:
                    Debug.DrawRay(NextPosV[i], NextDirV[i].normalized * 10, Color.blue);
                    break;
                case 1:
                    Debug.DrawRay(NextPosV[i], NextDirV[i].normalized * 10, Color.green);
                    break;
                case 2:
                    Debug.DrawRay(NextPosV[i], NextDirV[i].normalized * 10, Color.yellow);
                    break;
                case 3:
                    Debug.DrawRay(NextPosV[i], NextDirV[i].normalized * 10, Color.cyan);
                    break;
                case 4:
                    Debug.DrawRay(NextPosV[i], NextDirV[i].normalized * 10, Color.red);
                    break;
            }
        }
    }

    void NextFrame()
    {
        for (int i = 0; i < BoidID.Length; i++)
        {
            PrePosV[i] = NextPosV[i];
            PreDirV[i] = NextDirV[i];
        }
    }

    void Boundary()
    {
        for (int i = 0; i < N; i++)
        {
            if (NextPosV[i].x > tankSize)
            {
                var pos = Vector3.zero;
                pos = NextPosV[i];

                pos.x -= tankSize * 2;

                NextPosV[i] = pos;
            } else if (NextPosV[i].x < -tankSize)
            {
                var pos = Vector3.zero;
                pos = NextPosV[i];

                pos.x += tankSize * 2;

                NextPosV[i] = pos;
            }

            if (NextPosV[i].y > tankSize)
            {
                var pos = Vector3.zero;
                pos = NextPosV[i];

                pos.y -= tankSize * 2;

                NextPosV[i] = pos;
            } else if (NextPosV[i].y < -tankSize)
            {
                var pos = Vector3.zero;
                pos = NextPosV[i];

                pos.y += tankSize * 2;

                NextPosV[i] = pos;
            }

            if (NextPosV[i].z > tankSize)
            {
                var pos = Vector3.zero;
                pos = NextPosV[i];

                pos.z -= tankSize * 2;

                NextPosV[i] = pos;
            } else if (NextPosV[i].z < -tankSize)
            {
                var pos = Vector3.zero;
                pos = NextPosV[i];

                pos.z += tankSize * 2;

                NextPosV[i] = pos;
            }
        }
    }
    void Wall()
    {
        for (int i = 0; i < N; i++)
        {
            /*
            if (NextPosV[i].x > tankSize)
            {
                var pos = Vector3.zero;
                pos = NextPosV[i];

                NextDirV[i] = NextDirV[i] - 2 * Vector3.Dot(NextDirV[i], Vector3.left) * Vector3.left;

                pos.x = tankSize;

                NextPosV[i] = pos;
            }
            else if (NextPosV[i].x < -tankSize)
            {
                var pos = Vector3.zero;
                pos = NextPosV[i];

                NextDirV[i] = NextDirV[i] - 2 * Vector3.Dot(NextDirV[i], Vector3.right) * Vector3.right;

                pos.x = -tankSize;

                NextPosV[i] = pos;
            }

            if (NextPosV[i].y > tankSize)
            {
                var pos = Vector3.zero;
                pos = NextPosV[i];

                NextDirV[i] = NextDirV[i] - 2 * Vector3.Dot(NextDirV[i], Vector3.down) * Vector3.down;

                pos.y = tankSize;

                NextPosV[i] = pos;
            }
            else if (NextPosV[i].y < -tankSize)
            {
                var pos = Vector3.zero;
                pos = NextPosV[i];

                NextDirV[i] = NextDirV[i] - 2 * Vector3.Dot(NextDirV[i], Vector3.up) * Vector3.up;

                pos.y = -tankSize;

                NextPosV[i] = pos;
            }

            if (NextPosV[i].z > tankSize)
            {
                var pos = Vector3.zero;
                pos = NextPosV[i];

                NextDirV[i] = NextDirV[i] - 2 * Vector3.Dot(NextDirV[i], Vector3.back) * Vector3.back;

                pos.z = tankSize;

                NextPosV[i] = pos;
            }
            else if (NextPosV[i].z < -tankSize)
            {
                var pos = Vector3.zero;
                pos = NextPosV[i];

                NextDirV[i] = NextDirV[i] - 2 * Vector3.Dot(NextDirV[i], Vector3.forward) * Vector3.forward;

                pos.z = -tankSize;

                NextPosV[i] = pos;
            }
            */
            RaycastHit hit;

            if (Physics.Raycast(NextPosV[i], NextDirV[i], out hit, 3.0f))
            {
                NextDirV[i] = NextDirV[i] - 2 * Vector3.Dot(NextDirV[i], hit.normal) * hit.normal;
            }
        }
    }

    [SerializeField]
    private GameObject[] target;

    void ActiveObj()
    {
        for (int i = 0; i < target.Length; i++)
        {
            target[i].SetActive(true);
        }
    }
    void DeactiveObj()
    {
        for (int i = 0; i < target.Length; i++)
        {
            target[i].SetActive(false);
        }
    }
    void CalcGroup()
    {
        var P = Vector3.zero;
        var M = Vector3.zero;
        var C = Vector3.zero;
        var D = Vector3.zero;

        Pgroup = 0.0f;
        Mgroup = 0.0f;
        Cgroup = Vector3.zero;
        Dgroup = Vector3.zero;

        for (int i = 0; i < BoidID.Length; i++)
        {
            C += NextPosV[i];
            D += NextDirV[i];
        }

        Cgroup = C / N;
        Dgroup = D / N;

        for (int i = 0; i < BoidID.Length; i++)
        {
            P += NextDirV[i];
            //Debug.Log("NextDirV[i] :" + NextDirV[i]);
            var r = NextPosV[i] - Cgroup;

            r = r.normalized;

            M += Vector3.Cross(r, NextDirV[i]);
            //Debug.Log("Cross :" + Vector3.Cross(r, NextDirV[i]));
        }

        Pgroup = P.magnitude / N;
        Mgroup = M.magnitude / N;
        //Debug.Log("Mgroup :" + Mgroup);
    }

    [SerializeField]
    private int TIME = 5000;

    private int time = 0;

    private int count = 0;

    void Count()
    {
        time++;

        //Debug.Log("time :" + time);

        AverageP[count] += Pgroup;
        AverageM[count] += Mgroup;

        if (time % TIME == 0)
        {
            Debug.Log("time :" + time);

            Debug.Log("Pgroup :" + Pgroup);
            Debug.Log("Mgroup :" + Mgroup);

            AverageP[count] /= TIME;
            AverageM[count] /= TIME;

            Debug.Log("AverageP :" + AverageP[count]);
            Debug.Log("AverageM :" + AverageM[count]);

            //Debug.Break();

            Debug.Log("orientation :" + orientation);

            if (count <= 11)
            {
                orientation += 0.25f;
            }
            else
            {
                orientation -= 0.25f;
            }

            Debug.Log("Count: " + count);

            count++;

            if(count > 24)
            {
                Debug.Break();
            }
        }
    }
    void Start()
    {
        GenarateBoids();
    }

    void Update()
    {
        CheckDistance();

        if (!toggle)
        {
            Boundary();
            DeactiveObj();
        }
        else if (toggle)
        {
            Wall();
            ActiveObj();
        }

        UpdatePosition();

        CalcGroup();

        Draw();

        NextFrame();

        Count();
    }
}
