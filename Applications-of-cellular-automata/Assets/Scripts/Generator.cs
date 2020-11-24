using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    // Start is called before the first frame update
    public int width=100;
    public int height=100;
    public float span=1;
    public float maxHeight=10;
    public GameObject cubePrefab;
    float delta=0;
    GameObject[,] cell;
    int[] dx={0,1,1,1,0,-1,-1,-1};
    int[] dz={1,1,0,-1,-1,-1,0,1};
    void Start()
    {
        this.cell=new GameObject[height,width];
        //セルの初期化
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //cubeをセルとして使う
                GameObject cube = Instantiate(cubePrefab) as GameObject;
                cube.GetComponent<Renderer>().material.color = Color.red;
                cube.transform.position = new Vector3 (i, 0, j);
                Vector3 scale=cube.transform.localScale;
                scale.y=0;
                cube.transform.localScale=scale;
                this.cell[i,j]=cube;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Space押した時だけ動く
        if(TimeChecker())
        {
            Solver();
        }
        else
        {
            Edit();
        }
    }
    //鹿威し
    bool TimeChecker()
    {
        this.delta += Time.deltaTime;//updateの間隔の時間
        if (this.delta > this.span&Input.GetKey(KeyCode.Space)){
            this.delta=0;
            return true;
        }else{
            return false;
        }
    }
    //カウント
    float Count(int x,int z){
        float high=0f;
        for (int  i= 0;  i< 8; i++)
        {
            
            int nx=x+dx[i];
            int nz=z+dz[i];
            if((0 <= nx & nx < height) & (0 <= nz & nz < width))
            {
                high+=cell[nx,nz].transform.localScale.y;
            }
        }
        return high;
    }
    //ルール
    float Rule(float cellHeight,int x,int z)
    {
        //dead
        if(cell[x,z].transform.localScale.y==0)
        {
            if(maxHeight<=cellHeight & cellHeight<maxHeight*3)
            {
                cellHeight=((cellHeight-maxHeight)/(2*maxHeight))*maxHeight;
            }else if(maxHeight*3<=cellHeight & cellHeight<maxHeight*6)
            {
                cellHeight=((maxHeight*6-cellHeight)/(3*maxHeight))*maxHeight;
            }else
            {
                cellHeight=0;
            }
        }else//alive
        {
            float nowCell=cell[x,z].transform.localScale.y;
            if(maxHeight<=cellHeight & cellHeight<maxHeight*2)
            {
                cellHeight=((cellHeight-maxHeight)/(maxHeight))*nowCell;
            }else if(maxHeight*2<=cellHeight & cellHeight<maxHeight*3)
            {
                //かわらんち
                cellHeight=nowCell;
            }else if(maxHeight*3<=cellHeight & cellHeight<maxHeight*6)
            {
                cellHeight=((maxHeight*6-cellHeight)/(3*maxHeight))*nowCell;
            }else
            {
                cellHeight=0;
            }
        }

        return cellHeight;
    }
    //cellの更新
    void Solver(){
        //次のcell
        float[,] tmpCell=new float[height,width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                tmpCell[i,j]=Count(i,j);
            }
        }
        //ルールに従って更新
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 scale=cell[i,j].transform.localScale;
                scale.y=Rule(tmpCell[i,j],i,j);
                cell[i,j].transform.localScale=scale;
            }
        }

    }
    //Editモード
    void Edit()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                int x = Mathf.RoundToInt(hit.point.x);
                int z = Mathf.RoundToInt(hit.point.z);
                if((0 <= x & x < height) & (0 <= z & z < width))
                {
                    Vector3 scale=cell[x,z].transform.localScale;
                    scale.y=maxHeight;
                    cell[x,z].transform.localScale=scale;
                }
            }
        }
    }

}
