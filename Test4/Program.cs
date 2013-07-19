using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMutationClasses;

namespace Test4
{
    class Program
    {
        static int[,] Maze = new int[32, 32];

        static List<Generation> Gens = new List<Generation>();
        static Random r = new Random();
        static int mutations = 5;

        static void Main ( string[] args )
        {
            LoadMaze();

            Console.WindowHeight = 32;



            // generate base generations
            for ( var i =0; i < 16; i++ )
            {
                Generation og = new Generation();
                //Mutate(ref og);
                Run(ref og);

                Gens.Add(og);
            }


            int round = 1;
            while ( true )
            {
                Console.CursorTop = 2;
                Console.CursorLeft = 40;
                Console.Write("Round: {0}     ", round);

                Console.CursorTop = 1;
                Console.CursorLeft = 40;
                Console.Write("         ");

                for ( var k = 0; k < Gens.Count; k++ )
                {
                    Generation g = new Generation(Gens[k]);

                    Console.CursorTop = 3;
                    Console.CursorLeft = 40;
                    Console.Write("Item: {0}/{1}     ", k + 1, Gens.Count);

                    Console.CursorTop = 4;
                    Console.CursorLeft = 40;
                    Console.Write("Generation: {0}     ", g.generationid);

                    Console.CursorTop = 10 + k;
                    Console.CursorLeft = 40;
                    Console.Write("*{0}: {1} ({2})          ", k + 1, Gens[k].score, Math.Round(Gens[k].fitness, 4));

                    // Mutate
                    //Mutate(ref g);

                    // Run
                    Run(ref g);

                    if ( g.dist == 0 )
                    {
                        Console.CursorTop = 30;
                        Console.CursorLeft = 40;
                        Console.Write("Done!");
                        Console.ReadLine();
                    }

                    //if ( g.score > oldGen.score && g.dist < oldGen.dist )
                    //if ( Math.Abs(g.score - oldGen.score) < 10 && g.dist < oldGen.dist )
                    if ( g.fitness > Gens[k].fitness ) // && g.score > Gens[k].score )
                    {
                        Gens[k] = g;
                        //Thread.Sleep(200);
                    }

                    Console.CursorTop = 10 + k;
                    Console.CursorLeft = 40;
                    Console.Write("{0}: {1} ({2})          ", k + 1, Gens[k].score, Math.Round(Gens[k].fitness, 4));
                }


                Gens.Sort(GeneticsSorter); // Sort by fitness

                // Scores
                for ( var k = 0; k < Gens.Count; k++ )
                {
                    Console.CursorTop = 10 + k;
                    Console.CursorLeft = 40;
                    Console.Write("{0}: {1} ({2})          ", k + 1, Gens[k].score, Math.Round(Gens[k].fitness, 4));
                }


                // Genetic Crossover & Mutation

                Console.CursorTop = 1;
                Console.CursorLeft = 40;
                Console.Write("BREEDING");
                Console.CursorTop = 2;
                Console.CursorLeft = 40;
                Console.Write("                    ");
                Console.CursorTop = 3;
                Console.CursorLeft = 40;
                Console.Write("                    ");
                Console.CursorTop = 4;
                Console.CursorLeft = 40;
                Console.Write("                    ");

                List<Generation> newGens = new List<Generation>();
                //for ( var k = 0; k < Gens.Count; k += 2 )
                //{
                //    Generation ng1 = new Generation(Gens[k], Gens[k + 1]);
                //    Generation ng2 = new Generation(Gens[k], Gens[k + 1]);

                //    Mutate(ref ng1);
                //    Mutate(ref ng2);

                //    Run(ref ng1);
                //    Run(ref ng2);

                //    newGens.Add(ng1);
                //    newGens.Add(ng2);
                //}

                // Alternate breeding strategy
                //for ( var k = 1; k < ( Gens.Count / 2 ) + 1; k++ )
                //{
                //    Generation ng1 = new Generation(Gens[0], Gens[k]);
                //    //Generation ng2 = new Generation(Gens[0], Gens[k]);

                //    Mutate(ref ng1);
                //    //Mutate(ref ng2);

                //    Run(ref ng1, false);
                //    //Run(ref ng2);

                //    newGens.Add(ng1);
                //}

                //for ( var k = 0; k < ( Gens.Count / 2 ) + 1; k++ )
                //{
                //    if ( k == 1 )
                //        continue;

                //    Generation ng1 = new Generation(Gens[1], Gens[k]);
                //    //Generation ng2 = new Generation(Gens[0], Gens[k]);

                //    Mutate(ref ng1);
                //    //Mutate(ref ng2);

                //    Run(ref ng1, false);
                //    //Run(ref ng2);

                //    newGens.Add(ng1);
                //}


                for ( var k = 0; k < Gens.Count; k += 2 )
                {
                    Generation ng1 = new Generation(Gens[k], Gens[k + 1]);
                    Generation ng2 = new Generation(Gens[k], Gens[k + 1]);

                    //Mutate(ref ng1);
                    //Mutate(ref ng2);

                    //Run(ref ng1, false);
                    //Run(ref ng2, false);

                    newGens.Add(ng1);
                    newGens.Add(ng2);
                }

                Gens = newGens;

                // Scores
                //for ( var k = 0; k < Gens.Count; k++ )
                //{
                //    Console.CursorTop = 10 + k;
                //    Console.CursorLeft = 40;
                //    Console.Write("{0}: {1} ({2})          ", k + 1, Gens[k].score, Math.Round(Gens[k].fitness, 4));
                //}

                round++;
            }

            Console.ReadKey();
        }

