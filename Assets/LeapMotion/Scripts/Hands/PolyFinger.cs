/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using Leap;

/** Draws the bones of the finger as a polygonal tube. **/
public class PolyFinger : FingerModel {

  const int MAX_SIDES = 30;
  const int TRIANGLE_INDICES_PER_QUAD = 6;
  const int VERTICES_PER_QUAD = 4;

  /** The number of sides for finger tube. */
  public int sides = 4;
  /** Whether to smooth the mesh normals, providing a smoother appearance.*/
  public bool smoothNormals = false;
  public float startingAngle = 0.0f;
  /** An array containing the width of the finger at each joint. */
  public float[] widths = new float[NUM_JOINTS];
  
  protected Vector3[] vertices_;
  protected Vector3[] normals_;
  protected Vector3[] joint_vertices_;

  protected Mesh mesh_;
  protected Mesh cap_mesh_;
  protected Vector3[] cap_vertices_;

  /** Creates the finger mesh. */
  public override void InitFinger() {
    InitJointVertices();
    InitCapsMesh();
    InitMesh();
    GetComponent<MeshFilter>().mesh = new Mesh();

    UpdateFinger();
  }

  /** Updates the mesh.*/
  public override void UpdateFinger() {
    UpdateMesh();
    UpdateCapMesh();
  }

  void OnDestroy() {
    Destroy(mesh_);
    Destroy(cap_mesh_);
    Destroy(GetComponent<MeshFilter>().mesh);
  }

