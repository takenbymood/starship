using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]



public class VoxelGrid : MonoBehaviour
{

	public float scale; //the size of one voxel
	public int xSize, ySize, zSize;
	public int chunkSize; //the dimensions of one cubic chunk
	private int[,,] grid; //the voxel grid
	private Vector3[] vertices;
	private int[] triangles;
	private int voxelCount;
	private Mesh mesh;

	// enum bitflags for faces
	public enum FaceFlags {
	    xBack   	= 1,    
	    xForward  	= 2,    
	    yBack   	= 4,   
	    yForward 	= 8,   
	    zBack 		= 16,     
	    zForward 	= 32
	}

	public static Vector3[] cubeVertices = {
		new Vector3 (0.0f, 0.0f, 0.0f),
		new Vector3 (1.0f, 0.0f, 0.0f),
		new Vector3 (1.0f, 1.0f, 0.0f),
		new Vector3 (0.0f, 1.0f, 0.0f),
		new Vector3 (0.0f, 1.0f, 1.0f),
		new Vector3 (1.0f, 1.0f, 1.0f),
		new Vector3 (1.0f, 0.0f, 1.0f),
		new Vector3 (0.0f, 0.0f, 1.0f),
    };

 //    public static int[] cubeTriangles = {
	// 	0, 2, 1, //face front
	// 	0, 3, 2,
	// 	2, 3, 4, //face top
	// 	2, 4, 5,
	// 	1, 2, 5, //face right
	// 	1, 5, 6,
	// 	0, 7, 4, //face left
	// 	0, 4, 3,
	// 	5, 4, 7, //face back
	// 	5, 7, 6,
	// 	0, 6, 7, //face bottom
	// 	0, 1, 6
	// };

	public static int[] frontFace = {
		0, 2, 1, //face front
		0, 3, 2
	};

	public static int[] topFace = {
		2, 3, 4, //face top
		2, 4, 5
	};

	public static int[] rightFace = {
		1, 2, 5, //face right
		1, 5, 6
	};

	public static int[] leftFace = {
		0, 7, 4, //face left
		0, 4, 3
	};

	public static int[] backFace = {
		5, 4, 7, //face back
		5, 7, 6
	};

	public static int[] bottomFace = {
		0, 6, 7, //face bottom
		0, 1, 6
	};

	void Awake(){
		mesh = GetComponent<MeshFilter>().mesh;
	}

	void GenerateGridData(){
		grid = new int[xSize,ySize,zSize];
        for(int i = 0; i < xSize; i++){
        	for(int j = 0; j < ySize; j++){
        		for(int k = 0; k < zSize; k++){
        			grid[i,j,k] = Random.Range(0,2);
        		}
        	}
        }
	}

	void MakeCube(){

	}

	void GenerateMesh(){
		int v=0;
        int t=0;
        int nFaces = 0;

        List<Vector3> gridSurface = new List<Vector3>();

        List<int> gridFaces = new List<int>();

        for(int i = 0; i < xSize; i++){
        	for(int j = 0; j < ySize; j++){
        		for(int k = 0; k < zSize; k++){
        			if(grid[i,j,k] != 0){

        				//only include cubes which are exposed to the surface

        				int faces = 0;

        				//check if xBack and xForward are empty
        				bool iB = i>0?grid[i-1,j,k] == 0:true;
        				bool iF = i<(xSize-1)?grid[i+1,j,k]==0:true;
        				
        				

        				//same for yBack and yForward
        				bool jB = j>0?grid[i,j-1,k] == 0:true;
        				bool jF = j<(ySize-1)?grid[i,j+1,k]==0:true;


        				bool kB = k>0?grid[i,j,k-1] == 0:true;
        				bool kF = k<(zSize-1)?grid[i,j,k+1]==0:true;

        				



        				if(iB || iF || jB || jF || kB || kF){
        					gridSurface.Add(new Vector3(i,j,k));
        					//append bit flags for faces

	        				if(iB){
	        					faces += (int)FaceFlags.xBack;
	        					nFaces += 1;
	        				}
	        				if(iF){
	        					faces += (int)FaceFlags.xForward;
	        					nFaces += 1;
	        				}

	        				if(jB){
	        					faces += (int)FaceFlags.yBack;
	        					nFaces += 1;
	        				}
	        				if(jF){
	        					faces += (int)FaceFlags.yForward;
	        					nFaces += 1;
	        				}

	        				if(kB){
	        					faces += (int)FaceFlags.zBack;
	        					nFaces += 1;
	        				}
	        				if(kF){
	        					faces += (int)FaceFlags.zForward;
	        					nFaces += 1;
	        				}

	        				gridFaces.Add(faces);

        				}

        			}
        			
        		}
        	}
        }

        int activeVoxelCount = gridSurface.Count;

        vertices = new Vector3[activeVoxelCount*8];
    	triangles = new int[nFaces*6];

        for(int g=0;g<gridSurface.Count;g++){
        	Vector3 gP = gridSurface[g];
        	float gi = gP[0];
        	float gj = gP[1];
        	float gk = gP[2];
        	var faces = (FaceFlags) gridFaces[g];
			Vector3 pos = new Vector3(scale*((float)gi),scale*((float)gj),scale*((float)gk));
			for(int lv=0; lv < 8; lv++){
				vertices[lv+v] =  pos + scale*cubeVertices[lv];
			}
			int nDrawn = 0;
			
			if((faces & FaceFlags.xBack) != 0){
				for(int lt=0; lt < 6; lt++){
					triangles[lt+t] = leftFace[lt]+v;
				}
				t+=6;
			}
			if((faces & FaceFlags.xForward) != 0){
				for(int lt=0; lt < 6; lt++){
					triangles[lt+t] = rightFace[lt]+v;
				}
				t+=6;
			}
			if((faces & FaceFlags.yBack) != 0){
				for(int lt=0; lt < 6; lt++){
					triangles[lt+t] = bottomFace[lt]+v;
				}
				t+=6;
			}
			if((faces & FaceFlags.yForward) != 0){
				for(int lt=0; lt < 6; lt++){
					triangles[lt+t] = topFace[lt]+v;
				}
				t+=6;
			}
			if((faces & FaceFlags.zForward) != 0){
				for(int lt=0; lt < 6; lt++){
					triangles[lt+t] = backFace[lt]+v;
				}
				t+=6;
			}
			if((faces & FaceFlags.zBack) != 0){
				for(int lt=0; lt < 6; lt++){
					triangles[lt+t] = frontFace[lt]+v;
				}
				t+=6;
			}
				
				
			v+=8;
			// t+=nDrawn*6;
        }
        				
    }

	void UpdateMeshRenderer(){
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
        mesh.RecalculateNormals();
	}

    // Start is called before the first frame update
    void Start()
    {
    	voxelCount = xSize*ySize*zSize;
    	

        GenerateGridData();
        GenerateMesh();
        UpdateMeshRenderer();
        
		
    }

    // Update is called once per frame
    void Update()
    {
        mesh.RecalculateNormals();
    }
}