        private static int GeneticsSorter ( Generation g1, Generation g2 )
        {
            return ( g1.fitness < g2.fitness ? 1 : ( g1.fitness == g2.fitness ? 0 : -1 ) );
            //return ( g1.score < g2.score ? 1 : ( g1.score == g2.score ? 0 : -1 ) );
        }

        static void Mutate ( ref Generation g )
        {
            int muts = (int)Math.Floor((double)( (double)g.knowledge.Count * (double)( (double)mutations / 100 ) ));
            bool onlyAdd = false;

            if ( muts == 0 )
            {
                muts = 2;
                onlyAdd = true;
            }

            for ( var m = 0; m < muts; m++ )
            {
                int next = onlyAdd ? 0 : r.Next(6);

                switch ( next )
                {
                    case 0: // Add

                        break;
                    case 1: // Del   
                    case 2:
                        if ( g.actions.Count == 0 )
                            break;

                        g.actions.RemoveAt(r.Next(g.actions.Count));
                        break;
                    case 3: // Change    
                    case 4:
                        if ( g.actions.Count == 0 )
                            break;

                        switch ( r.Next(4) )
                        {
                            case 0:
                                g.actions[r.Next(g.actions.Count)].action = Actions.Action.M_DOWN;
                                break;
                            case 1:
                                g.actions[r.Next(g.actions.Count)].action = Actions.Action.M_UP;
                                break;
                            case 2:
                                g.actions[r.Next(g.actions.Count)].action = Actions.Action.M_LEFT;
                                break;
                            case 3:
                                g.actions[r.Next(g.actions.Count)].action = Actions.Action.M_RIGHT;
                                break;
                        }
                        break;
                    case 5: // Nothing
                        break;
                }
            }
        }

        static void LoadMaze ()
        {
            Bitmap mzbmp = new Bitmap("maze.bmp");

            for ( int x = 0; x < mzbmp.Width; x++ )
            {
                for ( int y = 0; y < mzbmp.Height; y++ )
                {
                    Color c = mzbmp.GetPixel(x, y);
                    Maze[x, y] = c.B;
                }
            }
        }

        static void Run ( ref Generation g )
        {
            Run(ref g, true);
        }

        static Knowledge MakeKnowledge ( int top, int down, int left, int right )
        {
            Knowledge nk = new Knowledge();

            nk.top = top;
            nk.down = down;
            nk.left = left;
            nk.right = right;

            switch ( r.Next(4) )
            {
                case 0:
                    nk.action = Actions.Action.M_DOWN;
                    break;
                case 1:
                    nk.action = Actions.Action.M_UP;
                    break;
                case 2:
                    nk.action = Actions.Action.M_LEFT;
                    break;
                case 3:
                    nk.action = Actions.Action.M_RIGHT;
                    break;
            }

            return nk;
        }

        struct Pos
        {
            public int x;
            public int y;

            public Pos ( int x, int y )
            {
                this.x = x;
                this.y = y;
            }
        }

        static bool WasHere ( ref List<Pos> poses, int x, int y )
        {
            Pos CheckPos = new Pos(x, y);
            if ( poses.Contains(CheckPos) )
                return true;

            return false;
        }

