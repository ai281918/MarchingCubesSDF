using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCTest : MonoBehaviour
{
    public MarchingCubesVisualization marchingCubesVisualization;
    float[,,] sdf;

    private void Awake() {
         sdf = new float[64, 64, 64];
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<64;++i){
            for(int j=0;j<64;++j){
                for(int k=0;k<64;++k){
                    sdf[i, j, k] = (i > 16 && i < 48 && j > 16 && j < 48 && k > 16 && k < 48) ? -1 : 1;
                }
            }
        }
        marchingCubesVisualization.Visualization(sdf);
    }
}
