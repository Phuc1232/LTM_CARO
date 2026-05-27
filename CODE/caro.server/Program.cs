using caro.server.models;
using caro.server.network;
using caro.server.services;
using caro.share.DTOs;
using caro.share.DTOs.Constants;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace caro.server
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize(); 
            //Application.Run(new Form1());
            //khoi dong server
            TCPServerManager server = new TCPServerManager();
            await server.StartServerAsync();

        }
    }
}