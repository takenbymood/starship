using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTerrain : MonoBehaviour
{

	public float scale = 0.25f;
	public int chunkSize = 10;
	public int xChunks = 0;
	public int yChunks = 0;
	public int zChunks = 0;

	public GameObject chunkType;


    // Start is called before the first frame update
    void Start()
    {
    	float gridScale = ((float) chunkSize)*scale;

        int nChunks = xChunks*yChunks*zChunks;

        for(int i=0; i<xChunks; i++){
        	for(int j=0; j<yChunks; j++){
        		for(int k=0; k<zChunks; k++){
			        GameObject chunk = Instantiate(chunkType, new Vector3(((float)i)*gridScale, ((float)j)*gridScale, ((float)k)*gridScale), Quaternion.identity);
			        chunk.transform.parent = transform;
			        VoxelChunk chunkCtrl = chunk.GetComponent<VoxelChunk>();
			        chunkCtrl.scale = scale;
			        chunkCtrl.SetSize(chunkSize,chunkSize,chunkSize);
			        chunkCtrl.RegenerateMesh();
			   	}
	    	}
	    }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
