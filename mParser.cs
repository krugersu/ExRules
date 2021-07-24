using Newtonsoft.Json.Linq;
using System.IO;
using Serilog;
//using Serilog.Events;
using System;


namespace ExRules
{


    internal class mParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value><strong>Файл с правилами шапки</strong></value>
        public string FileRulesProperty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value>Файл с правилами для табличной части</value>
        ///  <remarks>
        ///description
        ///</remarks>
        public string FileRulesTabPart { get; set; }
        public string FileData { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value>Имя объекта метаданных</value>
        public string NameRules { get; set; }
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

        }

        /// <summary>  
        ///  Запускает разбор файла
        /// </summary>  
        public int StartParsing()
        {
            try
            {

                if (File.Exists(FileRulesProperty))
                {
                    contentsRulesProperty = File.ReadAllText(FileRulesProperty);
                    // Разбор файла с правилами шапки
                    CurRules = JObject.Parse(contentsRulesProperty);
                }

                if (File.Exists(FileRulesTabPart))
                {
                    contentsRulesTabPart = File.ReadAllText(FileRulesTabPart);
                    // Разбор файла с правилами табличных частей
                    CurRulesTab = JObject.Parse(contentsRulesTabPart);
                }

                if (File.Exists(FileData))
                {
                    contentsData = File.ReadAllText(FileData);
                    // Разбор файла с данными
                    stuff = JObject.Parse(contentsData);
                }

                if (CurRules != null)
                {
                    ParseProperty();
                }

                if (CurRulesTab != null)
                {
                    // Проверка содержит ли файл таб части со составными реквизитами
                    if (CurRulesTab.SelectToken("$..tCombo") != null)
                    {
                        ParseTabPartWhithMultiValue();
                    }
                    else
                    {
                        ParseTabPart();
                    }
                }
                //

                return 0;
            }
            catch (System.Exception ex)
            {
                Log.Error($"Исключение: {ex.Message}");
                Log.Error($"Метод: {ex.TargetSite}");
                Log.Error($"Трассировка стека: {ex.StackTrace}");
                return 1;
            }

        }

        /// <summary>  
        ///  Обработка реквизитов шапки
        /// </summary>  
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

                    // Проверка - если это группа (справочника), то пропускаем анализ +
                    if (Sdata["#value"]["IsFolder"] == true)
                        continue;
                    // Проверка на группу -  

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
                                    // Если происходит добавление перед таб частью, а таб части в элемента нет, тогда добавляем в конец
                                    if (Sdata["#value"].Property(Bef.ToString()) != null)
                                    {
                                        Sdata["#value"].Property(Bef.ToString()).AddBeforeSelf(new JProperty(SourceName.ToString(), TypeData));
                                    }
                                    else
                                    {
                                        Sdata["#value"].Add(new JProperty(SourceName.ToString(), TypeData));
                                    }
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
                                    // Если происходит перенос перед таб частью, а таб части в элемента нет, тогда переносим в конец 
                                    if (Sdata["#value"].Property(Bef.ToString()) != null)
                                    {
                                        Sdata["#value"].Property(Bef.ToString()).AddBeforeSelf(new JProperty(SourceName.ToString(), exValue));
                                    }
                                    else
                                    {
                                        Sdata["#value"].Add(new JProperty(SourceName.ToString(), exValue));
                                    }

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

        // private void ParseTabPartWhithMultiValue()
        // {

        //     // Разбор правил табличных частей
        //     NameRules = "ТабличныеЧасти";

        //     // Список табличных частей

        //     int x = 0;


        //     JToken RlsTab = CurRulesTab.SelectToken("$.#value[?(@.name.#value == " + "'" + NameRules + "'" + " )]");
        //     if (RlsTab != null) // Если описание табличных частей не отсутствует
        //     {
        //         foreach (var curR in RlsTab["Value"]["#value"])
        //         {

