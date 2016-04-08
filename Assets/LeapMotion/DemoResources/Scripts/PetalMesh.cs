/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class PetalMesh : MonoBehaviour {

  public int numVertices = 20;
  public Collider firstSegment;
  public Collider lastSegment;
  public AnimationCurve flowerCurve;
  public AnimationCurve morphCurve;
  public AnimationCurve growthDelayCurve;
  public float morph = 0.0f;
  public float growthProgress = 1.0f;

  private Vector3[] vertices_;
  private Mesh front_;
  private Mesh back_;

  void Start () {
    GetComponent<MeshFilter>().mesh = new Mesh();
    InitMesh();
  }

  void Update () {
    UpdateMesh();
    front_.MarkDynamic();
    front_.vertices = vertices_;
    back_.MarkDynamic();
    back_.vertices = vertices_;
    front_.RecalculateNormals();
    back_.RecalculateNormals();

    MeshFilter filter = GetComponent<MeshFilter>();

    CombineInstance[] combine = new CombineInstance[2];
    combine[0].mesh = front_;
    combine[1].mesh = back_;

    filter.mesh.CombineMeshes(combine, true, false);
    filter.mesh.RecalculateBounds();
  }

  private void InitMesh() {
    front_ = new Mesh();
    back_ = new Mesh();

    vertices_ = new Vector3[numVertices];
    Vector2[] uv = new Vector2[numVertices];

    int[] front_triangles = new int[6 * (numVertices - 2)];
    int[] back_triangles = new int[6 * (numVertices - 2)];

    for (int v = 0; v < numVertices; ++v)
      vertices_[v] = new Vector3(0, 0, 0);

    for (int v = 0; v < numVertices / 2; ++v) {
      uv[2 * v] = new Vector2(0, (2.0f * v) / (numVertices - 2));
      uv[2 * v + 1] = new Vector2(1, (2.0f * v) / (numVertices - 2));
    }

    for (int v = 0; v < (numVertices - 2) / 2; ++v) {
      front_triangles[6 * v] = 2 * v;
      front_triangles[6 * v + 1] = 2 * v + 1;
      front_triangles[6 * v + 2] = 2 * v + 2;

      front_triangles[6 * v + 3] = 2 * v + 1;
      front_triangles[6 * v + 4] = 2 * v + 3;
      front_triangles[6 * v + 5] = 2 * v + 2;

      back_triangles[6 * v] = 2 * v;
      back_triangles[6 * v + 1] = 2 * v + 2;
      back_triangles[6 * v + 2] = 2 * v + 1;

      back_triangles[6 * v + 3] = 2 * v + 1;
      back_triangles[6 * v + 4] = 2 * v + 2;
      back_triangles[6 * v + 5] = 2 * v + 3;
    }

    front_.vertices = vertices_;
    front_.uv = uv;
    front_.triangles = front_triangles;

    back_.vertices = vertices_;
    back_.uv = uv;
    back_.triangles = back_triangles;
  }

  private void UpdateMesh() {
    for (int v = 0; v < numVertices / 2; ++v) {
      float phase = (2.0f * v) / (numVertices - 2);
      float flower_width = flowerCurve.Evaluate(phase);
      float width = flower_width + morph * (morphCurve.Evaluate(phase) - flower_width);
      float growth_delay = growthDelayCurve.Evaluate(phase);
      float growth_amount = (growthProgress - growth_delay) / (1.0f - growth_delay);
      width *= Mathf.Clamp(growth_amount, 0.0f, 1.0f);

      Vector3 vertex1 = new Vector3(2.0f * phase - 1, 0, -width);
      Vector3 vertex1_a = firstSegment.transform.TransformPoint(vertex1);
      Vector3 vertex1_b = lastSegment.transform.TransformPoint(vertex1);
      Vector3 vertex1_t = vertex1_a + phase * (vertex1_b - vertex1_a);

      Vector3 vertex2 = new Vector3(2.0f * phase - 1, 0, width);
      Vector3 vertex2_a = firstSegment.transform.TransformPoint(vertex2);
      Vector3 vertex2_b = lastSegment.transform.TransformPoint(vertex2);
      Vector3 vertex2_t = vertex2_a + phase * (vertex2_b - vertex2_a);

      vertices_[2 * v] = transform.InverseTransformPoint(vertex1_t);
      vertices_[2 * v + 1] = transform.InverseTransformPoint(vertex2_t);
    }
  }
}
