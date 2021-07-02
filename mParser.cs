using Newtonsoft.Json.Linq;
using System.IO;
using Serilog;
//using Serilog.Events;
using System;

namespace ExRules
{


    internal class mParser
    {
        public string FileRulesProperty;
        public string FileRulesTabPart;
        public string FileData;
        public string NameRules;
        public dynamic stuff;


        private static string contentsRulesProperty;
        private static string contentsRulesTabPart;

        private static string contentsData;
        private JObject CurRules;
        private JObject CurRulesTab;

        private JToken Rls;
        private object TypeData;

        // TODO: В табличных частях не дописан пропуск и перенос
        // TODO: Проверить, получится или нет проверять, что свойство существует
        // TODO: Переделать параметры, передавать только имя объекта, а остальное конмтруировать, можно такое условие по ключу  
        // FIXME  тест

        public mParser()
        {
            { { } }
        }


        public int StartParsing()
        {
            try
            {
                contentsRulesProperty = File.ReadAllText(FileRulesProperty);
                contentsRulesTabPart = File.ReadAllText(FileRulesTabPart);
                contentsData = File.ReadAllText(FileData);


                // Разбор файла с правилами шапки
                CurRules = JObject.Parse(contentsRulesProperty);

                // Разбор файла с правилами табличных частей
                CurRulesTab = JObject.Parse(contentsRulesTabPart);

                // Разбор файла с данными
                stuff = JObject.Parse(contentsData);

                ParseProperty();

            }
            catch (System.Exception ex)
            {
                Log.Error($"Исключение: {ex.Message}");
                Log.Error($"Метод: {ex.TargetSite}");
                Log.Error($"Трассировка стека: {ex.StackTrace}");

            }




            return 0;
        }

