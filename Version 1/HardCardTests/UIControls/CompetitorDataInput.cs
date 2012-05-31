using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Collections;
using Hardcard.Scoring;
using EventProject;
using System.Collections.Specialized;

namespace UIControls
{
    public partial class CompetitorDataInput : UserControl
    {
        /// <summary>
        /// EnterButtonPressedEvent should be used by components that are interested in
        /// capturing when the user hits the 'enter' key.
        /// </summary>
        public delegate void EnterButtonPressedHandler();
        public event EnterButtonPressedHandler EnterButtonPressedEvent;

        private Hashtable zipHash;

        public CompetitorDataInput()
        {
            InitializeComponent();
            LoadZipCodes("zips.txt");
            AddEnterKeyHandlerToControls();
            genderComboBox.SelectedIndex = 0;//default value

            dobSelector.LostFocus += new EventHandler(dobSelector_LostFocus);
        }

        void dobSelector_LostFocus(object sender, EventArgs e)
        {
            //debug
        }

        private void ageTextBox_TextChanged(object sender, EventArgs e)
        {
            //set RED text color if input value doesn't make sense
            ageTextBox.ForeColor = Color.Black;
            try
            {
                int value = int.Parse(ageTextBox.Text);
                if (value <= 0 || value > 120)
                    ageTextBox.ForeColor = Color.Red;
            }
            catch
            {
                ageTextBox.ForeColor = Color.Red;
            }
        }

        private void zipTextBox_TextChanged(object sender, EventArgs e)
        {
            //set RED text color if input value doesn't make sense
            zipTextBox.ForeColor = Color.Black;
            try
            {
                int value = int.Parse(zipTextBox.Text);
                if (value < 0)
                    zipTextBox.ForeColor = Color.Red;
            }
            catch
            {
                zipTextBox.ForeColor = Color.Red;
                return;
            }

            //lookup location based on a zip code
            String result = zipHash[zipTextBox.Text] as String;
            if (result != null)
            {
                String[] stateCity = result.Split(new char[] { ',' });
                cityTextBox.Text = stateCity[1];
                stateComboBox.Text = stateCity[0];
            }
        }

        private void zipTextBoxAdd_TextChanged(object sender, EventArgs e)
        {
            //set RED text color if input value doesn't make sense
            zipTextBoxAdd.ForeColor = Color.Black;
            try
            {
                int value = int.Parse(zipTextBoxAdd.Text);
                if (value < 0)
                    zipTextBoxAdd.ForeColor = Color.Red;
            }
            catch
            {
                zipTextBoxAdd.ForeColor = Color.Red;
            }
        }

        private void dobSelector_ValueChanged(object sender, EventArgs e)
        {
            //calculate age
            try
            {
                DateTime birthDate = dobSelector.Value;
                DateTime currentDate = DateTime.Now;
                TimeSpan span = currentDate - birthDate;
                int age = currentDate.Year - birthDate.Year;
                if (currentDate.Month < birthDate.Month || (currentDate.Month == birthDate.Month && currentDate.Day < birthDate.Day))
                {
                    age--;
                }
                ageTextBox.Text = "" + age;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception Caught!" + exc.StackTrace);
                DataManager.Log("Exception Caught!" + exc.StackTrace);
            }
        }


        private void LoadZipCodes(String filename)
        {
            try
            {
                zipHash = new Hashtable();
                StreamReader sr = new StreamReader(filename);
                String line = "";

                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Replace("\"", "");
                    String[] result = line.Split(new char[] { ',' });
                    if (zipHash[result[1]] == null)
                        zipHash.Add(result[1], result[2] + "," + result[3]);
                }
                sr.Close();
            }
            catch (Exception e) //argument exception - adding same key twice
            {
                Console.WriteLine("Exception caught!" + e.StackTrace);
                DataManager.Log("Exception Caught!" + e.StackTrace);
            }
        }

