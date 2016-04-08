/// <summary>
/// Time-step independent exponential smoothing.
/// </summary>
/// <remarks>
/// When moving at a constant speed: speed * delay = Value - ExponentialSmoothing.value.
/// </remarks>
public class ExponentialSmoothing
{
  public float value = 0f; // Filtered value
  public float delay = 0f; // Mean delay
  public bool reset = true; // Reset on Next Update

  public void SetAlpha(float alpha, float deltaTime = 1f)
  {
    this.delay = deltaTime * alpha / (1f - alpha);
  }

  public float Update(float value, float deltaTime = 1f)
  {
    if (deltaTime > 0f &&
        !reset) {
      float gamma = delay / deltaTime;
      float alpha = gamma / (1f + gamma);
      // NOTE: If deltaTime -> 0 then alpha -> 1,
      // reducing the filter to this.value = value.
      this.value *= 1f - alpha;
      this.value += alpha * value;
    } else {
      this.value = value;
      reset = false;
    }
    return this.value;
  }
}
