
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RulesEditor
{

    struct RulesProperty
    {
        /// <summary>
        /// Имя реквизита
        /// </summary>
        /// <value> String</value>
        [JsonProperty("Имя реквизита")]
        public string NameProperty { get; set; }
        /// <summary>
        /// ИмяСвойстваИсточник
        /// </summary>
        /// <value>String</value>
        [JsonProperty("Имя свойства источника")]
        public string PropertyNameSource { get; set; }
        /// <summary>
        /// ИмяСвойстваПриемник
        /// </summary>
        /// <value>String</value>
        [JsonProperty("Имя свойства приемника")]
        public string PropertyNameDestination { get; set; }
        /// <summary>
        /// ТипСтрокойПриемник
        /// </summary>
        /// <value>String</value>
        [JsonProperty("Тип строкой приемник")]
        public string TypeStringDestination { get; set; }
        /// <summary>
        /// Действие
        /// </summary>
        /// <value>String</value>
        [JsonProperty("Действие")]
        public string Action { get; set; }
        /// <summary>
        /// Порядок
        /// </summary>
        /// <value>Int</value>
        [JsonProperty("Порядок")]
        public int Order { get; set; }
        /// <summary>
        /// Перед
        /// </summary>
        /// <value>String</value>
        [JsonProperty("Перед")]
        public string Before { get; set; }
    }

    internal class mRules

    {
        /// <summary>
        /// Имя правила
        /// </summary>
        /// <value>String</value>
        [JsonProperty("Имя объекта")]
        public string NameRules { get; set; }
        /// <summary>
        /// Есть составной реквизит
        /// </summary>
        /// <value>bool</value>
        [JsonProperty("Есть составной реквизит")]
        public bool MultiValue { get; set; }
        /// <summary>
        /// Список структур с правилами для реквизитов
        /// </summary>
        /// <typeparam name="RulesProperty"></typeparam>
        /// <returns>Структура с правилами</returns>
        [JsonProperty("Список правил")]
        public List<RulesProperty> listRules { get; set; } = new List<RulesProperty>();


    }

    internal class mRulesTabPart

    {
        /// <summary>
        /// Имя правила
        /// </summary>
        /// <value>String</value>
        public string NameRules { get; set; }
        /// <summary>
        /// Есть составной реквизит
        /// </summary>
        /// <value>bool</value>
        public bool MultiValue { get; set; }
        /// <summary>
        /// Список структур с правилами для реквизитов
        /// </summary>
        /// <typeparam name="RulesProperty"></typeparam>
        /// <returns>Структура с правилами</returns>
        public List<RulesProperty> listRules { get; set; } = new List<RulesProperty>();


    }

}