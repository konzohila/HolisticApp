using System;
using System.IO;
using HolisticApp.Data;
using Microsoft.Maui.Controls;

namespace HolisticApp
{
    public partial class App : Application
    {
        // Statische Instanz, auf die von überall zugegriffen werden kann
        public static UserDatabase? UserDatabase { get; private set; }

        public App()
        {
            InitializeComponent();
            
            // Verbindungszeichenfolge für MySQL
            string connectionString = "Server=10.0.2.2;Database=holisticapp;User=root;Password=;";
            UserDatabase = new UserDatabase(connectionString);
        }
        
        // Überschreibe CreateWindow, um deine Startseite zu definieren
        protected override Window CreateWindow(IActivationState activationState)
        {
            // Hier wird eine NavigationPage erzeugt, die als Root-Page dient.
            return new Window(new NavigationPage(new Views.LoginPage()));
        }
    }
}