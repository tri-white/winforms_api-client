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

            competitionAddButton.Enabled = false;
            changeModeCompetitionMethod();
            loadCompetitions();
            competitionIdNumeric_ValueChanged(null, null);

            regulationsAddButton.Enabled = false;
            changeModeRegulationMethod();
            loadRegulations();
            numericUpDown3_ValueChanged(null, null);
        }

        private void changeModeSportsman_Click(object sender, EventArgs e)
        {
            changeModeSportsmanMethod();
            clearSportsmanFields();
        }
        private void competitionChangeModeButton_Click(object sender, EventArgs e)
        {
            changeModeCompetitionMethod();
            clearCompetitionFields();
        }
        private void regulationsChangeModeButton_Click(object sender, EventArgs e)
        {
            changeModeRegulationMethod();
            clearRegulationFields();
        }

        public void changeModeSportsmanMethod()
        {
            if (addSportsmanButton.Enabled == true)
            {
                addSportsmanButton.Enabled = false;
                editSportsmanButton.Enabled = true;
                deleteSportsmanButton.Enabled = true;
                sportsmansIdNumeric.Enabled = false;
            }
            else
            {
                addSportsmanButton.Enabled = true;
                editSportsmanButton.Enabled = false;
                deleteSportsmanButton.Enabled = false;
                sportsmansIdNumeric.Enabled = true;
            }

        }
        public void changeModeCompetitionMethod()
        {
            if (competitionAddButton.Enabled == true)
            {
                competitionAddButton.Enabled = false;
                competitionEditButton.Enabled = true;
                competitionDeleteButton.Enabled = true;
                competitionIdNumeric.Enabled = false;
            }
            else
            {
                competitionAddButton.Enabled = true;
                competitionEditButton.Enabled = false;
                competitionDeleteButton.Enabled = false;
                competitionIdNumeric.Enabled = true;
            }

        }
        public void changeModeRegulationMethod()
        {
            if (regulationsAddButton.Enabled == true)
            {
                regulationsAddButton.Enabled = false;
                regulationsEditButton.Enabled = true;
                regulationsDeleteButton.Enabled = true;
                regulationsIdNumeric.Enabled = false;
            }
            else
            {
                regulationsAddButton.Enabled = true;
                regulationsEditButton.Enabled = false;
                regulationsDeleteButton.Enabled = false;
                regulationsIdNumeric.Enabled = true;
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
        public void clearCompetitionFields()
        {
            competitionNameTextbox.Text = "";
            competitionPrizepoolNumeric.Value = 0;
            competitionSportstypeCombobox.SelectedIndex = -1;
            competitionLocationTextbox.Text = "";
            competitionDatetimePicker.Value = DateTime.Now;
        }
        public void clearRegulationFields()
        {
            regulationsNameTextbox.Text = "";
            regulationsDescriptionTextbox.Text = "";
            regulationsRequirementsTextbox.Text = "";
            regulationsGenderCombobox.SelectedIndex = -1;

        }
        private void refreshSportsmanButton_ClickAsync(object sender, EventArgs e)
        {
            int page = int.Parse(currentPageSportsmans.Value.ToString());
            string search = searchSportsmanTextbox.Text.ToString();
            loadSportsmans(search,page);

        }
        private void findSportsmanButton_Click(object sender, EventArgs e)
        {
            currentPageSportsmans.Value = 1;
            refreshSportsmanButton_ClickAsync(null, null);

        }

        private void refreshCompetitionsButton_Click(object sender, EventArgs e)
        {
            int page = int.Parse(currentPageCompetitions.Value.ToString());
            string search = searchInputCompetitions.Text.ToString();
            loadCompetitions(search, page);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentPageCompetitions.Value = 1;
            refreshCompetitionsButton_Click(null, null);
        }


        private void regulationsRefreshButton_Click(object sender, EventArgs e)
        {
            int page = int.Parse(currentPageRegulationsNumeric.Value.ToString());
            string search = regulationsSearchinputTextbox.Text.ToString();
            loadRegulations(search, page);
        }

        private void regulationsSearchButton_Click(object sender, EventArgs e)
        {
            currentPageCompetitions.Value = 1;
            regulationsRefreshButton_Click(null, null);
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
                    JToken jsonResponse = JToken.Parse(responseBody);
                    JArray sportsmanArray = (JArray)jsonResponse["data"];

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
        public async void loadCompetitions(string searchKey = "", int page = 1)
        {
            try
            {
                string apiUrl = "http://127.0.0.1:8000/api/competitions?page=" + page + "&name[contains]=" + searchKey;
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JToken jsonResponse = JToken.Parse(responseBody);
                    JArray sportsmanArray = (JArray)jsonResponse["data"];

                    List<Competition> competitions = sportsmanArray.ToObject<List<Competition>>();

                    dataGridView2.DataSource = competitions;


                    int lastPage = (int)jsonResponse["meta"]["last_page"];
                    
                    currentPageCompetitions.Maximum = lastPage;
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
        public async void loadRegulations(string searchKey = "", int page = 1)
        {
            try
            {
                string apiUrl = "http://127.0.0.1:8000/api/regulations?page=" + page + "&name[contains]=" + searchKey;
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JToken jsonResponse = JToken.Parse(responseBody);
                    JArray sportsmanArray = (JArray)jsonResponse["data"];

                    List<Regulation> regulations = sportsmanArray.ToObject<List<Regulation>>();

                    dataGridView3.DataSource = regulations;


                    int lastPage = (int)jsonResponse["meta"]["last_page"];
                    currentPageRegulationsNumeric.Maximum = lastPage;
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
                    sportsmansIdNumeric.BackColor = System.Drawing.Color.White; 
                    MessageBox.Show($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                sportsmansIdNumeric.BackColor = System.Drawing.Color.White; 
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        private async void competitionIdNumeric_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(competitionIdNumeric.Value.ToString()))
                {
                    changeModeCompetitionMethod();
                    return;
                }

                int input = Convert.ToInt32(competitionIdNumeric.Value);

                string apiUrl = $"http://127.0.0.1:8000/api/competitions/{input}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    competitionIdNumeric.BackColor = System.Drawing.Color.LightGreen;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    competitionIdNumeric.BackColor = System.Drawing.Color.LightCoral;
                }
                else
                {
                    competitionIdNumeric.BackColor = System.Drawing.Color.White; 
                    MessageBox.Show($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                competitionIdNumeric.BackColor = System.Drawing.Color.White; 
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private async void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(numericUpDown3.Value.ToString()))
                {
                    changeModeSportsmanMethod();
                    return;
                }

                int input = Convert.ToInt32(numericUpDown3.Value);

                string apiUrl = $"http://127.0.0.1:8000/api/regulations/{input}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    numericUpDown3.BackColor = System.Drawing.Color.LightGreen;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    numericUpDown3.BackColor = System.Drawing.Color.LightCoral;
                }
                else
                {
                    numericUpDown3.BackColor = System.Drawing.Color.White; // Reset back color
                    MessageBox.Show($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                numericUpDown3.BackColor = System.Drawing.Color.White; // Reset back color
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

        private async void competitionIdLoadButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(competitionIdNumeric.Value.ToString()))
                {
                    MessageBox.Show("Please enter a valid competition ID.");
                    return;
                }

                int input = Convert.ToInt32(competitionIdNumeric.Value);

                string apiUrl = $"http://127.0.0.1:8000/api/competitions/{input}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject responseObject = JObject.Parse(responseBody);
                    JObject dataObject = (JObject)responseObject["data"];
                    Competition competition = dataObject.ToObject<Competition>();

                    competitionNameTextbox.Text = competition.Name;
                    competitionSportstypeCombobox.Text = competition.SportsType;
                    competitionPrizepoolNumeric.Value = int.Parse(competition.PrizePool.ToString());
                    competitionLocationTextbox.Text = competition.EventLocation;
                    competitionDatetimePicker.Text = competition.EventDate;

                    competitionAddButton.Enabled = true;
                    changeModeCompetitionMethod();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Competiton not found.");

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

        private async void regulationsLoadId_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(numericUpDown3.Value.ToString()))
                {
                    MessageBox.Show("Please enter a valid regulation ID.");
                    return;
                }

                int input = Convert.ToInt32(numericUpDown3.Value);

                string apiUrl = $"http://127.0.0.1:8000/api/regulations/{input}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject responseObject = JObject.Parse(responseBody);
                    JObject dataObject = (JObject)responseObject["data"];
                    Regulation regulation = dataObject.ToObject<Regulation>();

                    regulationsNameTextbox.Text = regulation.Name;
                    regulationsDescriptionTextbox.Text = regulation.Description;
                    regulationsGenderCombobox.Text = regulation.Gender;
                    regulationsRequirementsTextbox.Text = regulation.MinimalRequirements;


                    regulationsAddButton.Enabled = true;
                    changeModeRegulationMethod();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Regulation not found.");

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
        private async void competitionDeleteButton_Click(object sender, EventArgs e)
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

        private async void regulationsDeleteButton_Click(object sender, EventArgs e)
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
        private async void competitionAddButton_Click(object sender, EventArgs e)
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

        private async void regulationsAddButton_Click(object sender, EventArgs e)
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
        private async void competitionEditButton_Click(object sender, EventArgs e)
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

        private async void regulationsEditButton_Click(object sender, EventArgs e)
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
        private void currentPageSportsmans_ValueChanged(object sender, EventArgs e)
        {
            refreshSportsmanButton_ClickAsync(null, null);
        }

        private void currentPageRegulationsNumeric_ValueChanged(object sender, EventArgs e)
        {
            refreshCompetitionsButton_Click(null, null);
        }

        private void currentPageCompetitions_ValueChanged(object sender, EventArgs e)
        {
            regulationsRefreshButton_Click(null, null);
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
    public class Competition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EventDate { get; set; }
        public string EventLocation { get; set; }
        public string SportsType { get; set; }
        public string PrizePool { get; set; }
    }
    public class Regulation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Gender { get; set; }
        public string MinimalRequirements { get; set; }
    }
}
