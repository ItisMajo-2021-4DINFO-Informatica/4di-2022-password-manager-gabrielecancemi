using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Notes.Models;
using Xamarin.Forms;
using System.Security.Cryptography;
using System.Text;


namespace Notes.Views
{
    public partial class NotesPage : ContentPage
    {
        string chiave = "p9e6s6wn2,a4z8d4f3w8d4f3w87wqèas";
        string vettore = "p9u6r3e2wavzm<.-";
        public NotesPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var notes = new List<Note>();

            // Create a Note object from each file.
            var files = Directory.EnumerateFiles(App.FolderPath, "*.notes.txt");
            foreach (var filename in files)
            {
                string allText = File.ReadAllText(filename);


                string testoDecriptato = Decrittografia(allText);



                string[] campi = testoDecriptato.Split('+');
                notes.Add(new Note
                {
                    Filename = filename,
                    ServiceName = campi[0],
                    Username = campi[1],
                    Password = campi[2],
                    URL = campi[3],
                    Date = File.GetCreationTime(filename)
                });
                
            }

            // Set the data source for the CollectionView to a
            // sorted collection of notes.
            collectionView.ItemsSource = notes
                .OrderBy(d => d.Date)
                .ToList();
        }

        async void OnAddClicked(object sender, EventArgs e)
        {
            // Navigate to the NoteEntryPage, without passing any data.
            await Shell.Current.GoToAsync(nameof(NoteEntryPage));
        }

        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                // Navigate to the NoteEntryPage, passing the filename as a query parameter.
                Note note = (Note)e.CurrentSelection.FirstOrDefault();
                await Shell.Current.GoToAsync($"{nameof(NoteEntryPage)}?{nameof(NoteEntryPage.ItemId)}={note.Filename}");
            }
        }

        public string Decrittografia( string allText)
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

            return(decritto);
        }
    }
}