  void Update() {
    if (vertices_ == null)
      return;

    mesh_.vertices = vertices_;

    if (smoothNormals)
      mesh_.normals = normals_;
    else
      mesh_.RecalculateNormals();

    cap_mesh_.vertices = cap_vertices_;
    cap_mesh_.RecalculateNormals();

    CombineInstance[] combine = new CombineInstance[2];
    combine[0].mesh = mesh_;
    combine[1].mesh = cap_mesh_;

    GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, false);
    GetComponent<MeshFilter>().mesh.RecalculateBounds();
  }

  protected Quaternion GetJointRotation(int joint) {
    if (joint <= 0)
      return GetBoneRotation(joint);
    if (joint >= NUM_BONES)
      return GetBoneRotation(joint - 1);

    return Quaternion.Slerp(GetBoneRotation(joint - 1), GetBoneRotation(joint), 0.5f);
  }

  protected void InitJointVertices() {
    joint_vertices_ = new Vector3[sides];
    for (int s = 0; s < sides; ++s) {
      float angle = startingAngle + s * 360.0f / sides;
      joint_vertices_[s] = Quaternion.AngleAxis(angle, -Vector3.forward) * Vector3.up;
    }
  }

  protected void UpdateMesh() {
    int vertex_index = 0;

    for (int i = 0; i < NUM_BONES; ++i) {
      Vector3 joint_position = transform.InverseTransformPoint(GetJointPosition(i));
      Vector3 next_joint_position = transform.InverseTransformPoint(GetJointPosition(i + 1));
      Quaternion joint_rotation = Quaternion.Inverse(transform.rotation) * (GetJointRotation(i));
      Quaternion next_joint_rotation = Quaternion.Inverse(transform.rotation) *
                                       GetJointRotation(i + 1);

      for (int s = 0; s < sides; ++s) {
        int next_side = (s + 1) % sides;

        if (smoothNormals) {
          Vector3 normal = joint_rotation * joint_vertices_[s];
          Vector3 next_normal = joint_rotation * joint_vertices_[next_side];

          normals_[vertex_index] = normals_[vertex_index + 2] = normal;
          normals_[vertex_index + 1] = normals_[vertex_index + 3] = next_normal;
        }

        Vector3 offset = joint_rotation * (widths[i] * joint_vertices_[s]);
        vertices_[vertex_index++] = joint_position + offset;

        offset = joint_rotation * (widths[i] * joint_vertices_[next_side]);
        vertices_[vertex_index++] = joint_position + offset;

        offset = next_joint_rotation * (widths[i + 1] * joint_vertices_[s]);
        vertices_[vertex_index++] = next_joint_position + offset;

        offset = next_joint_rotation * (widths[i + 1] * joint_vertices_[next_side]);
        vertices_[vertex_index++] = next_joint_position + offset;
      }
    }
  }

  protected void UpdateCapMesh() {
    Vector3 base_position = transform.InverseTransformPoint(GetJointPosition(0));
    Vector3 tip_position = transform.InverseTransformPoint(GetJointPosition(NUM_JOINTS - 1));
    Quaternion base_rotation = Quaternion.Inverse(transform.rotation) * GetJointRotation(0);
    Quaternion tip_rotation = Quaternion.Inverse(transform.rotation) *
                              GetJointRotation(NUM_JOINTS - 1);

    for (int s = 0; s < sides; ++s) {
      cap_vertices_[s] = base_position + base_rotation * (widths[0] * joint_vertices_[s]);
      cap_vertices_[sides + s] = tip_position + tip_rotation *
                                 (widths[NUM_JOINTS - 1] * joint_vertices_[s]);
    }
  }

  protected void InitMesh() {
    mesh_ = new Mesh();
    mesh_.MarkDynamic();

    int vertex_index = 0;
    int num_vertices = VERTICES_PER_QUAD * sides * NUM_BONES;
    vertices_ = new Vector3[num_vertices];
    normals_ = new Vector3[num_vertices];
    Vector2[] uv = new Vector2[num_vertices];

    int triangle_index = 0;
    int num_triangles = TRIANGLE_INDICES_PER_QUAD * sides * NUM_BONES;
    int[] triangles = new int[num_triangles];

    for (int i = 0; i < NUM_BONES; ++i) {
      for (int s = 0; s < sides; ++s) {

        triangles[triangle_index++] = vertex_index;
        triangles[triangle_index++] = vertex_index + 2;
        triangles[triangle_index++] = vertex_index + 1;

        triangles[triangle_index++] = vertex_index + 2;
        triangles[triangle_index++] = vertex_index + 3;
        triangles[triangle_index++] = vertex_index + 1;

        uv[vertex_index] = new Vector3((1.0f * s) / sides, (1.0f * i) / NUM_BONES);
        uv[vertex_index + 1] = new Vector3((1.0f + s) / sides, (1.0f * i) / NUM_BONES);
        uv[vertex_index + 2] = new Vector3((1.0f * s) / sides, (1.0f + i) / NUM_BONES);
        uv[vertex_index + 3] = new Vector3((1.0f + s) / sides, (1.0f + i) / NUM_BONES);

        vertices_[vertex_index++] = new Vector3(0, 0, 0);
        vertices_[vertex_index++] = new Vector3(0, 0, 0);
        vertices_[vertex_index++] = new Vector3(0, 0, 0);
        vertices_[vertex_index++] = new Vector3(0, 0, 0);
      }
    }
    mesh_.vertices = vertices_;
    mesh_.normals = normals_;
    mesh_.uv = uv;
    mesh_.triangles = triangles;
  }

  protected void InitCapsMesh() {
    cap_mesh_ = new Mesh();
    cap_mesh_.MarkDynamic();

    cap_vertices_ = cap_mesh_.vertices;
    int num_vertices = 2 * sides;
    if (num_vertices != cap_vertices_.Length)
      Array.Resize(ref cap_vertices_, num_vertices);

    Vector2[] uv = cap_mesh_.uv;
    if (uv.Length != num_vertices)
      Array.Resize(ref uv, num_vertices);

    int triangle_index = 0;
    int[] triangles = cap_mesh_.triangles;
    int num_triangles = 2 * 3 * (sides - 2);
    if (num_triangles != triangles.Length)
      Array.Resize(ref triangles, num_triangles);

    for (int i = 0; i < sides; ++i) {
      cap_vertices_[i] = new Vector3(0, 0, 0);
      cap_vertices_[i + sides] = new Vector3(0, 0, 0);
      uv[i] = 0.5f * joint_vertices_[i];
      uv[i] += new Vector2(0.5f, 0.5f);
      uv[i + sides] = 0.5f * joint_vertices_[i];
      uv[i + sides] += new Vector2(0.5f, 0.5f);
    }

    for (int i = 0; i < sides - 2; ++i) {
      triangles[triangle_index++] = 0;
      triangles[triangle_index++] = i + 1;
      triangles[triangle_index++] = i + 2;

      triangles[triangle_index++] = sides;
      triangles[triangle_index++] = sides + i + 2;
      triangles[triangle_index++] = sides + i + 1;
    }

    cap_mesh_.vertices = cap_vertices_;
    cap_mesh_.uv = uv;
    cap_mesh_.triangles = triangles;
  }
}
