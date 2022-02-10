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
        string chiave = "p9e6s6wn2,a4z8d4f3w8d4f3w87wqèas";
        string vettore = "p9u6r3e2wavzm<.-";
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
                string testoDecriptato = Decrittografia(allText);
                string[] campi = testoDecriptato.Split('+');
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
                string allText = note.ServiceName + "+" +
                    note.Username + "+" +
                    note.Password + "+" +
                    note.URL;


                File.WriteAllText(filename, Crittografia(allText));
            }
            else
            {
                // Update the file.
                string allText = note.ServiceName + "+" +
                                note.Username + "+" +
                                note.Password + "+" +
                                note.URL;
                File.WriteAllText(note.Filename, Crittografia(allText));
            }

            // Navigate backwards
            await Shell.Current.GoToAsync("..");
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

       


        public string Crittografia( string allText)
        {
            byte[] testoDacriptare = ASCIIEncoding.ASCII.GetBytes(allText);

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = ASCIIEncoding.ASCII.GetBytes(chiave);
            aes.IV = ASCIIEncoding.ASCII.GetBytes(vettore);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] testoCriptato = ict.TransformFinalBlock(testoDacriptare, 0, testoDacriptare.Length);
            ict.Dispose();

            string critto = Convert.ToBase64String(testoCriptato);
            return critto;
        }


        public string Decrittografia(string allText)
        {
            byte[] testoCriptato = Convert.FromBase64String(allText);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = ASCIIEncoding.ASCII.GetBytes(chiave);
            aes.IV = ASCIIEncoding.ASCII.GetBytes(vettore);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;


            ICryptoTransform tdc = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] testoDecriptato = tdc.TransformFinalBlock(testoCriptato, 0, testoCriptato.Length);
            tdc.Dispose();
            string decritto = ASCIIEncoding.ASCII.GetString(testoDecriptato);

            return (decritto);
        }

        private void Button_Clicked(object sender, EventArgs e)
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
    }
}