using System;
using System.IO;
using Notes.Models;
using Xamarin.Forms;
using System.Security.Cryptography;
using System.Text;



namespace Notes.Views
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class NoteEntryPage : ContentPage
    {
        public string ItemId
        {
            set
            {
                LoadNote(value);
            }
        }

        public NoteEntryPage()
        {
            InitializeComponent();

            // Set the BindingContext of the page to a new Note.
            BindingContext = new Note();
        }

        void LoadNote(string filename)
        {
            try
            {
                string allText = File.ReadAllText(filename);
                string[] campi = allText.Split('§');
                // Retrieve the note and set it as the BindingContext of the page.
                Note note = new Note
                {
                    Filename = filename,
                    ServiceName = campi[0],
                    Username = campi[1],
                    Password = campi[2],
                    URL = campi[3],
                    Date = File.GetCreationTime(filename)
                };
                BindingContext = note;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load note.");
            }
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;

            if (string.IsNullOrWhiteSpace(note.Filename))
            {
                // Save the file.
                var filename = Path.Combine(App.FolderPath, $"{Path.GetRandomFileName()}.notes.txt");
                string allText = note.ServiceName + "§" +
                    note.Username + "§" +
                    note.Password + "§" +
                    note.URL;

                Aes myAes = Aes.Create();
                myAes.Key = Encoding.ASCII.GetBytes("5415D8C5 AB58F7BB C39E84B7 BD2E9CC6 D45FE538 1A9A2091 735582E1 A90EA0B7");
                myAes.IV = Encoding.ASCII.GetBytes("0AC732E1 BEA37032 7A444755 0B26C55F");


                byte[] encrypted = EncryptStringToBytes_Aes(allText, myAes.Key, myAes.IV);




                File.WriteAllText(filename, allText);
            }
            else
            {
                // Update the file.
                string allText = note.ServiceName + "§" +
                                note.Username + "§" +
                                note.Password + "§" +
                                note.URL;
                File.WriteAllText(note.Filename, allText);
            }

            // Navigate backwards
            await Shell.Current.GoToAsync("..");
        }



        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }







        async void GeneraClick(object sender, EventArgs e)
        {
            const string LOWER_CASE = "abcdefghijklmnopqursuvwxyz";
            const string UPPER_CAES = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NUMBERS = "123456789";
            const string SPECIALS = @"!@£$%^&*()#€";

            char[] _password = new char[10];
            string charSet = ""; // Initialise to blank
            System.Random _random = new Random();
            int counter;

            // Build up the character set to choose from
            charSet += LOWER_CASE;

            charSet += UPPER_CAES;

            charSet += NUMBERS;

            charSet += SPECIALS;

            for (counter = 0; counter < 10; counter++)
            {
                _password[counter] = charSet[_random.Next(charSet.Length - 1)];
            }

            TxtPass.Text = String.Join(null, _password);
        }



        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;

            // Delete the file.
            if (File.Exists(note.Filename))
            {
                File.Delete(note.Filename);
            }

            // Navigate backwards
            await Shell.Current.GoToAsync("..");
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}