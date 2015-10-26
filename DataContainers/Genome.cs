using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Genome : ObservableCollection<Chromosome>
    {
        public event ListChangedEventDelegate ListChanged;
        public delegate void ListChangedEventDelegate();

        private List<Chromosome> _genome = new List<Chromosome>();
        static public int POP_SIZE = 100;

        #region Properties

        private double? _totalFitness;
        public double TotalFitness
        {
            get
            {
                if (_totalFitness == null)
                {
                    _totalFitness = 0.0;
                    foreach (var chromo in this)
                        _totalFitness += chromo.Fitness;
                }
                return _totalFitness.Value;
            }
        } 

        #endregion

        #region Constructs

        public Genome(bool empty)
        {
            this.CollectionChanged += new NotifyCollectionChangedEventHandler(items_CollectionChanged);

            if (!empty)
            {
                for (int i = 0; i < POP_SIZE; i++)
                {
                    Add(new Chromosome());
                }
            }
        }

        public Genome() : this(false) { }

        #endregion

        public Chromosome Roulette()
        {
            //generate a random number between 0 & total fitness count
            double Slice = (double)(Chromosome.RandomNumber * TotalFitness);

            //go through the chromosones adding up the fitness so far
            double FitnessSoFar = 0.0f;
            Chromosome result;

            for (int i = 0; i < this.Count; i++)
            {
                FitnessSoFar += this[i].Fitness;

                //if the fitness so far > random number return the chromo at this point

                if (FitnessSoFar >= Slice)
                {
                    result = this[i].Copy();
                    this.RemoveItem(i);

                    return result;
                }
            }

            return null;
        }

        #region Events

        private void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.OldItems != null)
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);

            if (e.NewItems != null)
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);

            if (e.Action != NotifyCollectionChangedAction.Move)
                _totalFitness = null;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("Fitness"))
            {
                _totalFitness = null;
            }
        }

        #endregion
    }
}
