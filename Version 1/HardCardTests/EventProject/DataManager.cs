using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hardcard.Scoring;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data.OleDb;
using System.Data;
using System.Collections.Specialized;
using LumenWorks.Framework.IO.Csv;
using System.IO.IsolatedStorage;
using System.Security.AccessControl;

namespace EventProject
{
    public class DataManager
    {        
        private static DataManager instance;
        //const string ISOLATED_FILE_NAME = "Cryp.hcd";
        const string tagDirectory = "C:\\Hardcard\\tags";
        const string keyFileName = "keyFile.hcd";

        private DataManager()
        {
            events = new BindingList<Event>();
            //competitors = new BindingList<Competitor>();
            competitors = new BindingListWithSort<Competitor>();
            classes = new BindingList<Class>();
            eventEntries = new BindingList<EventEntry>();
            validTags = new Dictionary<String, object>();

            //LoadTestData();

            DeserializeData();
        }

        public static DataManager Instance
        {
            get 
            {
                if (instance == null)
                    instance = new DataManager();
                return instance;
            }
        }

        //let's keep first few values for hardcoded debug cases
        private static int uniqueIDGenerator = 20;
        
        /// <summary>
        /// Returns unique integer ids.
        /// </summary>
        /// <returns></returns>
        public static int getNextID()
        {
            return uniqueIDGenerator++;
        }

        private BindingList<Event> events;
        public BindingList<Event> Events
        {
            get { return events; }
            set { events = value; }
        }

        //private BindingList<Competitor> competitors;
        //public BindingList<Competitor> Competitors
        //{
        //    get { return competitors; }
        //    set { competitors = value; }
        //}
        private BindingListWithSort<Competitor> competitors;
        public BindingListWithSort<Competitor> Competitors
        {
            get { return competitors; }
            set { competitors = value; }
        }

        private BindingList<Class> classes;
        public BindingList<Class> Classes
        {
            get { return classes; }
            set { classes = value; }
        }

        private BindingList<EventEntry> eventEntries;
        public BindingList<EventEntry> EventEntries
        {
            get { return eventEntries; }
            set { eventEntries = value; }
        }

        private Dictionary<String, Object> validTags;
        public Dictionary<String, Object> ValidTags
        {
            get { return validTags; }
            set { validTags = value; }
        }

        //to sync selections in main screen with race standings screen
        public int EventSelectedIndex = -1;
        public int SessionSelectedIndex = -1;
        public int MinimumLapTime = 30;//in seconds

        /// <summary>
        /// This method finds a competitor object with a unique id.
        /// </summary>
        /// <param name="ID">The unique identifier of the competitor.</param>
        /// <returns>Competitor with the given id, or null if not found.</returns>
        public Competitor GetCompetitorByID(int ID)
        {
            if(competitors == null) return null;

            foreach (Competitor c in competitors)
            {
                if (c.ID == ID)
                    return c;
            }

            return null;
        }

        public EventEntry GetEventEntryByID(int ID)
        {
            if (eventEntries == null) return null;

            foreach (EventEntry eve in eventEntries)
            {
                if (eve.ID == ID)
                    return eve;
            }

            return null;
        }

        /// <summary>
        /// Find a competitor with the given tagID.
        /// Please note that both tagID and tagID2 of the competitor object
        /// are checked.
        /// </summary>
        /// <param name="tagID"></param>
        /// <returns>Competitor with the given tag id, or null if not found.</returns>
        public Competitor GetCompetitorByTagID(TagId tagID)
        {
            if (competitors == null) return null;

            foreach(Competitor c in competitors)
            {
                if (c.TagNumber.Equals(tagID) || c.TagNumber2.Equals(tagID))
                    return c;
            }
            return null;
        }


        /// <summary>
        /// Find a competitor (or a list of competitors) with the given 
        /// first and last names.
        /// </summary>
        /// <returns>
        /// A list of competitors with the given first/last names.
        /// If no competitors found, empty list is returned.
        /// </returns>
        public List<Competitor> GetCompetitorByName(String firstName, String lastName)
        {
            List<Competitor> result = new List<Competitor>();

            foreach (Competitor c in competitors)
            {
                if (c.FirstName.Equals(firstName) && c.LastName.Equals(lastName))
                    result.Add(c);
            }

            return result;
        }

        public List<Competitor> GetCompetitorByLastName(String lastName)
        {
            List<Competitor> result = new List<Competitor>();

            foreach (Competitor c in competitors)
            {
                if (c.LastName.Equals(lastName))
                    result.Add(c);
            }

            return result;
        }

