/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class StemMesh : MonoBehaviour {

  const int TRIANGLE_INDICES_PER_QUAD = 6;
  const int VERTICES_PER_QUAD = 4;

  public Collider[] segments;
  public int sides = 4;
  public AnimationCurve stemCurve;
  public float growthProgress = 0.0f;
  public float stumpDiminishTime = 2.0f;

  private Vector3[] vertices_;
  private bool[] broken;
  private float stump_width_ = 1.0f;
  private bool is_diminishing_ = false;

  void Start () {
    broken = new bool[segments.Length];
    for (int i = 0; i < broken.Length; ++i)
      broken[i] = false;

    MeshFilter filter = GetComponent<MeshFilter>();
    filter.mesh = new Mesh();
    InitMesh();
    filter.mesh.RecalculateBounds();
    filter.mesh.RecalculateNormals();
  }

  void Update () {
    UpdateMesh();
    MeshFilter filter = GetComponent<MeshFilter>();

    filter.mesh.MarkDynamic();
    filter.mesh.vertices = vertices_;
    filter.mesh.RecalculateBounds();
    filter.mesh.RecalculateNormals();
  }

  public bool IsBroken() {
    if (broken == null)
      return false;

    for (int i = 0; i < broken.Length; ++i) {
      if (broken[i])
        return true;
    }
    return false;
  }

  public bool IsStumpClear() {
    return stump_width_ == 0.0f;
  }

  public void RemoveStump() {
    is_diminishing_ = true;
  }

  private void InitMesh() {
    Mesh mesh = GetComponent<MeshFilter>().mesh;

    int num_vertices = sides * segments.Length;
    vertices_ = new Vector3[num_vertices];
    Vector2[] uv = new Vector2[num_vertices];

    int[] triangles = new int[TRIANGLE_INDICES_PER_QUAD * (num_vertices - sides)];

    for (int v = 0; v < num_vertices; ++v) {
      vertices_[v] = new Vector3(0, 0, 0);
      uv[v] = new Vector3(0, 0, 0);
    }

    int vertex_index = 0;
    for (int seg = 0; seg < segments.Length - 1; ++seg) {
      if (segments[seg + 1].GetComponent<HingeJoint>() == null) {
        for (int i = 0; i < sides * TRIANGLE_INDICES_PER_QUAD; ++i)
          triangles[vertex_index++] = 0;
      }
      else {
        for (int side = 0; side < sides; ++side) {
          int next_side = (side + 1) % sides;
          triangles[vertex_index++] = sides * seg + side;
          triangles[vertex_index++] = sides * seg + next_side;
          triangles[vertex_index++] = sides * (seg + 1) + next_side;

          triangles[vertex_index++] = sides * seg + side;
          triangles[vertex_index++] = sides * (seg + 1) + next_side;
          triangles[vertex_index++] = sides * (seg + 1) + side;
        }
      }
    }

    mesh.vertices = vertices_;
    mesh.uv = uv;
    mesh.triangles = triangles;
  }

  protected void Separate(int index) {
    for (int i = 0; i < index; ++i)
      segments[i] = null;
  }

  private void Break(int index) {
    for (int i = index; i < segments.Length; ++i) {
      segments[i] = null;
    }
  }

  private void KillStump() {
    for (int i = 0; i < broken.Length && !broken[i]; ++i)
      segments[i].GetComponent<Collider>().enabled = false;
  }

  private void UpdateMesh() {
    if (is_diminishing_ && stump_width_ != 0.0f) {
      stump_width_ -= Time.deltaTime / stumpDiminishTime;
      stump_width_ = Mathf.Clamp(stump_width_, 0.0f, 1.0f);
      if (stump_width_ == 0.0f)
        KillStump();
    }

    for (int i = 0; i < segments.Length; ++i) {
      if (!broken[i] && segments[i].GetComponent<HingeJoint>() == null) {
        InitMesh();
        broken[i] = true;
      }
    }

    int vertex_index = 0;
    float angle = 360.0f / sides;

    bool is_stump = true;
    for (int i = 0; i < segments.Length; ++i) {
      float phase = (1.0f * i) / (segments.Length - 1);
      float width = Mathf.Clamp(segments.Length * growthProgress - i, 0.0f, 1.0f);

      is_stump = is_stump && !broken[i];
      if (is_stump)
        width *= stump_width_;

      Vector3 offset = new Vector3(width * stemCurve.Evaluate(phase), 0, 0);

      for (int side = 0; side < sides; ++side) {
        vertices_[vertex_index++] =
            transform.InverseTransformPoint(segments[i].transform.TransformPoint(offset));
        offset = Quaternion.AngleAxis(angle, Vector3.up) * offset;
      }
    }
  }
}
