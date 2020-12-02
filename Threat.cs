using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    public class Threat
    {
        private int id;
        private string name;
        private string description;
        private string source;
        private string target;
        private bool isNotConfidential;
        private bool isNotComplete;
        private bool isUnavailable;

        public Threat()
        {
        }

        public Threat(int id, string name, string description, string source, string target, bool isNotConfidential, bool isNotComplete, bool isUnavailable)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.source = source;
            this.target = target;
            this.isNotConfidential = isNotConfidential;
            this.isNotComplete = isNotComplete;
            this.isUnavailable = isUnavailable;
        }

        public int Id
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public string Source
        {
            get => source;
            set => source = value;
        }

        public string Target
        {
            get => target;
            set => target = value;
        }

        public bool IsNotConfidential
        {
            get => isNotConfidential;
            set => isNotConfidential = value;
        }

        public bool IsNotComplete
        {
            get => isNotComplete;
            set => isNotComplete = value;
        }

        public bool IsUnavailable
        {
            get => isUnavailable;
            set => isUnavailable = value;
        }

        public bool isTheSame(Threat threat)
        {
            return Id==threat.Id &&
                   Name == threat.Name &&
                   Description == threat.Description &&
                   Source == threat.Source &&
                   Target == threat.Target &&
                   IsNotConfidential == threat.IsNotConfidential &&
                   IsNotComplete == threat.IsNotComplete &&
                   IsUnavailable == threat.IsUnavailable
            ;
        }
    }
}
