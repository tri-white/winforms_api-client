using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace api_laravel
{

    public partial class Form1 : Form
    {
        private HttpClient _httpClient;
        public Form1()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            addSportsmanButton.Enabled = false;
            changeModeSportsmanMethod();
            loadSportsmans();
            sportsmansIdNumeric_ValueChanged(null,null);

        }

        private void changeModeSportsman_Click(object sender, EventArgs e)
        {
            changeModeSportsmanMethod();
            clearSportsmanFields();
        }
        public void changeModeSportsmanMethod()
        {
            if (addSportsmanButton.Enabled == true)
            {
                addSportsmanButton.Enabled = false;
                editSportsmanButton.Enabled = true;
                deleteSportsmanButton.Enabled = true;
                nextPageSportsmansButton.Enabled = true;
                sportsmansIdNumeric.Enabled = false;
            }
            else
            {
                addSportsmanButton.Enabled = true;
                editSportsmanButton.Enabled = false;
                deleteSportsmanButton.Enabled = false;
                nextPageSportsmansButton.Enabled = false;
                sportsmansIdNumeric.Enabled = true;
            }

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
        }
        public void clearSportsmanFields()
        {
            sportsmanCategoryCombobox.SelectedIndex = -1;
            sportsmanGenderCombobox.SelectedIndex = -1;
            sportsmanEmailTextbox.Text = "";
            sportsmanNameTextbox.Text = "";
            sportsmanSponsorTextbox.Text = "";
        }
        private void refreshSportsmanButton_ClickAsync(object sender, EventArgs e)
        {
            int page = int.Parse(currentPageSportsmans.Value.ToString());

            loadSportsmans("",page);

            currentPageSportsmans.Value = page;
        }
        private void findSportsmanButton_Click(object sender, EventArgs e)
        {
            loadSportsmans(searchSportsmanTextbox.Text.ToString(), 1);

            currentPageSportsmans.Value = 1;

        }
        public async void loadSportsmans(string searchKey="", int page=1)
        {
            try
            {
                string apiUrl = "http://127.0.0.1:8000/api/sportsmans?page=" + page+"&name[contains]="+searchKey;
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


                    int lastPage = (int)jsonResponse["meta"]["last_page"];
                    currentPageSportsmans.Maximum = lastPage;
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

        private async void sportsmansIdNumeric_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(sportsmansIdNumeric.Value.ToString()))
                {
                    changeModeSportsmanMethod();
                    return;
                }

                int input = Convert.ToInt32(sportsmansIdNumeric.Value);

                string apiUrl = $"http://127.0.0.1:8000/api/sportsmans/{input}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    sportsmansIdNumeric.BackColor = System.Drawing.Color.LightGreen;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    sportsmansIdNumeric.BackColor = System.Drawing.Color.LightCoral;
                }
                else
                {
                    sportsmansIdNumeric.BackColor = System.Drawing.Color.White; // Reset back color
                    MessageBox.Show($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                sportsmansIdNumeric.BackColor = System.Drawing.Color.White; // Reset back color
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(sportsmansIdNumeric.Value.ToString()))
                {
                    MessageBox.Show("Please enter a valid sportsman ID.");
                    return;
                }

                int input = Convert.ToInt32(sportsmansIdNumeric.Value);

                string apiUrl = $"http://127.0.0.1:8000/api/sportsmans/{input}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject responseObject = JObject.Parse(responseBody);
                    JObject dataObject = (JObject)responseObject["data"];
                    Sportsman sportsman = dataObject.ToObject<Sportsman>();

                    // Assuming you have text boxes for each field, you can set their text values
                    sportsmanNameTextbox.Text = sportsman.Name;
                    sportsmanEmailTextbox.Text = sportsman.Email;
                    sportsmanGenderCombobox.Text = sportsman.Gender;
                    sportsmanCategoryCombobox.Text = sportsman.Category;
                    sportsmanSponsorTextbox.Text = sportsman.Sponsor;


                    addSportsmanButton.Enabled = true;
                    changeModeSportsmanMethod();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Sportsman not found.");
                    
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

        private async void deleteSportsmanButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(sportsmansIdNumeric.Value.ToString()))
                {
                    MessageBox.Show("Please enter a valid sportsman ID.");
                    return;
                }

                int sportsmanId = Convert.ToInt32(sportsmansIdNumeric.Value);
                string apiUrl = $"http://localhost:8000/api/sportsmans/{sportsmanId}";

                HttpResponseMessage response = await _httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Sportsman deleted successfully.");

                    sportsmansIdNumeric_ValueChanged(null, null);
                    changeModeSportsmanMethod();
                    findSportsmanButton_Click(null, null);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Sportsman not found.");
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

        private async void addSportsmanButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sportsmanNameTextbox.Text) ||
                     string.IsNullOrWhiteSpace(sportsmanGenderCombobox.Text) ||
                     string.IsNullOrWhiteSpace(sportsmanCategoryCombobox.Text))
                {
                    MessageBox.Show("Please fill in all required fields (Name, Gender, Category).");
                    return;
                }

                Sportsman newSportsman = new Sportsman
                {
                    Name = sportsmanNameTextbox.Text,
                    Gender = sportsmanGenderCombobox.Text,
                    Category = sportsmanCategoryCombobox.Text
                };

                if (!string.IsNullOrWhiteSpace(sportsmanEmailTextbox.Text))
                {
                    newSportsman.Email = sportsmanEmailTextbox.Text;
                }

                if (!string.IsNullOrWhiteSpace(sportsmanSponsorTextbox.Text))
                {
                    newSportsman.Sponsor = sportsmanSponsorTextbox.Text;
                }

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                };

                var jsonBody = JsonConvert.SerializeObject(new
                {
                    newSportsman.Name,
                    newSportsman.Email,
                    newSportsman.Gender,
                    newSportsman.Category,
                    newSportsman.Sponsor
                }, settings);

                HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("http://localhost:8000/api/sportsmans", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("New sportsman created successfully.");

                    searchSportsmanTextbox.Text = newSportsman.Name;
                    findSportsmanButton_Click(null,null);
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

        private async void editSportsmanButton_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedSportsmanId = int.Parse(sportsmansIdNumeric.Value.ToString());

                Sportsman updatedSportsman = new Sportsman
                {
                    Name = sportsmanNameTextbox.Text,
                    Gender = sportsmanGenderCombobox.Text,
                    Category = sportsmanCategoryCombobox.Text
                };

                if (!string.IsNullOrWhiteSpace(sportsmanEmailTextbox.Text))
                {
                    updatedSportsman.Email = sportsmanEmailTextbox.Text;
                }

                if (!string.IsNullOrWhiteSpace(sportsmanSponsorTextbox.Text))
                {
                    updatedSportsman.Sponsor = sportsmanSponsorTextbox.Text;
                }

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                };

                var jsonBody = JsonConvert.SerializeObject(new
                {
                    updatedSportsman.Name,
                    updatedSportsman.Email,
                    updatedSportsman.Gender,
                    updatedSportsman.Category,
                    updatedSportsman.Sponsor
                }, settings);

                HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                string apiUrl = $"http://localhost:8000/api/sportsmans/{selectedSportsmanId}";

                HttpResponseMessage response = await _httpClient.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Sportsman updated successfully.");

                    searchSportsmanTextbox.Text = updatedSportsman.Name;
                    findSportsmanButton_Click(null, null);
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
    public class Sportsman
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Category { get; set; }
        public string Sponsor { get; set; }
    }
}
