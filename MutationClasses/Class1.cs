using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutationClasses
{
    [Serializable]
    public class Actions
    {
        public enum Action
        {
            ADD,
            SUB,
            DIV,
            MULT
        }


    }

    [Serializable]
    public class Action
    {
        public Actions.Action action;
        public int val;
    }

    [Serializable]
    public class Generation
    {
        public List<Action> actions;
        public int generationid;

        public Generation ()
        {
            this.actions = new List<Action>();
            this.generationid = -1;
        }

        public Generation ( Generation parent )
        {
            this.actions = new List<Action>(parent.actions);
            this.generationid = parent.generationid;
        }
    }
}