        //             Console.WriteLine(curR["#value"] + " Наименование таб части");
        //             string CurTabCh = curR["#value"].ToString();
        //             JToken RlsRk = CurRulesTab.SelectToken("$.#value[?(@.name.#value == " + "'" + curR["#value"].ToString() + "'" + " )]");
        //             x++;
        //             foreach (var CurObj in RlsRk["Value"]["#value"])
        //             {
        //                 //            Console.WriteLine(CurObj);

        //                 JToken Action = CurObj.SelectToken("$.Value.#value[?(@.name.#value == 'Действие' )].Value.#value");
        //                 JToken SourceName = CurObj.SelectToken("$.Value.#value[?(@.name.#value == 'ИмяСвойстваИсточник' )].Value.#value");
        //                 JToken RecName = CurObj.SelectToken("$.Value.#value[?(@.name.#value == 'ИмяСвойстваПриемник' )].Value.#value");
        //                 JToken TypeName = CurObj.SelectToken("$.Value.#value[?(@.name.#value == 'ТипСтрокойПриемник' )].Value.#value");
        //                 JToken Order = CurObj.SelectToken("$.Value.#value[?(@.name.#value == 'Порядок' )].Value.#value");
        //                 JToken Bef = CurObj.SelectToken("$.Value.#value[?(@.name.#value == 'Перед' )].Value.#value");

        //                 TypeData = GetTypeValue(TypeName.ToString());
        //                 Console.WriteLine(TypeData);


        //                 JObject AddingToken = new JObject
        //                     {
        //                         { "#type", TypeName },
        //                         { "#value", TypeData.ToString() }

        //                     };


        //                 dynamic TabC = stuff.SelectToken("$..#value[" + "'" + CurTabCh + "'" + "] ");
        //                 if (TabC != null)
        //                 {


        //                     foreach (var Sdata in TabC)
        //                     {

        //                         //  Console.WriteLine((Sdata["#value"]["Ref"]));
        //                         string tact = Action.ToString();

        //                         switch (tact)
        //                         {
        //                             case "Пропустить":
        //                                 // Sdata["#value"].Property(SourceName.ToString()).Remove();
        //                                 Sdata.Property(SourceName.ToString()).Remove();
        //                                 break;
        //                             case "Добавить":
        //                                 if (Bef.ToString() == "")
        //                                 {
        //                                     try
        //                                     {
        //                                         Sdata.Add(new JProperty(SourceName.ToString(), AddingToken));
        //                                     }
        //                                     catch (System.Exception ex)
        //                                     {
        //                                         Log.Error($"Исключение: {ex.Message}");
        //                                         Log.Error($"Метод: {ex.TargetSite}");
        //                                         Log.Error($"Трассировка стека: {ex.StackTrace}");
        //                                     }

        //                                 }
        //                                 else
        //                                 {
        //                                     Sdata.Property(Bef.ToString()).AddBeforeSelf(new JProperty(SourceName.ToString(), AddingToken));
        //                                 }

        //                                 break;
        //                             case "Переименовать":

        //                                 try
        //                                 {
        //                                     Sdata.Property(SourceName.ToString()).Replace(new JProperty(RecName.ToString(), Sdata[SourceName.ToString()]));
        //                                 }
        //                                 catch (System.Exception ex)
        //                                 {

        //                                     Log.Error($"Исключение: {ex.Message}");
        //                                     Log.Error($"Метод: {ex.TargetSite}");
        //                                     Log.Error($"Трассировка стека: {ex.StackTrace}");
        //                                 }

        //                                 break;
        //                             default:

        //                                 break;
        //                         }

        //                         //     }
        //                     }
        //                 }
        //                 else
        //                 {
        //                     Log.Warning("В файле данных " + NameRules + " отсутствует информация от табличных частях!");
        //                 }
        //             }

        //         }
        //     }

