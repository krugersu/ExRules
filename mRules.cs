
using System.Collections.Generic;

namespace ExRules
{

    struct RulesProperty
    {
        public string NameProperty;
        public string PropertyNameSource;
        public string PropertyNameDestination;
        public string TypeStringDestination;
        public string Action;
        public string Order;
        public string Before;
    }

    internal class mRules

    {
        public string NameRules;
        public bool MultiValue;
        List<RulesProperty> listRules;
    }

}