using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MutationClasses;

namespace MutationTests
{
    class Program
    {
        static Generation lastGen = new Generation();

        static void Main ( string[] args )
        {
            Console.WriteLine("Calculation Goal:");
            int goal = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Mutation Percentage (1-100):");
            int mutinp = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Max Genes:");
            int maxGenes = Convert.ToInt32(Console.ReadLine());

            int mutations = Convert.ToInt32(Math.Floor(( (double)maxGenes * ( (double)mutinp / 100 ) )));

            Console.WriteLine("Max Genes: {0}, Mutating Genes: {1}, Goal: {2}", maxGenes, mutations, goal);
            Console.WriteLine("Press Any Key To Start Evolution");
            Console.ReadKey();

            Random r = new Random();
            int lastGenResult = RunGen(lastGen);

            // 100k generations
            for ( int mi = 0; mi < 1000000; mi++ )
            {
                Generation newGen = new Generation(lastGen);

                int thismaxMut = r.Next(mutations);

                for ( int i = 0; i < thismaxMut; i++ )
                {
                    int next = r.Next(4);

                    if ( newGen.actions.Count == maxGenes )
                    {
                        switch ( next )
                        {
                            case 0:
                            case 1:  // Modify
                                if ( newGen.actions.Count == 0 )
                                    break;
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
                            case 2: // Remove
                                if ( newGen.actions.Count == 0 )
                                    break;
                                Console.Write("r");

                                int oldActionRIndex = r.Next(newGen.actions.Count);
                                newGen.actions.RemoveAt(oldActionRIndex);
                                break;
                            case 3: // Do Nothing    
                                Console.Write("n");
                                break;

                        }
                    }
                    else
                    {
                        switch ( next )
                        {
                            case 0:  // New action
                                Console.Write("a");
                                MutationClasses.Action newAction = new MutationClasses.Action();
                                switch ( r.Next(1, 5) )
                                {
                                    case 1:
                                        newAction.action = Actions.Action.ADD;
                                        break;
                                    case 2:
                                        newAction.action = Actions.Action.SUB;
                                        break;
                                    case 3:
                                        newAction.action = Actions.Action.DIV;
                                        break;
                                    case 4:
                                        newAction.action = Actions.Action.MULT;
                                        break;
                                }

                                newAction.val = r.Next(0, goal + 1);

                                newGen.actions.Add(newAction);
                                break;

                            case 1: // Modify action
                                if ( newGen.actions.Count == 0 )
                                    break;
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

                            case 2: // Remove action   
                                if ( newGen.actions.Count == 0 )
                                    break;
                                Console.Write("r");

                                int oldActionRIndex = r.Next(newGen.actions.Count);
                                newGen.actions.RemoveAt(oldActionRIndex);
                                break;

                            case 3: // Do Nothing    
                                Console.Write("n");
                                break;
                        }
                    }
                }

                int newGenResult = RunGen(newGen);
                int distNew = Math.Abs(goal - newGenResult);
                int distOld = Math.Abs(goal - lastGenResult);

                Console.WriteLine(" - {0}({2}) / {1}", distNew, distOld, newGenResult);
                lastGen.generationid++;


                if ( distNew == 0 )
                {
                    Console.WriteLine("Goal Reached! Printing Genetics: ({0} Genes)", newGen.actions.Count);
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
                    Console.WriteLine(genstr);
                    lastGen = newGen;
                    Console.WriteLine("Generation ID: {0}", newGen.generationid);
                    break;
                }

                if ( newGenResult > 0 && distNew < distOld )
                {
                    lastGen = newGen;
                    lastGenResult = RunGen(lastGen);
                    Console.WriteLine("Better Generation! New Result: {0}, New Distance: {1}", newGenResult, distNew);
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
