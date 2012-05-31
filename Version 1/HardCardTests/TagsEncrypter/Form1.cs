using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace TagsEncrypter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            selectTagFileButton.Click += new EventHandler(selectTagFileButton_Click);
            encryptFileButton.Click += new EventHandler(encryptFileButton_Click);

            selectEncryptedFileButton.Click += new EventHandler(selectEncryptedFileButton_Click);
            selectKeyFileButton.Click += new EventHandler(selectKeyFileButton_Click);
        }

        private void selectEncryptedFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Title = "Select Encrypted File";
            fd.Filter = "bin files (*.bin)|*.bin";
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                encryptedFileTextField.Text = fd.FileName;
            }
            fd.Dispose();
        }

        private void selectKeyFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Title = "Select Key File";
            fd.Filter = "bin files (*.bin)|*.bin";
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                keyFileTextField.Text = fd.FileName;
            }
            fd.Dispose();
        }

        private void encryptFileButton_Click(object sender, EventArgs e)
        {
            if (encryptStringTextField.Text.Length <= 0)
            {
                MessageBox.Show("Cannot use empty string to encode!");
                return;
            }

            if (encryptedFileTextField.Text.Length <= 0)
            {
                MessageBox.Show("Please select an encrypted file path first.");
                return;
            }

            if (keyFileTextField.Text.Length <= 0)
            {
                MessageBox.Show("Please select a key file path first.");
                return;
            }

            try
            {
                FileStream fsFileOut = File.OpenWrite(encryptedFileTextField.Text);
                TripleDESCryptoServiceProvider cryptAlgorithm = new TripleDESCryptoServiceProvider();
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytes = encoding.GetBytes(encryptStringTextField.Text);
                byte[] bytesIVString = encoding.GetBytes(userSettingsStringTextBox.Text);
                byte[] cryptKey = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
                byte[] cryptIV = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };

                for (int i = 0; i < bytes.Length; i++)
                {
                    if (i >= 24)
                        break;
                    cryptKey[i] = bytes[i];
                }
                //for (int i = 0; i < bytesIVString.Length; i++)
                //{
                //    if (i >= 8)
                //        break;
                //    cryptIV[i] = bytesIVString[i];
                //}

                //substitute first half of the key with a user-specific string
                for (int i = 0; i < 12; i++)
                {
                    if (i >= bytesIVString.Length)
                        break;
                    cryptKey[i] = bytesIVString[i];
                }

                cryptAlgorithm.Key = cryptKey;
                cryptAlgorithm.IV = cryptIV;

                CryptoStream csEncrypt = new CryptoStream(fsFileOut, cryptAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
                StreamWriter swEncStream = new StreamWriter(csEncrypt);

                if (validTags != null)
                {
                    foreach (String key in validTags.Keys)
                    {
                        swEncStream.WriteLine(key + "," + validTags[key]);
                    }
                }

                swEncStream.Flush();
                swEncStream.Close();

                // Create the key file
                FileStream fsFileKey = File.Create(keyFileTextField.Text);
                BinaryWriter bwFile = new BinaryWriter(fsFileKey);
                bwFile.Write(cryptAlgorithm.Key);
                bwFile.Write(cryptAlgorithm.IV);
                bwFile.Flush();
                bwFile.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
                MessageBox.Show("Sorry, there was a problem: " + exc.StackTrace);
                return;
            }
            MessageBox.Show("Encrypted file and key file were created successfully.");
        }

        private void selectTagFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tagFileStringTextBox.Text = fd.FileName;
                ReadTagFile(fd.FileName);
            }
            fd.Dispose();
        }

        Dictionary<string, object> validTags = new Dictionary<string,object>();
        private void ReadTagFile(String filename)
        {
            try
            {
                StreamReader sr = new StreamReader(filename);
                String line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    String newKey = line.Trim(new char[] { '\t', '\n', ' ' });
                    if (!validTags.ContainsKey(newKey))
                    {
                        validTags.Add(newKey, "");
                    }
                }                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
