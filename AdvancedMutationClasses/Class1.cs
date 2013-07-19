using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMutationClasses
{
    [Serializable]
    public class Actions
    {
        public enum Action
        {
            M_UP,
            M_DOWN,
            M_LEFT,
            M_RIGHT,
            NONE
        }

        public enum AssertValues
        {
            PlayerX,
            PlayerY
        }

        public enum AssertActions
        {
            Check_Passable
        }

        public delegate int Assert ( int a, int b );
    }

    [Serializable]
    public class Action
    {
        public Actions.Action action;
        public object val;
    }

    [Serializable]
    public class Knowledge
    {
        public int top;
        public int down;
        public int left;
        public int right;

        public Actions.Action action;

        public int score = 0;

        public Knowledge ()
        {
        }

        public Knowledge ( Knowledge parent )
        {
            this.top = parent.top;
            this.down = parent.down;
            this.left = parent.left;
            this.right = parent.right;
            this.score = parent.score;
            this.action = parent.action;
        }

        public Knowledge ( int top, int down, int left, int right, Actions.Action action )
        {
            this.top = top;
            this.down = down;
            this.left = left;
            this.right = right;
            this.action = action;
        }

    }

    [Serializable]
    public class Generation
    {
        public List<Action> actions;
        public List<Knowledge> knowledge;
        public int score = 0;
        public double dist = 0;
        public double fitness
        {
            get
            {
                double distFitness = 1.0 - ( dist / Math.Sqrt(2048) ); // .9375  // 
                //double scoreFitness = score / ( actions.Count == 0 ? 1 : actions.Count );  // 1.1470

                return distFitness;
            }
        }
        public int generationid;

        public Generation ()
        {
            this.actions = new List<Action>();
            this.knowledge = new List<Knowledge>();
            this.generationid = -1;
        }

        public Generation ( Generation parent )
        {
            //this.actions = new List<Action>(parent.actions);
            this.knowledge = new List<Knowledge>(parent.knowledge);
            this.generationid = parent.generationid;
        }

        public Knowledge[] MakeDecision ( int top, int down, int left, int right )
        {
            List<Knowledge> kns = new List<Knowledge>();

            foreach ( Knowledge k in this.knowledge )
            {
                if ( k.top == top && k.down == down && k.left == left && k.right == right )
                {
                    if ( k.score > -10 )
                    {
                        kns.Add(k);
                    }
                }
            }

            if ( kns.Count > 0 )
            {
                kns.Sort(KnowledgeSorter);
                return kns.ToArray();
            }

            return null;

        }

        public void Reward ( Knowledge k )
        {
            if ( this.knowledge.Contains(k) )
            {
                this.knowledge[this.knowledge.IndexOf(k)].score++;
            }
        }

        public void Punish ( Knowledge k )
        {
            if ( this.knowledge.Contains(k) )
            {
                int idx=    this.knowledge.IndexOf(k);
                this.knowledge[idx].score--;
                if ( this.knowledge[idx].score <= -10 )
                {
                    this.knowledge.Remove(this.knowledge[idx]);
                }
            }
        }

        public int KnowledgeSorter ( Knowledge a, Knowledge b )
        {
            return ( a.score < b.score ? 1 : ( a.score == b.score ? 0 : -1 ) );
        }

        // Crossover
        public Generation ( Generation parent1, Generation parent2 )
        {
            Random r = new Random();

            if ( parent1.knowledge.Count == 0 || parent2.knowledge.Count == 0 )
            {
                if ( parent1.knowledge.Count == 0 && parent1.knowledge.Count == 0 )
                {
                    this.knowledge = new List<Knowledge>();
                }
                else if ( parent1.actions.Count == 0 )
                    this.knowledge = new List<Knowledge>(parent2.knowledge);
                else
                    this.knowledge = new List<Knowledge>(parent1.knowledge);

                return;
            }

            Generation lowest = parent1.knowledge.Count > parent2.knowledge.Count ? new Generation(parent2) : new Generation(parent1);
            Generation highest = parent1.knowledge.Count > parent2.knowledge.Count ? new Generation(parent1) : new Generation(parent2);

            //int parts = (int)Math.Min(lowest.actions.Count, 10);

            this.knowledge = new List<Knowledge>();
            this.generationid = highest.generationid + 1;

            for ( int i = 0; i < highest.knowledge.Count; i++ )
            {
                if ( i < lowest.knowledge.Count )
                {
                    switch ( r.Next(2) )
                    {
                        case 0:
                            this.knowledge.Add(lowest.knowledge[i]);
                            break;
                        case 1:
                            this.knowledge.Add(highest.knowledge[i]);
                            break;
                    }
                }
                else
                    this.knowledge.Add(highest.knowledge[i]);
            }
        }
    }
}