        // }
        /// <summary>
        /// Обработка табличных частей содержащих составные реквизиты 
        /// </summary>
        private void ParseTabPartWhithMultiValue()
        {

            // Разбор правил табличных частей
            NameRules = "ТабличныеЧасти";

            // Список табличных частей

            int x = 0;


            JToken RlsTab = CurRulesTab.SelectToken("$.#value[?(@.name.#value == " + "'" + NameRules + "'" + " )]");
            if (RlsTab != null) // Если описание табличных частей не отсутствует
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


                        JObject AddingToken = new JObject
                            {
                                { "#type", TypeName },
                                { "#value", TypeData.ToString() }

                            };


                        var TabC = stuff.SelectTokens("$..#value[" + "'" + CurTabCh + "'" + "] ");
                        if (TabC != null)
                        {


                            foreach (var Sdata in TabC)
                            {

                                //  Console.WriteLine((Sdata["#value"]["Ref"]));
                                string tact = Action.ToString();

                                switch (tact)
                                {
                                    case "Пропустить":
                                        if (Sdata != null)//
                                        {
                                            for (int i = 0; i < Sdata.Count; i++)
                                            {
                                                Sdata[i].Property(SourceName.ToString()).Remove();
                                            }

                                        }
                                        else
                                        {
                                            Sdata.Property(SourceName.ToString()).Remove();
                                        }

                                        break;
                                    case "Добавить":
                                        if (Bef.ToString() == "")
                                        {
                                            try
                                            {
                                                for (int i = 0; i < Sdata.Count; i++)
                                                {
                                                    Sdata[i].Add(new JProperty(SourceName.ToString(), AddingToken));
                                                }
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
                                            for (int i = 0; i < Sdata.Count; i++)
                                            {
                                                Sdata[i].Property(Bef.ToString()).AddBeforeSelf(new JProperty(SourceName.ToString(), AddingToken));
                                            }
                                        }


                                        break;
                                    case "Переименовать":

                                        try
                                        {
                                            for (int i = 0; i < Sdata.Count; i++)
                                            {
                                                Sdata[i].Property(SourceName.ToString()).Replace(new JProperty(RecName.ToString(), Sdata[SourceName.ToString()]));
                                            }
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
                        var TabC = stuff.SelectTokens("$..#value[" + "'" + CurTabCh + "'" + "] ");
                        if (TabC != null)
                        {


                            foreach (var Sdata in TabC)
                            {

                                //  Console.WriteLine((Sdata["#value"]["Ref"]));
                                string tact = Action.ToString();

                                switch (tact)
                                {
                                    case "Пропустить":
                                        if (Sdata != null)//
                                        {
                                            for (int i = 0; i < Sdata.Count; i++)
                                            {
                                                Sdata[i].Property(SourceName.ToString()).Remove();
                                            }

                                        }
                                        else
                                        {
                                            Sdata.Property(SourceName.ToString()).Remove();
                                        }
                                        break;
                                    case "Добавить":
                                        if (Bef.ToString() == "")
                                        {
                                            try
                                            {
                                                for (int i = 0; i < Sdata.Count; i++)
                                                {
                                                    Sdata[i].Add(new JProperty(SourceName.ToString(), TypeData));
                                                }
                                                //  Sdata.Add(new JProperty(SourceName.ToString(), TypeData));
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
                                            for (int i = 0; i < Sdata.Count; i++)
                                            {
                                                Sdata[i].Property(Bef.ToString()).AddBeforeSelf(new JProperty(SourceName.ToString(), TypeData));
                                            }

                                        }

                                        break;
                                    case "Переименовать":

                                        try
                                        {
                                            for (int i = 0; i < Sdata.Count; i++)
                                            {
                                                Sdata[i].Property(SourceName.ToString()).Replace(new JProperty(RecName.ToString(), Sdata[i][SourceName.ToString()]));
                                            }
                                            //   Sdata.Property(SourceName.ToString()).Replace(new JProperty(RecName.ToString(), Sdata[SourceName.ToString()]));
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
        /// <summary>
        ///  Формирует значение для подстановки в элемент на основании типа
        /// </summary>
        /// <param name="Act">тип значения</param>
        /// <returns>значение</returns>
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
                case "Дата":
                    return "0001-01-01T00:00:00";
                case "Неопределено":
                    return null;
                case "":
                    return "";
                default:
                    return "00000000-0000-0000-0000-000000000000";

            }


        }

    }
}