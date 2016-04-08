/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Leap;

/** The states of the record-playback machine. */
public enum RecorderState {
  Idling = 0,
  Recording = 1,
  Playing = 2
}

/**
 * Maintains a buffer of recorded frames and tracks the state of playback and recording.
 */
public class LeapRecorder {

  /** Playback speed. */
  public float speed = 1.0f;
  /** Whether to lop back to the beginning when the last recorded frame is played.*/
  public bool loop = true;
  /** The current play state. */
  public RecorderState state = RecorderState.Playing;

  protected List<byte[]> frames_;
  protected float frame_index_;
  protected Frame current_frame_ = new Frame();
  
  /** Creates a new LeapRecorder object. This doesn't make sense outside the context of a HandController object. */
  public LeapRecorder() {
    Reset();
  }

  /** Sets the play state to idle. */
  public void Stop() {
    state = RecorderState.Idling;
    frame_index_ = 0.0f;
  }

  /** Sets the play state to idle. */
  public void Pause() {
    state = RecorderState.Idling;
  }

  /** Sets the play state to playing. */
  public void Play() {
    state = RecorderState.Playing;
  }

  /** Sets the play state to recording. */
  public void Record() {
    state = RecorderState.Recording;
  }
  
  /** Discards any recorded frames. */
  public void Reset() {
    frames_ = new List<byte[]>();
    frame_index_ = 0;
  }
  
  /** Restores the default behaviors. */
  public void SetDefault() {
    speed = 1.0f;
    loop = true;
  }

  /** Returns the ratio of the current playback position to the total recording length. */
  public float GetProgress() {
    return frame_index_ / frames_.Count;
  }

  /** Returns the playback position. */
  public int GetIndex() {
    return (int)frame_index_;
  }

  /** 
   * Sets the playback position to the specified frame count (or the last frame if the 
   * specified index is after the last frame.
   */
  public void SetIndex(int new_index) { 
    if (new_index >= frames_.Count) {
      frame_index_ = frames_.Count - 1;
    }
    else {
      frame_index_ = new_index; 
    }
  }
  
  /** Serializes a Leap Frame object and adds it to the end of the recording. */
  public void AddFrame(Frame frame) {
    frames_.Add(frame.Serialize);
  }

  /** Returns the current frame without advancing the playhead. This frame could be invalid. */
  public Frame GetCurrentFrame() {
    return current_frame_;
  }
  
  /** Advances the playhead, deserializes the frame, and returns it.*/
  public Frame NextFrame() {
    current_frame_ = new Frame();
    if (frames_.Count > 0) {
      if (frame_index_ >= frames_.Count && loop) {
        frame_index_ -= frames_.Count;
      }
      else if (frame_index_ < 0 && loop) {
        frame_index_ += frames_.Count;
      }
      if (frame_index_ < frames_.Count && frame_index_ >= 0) {
        current_frame_.Deserialize(frames_[(int)frame_index_]);
        frame_index_ += speed;
      }
    }
    return current_frame_;
  }
  
    /** Deserializes all the recorded frames and returns them in a new list. */
  public List<Frame> GetFrames() {
    List<Frame> frames = new List<Frame>();
    for (int i = 0; i < frames_.Count; ++i) {
      Frame frame = new Frame();
      frame.Deserialize(frames_[i]);
      frames.Add(frame);
    }
    return frames;
  }
  
  /** The number of recorded frames. */
  public int GetFramesCount() {
    return frames_.Count;
  }
  
  /** Saves the recorded frames to a file, overwriting an existing file. */
  public string SaveToNewFile() {
    string path = Application.persistentDataPath + "/Recording_" +
                  System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".bytes";

    if (File.Exists(@path)) {
      File.Delete(@path);
    }

    FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write);
    for (int i = 0; i < frames_.Count; ++i) {
      byte[] frame_size = new byte[4];
      frame_size = System.BitConverter.GetBytes(frames_[i].Length);
      stream.Write(frame_size, 0, frame_size.Length);
      stream.Write(frames_[i], 0, frames_[i].Length);
    }
    
    stream.Close();
    return path;
  }
  
  /** Loads saved frames from a file. */
  public void Load(TextAsset text_asset) {
    byte[] data = text_asset.bytes;
    frame_index_ = 0;
    frames_.Clear();
    int i = 0;
    while (i < data.Length) {
      byte[] frame_size = new byte[4];
      Array.Copy(data, i, frame_size, 0, frame_size.Length);
      i += frame_size.Length;
      byte[] frame = new byte[System.BitConverter.ToUInt32(frame_size, 0)];
      Array.Copy(data, i, frame, 0, frame.Length);
      i += frame.Length;
      frames_.Add(frame);
    }
  }
}
