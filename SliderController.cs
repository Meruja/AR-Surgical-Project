using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityVolumeRendering
{
    public class SliderController : MonoBehaviour
    {
        [SerializeField]
        private VolumeRenderedObject volumeRenderedObject;

        public Text valueText;
        private float smoothingValue = 0.5f;
        private Texture3D dataTexture;

        // Start is called before the first frame update
        void Start()
        {
            volumeRenderedObject = GameObject.FindGameObjectWithTag("VolumeObject").GetComponent<VolumeRenderedObject>();
            //smoothingSlider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });

        }
        public void OnSliderChanged(float value)
        {
            valueText.text = value.ToString();
            smoothingValue = value;
            ApplySmoothing();
        }
        void ApplySmoothing()
        {
            // Get the current dataset
            VolumeDataset dataset = volumeRenderedObject.dataset;
            // Get the pixel data from the dataset
            Texture3D dataTexture = dataset.GetDataTexture();
            Color[] pixels = dataTexture.GetPixels();
            // Apply filter to the pixel data with the specified smoothing value
            for (int i = 0; i < pixels.Length; i++)
            {
                float pixelValue = pixels[i].r;
                // Call Gaussian Filter
                float smoothedValue = Smooth(pixelValue, smoothingValue, dataTexture);
                pixels[i] = new Color(smoothedValue, smoothedValue, smoothedValue);
            }
            // Update the dataset with the modified pixel data
            dataTexture.SetPixels(pixels);
            dataTexture.Apply();
            // Update the volume rendered object with the modified dataset
            volumeRenderedObject.meshRenderer.sharedMaterial.SetTexture("_DataTex", dataset.GetDataTexture());
        }

        float Smooth(float value, float smoothing, Texture3D dataTexture)
        {
            // Apply a simple Gaussian smoothing function
            //float weight = Mathf.Exp(-(smoothing * smoothing));
            float smoothedValue = 0.0f;
            float totalWeight = 0.0f;

            //GetPixel method, which takes an int index for the x, y, and z axes.
            for (int i = -2; i <= 2; i++)
            {
                print(dataTexture.width);
                float u = (value + i) / (float)dataTexture.width;
                if (dataTexture != null && u >= 0f && u <= 1f)
                {
                    int xIndex = Mathf.Clamp((int)(u + i), 0, dataTexture.width - 1);
                    float neighborValue = dataTexture.GetPixel(xIndex, 0, 0).r;
                    //neighborValue = 0.1f;
                    if (neighborValue != 0)
                    {
                        float neighborWeight = Mathf.Exp(-(i * i) / (2.0f * smoothing * smoothing));
                        smoothedValue += neighborValue * neighborWeight;
                        totalWeight += neighborWeight;
                    }
                    //float neighborValue = dataTexture.GetPixelBilinear(u, 0.5f, 0.5f).r;

                }
            }
            return smoothedValue / totalWeight;
        }

    }
}

