using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Cell{
    public int solid;
    public float x, y, z;
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MarchingCubesVisualization : MonoBehaviour
{
    Vector3Int id;
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    Mesh mesh;

    private void Awake() {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    public void Visualization(float[,,] sdf){
        Vector3Int size = new Vector3Int(sdf.GetLength(0), sdf.GetLength(1), sdf.GetLength(2));
        Cell[,,] cells = new Cell[size.x, size.y, size.z];

        ExtractCell(size, sdf, cells);
        ExtractMesh(size, cells);
        UpdateMesh();
    }

    void ExtractCell(Vector3Int size, float[,,] sdf, Cell[,,] cells){
        for(int i=0;i<size.x;++i){
            for(int j=0;j<size.y;++j){
                for(int k=0;k<size.z;++k){
                    cells[i, j, k].solid = sdf[i, j, k] <= 0 ? 1 : 0;
                    if(i != size.x-1){
                        if(Mathf.Sign(sdf[i, j, k]) != Mathf.Sign(sdf[i+1, j, k])){
                            cells[i, j, k].x = Mathf.Abs(sdf[i, j, k]) / (Mathf.Abs(sdf[i, j, k]) + Mathf.Abs(sdf[i+1, j, k]));
                        }
                    }
                    if(j != size.y-1){
                        if(Mathf.Sign(sdf[i, j, k]) != Mathf.Sign(sdf[i, j+1, k])){
                            cells[i, j, k].y = Mathf.Abs(sdf[i, j, k]) / (Mathf.Abs(sdf[i, j, k]) + Mathf.Abs(sdf[i, j+1, k]));
                        }
                    }
                    if(k != size.z-1){
                        if(Mathf.Sign(sdf[i, j, k]) != Mathf.Sign(sdf[i, j, k+1])){
                            cells[i, j, k].z = Mathf.Abs(sdf[i, j, k]) / (Mathf.Abs(sdf[i, j, k]) + Mathf.Abs(sdf[i, j, k+1]));
                        }
                    }
                }
            }
        }
    }

    void ExtractMesh(Vector3Int size, Cell[,,] cells){
        Clear();

        int [,,,] verticeID = new int[size.x, size.y, size.z, 3];

        for (int i=0;i<size.x-1;++i)
        {
            for (int j=0;j<size.y-1;++j)
            {
                for (int k=0;k<size.z-1;++k)
                {
                    int enumVertice = (cells[i, j, k].solid) |
                                    (cells[i+1, j, k].solid << 1) |
                                    (cells[i, j, k+1].solid << 2) |
                                    (cells[i+1, j, k+1].solid << 3) |
                                    (cells[i, j+1, k].solid << 4) |
                                    (cells[i+1, j+1, k].solid << 5) |
                                    (cells[i, j+1, k+1].solid << 6) |
                                    (cells[i+1, j+1, k+1].solid << 7);

                    for(int n = 1; n <= MarchingCubesTable.newVerticeTable[enumVertice, 0]; ++n){
                        switch (MarchingCubesTable.newVerticeTable[enumVertice, n])
                        {
                            case 0:
                                if(j == 0 && k == 0){
                                    verticeID[i, j, k, 0] = vertices.Count;
                                    vertices.Add(new Vector3(cells[i, j, k].x + i, j, k));
                                }
                                break;
                            case 1:
                                if(j == 0){
                                    verticeID[i+1, j, k, 2] = vertices.Count;
                                    vertices.Add(new Vector3(i+1, j, cells[i+1, j, k].z + k));
                                }
                                break;
                            case 2:
                                if(j == 0){
                                    verticeID[i, j, k+1, 0] = vertices.Count;
                                    vertices.Add(new Vector3(cells[i, j, k+1].x + i, j, k+1));
                                }
                                break;
                            case 3:
                                if(i == 0 && j == 0){
                                    verticeID[i, j, k, 2] = vertices.Count;
                                    vertices.Add(new Vector3(i, j, cells[i, j, k].z + k));
                                }
                                break;
                            case 4:
                                if(i == 0 && k == 0){
                                    verticeID[i, j, k, 1] = vertices.Count;
                                    vertices.Add(new Vector3(i, cells[i, j, k].y + j, k));
                                }
                                break;
                            case 5:
                                if(k == 0){
                                    verticeID[i+1, j, k, 1] = vertices.Count;
                                    vertices.Add(new Vector3(i+1, cells[i+1, j, k].y + j, k));
                                }
                                break;
                            case 6:
                                if(i == 0){
                                    verticeID[i, j, k+1, 1] = vertices.Count;
                                    vertices.Add(new Vector3(i, cells[i, j, k+1].y + j, k+1));
                                }
                                break;
                            case 7:
                                verticeID[i+1, j, k+1, 1] = vertices.Count;
                                vertices.Add(new Vector3(i+1, cells[i+1, j, k+1].y + j, k+1));
                                break;
                            case 8:
                                if(k == 0){
                                    verticeID[i, j+1, k, 0] = vertices.Count;
                                    vertices.Add(new Vector3(cells[i, j+1, k].x + i, j+1, k));

                                }
                                break;
                            case 9:
                                verticeID[i+1, j+1, k, 2] = vertices.Count;
                                vertices.Add(new Vector3(i+1, j+1, cells[i+1, j+1, k].z + k));
                                break;
                            case 10:
                                verticeID[i, j+1, k+1, 0] = vertices.Count;
                                vertices.Add(new Vector3(cells[i, j+1, k+1].x + i, j+1, k+1));
                                break;
                            case 11:
                                if(i == 0){
                                    verticeID[i, j+1, k, 2] = vertices.Count;
                                    vertices.Add(new Vector3(i, j+1, cells[i, j+1, k].z + k));
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    for (int n = 1; n <= MarchingCubesTable.polygonTable[enumVertice, 0]*3; ++n)
                    {
                        triangles.Add(verticeID[i+MarchingCubesTable.polygonOffset[MarchingCubesTable.polygonTable[enumVertice, n], 0], j+MarchingCubesTable.polygonOffset[MarchingCubesTable.polygonTable[enumVertice, n], 1], k+MarchingCubesTable.polygonOffset[MarchingCubesTable.polygonTable[enumVertice, n], 2], MarchingCubesTable.polygonOffset[MarchingCubesTable.polygonTable[enumVertice, n], 3]]);
                    }
                }
            }
        }
    }

    void Clear(){
        vertices.Clear();
        triangles.Clear();
    }

    void UpdateMesh(){
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
