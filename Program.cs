using System;
using Newtonsoft.Json.Linq;
using System.IO;
using Serilog;
using Serilog.Events;
using Newtonsoft.Json;
using System.Collections.Generic;



namespace ExRules
{
    class Program
    {
        private static int result = 0;

        static int Main(string[] args)
        {

            //  Log.Logger = new LoggerConfiguration()
            // .MinimumLevel.Debug()
            // .WriteTo.Console()
            // .WriteTo.File("logs\\my_log.log", rollingInterval: RollingInterval.Day)
            // .CreateLogger();

            // TODO:  Доделать проверку файлов и загрузки данных, что бы просто пропускаит обработку без ошибок

            string path = Directory.GetCurrentDirectory();
            // mSettings mSettings 
            mSettings CurSet = new mSettings();

            CurSet.jsonString = File.ReadAllText(path + "\\setting.json");
            CurSet = JsonConvert.DeserializeObject<mSettings>(CurSet.jsonString);

            // CurSet = JsonConvert.DeserializeObject<mSettings>(CurSet.jsonString);
            string LogFile = CurSet.PathLog;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(LogFile + "error_log.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(LogFile + "info_log.log", rollingInterval: RollingInterval.Day))
                //.WriteTo.File("logs\\info_log.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();


            //     object TypeData;
            MMessages mMessages = new MMessages();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Title = "Обработка правил выгрузки 0.5";

            Console.WriteLine("Версия 0.5");

            mParser ParserJson = new mParser
            {
                FileRulesProperty = args[0],
                FileRulesTabPart = args[1],
                FileData = args[2],
                NameRules = args[3]
            };
            // mParser ParserJson = new();
            // // Файл правил реквизиты
            // ParserJson.FileRulesProperty = "/home/bat/Project/ExRules/SДополнительныеОтчетыИОбработки.json";  //;args[0];
            // // Файл правил табличная часть
            // ParserJson.FileRulesTabPart = "/home/bat/Project/ExRules/FДополнительныеОтчетыИОбработки.json";  //;args[1];
            // // Файл данных для корректировки
            // ParserJson.FileData = "/home/bat/Project/ExRules/bin/Debug/net5.0/ДополнительныеОтчетыИОбработки.json";  //;args[2];
            // ParserJson.NameRules = "ДополнительныеОтчетыИОбработки";                                                  //;args[3];

            Log.Information("Обработка файла - " + ParserJson.NameRules);
            int result = ParserJson.StartParsing();

            if (result == 0)
            {
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(ParserJson.stuff, Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText(ParserJson.FileData, output);
                Log.Information("Успешное завершение - " + ParserJson.NameRules);
            }

            Log.CloseAndFlush();

            return result;
        }



    }
}



