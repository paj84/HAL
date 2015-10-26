using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Chromosome : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static Random rand = new Random((int)DateTime.Now.ToFileTimeUtc());

        static public double CROSSOVER_RATE = 0.7,
            MUTATION_RATE = 0.001;

        static public int CHROMO_LENGTH = 300,
            GENE_LENGTH = 4,
            MAX_ALLOWABLE_GENERATIONS = 400;


        #region Properties

        private double _fitness;
        public double Fitness
        {
            get { return _fitness; }
            set
            {
                SetField(ref _fitness, value);
            }
        }

        public string Bits;
        static public double RandomNumber
        {
            get { return (double)rand.Next(1000) / 1000; }
        } 

        #endregion

        #region constructors

        public Chromosome(string bits, double fitness)
        {
            this.Bits = bits;
            this.Fitness = fitness;
        }

        public Chromosome() : this(GenerateBits(), 0.0) { }

        #endregion

        public void Mutate()
        {
            StringBuilder sb = new StringBuilder(Bits);
            for (int i = 0; i < this.Bits.Length; i++)
            {
                if (RandomNumber < MUTATION_RATE)
                {
                    if (sb[i] == '1')

                        sb[i] = '0';

                    else

                        sb[i] = '1';
                }
            }
            Bits = sb.ToString();
        }

        public void Crossover(ref Chromosome other)
        {
            //dependent on the crossover rate
            if (RandomNumber < CROSSOVER_RATE)
            {
                //create a random crossover point
                int crossover = (int)(RandomNumber * CHROMO_LENGTH);

                string t1 = this.Bits.Substring(0, crossover) + other.Bits.Substring(crossover, CHROMO_LENGTH - crossover);
                string t2 = other.Bits.Substring(0, crossover) + this.Bits.Substring(crossover, CHROMO_LENGTH - crossover);

                this.Bits = t1; other.Bits = t2;
            }
        }
        public Chromosome Copy()
        {
            return new Chromosome((string)this.Bits.Clone(), this.Fitness);
        }

        static private string GenerateBits()
        {
            var sb = new StringBuilder();
            char bit;

            for (int i = 0; i < CHROMO_LENGTH; i++)
            {
                bit = (RandomNumber > 0.5f) ? '1' : '0';
                sb.Append(bit);
            }

            return sb.ToString();
        }

        #region Events

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}