        private void LoadTestData()
        {
            BindingList<DateTime> event1Dates = new BindingList<DateTime>();
            event1Dates.Add(new DateTime(2011, 02, 15));
            event1Dates.Add(new DateTime(2011, 02, 16));
            Event event1 = new Event()
            {
                ID = 1,
                name = "OSU Race February",
                city = "Columbus",
                state = "OH",
                dates = event1Dates
            };

            BindingList<DateTime> event2Dates = new BindingList<DateTime>();
            event2Dates.Add(new DateTime(2011, 03, 20));
            event2Dates.Add(new DateTime(2011, 03, 21));
            Event event2 = new Event()
            {
                ID = 2,
                name = "OSU Race March",
                city = "Columbus",
                state = "OH",
                dates = event2Dates
            };

            events.Add(event1);
            events.Add(event2);
            
            Competitor cmp = new Competitor()
            {
                ID = 1,
                LastName = "Doe",
                FirstName = "John",
                Address = new Address("OSU", "Columbus", "OH", 43201),
                Phone = new PhoneNumber("123-456-7890"),
                DOB = DateTime.Now,
                Age = 29,
                Gender = true,
                Sponsors = "Honda",
                BikeBrand = "Honda",
                BikeNumber = "33",
                TagNumber = new TagId("0343")
            };

            Competitor cmp2 = new Competitor()
            {
                ID = 2,
                LastName = "Smith",
                FirstName = "Mike",
                Address = new Address("OSU", "Columbus", "OH", 43201),
                Phone = new PhoneNumber("123-456-7890"),
                DOB = DateTime.Now,
                Age = 40,
                Gender = true,
                Sponsors = "Suzuki",
                BikeBrand = "Suzuki",
                BikeNumber = "37",
                TagNumber = new TagId("0365")
            };            
            
            competitors.Add(cmp);
            competitors.Add(cmp2);

            event1.competitors.Add(cmp);
            event2.competitors.Add(cmp);

            event1.competitors.Add(cmp2);
            event2.competitors.Add(cmp2);

            Class class125cc = new Class();
            class125cc.name = "125cc";
            class125cc.description = "125cc";

            Class class250cc = new Class();
            class250cc.name = "250cc";
            class250cc.description = "250cc";

            classes.Add(class125cc);
            classes.Add(class250cc);

            Race event1Race1 = new Race("qualify race 1", "Qualifying");
            Race event1Race2 = new Race("qualify race 2", "Qualifying");
            Race event1Race3 = new Race("final race", "Race");

            Race event2Race1 = new Race("qualify race 1", "Qualifying");
            Race event2Race2 = new Race("qualify race 2", "Qualifying");
            Race event2Race3 = new Race("final race", "Race");

            event1.races = new BindingList<Race>() { event1Race1, event1Race2, event1Race3 };
            event2.races = new BindingList<Race>() { event2Race1, event2Race2, event2Race3 };

            EventEntry eve1 = new EventEntry();
            eve1.competitor = cmp;
            eve1.eventID = event1.ID;
            eve1.className = "250cc";

            EventEntry eve2 = new EventEntry();
            eve2.competitor = cmp2;
            eve2.eventID = event1.ID;
            eve2.className = "250cc";

            eventEntries.Add(eve1);
            eventEntries.Add(eve2);

            LoadTagFile("");
        }

        public void SerializeData()
        {
            try
            {
                string appPath = Path.GetDirectoryName(Config.Instance.DataDirectory);
                Directory.SetCurrentDirectory(appPath);

                //try
                //{
                //    File.Delete("competitors_data.bin");
                //    File.Delete("events_data.bin");
                //    File.Delete("classes_data.bin");
                //    File.Delete("event_entries_data.bin");
                //}
                //catch (Exception exception)
                //{
                //    Console.WriteLine(exception.StackTrace);
                //}

                using (Stream stream = File.Open("classes_data.bin", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, classes);
                }
                using (Stream stream = File.Open("competitors_data.bin", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, competitors);
                }
                using (Stream stream = File.Open("event_entries_data.bin", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, eventEntries);
                }
                using (Stream stream = File.Open("events_data.bin", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, events);
                }
                using (Stream stream = File.Open("other_data.bin", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    List<int> indexes = new List<int>();
                    indexes.Add(EventSelectedIndex);
                    indexes.Add(SessionSelectedIndex);
                    indexes.Add(MinimumLapTime);
                    bin.Serialize(stream, indexes);
                }
                SerializeTags();

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught!" + e.StackTrace);
                DataManager.Log("Exception caught!" + e.StackTrace);
            }
        }
        private void SerializeTags()
        {
            Directory.SetCurrentDirectory(tagDirectory);
            //to be able to overwrite the file, set it attributes as "normal" first
            if (File.Exists(tagDirectory + "\\tags.encrypted.txt"))
                File.SetAttributes(tagDirectory + "\\tags.encrypted.txt", FileAttributes.Normal);
            if (File.Exists(tagDirectory + "\\key.bin"))
                File.SetAttributes(tagDirectory + "\\key.bin", FileAttributes.Normal);

            FileStream fsFileOut = File.Create("tags.encrypted.txt");
            TripleDESCryptoServiceProvider cryptAlgorithm = new TripleDESCryptoServiceProvider();
            cryptAlgorithm.Key = getKey(); // new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
            cryptAlgorithm.IV = getIV(); // new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
            //if (Properties.Settings.Default.cryptKey == null || Properties.Settings.Default.cryptIV == null)
            //    return;

            //cryptAlgorithm.Key = encoding.GetBytes(Properties.Settings.Default.cryptKey);
            //cryptAlgorithm.IV = encoding.GetBytes(Properties.Settings.Default.cryptIV);

            CryptoStream csEncrypt = new CryptoStream(fsFileOut, cryptAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
            StreamWriter swEncStream = new StreamWriter(csEncrypt);
            if (validTags != null)
            {
                foreach (String key in validTags.Keys)
                {
                    swEncStream.WriteLine(key + "," + validTags[key]);
                    //Console.WriteLine(key + "," + validTags[key]);
                }
            }
            swEncStream.Flush();
            swEncStream.Close();

            // Create the key file
            FileStream fsFileKey = File.Create("key.bin");
            BinaryWriter bwFile = new BinaryWriter(fsFileKey);
            bwFile.Write(cryptAlgorithm.Key);
            bwFile.Write(cryptAlgorithm.IV);
            bwFile.Flush();
            bwFile.Close();

            //reset back
            File.SetAttributes(tagDirectory + "\\tags.encrypted.txt", File.GetAttributes(tagDirectory + "\\tags.encrypted.txt") | FileAttributes.Hidden);
            File.SetAttributes(tagDirectory + "\\key.bin", File.GetAttributes(tagDirectory + "\\key.bin") | FileAttributes.Hidden);

        }

        private byte[] getIV()
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes("Hardcard");
        }

        private byte[] getKey()
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes("2011RACHardcardZZY992134");
        }

