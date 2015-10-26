using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        private static int BinToDec(string bits)
        {
            int val = 0;
            int value_to_add = 1;

            for (int i = bits.Length; i > 0; i--)
            {


                if (bits[i - 1] == '1')

                    val += value_to_add;

                value_to_add *= 2;

            }//next bit
            
            return val;
        }
        private static double AssignFitness(string bits, double target_value)
        {
            //holds decimal values of gene sequence
            var buffer = new int[(int)(Chromosome.CHROMO_LENGTH / Chromosome.GENE_LENGTH)];

            int num_elements = ParseBits(bits, buffer);

            // ok, we have a buffer filled with valid values of: operator - number - operator - number..
            // now we calculate what this represents.
            double result = 0.0f;

            for (int i = 0; i < num_elements - 1; i += 2)
            {
                switch (buffer[i])
                {
                    case 10:

                        result += buffer[i + 1];
                        break;

                    case 11:

                        result -= buffer[i + 1];
                        break;

                    case 12:

                        result *= buffer[i + 1];
                        break;

                    case 13:

                        result /= buffer[i + 1];
                        break;

                }//end switch
            }

            // Now we calculate the fitness. First check to see if a solution has been found
            // and assign an arbitarily high fitness score if this is so.

            if (result == (double)target_value)

                return 999.0f;

            else

                return 1 / (double)Math.Abs((double)(target_value - result));
            //	return result;
        }
        private static void PrintChromo(string bits)
        {
            //holds decimal values of gene sequence
            var buffer = new int[(int)(Chromosome.CHROMO_LENGTH / Chromosome.GENE_LENGTH)];

            //parse the bit string
            int num_elements = ParseBits(bits, buffer);

            for (int i = 0; i < num_elements; i++)
            {
                PrintGeneSymbol(buffer[i]);
            }

            return;
        }
        private static void PrintGeneSymbol(int val)
        {
            var print = String.Empty;
            if (val < 10)
                print += val + " ";

            else
            {
                switch (val)
                {

                    case 10:

                        print += "+";
                        break;

                    case 11:

                        print += "-";
                        break;

                    case 12:

                        print += "*";
                        break;

                    case 13:

                        print += "/";
                        break;

                }//end switch

                print += " ";
            }
            Console.Write(print);
            return;
        }
        private static int ParseBits(string bits, int[] buffer)
        {

            //counter for buffer position
            int cBuff = 0;

            // step through bits a gene at a time until end and store decimal values
            // of valid operators and numbers. Don't forget we are looking for operator - 
            // number - operator - number and so on... We ignore the unused genes 1111
            // and 1110

            //flag to determine if we are looking for an operator or a number
            bool bOperator = true;

            //storage for decimal value of currently tested gene
            int this_gene = 0;

            for (int i = 0; i < Chromosome.CHROMO_LENGTH; i += Chromosome.GENE_LENGTH)
            {
                //convert the current gene to decimal
                this_gene = BinToDec(bits.Substring(i, Chromosome.GENE_LENGTH));

                //find a gene which represents an operator
                if (bOperator)
                {
                    if ((this_gene < 10) || (this_gene > 13))

                        continue;

                    else
                    {
                        bOperator = false;
                        buffer[cBuff++] = this_gene;
                        continue;
                    }
                }

                //find a gene which represents a number
                else
                {
                    if (this_gene > 9)

                        continue;

                    else
                    {
                        bOperator = true;
                        buffer[cBuff++] = this_gene;
                        continue;
                    }
                }

            }//next gene

            //	now we have to run through buffer to see if a possible divide by zero
            //	is included and delete it. (ie a '/' followed by a '0'). We take an easy
            //	way out here and just change the '/' to a '+'. This will not effect the 
            //	evolution of the solution
            for (int i = 0; i < cBuff; i++)
            {
                if ((buffer[i] == 13) && (buffer[i + 1] == 0))

                    buffer[i] = 10;
            }

            return cBuff;
        }
        private static Chromosome Roulette(double total_fitness, Chromosome[] Population)
        {
            //generate a random number between 0 & total fitness count
            double Slice = (double)(Chromosome.RandomNumber * total_fitness);

            //go through the chromosones adding up the fitness so far
            double FitnessSoFar = 0.0f;

            for (int i = 0; i < Genome.POP_SIZE; i++)
            {
                FitnessSoFar += Population[i].Fitness;

                //if the fitness so far > random number return the chromo at this point
                if (FitnessSoFar >= Slice)

                    return Population[i];
            }

            return null;
        }

        static void Main(string[] args)
        {

            //just loop endlessly until user gets bored :0)
            while (true)
            {
                //storage for our population of chromosomes.
                var Population = new Genome();

                //get a target number from the user.
                double Target;
                Console.WriteLine("Please, enter a number:");
                while(!Double.TryParse(Console.ReadLine(), out Target))
                {
                    Console.WriteLine("You must enter a Number. PLease try again!");
                    Console.WriteLine("Please, enter a number:");
                }

                int GenerationsRequiredToFindASolution = 0;

                //we will set this flag if a solution has been found
                bool bFound = false;

                //enter the main GA loop
                while (!bFound)
                {
                    // test and update the fitness of every chromosome in the 
                    // population
                    for (int i = 0; i < Population.Count; i++)
                    {
                        Population[i].Fitness = AssignFitness(Population[i].Bits, Target);
                    }

                    // check to see if we have found any solutions (fitness will be 999)
                    for (int i = 0; i < Genome.POP_SIZE; i++)
                    {
                        if (Population[i].Fitness >= 999.0)
                        {
                            Console.WriteLine("Solution found in {0} generations!", GenerationsRequiredToFindASolution);

                            PrintChromo(Population[i].Bits);

                            bFound = true;

                            break;
                        }
                    }

                    // create a new population by selecting two parents at a time and creating offspring
                    // by applying crossover and mutation. Do this until the desired number of offspring
                    // have been created. 

                    //define some temporary storage for the new population we are about to create
                    var temp = new Genome(true);

                    //loop until we have created POP_SIZE new chromosomes
                    while (Population.Count > 0)
                    {
                        // we are going to create the new population by grabbing members of the old population
                        // two at a time via roulette wheel selection.
                        var offspring1 = Population.Roulette();
                        var offspring2 = Population.Roulette();

                        //add crossover dependent on the crossover rate
                        offspring1.Crossover(ref offspring2);

                        //now mutate dependent on the mutation rate
                        offspring1.Mutate();
                        offspring2.Mutate();

                        //reset fitness
                        offspring1.Fitness = offspring2.Fitness = 0.0;

                        //add these offspring to the new population. (assigning zero as their
                        //fitness scores)
                        temp.Add(offspring1.Copy());
                        temp.Add(offspring2.Copy());

                    }//end loop

                    //copy temp population into main population array
                    Population = temp;

                    ++GenerationsRequiredToFindASolution;

                    // exit app if no solution found within the maximum allowable number
                    // of generations
                    if (GenerationsRequiredToFindASolution > Chromosome.MAX_ALLOWABLE_GENERATIONS)
                    {
                        Console.Write("No solutions found this run!");

                        bFound = true;
                    }

                }

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

            }//end while
        }
    }
}
