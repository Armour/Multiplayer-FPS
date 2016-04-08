/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class CubeWave : MonoBehaviour {

  private const float BRIGHTNESS_SCALE = 5.0f;
  private const float BASE_BRIGHTNESS = 0.2f;

  public int gridWidth = 25;
  public int gridHeight = 25;
  public float cellWidth = 1.0f;
  public float cellHeight = 1.0f;
  public float springConstant = 200.0f;
  public float damping = 0.01f;

  public Transform model;
  public Light lowGlow;
  public Light highGlow;
  public Color lowColor;
  public Color highColor;

  Transform[,] cube_grid_;

  void Start() {
    cube_grid_ = new Transform[gridHeight + 2, gridWidth + 2];
    for (int r = 0; r < gridHeight + 2; ++r) {
      for (int c = 0; c < gridWidth + 2; ++c) {
        cube_grid_[r, c] = transform;
      }
    }

    float center_x = gridWidth * cellWidth / 2.0f;
    float center_y = gridHeight * cellHeight / 2.0f;

    // Setup the cube grid.
    for (int r = 1; r <= gridHeight; ++r) {
      for (int c = 1; c <= gridWidth; ++c) {
        Vector3 location = new Vector3(c * cellWidth - center_x, 0, r * cellHeight - center_y);
        cube_grid_[r, c] = Instantiate(model, transform.position + location,
                                       transform.rotation * model.transform.rotation) as Transform;
        cube_grid_[r, c].parent = transform;
      }
    }
  }

  void Update() {
    float low_total_light = 0.0f;
    float high_total_light = 0.0f;

    for (int r = 1; r <= gridHeight; ++r) {
      for (int c = 1; c <= gridWidth; ++c) {

        // Discrete wave equation with damping.
        float neighbor_sum = (cube_grid_[r - 1, c].position.y + cube_grid_[r, c - 1].position.y +
                              cube_grid_[r + 1, c].position.y + cube_grid_[r, c + 1].position.y);

        float delta_from_rest = 0.25f * neighbor_sum - cube_grid_[r, c].position.y;
        cube_grid_[r, c].GetComponent<Rigidbody>().AddForce(springConstant * Vector3.up * delta_from_rest);
        cube_grid_[r, c].GetComponent<Rigidbody>().velocity *= (1 - damping);

        // Set color of cube and add to glow amount based on current height.
        float delta_zero = transform.position.y - cube_grid_[r, c].position.y;
        float brightness = BASE_BRIGHTNESS +
                           BRIGHTNESS_SCALE * Mathf.Log(1 + 0.2f * delta_zero);

        if (brightness < 0) {
          cube_grid_[r, c].GetComponent<Renderer>().material.SetColor("_Color", highColor * (-brightness));
          high_total_light -= brightness / (gridHeight * gridWidth);
        }
        else {
          cube_grid_[r, c].GetComponent<Renderer>().material.SetColor("_Color", lowColor * brightness);
          low_total_light += brightness / (gridHeight * gridWidth);
        }
      }
    }

    // Set glow amount.
    if (lowGlow != null)
      lowGlow.intensity = low_total_light;
    if (highGlow != null)
      highGlow.intensity = high_total_light;
  }
}