        private void ParseProperty()
        {
            Rls = CurRules.SelectToken("$.#value..#value[?(@.name.#value == " + "'" + NameRules + "'" + " )]");

            foreach (var curR in Rls["Value"]["#value"])
            {

                JToken Action = curR.SelectToken("$.Value.#value[?(@.name.#value == 'Действие' )].Value.#value");
                JToken SourceName = curR.SelectToken("$.Value.#value[?(@.name.#value == 'ИмяСвойстваИсточник' )].Value.#value");
                JToken RecName = curR.SelectToken("$.Value.#value[?(@.name.#value == 'ИмяСвойстваПриемник' )].Value.#value");
                JToken TypeName = curR.SelectToken("$.Value.#value[?(@.name.#value == 'ТипСтрокойПриемник' )].Value.#value");
                JToken Order = curR.SelectToken("$.Value.#value[?(@.name.#value == 'Порядок' )].Value.#value");
                JToken Bef = curR.SelectToken("$.Value.#value[?(@.name.#value == 'Перед' )].Value.#value");



                // Определение типа
                TypeData = GetTypeValue(TypeName.ToString());
                //               

                // Обработка основных реквизитов
                foreach (var Sdata in stuff["#value"])
                {
                    string tact = Action.ToString();

                    switch (tact)
                    {
                        case "Пропустить":
                            Sdata["#value"].Property(SourceName.ToString()).Remove();
                            break;
                        case "Добавить":
                            if (Bef.ToString() == "")
                            {
                                try
                                {
                                    Sdata["#value"].Add(new JProperty(SourceName.ToString(), TypeData));
                                }
                                catch (System.Exception ex)
                                {
                                    Log.Error($"Исключение: {ex.Message}");
                                    Log.Error($"Метод: {ex.TargetSite}");
                                    Log.Error($"Трассировка стека: {ex.StackTrace}");
                                }
                            }
                            else
                            {
                                try
                                {
                                    Sdata["#value"].Property(Bef.ToString()).AddBeforeSelf(new JProperty(SourceName.ToString(), TypeData));
                                }
                                catch (System.Exception ex)
                                {
                                    Log.Error($"Исключение: {ex.Message}");
                                    Log.Error($"Метод: {ex.TargetSite}");
                                    Log.Error($"Трассировка стека: {ex.StackTrace}");
                                }

                            }

                            break;
                        case "Переименовать":

                            try
                            {
                                Sdata["#value"].Property(SourceName.ToString()).Replace(new JProperty(RecName.ToString(), Sdata["#value"][SourceName.ToString()]));
                            }
                            catch (System.Exception ex)
                            {
                                Log.Error($"Исключение: {ex.Message}");
                                Log.Error($"Метод: {ex.TargetSite}");
                                Log.Error($"Трассировка стека: {ex.StackTrace}");

                            }


                            //*****************************************************
                            break;
                        case "Перенести":
                            string exValue = Sdata["#value"][SourceName.ToString()];
                            try
                            {
                                Sdata["#value"].Property(SourceName.ToString()).Remove();
                                if (Bef.ToString() == "")
                                {

                                    Sdata["#value"].Add(new JProperty(SourceName.ToString(), exValue));
                                }
                                else
                                {
                                    Sdata["#value"].Property(Bef.ToString()).AddBeforeSelf(new JProperty(SourceName.ToString(), exValue));
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Log.Error($"Исключение: {ex.Message}");
                                Log.Error($"Метод: {ex.TargetSite}");
                                Log.Error($"Трассировка стека: {ex.StackTrace}");

                            }
                            //***************************************************9


                            break;
                        default:

                            break;
                    }

                }
            }




        }

        private void ParseTabPart()
        {

            // Разбор правил табличных частей
            NameRules = "ТабличныеЧасти";

            // Список табличных частей
            int x = 0;
            JToken RlsTab = CurRulesTab.SelectToken("$.#value[?(@.name.#value == " + "'" + NameRules + "'" + " )]");
            if (RlsTab != null) // Если обписание табличных частей отсутствует
            {
                foreach (var curR in RlsTab["Value"]["#value"])
                {

                    Console.WriteLine(curR["#value"] + " Наименование таб части");
                    string CurTabCh = curR["#value"].ToString();
                    JToken RlsRk = CurRulesTab.SelectToken("$.#value[?(@.name.#value == " + "'" + curR["#value"].ToString() + "'" + " )]");
                    x++;
                    foreach (var CurObj in RlsRk["Value"]["#value"])
                    {
                        //            Console.WriteLine(CurObj);

                        JToken Action = CurObj.SelectToken("$.Value.#value[?(@.name.#value == 'Действие' )].Value.#value");
                        JToken SourceName = CurObj.SelectToken("$.Value.#value[?(@.name.#value == 'ИмяСвойстваИсточник' )].Value.#value");
                        JToken RecName = CurObj.SelectToken("$.Value.#value[?(@.name.#value == 'ИмяСвойстваПриемник' )].Value.#value");
                        JToken TypeName = CurObj.SelectToken("$.Value.#value[?(@.name.#value == 'ТипСтрокойПриемник' )].Value.#value");
                        JToken Order = CurObj.SelectToken("$.Value.#value[?(@.name.#value == 'Порядок' )].Value.#value");
                        JToken Bef = CurObj.SelectToken("$.Value.#value[?(@.name.#value == 'Перед' )].Value.#value");

                        TypeData = GetTypeValue(TypeName.ToString());
                        Console.WriteLine(TypeData);


                        //    JToken TabC = stuff.SelectToken("$.Value.#value[?(@.name.#value == 'КонтактнаяИнформация' )].Value.#value");
                        dynamic TabC = stuff.SelectToken("$..#value[" + "'" + CurTabCh + "'" + "] ");
                        if (TabC != null)
                        {


                            foreach (var Sdata in TabC)
                            {

                                //  Console.WriteLine((Sdata["#value"]["Ref"]));
                                string tact = Action.ToString();

                                switch (tact)
                                {
                                    case "Пропустить":
                                        //                 Sdata["#value"].Property(SourceName.ToString()).Remove();
                                        break;
                                    case "Добавить":
                                        if (Bef.ToString() == "")
                                        {
                                            try
                                            {
                                                Sdata.Add(new JProperty(SourceName.ToString(), TypeData));
                                            }
                                            catch (System.Exception ex)
                                            {
                                                Log.Error($"Исключение: {ex.Message}");
                                                Log.Error($"Метод: {ex.TargetSite}");
                                                Log.Error($"Трассировка стека: {ex.StackTrace}");
                                            }

                                        }
                                        else
                                        {
                                            Sdata.Property(Bef.ToString()).AddBeforeSelf(new JProperty(SourceName.ToString(), TypeData));
                                        }

                                        break;
                                    case "Переименовать":

                                        try
                                        {
                                            Sdata.Property(SourceName.ToString()).Replace(new JProperty(RecName.ToString(), Sdata[SourceName.ToString()]));
                                        }
                                        catch (System.Exception ex)
                                        {

                                            Log.Error($"Исключение: {ex.Message}");
                                            Log.Error($"Метод: {ex.TargetSite}");
                                            Log.Error($"Трассировка стека: {ex.StackTrace}");
                                        }

                                        break;
                                    default:

                                        break;
                                }

                                //     }
                            }
                        }
                        else
                        {
                            Log.Warning("В файле данных " + NameRules + " отсутствует информация от табличных частях!");
                        }
                    }
                    //         Console.WriteLine(x);

                    // }

                }
            }

        }

        private object GetTypeValue(string Act)
        {
            switch (Act)
            {
                case "Булево":
                    return false;
                case "Строка":
                    return "";
                case "Число":
                    return "0";
                case "":
                    return "";
                default:
                    return "00000000-0000-0000-0000-000000000000";

            }


        }

    }


}