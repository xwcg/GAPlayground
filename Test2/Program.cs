using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MutationClasses;
                                     
namespace Test2
{                    
    class Program
    {
        static Generation lastGen = new Generation();

        static void Main ( string[] args )
        {
            BinaryFormatter deserialzer = new BinaryFormatter();
            FileStream readStream  = File.OpenRead("gen.gen");
            lastGen = (Generation)deserialzer.Deserialize(readStream);
            readStream.Close();
            readStream.Dispose();

            int goal = 4242;
            int mutations = 5;
            Random r = new Random();
            int lastGenResult = RunGen(lastGen);

            // 100k generations
            for ( int mi = 0; mi < 100000; mi++ )
            {
                Generation newGen = new Generation(lastGen);

                for ( int i = 0; i < mutations; i++ )
                {
                    switch ( r.Next(3) )
                    {
                        case 0: // Modify action
                            Console.Write("m");

                            int oldActionIndex = r.Next(newGen.actions.Count);
                            switch ( r.Next(1, 6) )
                            {
                                case 1:
                                    newGen.actions[oldActionIndex].action = Actions.Action.ADD;
                                    break;
                                case 2:
                                    newGen.actions[oldActionIndex].action = Actions.Action.SUB;
                                    break;
                                case 3:
                                    newGen.actions[oldActionIndex].action = Actions.Action.DIV;
                                    break;
                                case 4:
                                    newGen.actions[oldActionIndex].action = Actions.Action.MULT;
                                    break;
                                case 5:
                                    break;
                            }
                            newGen.actions[oldActionIndex].val = r.Next(0, goal + 1);
                            break;

                        case 1: // Remove action   
                            if ( newGen.actions.Count <= 1 )
                                break;
                            Console.Write("r");

                            int oldActionRIndex = r.Next(newGen.actions.Count);
                            newGen.actions.RemoveAt(oldActionRIndex);
                            break;

                        case 2: // Do Nothing    
                            Console.Write("n");
                            break;
                    }
                }

                int newGenResult = RunGen(newGen);
                int distNew = Math.Abs(goal - newGenResult);
                int distOld = Math.Abs(goal - lastGenResult);

                Console.WriteLine(" - {0} / {1}", newGen.actions.Count, lastGen.actions.Count);
                lastGen.generationid++;


                if ( newGenResult == goal && newGen.actions.Count == 1 )
                {
                    Console.WriteLine("Goal Reached! Printing Genetics:");
                    string genstr = "";
                    foreach ( MutationClasses.Action a in newGen.actions )
                    {
                        //Console.WriteLine("{0} with {1}", a.action.ToString(), a.val);
                        switch ( a.action )
                        {
                            case Actions.Action.ADD:
                                genstr += "A";
                                break;
                            case Actions.Action.SUB:
                                genstr += "S";
                                break;
                            case Actions.Action.DIV:
                                genstr += "D";
                                break;
                            case Actions.Action.MULT:
                                genstr += "M";
                                break;
                        }
                        genstr += a.val;
                    }
                    lastGen = newGen;
                    Console.WriteLine(genstr);
                    Console.WriteLine("Generation ID: {0}", newGen.generationid);
                    break;
                }

                if ( newGenResult == goal && newGen.actions.Count < lastGen.actions.Count )
                {
                    lastGen = newGen;
                    lastGenResult = RunGen(lastGen);
                    Console.WriteLine("Better Generation! Result: {0}, Actions: {1}", newGenResult, newGen.actions.Count);
                }
            }

            BinaryFormatter f = new BinaryFormatter();
            Stream genSave = File.Create("gen.gen");
            f.Serialize(genSave, lastGen);
            genSave.Flush();
            genSave.Close();
            genSave.Dispose();

            Console.WriteLine("Saved gen as gen.gen");

            Console.ReadKey();
        }

        static int RunGen ( Generation g )
        {
            int result = 0;

            foreach ( MutationClasses.Action a in g.actions )
            {
                switch ( a.action )
                {
                    case Actions.Action.ADD:
                        result += a.val;
                        break;
                    case Actions.Action.SUB:
                        result -= a.val;
                        break;
                    case Actions.Action.DIV:
                        if ( a.val == 0 )
                        {
                            return -1;
                        }

                        result /= a.val;
                        break;
                    case Actions.Action.MULT:
                        result *= a.val;
                        break;
                }
            }

            return result;
        }
    }
}
