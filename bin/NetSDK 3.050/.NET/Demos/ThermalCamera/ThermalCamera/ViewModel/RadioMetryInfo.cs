using IVSDemo.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThermalCamera.ViewModel
{
    class RadioMetryInfo:BindableObject
    {
        private string meterType;

        public string MeterType
        {
            get { return meterType; }
            set
            {
                if (meterType != value)
                {
                    meterType = value;
                    OnPropertyChanged("MeterType");
                }
            }
        }

        private string temperUnit;

        public string TemperUnit
        {
            get { return temperUnit; }
            set
            {
                if (temperUnit != value)
                {
                    temperUnit = value;
                    OnPropertyChanged("TemperUnit");
                }
            }
        }

        private float temperAver;

        public float TemperAver
        {
            get { return temperAver; }
            set
            {
                if (temperAver != value)
                {
                    temperAver = value;
                    OnPropertyChanged("TemperAver");
                }
            }
        }

        private float temperMin;

        public float TemperMin
        {
            get { return temperMin; }
            set
            {
                if (temperMin != value)
                {
                    temperMin = value;
                    OnPropertyChanged("TemperMin");
                }
            }
        }

        private float temperMax;

        public float TemperMax
        {
            get { return temperMax; }
            set
            {
                if (temperMax != value)
                {
                    temperMax = value;
                    OnPropertyChanged("TemperMax");
                }

            }
        }

        private float temperMid;

        public float TemperMid
        {
            get { return temperMid; }
            set
            {
                if (temperMid != value)
                {
                    temperMid = value;
                    OnPropertyChanged("TemperMid");
                }
            }
        }

        private float temperStd;

        public float TemperStd
        {
            get { return temperStd; }
            set
            {
                if (temperStd != value)
                {
                    temperStd = value;
                    OnPropertyChanged("TemperStd");
                }
            }
        }
    }
}