        /// <summary>
        /// Clear text and any selections for all the controls in the UI.
        /// May be helpful when all the information has been entered and 
        /// new user's data should be started to be added.
        /// </summary>
        public void ClearControls()
        {
            lastNameTextBox.Text = "";
            firstNameTextBox.Text = "";
            addressLineTextBox.Text = "";
            cityTextBox.Text = "";
            zipTextBox.Text = "";
            zipTextBoxAdd.Text = "";
            phoneTextBox.Text = "";
            dobSelector.Value = DateTime.Now;
            ageTextBox.Text = "";
            genderComboBox.SelectedIndex = 0;//-1;
            sponsorsTextBox.Text = "";
            bikeBrandTextBox.Text = "";
            bikeNumberTextBox.Text = "";
            bikeTagTextBox.Text = "";
            bikeTag2TextBox.Text = "";
            stateComboBox.SelectedIndex = -1;
            syncChangesCheckBox.Checked = true;//false;

            //set focus to the first element: lastNameTextBox
            lastNameTextBox.Focus();
        }

        /// <summary>
        /// This method is necessary to pass key events to a parent control
        /// from the children UI elements. Unfortunately, there seems not to
        /// be a property or method that can perform this, thus we
        /// have to do it manually.
        /// </summary>
        private void AddEnterKeyHandlerToControls()
        {
            lastNameTextBox.KeyDown += new KeyEventHandler(keyDownHanler);
            firstNameTextBox.KeyDown += new KeyEventHandler(keyDownHanler);
            addressLineTextBox.KeyDown += new KeyEventHandler(keyDownHanler);
            cityTextBox.KeyDown += new KeyEventHandler(keyDownHanler);
            zipTextBox.KeyDown += new KeyEventHandler(keyDownHanler);
            zipTextBoxAdd.KeyDown += new KeyEventHandler(keyDownHanler);
            phoneTextBox.KeyDown += new KeyEventHandler(keyDownHanler);
            dobSelector.KeyDown += new KeyEventHandler(keyDownHanler);
            ageTextBox.KeyDown += new KeyEventHandler(keyDownHanler);
            genderComboBox.KeyDown += new KeyEventHandler(keyDownHanler);
            sponsorsTextBox.KeyDown += new KeyEventHandler(keyDownHanler);
            bikeBrandTextBox.KeyDown += new KeyEventHandler(keyDownHanler);
            bikeNumberTextBox.KeyDown += new KeyEventHandler(keyDownHanler);
            bikeTagTextBox.KeyDown += new KeyEventHandler(keyDownHanler);
            bikeTag2TextBox.KeyDown += new KeyEventHandler(keyDownHanler);
            stateComboBox.KeyDown += new KeyEventHandler(keyDownHanler);
        }