        static void Run ( ref Generation g, bool draw )
        {
            int score = 0;
            int pX = 0;
            int pY = 0;
            double lastDist = Distance(pX, pY);
            int actionid = 0;
            int colls = 0;
            if ( draw )
                DrawMaze(pX, pY);

            List<Pos> prevPos = new List<Pos>();

            #region KnowledgeLoop
            while ( colls <= 5 )
            {
                Knowledge[] kns = g.MakeDecision(GetMazePos(pX, pY - 1),
                    GetMazePos(pX, pY + 1),
                    GetMazePos(pX - 1, pY),
                    GetMazePos(pX + 1, pY));

                if ( kns == null )
                {
                    g.knowledge.Add(MakeKnowledge(GetMazePos(pX, pY - 1),
                    GetMazePos(pX, pY + 1),
                    GetMazePos(pX - 1, pY),
                    GetMazePos(pX + 1, pY)));
                    continue;
                }

                foreach ( Knowledge k in kns )
                {
                    Actions.Action a = k.action;

                    switch ( a )
                    {
                        case Actions.Action.M_UP:
                            if ( IsPassable(pX, pY - 1) )
                            {
                                pY--;
                                colls = 0;

                                if ( WasHere(ref prevPos, pX, pY) )
                                    g.Punish(k);
                                else
                                    g.Reward(k);

                                prevPos.Add(new Pos(pX, pY));
                                break;
                            }
                            else
                            {
                                colls++;
                                g.Punish(k);
                                g.knowledge.Add(MakeKnowledge(GetMazePos(pX, pY - 1),
                                    GetMazePos(pX, pY + 1),
                                    GetMazePos(pX - 1, pY),
                                    GetMazePos(pX + 1, pY)));
                                continue;
                            }
                            break;
                        case Actions.Action.M_DOWN:
                            if ( IsPassable(pX, pY + 1) )
                            {
                                pY++;
                                colls = 0;

                                if ( WasHere(ref prevPos, pX, pY) )
                                    g.Punish(k);
                                else
                                    g.Reward(k);

                                prevPos.Add(new Pos(pX, pY));
                                break;
                            }
                            else
                            {
                                colls++;
                                g.Punish(k);
                                g.knowledge.Add(MakeKnowledge(GetMazePos(pX, pY - 1),
                                    GetMazePos(pX, pY + 1),
                                    GetMazePos(pX - 1, pY),
                                    GetMazePos(pX + 1, pY)));
                                continue;
                            }
                            break;
                        case Actions.Action.M_LEFT:
                            if ( IsPassable(pX - 1, pY) )
                            {
                                pX--;
                                colls = 0;

                                if ( WasHere(ref prevPos, pX, pY) )
                                    g.Punish(k);
                                else
                                    g.Reward(k);

                                prevPos.Add(new Pos(pX, pY));
                                break;
                            }
                            else
                            {
                                colls++;
                                g.Punish(k);
                                g.knowledge.Add(MakeKnowledge(GetMazePos(pX, pY - 1),
                                    GetMazePos(pX, pY + 1),
                                    GetMazePos(pX - 1, pY),
                                    GetMazePos(pX + 1, pY)));
                                continue;
                            }
                            break;
                        case Actions.Action.M_RIGHT:
                            if ( IsPassable(pX + 1, pY) )
                            {
                                pX++;
                                colls = 0;

                                if ( WasHere(ref prevPos, pX, pY) )
                                    g.Punish(k);
                                else
                                    g.Reward(k);

                                prevPos.Add(new Pos(pX, pY));
                                break;
                            }
                            else
                            {
                                colls++;
                                g.Punish(k);
                                g.knowledge.Add(MakeKnowledge(GetMazePos(pX, pY - 1),
                                    GetMazePos(pX, pY + 1),
                                    GetMazePos(pX - 1, pY),
                                    GetMazePos(pX + 1, pY)));
                                continue;
                            }
                            break;
                    }
                }

                double newDist = Distance(pX, pY);

                if ( draw )
                {
                    Console.CursorTop = 5;
                    Console.CursorLeft = 40;
                    Console.Write("Distance: {0}     ", Math.Round(newDist, 2));
                    //Console.CursorTop = 6;
                    //Console.CursorLeft = 40;
                    //Console.Write("Action: {0}/{1}    ", actionid, g.actions.Count);
                    Console.CursorTop = 7;
                    Console.CursorLeft = 40;
                    Console.Write("Score: {0}    ", score);
                    Console.CursorTop = 8;
                    Console.CursorLeft = 40;
                    Console.Write("Fitness: {0}            ", Math.Round(g.fitness, 2));
                }

                if ( newDist < lastDist )
                    score++;
                else if ( newDist > lastDist )
                    score -= 2;
                else if ( colls > 1 )
                    score -= 4;

                g.score = score;
                g.dist = newDist;

                if ( draw )
                {
                    DrawMaze(pX, pY);
                    Thread.Sleep(200);
                }
            }
            #endregion

            #region ActionLoop
            /*
            foreach ( AdvancedMutationClasses.Action a in g.actions )
            {
                actionid++;
                switch ( a.action )
                {
                    case Actions.Action.M_UP:
                        if ( IsPassable(pX, pY - 1) )
                        {
                            pY--;
                            colls = 0;
                        }
                        else
                        {
                            //Knowledge k = new Knowledge();
                            //k.assertion = Actions.Action.M_UP;
                            //k.assertA = pX;
                            //k.assertB = pY;
                            //k.assert = GetMazePos;
                            //k.prefer
                            colls++;
                        }
                        break;
                    case Actions.Action.M_DOWN:
                        if ( IsPassable(pX, pY + 1) )
                        {
                            pY++;
                            colls = 0;
                        }
                        else
                            colls++;
                        break;
                    case Actions.Action.M_LEFT:
                        if ( IsPassable(pX - 1, pY) )
                        {
                            pX--;
                            colls = 0;
                        }
                        else
                            colls++;
                        break;
                    case Actions.Action.M_RIGHT:
                        if ( IsPassable(pX + 1, pY) )
                        {
                            pX++;
                            colls = 0;
                        }
                        else
                            colls++;
                        break;
                }

                //if ( colls >= 3 )
                //{
                //    score /= 2;
                //    g.score = score;
                //    break;
                //}

                double newDist = Distance(pX, pY);

                if ( draw )
                {
                    Console.CursorTop = 5;
                    Console.CursorLeft = 40;
                    Console.Write("Distance: {0}     ", Math.Round(newDist, 2));
                    Console.CursorTop = 6;
                    Console.CursorLeft = 40;
                    Console.Write("Action: {0}/{1}    ", actionid, g.actions.Count);
                    Console.CursorTop = 7;
                    Console.CursorLeft = 40;
                    Console.Write("Score: {0}    ", score);
                    Console.CursorTop = 8;
                    Console.CursorLeft = 40;
                    Console.Write("Fitness: {0}            ", Math.Round(g.fitness, 2));
                }

                if ( newDist < lastDist )
                    score++;
                else if ( newDist > lastDist )
                    score -= 2;
                else if ( colls > 1 )
                    score -= 4;

                g.score = score;
                g.dist = newDist;

                if ( draw )
                    DrawMaze(pX, pY);
            }
            */
            #endregion

            g.score = score;
            g.dist = Distance(pX, pY);
            //mazeDrawn = false;
        }

