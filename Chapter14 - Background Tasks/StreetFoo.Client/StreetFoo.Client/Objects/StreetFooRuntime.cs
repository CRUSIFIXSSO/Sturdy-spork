﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using TinyIoC;
using Windows.Networking.Connectivity;

namespace StreetFoo.Client
{
    public static class StreetFooRuntime
    {
        // holds a reference to how we started...
        public static string Module { get; private set; }

        // holds a reference to the logon token...
        internal static string LogonToken { get; private set; }

        // holds a refrence to the database connections...
        internal const string SystemDatabaseConnectionString = "StreetFoo-system.db";
        internal static string UserDatabaseConnectionString = null;

        // defines the base URL of our services...
        internal const string ServiceUrlBase = "http://streetfoo.apphb.com/handlers/";

        // starts the application/sets up state...
        public static async Task Start(string module)
        {
            Module = module;

            // initialize TinyIoC...
            TinyIoCContainer.Current.AutoRegister();

            // initialize the system database... 
            var conn = GetSystemDatabase();
            await conn.CreateTableAsync<SettingItem>();
		}

        internal static bool HasLogonToken
        {
            get
            {
                return !(string.IsNullOrEmpty(LogonToken));
            }
        }

        internal static async Task LogonAsync(string username, string token)
        {
            // set the database to be a user specific one... (assumes the username doesn't have evil chars in it
            // - for production you may prefer to use a hash)...
            UserDatabaseConnectionString = string.Format("StreetFoo-user-{0}.db", username);

            // store the logon token...
            LogonToken = token;

            // initialize the database - has to be done async...
            var conn = GetUserDatabase();
            await conn.CreateTableAsync<ReportItem>();
        }

        internal static SQLiteAsyncConnection GetSystemDatabase()
        {
            return new SQLiteAsyncConnection(SystemDatabaseConnectionString);
        }

        internal static SQLiteAsyncConnection GetUserDatabase()
        {
            return new SQLiteAsyncConnection(UserDatabaseConnectionString);
        }

        internal static bool HasConnectivity
        {
            get
            {
                var profile = NetworkInformation.GetInternetConnectionProfile();
                return profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            }
        }
    }
}
