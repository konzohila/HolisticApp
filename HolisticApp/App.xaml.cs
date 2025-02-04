using System;
using System.IO;
using HolisticApp.Data;
using Microsoft.Maui.Controls;

namespace HolisticApp
{
    public partial class App : Application
    {
        // Statische Instanz, auf die von überall zugegriffen werden kann
        public static UserDatabase UserDatabase { get; private set; }

        public App()
        {
            InitializeComponent();

            // Erstelle den Pfad zur Datenbank
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UserDatabase.db3");
            UserDatabase = new UserDatabase(dbPath);

            // Setze die MainPage als NavigationPage, um Navigation zwischen Seiten zu ermöglichen
            MainPage = new NavigationPage(new Views.LoginPage());
        }
    }
}