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
            
            // Verbindungszeichenfolge für MySQL
            string connectionString = "Server=10.0.2.2;Database=holisticapp;User=root;Password=;";
            UserDatabase = new UserDatabase(connectionString);

            // Setze die MainPage als NavigationPage, um Navigation zwischen Seiten zu ermöglichen
            MainPage = new NavigationPage(new Views.LoginPage());
        }
    }
}