        static double Distance ( int x, int y )
        {
            int xd = 32 - x;
            int yd = 32 - y;
            double dist = Math.Sqrt(xd * xd + yd * yd);
            return dist;
        }

        static bool IsPassable ( int x, int y )
        {
            if ( x < 0 || x >= 32 || y < 0 || y >= 32 )
                return false;

            if ( Maze[x, y] == 0 )
                return false;

            return true;
        }

        static int GetMazePos ( int x, int y )
        {
            if ( x < 0 || x >= 32 || y < 0 || y >= 32 )
                return 255;

            return Maze[x, y];
        }

        static bool mazeDrawn = false;

        static int prevPX = 0;
        static int prevPY = 0;
        static void DrawMaze ( int pX, int pY )
        {
            Console.BackgroundColor = ConsoleColor.Black;
            string sq = "X";
            if ( !mazeDrawn )
            {
                for ( int x = 0; x < 32; x++ )
                {
                    for ( int y =0; y < 32; y++ )
                    {
                        //if ( x == pX && y == pY )
                        //{
                        //    Console.CursorLeft = x;
                        //    Console.CursorTop = y;
                        //    Console.ForegroundColor = ConsoleColor.Red;
                        //    Console.Write("x");
                        //}
                        //else
                        //{

                        Console.CursorLeft = x;
                        Console.CursorTop = y;
                        if ( Maze[x, y] == 0 )
                        {

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(sq);

                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(" ");
                        }

                    }
                }
                mazeDrawn = true;
            }
            else
            {
                Console.SetCursorPosition(prevPX, prevPY);
                Console.Write(" ");
                Console.SetCursorPosition(pX, pY);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("x");
                prevPX = pX;
                prevPY = pY;
            }

        }
    }
}
