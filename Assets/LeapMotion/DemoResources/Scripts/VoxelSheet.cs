using UnityEngine;
using System.Collections;

public class VoxelSheet : MonoBehaviour {

  public Transform voxelModel;
  public int maxVoxels = 1000;

  protected Transform[] voxelStash;

  void Start() {
    voxelStash = new Transform[maxVoxels];
    for (int i = 0; i < maxVoxels; ++i) {
      voxelStash[i] = Instantiate(voxelModel) as Transform;
      voxelStash[i].gameObject.SetActive(true);
    }
  }

  void OnDestroy() {
    for (int i = 0; i < maxVoxels; ++i) {
      if (voxelStash[i] != null)
        Destroy(voxelStash[i].gameObject);
    }
  }

  void Update() {
    float voxels_x = transform.lossyScale.x / voxelModel.localScale.x;
    float voxels_z = transform.lossyScale.z / voxelModel.localScale.z;

    int voxel_index = 0;
    for (int i = 0; i < voxels_x / 0.7f; ++i) {
      for (int j = 0; j < voxels_z / 0.7f; ++j) {
        if (voxel_index >= voxelStash.Length)
          return;

        Vector3 local_vector = new Vector3(0.7f * (i - voxels_x / 1.4f) / voxels_x, 0.0f, 0.7f * (j - voxels_z / 1.4f) / voxels_z);
        Vector3 global_vector = transform.TransformPoint(local_vector);

        global_vector.x = ((int)(global_vector.x / voxelModel.localScale.x)) * voxelModel.localScale.x;
        global_vector.y = ((int)(global_vector.y / voxelModel.localScale.y)) * voxelModel.localScale.y;
        global_vector.z = ((int)(global_vector.z / voxelModel.localScale.z)) * voxelModel.localScale.z;

        voxelStash[voxel_index].position = global_vector;
        voxel_index++;
      }
    }

    for (; voxel_index < voxelStash.Length; ++voxel_index)
      voxelStash[voxel_index].position = Vector3.zero;
  }
}