        public void DeserializeData()
        {
            //check number of entries and set the UniqueNumberGenerator that exceeds that (so we don't use
            //same ids)            
            string appPath = Path.GetDirectoryName(Config.Instance.DataDirectory);
            Directory.SetCurrentDirectory(appPath);
            try
            {
                using (Stream stream = File.Open("classes_data.bin", FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();

                    var deserializedClasses = (BindingList<Class>)bin.Deserialize(stream);
                    this.classes = deserializedClasses;
                    stream.Close();
                }
                using (Stream stream = File.Open("competitors_data.bin", FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();

                    //var deserializedCompetitors = (BindingList<Competitor>)bin.Deserialize(stream);
                    var deserializedCompetitors = (BindingListWithSort<Competitor>)bin.Deserialize(stream);
                    this.competitors = deserializedCompetitors;
                    stream.Close();                    
                }
                using (Stream stream = File.Open("event_entries_data.bin", FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();

                    var deserializedEventEntries = (BindingList<EventEntry>)bin.Deserialize(stream);
                    this.eventEntries = deserializedEventEntries;

                    //have to manually reset correct competitors - doesn't work otherwise for some reason
                    foreach (EventEntry eve in eventEntries)
                    {
                        int cID = eve.competitorID;
                        Competitor c = GetCompetitorByID(cID);
                        if (c != null)
                        {
                            eve.competitor = c;
                            //make sure event entries and events and competitors properly connected;
                            //need to set up proper relationships, not only the above one

                            //probably go through each race, check corresponding races/event entries etc.
                        }
                    }
                    stream.Close();
                }
                using (Stream stream = File.Open("other_data.bin", FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();

                    var deserializedEvents = (List<int>)bin.Deserialize(stream);
                    List<int> indexes = deserializedEvents;
                    EventSelectedIndex = indexes[0];
                    SessionSelectedIndex = indexes[1];
                    MinimumLapTime = indexes[2];
                    stream.Close();
                }
                using (Stream stream = File.Open("events_data.bin", FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();

                    var deserializedEvents = (BindingList<Event>)bin.Deserialize(stream);
                    this.events = deserializedEvents;
                    stream.Close();
                }
                
                //as with event entries, have to reset competitors for competitor races
                foreach (Event ev in events)
                {
                    foreach (Race race in ev.races)
                    {
                        foreach (CompetitorRace cr in race.competitorRaceList)
                        {
                            int cID = cr.competitorID;
                            Competitor c = GetCompetitorByID(cID);
                            if (c != null)
                                cr.competitor = c;

                            //do reset for the EventEntries as well
                            //deserialized evententry in the CR is not the correct one;
                            //find and set the correct one (even if it's null)
                            if (cr.EventEntry != null)
                            {
                                EventEntry relatedEventEntry = GetEventEntryByID(cr.EventEntry.ID);
                                cr.EventEntry = relatedEventEntry;
                            }
                        }
                    }
                }

                LoadEncrypedTags();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught!" + e.StackTrace);
                DataManager.Log("Exception caught!" + e.StackTrace);
            }

            uniqueIDGenerator = GetCurrentMaxID();
        }

        private void LoadEncrypedTags()
        {
        //    IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
        //    IsolatedStorageFile.GetMachineStoreForApplication();
        //    string[] fileNames = isoStore.GetFileNames(ISOLATED_FILE_NAME);
        //    bool fileExists = false;
        //    foreach (string file in fileNames)
        //    {
        //        if (file == ISOLATED_FILE_NAME)
        //        {
        //            fileExists = true;
        //            break;
        //        }
        //    }
        //    string keyString = String.Empty;
        //    if (fileExists)
        //    {
        //        using (IsolatedStorageFileStream iStream = new IsolatedStorageFileStream(ISOLATED_FILE_NAME, FileMode.Open, isoStore))
        //        {
        //            StreamReader reader = new StreamReader(iStream);
        //            keyString = reader.ReadLine();
        //        }
        //    }
            if (!Directory.Exists(tagDirectory))
            {
                //DirectorySecurity directorySecurity = new DirectorySecurity();
                Directory.CreateDirectory(tagDirectory);
            }
            //See if directory has hidden flag, if not, make hidden
            DirectoryInfo di = new DirectoryInfo(tagDirectory);
            if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            {
                //Add Hidden flag    
                di.Attributes |= FileAttributes.Hidden;
            }
            Directory.SetCurrentDirectory(tagDirectory);

            string keyString = String.Empty;
            if (File.Exists(keyFileName))
            {
                StreamReader keyFile = new StreamReader(keyFileName);
                keyString = keyFile.ReadLine();
            }

            if (File.Exists("tags.encrypted.txt") && File.Exists("key.bin"))
            {
                FileStream fsFileIn = File.OpenRead("tags.encrypted.txt");
                FileStream fsKeyFile = File.OpenRead("key.bin");

                TripleDESCryptoServiceProvider cryptAlgorithm = new TripleDESCryptoServiceProvider();
                BinaryReader brFile = new BinaryReader(fsKeyFile);

                byte[] algKey = brFile.ReadBytes(24);

                //UTF8Encoding encoding = new UTF8Encoding();
                ////byte[] ivStringBytes = encoding.GetBytes(Config.Instance.UserKeyString);
                //byte[] ivStringBytes = encoding.GetBytes(keyString);
                //byte[] IV = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
                //for (int i = 0; i < 12; i++)
                //{
                //    if (i >= ivStringBytes.Length)
                //        break;
                //    algKey[i] = ivStringBytes[i];
                //}

                //cryptAlgorithm.Key = algKey; // encoding.GetBytes(Properties.Settings.Default.cryptKey);
                //cryptAlgorithm.IV = IV;//Properties.Settings.Default.cryptIV);
                cryptAlgorithm.Key = getKey();
                cryptAlgorithm.IV = getIV();

                CryptoStream csEncrypt = new CryptoStream(fsFileIn, cryptAlgorithm.CreateDecryptor(), CryptoStreamMode.Read);

                StreamReader srCleanStream = new StreamReader(csEncrypt);
                String lineIn = "";
                while ((lineIn = srCleanStream.ReadLine()) != null)
                {
                    String[] inputStrArr = lineIn.Split(new char[] { ',' });

                    String newKey = inputStrArr[0].Trim(new char[] { '\t', '\n', ' ' });
                    String newKeyUsedStr = inputStrArr[1].Trim(new char[] { '\t', '\n', ' ' });
                    if (!validTags.Keys.Contains(newKey) && isTagValid(newKey, newKeyUsedStr))
                    {
                        validTags.Add(newKey, newKeyUsedStr);
                    }
                }

                srCleanStream.Close();
                fsFileIn.Close();
                fsKeyFile.Close();
            }
        }

        /// <summary>
        /// Given a string representation of a date, this method checks whether
        /// this tag is still valid. The rule is the following: if tag starts
        /// with b, it is valid for the full year 2011. Otherwise, it is valid
        /// for a week after its first use.
        /// </summary>
        /// <param name="tagIDStr">TagId value string.</param>
        /// <param name="usedStr">This a either an empty string, or string
        /// representation of a date.</param>
        /// <returns></returns>
        private bool isTagValid(String tagIDStr, String usedStr)
        {
            if (usedStr.Length == 0)
                return true;
            int validTagDuration = 7;
            if (tagIDStr.ToLower().StartsWith("b"))
                validTagDuration = 370;

            //if usedStr not empty, it has DateTime of the first use;
            DateTime firstUsedDate = DateTime.Now;
            DateTime.TryParse(usedStr, out firstUsedDate);
            firstUsedDate = firstUsedDate.AddDays(validTagDuration);
            int res = DateTime.Compare(firstUsedDate, DateTime.Now);
            if (res < 0)
                return false;
            else
                return true;
        }

        public void MarkTagAsUsed(TagId tagID)
        {
            try
            {
                /*
                String value = Properties.Settings.Default.tagsDictionary[tagID.Value] as String;
                if (value != null) //put date/time when first used
                {
                    if (validTags[tagID.Value] != null && validTags[tagID.Value].Equals(""))
                    {
                        Properties.Settings.Default.tagsDictionary[tagID.Value] = DateTime.Now.ToString();//"used";
                        validTags[tagID.Value] = DateTime.Now.ToString(); //"used";
                    }
                }
                */
                if (validTags[tagID.Value] != null && validTags[tagID.Value].Equals(""))
                {
                    validTags[tagID.Value] = DateTime.Now.ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DataManager.Log("Exception caught!" + e.StackTrace);
            }
        }

        public bool LoadTagFile(String filename)
        {
            try
            {
                /*
                if (Properties.Settings.Default.tagsDictionary == null)
                    Properties.Settings.Default.tagsDictionary = new System.Collections.Specialized.HybridDictionary();
                 */
                StreamReader sr = new StreamReader(filename);
                String line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    String newKey = line.Trim(new char[] { '\t', '\n', ' ' });
                    /*
                    if (!Properties.Settings.Default.tagsDictionary.Contains(newKey))
                    {
                        Properties.Settings.Default.tagsDictionary.Add(newKey, "");
                        validTags.Add(newKey, "");
                    }
                    */
                    if (!validTags.Keys.Contains(newKey))
                    {
                        validTags.Add(newKey, "");
                    }
                }
                /*
                Properties.Settings.Default.Save();
                */
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DataManager.Log("Exception caught!" + e.StackTrace);
                return false;
            }

            return true;
        }

        public bool LoadTagFileEncrypted(String encryptedFilename, String keyFilename, String keyString)
        {
            try
            {
                if (File.Exists(encryptedFilename) && File.Exists(keyFilename))
                {
                    FileStream fsFileIn = File.OpenRead(encryptedFilename);
                    FileStream fsKeyFile = File.OpenRead(keyFilename);

                    TripleDESCryptoServiceProvider cryptAlgorithm = new TripleDESCryptoServiceProvider();
                    BinaryReader brFile = new BinaryReader(fsKeyFile);
                    byte[] algKey = brFile.ReadBytes(24);

                    UTF8Encoding encoding = new UTF8Encoding();
                    //byte[] ivStringBytes = encoding.GetBytes(Config.Instance.UserKeyString);
                    byte[] ivStringBytes = encoding.GetBytes(keyString);
                    byte[] IV = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
                    for (int i = 0; i < 12; i++)// i < 8
                    {
                        //if (i < ivStringBytes.Length)
                        //    IV[i] = ivStringBytes[i];
                        if (i >= ivStringBytes.Length)
                            break;
                        algKey[i] = ivStringBytes[i];
                    }
                    cryptAlgorithm.Key = algKey;
                    cryptAlgorithm.IV = IV;// brFile.ReadBytes(8);

                    Dictionary<string, object> newValidTags = new Dictionary<string, object>();
                    HybridDictionary newHybridDictionary = new HybridDictionary();

                    CryptoStream csEncrypt = new CryptoStream(fsFileIn, cryptAlgorithm.CreateDecryptor(), CryptoStreamMode.Read);

                    StreamReader srCleanStream = new StreamReader(csEncrypt);
                    String lineIn = "";
                    while ((lineIn = srCleanStream.ReadLine()) != null)
                    {
                        String[] inputStrArr = lineIn.Split(new char[] { ',' });

                        String newKey = inputStrArr[0].Trim(new char[] { '\t', '\n', ' ' });
                        String newKeyUsedStr = inputStrArr[1].Trim(new char[] { '\t', '\n', ' ' });
                        /*
                        if (!newHybridDictionary.Contains(newKey) && isTagValid(newKey, newKeyUsedStr))
                        {
                            newHybridDictionary.Add(newKey, newKeyUsedStr);
                            newValidTags.Add(newKey, newKeyUsedStr);
                        }
                        */
                        if (!newValidTags.Keys.Contains(newKey) && isTagValid(newKey, newKeyUsedStr))
                        {
                            newValidTags.Add(newKey, newKeyUsedStr);
                        }
                    }

                    srCleanStream.Close();
                    fsFileIn.Close();
                    fsKeyFile.Close();

                    //Properties.Settings.Default.tagsDictionary = newHybridDictionary;
                    //validTags = newValidTags;
                    //do not remove, add
                    /*
                    if (Properties.Settings.Default.tagsDictionary == null)
                        Properties.Settings.Default.tagsDictionary = new HybridDictionary();
                    */
                    if (validTags == null)
                        validTags = new Dictionary<string, object>();

                    foreach (string key in newValidTags.Keys)
                    {
                        if (!validTags.Keys.Contains(key))
                            validTags.Add(key, newValidTags[key]);
                    }

                    /*
                    foreach (string key in newHybridDictionary.Keys)
                    {
                        if (!Properties.Settings.Default.tagsDictionary.Contains(key))
                            Properties.Settings.Default.tagsDictionary.Add(key, newHybridDictionary[key]);
                    }
                    */

                    //-------------------------------------------------------
                    // Check if the file already exists in isolated storage.
                    //-------------------------------------------------------

                    //IsolatedStorageFile isoStore = IsolatedStorageFile.GetMachineStoreForAssembly();

                    //string[] fileNames = isoStore.GetFileNames( ISOLATED_FILE_NAME );
                    //bool fileExists = false;
                    //foreach (string file in fileNames)
                    //{
                    //    if (file == ISOLATED_FILE_NAME)
                    //    {
                    //        fileExists = true;
                    //        break;
                    //    }
                    //}
                    //using (IsolatedStorageFileStream oStream = new IsolatedStorageFileStream(ISOLATED_FILE_NAME, FileMode.OpenOrCreate, isoStore))
                    //Directory.SetCurrentDirectory(tagDirectory);
                    //using (StreamWriter writer = new StreamWriter(keyFilename))
                    //{
                    //    UTF8Encoding enc = new UTF8Encoding();
                    //    writer.WriteLine(keyString);
                    //    //writer.WriteLine(enc.GetString(cryptAlgorithm.Key));
                    //    //writer.WriteLine(enc.GetString(cryptAlgorithm.IV));
                    //}
                }
                else
                {
                    MessageBox.Show("Sorry, at least one of the files you specified does not exist!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught!" + e.StackTrace);
                DataManager.Log("Exception caught!" + e.StackTrace);
                return false;
            }

            return true;
        }

        #region Export/Import Event Schedule
        public bool ExportEventSchedule(String filename, BindingList<Race> races)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filename);
                sw.WriteLine("name,type,date,classes");
                foreach (Race race in races)
                {
                    sw.WriteLine(race.ToStringListValuesUpdated());
                }
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool ImportEventSchedule(String filename, out BindingList<Race> result)
        {
            result = new BindingList<Race>();

            try
            {
                //DataSet ds = new DataSet();
                //string strConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                //    "Data Source=" + System.IO.Path.GetDirectoryName(filename) + ";" + "Extended Properties=\"Text;FMT=Delimited\"";
                //using (System.Data.OleDb.OleDbConnection conText = new System.Data.OleDb.OleDbConnection(strConnectionString))
                //{
                //    System.Data.OleDb.OleDbDataAdapter oleb = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM [" + System.IO.Path.GetFileName(filename) + "]", conText);
                //    oleb.Fill(ds, "whatever");
                //    oleb.Dispose();
                //}

                //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    try
                //    {
                //        //name,type,date,classes
                //        String nameString = ds.Tables[0].Rows[i]["name"].ToString();
                //        String typeString = ds.Tables[0].Rows[i]["type"].ToString();
                //        String dateString = ds.Tables[0].Rows[i]["date"].ToString();
                //        String classesString = ds.Tables[0].Rows[i]["classes"].ToString();
                
                //on the reasons for not using OleDB, please see below method that imports competitors
                using (CsvReader csv = new CsvReader(new StreamReader(filename), true))
                {
                    csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;

                    while (csv.ReadNextRecord())
                    {
                        try
                        {
                            String nameString = (csv["name"] != null) ? csv["name"].ToString() : "";
                            String typeString = (csv["type"] != null) ? csv["type"].ToString() : "";
                            String dateString = (csv["date"] != null) ? csv["date"].ToString() : "";
                            String classesString = (csv["classes"] != null) ? csv["classes"].ToString() : "";

                            Race newRace = new Race(nameString, typeString);
                            DateTime d = DateTime.Now;
                            DateTime.TryParse(dateString, out d);
                            newRace.Dates.Add(d);

                            String[] classesArr = classesString.Split(new char[] { ';' });
                            for (int j = 0; j < classesArr.Length; j += 2)
                            {
                                if (j + 1 >= classesArr.Length || j > classesArr.Length) break;

                                String className = classesArr[j];
                                String classDescription = classesArr[j + 1];

                                newRace.validClasses.Add(new Class(className, classDescription));

                                if (!isActiveClass(className, classDescription))
                                {
                                    this.Classes.Add(new Class(className, classDescription));
                                }
                            }
                            result.Add(newRace);
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc.StackTrace);
                            continue;
                        }        
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
                return false;
            }

            return true;
        }
        #endregion

        #region Export/Import Competitors
        public bool ExportCompetitors(String filename)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filename);
                sw.WriteLine("ID,lastName,firstName,address,phoneNumber,dob,age,gender,sponsors,bikeBrand,compNumber,tagNumber,tagNumber2");
                foreach (Competitor c in competitors)
                {
                    sw.WriteLine(c.ToStringListValuesUpdated());
                }
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        //TODO: rename to ImportCompetitorsCSV
        public bool ImportCompetitorsXLS(String filename, bool append)
        {
            try
            {
                //DataSet ds = new DataSet();
                //string strConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                //    "Data Source=" + System.IO.Path.GetDirectoryName(filename) + ";" + "Extended Properties=\"Text;HDR=Yes;FMT=Delimited\"";
                //using (System.Data.OleDb.OleDbConnection conText = new System.Data.OleDb.OleDbConnection(strConnectionString))
                //{
                //    System.Data.OleDb.OleDbDataAdapter oleb = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM [" + System.IO.Path.GetFileName(filename) + "]", conText);
                //    oleb.Fill(ds, "whatever");
                //    oleb.Dispose();
                //}

                if(!append)
                    this.competitors.Clear();

                //OleDB has a nasty problem - if it reads a field, say 20, it assumes that it is number, and
                //in the following rows this fields is supposed to be integer
                //the way to enforce certain data types is to have schema file (.ini) in the same directory as the 
                //original file; unfortunately it looks like there is no other (nicer) way to achieve this
                //
                //Thus, instead of using OleDB, we use a free library for reading CSV files:
                //http://www.codeproject.com/KB/database/CsvReader.aspx
                //
                using (CsvReader csv = new CsvReader(new StreamReader(filename), true))
                {
                    //int fieldCount = csv.FieldCount;
                    //string[] headers = csv.GetFieldHeaders();

                    csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;

                    while (csv.ReadNextRecord())
                    {
                        String idStr = (csv["ID"] != null) ? csv["ID"].ToString() : "";
                        String lastNameStr = (csv["lastName"] != null) ? csv["lastName"].ToString() : "";
                        String firstNameStr = (csv["firstName"] != null) ? csv["firstName"].ToString() : "";
                        String addressStr = (csv["address"] != null) ? csv["address"].ToString() : "";
                        String phoneStr = (csv["phoneNumber"] != null) ? csv["phoneNumber"].ToString() : "";
                        String dobStr = (csv["dob"] != null) ? csv["dob"].ToString() : "";
                        String ageStr = (csv["age"] != null) ? csv["age"].ToString() : "";
                        String genderStr = (csv["gender"] != null) ? csv["gender"].ToString() : "";
                        String sponsorsStr = (csv["sponsors"] != null) ? csv["sponsors"].ToString() : "";
                        String bikeBrandStr = (csv["bikeBrand"] != null) ? csv["bikeBrand"].ToString() : "";
                        String bikeNumberStr = (csv["compNumber"] != null) ? csv["compNumber"].ToString() : "";
                        String tagNumberStr = (csv["tagNumber"] != null) ? csv["tagNumber"].ToString() : "";
                        String tagNumber2Str = (csv["tagNumber2"] != null) ? csv["tagNumber2"].ToString() : "";

                        //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        //{
                        //    String idStr = ds.Tables[0].Rows[i]["ID"].ToString();
                        //    String lastNameStr = ds.Tables[0].Rows[i]["lastName"].ToString();
                        //    String firstNameStr = ds.Tables[0].Rows[i]["firstName"].ToString();
                        //    String addressStr = ds.Tables[0].Rows[i]["address"].ToString();
                        //    String phoneStr = ds.Tables[0].Rows[i]["phoneNumber"].ToString();
                        //    String dobStr = ds.Tables[0].Rows[i]["dob"].ToString();
                        //    String ageStr = ds.Tables[0].Rows[i]["age"].ToString();
                        //    String genderStr = ds.Tables[0].Rows[i]["gender"].ToString();
                        //    String sponsorsStr = ds.Tables[0].Rows[i]["sponsors"].ToString();
                        //    String bikeBrandStr = ds.Tables[0].Rows[i]["bikeBrand"].ToString();
                        //    String bikeNumberStr = ds.Tables[0].Rows[i]["bikeNumber"].ToString();
                        //    String tagNumberStr = ds.Tables[0].Rows[i]["tagNumber"].ToString();
                        //    String tagNumber2Str = ds.Tables[0].Rows[i]["tagNumber2"].ToString();

                        String[] addressArr = new String[] { "", "", "", "12345" };
                        try
                        {
                            String[] adrAr = addressStr.Split(new char[] { ';' });
                            if (adrAr[0] != null)
                                addressArr[0] = adrAr[0];
                            if (adrAr[1] != null)
                                addressArr[1] = adrAr[1];
                            if (adrAr[2] != null)
                                addressArr[2] = adrAr[2];
                            if (adrAr[3] != null)
                                addressArr[3] = adrAr[3];
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.StackTrace);
                            DataManager.Log(e.StackTrace);
                        }

                        int cID = 0;
                        String lastName = "";
                        String firstName = "";
                        Address address = new Address("", "", "", 12345);
                        PhoneNumber phoneNumber = new PhoneNumber("");
                        DateTime dob = DateTime.Now;
                        int age = 20;
                        bool gender = true;
                        String sponsors = "";
                        String bikeBrand = "";
                        String bikeNumber = "0";
                        TagId tagNumber = new TagId();
                        TagId tagNumber2 = new TagId();

                        try
                        {
                            try
                            {
                                cID = int.Parse(idStr);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                                DataManager.Log(e.StackTrace);
                            }
                            lastName = lastNameStr;
                            firstName = firstNameStr;
                            try
                            {
                                address = new Address(addressArr[0].Trim(), addressArr[1].Trim(), addressArr[2].Trim(), int.Parse(addressArr[3].Trim()));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                                DataManager.Log(e.StackTrace);
                            }
                            phoneNumber = new PhoneNumber(phoneStr);

                            try
                            {
                                dob = DateTime.Parse(dobStr);

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                                DataManager.Log(e.StackTrace);
                            }

                            try
                            {
                                age = int.Parse(ageStr);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                                DataManager.Log(e.StackTrace);
                            }
                            gender = genderStr.ToLower().Equals("true") ? true : false;
                            if (genderStr.ToLower().Equals(""))
                                gender = true;
                            sponsors = sponsorsStr;
                            bikeBrand = bikeBrandStr;

                            bikeNumber = bikeNumberStr;
                            //try
                            //{
                            //    bikeNumber = int.Parse(bikeNumberStr);
                            //}
                            //catch (Exception e)
                            //{
                            //    Console.WriteLine(e.StackTrace);
                            //    DataManager.Log(e.StackTrace);
                            //}

                            tagNumber = new TagId(tagNumberStr);
                            tagNumber2 = new TagId(tagNumber2Str);
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc.StackTrace);
                            DataManager.Log("Exception caught!" + exc.StackTrace);
                        }
                        Competitor c = new Competitor(cID, lastName, firstName, address, phoneNumber,
                            dob, age, gender, sponsors, bikeBrand, bikeNumber, tagNumber, tagNumber2);
                        this.competitors.Add(c);
                    }
                }
            }
            catch (Exception exc)
            {
                return false;
            }

            return true;
        }
        #endregion

        public bool ExportEventEntries(String filename)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filename);
                //sw.WriteLine("ID,lastName,firstName,address,phoneNumber,dob,age,gender,sponsors,bikeBrand,bikeNumber,tagNumber,tagNumber2");
                //foreach (Competitor c in competitors)
                //{
                //    sw.WriteLine(c.ToStringListValuesUpdated());
                //}
                
                sw.WriteLine("competitorID,lastName,firstName,eventID,bikeBrand,bikeNumber,className,tagNumber,tagNumber2");
                foreach (EventEntry eve in eventEntries)
                {
                    sw.WriteLine(eve.ToStringListValues());
                }
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public void ExportPassingsToFileCSV(String filename, BindingList<PassingsInfo> passingsList)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filename);
                sw.WriteLine("TagID,Frequency,SignalStrength,Antenna,Time,DateTime,Hits,competitorID,competitionNumber,firstName,lastName,lapTime,deleted");

                BindingList<PassingsInfo> bindings = passingsList;//(passingsDataGrid.DataSource as BindingList<PassingsInfo>);
                foreach (PassingsInfo pi in bindings)
                {
                    sw.WriteLine(pi.ToStringListValues());
                }

                sw.Flush();
                sw.Close();
            }
            catch (IOException ioe)
            {
                MessageBox.Show("Sorry, could not save the file.");
            }
        }

