using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SimpsonsVision.DataModels;
using SimpsonsVision.Model;
using Xamarin.Forms;

namespace SimpsonsVision
{
    public partial class Camera : ContentPage
    {
		double max;
		string maxTag;

        public Camera()
        {
            InitializeComponent();
        }

		private async void Load_Camera(object sender, EventArgs e)
		{
            await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				await DisplayAlert("Error", "Camera is not available", "OK");
				return;
			}

			MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
			{
				PhotoSize = PhotoSize.Medium,
				Directory = "Sample",
				Name = $"{DateTime.UtcNow}.jpg"
			});

			if (file == null)
				return;

			image.Source = ImageSource.FromStream(() =>
			{
				return file.GetStream();
			});

            TagName.Text = "";
            PredictionPercent.Text = "";
			maxTag = "Not Detected";
            max = 0.0;

            await MakePredictionRequest(file);
            await postResultAsync();
		}

		async Task postResultAsync()
		{

			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;

			var position = await locator.GetPositionAsync();

            SimpsonsVisionTable model = new SimpsonsVisionTable()
            {
                Tag = maxTag,
                Prediction = max,
                Longitude = (float)position.Longitude,
                Latitude = (float)position.Latitude
            };

			await AzureManager.AzureManagerInstance.PostSimpsonsVisionTableData(model);
		}

		static byte[] GetImageAsByteArray(MediaFile file)
		{
			var stream = file.GetStream();
			BinaryReader binaryReader = new BinaryReader(stream);
			return binaryReader.ReadBytes((int)stream.Length);
		}

		async Task MakePredictionRequest(MediaFile file)
		{

			var client = new HttpClient();

			client.DefaultRequestHeaders.Add("Prediction-Key", "a8e10e08aa534e8ab77978e142738e32");

			string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/f442813c-61b0-428d-99b9-7133dd8cda28/image";

			HttpResponseMessage response;

			byte[] byteData = GetImageAsByteArray(file);

			using (var content = new ByteArrayContent(byteData))
			{

				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				response = await client.PostAsync(url, content);


				if (response.IsSuccessStatusCode)
				{
                    int detected = 0;
                    int num = 0;

                    var responseString = await response.Content.ReadAsStringAsync();

					JObject customVision = JObject.Parse(responseString);

                    var Probability = from predict in customVision["Predictions"] select (double)predict["Probability"];
					var Tag = from predict in customVision["Predictions"] select (string)predict["Tag"];
					
                    foreach (var per in Probability)
					{
                        num += 1;
                        double prob = per * 100.0;

                        if (prob >= 10.0 && prob > max)
                        {
                            detected = num;
                            max = prob;
                        }

                        PredictionPercent.Text += String.Format("{0:0.##\\%}", prob) + "\n";
					}

                    num = 0;

					foreach (var name in Tag)
					{
                        num += 1;

                        if (detected == num)
                        {
                            maxTag = name;
                        }

                        TagName.Text += name + ": " + "\n";
					}
				}

				file.Dispose();
			}
		}
    }
}