        private void keyDownHanler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FireEnterKeyPressedEvent();
            }
        }

        public void FireEnterKeyPressedEvent()
        {
            if (EnterButtonPressedEvent == null) return;

            EnterButtonPressedEvent();
        }

        private void SaveData(String filename)
        {
            //save input data to a file
            //StreamWriter sw = new StreamWriter(filename);
            //
        }

        /// <summary>
        /// Update the competitor with the values from the UI elements.
        /// Please note that competitor parameter is a reference parameter.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>True if the update was successfull, false otherwise.</returns>
        public bool UpdateCompetitorData(ref Competitor c)
        {
            int id;
            String lastName;
            String firstName;
            Address address;
            PhoneNumber phone;
            DateTime dob;
            int age;
            bool gender;
            String sponsors;
            String bikeBrand;
            String bikeNumber;
            TagId tagNumber;
            TagId tagNumber2;
            String errorMessage;
            bool propagateChanges;

            bool dataResult = this.GetData(out id, out lastName, out firstName, out address, out phone, out dob,
                out age, out gender, out sponsors, out bikeBrand, out bikeNumber, out tagNumber, out tagNumber2, out errorMessage, out propagateChanges);
            if (!dataResult)
            {
                return false;
            }
            c.propagateChanges = propagateChanges;
            c.LastName = lastName;
            c.FirstName = firstName;
            c.Address = address;
            c.Phone = phone;
            c.DOB = dob;
            c.Age = age;
            c.Gender = gender;
            c.Sponsors = sponsors;
            c.BikeBrand = bikeBrand;
            c.BikeNumber = bikeNumber;
            c.TagNumber = tagNumber;
            c.TagNumber2 = tagNumber2;

            return true;
        }

        /// <summary>
        /// Returns all the values from the UI elements.
        /// </summary>
        /// <returns>Returns false if there was a problem retrieving data from the gui components.</returns>
        public bool GetData(
            out int ID, out String lastName, out String firstName, out Address address,
            out PhoneNumber phoneNumber, out DateTime dob, out int age, out bool gender, out String sponsors,
            out String bikeBrand, out String bikeNumber, out TagId tagNumber, out TagId tagNumber2, out String errorMessage, out bool propagateChanges)
        {
            errorMessage = "";
            String errorControlString = "";//changed to inform where was a problem with UI
            try
            {
                //ID = 0;//debug
                propagateChanges = syncChangesCheckBox.Checked;
                ID = DataManager.getNextID();
                lastName = lastNameTextBox.Text;
                firstName = firstNameTextBox.Text;
                errorControlString = "ZIP";                
                int zipInt = 0;
                try 
                {
                    zipInt = int.Parse(zipTextBox.Text + zipTextBoxAdd.Text);
                }
                catch {}
                address = new Address(addressLineTextBox.Text, cityTextBox.Text, stateComboBox.Text, zipInt);
                errorControlString = "Phone number";
                phoneNumber = new PhoneNumber(phoneTextBox.Text);
                dob = dobSelector.Value;
                errorControlString = "Age";
                age = 20;
                try
                {
                    age = int.Parse(ageTextBox.Text);
                }
                catch {}
                errorControlString = "Gender";
                gender = (genderComboBox.Text == "Male") ? true : false;
                if (genderComboBox.SelectedIndex == -1)
                    throw new Exception("Gender not selected!");
                sponsors = sponsorsTextBox.Text;
                bikeBrand = bikeBrandTextBox.Text;
                errorControlString = "Bike Number";
                //bikeNumber = "0";
                bikeNumber = bikeNumberTextBox.Text;
                //try
                //{
                //    bikeNumber = int.Parse(bikeNumberTextBox.Text);
                //}
                //catch { }
                tagNumber = new TagId(bikeTagTextBox.Text);
                tagNumber2 = new TagId(bikeTag2TextBox.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught!" + e.StackTrace);
                DataManager.Log("Exception Caught!" + e.StackTrace);

                //populate with some default values; out keyword enforces us 
                //to put at least something in these values

                propagateChanges = false;
                ID = 0;
                lastName = "";
                firstName = "";
                address = new Address("", "", "", 12345);
                phoneNumber = new PhoneNumber("");
                dob = DateTime.Now;
                age = 20;
                gender = true;
                sponsors = "";
                bikeBrand = "";
                bikeNumber = "0";
                tagNumber = new TagId();
                tagNumber2 = new TagId();

                errorMessage = "Sorry, there was a problem with field " + errorControlString + ".";

                return false;
            }

            return true;
        }

        /// <summary>
        /// Populate the UI controls with the given data. Please note that there is no
        /// error checking, we assume that the data is properly formatted, etc.
        /// </summary>
        public void SetData(String lastName, String firstName, Address address,
            PhoneNumber phoneNumber, DateTime dob, int age, bool gender, String sponsors,
            String bikeBrand, String bikeNumber, TagId tagID, TagId tagID2)
        {
            lastNameTextBox.Text = lastName;
            firstNameTextBox.Text = firstName;
            addressLineTextBox.Text = address.AddressLine;
            cityTextBox.Text = address.City;
            stateComboBox.SelectedValue = address.State;
            if ((address.Zip + "").Length == 5)
                zipTextBox.Text = address.Zip + "";
            else
            {
                if ((address.Zip + "").Length >= 5)
                {
                    zipTextBox.Text = (address.Zip + "").Substring(0, 5);
                    zipTextBoxAdd.Text = (address.Zip + "").Substring(5);
                }
            }
            phoneTextBox.Text = phoneNumber.Number;
            dobSelector.Value = dob;
            ageTextBox.Text = age + "";
            if (gender)
                genderComboBox.SelectedIndex = 0;//male
            else
                genderComboBox.SelectedIndex = 1;

            sponsorsTextBox.Text = sponsors;
            bikeBrandTextBox.Text = bikeBrand;
            bikeNumberTextBox.Text = bikeNumber + "";
            bikeTagTextBox.Text = tagID + "";
            bikeTag2TextBox.Text = tagID2 + "";
        }

        /// <summary>
        /// Populate the UI controls with the given competitor's data. Please note that there is no
        /// error checking, we assume that the data is properly formatted, etc.
        /// </summary>        
        public void SetData(Competitor c)
        {
            syncChangesCheckBox.Checked = c.propagateChanges;
            this.SetData(c.LastName, c.FirstName, c.Address, c.Phone, c.DOB,
                    c.Age, c.Gender, c.Sponsors, c.BikeBrand, c.BikeNumber, c.TagNumber, c.TagNumber2);
        }

        /// <summary>
        /// This method checks whether any of the data fields have different information
        /// than the one in the passed competitor object.
        /// </summary>
        /// <param name="c">Competitor object to compare with.</param>
        public bool ChangesMade(Competitor c)
        {
            int id;
            String lastName;
            String firstName;
            Address address;
            PhoneNumber phone;
            DateTime dob;
            int age;
            bool gender;
            String sponsors;
            String bikeBrand;
            String bikeNumber;
            TagId tagNumber;
            TagId tagNumber2;
            bool propagateChanges;

            String errorMessage;

            bool result = this.GetData(out id, out lastName, out firstName, out address, out phone, out dob,
                out age, out gender, out sponsors, out bikeBrand, out bikeNumber, out tagNumber, out tagNumber2, out errorMessage, out propagateChanges);

            if (!lastName.Equals(c.LastName) || !firstName.Equals(c.FirstName) || !address.Equals(c.Address) || !phone.Equals(c.Phone) ||
                !dob.Equals(c.DOB) || age != c.Age || gender != c.Gender || !sponsors.Equals(c.Sponsors) || !bikeBrand.Equals(c.BikeBrand) ||
                !bikeNumber.Equals(c.BikeNumber) || !tagNumber.Equals(c.TagNumber) || !tagNumber2.Equals(c.TagNumber2))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method forces refresh to auto-complete list that is used in last name text box.
        /// The auto-complete list picks up the values from Competitors List in DataManager.
        /// </summary>
        public void RefreshAutoCompleteList()
        {
            lastNameTextBox.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            foreach (Competitor c in DataManager.Instance.Competitors)
            {
                lastNameTextBox.AutoCompleteCustomSource.Add(c.LastName);
            }
        }

        public void SetUseLastNameAutoComplete(bool use)
        {
            if (!use)
                lastNameTextBox.AutoCompleteMode = AutoCompleteMode.None;
            else
                lastNameTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;

            RefreshAutoCompleteList();

            if(use)
                lastNameTextBox.TextChanged += lastNameTextBox_TextChanged;
        }

        private void lastNameTextBox_TextChanged(object sender, EventArgs e)
        {
            //fill the fields with the given competitor info
            if (lastNameTextBox.AutoCompleteCustomSource.Contains(lastNameTextBox.Text))
            {
                List<Competitor> result = DataManager.Instance.GetCompetitorByLastName(lastNameTextBox.Text);
                if (result.Count > 0)
                {
                    //don't reset already set up value in the TagID and TagID2 boxes
                    String oldTagID = bikeTagTextBox.Text;
                    String oldTagID2 = bikeTag2TextBox.Text;

                    Competitor c = result[0];
                    this.SetData(c);

                    bikeTagTextBox.Text = oldTagID;
                    bikeTag2TextBox.Text = oldTagID2;
                }
            }
        }

    }
}
