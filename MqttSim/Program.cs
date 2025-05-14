// See https://aka.ms/new-console-template for more information
using SparkplugNet.Core.Enumerations;
using SparkplugNet.Core.Node;
using Serilog;
//using SystemCancellationToken = System.Threading.CancellationToken;
using System;
using OpenHIoT.MqttSim;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.MqttSim.Devices;
/*
global using MQTTnet.Protocol;

global using Serilog;

global using SparkplugNet.Core.Application;
global using SparkplugNet.Core.Enumerations;
global using SparkplugNet.Core.Node;

global using VersionAData = SparkplugNet.VersionA.Data;
global using VersionBData = SparkplugNet.VersionB.Data;
*/
Console.WriteLine("Simulator Starts!");
Products.Instance = Products.GetProducts();
StartUp.Start(Products.Instance);


while(true)
    Thread.Sleep(2000);

