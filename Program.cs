﻿using System;
using Newtonsoft.Json.Linq;
using System.IO;
using Serilog;
using Serilog.Events;



namespace ExRules
{
    class Program
    {
        private static int result = 0;

        static int Main(string[] args)
        {

            mParser ParserJson = new mParser();


            //  Log.Logger = new LoggerConfiguration()
            // .MinimumLevel.Debug()
            // .WriteTo.Console()
            // .WriteTo.File("logs\\my_log.log", rollingInterval: RollingInterval.Day)
            // .CreateLogger();

            // TODO: Доделать проверку файлов и загрузки данных, что бы просто пропускаит обработку без ошибок

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs\\error_log.log", rollingInterval: RollingInterval.Day)
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File("logs\\info_log.log", rollingInterval: RollingInterval.Day))
            //.WriteTo.File("logs\\info_log.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
            .CreateLogger();


            //     object TypeData;
            MMessages mMessages = new MMessages();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Title = "Обработка правил выгрузки 0.5";

            Console.WriteLine("Версия 0.5");

            // Файл правил реквизиты
            ParserJson.FileRulesProperty = "/home/bat/Project/ExRules/SПрофилиГруппДоступа.json";  //;args[0];
                                                                                                   // Файл правил табличная часть

            ParserJson.FileRulesTabPart = "/home/bat/Project/ExRules/FПрофилиГруппДоступа.json";  //;args[1];
            // Файл данных для корректировки
            ParserJson.FileData = "/home/bat/Project/ExRules/bin/Debug/net5.0/ПрофилиГруппДоступа.json";  //;args[2];
            ParserJson.NameRules = "ПрофилиГруппДоступа";                                                  //;args[3];

            Log.Information("Обработка файла - " + ParserJson.NameRules);
            int result = ParserJson.StartParsing();

            if (result == 0)
            {
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(ParserJson.stuff, Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText(ParserJson.FileData, output);
            }

            Log.CloseAndFlush();

            return result;
        }



    }
}