        public void ExportPassingsToFileHTML(String filename, BindingList<PassingsInfo> passingsList)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filename);
                sw.WriteLine("<HTML>");
                sw.WriteLine("<HEAD><TITLE></TITLE></HEAD>");
                sw.WriteLine("<FONT  face=\"Arial size=\"1\"><TABLE  width=\"90%\" align=\"center\">");
                sw.WriteLine("<TABLE border=\"1\">");
                sw.WriteLine("<TR align=\"left\">");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">TagID</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">Frequency</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">SignalStrength</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">Antenna</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">Time</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">DateTime</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">Hits</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">competitorID</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">firstName</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">lastName</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">lapTime</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">deleted</FONT></TH>");
                sw.WriteLine("</TR>");

                BindingList<PassingsInfo> bindings = passingsList;//(passingsDataGrid.DataSource as BindingList<PassingsInfo>);
                foreach (PassingsInfo pi in bindings)
                {
                    sw.WriteLine("<tr>");
                    sw.WriteLine(pi.ToStringListValuesHTML());
                    sw.WriteLine("</tr>");
                }
                sw.WriteLine("</TABLE>");
                sw.WriteLine("</FONT>");
                sw.WriteLine("</HTML>");

                sw.Flush();
                sw.Close();
            }
            catch (IOException ioe)
            {
                MessageBox.Show("Sorry, could not save the file.");
            }
        }

        public void ExportRaceInfoFileCSV(String filename, SortableBindingList<CompetitorRace> currentRaceList)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filename);
                //sw.WriteLine("firstName,lastName,TagID,TagID2,competitorID,compNumber,className,currentPlace,bestLap,lastLap,lapsCompleted");
                sw.WriteLine("currentPlace,compNumber,firstName,lastName,bikeBrand,className,lapsCompleted,lastLap,bestLap,TagID,TagID2,competitorID");

                foreach (CompetitorRace cr in currentRaceList)
                {
                    sw.WriteLine(cr.ToStringListValues());
                }

                sw.Flush();
                sw.Close();
            }
            catch (IOException ioe)
            {
                MessageBox.Show("Sorry, could not save the file.");
            }
        }

        public void ExportRaceInfoFileHTML(String filename, SortableBindingList<CompetitorRace> currentRaceList)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filename);
                sw.WriteLine("<HTML>");
                sw.WriteLine("<HEAD><TITLE></TITLE></HEAD>");
                sw.WriteLine("<FONT  face=\"Arial size=\"1\"><TABLE  width=\"90%\" align=\"center\">");
                sw.WriteLine("<TABLE border=\"1\">");
                sw.WriteLine("<TR align=\"left\">");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">currentPlace</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">compNumber</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">firstName</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">lastName</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">bikeBrand</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">className</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">lapsCompleted</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">lastLap</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">bestLap</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">TagID</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">TagID2</FONT></TH>");
                sw.WriteLine("<TH><FONT face=\"Arial\" size=\"2\">competitorID</FONT></TH>");
                sw.WriteLine("</TR>");

                foreach (CompetitorRace cr in currentRaceList)
                {
                    sw.WriteLine("<tr>");
                    String[] crStrArray = cr.ToStringListValues().Split(new char[] { ',' });
                    for (int i = 0; i < crStrArray.Length; i++)
                    {
                        sw.Write("<td>");
                        sw.Write(crStrArray[i]);
                        sw.Write("</td>");
                    }
                    sw.WriteLine("</tr>");
                }

                sw.WriteLine("</TABLE>");
                sw.WriteLine("</FONT>");
                sw.WriteLine("</HTML>");

                sw.Flush();
                sw.Close();
            }
            catch (IOException ioe)
            {
                MessageBox.Show("Sorry, could not save the file.");
            }
        }
        
        private static StreamWriter streamLogger;
        public static void Log(String info)
        {
            try
            {
                if (streamLogger == null)
                {
                    string appPath = Path.GetDirectoryName(Config.Instance.DataDirectory);
                    Directory.SetCurrentDirectory(appPath);
                    streamLogger = new StreamWriter("log.txt");
                }
                streamLogger.WriteLine(info);
                streamLogger.Flush();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);//how to log this? :)
            }
        }

        private bool isActiveClass(String className, String classDescription)
        {
            foreach (Class c in this.Classes)
            {
                if (c.name != null && c.name.Equals(className))//should I check for description too?
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Search for all ids in the data and find the largest one (we have to ensure
        /// we never use the same id twice) - and then set uniqueIDGenerator to be larger
        /// than that.
        /// </summary>
        /// <returns></returns>
        public int GetCurrentMaxID()
        {
            int maxID = 0;
            try
            {

                foreach (Event ev in this.events)
                {
                    if (ev.ID > maxID)
                        maxID = ev.ID;
                    foreach (Race race in ev.races)
                    {
                        if (race.ID > maxID)
                            maxID = race.ID;
                    }
                    foreach (Class cl in ev.classes)
                    {
                        if (cl.classNumber > maxID)
                            maxID = cl.classNumber;
                    }
                }

                foreach (Competitor c in this.competitors)
                {
                    if (c.ID > maxID)
                        maxID = c.ID;
                }

                foreach (Class cl in this.classes)
                {
                    if (cl.classNumber > maxID)
                        maxID = cl.classNumber;
                }

                foreach (EventEntry ee in this.eventEntries)
                {
                    if (ee.ID > maxID)
                        maxID = ee.ID;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught!" + e.StackTrace);
                DataManager.Log("Exception caught!" + e.StackTrace);
            }

            return maxID + 50;//add some gap just in case
        }
    }
}
