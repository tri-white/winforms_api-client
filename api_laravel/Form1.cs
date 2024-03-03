using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace api_laravel
{
    public class Sportsman
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Category { get; set; }
        public string Sponsor { get; set; }
    }
    public partial class Form1 : Form
    {
        private HttpClient _httpClient;
        public Form1()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        private void changeModeSportsman_Click(object sender, EventArgs e)
        {
            changeModeSportsmanMethod();
        }
        public void changeModeSportsmanMethod()
        {
            addSportsmanButton.Enabled = !addSportsmanButton.Enabled;
            editSportsmanButton.Enabled = !editSportsmanButton.Enabled;
            deleteSportsmanButton.Enabled = !deleteSportsmanButton.Enabled;
            changeModeSportsman.Enabled = !changeModeSportsman.Enabled;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
        }

        private async void refreshSportsmanButton_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                string apiUrl = "http://127.0.0.1:8000/api/sportsmans";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Parse the JSON string to get only the "data" array
                    JToken jsonResponse = JToken.Parse(responseBody);
                    JArray sportsmanArray = (JArray)jsonResponse["data"];
                   
                    // Deserialize the "data" array into a list of Sportsman objects
                    List<Sportsman> sportsmans = sportsmanArray.ToObject<List<Sportsman>>();

                    dataGridView1.DataSource = sportsmans;
                }
                else
                {
                    MessageBox.Show($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
